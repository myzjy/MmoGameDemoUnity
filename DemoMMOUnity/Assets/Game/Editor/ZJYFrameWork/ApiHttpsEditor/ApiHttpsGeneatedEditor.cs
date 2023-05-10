using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Serialization;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork;
using ZJYFrameWork.AttributeCustom;
using ZJYFrameWork.Common.Utility;
using JsonConvert = Unity.Plastic.Newtonsoft.Json.JsonConvert;

namespace ZJYFrameWorkEditor.ApiHttpsEditor
{
    //错误
    class Modified : System.Exception
    {
    }

    /// <summary>
    /// 编辑器下生成Api Http文件
    /// </summary>
    public class ApiHttpsGeneratedEditor : UnityEditor.EditorWindow
    {
        // ReSharper disable once InconsistentNaming
        private const string openApiEditor = "Tools/ApiCS/制作api协议文件";
        private const string assetOpenInstructionsEditorStr = "Assets/Tools/ApiCS/Open ApiCS Editor";
        public string csModelName = ".Generated";
        private TextAsset _mSelectInstructions = null;
        private static TextAsset m_lastInstructions = null;
        static Instructions m_parsedInstructions;

        private static HttpsInstructions mHttpClip = null;
        private static string m_clipName;

        private static string clipEnumValue;
        private static Type clipEnumType;
        private Vector2 m_scroll;

        private static string[] sortEnumName;

        //类指令
        private static object sortInstruction;
        private static Type sortEnumType;

        /// <summary>
        /// 打开界面
        /// </summary>
        [MenuItem(openApiEditor)]
        public static void OpenWindows()
        {
            GetWindow(typeof(ApiHttpsGeneratedEditor));
        }

        public void ImportSelectionWindows()
        {
            _mSelectInstructions = Selection.activeObject as TextAsset;
        }

        [MenuItem(assetOpenInstructionsEditorStr, false)]
        static void OpenAndImportSelection()
        {
        }

        public static bool ValidImportObject(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is TextAsset))
            {
                return false;
            }

            string path = AssetDatabase.GetAssetPath(obj);
            if (!path.EndsWith(".json"))
            {
                return false;
            }

            if (!path.Contains("Game/AssetBundle/Tutorial"))
            {
                return false;
            }

            return true;
        }


        string[] m_langKeys;
        string[] m_langValues;

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                _mSelectInstructions =
                    EditorGUILayout.ObjectField(_mSelectInstructions, typeof(TextAsset), false) as TextAsset;

                GUI.enabled = _mSelectInstructions != null;
                if (GUILayout.Button("Reload", GUILayout.ExpandWidth(false)))
                {
                    if (EditorUtility.DisplayDialog("Reload", "保存刷新", "OK", "Cancel"))
                    {
                        m_lastInstructions = null;
                    }
                }

                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();

            if (_mSelectInstructions != m_lastInstructions)
            {
                m_lastInstructions = _mSelectInstructions;
                mHttpClip = null;
                clipEnumValue = "";
                clipEnumType = null;
                try
                {
                    if (_mSelectInstructions != null)
                    {
                        if (m_lastInstructions != null)
                            m_parsedInstructions = //JsonConvert.DeserializeObject<Instructions>(m_lastInstructions.text);
                                Serializer.jsonSerializer.Deserialize<Instructions>(m_lastInstructions.text);
                    }
                    else
                    {
                        m_parsedInstructions = null;
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                    m_parsedInstructions = null;
                }
            }

            if (m_parsedInstructions != null)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Save"))
                {
                    Save();
                }

                EditorGUILayout.EndHorizontal();
                try
                {
                    m_scroll = EditorGUILayout.BeginScrollView(m_scroll);
                    RenderInstructions(m_parsedInstructions);
                }
                catch (Modified)
                {
                    Repaint();
                }
                finally
                {
                    EditorGUILayout.EndScrollView();
                }
            }
        }

        void RenderInstructions(Instructions instructions)
        {
            Color bgColor = GUI.backgroundColor;
            RenderInsert(null);
            for (int j = 0; j < instructions.InstructionsList.Count; j++)
            {
                var i = instructions.InstructionsList[j];
                GUI.backgroundColor = bgColor;
                try
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                        {
                            instructions.InstructionsList.Remove(i);
                            throw new Modified();
                        }

                        GUILayout.FlexibleSpace();
                        {
                            var instruction = instructions.InstructionsList.ElementAt(j);
                            using (new EditorGUI.DisabledScope(instruction == null || instruction is NoneInstruction))
                            {
                                if (GUILayout.Button("Copy", GUILayout.ExpandWidth(false)))
                                {
                                    mHttpClip = instruction;
                                    m_clipName = mHttpClip.GetType().ToString();
                                }
                            }

                            using (new EditorGUI.DisabledScope(mHttpClip == null))
                            {
                                if (GUILayout.Button("Paste", GUILayout.ExpandWidth(false)))
                                {
                                    var obj = ClipToNewInstance(m_clipName);
                                    instructions.InstructionsList[j] = (HttpsInstructions)obj;
                                    throw new Modified();
                                }
                            }
                        }
                    }

                    GUI.backgroundColor = i.Equals(mHttpClip) ? Color.red : bgColor;

                    var ret = RenderValue(instructions.InstructionsList[j], i.GetType(), i);
                    if (ret.First)
                    {
                        instructions.InstructionsList[j] = ret.Second as HttpsInstructions;
                        throw new Modified();
                    }
                }
                finally
                {
                    EditorGUILayout.EndVertical();
                }

                GUI.backgroundColor = bgColor;
                RenderInsert(i);
            }
        }

        object ClipToNewInstance(string typeName)
        {
            var t = TypeExtensions.GetType(typeName);
            var obj = System.Activator.CreateInstance(t);
            var fieldInfo = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (System.Reflection.FieldInfo info in fieldInfo)
            {
                switch (info.FieldType.ToString())
                {
                    case "System.String":
                        info.SetValue(obj, (string)info.GetValue(mHttpClip));
                        break;
                    case "System.Boolean":
                        info.SetValue(obj, (bool)info.GetValue(mHttpClip));
                        break;

                    case "System.Int32":
                        info.SetValue(obj, (int)info.GetValue(mHttpClip));
                        break;
                    case "System.UInt32":
                        info.SetValue(obj, (uint)info.GetValue(mHttpClip));
                        break;
                    case "System.Int64":
                        info.SetValue(obj, (long)info.GetValue(mHttpClip));
                        break;
                    case "System.UInt64":
                        info.SetValue(obj, (ulong)info.GetValue(mHttpClip));
                        break;

                    case "System.Single":
                        info.SetValue(obj, (float)info.GetValue(mHttpClip));
                        break;
                    case "UnityEngine.Vector3":
                        info.SetValue(obj, (Vector3)info.GetValue(mHttpClip));
                        break;
                    case "UnityEngine.Vector2":
                        info.SetValue(obj, (Vector2)info.GetValue(mHttpClip));
                        break;
                    case "UnityEngine.Quaternion":
                        info.SetValue(obj, (Quaternion)info.GetValue(mHttpClip));
                        break;
                    case "UnityEngine.Color":
                        info.SetValue(obj, (Color)info.GetValue(mHttpClip));
                        break;
                    case "UnityEngine.Rect":
                        info.SetValue(obj, (Rect)info.GetValue(mHttpClip));
                        break;

                    default:
                    {
                        if (info.FieldType.IsEnum)
                        {
                        }
                        else if (info.FieldType.IsGenericType)
                        {
                        }
                        else if (info.FieldType.IsArray)
                        {
                        }
                        // Embedded Object
                        else
                        {
                        }
                    }
                        break;
                }
            }

            return obj;
        }

        void RenderInsert(HttpsInstructions before)
        {
            if (GUILayout.Button("insert", GUILayout.ExpandWidth(false)))
            {
                var insert = new NoneInstruction();
                if (before == null)
                {
                    m_parsedInstructions.InstructionsList.Insert(0, insert);
                }
                else
                {
                    var idx = m_parsedInstructions.InstructionsList.FindIndex(x => x == before);
                    m_parsedInstructions.InstructionsList.Insert(idx + 1, insert);
                }

                throw new Modified();
            }
        }

        STuple<bool, object> RenderObject(object inst)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                        BindingFlags.FlattenHierarchy;

            foreach (var f in inst.GetType().GetFields(flags))
            {
                var attr = f.GetAttribute<SerializableFieldAttribute>();
                if (attr != null)
                {
                    try
                    {
                        Color bgColor = GUI.backgroundColor;
                        GUI.backgroundColor = Color.gray;
                        EditorGUILayout.BeginVertical(GUI.skin.box);

                        RenderField(inst, f, attr);
                    }
                    finally
                    {
                        EditorGUILayout.EndVertical();
                    }
                }
            }

            return notChanged;
        }

        void RenderField(object inst, FieldInfo f, SerializableFieldAttribute attr)
        {
            try
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(attr.Description == null ? f.Name : attr.Description);
                var ret = RenderValue(inst, f.FieldType, f.GetValue(inst));
                if (ret.First)
                {
                    f.SetValue(inst, ret.Second);
                    throw new Modified();
                }
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }


        static STuple<bool, object> notChanged = STuple.Create<bool, object>(false, null);

        /// <summary>
        /// 测试值 创建值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="originalValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        static STuple<bool, object> Test<T>(T value, object originalValue)
        {
            if (EqualityComparer<T>.Default.Equals(value, (T)originalValue))
            {
                return notChanged;
            }
            else
            {
                return STuple.Create(true, (object)value);
            }
        }

        private STuple<bool, object> RenderList(Type type, IList list)
        {
            if (list == null)
                return STuple.Create(true, Activator.CreateInstance(typeof(List<>).MakeGenericType(type)));
            try
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                for (var i = 0; i < list.Count; i++)
                {
                    var obj = list[i];
                    var ret = RenderValue(list, type, obj);
                    if (!ret.First) continue;
                    list[i] = ret.Second;
                    throw new Modified();
                }

                return notChanged;
            }
            finally
            {
                EditorGUILayout.EndVertical();
            }
        }

        STuple<bool, object> RenderArray(Type type, Array array)
        {
            if (array == null)
            {
                return STuple.Create(true, (object)Array.CreateInstance(type, 0));
            }

            try
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    var obj = array.GetValue(i);
                    var ret = RenderValue(array, type, obj);
                    if (ret.First)
                    {
                        array.SetValue(ret.Second, i);
                        throw new Modified();
                    }
                }

                return notChanged;
            }
            finally
            {
                EditorGUILayout.EndVertical();
            }
        }

        public static List<Type> subClasses = SubClassTable.GetClassIE(typeof(InstructionType)).ToList();

        public   static string[] typeSelections = subClasses.Select(x =>
        {
            var attr = x.GetAttribute<DescribeAttribute>();
            if (attr != null)
            {
                return attr.Description;
            }
            else
            {
                return x.ToString();
            }
        }).ToArray();


        public STuple<bool, object> RenderInstruction(HttpsInstructions inst)
        {
            var idx = subClasses.FindIndex(x => x == inst.GetType());
            var val = EditorGUILayout.Popup(idx, typeSelections);
            return val != idx ? STuple.Create(true, (object)Activator.CreateInstance(subClasses[val])) : notChanged;
        }

        STuple<bool, object> RenderValue(object inst, Type type, object value)
        {
            // GUILayout.Box();
            if (type == typeof(int))
            {
                return Test(EditorGUILayout.IntField((int)value), value);
            }

            if (type == typeof(float))
            {
                return Test(EditorGUILayout.FloatField((float)value), value);
            }

            if (type == typeof(string))
            {
                return value == null
                    ? STuple.Create(true, (object)"")
                    : Test(GUILayout.TextArea(value as string), value);
            }

            if (type.IsEnum)
            {
                var ret = notChanged;
                using (new EditorGUILayout.VerticalScope())
                {
                    var sortTarget = sortInstruction == inst && sortEnumType == value.GetType();

                    if (!sortTarget)
                    {
                        ret = Test(EditorGUILayout.EnumPopup((Enum)value), value);
                    }
                    else
                    {
                        var selectedIndex = Array.IndexOf(sortEnumName,
                            TextSplitUpperColmuns(value.ToString(), SingleColmunsTarget));
                        var index = EditorGUILayout.Popup(selectedIndex, sortEnumName);
                        if (index != selectedIndex)
                        {
                            ret.First = true;
                            ret.Second = Enum.Parse(value.GetType(), sortEnumName[index].Replace("/", ""));
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        // Copy
                        if (GUILayout.Button("Copy", GUILayout.ExpandWidth(false)))
                        {
                            var clipValue = ret.Second != null ? ret.Second : value;
                            clipEnumValue = ((Enum)clipValue).ToString();
                            clipEnumType = clipValue.GetType();
                        }

                        // Paste
                        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(clipEnumValue) ||
                                                           value.GetType() != clipEnumType))
                        {
                            if (GUILayout.Button("Paste", GUILayout.ExpandWidth(false)))
                            {
                                ret.Second = Enum.Parse(clipEnumType, clipEnumValue);
                                ret.First = value != ret.Second;
                            }
                        }

                        bool toggle = GUILayout.Toggle(sortTarget, "Sort", GUI.skin.button,
                            GUILayout.ExpandWidth(false));
                        if (toggle != sortTarget)
                        {
                            sortInstruction = sortInstruction != inst ? inst : null;
                            sortEnumType = sortInstruction != null ? value.GetType() : null;

                            if (sortInstruction != null)
                            {
                                sortEnumName = Enum.GetNames(value.GetType());
                                Array.Sort(sortEnumName);

                                // スラッシュで階層を切る仕込み
                                for (int i = 0; i < sortEnumName.Length; ++i)
                                {
                                    sortEnumName[i] = TextSplitUpperColmuns(sortEnumName[i], SingleColmunsTarget);
                                }
                            }
                            else
                            {
                                sortEnumName = null;
                            }
                        }
                    }
                }

                return ret;
            }

            if (type == typeof(bool))
            {
                return Test(EditorGUILayout.Toggle((bool)value), value);
            }

            if (type == typeof(Color))
            {
                return Test(EditorGUILayout.ColorField((Color)value), value);
            }

            if (type == typeof(Vector3))
            {
                return Test(EditorGUILayout.Vector3Field("vec3", (Vector3)value), value);
            }

            if (type == typeof(Vector2))
            {
                return Test(EditorGUILayout.Vector2Field("vec2", (Vector2)value), value);
            }

            if (type.IsArray)
            {
                return RenderArray(type.GetElementType(), value as Array);
            }

            if (value is IList)
            {
                return RenderList(type.GetGenericArguments()[0], value as IList);
            }

            if (value is HttpsInstructions)
            {
                try
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    var ret = RenderInstruction((HttpsInstructions)value);
                    if (ret.First) return ret;
                    return RenderObject(value);
                }
                finally
                {
                    EditorGUILayout.EndVertical();
                }
            }

            if (value == null)
            {
                return STuple.Create(true, Activator.CreateInstance(type));
            }

            try
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                return RenderObject(value);
            }
            finally
            {
                EditorGUILayout.EndVertical();
            }
        }

        string[] SingleColmunsTarget =
        {
            "Camera", "Cosmic", "Footer", "Header", "Ingame", "Lottery", "Menu", "Parade", "Street",
        };

        string TextSplitUpperColmuns(string text, string[] singleColmunsTarget, int firstColmunsCount = 3,
            bool splitBackward = false)
        {
            if (singleColmunsTarget != null)
            {
                foreach (var single in singleColmunsTarget)
                {
                    if (!text.StartsWith(single))
                    {
                        continue;
                    }

                    return text.Insert(single.Length, "/");
                }
            }

            if (firstColmunsCount <= 0)
            {
                return text;
            }

            string tmp = "";
            int upperCount = 0;
            int splitColmunsCount = firstColmunsCount;

            foreach (var n in text)
            {
                if (char.IsUpper(n))
                {
                    ++upperCount;
                    if (upperCount >= splitColmunsCount)
                    {
                        splitColmunsCount = 0;
                        upperCount = 0;
                        tmp += "/";

                        if (!splitBackward)
                        {
                            tmp += text.Remove(0, tmp.Replace("/", "").Length);
                            break;
                        }
                        else
                        {
                            string[] elem = System.Text.RegularExpressions.Regex.Split(text,
                                @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[a-z])(?=[A-Z])");
                            tmp += string.Join("/", elem);
                            break;
                        }
                    }
                }

                tmp += n;
            }

            return tmp;
        }

        public void Save()
        {
            var path = AssetDatabase.GetAssetPath(_mSelectInstructions);
            File.WriteAllText(path, Serializer.jsonSerializer.Serialize(m_parsedInstructions));


            HttpInstructionsImporter.ProcessFile(path);
            AssetDatabase.Refresh();
        }
    }
}