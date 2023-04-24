using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable.Common
{
    /// <summary>
    /// 面板基础
    /// </summary>
    public class DialogBase : UIView
    {
        [SerializeField] protected Button backgroundCloseButton;

        protected override void Awake()
        {
            base.Awake();
         

            Active(false);
            Init();
        }


        public virtual void Init()
        {
        }

        public void Active(bool active)
        {
            if (active)
            {
                OnShow();
            }
            else
            {
                OnClose();
            }
        }

        public bool Open(Action onComplete = null)
        {
            if (GetSelfObjCanvasGroup == null)
            {
                return false;
            }

            OnOpen();
            Active(true);
            StartCoroutine(OpenRequest(onComplete));

            return true;
        }

        public virtual void OnOpen()
        {
            
        }

        public virtual void OpenAction(Action onComplete)
        {
            if (onComplete != null)
            {
                onComplete.Invoke();
            }
        }

    
        public void Close(Action onComplete = null, bool unload = false)
        {
            OnClose();
            CloseAction(() =>
            {
                if (unload)
                {
                    Resources.UnloadUnusedAssets();
                    System.GC.Collect();
                }

                Active(false);
                if (onComplete != null)
                {
                    onComplete();
                }
            });
        }

        public virtual void CloseAction(Action onComplete)
        {
            if (onComplete != null)
            {
                onComplete();
            }
        }

        public void OnDestroyAction()
        {
        }

        public void EscapeClose()
        {
        }

        protected IEnumerator OpenRequest(System.Action onComplete)
        {
            yield return OnOpenRequest();
            OpenAction(() => { onComplete?.Invoke(); });
            yield break;
        }

        protected virtual IEnumerator OnOpenRequest()
        {
            yield break;
        }
    }
}