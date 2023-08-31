using GameUtil;
using UnityEngine.UI;
using ZJYFrameWork.Module.UICommon;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UISerializable
{
    public class UIOperationalInterfacePanelView : UIBaseView<OperationalInterfacePanelView>
    {
        public override void OnInit()
        {
            //设置保存
            SpringContext.GetBean<UIOperationalInterfaceController>().SetUIView(this);
            viewPanel.Grid.OnItemShow = OnItemOperationalGrid;
            viewPanel.Grid.itemCount = 0;
            viewPanel.Grid.RefrashItemGrid();
            viewPanel.ReturnButton.SetListener(() =>
            {
                viewPanel.Grid.itemCount = 0;
                viewPanel.Grid.RefrashItemGrid();
                OnHide();
            });
            OnShow();
        }


        public override void OnShow()
        {
            var list = SpringContext.GetBean<ServerDataManager>().PuzzleConfigList;
            viewPanel.Grid.itemCount = list.Count;
            //刷新
            viewPanel.Grid.RefrashItemGrid();
        }

        private void OnItemOperationalGrid(GridItem item)
        {
            item.itemData.gameObject.SetActive(true);
            var list = SpringContext.GetBean<ServerDataManager>().PuzzleConfigList;

            var data = list[item.Index];
            var objectKey = item.itemData;
            var mapGuideNameText = objectKey.GetObjType<Text>("MapGuideName_Text");
            var MapGuideButton = objectKey.GetObjType<Button>("MapGuideButton");
            mapGuideNameText.text = data.puzzleName;
            MapGuideButton.SetListener(() =>
            {
                
            });
        }

        public override void OnHide()
        {
            base.OnHide();
        }
    }
}