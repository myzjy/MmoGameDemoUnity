using ZJYFrameWork.Module.UICommon;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UISerializable
{
    public class UIOperationalInterfacePanelView:UIBaseView<OperationalInterfacePanelView>
    {
        public override void OnInit()
        {
            viewPanel.Grid.OnItemShow = OnItemOperationalGrid;
            viewPanel.Grid.itemCount = 0;
            viewPanel.Grid.RefrashItemGrid();
        }
        

        public override void OnShow()
        {
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