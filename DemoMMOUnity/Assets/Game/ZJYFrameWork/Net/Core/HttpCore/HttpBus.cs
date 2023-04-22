using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.Spring.Utils;
using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.Net.Core.HttpCore
{
    public abstract class HttpBus
    {
        /// <summary>
        /// 扫描注解
        /// </summary>
        public static void Scan()
        {
            var allBeans = SpringContext.GetAllBeans();
            foreach (var bean in allBeans)
            {
            }
        }

        private static void RegisterHttpReceiver(object bean)
        {
            var classType = bean.GetType();
            if (!classType.IsDefined(typeof(RequestMappingAttribute), false))
            {
                return;
            }

            var custom = classType.GetCustomAttributesData();
            foreach (var i in custom)
            {
                var type = i.AttributeType;
                if (!type.Name.Equals("RequestMappingAttribute"))
                {
                    continue;
                }

                //
                var constructorArguments = i.ConstructorArguments;
            }

            //这个类是否被 RequestMapping 表示
            var methods = AssemblyUtils.GetMethodsByAnnoInPOJOClass(classType, typeof(RequestMappingAttribute));
            foreach (var ite in methods)
            {
                Debug.Log(ite);
            }
        }
    }
}