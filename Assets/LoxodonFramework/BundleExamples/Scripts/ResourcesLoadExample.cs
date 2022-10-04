#if ASSET_BUNDLE_DEVELOP_EDITOR

using UnityEngine;

using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Contexts;

namespace ZJYFrameWork.Examples.Bundle
{

    public class ResourcesLoadExample : MonoBehaviour
    {
        void Start()
        {

            /* Loads a text. Path:Resources/test.txt */
            ApplicationContext context = Context.GetApplicationContext();
            IResources resources = context.GetService<IResources>();
            TextAsset text = resources.InResources().LoadAsset<TextAsset>("test.txt");
            Debug.Log(text.text);

            // or

            LocalResources localResources = new LocalResources();
            text = localResources.LoadAsset<TextAsset>("test.txt");
            Debug.Log(text.text);
        }


    }
}
#endif