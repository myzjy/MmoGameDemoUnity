using ZJYFrameWork.Constant;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Procedure.Scene
{
    public class SceneValue
    {
        public SceneEnum SceneEnum;
        public string assetName;

        public static SceneValue valueOf(SceneEnum sceneEnum, string assetName)
        {
            SceneValue value = new SceneValue
            {
                assetName = assetName,
                SceneEnum = sceneEnum
            };
            return value;
        }
    }
    [Bean]
    public class ProcedureChangeScene: FsmState<IProcedureFsmManager>
    {
        
    }
}