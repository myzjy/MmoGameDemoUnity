using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork;
using ZJYFrameWork.AttributeCustom;
using ZJYFrameWork.Common.Utility;

namespace GameEditor.ApiHttpsEditor
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

        #region Editor GUI

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            #region 中间处理层

            {
                //转换选着
                _mSelectInstructions =
                    EditorGUILayout.ObjectField(_mSelectInstructions, typeof(TextAsset), false) as TextAsset;
                GUI.enabled = _mSelectInstructions != null;
                if (GUILayout.Button("Reload", GUILayout.ExpandWidth(false)))
                {
                    if (EditorUtility.DisplayDialog("Reload", "进行刷新", "OK", "Cancel"))
                    {
                        m_lastInstructions = null;
                    }
                }

                GUI.enabled = true;
            }

            #endregion

            EditorGUILayout.EndHorizontal();
            if (_mSelectInstructions != m_lastInstructions)
            {
                m_lastInstructions = _mSelectInstructions;
                mHttpClip = null;
                clipEnumValue = "";
                clipEnumType = null;
                try
                {
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    m_parsedInstructions = null;
                }
            }

            if (m_parsedInstructions != null)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Save"))
                {
                    // Save();
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
            //背景颜色
            Color bgColor = GUI.backgroundColor;
            RenderInsert(null);
            for (int i = 0; i < instructions.InstructionsList.Count; i++)
            {
                var item = instructions.InstructionsList[i];
                GUI.backgroundColor = bgColor;
                try
                {
                    #region Box

                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    #region 当前格子框

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                        {
                            instructions.InstructionsList.Remove(item);
                            throw new Modified();
                        }

                        GUILayout.FlexibleSpace();
                        {
                            var instruction = instructions.InstructionsList.ElementAt(i);
                            using (new EditorGUI.DisabledScope(instruction == null || instruction is NoneInstruction))
                            {
                                if (GUILayout.Button("Copy", GUILayout.ExpandWidth(false)))
                                {
                                    mHttpClip = instruction;
                                    if (mHttpClip != null) m_clipName = mHttpClip.GetType().ToString();
                                }
                            }

                            using (new EditorGUI.DisabledScope(mHttpClip == null))
                            {
                                if (GUILayout.Button("Paste", GUILayout.ExpandWidth(false)))
                                {
                                    var obj = ClipToNewInstance(m_clipName);
                                    instructions.InstructionsList[i] = (HttpsInstructions)obj;
                                    throw new Modified();
                                }
                            }
                        }
                    }

                    //根据当前片段是否一样进行修改
                    GUI.backgroundColor = item.Equals(mHttpClip) ? Color.red : bgColor;

                    #endregion

                    #endregion
                }
                catch (Exception e)
                {
                }
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

        /// <summary>
        /// 是否有插入属性
        /// </summary>
        /// <param name="before"></param>
        /// <exception cref="Modified"></exception>
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

        #endregion


        private static List<Type> subClasses = SubClassTable.GetClassIE(typeof(InstructionType)).ToList();

        static string[] typeSelections = subClasses.Select(x =>
        {
            var attr = x.GetAttribute<DescribeAttribute>();
            if (attr != null)
            {
                return attr.description;
            }
            else
            {
                return x.ToString();
            }
        }).ToArray();

        STuple<bool, object> RenderValue(object inst, Type type, object value)
        {
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
                STuple<bool, object> ret = notChanged;
                using (new EditorGUILayout.VerticalScope())
                {
                    bool sortTarget = sortInstruction == inst && sortEnumType == value.GetType();

                    if (!sortTarget)
                    {
                        ret = Test(EditorGUILayout.EnumPopup((Enum)value), value);
                    }
                    else
                    {
                        int selectedIndex = Array.IndexOf(sortEnumName,
                            TextSplitUpperColmuns(value.ToString(), SingleColmunsTarget));
                        int index = EditorGUILayout.Popup(selectedIndex, sortEnumName);
                        if (index != selectedIndex)
                        {
                            ret.first = true;
                            ret.second = Enum.Parse(value.GetType(), sortEnumName[index].Replace("/", ""));
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        // Copy
                        if (GUILayout.Button("Copy", GUILayout.ExpandWidth(false)))
                        {
                            var clipValue = ret.second != null ? ret.second : value;
                            clipEnumValue = ((Enum)clipValue).ToString();
                            clipEnumType = clipValue.GetType();
                        }

                        // Paste
                        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(clipEnumValue) ||
                                                           value.GetType() != clipEnumType))
                        {
                            if (GUILayout.Button("Paste", GUILayout.ExpandWidth(false)))
                            {
                                ret.second = Enum.Parse(clipEnumType, clipEnumValue);
                                ret.first = value != ret.second;
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
                    if (ret.first) return ret;
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

        /// <summary>
        /// 没有改变object
        /// </summary>
        static STuple<bool, object> notChanged = STuple.Create<bool, object>(false, null);

        static STuple<bool, object> Test<T>(T value, object originalValue)
        {
            return EqualityComparer<T>.Default.Equals(value, (T)originalValue)
                ? notChanged
                : STuple.Create(true, (object)value);
        }

        /// <summary>
        /// 渲染当前行
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 渲染所制作的指令
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        STuple<bool, object> RenderInstruction(HttpsInstructions inst)
        {
            //根据传递过来的指令或者数据 找出type
            var idx = subClasses.FindIndex(x => x == inst.GetType());
            //创建一个通用的弹出选择字段。
            var val = EditorGUILayout.Popup(idx, typeSelections);
            if (val != idx)
            {
                return STuple.Create(true, (object)Activator.CreateInstance(subClasses[val]));
            }
            else
            {
                return notChanged;
            }
        }

        /// <summary>
        /// 渲染当前field
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="f"></param>
        /// <param name="attr"></param>
        /// <exception cref="Modified"></exception>
        void RenderField(object inst, FieldInfo f, SerializableFieldAttribute attr)
        {
            try
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(attr.description == null ? f.Name : attr.description);
                var ret = RenderValue(inst, f.FieldType, f.GetValue(inst));
                if (ret.first)
                {
                    f.SetValue(inst, ret.second);
                    throw new Modified();
                }
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 渲染列表 开始渲染
        /// </summary>
        /// <param name="type">需要渲染Type类型</param>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="Modified"></exception>
        STuple<bool, object> RenderList(Type type, IList list)
        {
            if (list == null)
                return STuple.Create(true, Activator.CreateInstance(typeof(List<>).MakeGenericType(type)));
            try
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                for (int i = 0; i < list.Count; i++)
                {
                    var obj = list[i];
                    var ret = RenderValue(list, type, obj);
                    if (ret.first)
                    {
                        list[i] = ret.second;
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

        /// <summary>
        /// 渲染数组
        /// </summary>
        /// <param name="type"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        /// <exception cref="Modified"></exception>
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
                    if (ret.first)
                    {
                        array.SetValue(ret.second, i);
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

        string[] SingleColmunsTarget =
        {
            "Camera",
            "Cosmic",
            "Footer",
            "Header",
            "Ingame",
            "Lottery",
            "Menu",
            "Parade",
            "Street",
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

            var tmp = "";
            var upperCount = 0;
            var splitColmunsCount = firstColmunsCount;

            foreach (var n in text)
            {
                if (splitColmunsCount > 0 && char.IsUpper(n))
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
    }
}