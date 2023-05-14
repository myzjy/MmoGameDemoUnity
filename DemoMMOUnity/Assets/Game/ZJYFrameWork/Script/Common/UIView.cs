using System;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace ZJYFrameWork.UISerializable
{
    [LuaCallCSharp]
    public class UIView : UISerializableKeyObject
    {
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
            // mSelfObj.SetActive(false);
            // MObjCanvas.enabled = false;
            // MObjCanvasGroup.alpha = 0;
            // MGraphicRaycaster.enabled = false;
        }
    }
}