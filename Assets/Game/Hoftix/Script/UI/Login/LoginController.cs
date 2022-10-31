using UnityEngine;
using UnityEngine.Serialization;

namespace ZJYFrameWork.UISerializable
{
    public class LoginController : MonoBehaviour
    {
        [FormerlySerializedAs("LoginPartView")]
        public LoginPartView loginPartView = null;

        [FormerlySerializedAs("RegisterPart_RegisterPartView")]
        public RegisterPartView registerPartRegisterPartView = null;
    }
}