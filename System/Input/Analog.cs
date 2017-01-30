using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Analog
	{
		public double AirSmoothness;
		public double GroundSmoothness;
		//public bool IsHorizontalMovement;
		internal Entity _Entity;
		private string _Negative;
		private string _Positive;
		private double _Value;
		private bool _GotPressed;
		private double _LastV;

		//public bool GotPressed
		//{
		//	get { return _GotPressed; }
		//}

		public bool IsDown
		{
			get { return this._Entity.Controls[this._Negative].IsDown || this._Entity.Controls[this._Positive].IsDown; }
		}
		public bool IsPressed
		{
			get { return this._Entity.Controls[this._Negative].IsPressed || this._Entity.Controls[this._Positive].IsPressed; }
		}
		public bool IsDoubleTapped
		{
			get { return this._Entity.Controls[this._Negative].IsDoubleTapped || this._Entity.Controls[this._Positive].IsDoubleTapped; }
		}
		public double Value
		{
			get { return _Value; }
			internal set { this._Value = value; }
		}
		public int PressValue
		{
			get { return (_Entity.Controls[_Negative].IsDown ? -1 : 0) + (_Entity.Controls[this._Positive].IsDown ? 1 : 0); }
		}

		public Analog(string _negative, string _positive)
		{
			this._Negative = _negative;
			this._Positive = _positive;
		}

		internal void _Update()
		{
			double v = 0;
			if (_Entity.Controls[_Negative].IsDown)
				v -= 1;
			if (_Entity.Controls[_Positive].IsDown)
				v += 1;

			if (v != _LastV)
			{
				_LastV = v;
				_GotPressed = v != 0;
			}
			else
				_GotPressed = false;

			//double d = (_Entity.IsOnGround ? GroundSmoothness : AirSmoothness);
			//_Value = _Value * d + v * (1 - d);
		}
	}
}