using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mekanik;

namespace Mekanik
{
	public class Controls
	{
		public int DoubleTapCountdown;
		internal Dictionary<string, ControlButton> _Buttons = new Dictionary<string, ControlButton>();

		public ControlButton this[string _name]
		{
			get
			{
				if (!_Buttons.ContainsKey(_name))
					_Buttons[_name] = new ControlButton();
				return _Buttons[_name];
			}
		}

		internal void _Update()
		{
			foreach (ControlButton button in _Buttons.Select(item => item.Value))
			{
				button._GotPressed = false;
				button._GotDoubleTapped = false;
				button._GotReleased = false;
				button._DoubleTapCountdown = Meth.Max(0, button._DoubleTapCountdown - 1);
				button._NumberOfPresses = 0;
			}
		}

		internal void _Press(string _name)
		{
			ControlButton b = this[_name];

			if (!b.IsDown)
			{
				if (b._DoubleTapCountdown > 0)
				{
					b._DoubleTapCountdown = 0;
					b._IsDoubleTapped = true;
					b._GotDoubleTapped = true;
				}
				else
				{
					b._DoubleTapCountdown = this.DoubleTapCountdown;
					b._IsPressed = true;
					b._GotPressed = true;
				}
			}
			b._IsDown = true;
			b._NumberOfPresses++;
		}

		public void Press(string _name)
		{
			ControlButton b = this[_name];
			b._IsPressed = true;
			b._GotPressed = true;
			b._IsDoubleTapped = false;
			b._IsDown = true;
		}

		public void PressOnce(string _name)
		{
			ControlButton b = this[_name];
			b._IsPressed = false;
			b._GotPressed = true;
			b._IsDoubleTapped = false;
			b._IsDown = false;
			b._NumberOfPresses++;
		}

		public void DoubleTap(string _name)
		{
			ControlButton b = this[_name];
			b._IsDoubleTapped = true;
			b._GotDoubleTapped = true;
			b._IsPressed = false;
			b._IsDown = true;
		}

		public void DoubleTapOnce(string _name)
		{
			ControlButton b = this[_name];
			b._IsDoubleTapped = false;
			b._GotDoubleTapped = true;
			b._IsPressed = false;
			b._IsDown = false;
		}

		public void RemoveDoubleTap(string _name)
		{
			ControlButton b = this[_name];
			b._IsDoubleTapped = false;
			b._IsPressed = true;
			b._GotPressed = true;
		}

		public void Release(string _name)
		{
			ControlButton b = this[_name];
			b._IsPressed = false;
			b._GotPressed = false;
			b._IsDoubleTapped = false;
			b._GotDoubleTapped = false;
			b._GotReleased = true;
			b._IsDown = false;
		}

		public void ReleaseAll()
		{
			foreach (KeyValuePair<string, ControlButton> buttonpair in _Buttons)
				Release(buttonpair.Key);
		}

		public void ResetCountdown()
		{
			foreach (KeyValuePair<string, ControlButton> button in this._Buttons)
				button.Value._DoubleTapCountdown = 0;
		}
	}
}