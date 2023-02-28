using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.UI.UIModel;

namespace ZJYFrameWork.UISerializable
{
    public class UIDataLoading : DialogBase
    {
        /// <summary>
        /// 进度条
        /// </summary>
        [SerializeField] private SlicedImageFillHorizon progress = null;

        /// <summary>
        /// 显示具体进度数据
        /// </summary>
        [SerializeField] private Text progressText = null;

        /// <summary>
        /// 提示框
        /// </summary>
        [SerializeField] private Text tipsProgressText = null;

        public override void Init()
        {
            progress.FillAmount = 0f;
            if (tipsProgressText != null)
                tipsProgressText.text = "正在获取新资源，请稍后......";
            if (progressText != null)
            {
                progressText.text = "";
            }

            OnClose();
        }

        private void ShowCanvas()
        {
            progress.FillAmount = 0f;
            progressText.text = $"正在准备资源";

            CanvasKillSequence();
            OnShow();
        }

        public void SetSceneProgress(float size, float max)
        {
            if (size == 0)
            {
                ShowCanvas();
            }

            if (tipsProgressText != null)
                tipsProgressText.text = "正在加载场景，请稍后......";
            float downloadedCounter = size;

            float counter = max - size;
            float counterMax = max;
            float rate = downloadedCounter / counterMax;
            progress.FillAmount = rate;
            progressText.text = string.Empty;
            if (counter == 0)
            {
                HideCanvas();
            }

            if (counter < 0)
            {
                counter = 0;
            }
        }


        public void SetShowCount(float size, float max, string downText)
        {
            if (size == 0)
            {
                ShowCanvas();
            }

            float downloadedCounter = size;

            float counter = max - size;
            float counterMax = max;
            float rate = downloadedCounter / counterMax;
            if (tipsProgressText != null)
                tipsProgressText.text = "正在获取新资源，请稍后......";
            progress.FillAmount = rate;
            progressText.text = $"{downText} {downloadedCounter}/{counterMax}个文件";
            if (counter == 0)
            {
                HideCanvas();
            }

            if (counter < 0)
            {
                counter = 0;
            }
        }

        private Sequence canvasSeq;
        private const float CanvasHideDuration = 0.2f;

        private void HideCanvas()
        {
            progress.FillAmount = 1f;
            progressText.text = string.Empty;

            CanvasKillSequence();
            canvasSeq = DOTween.Sequence();
            canvasSeq.AppendInterval(CanvasHideDuration);
            canvasSeq.AppendCallback(() => { OnClose(); });
        }

        private void CanvasKillSequence()
        {
            if (canvasSeq != null)
            {
                canvasSeq.Kill();
                canvasSeq = null;
            }
        }
    }
}