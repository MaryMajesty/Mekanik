using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public struct Rect
	{
		public double X;
		public double Y;
		public double Width;
		public double Height;

		public Vector Position
		{
			get { return new Vector(this.X, this.Y); }

			set
			{
				this.X = value.X;
				this.Y = value.Y;
			}
		}
		public Vector Size
		{
			get { return new Vector(this.Width, this.Height); }

			set
			{
				this.Width = value.X;
				this.Height = value.Y;
			}
		}

		public Rect(double _x, double _y, double _width, double _height)
		{
			this.X = _x;
			this.Y = _y;
			this.Width = _width;
			this.Height = _height;
		}

		public Rect(Vector _position, Vector _size)
		{
			this.X = _position.X;
			this.Y = _position.Y;
			this.Width = _size.X;
			this.Height = _size.Y;
		}

		public Rect MakeFit(Vector _size)
		{
			double s = Meth.Min(this.Width / _size.X, this.Height / _size.Y);
			return new Rect((this.Size - _size * s) / 2, _size * s);
		}

		public bool Overlaps(Rect _other) => /*(Meth.Abs(this.X - _other.X) <= this.Width + _other.Width && Meth.Abs(this.Y - _other.Y) <= this.Height + _other.Height) && */(this.X + this.Width > _other.X && this.X < _other.X + _other.Width && this.Y + this.Height > _other.Y && this.Y < _other.Y + _other.Height);

		public bool Contains(Vector _vector) => _vector.X >= this.X && _vector.X < this.X + this.Width && _vector.Y >= this.Y && _vector.Y < this.Y + this.Height;

		public bool ContainsE(Vector _vector) => _vector.X >= this.X && _vector.X <= this.X + this.Width && _vector.Y >= this.Y && _vector.Y <= this.Y + this.Height;

		public Rect Zoom(Vector _position, double _factor) => new Rect(this.Position + (_position - this.Position) * (1 - 1 / _factor), this.Size / _factor);

		//public bool Contains(Rect _rect) 
		//{
		//	return _rect.X >= this.X && _rect.X + _rect.Width <= this.X + this.Width && _rect.Y >= this.Y && _rect.Y + _rect.Height <= this.Y + this.Height;
		//}

		//public static Rect operator +(Rect _one, Vector _two) { return new Rect(_one.X + _two.X, _one.Y + _two.Y, _one.Width, _one.Height); }
		//public static Rect operator -(Rect _one, Vector _two) { return new Rect(_one.X - _two.X, _one.Y - _two.Y, _one.Width, _one.Height); }

		//public static Rect operator *(Rect _one, Vector _two)
		//{
		//	Rect @out = new Rect(_one.Position * _two, _one.Size * _two);
		//	if (_two.X < 0)
		//	{
		//		double x = @out.X;
		//		@out.X = @out.Width + x;
		//		@out.Width *= -1;
		//	}
		//	if (_two.Y < 0)
		//	{
		//		double y = @out.Y;
		//		@out.Y = @out.Height + y;
		//		@out.Height *= -1;
		//	}
		//	return @out;
		//}

		//public override string ToString() { return "Rect { " + "X " + this.X.String() + "; Y " + this.Y.String() + "; Width " + this.Width.String() + "; Height " + this.Height.String() + "; }"; }

		//public static implicit operator SFML.Graphics.View(Rect @this) { return new SFML.Graphics.View(new SFML.Graphics.FloatRect((float)@this.Position.X, (float)@this.Position.Y, (float)@this.Size.X, (float)@this.Size.Y))/* { Rotation = (float)(@this.Rotation / Meth.Tau * 360) * -1 }*/; }
		//public static explicit operator Rect(SFML.Graphics.View @this) { return new Rect((Vector)@this.Center - (Vector)@this.Size / 2, (Vector)@this.Size)/* { Rotation = (double)@this.Rotation / 360 * Meth.AngleUnit * -1 }*/; }
	}
}