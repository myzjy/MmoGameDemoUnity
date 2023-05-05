using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;

public static class XLuaGenConfig
{
    [LuaCallCSharp] public static List<Type> LuaCallCSharp = new List<Type>()
    {
        typeof(System.Object),
        typeof(UnityEngine.Object),
        typeof(Vector2),
        typeof(Vector3),
        typeof(Vector4),
        typeof(Quaternion),
        typeof(Color),
        typeof(Ray),
        typeof(Bounds),
        typeof(Ray2D),
        typeof(Time),
        typeof(GameObject),
        typeof(Component),
        typeof(Behaviour),
        typeof(Transform),
        typeof(Resources),
        typeof(TextAsset),
        typeof(Keyframe),
        typeof(AnimationCurve),
        typeof(AnimationClip),
        typeof(MonoBehaviour),
        typeof(ParticleSystem),
        typeof(SkinnedMeshRenderer),
        typeof(Renderer),
        // typeof(WWW),
        typeof(Light),
        typeof(Mathf),
        typeof(System.Collections.Generic.List<int>),
        typeof(Action<string>),
        typeof(InputField),
        typeof(InputField.ContentType),
        typeof(InputField.InputType),
        typeof(InputField.CharacterValidation),
        typeof(InputField.LineType),
        typeof(InputField.SubmitEvent),
        typeof(InputField.OnChangeEvent),
        typeof(TextAnchor),
        typeof(RaycastHit),
        typeof(Touch),
        typeof(TouchPhase),
        typeof(LayerMask),
        typeof(Plane),
        typeof(RectTransform),
        typeof(RectTransform.Axis),
        typeof(RectTransform.Edge),
        typeof(TextMesh),
        typeof(Graphic),
        typeof(UIBehaviour),
        typeof(MaskableGraphic),
        typeof(MaskableGraphic.CullStateChangedEvent),
        typeof(ScrollRect),
        typeof(ScrollRect.MovementType),
        typeof(ScrollRect.ScrollbarVisibility),
        typeof(Image),
        typeof(Image.Type),
        typeof(Image.FillMethod),
        typeof(Image.OriginHorizontal),
        typeof(Image.OriginVertical),
        typeof(Image.Origin90),
        typeof(Image.Origin180),
        typeof(Image.Origin360),
        typeof(Animator),
        // typeof(CircleRawImage),
        typeof(RawImage),
        typeof(Camera),
        typeof(Camera.CameraCallback),
        typeof(Camera.FieldOfViewAxis),
        typeof(Camera.GateFitParameters),
        typeof(Camera.MonoOrStereoscopicEye),
        typeof(Camera.StereoscopicEye),
        typeof(Camera.GateFitMode),
        typeof(Physics),
        // typeof(System.RuntimeType),
        typeof(Input),
        typeof(Sprite),
        typeof(Texture),
        typeof(Texture2D),
        typeof(Texture2D.EXRFlags),
        typeof(Button),
        typeof(KeyCode),
        typeof(Screen),
        typeof(RenderTexture),
        typeof(CinemachineFreeLook),
        typeof(AxisState),
        typeof(UnityEngine.UI.ScrollRect.ScrollRectEvent),
        typeof(UnityEngine.Debug)
    };
    [LuaCallCSharp]
    [CSharpCallLua]
    public static List<Type> CSharpCallLuaUnityMMO
    {
        get
        {
            Type[] types = Assembly.Load("Assembly-CSharp").GetTypes();
            List<Type> list = (from type in types
                where (type.Namespace == "ZJYFrameWork.Execution"
                       || type.Namespace == "ZJYFrameWork.Base.Component"
                       || type.Namespace == "ZJYFrameWork.AssetBundles.Bundles"
                       || type.Namespace == "ZJYFrameWork.Procedure"
                       || type.Namespace == "ZJYFrameWork.UISerializable.Common"
                       || type.Namespace == "ZJYFrameWork.UISerializable"
                       || type.Namespace == "ZJYFrameWork.AssetBundles.AssetBundlesManager"
                       || type.Namespace == "ZJYFrameWork.Base.Model"
                       || type.Namespace == "ZJYFrameWork.Net.Core.Model"
                       || type.Namespace == "BestHTTP"
                       || type.Namespace == "ZJYFrameWork.Module.ServerConfig.Controller"
                       || type.Namespace == "ZJYFrameWork.Hotfix.Module.Login.Controller"
                       || type.Namespace == "ZJYFrameWork.Net.CsProtocol.Buffer"
                       || type.Namespace == "ZJYFrameWork.Net.CsProtocol.Buffer.Protocol"
                       || type.Namespace == "ZJYFrameWork.Setting"
                       || type.Namespace == "ZJYFrameWork.Net"
                       || type.Namespace == "ZJYFrameWork.WebRequest")
                select type).ToList();
            return list;
        }
    }
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharpTextMeshPro = new List<Type>() {
        typeof(TextMeshProUGUI),
        typeof(TMP_InputField),
        typeof(TMP_Text),
    };
    //C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface
    [CSharpCallLua]
    [LuaCallCSharp]
    public static List<Type> CSharpCallLua = new List<Type>() {
        typeof(Action),
        typeof(Func<double, double, double>),
        typeof(Func<long, long>),
        typeof(Func<long, string>),
        typeof(Func<string, long>),
        typeof(Action<string>),
        typeof(Action<double>),
        typeof(Action<bool>),
        typeof(Action<float, float>),
        typeof(UnityEngine.Events.UnityAction),
        typeof(UnityEngine.Events.UnityAction<long>),
        typeof(UnityEngine.Events.UnityAction<long,long>),
        typeof(UnityEngine.Events.UnityAction<string>),
        typeof(UnityEngine.Events.UnityAction<UnityEngine.Vector2>),
        typeof(UnityEngine.Events.UnityEvent<UnityEngine.Vector2>),
        typeof(System.Collections.IEnumerator),
    };
    //黑名单
    [BlackList] public static List<List<string>> BlackList = new List<List<string>>()
    {
        new List<string>() { "System.Xml.XmlNodeList", "ItemOf" },
        new List<string>() { "UnityEngine.WWW", "movie" },
#if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
#endif
        new List<string>() { "UnityEngine.Texture2D", "alphaIsTransparency" },
        new List<string>() { "UnityEngine.Security", "GetChainOfTrustValue" },
        new List<string>() { "UnityEngine.CanvasRenderer", "onRequestRebuild" },
        new List<string>() { "UnityEngine.Light", "areaSize" },
        new List<string>() { "UnityEngine.Light", "shadowRadius" },
        new List<string>() { "UnityEngine.Light", "shadowAngle" },
        new List<string>() { "UnityEngine.Light", "SetLightDirty" },
        new List<string>() { "UnityEngine.Light", "lightmapBakeType" },
        new List<string>() { "UnityEngine.WWW", "MovieTexture" },
        new List<string>() { "UnityEngine.WWW", "GetMovieTexture" },
        new List<string>() { "UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup" },
#if !UNITY_WEBPLAYER
        new List<string>() { "UnityEngine.Application", "ExternalEval" },
#endif
        new List<string>() { "UnityEngine.GameObject", "networkView" }, //4.6.2 not support
        new List<string>() { "UnityEngine.Component", "networkView" }, //4.6.2 not support
        new List<string>()
            { "System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections" },
        new List<string>() { "System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity" },
        new List<string>()
            { "System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections" },
        new List<string>()
            { "System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity" },
        new List<string>()
        {
            "System.IO.DirectoryInfo", "CreateSubdirectory", "System.String",
            "System.Security.AccessControl.DirectorySecurity"
        },
        new List<string>() { "System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity" },
        new List<string>() { "UnityEngine.MonoBehaviour", "runInEditMode" },
    };
#if UNITY_2018_1_OR_NEWER
    [BlackList]
    public static Func<MemberInfo, bool> MethodFilter = (memberInfo) =>
    {
        if (memberInfo.DeclaringType is not { IsGenericType: true } ||
            memberInfo.DeclaringType.GetGenericTypeDefinition() != typeof(Dictionary<,>)) return false;
        switch (memberInfo.MemberType)
        {
            case MemberTypes.Constructor:
            {
                ConstructorInfo constructorInfo = memberInfo as ConstructorInfo;
                if (constructorInfo != null)
                {
                    var parameterInfos = constructorInfo.GetParameters();
                    if (parameterInfos.Length > 0)
                    {
                        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(parameterInfos[0].ParameterType))
                        {
                            return true;
                        }
                    }
                }

                break;
            }
            case MemberTypes.Method:
            {
                var methodInfo = memberInfo as MethodInfo;
                if (methodInfo != null && (methodInfo.Name == "TryAdd" || methodInfo.Name == "Remove" && methodInfo.GetParameters().Length == 2))
                {
                    return true;
                }

                break;
            }
            case MemberTypes.All:
                break;
            case MemberTypes.Custom:
                break;
            case MemberTypes.Event:
                break;
            case MemberTypes.Field:
                break;
            case MemberTypes.NestedType:
                break;
            case MemberTypes.Property:
                break;
            case MemberTypes.TypeInfo:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return false;
    };
#endif
}