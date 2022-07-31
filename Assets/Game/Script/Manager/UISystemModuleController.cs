using System.Collections.Generic;
using ZJYFrameWork.UISerializable.UIModel;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.UISerializable.Manager
{
    public class UISystemModuleController : UIModuleSystemController
    {
        private List<UIModelInterface> modelList = new List<UIModelInterface>();

        protected void RegisterModule(UIModelInterface _model)
        {
            foreach (var item in _model.Notification())
            {
                UIManager.Instance.GetSystem<IUISystemModule>().RegisterEvent(item, _model.NotificationHandler);
            }
        }

        public void Init()
        {
        }
    }
}