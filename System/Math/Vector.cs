using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public struct Vector
	{
		public double X;
		public double Y;
		public double Z;

		public double Angle
		{
			get
			{
				double a = Meth.Atan2(this.X, this.Y);
				if (a < 0)
					return Meth.Tau + a;
				else
					return a;
			}

			set { this = Vector.FromAngle(value, this.Length); }
		}
		public double Length
		{
			get { return Meth.Pythagoras(this.X, this.Y, this.Z); }

			set
			{
				if (this.Length > 0)
					this *= value / this.Length;
			}
		}
		public Vector Normal
		{
			get { return Vector.FromAngle(this.Angle); }
		}
		public Vector OnlyX
		{
			get { return new Vector(this.X, 0); }
		}
		public Vector OnlyY
		{
			get { return new Vector(0, this.Y); }
		}
		public Point Up
		{
			get { return new Point(Meth.Up(this.X), Meth.Up(this.Y)); }
		}

		public Vector(double _value)
		{
			this.X = _value;
			this.Y = _value;
			this.Z = 0;
		}

		public Vector(double _x, double _y)
		{
			this.X = _x;
			this.Y = _y;
			this.Z = 0;
		}

		public Vector(double _x, double _y, double _z)
		{
			this.X = _x;
			this.Y = _y;
			this.Z = _z;
		}

		public override string ToString() => X.String() + ", " + Y.String();

		public Vector Flip() => new Vector(this.Y, this.X);

		public double Project(Vector _vector) => (this.X * _vector.X + this.Y * _vector.Y) / this.Length;
		
		public static double Project(double _angle, Vector _vector) { return Vector.FromAngle(_angle).Project(_vector); }

		public static Vector Parse(string _string)
		{
			Func<string, string> removespaces = item =>
				{
					while (item[0] == ' ')
						item = item.Substring(1);
					while (item[item.Length - 1] == ' ')
						item = item.Substring(0, item.Length - 1);
					return item;
				};

			if (_string.Contains(','))
			{
				string x = removespaces(_string.Substring(0, _string.IndexOf(',')));
				string y = removespaces(_string.Substring(_string.IndexOf(',') + 1));
				return new Vector(double.Parse(x), double.Parse(y));
			}
			else
				return new Vector(double.Parse(_string));
		}

		public static Vector FromAngle(double _angle, double _length = 1) => new Vector(Meth.Cos(_angle), Meth.Sin(_angle)) * _length;

		public static double GetRadialDifference(Vector _one, Vector _two)
		{
			Bunch<double> diffs = new Bunch<double>() { _two.Angle - _one.Angle - Meth.Tau, _two.Angle - _one.Angle, _two.Angle - _one.Angle + Meth.Tau };
			double min = double.MaxValue;
			foreach (double diff in diffs)
			{
				if (Meth.Abs(diff) < Meth.Abs(min))
					min = diff;
			}
			if (min == Meth.Tau / -2)
				return Meth.Tau / 2;
			return min;
		}
		


		public static bool operator ==(Vector _one, Vector _two) => _one.X == _two.X && _one.Y == _two.Y && _one.Z == _two.Z;
		public static bool operator !=(Vector _one, Vector _two) => !(_one == _two);

		public static Vector operator +(Vector _one, Vector _two) => new Vector(_one.X + _two.X, _one.Y + _two.Y, _one.Z + _two.Z);
		public static Vector operator -(Vector _one, Vector _two) => new Vector(_one.X - _two.X, _one.Y - _two.Y, _one.Z - _two.Z);

		public static Vector operator *(Vector _one, Vector _two) => new Vector(_one.X * _two.X, _one.Y * _two.Y, _one.Z * _two.Z);
		public static Vector operator /(Vector _one, Vector _two) => new Vector(_one.X / _two.X, _one.Y / _two.Y);

		public static Vector operator +(Vector _one, Point _two) => new Vector(_one.X + _two.X, _one.Y + _two.Y);
		public static Vector operator -(Vector _one, Point _two) => new Vector(_one.X - _two.X, _one.Y - _two.Y);

		public static Vector operator *(Vector _one, Point _two) => new Vector(_one.X * _two.X, _one.Y * _two.Y);
		public static Vector operator /(Vector _one, Point _two) => new Vector(_one.X / _two.X, _one.Y / _two.Y);

		public static Vector operator *(Vector _one, int _two) => new Vector(_one.X * _two, _one.Y * _two, _one.Z * _two);
		public static Vector operator /(Vector _one, int _two) => new Vector(_one.X / _two, _one.Y / _two, _one.Z / _two);

		public static Vector operator *(Vector _one, double _two) => new Vector(_one.X * _two, _one.Y * _two, _one.Z * _two);
		public static Vector operator /(Vector _one, double _two) => new Vector(_one.X / _two, _one.Y / _two, _one.Z / _two);
		
		public static Vector operator %(Vector _one, Vector _two) => new Vector(Meth.RMod(_one.X, _two.X), Meth.RMod(_one.Y, _two.Y));

		public static Vector operator -(Vector @this) => @this * -1;

		

		public static implicit operator Microsoft.Xna.Framework.Vector2(Vector @this) => new Microsoft.Xna.Framework.Vector2((float)(@this.X / GameBase.Meter), (float)(@this.Y / GameBase.Meter));
		public static implicit operator Vector(Microsoft.Xna.Framework.Vector2 @this) => new Vector(@this.X, @this.Y) * GameBase.Meter;

		public static implicit operator Vector(Point @this) => new Vector(@this.X, @this.Y);

		public static implicit operator Vector(double @this) => new Vector(@this);
	}
}
