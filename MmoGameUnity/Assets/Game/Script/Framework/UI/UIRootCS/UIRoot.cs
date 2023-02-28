using UnityEngine;
using UnityEngine.Serialization;

namespace ZJYFrameWork.UISerializable.Framwork.UIRootCS
{
    public class UIRoot : MonoBehaviour
    {
        /// <summary>
        /// 背景panel
        /// </summary>
        [SerializeField] private GameObject bgPanel;

        [FormerlySerializedAs("BgTransformPanel")] [SerializeField]
        private Transform bgTransformPanel;

        [FormerlySerializedAs("UIPanel")] [SerializeField]
        private GameObject uiPanel;

        [FormerlySerializedAs("UITransformPanel")] [SerializeField]
        private Transform uiTransformPanel;

        [FormerlySerializedAs("UITopPanel")] [SerializeField]
        private GameObject uiTopPanel;

        [FormerlySerializedAs("UITopTransformPanel")] [SerializeField]
        private Transform uiTopTransformPanel;

        [FormerlySerializedAs("NoticeCanvasPanel")] [SerializeField]
        private GameObject noticeCanvasPanel;

        [FormerlySerializedAs("NoticeCanvasTransformPanel")] [SerializeField]
        private Transform noticeCanvasTransformPanel;

        [FormerlySerializedAs("ActiviesCanvasTransformPanel")] [SerializeField]
        private Transform activiesCanvasTransformPanel;

        [FormerlySerializedAs("ActiviesNoticeCanvasTransformPanel")] [SerializeField]
        private Transform activiesNoticeCanvasTransformPanel;

        public GameObject GetBgPanel => bgPanel;

        public Transform GetBgTransformPanel => bgTransformPanel;

        public GameObject GetUIPanel => uiPanel;

        public Transform GetUITransformPanel => uiTransformPanel;

        public GameObject GetUITopPanel => uiTopPanel;

        public Transform GetUITopTransformPanel => uiTopTransformPanel;

        public GameObject GetNoticeCanvasPanel => noticeCanvasPanel;

        public Transform GetNoticeCanvasTransformPanel => noticeCanvasTransformPanel;

        public Transform GetActivieseCanvasTransformPanel => activiesCanvasTransformPanel;

        public Transform ActiviesGetNoticeCanvasTransformPanel => activiesNoticeCanvasTransformPanel;

        public void SortOrder()
        {
            bgPanel.GetComponent<Canvas>().sortingOrder = 20;
            uiPanel.GetComponent<Canvas>().sortingOrder = 50;
            activiesCanvasTransformPanel.GetComponent<Canvas>().sortingOrder = 55;
            uiTopPanel.GetComponent<Canvas>().sortingOrder = 80;
            noticeCanvasPanel.GetComponent<Canvas>().sortingOrder = 100;
        }
    }
}