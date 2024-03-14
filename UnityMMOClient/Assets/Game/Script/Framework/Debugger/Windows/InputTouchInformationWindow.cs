using System.Collections.Generic;
using UnityEngine;
using ZJYFrameWork.Debugger.Widows.Model;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Debugger.Windows
{
    public sealed class InputTouchInformationWindow : ScrollableDebuggerWindowBase
    {
        protected override void OnDrawScrollableWindow()
        {
            GUILayout.Label("<b>Input Touch Information</b>");
            GUILayout.BeginVertical("box");
            {
                DrawItem("Touch Supported", Input.touchSupported.ToString());
                DrawItem("Touch Pressure Supported", Input.touchPressureSupported.ToString());
                DrawItem("Stylus Touch Supported", Input.stylusTouchSupported.ToString());
                DrawItem("Simulate Mouse With Touches", Input.simulateMouseWithTouches.ToString());
                DrawItem("Multi Touch Enabled", Input.multiTouchEnabled.ToString());
                DrawItem("Touch Count", Input.touchCount.ToString());
                DrawItem("Touches", GetTouchesString(Input.touches));
            }
            GUILayout.EndVertical();
        }

        private static string GetTouchString(Touch touch)
        {
            return StringUtils.Format("{}, {}, {}, {}, {}", touch.position.ToString(), touch.deltaPosition.ToString(), touch.rawPosition.ToString(), touch.pressure.ToString(), touch.phase.ToString());
        }

        private string GetTouchesString(IReadOnlyList<Touch> touches)
        {
            var touchStrings = new string[touches.Count];
            for (var i = 0; i < touches.Count; i++)
            {
                touchStrings[i] = GetTouchString(touches[i]);
            }

            return string.Join("; ", touchStrings);
        }
    }
}