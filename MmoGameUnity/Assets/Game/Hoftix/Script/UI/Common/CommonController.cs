using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Ocsp;
using UnityEngine;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.UISerializable
{
    public class CommonController : Singleton<CommonController>
    {
        public LoadingRotate loadingRotate;
        public Snackbar snackbar;

        protected override void Init()
        {
            base.Init();
        }
    }
}