using UnityEditor;
using UnityEngine;

namespace FilterConsole
{
    public class Splitter
    {
        public enum SplitMode
        {
            Horizonal = 1,
            Vertical = 2,
        }

        public float value
        {
            get { return splitterValue; }
            set { splitterValue = value; }
        }

        public delegate void SplitViewOnGUIDelegate(Rect drawRect);

        private SplitViewOnGUIDelegate onDrawLeftDelegate;
        private SplitViewOnGUIDelegate onDrawRightDelegate;

        private float splitterValue;
        private SplitMode lockMode;
        private float lockValues;
        private bool resize;
        private MouseCursor mouseCursor;

        private Rect leftRect = new Rect();
        private Rect rightRect = new Rect();
        private Rect resizeRect = new Rect();
        private RectOffset rectOffset;
        private float barSize;
        private float ratio = -1f;

        private static readonly RectOffset VerticalRectOffset = new RectOffset(7, 8, 0, 0);
        private static readonly RectOffset HorizontalRectOffset = new RectOffset(0, 0, 7, 8);
        private static readonly Color SplitterColor = new Color(0, 0, 0, 1.0f);

        public Splitter(
            float initialValue,
            SplitViewOnGUIDelegate onDrawLeftDelegate,
            SplitViewOnGUIDelegate onDrawRightDelegate,
            SplitMode lockMode,
            float minValue,
            float barSize = 16f
        )
        {
            splitterValue = initialValue;
            this.onDrawLeftDelegate = onDrawLeftDelegate;
            this.onDrawRightDelegate = onDrawRightDelegate;
            this.lockMode = lockMode;
            lockValues = minValue;
            mouseCursor = lockMode == SplitMode.Vertical ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical;
            rectOffset = lockMode == SplitMode.Vertical ? VerticalRectOffset : HorizontalRectOffset;
            this.barSize = barSize;
        }

        public bool DoSplitter(Rect rect)
        {
            if (onDrawLeftDelegate == null || onDrawRightDelegate == null)
            {
                Debug.LogError("delegate is null");
                return false;
            }

            // LeftWindow
            leftRect.x = rect.x;
            leftRect.y = rect.y;
            leftRect.width = lockMode == SplitMode.Vertical ? splitterValue : rect.width;
            leftRect.height = lockMode == SplitMode.Vertical ? rect.height : splitterValue;
            onDrawLeftDelegate(leftRect);

            // rightWindow
            rightRect.x = lockMode == SplitMode.Vertical ? rect.x + splitterValue : rect.x;
            rightRect.y = lockMode == SplitMode.Vertical ? rect.y : rect.y + splitterValue;
            rightRect.width = lockMode == SplitMode.Vertical ? rect.width - splitterValue : rect.width;
            rightRect.height = lockMode == SplitMode.Vertical ? rect.height : rect.height - splitterValue;
            onDrawRightDelegate(rightRect);

            return HandlePanelResize(rect);
        }

        private bool HandlePanelResize(Rect rect)
        {
            resizeRect.x = lockMode == SplitMode.Vertical ? rect.x + splitterValue - barSize / 2 : rect.x;
            resizeRect.y = lockMode == SplitMode.Vertical ? rect.y : rect.y + splitterValue - barSize / 2;
            resizeRect.width = lockMode == SplitMode.Vertical ? barSize : rect.width;
            resizeRect.height = lockMode == SplitMode.Vertical ? rect.height : barSize;
            EditorGUIUtility.AddCursorRect(resizeRect, mouseCursor);

            float clampMax = lockMode == SplitMode.Vertical ? rect.width - lockValues : rect.height - lockValues;
            float targetSplitterValue = lockMode == SplitMode.Vertical ? rect.width : rect.height;

            if (Event.current.type == EventType.MouseDown && resizeRect.Contains(Event.current.mousePosition))
            {
                resize = true;
            }

            if (Event.current.type == EventType.MouseUp)
            {
                resize = false;
            }

            if (resize && Event.current.type == EventType.MouseDrag)
            {
                float targetValue = lockMode == SplitMode.Vertical
                    ? Event.current.mousePosition.x
                    : Event.current.mousePosition.y;
                float diffValue = lockMode == SplitMode.Vertical ? rect.width : rect.height;
                ratio = targetValue / diffValue;
            }
            else if ((Event.current.type != EventType.Layout) && (Event.current.type != EventType.Used))
            {
                ratio = (targetSplitterValue * ratio) / targetSplitterValue;
            }
            else if (ratio < 0f)
            {
                ratio = lockMode == SplitMode.Vertical
                    ? splitterValue / (float)rect.width
                    : splitterValue / (float)rect.height;
            }

            splitterValue = Mathf.Clamp(targetSplitterValue * ratio, lockValues, clampMax);

            EditorGUI.DrawRect(rectOffset.Remove(resizeRect), SplitterColor);

            return resize;
        }
    }
}