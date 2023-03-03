using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UISerializable
{
    /// <summary>
    /// loadingView 
    /// </summary>
    public class LoadingUIView : UIBaseView<LoadingPanelView>
    {
        private float NowProgressNum;

        public override void OnInit()
        {
            OnShow();
            viewPanel.LoadingController.Build(this);
        }

        public override void OnShow()
        {
            GetSelfUIView.OnShow();

            //重置 第一次场景加载登录 之后 进度80% 剩余的是请求数据，完成之后
            viewPanel.leftSlider_Slider.value = 0f;
            viewPanel.rightSlider_Slider.value = 0f;
        }

        public void ResetProgressBar()
        {
            viewPanel.leftSlider_Slider.value = 0f;
            viewPanel.rightSlider_Slider.value = 0f;
        }


        public void SetNowProgressNum(float nums)
        {
            NowProgressNum = nums;
            var progress = NowProgressNum;
            viewPanel.leftSlider_Slider.value = progress;
            viewPanel.rightSlider_Slider.value = progress;
            viewPanel.progressNum_Text.text = $"{NowProgressNum * 100}%";
        }

        /// <summary>
        /// 在登录之后，场景跳转完成之后，开始请求基础数据
        /// 1,
        /// </summary>
        /// <param name="progressNum"></param>
        public void RefreshProgressLoginLater(float progressNum)
        {
            NowProgressNum += progressNum;
            var progress = NowProgressNum;
            viewPanel.leftSlider_Slider.value = progress;
            viewPanel.rightSlider_Slider.value = progress;
            viewPanel.progressNum_Text.text = $"{NowProgressNum * 100}%";
        }
    }
}