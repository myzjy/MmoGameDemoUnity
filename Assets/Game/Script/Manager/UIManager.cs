// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Reflection;
// using ZJYFrameWork.Framwork;
// using ZJYFrameWork.UISerializable.Framwork.UIRootCS;
// using ZJYFrameWork.UISerializable.UIModel;
// using UnityEngine;
//
// namespace ZJYFrameWork.UISerializable.Manager
// {
//     public class UIManager : Singleton<UIManager>
//     {
//         //UIRoot
//         [SerializeField] private UIRoot Root = null;
//
//         // private static readonly Type typePunRPC = typeof(ModelRPC);
//         private Dictionary<Type, IUIView> UIViewDict;
//
//         // private AddressableLoadHistory addressableLoadHistory;
//         // public AddressableLoadHistory AddressableLoadHistory => addressableLoadHistory;
//         public bool IsInit => isInit;
//         private bool isInit = false;
//
//         protected override void Init()
//         {
//             if (Root == null)
//             {
//                 var roots = transform.parent.GetComponentInChildren<UIRoot>();
//                 Root = roots;
//             }
//
//             Root.SortOrder();
//             isInit = true;
//         }
//
//         public UIRoot GetRoot()
//         {
//             return Root;
//         }
//
//
//     
//     }
// }