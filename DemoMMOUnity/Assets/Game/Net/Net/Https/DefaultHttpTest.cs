using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.WebRequest
{
    [Bean]
    [RequestMapping(path:new string[]{"/si"})]
    public class DefaultHttpTest
    {
        [RequestMapping(path:new []{"/li"},methods:new [] { HttpMethods.Get })]
        public void Set()
        {
            
        }
    }
}