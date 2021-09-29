﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using WindowsInput;
using WindowsInput.Events;
using WindowsInput.Events.Sources;

namespace TabletFriend
{
	public static class ToggleManager
	{
		public static HashSet<KeyCode> _heldKeys = new HashSet<KeyCode>();
		private static long _inputLocked;

		private static IKeyboardEventSource _keyboard;

		private static Dictionary<KeyCode, List<ToggleButton>> _buttons = new Dictionary<KeyCode, List<ToggleButton>>();

		public static void AddButton(KeyCode key, ToggleButton button)
		{
			List<ToggleButton> list;

			if (!_buttons.TryGetValue(key, out list))
			{
				list = new List<ToggleButton>();
				_buttons.Add(key, list);
			}

			list.Add(button);
		}


		public static void DisableButtons(KeyCode key)
		{
			if (_buttons.TryGetValue(key, out var list))
			{
				foreach (var button in list.ToArray())
				{
					Application.Current.Dispatcher.Invoke(
						delegate
						{
							button.IsChecked = false;
						}
					);
				}
			}
		}


		public static void ClearButtons() =>
			new Dictionary<KeyCode, List<ToggleButton>>();


		public static bool IsHeld(KeyCode key) =>
			_heldKeys.Contains(key);

		public static void Init()
		{
			_keyboard = Capture.Global.KeyboardAsync();

			//Capture all events from the keyboard
			_keyboard.KeyEvent += KeyEvent;
		}


		public static async Task Toggle(KeyCode key)
		{
			Interlocked.Exchange(ref _inputLocked, 1);

			if (!IsHeld(key))
			{
				await Simulate.Events().Hold(key).Invoke();
				_heldKeys.Add(key);
			}
			else
			{
				await Simulate.Events().Release(key).Invoke();
				_heldKeys.Remove(key);
			}

			Interlocked.Exchange(ref _inputLocked, 0);
		}

		private static void KeyEvent(object sender, EventSourceEventArgs<KeyboardEvent> e)
		{
			var key = e.Data?.KeyUp?.Key;
			if (!key.HasValue)
			{ 
				key = e.Data?.KeyDown?.Key;
			}
			if (Interlocked.Read(ref _inputLocked) == 0 && key.HasValue && IsHeld(key.Value))
			{
				DisableButtons(key.Value);
				_heldKeys.Remove(key.Value);
			}
		}
	}
}
