using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public class Snackbar : MonoBehaviour
    {
        // #region 竖屏
        //
        // public GameObject Root;
        // public CanvasGroup serverErrorCanvasGroup;
        // public Text serverError;
        // public CanvasGroup errorCanvasGroup;
        // public Text error;
        //
        // public CanvasGroup infoCanvasGroup;
        // public Text info;
        //
        // public CanvasGroup serverInfoCanvasGroup;
        // public Text serverInfo;
        //
        // #endregion

        #region 横屏 LandscapeLeft

        public GameObject RootLandscapeLeft;

        public CanvasGroup serverErrorLandscapeLeftCanvasGroup;
        public Text serverLandscapeLeftError;
        public CanvasGroup errorLandscapeLeftCanvasGroup;
        public Text errorLandscapeLeft;

        public CanvasGroup infoLandscapeLeftCanvasGroup;
        public Text infoLandscapeLeft;

        public CanvasGroup serverInfoLandscapeLeftCanvasGroup;
        public Text serverInfoLandscapeLeft;

        #endregion

        public void SeverError(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            // switch (Screen.orientation)
            // {
            // case ScreenOrientation.Portrait:
            // case ScreenOrientation.PortraitUpsideDown:
            // {
            //     Root.SetActive(true);
            //     RootLandscapeLeft.SetActive(false);
            //     serverError.text = message;
            //     serverErrorCanvasGroup.gameObject.SetActive(true);
            //     serverErrorCanvasGroup.alpha = 0;
            //     serverErrorCanvasGroup.DOFade(1f, 0.8f).SetEase(Ease.Linear).SetLoops(1, LoopType.Yoyo);
            //     
            // }
            //     break;
            // case ScreenOrientation.LandscapeLeft:
            // case ScreenOrientation.LandscapeRight:
            // {
            // Root?.SetActive(false);
            RootLandscapeLeft.SetActive(true);
            serverLandscapeLeftError.text = message;
            serverErrorLandscapeLeftCanvasGroup.gameObject.SetActive(true);
            serverErrorLandscapeLeftCanvasGroup.alpha = 0;
            serverErrorLandscapeLeftCanvasGroup.DOFade(1f, 0.8f).SetEase(Ease.Linear).SetLoops(1, LoopType.Yoyo);
            // }
            //     break;
            // case ScreenOrientation.AutoRotation:
            //     break;
            // default:
            //     throw new ArgumentOutOfRangeException();
            // }
            StartCoroutine(HideServerError(message));
        }

        private IEnumerator HideServerError(string message)
        {
            yield return new WaitForSeconds(3f);
            // switch (Screen.orientation)
            // {
            // case ScreenOrientation.Unknown:
            //     break;
            // case ScreenOrientation.Portrait:
            // case ScreenOrientation.PortraitUpsideDown:
            // {
            //     if (message.Equals(serverError.text))
            //     {
            //         serverErrorCanvasGroup.DOFade(0f, 1.2f)
            //             .SetEase(Ease.Linear)
            //             .SetLoops(1, LoopType.Yoyo)
            //             .OnComplete(() =>
            //             {
            //                 if (message.Equals(serverError.text))
            //                 {
            //                     serverError.text = null;
            //                     serverErrorCanvasGroup.gameObject.SetActive(false);
            //                 }
            //             });
            //     }
            //     
            // }
            //     break;
            // case ScreenOrientation.LandscapeLeft:
            // case ScreenOrientation.LandscapeRight:
            // {
            if (message.Equals(serverLandscapeLeftError.text))
            {
                serverErrorLandscapeLeftCanvasGroup.DOFade(0f, 1.2f)
                    .SetEase(Ease.Linear)
                    .SetLoops(1, LoopType.Yoyo)
                    .OnComplete(() =>
                    {
                        if (message.Equals(serverLandscapeLeftError.text))
                        {
                            serverLandscapeLeftError.text = null;
                            serverErrorLandscapeLeftCanvasGroup.gameObject.SetActive(false);
                        }
                    });
            }
            //     }
            //         break;
            //     case ScreenOrientation.AutoRotation:
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
        }
    }
}