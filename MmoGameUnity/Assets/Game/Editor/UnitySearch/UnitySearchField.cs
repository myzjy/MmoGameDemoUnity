using UnityEditor;
using UnityEngine;

namespace UnitySearch
{
	internal class UnitySearchField
	{
		private bool _autoSetFocusOnFindCommand = true;
		private string _controlIDName;
		private bool _wantsFocus;
		
		internal delegate void SearchFieldCallback(bool isUp);

		internal event SearchFieldCallback downOrUpArrowKeyPressed;

		internal UnitySearchField()
		{
			_controlIDName = GUIUtility.GetControlID("UnitySearchField".GetHashCode(), FocusType.Passive).ToString();
		}

		internal void SetFocus()
		{
			_wantsFocus = true;
		}

		internal string OnGUI(Rect rect, string text, GUIStyle style, GUIStyle clearStyle)
		{
			CommandEventHandling();
			FocusAndKeyHandling();
			var fixedWidth = 35f;
			var position1 = rect;
			position1.width -= fixedWidth;
			GUI.SetNextControlName(_controlIDName);
			text = EditorGUI.TextField(position1, text, style);
			if (!string.IsNullOrEmpty(text))
			{
				var position2 = rect;
				position2.x += rect.width - fixedWidth;
				position2.width = fixedWidth;
				if (GUI.Button(position2,"☓", clearStyle))
				{
					text = "";
					GUIUtility.keyboardControl = 0;
				}
			}

			return text;
		}

		private void FocusAndKeyHandling()
		{
			var current = Event.current;
			if (_wantsFocus && current.type == EventType.Repaint)
			{
				GUI.FocusControl(_controlIDName);
				EditorGUIUtility.editingTextField = true;
				_wantsFocus = false;
			}
			
			if (downOrUpArrowKeyPressed == null)
				return;

			if (current.type != EventType.KeyDown)
				return;

			if (current.keyCode != KeyCode.DownArrow && current.keyCode != KeyCode.UpArrow)
			{
				if (current.keyCode == KeyCode.RightArrow || current.keyCode == KeyCode.LeftArrow)
					if (GUI.GetNameOfFocusedControl() != _controlIDName)
						GUI.FocusControl(_controlIDName);
				
				return;
			}
			
			if (GUI.GetNameOfFocusedControl() != _controlIDName)
				return;
			
			if (GUIUtility.hotControl != 0)
				return;
			
			downOrUpArrowKeyPressed(current.keyCode == KeyCode.UpArrow);
			current.Use();
		}

		private void CommandEventHandling()
		{
			var current = Event.current;
			if (current.type != EventType.ExecuteCommand && current.type != EventType.ValidateCommand ||
			    !_autoSetFocusOnFindCommand || current.commandName != "Find")
				return;
			
			if (current.type == EventType.ExecuteCommand)
				SetFocus();
			
			current.Use();
		}
	}
}