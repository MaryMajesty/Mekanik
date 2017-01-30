using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public static class Meth
	{
		public const double Tau = Math.PI * 2;
		private static Random _Random = new Random(DateTime.Now.Millisecond);

		public static double Random
		{
			get { return Meth._Random.NextDouble(); }
		}
		public static double RandomN
		{
			get { return Meth.Random * 2 - 1; }
		}
		public static byte RandomByte
		{
			get { return (byte)Meth._Random.Next(256); }
		}
		public static int RandomBinary
		{
			get { return Meth._Random.Next(2); }
		}
		public static int RandomInt
		{
			get { return Meth._Random.Next(int.MinValue, int.MaxValue); }
		}
		public static int RandomPN
		{
			get { return Meth.Random > 0.5 ? 1 : -1; }
		}
		
		public static double Pythagoras(params double[] _nums) { return Root(_nums.Sum(item => item * item)); }

		public static double Cos(double _angle) { return Math.Cos(_angle); }
		public static double Sin(double _angle) { return Math.Sin(_angle); }
		public static double Tan(double _angle) { return Math.Tan(_angle); }
		public static double Atan2(double _x, double _y) { return Math.Atan2(_y, _x); }
		public static double Root(double _number) { return Math.Sqrt(_number); }
		public static double Pow(double _base, double _exponent) { return Math.Pow(_base, _exponent); }
		public static double Log(double _base, double _number) { return Math.Log(_number, _base); }
		public static double Square(double _number) { return _number * _number; }
		public static double InfToOne(double _number) { return (1 - 1 / (Meth.Abs(_number) + 1)) * Meth.Sign(_number); }

		public static double Asin(double _number) { return Math.Asin(_number) / Tau; }
		public static double Acos(double _number) { return Math.Acos(_number) / Tau; }
		public static double Atan(double _number) { return Math.Atan(_number) / Tau; }

		public static int Down(double _number) { return (int)Math.Floor(_number); }
		public static int Up(double _number) { return (int)Math.Ceiling(_number); }
		public static double Abs(double _number) { return Math.Abs(_number); }
		public static int Abs(int _number) { return Math.Abs(_number); }
		public static int Round(double _number) { return (int)Math.Round(_number); }
		public static int Sign(double _number) { return Math.Sign(_number); }

		public static double Min(params double[] _numbers) { return _numbers.OrderBy(item => item).ToArray()[0]; }
		public static int Min(params int[] _numbers) { return _numbers.OrderBy(item => item).ToArray()[0]; }
		public static double Max(params double[] _numbers) { return _numbers.OrderByDescending(item => item).ToArray()[0]; }
		public static int Max(params int[] _numbers) { return _numbers.OrderByDescending(item => item).ToArray()[0]; }
		public static double Limit(double _min, double _num, double _max) { return Meth.Min(Meth.Max(_num, _min), _max); }
		public static int Limit(int _min, int _num, int _max) { return Meth.Min(Meth.Max(_num, _min), _max); }

		public static double RMod(double _number, double _mod)
		{
			double r = _number % _mod;
			return (r < 0) ? (r + _mod) : r;
		}
		public static int RMod(int _number, int _mod) { return (int)RMod((double)_number, _mod); }

		public static int Mod2(int _number, int _mod) { return (_number % _mod == 0) ? _mod : _number % _mod; }
		public static double Mod2(double _number, double _mod) { return (_number % _mod == 0) ? _mod : _number % _mod; }

		public static double Smooth(double _percent) { return (Meth.Cos(Meth.Limit(0, _percent, 1) * Meth.Tau / 2) * -1 + 1) / 2; }
		public static double RSmooth(double _percent) { return 1 - Smooth(_percent); }

		public static bool Kinda(double _number, double _equals, double _tolerance) { return _number <= _equals + _tolerance && _number >= _equals - _tolerance; }
	}
}