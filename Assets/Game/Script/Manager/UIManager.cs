using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ZJYFrameWork.Framwork;
using ZJYFrameWork.UISerializable.Framwork.UIRootCS;
using ZJYFrameWork.UISerializable.PanelView;
using ZJYFrameWork.UISerializable.UIModel;
using UnityEngine;

namespace ZJYFrameWork.UISerializable.Manager
{
    public delegate void MessageOpenPanel(string meesage);

    public delegate void MessageClosePanel();

    public class UIManager : Singleton<UIManager>
    {
        //UIRoot
        [SerializeField] private UIRoot Root = null;

        // private static readonly Type typePunRPC = typeof(ModelRPC);
        private Dictionary<Type, IUIView> UIViewDict;

        // private AddressableLoadHistory addressableLoadHistory;
        // public AddressableLoadHistory AddressableLoadHistory => addressableLoadHistory;
        public bool IsInit => isInit;
        private bool isInit = false;

        public MessageOpenPanel OnOpenMessage;
        public MessageClosePanel OnCloseMessage;

        protected override void Init()
        {
            if (Root == null)
            {
                var roots = transform.parent.GetComponentInChildren<UIRoot>();
                Root = roots;
            }

            Root.SortOrder();
            // OnOpenMessage = (message) =>
            // {
            //     MessageData data = new MessageData();
            //     data.titleStr = "通知";
            //     data.messageSt = message;
            //     GetSystem<IUISystemModule>().DispatchEvent(UINotifEnum.SHOW_ALERTMESSAGE_TEXT, data);
            // };
            // OnCloseMessage = () =>
            // {
            //     GetSystem<IUISystemModule>().DispatchEvent(UINotifEnum.CLOSE_ALERTMESSAGE_PAENL);
            // };
            // addressableLoadHistory = new AddressableLoadHistory();
            UIViewDict = new Dictionary<Type, IUIView>();
            UIViewDict.Add(typeof(IUISystemModule), new UISystemManager());
            UIViewDict.Add(typeof(UIModuleSystemController), new UISystemModuleController());
            isInit = true;
            // List<MethodInfo> rpcMethodInfos =SupportClassPun.GetMethods();
        }

        public UIRoot GetRoot()
        {
            return Root;
        }


        public T GetSystem<T>() where T : class, IUIView
        {
            Type _type = typeof(T);
            IUIView view = null;
            if (UIViewDict.ContainsKey(_type))
            {
                view = UIViewDict[_type];
            }

            if (view == null)
            {
                Debug.LogError($"当前查找并没注册{_type}");
                return null;
            }
            else
            {
                return view as T;
            }
        }
    }
}