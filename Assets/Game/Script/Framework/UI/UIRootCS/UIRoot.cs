using UnityEngine;

namespace ZJYFrameWork.UISerializable.Framwork.UIRootCS
{
    public class UIRoot : MonoBehaviour
    {
        /// <summary>
        /// 背景panel
        /// </summary>
        [SerializeField] private GameObject bgPanel;
        public GameObject GetBgPanel => bgPanel;

        [SerializeField] private Transform BgTransformPanel;
        public Transform GetBgTransformPanel => BgTransformPanel;
        
        [SerializeField] private GameObject UIPanel;
        public GameObject GetUIPanel => UIPanel;

        [SerializeField] private Transform UITransformPanel;
        public Transform GetUITransformPanel => UITransformPanel;
        
        [SerializeField] private GameObject UITopPanel;
        public GameObject GetUITopPanel => UITopPanel;

        [SerializeField] private Transform UITopTransformPanel;
        public Transform GetUITopTransformPanel => UITopTransformPanel;
        
        [SerializeField] private GameObject NoticeCanvasPanel;
        public GameObject GetNoticeCanvasPanel => NoticeCanvasPanel;

        [SerializeField] private Transform NoticeCanvasTransformPanel;
        public Transform GetNoticeCanvasTransformPanel => NoticeCanvasTransformPanel;
        [SerializeField] private Transform ActiviesCanvasTransformPanel;
        public Transform GetActivieseCanvasTransformPanel => ActiviesCanvasTransformPanel;
        [SerializeField] private Transform ActiviesNoticeCanvasTransformPanel;
        public Transform ActiviesGetNoticeCanvasTransformPanel => ActiviesNoticeCanvasTransformPanel;
        public void SortOrder()
        {
            bgPanel.GetComponent<Canvas>().sortingOrder = 20;
            UIPanel.GetComponent<Canvas>().sortingOrder = 50;
            ActiviesCanvasTransformPanel.GetComponent<Canvas>().sortingOrder = 55;
            UITopPanel.GetComponent<Canvas>().sortingOrder = 80;
            NoticeCanvasPanel.GetComponent<Canvas>().sortingOrder = 100;
        }
    }
}