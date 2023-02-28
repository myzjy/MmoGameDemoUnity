using System;
using Tools.Util;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.UI.UIModel
{
    public class Dialog : DialogBase
    {
        public enum ButtonColor
        {
            Yellow,
            Red
        }

        public enum ButtonType
        {
            YesNo,
            Yes
        }

        public enum Result
        {
            OK,
            Yes,
            No,
        }

        /// <summary>
        /// 标题
        /// </summary>
        [SerializeField] protected Text titleText = null;

        /// <summary>
        /// 标题背景
        /// </summary>
        [SerializeField] protected GameObject titleBg = null;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] protected GameObject noTitleBg = null;

        /// <summary>
        /// 文字主题
        /// </summary>
        [SerializeField] protected Text bodyText = null;

        [SerializeField] protected Button noButton = null;
        [SerializeField] protected Text noButtonText = null;
        [SerializeField] protected Button yesButton = null;
        [SerializeField] protected Text yesButtonText = null;
        [SerializeField] protected Text yesRedButtonText = null;
        protected System.Action<Result> onClickAction = null;

        System.Action onEscClickEvent;
        ButtonType openButtonType;

        public override void Init()
        {
            base.Init();
        }

        public bool Open(ButtonType type, string title, string body, Action<Result> onClick = null,
            Action onOpen = null, Action onClose = null, Action onEscClick = null)
        {
            if (!base.Open())
            {
                return false;
            }

            SetTitleText(title);
            SetBodyText(body);
            SetupButtons(type);

            this.onEscClickEvent = onEscClick;
            if (backgroundCloseButton != null)
            {
                backgroundCloseButton.SetListener(() =>
                {
                    this.Close();
                    if (onClose != null)
                    {
                        onClose?.Invoke();
                    }
                });
            }

            onClickAction = onClick;
            return true;
        }

        public bool Open(ButtonType type, string title, string body, string YesButtonText, string NoButtonText,
            Action<Result> onClick = null,
            Action onOpen = null, Action onClose = null, Action onEscClick = null)
        {
            if (!base.Open())
            {
                return false;
            }

            SetTitleText(title);
            SetBodyText(body);
            SetupButtons(type);
            this.onEscClickEvent = onEscClick;
            if (backgroundCloseButton != null)
            {
                backgroundCloseButton.SetListener(() =>
                {
                    this.Close();
                    if (onClose != null)
                    {
                        onClose?.Invoke();
                    }
                });
            }

            if (yesButtonText != null)
            {
                yesButtonText.text = YesButtonText;
            }

            if (noButtonText != null)
            {
                noButtonText.text = NoButtonText;
            }

            onClickAction = onClick;
            return true;
        }

        protected void SetTitleText(string text)
        {
            if (titleText == null)
            {
                return;
            }

            titleText.text = text;
        }

        protected void SetBodyText(string text, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            if (bodyText == null)
            {
                return;
            }

            bodyText.text = Util.ReplaceNewLine(text);
            bodyText.alignment = alignment;
        }

        protected void SetupButtons(ButtonType type)
        {
            openButtonType = type;


            if (noButton != null)
            {
                noButton.enabled = false;
            }

            if (yesButton != null)
            {
                yesButton.enabled = false;
                if (yesRedButtonText != null)
                {
                    yesRedButtonText.enabled = false;
                }
            }

            switch (type)
            {
                case ButtonType.YesNo:
                    if (noButton != null)
                    {
                        noButton.enabled = true;
                    }

                    if (yesButton != null)
                    {
                        yesButton.enabled = true;
                    }

                    break;

                case ButtonType.Yes:
                    if (yesButton != null)
                    {
                        yesButton.enabled = true;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void OnButton(Result result)
        {
            this.Close();

            if (onClickAction != null)
            {
                onClickAction.Invoke(result);
            }
        }

        public void OnClickOk()
        {
            OnButton(Result.OK);
        }

        public void OnClickNo()
        {
            OnButton(Result.No);
        }

        public void OnClickYes()
        {
            OnButton(Result.Yes);
        }
    }
}