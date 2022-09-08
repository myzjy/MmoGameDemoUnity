// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using ZJYFrameWork.AssetBundleLoader;
//
// namespace Framework.AssetBundle.AsyncOperation
// {
//     /// <summary>
//     /// 读取实际资产带文件的类型
//     /// </summary>
//     public abstract class BundleFileAssetBundleLoaderImpl:AssetBundleLoaderImpl
//     {
// #if UNITY_EDITOR
//         public override void OnLoadAsset(Object asset)
//         {
//             var obj = asset as GameObject;
//             if(obj != null)
//             {
//                 ReloadShader(obj);
//             }
//         }
//
//         /// <summary>
//         /// 编辑器的资产带模式时的着色器重新粘贴
//         /// </summary>
//         /// <param name="obj"></param>
//         private void ReloadShader(GameObject obj)
//         {
//             // 重新粘贴所有共享素材的着色器
//             var materialHashSet = new HashSet<Material> ();
//             var renderers = obj.GetComponentsInChildren<Renderer>(true);
//             foreach (var t in renderers)
//             {
//                 materialHashSet.UnionWith(t.sharedMaterials);
//             }
//             foreach (var mat in materialHashSet.Where(mat => mat != null))
//             {
//                 mat.shader = Shader.Find(mat.shader.name);
//             }
//         }
// #endif
//     }
// }