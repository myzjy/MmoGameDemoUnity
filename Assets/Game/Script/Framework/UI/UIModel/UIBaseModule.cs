using System;
using System.Collections;
using Framework.AssetBundles.Utilty;
using ZJYFrameWork.Game.Framwork;
using ZJYFrameWork.UISerializable.Framwork.UIRootCS;
using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.AssetBundles;
using ZJYFrameWork.AssetBundles.Model;
using ZJYFrameWork.AssetBundles.Model.Callback;
using ZJYFrameWork.ObjectPool;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Manager;
using Object = UnityEngine.Object;

namespace ZJYFrameWork.UISerializable
{
    public abstract class UIBaseModule<TuiView, TuiPanelView> : UIModelInterface
        where TuiView : UIBaseView<TuiPanelView>, new()
        where TuiPanelView : UIViewInterface, new()
    {
        public TuiView selfView;
        private readonly LoadAssetCallbacks _loadAssetCallbacks;

        public UIBaseModule()
        {
            selfView = new TuiView();
            _loadAssetCallbacks =
                new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback, null, null);
        }

        /// <summary>
        /// 预制体 UI名
        /// </summary>
        /// <returns></returns>
        public abstract string PrefabName();


        public int InstanceID { get; protected set; }

        public virtual UICanvasType GetCanvasType()
        {
            return UICanvasType.None;
        }

        public virtual UISortType GetSortType()
        {
            return UISortType.First;
        }

        public virtual string[] Notification()
        {
            return Array.Empty<string>();
        }

        public virtual void NotificationHandler(UINotification _eventNotification)
        {
        }

        public UIView GetUIView()
        {
            return selfView?.GetSelfUIView;
        }

        public void Refresh()
        {
        }

        public bool IsActive
        {
            get
            {
                if (selfView != null)
                {
                    if (selfView.GetSelfUIView.GetSelfObjCanvasGroup)
                    {
                        var isActive = selfView.GetSelfUIView.GetSelfObjCanvasGroup.alpha > 0;
                        return isActive;
                    }
                    else
                    {
                        // #if UNITY_EDITOR
                        //                         Debug.LogError(
                        //                             $"UIView组件 缺少{typeof(Canvas)},缺少{typeof(GraphicRaycaster)},缺少{typeof(CanvasGroup)},");
                        // #endif
                    }

                    return false;
                }

                return false;
            }
        }

        private bool isResuse = false;

        protected void InstanceOrReuse(Action action = null)
        {
            if (isResuse)
            {
                if (InstanceID == 0) return;
                //从
                ReUse();
            }
            else
            {
                InstancePrefab();
                isResuse = true;
            }
        }

        //复用
        private void ReUse()
        {
            selfView.ReUse();
            switch (GetSortType())
            {
                case UISortType.First:
                {
                    selfView.GetSelfUIView.GetTransform.SetAsFirstSibling();
                }
                    break;
                case UISortType.Last:
                {
                    selfView.GetSelfUIView.GetTransform.SetAsLastSibling();
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        GameObject InstantiateGameObject(GameObject prefab, Action<GameObject> action)
        {
            UIRoot ROOT =SpringContext.GetBean<UIComponent>().GetRoot;
            var parent = GetPanelUIRoot(GetCanvasType());
            GameObject go = Object.Instantiate(prefab, parent.transform, true);
            var rectTransform = go.GetComponent<RectTransform>();
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;
            action.Invoke(go);
            return go;
        }


        private void InstancePrefab()
        {
            SpringContext.GetBean<AssetBundleManager>().LoadAsset(PrefabName(), _loadAssetCallbacks);
        }

        private Transform GetPanelUIRoot(UICanvasType _canvas)
        {
            UIRoot ROOT = SpringContext.GetBean<UIComponent>().GetRoot;
            switch (_canvas)
            {
                case UICanvasType.BG:
                {
                    return ROOT.GetBgTransformPanel;
                }
                case UICanvasType.UI:
                {
                    return ROOT.GetUITransformPanel;
                }
                case UICanvasType.TOP:
                {
                    return ROOT.GetUITopTransformPanel;
                }
                case UICanvasType.NOTICE:
                {
                    return ROOT.GetNoticeCanvasTransformPanel;
                }
                case UICanvasType.LOADING:

                    break;
                case UICanvasType.None:

                    break;
                case UICanvasType.ActiviesUI:
                {
                    return ROOT.GetActivieseCanvasTransformPanel;
                }
                    break;

                default:
                    break;
            }

            return null;
        }

// GameObject
        private void LoadAssetSuccessCallback(string assetName, UnityEngine.Object asset, float duration,
            object userData)
        {
            // ObjectBase objectBase =(ObjectBase) userData;
            var obj = asset as GameObject;
            InstantiateGameObject(obj, res =>
            {
                var rtf = res.GetComponent<RectTransform>();
                var UIView = res.GetComponent<UIView>();
                if (rtf)
                {
                    rtf.offsetMin = Vector2.zero;
                    rtf.offsetMax = Vector2.one;
                }

                switch (GetSortType())
                {
                    case UISortType.First:
                    {
                        res.transform.SetAsFirstSibling();
                    }
                        break;
                    case UISortType.Last:
                    {
                        res.transform.SetAsLastSibling();
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                InstanceID = res.GetInstanceID();
                UIView.GetTransform.localScale = Vector3.one;
                UIView.GetTransform.localPosition = Vector3.zero;
                selfView.SetUIView(UIView);
                // 默认调用
                selfView.OnInit();
            });
        }

        private void LoadAssetFailureCallback(string soundAssetName, LoadResourceStatus status, string errorMessage,
            object userData)
        {
            Debug.LogError(errorMessage);
        }
    }
}