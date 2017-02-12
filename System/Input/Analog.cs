using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Analog
	{
		internal Bunch<Key> _Keys = new Bunch<Key>();
		//internal Bunch<bool> _KeyValues = new Bunch<bool>();
		internal Sandbox<Key, bool> _KeyDict;
		private GamePadTrigger _Trigger;
		private GamePadStick _Stick;
		private int _InputType;
		internal GamePad _GamePad;
		internal double _PressedThreshold;
		internal double _Threshold;
		internal double _Fluidity;
		private double _Value1D;
		private Vector _Value2D;
		private bool? _Vertical;
		private bool _LastPressed;

		public bool IsPressed
		{
			get
			{
				if (this._InputType == 1)
					return this.Value > this._PressedThreshold;
				else if (this._Keys.Count == 1)
					return this._GetPressed(0);
				else
					throw new Exception("The specified input type doesn't support 1D values.");
			}
		}
		private bool _GotPressed;
		public bool GotPressed
		{
			get { return this._GotPressed; }
		}
		private bool _GotReleased;
		public bool GotReleased
		{
			get { return this._GotReleased; }
		}
		public double Value
		{
			get
			{
				if (this._InputType == 0)
				{
					if (this._Keys.Count < 4)
						return this._Value1D;
					else
						throw new Exception("The specified input type doesn't support 1D values.");
				}
				else if (this._InputType == 1)
					return this._Value1D;
					//return this._GamePad.Inputs1D[this._Trigger];
				else
					throw new Exception("The specified input type doesn't support 1D values.");
			}
		}
		public Vector Vector
		{
			get
			{
				if (this._Keys.Count == 4)
					return this._Value2D;
				else if (this._InputType == 2)
					return this._GamePad.Inputs2D[this._Stick];
				else
					throw new Exception("The specified input type doesn't support 2D values.");
			}
		}
		public bool IsUsed
		{
			get
			{
				if (this._Keys.Count > 0)
				{
					bool b = this._Keys.Any(item => this._GetPressed(this._Keys.IndexOf(item)));
					if (this._Keys.Count == 4)
						return b && this._GetValue2D() != 0;
					else if (this._Keys.Count == 2)
						return b && this._GetValue1D() != 0;
					else
						return b;
				}
				else if (this._InputType == 1)
					return Meth.Abs(this._Value1D) > this._Threshold;
				else
					return this._Value2D.Length > this._Threshold;
			}
		}


		private bool _GetPressed(int _key)
		{
			Key key = this._Keys[_key];
			if (key < Key.Separator)
				return this._KeyDict[key];
			else
				return this._GamePad.Inputs0D[key];
		}

		public Analog(Key _key, double _fluidity = 0)
		{
			this._Keys = new Bunch<Key>(_key);
			//this._KeyValues = new Bunch<bool>(false);
			this._Fluidity = _fluidity;
		}

		public Analog(Key _low, Key _high, double _fluidity = 0.5)
		{
			this._Keys = new Bunch<Key>(_low, _high);
			//this._KeyValues = new Bunch<bool>(false, false);
			this._Fluidity = _fluidity;
		}

		public Analog(Key _left, Key _right, Key _up, Key _down, double _fluidity = 0.5)
		{
			this._Keys = new Bunch<Key>(_left, _right, _up, _down);
			//this._KeyValues = new Bunch<bool>(false, false, false, false);
			this._Fluidity = _fluidity;
		}

		public Analog(GamePadTrigger _trigger, double _threshold = 0.1, double _pressedthreshold = 0.5)
		{
			this._Trigger = _trigger;
			this._Threshold = _threshold;
			this._PressedThreshold = _pressedthreshold;
			this._InputType = 1;
		}

		public Analog(GamePadStick _stick, bool _vertical, double _threshold = 0.1)
		{
			this._Stick = _stick;
			this._InputType = 1;
			this._Vertical = _vertical;
			this._Threshold = _threshold;
		}

		public Analog(GamePadStick _stick, double _threshold = 0.1)
		{
			this._Stick = _stick;
			this._InputType = 2;
			this._Threshold = _threshold;
		}

		internal Analog _Clone()
		{
			if (this._Keys.Count == 1)
				return new Analog(this._Keys[0], _fluidity: this._Fluidity);
			else if (this._Keys.Count == 2)
				return new Analog(this._Keys[0], this._Keys[1], _fluidity: this._Fluidity);
			else if (this._Keys.Count == 4)
				return new Analog(this._Keys[0], this._Keys[1], this._Keys[2], this._Keys[3], _fluidity: this._Fluidity);
			else if (this._InputType == 1)
			{
				if (this._Vertical.HasValue)
					return new Analog(this._Stick, this._Vertical.Value, _threshold: this._Threshold);
				else
					return new Analog(this._Trigger, _pressedthreshold: this._PressedThreshold);
			}
			else
				return new Analog(this._Stick, _threshold: this._Threshold);
		}

		private double _GetValue1D()
		{
			if (this._Keys.Count == 1)
				return this._GetPressed(0) ? 1 : 0;
			else if (this._Keys.Count == 2)
				return (this._GetPressed(0) ? -1 : 0) + (this._GetPressed(1) ? 1 : 0);
			else if (this._InputType == 1)
			{
				double d;

				if (this._Vertical.HasValue)
				{
					Vector v = this._GamePad.Inputs2D[this._Stick];
					d = this._Vertical.Value ? v.Y : v.X;
				}
				else
					d = this._GamePad.Inputs1D[this._Trigger];

				if (Meth.Abs(d) < this._Threshold)
					d = 0;
				return d;
			}
			else
				throw new Exception("The specified input type doesn't support 1D values.");
		}

		private Vector _GetValue2D()
		{
			if (this._Keys.Count == 4)
				return new Vector((this._GetPressed(0) ? -1 : 0) + (this._GetPressed(1) ? 1 : 0), (this._GetPressed(2) ? -1 : 0) + (this._GetPressed(3) ? 1 : 0));
			else if (this._InputType == 2)
			{
				Vector v = this._GamePad.Inputs2D[this._Stick];
				if (v.Length < this._Threshold)
					v = 0;
				return v;
			}
			else
				throw new Exception("The specified input type doesn't support 2D values.");
		}

		internal void _Update()
		{
			if (this._Keys.Count == 1)
			{
				bool b = this._GetPressed(0);
				if (b != this._LastPressed)
				{
					this._LastPressed = b;
					if (b)
					{
						this._GotPressed = true;
						this._GotReleased = false;
					}
					else
					{
						this._GotReleased = true;
						this._GotPressed = false;
					}
				}
				else
				{
					this._GotPressed = false;
					this._GotReleased = false;
				}
			}

			if (this._Keys.Count == 4)
			{
				Vector v = this._GetValue2D();
				this._Value2D += (v - this._Value2D) * (1 - this._Fluidity);
				if ((this._Value2D - v).Length < 0.01)
					this._Value2D = v;
			}
			else if (this._InputType != 2)
			{
				double d = this._GetValue1D();
				this._Value1D += (d - this._Value1D) * (1 - this._Fluidity);
				if (Meth.Abs(this._Value1D - d) < 0.01)
					this._Value1D = d;
			}
		}
	}
}