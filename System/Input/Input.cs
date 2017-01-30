using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public abstract class InputSchemeRaw
	{
		internal Entity _Entity;
		internal string[] _Keys;

		internal Controls _Controls
		{
			get { return this._Entity.Controls; }
		}

		internal virtual void _Update() { }
	}

	public abstract class InputScheme<T> : InputSchemeRaw
	{
		protected T _Value;
		public T Value
		{
			get { return this._Value; }
		}

		public InputScheme(params string[] _keys)
		{
			this._Keys = _keys;
		}
		
		//protected abstract T _GetOutput();
	}

	public class Analog1 : InputScheme<double>
	{
		//private double _Value;
		public double Threshold = 0.2;

		public bool IsUsed
		{
			get { return Meth.Abs(this._Value) > this.Threshold; }
		}
		public int Direction
		{
			get { return Meth.Sign(this._Value); }
		}

		public Analog1(string _left, string _right) : base(_left, _right) { }

		internal override void _Update()
		{
			this._Value += ((this._Controls[this._Keys[0]].IsPressed ? -1 : 0) + (this._Controls[this._Keys[1]].IsPressed ? 1 : 0) - this._Value) / 2;
		}

		//protected override double _GetOutput() => this._Value;
	}
}