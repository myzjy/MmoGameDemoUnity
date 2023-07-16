using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable.Common
{
    /// <summary>
    /// 面板基础
    /// </summary>
    public class DialogBase : MonoBehaviour
    {
        [SerializeField] protected Button backgroundCloseButton;
        private int instanceID;
        [SerializeField] private GameObject mSelfObj;
        public GameObject GetSelfObject => mSelfObj;
        [SerializeField] private Transform MTransform;
        public Transform GetTransform => MTransform;

        // [SerializeField] private Canvas MObjCanvas;
        // public Canvas GetSelfObjCanvas => MObjCanvas;
        //
        [SerializeField] private CanvasGroup MObjCanvasGroup;
        public CanvasGroup GetSelfObjCanvasGroup => MObjCanvasGroup;
        //
        // [SerializeField] private GraphicRaycaster MGraphicRaycaster;
        // public GraphicRaycaster GetSelfGraphicRaycaster => MGraphicRaycaster;

        protected virtual void Awake()
        {
            instanceID = gameObject.GetInstanceID();
            Active(false);
            Init();
        }

        // private void OnEnable()
        // {
        //     throw new NotImplementedException();
        // }

        private void Reset()
        {
            mSelfObj = gameObject;
            MTransform = transform;
            // MObjCanvas = GetComponent<Canvas>();
            MObjCanvasGroup = GetComponent<CanvasGroup>();
            // MGraphicRaycaster = GetComponent<GraphicRaycaster>();
        }

        public void OnShow()
        {
            MObjCanvasGroup.alpha = 1;
            MObjCanvasGroup.interactable = true;
            MObjCanvasGroup.blocksRaycasts = true;
            
            // mSelfObj.SetActive(true);
            // MObjCanvas.enabled = true;
            // MObjCanvasGroup.alpha = 1;
            // MGraphicRaycaster.enabled = true;
        }

        public void OnClose()
        {
            MObjCanvasGroup.alpha = 0;
            MObjCanvasGroup.interactable = false;
            MObjCanvasGroup.blocksRaycasts = false;
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