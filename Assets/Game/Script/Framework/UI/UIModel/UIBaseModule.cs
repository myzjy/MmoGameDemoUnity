using System;
using System.Collections;
using ZJYFrameWork.Game.Framwork;
using ZJYFrameWork.UISerializable.Framwork.UIRootCS;
using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.UISerializable.Manager;
using Object = UnityEngine.Object;

namespace ZJYFrameWork.UISerializable.UIModel
{
    public class UIBaseModule<T_UI_View, T_UI_PanelView> : UIModelInterface
        where T_UI_View : UIBaseView<T_UI_PanelView>, new()
        where T_UI_PanelView : UIViewInterface, new()
    {
        public T_UI_View selfView;

        public UIBaseModule()
        {
            selfView = new T_UI_View();
        }

        /// <summary>
        /// 预制体 UI名
        /// </summary>
        /// <returns></returns>
        protected virtual string PrefabName()
        {
            //这个地方放预制体名字应该是由服务器传递数据过来我们读
            return string.Empty;
        }


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
            return new string[] { };
        }

        public virtual void NotificationHandler(UINotification _eventNotification)
        {
        }

        public UIView GetUIView()
        {
            return selfView?.GetSelfUIView;
        }

        public bool IsActive
        {
            get
            {
                if (selfView != null)
                {
                    // if (selfView.GetSelfUIView.GetSelfObject != null)
                    // {
                    //     return selfView.GetSelfUIView.GetSelfObject && selfView.GetSelfUIView.GetSelfObject.activeSelf;
                    // }
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

        protected IEnumerator InstanceOrReuse(Action action = null)
        {
            if (isResuse)
            {
                if (InstanceID == 0) yield break;
                //从
                ReUse();
            }
            else
            {
                yield return InstancePrefab();
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
            UIRoot ROOT = UIManager.Instance.GetRoot();
            var parent = GetPanelUIRoot(GetCanvasType());
            var start = DateTime.Now;
            GameObject go = Object.Instantiate(prefab, parent.transform, true);
            Debug.Log($"Instantiate use {(DateTime.Now - start).Milliseconds}ms");
            var rectTransform = go.GetComponent<RectTransform>();
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;
            action.Invoke(go);
            return go;
        }

        private IEnumerator InstancePrefab()
        {
            var start = DateTime.Now;
            yield break;
            // var loader = AssetBundleManager.Instance.LoadAssetBundleAsync(PrefabName());
            // var loader = AssetBundleManager.Instance.LoadAssetAsync(PrefabName(), typeof(GameObject));
            // IAddressableLoadAssetSystem _addressable = CommonManager.Instance.GetSystem<IAddressableLoadAssetSystem>();
            // yield return loader;


            // var obj = loader.asset as GameObject;
// #if UNITY_EDITOR
            // UnityEngine.Debug.Log($"{PrefabName()}读取成功！！{loader.asset}");
// #endif
            // var go = InstantiateGameObject(obj,
            // res =>
            // {
            // var rtf = res.GetComponent<RectTransform>();
            // var UIView = res.GetComponent<UIView>();
            // if (rtf)
            // {
            // rtf.offsetMin = Vector2.zero;
            // rtf.offsetMax = Vector2.one;
            // }

            // switch (GetSortType())
            // {
            // case UISortType.First:
            // {
            // res.transform.SetAsFirstSibling();
            // }
            // break;
            // case UISortType.Last:
            // {
            // res.transform.SetAsLastSibling();
            // }
            // break;
            // default:
            // throw new ArgumentOutOfRangeException();
            // }

            // InstanceID = res.GetInstanceID();
            // UIView.GetTransform.localScale = Vector3.one;
            // UIView.GetTransform.localPosition = Vector3.zero;
            // selfView.SetUIView(UIView);
            //// 默认调用
            // selfView.OnHide();
            // selfView.OnInit();
            // });
        }

        private Transform GetPanelUIRoot(UICanvasType _canvas)
        {
            UIRoot ROOT = UIManager.Instance.GetRoot();
            switch (_canvas)
            {
                case UICanvasType.BG:
                {
                    return ROOT.GetBgTransformPanel;
                }
                    break;
                case UICanvasType.UI:
                {
                    return ROOT.GetUITransformPanel;
                }
                    break;
                case UICanvasType.TOP:
                {
                    return ROOT.GetUITopTransformPanel;
                }
                    break;
                case UICanvasType.NOTICE:
                {
                    return ROOT.GetNoticeCanvasTransformPanel;
                }
                    break;
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
    }
}