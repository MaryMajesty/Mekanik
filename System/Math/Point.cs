using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
    public struct Point
    {
		private static Bunch<Point> _Directions = new Bunch<Point>(new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1));

		public int X;
		public int Y;

		public double Angle
		{
			get { return ((Vector)this).Angle; }
		}
		public double Length
		{
			get { return ((Vector)this).Length; }
		}
		
		public static Point Right
		{
			get { return Point._Directions[0]; }
		}
		public static Point Down
		{
			get { return Point._Directions[1]; }
		}
		public static Point Left
		{
			get { return Point._Directions[2]; }
		}
		public static Point Up
		{
			get { return Point._Directions[3]; }
		}
		public static Point Null
		{
			get { return new Point(0, 0); }
		}
		public Point NextClockwise
		{
			get { return Point._Directions[(Point._Directions.IndexOf(this) + 1) % 4]; }
		}
		public Point NextAntiClockwise
		{
			get { return Point._Directions[(Point._Directions.IndexOf(this) + 3) % 4]; }
		}
		public Point OnlyX
		{
			get { return new Point(this.X, 0); }
		}
		public Point OnlyY
		{
			get { return new Point(0, this.Y); }
		}

		public Point(int _value)
		{
			this.X = _value;
			this.Y = _value;
		}

		public Point(int _x, int _y)
		{
			this.X = _x;
			this.Y = _y;
		}

		public override string ToString() => this.X.String() + ", " + this.Y.String();
		
		public static Point Parse(string _string)
		{
			if (_string.ToLower() == "left")
				return Point.Left;
			else if (_string.ToLower() == "right")
				return Point.Right;
			else if (_string.ToLower() == "up")
				return Point.Up;
			else if (_string.ToLower() == "down")
				return Point.Down;
			else
				return Vector.Parse(_string);
		}

		public string ToWord()
		{
			if (this == Point.Right)
				return "Right";
			else if (this == Point.Left)
				return "Left";
			else if (this == Point.Down)
				return "Down";
			else if (this == Point.Up)
				return "Up";
			else
				return null;
		}


		
		public static bool operator ==(Point _one, Point _two) => _one.X == _two.X && _one.Y == _two.Y;
		public static bool operator !=(Point _one, Point _two) => !(_one == _two);

		public static Point operator +(Point _one, Point _two) => new Point(_one.X + _two.X, _one.Y + _two.Y);
		public static Point operator -(Point _one, Point _two) => new Point(_one.X - _two.X, _one.Y - _two.Y);

		public static Point operator *(Point _one, Point _two) => new Point(_one.X * _two.X, _one.Y * _two.Y);
		public static Vector operator /(Point _one, Point _two) => new Vector((double)_one.X / _two.X, (double)_one.Y / _two.Y);

		public static Vector operator +(Point _one, Vector _two) => new Vector(_one.X + _two.X, _one.Y + _two.Y);
		public static Vector operator -(Point _one, Vector _two) => new Vector(_one.X - _two.X, _one.Y - _two.Y);

		public static Vector operator *(Point _one, Vector _two) => new Vector(_one.X * _two.X, _one.Y * _two.Y);
		public static Vector operator /(Point _one, Vector _two) => new Vector(_one.X / _two.X, _one.Y / _two.Y);

		public static Point operator *(Point _one, int _two) => new Point(_one.X * _two, _one.Y * _two);
		public static Point operator /(Point _one, double _two) => new Vector(_one.X / _two, _one.Y / _two);

		public static Point operator -(Point _one) => _one * -1;

		public static Point operator %(Point _one, int _two) => new Point(Meth.RMod(_one.X, _two), Meth.RMod(_one.Y, _two));
		public static Point operator %(Point _one, Point _two) => new Point(Meth.RMod(_one.X, _two.X), Meth.RMod(_one.Y, _two.Y));



		//public static explicit operator Vector2f(Point @this) => new Vector2f(@this.X, @this.Y);
		//public static explicit operator Vector2i(Point @this) => new Vector2i(@this.X, @this.Y);
		//public static explicit operator Vector2u(Point @this) => new Vector2u((uint)@this.X, (uint)@this.Y);

		//public static explicit operator Point(Vector2f @this) => new Point((int)@this.X, (int)@this.Y);
		//public static explicit operator Point(Vector2i @this) => new Point(@this.X, @this.Y);
		//public static explicit operator Point(Vector2u @this) => new Point((int)@this.X, (int)@this.Y);

		public static implicit operator Point(Vector @this) => new Point(Meth.Down(@this.X), Meth.Down(@this.Y));

		public static implicit operator Point(Direction @this) => Point.Parse(@this.ToString());
    }
}