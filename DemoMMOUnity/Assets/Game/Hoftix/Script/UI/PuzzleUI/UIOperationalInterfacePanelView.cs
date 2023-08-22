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
        }

        public override void OnHide()
        {
            base.OnHide();
        }
    }
}