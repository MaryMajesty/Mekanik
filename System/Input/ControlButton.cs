using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mekanik;

namespace Mekanik
{
	public class ControlButton
	{
		internal bool _IsPressed;
		internal bool _GotPressed;
		internal bool _IsDoubleTapped;
		internal bool _GotDoubleTapped;
		internal bool _GotReleased;
		internal bool _IsDown;
		internal int _DoubleTapCountdown;
		internal int _NumberOfPresses;

		public bool IsPressed
		{
			get { return this._IsPressed; }
		}
		public bool GotPressed
		{
			get { return this._GotPressed; }
		}
		public bool IsDoubleTapped
		{
			get { return this._IsDoubleTapped; }
		}
		public bool GotDoubleTapped
		{
			get { return this._GotDoubleTapped; }
		}
		public bool GotReleased
		{
			get { return this._GotReleased; }
		}
		public bool IsDown
		{
			get { return this._IsDown; }
		}
		public int NumberOfPresses
		{
			get { return this._NumberOfPresses; }
		}
	}
}