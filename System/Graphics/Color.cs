using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public struct Color
	{
		//public byte R;
		//public byte G;
		//public byte B;
		//public byte A;

		//public double DR
		//{
		//	get { return this.R / 255.0; }
		//	set { this.R = (byte)(value * 255); }
		//}
		//public double DG
		//{
		//	get { return this.G / 255.0; }
		//	set { this.G = (byte)(value * 255); }
		//}
		//public double DB
		//{
		//	get { return this.B / 255.0; }
		//	set { this.B = (byte)(value * 255); }
		//}
		//public double DA
		//{
		//	get { return this.A / 255.0; }
		//	set { this.A = (byte)(value * 255); }
		//}

		public double DR;
		public double DG;
		public double DB;
		public double DA;

		public byte R
		{
			get { return (byte)(this.DR * 255); }
			set { this.DR = value / 255.0; }
		}
		public byte G
		{
			get { return (byte)(this.DG * 255); }
			set { this.DG = value / 255.0; }
		}
		public byte B
		{
			get { return (byte)(this.DB * 255); }
			set { this.DB = value / 255.0; }
		}
		public byte A
		{
			get { return (byte)(this.DA * 255); }
			set { this.DA = value / 255.0; }
		}


		public Color Reverse
		{
			get { return new Color((byte)(255 - R), (byte)(255 - B), (byte)(255 - G)); }
		}

		public static Color Red
		{
			get { return new Color(255, 0, 0); }
		}
		public static Color Green
		{
			get { return new Color(0, 255, 0); }
		}
		public static Color Blue
		{
			get { return new Color(0, 0, 255); }
		}
		public static Color Yellow
		{
			get { return new Color(255, 255, 0); }
		}
		public static Color Cyan
		{
			get { return new Color(0, 255, 255); }
		}
		public static Color Magenta
		{
			get { return new Color(255, 0, 255); }
		}
		public static Color White
		{
			get { return new Color(255); }
		}
		public static Color Black
		{
			get { return new Color(0); }
		}
		public static Color Transparent
		{
			get { return new Color(0, 0, 0, 0); }
		}
		public byte this[int _index]
		{
			get { return this.Bytes[_index]; }

			set
			{
				if (_index >= 0 && _index < 4)
				{
					if (_index == 0)
						this.R = value;
					if (_index == 1)
						this.G = value;
					if (_index == 2)
						this.B = value;
					if (_index == 3)
						this.A = value;
				}
				else
					throw new IndexOutOfRangeException();
			}
		}
		public byte[] Bytes
		{
			get { return (new byte[] { this.R, this.G, this.B, this.A }); }

			set
			{
				this.R = value[0];
				this.G = value[1];
				this.B = value[2];
				this.A = (value.Length == 4) ? value[3] : (byte)255;
			}
		}
		public double Hue
		{
			get
			{
				double max = Meth.Max(this.DR, this.DG, this.DB);
				double min = Meth.Min(this.DR, this.DG, this.DB);
				double c = max - min;

				if (this.DR == max)
					return (this.DG - this.DB) / c % 6;
				if (this.DG == max)
					return (this.DB - this.DR) / c + 2;
				return (this.DR - this.DG) / c + 4;
			}
		}
		public double Saturation
		{
			get
			{
				if (this.Value == 0)
					return 0;
				else
					return this.Chroma / this.Value;
			}
		}
		public double Value
		{
			get { return Meth.Max(this.DR, this.DG, this.DB); }
		}
		public double Chroma
		{
			get { return Meth.Max(this.DR, this.DG, this.DB) - Meth.Min(this.DR, this.DG, this.DB); }
		}
		public static Color Random
		{
			get { return new Color(Meth.RandomByte, Meth.RandomByte, Meth.RandomByte); }
		}
		public double PerceivedBrightness
		{
			get { return Meth.Root(0.299 * this.DR * this.DR + 0.587 * this.DG * this.DG + 0.114 * this.DB * this.DB); }
		}
		//public double PerceivedBrightnessAlt
		//{
		//	get { return 0.2126 * this.DR + 0.7152 * this.DG + 0.0722 * this.DB; }
		//}

		public Color(byte _brightness)
		{
			this.DR = _brightness / 255.0;
			this.DG = _brightness / 255.0;
			this.DB = _brightness / 255.0;
			this.DA = 1;

			//this.R = _brightness;
			//this.G = _brightness;
			//this.B = _brightness;
			//this.A = 255;
		}

		public Color(byte _r, byte _g, byte _b, byte _a = 255)
		{
			this.DR = _r / 255.0;
			this.DG = _g / 255.0;
			this.DB = _b / 255.0;
			this.DA = _a / 255.0;

			//this.R = _r;
			//this.G = _g;
			//this.B = _b;
			//this.A = _a;
		}

		public Color(byte[] _bytes)
		{
			this.DR = _bytes[0] / 255.0;
			this.DG = _bytes[1] / 255.0;
			this.DB = _bytes[2] / 255.0;
			this.DA = (_bytes.Length == 4) ? _bytes[3] / 255.0 : 1;

			//this.R = _bytes[0];
			//this.G = _bytes[1];
			//this.B = _bytes[2];
			//this.A = (_bytes.Length == 4) ? _bytes[3] : (byte)255;
		}

		internal Color(bool _double, double _r, double _g, double _b, double _a = 1)
		{
			this.DR = _r;
			this.DG = _g;
			this.DB = _b;
			this.DA = _a;
		}

		public override string ToString()
		{
			if (this.A != 255)
				return "Color { " + this.R.ToString() + "; " + this.G.ToString() + "; " + this.B.ToString() + "; " + this.A.ToString() + "; }";
			else
				return "Color { " + this.R.ToString() + "; " + this.G.ToString() + "; " + this.B.ToString() + "; }";
		}

		public Vector ToVector() => new Vector(this.DR, this.DG, this.DB);

		public Color NormalizePerceivedBrightness(double _brightness) => this * (_brightness / this.PerceivedBrightness);



		public static Color Mix(params Color[] _colors)
		{
			double[] vs = new double[4];
			foreach (Color color in _colors)
			{
				vs[0] += (double)color.R / _colors.Length;
				vs[1] += (double)color.G / _colors.Length;
				vs[2] += (double)color.B / _colors.Length;
				vs[3] += (double)color.A / _colors.Length;
			}
			return new Color((byte)vs[0], (byte)vs[1], (byte)vs[2], (byte)vs[2]);
		}

		public static Color FromHsv(double _hue, double _saturation, double _value)
		{
			double c = _value * _saturation;
			double h = Meth.RMod(_hue, 1) * 6;
			double x = c * (1 - Meth.Abs(h % 2 - 1));

			double r = 0, g = 0, b = 0;
			if (0 <= h && h < 1)
			{
				r = c;
				g = x;
			}
			else if (1 <= h && h < 2)
			{
				r = x;
				g = c;
			}
			else if (2 <= h && h < 3)
			{
				g = c;
				b = x;
			}
			else if (3 <= h && h < 4)
			{
				g = x;
				b = c;
			}
			else if (4 <= h && h < 5)
			{
				b = c;
				r = x;
			}
			else if (5 <= h && h < 6)
			{
				b = x;
				r = c;
			}

			double m = _value - c;

			return new Color((byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b + m) * 255));
		}

		public static Color Parse(string _string)
		{
			string[] bytes = _string.Split(',');
			bytes = bytes.Select(item => (item[item.Length - 1] == ' ') ? item.Substring(0, item.Length - 1) : item).ToArray();
			if (bytes.Length == 1)
			{
				byte @out;
				if (byte.TryParse(bytes[0], out @out))
					return new Color(@out);
				else
				{
					switch (_string.ToLower())
					{
						case "red":
							return Color.Red;
						case "green":
							return Color.Green;
						case "blue":
							return Color.Blue;
						case "yellow":
							return Color.Yellow;
						case "cyan":
							return Color.Cyan;
						case "magenta":
							return Color.Magenta;
						case "white":
							return Color.White;
						case "black":
							return Color.Black;
						case "transparent":
							return Color.Transparent;
						default:
							throw new Exception();
					}
				}
			}
			else if (bytes.Length == 3)
				return new Color(byte.Parse(bytes[0]), byte.Parse(bytes[1]), byte.Parse(bytes[2]));
			else if (bytes.Length == 4)
				return new Color(byte.Parse(bytes[0]), byte.Parse(bytes[1]), byte.Parse(bytes[2]), byte.Parse(bytes[3]));
			else
				throw new Exception();
		}

		public static Color operator *(Color _one, double _two) => new Color(true, _one.DR * _two, _one.DG * _two, _one.DB * _two, _one.DA);
		public static Color operator *(Color _one, Color _two) => new Color(true, _one.DR * _two.DR, _one.DG * _two.DG, _one.DB * _two.DB, _one.DA * _two.DA);
		//{
		//	double r1 = _one.R / 255.0, r2 = _two.R / 255.0;
		//	double g1 = _one.G / 255.0, g2 = _two.G / 255.0;
		//	double b1 = _one.B / 255.0, b2 = _two.B / 255.0;
		//	double a1 = _one.A / 255.0, a2 = _two.A / 255.0;
		//	return new Color((byte)(r1 * r2 * 255), (byte)(g1 * g2 * 255), (byte)(b1 * b2 * 255), (byte)(a1 * a2 * 255));
		//}

		public static Color operator /(Color _one, double _two) => new Color(true, _one.DR / _two, _one.DG / _two, _one.DB / _two, _one.DA);
		public static Color operator ^(Color _one, int _two) => new Color(true, _one.DR, _one.DG, _one.DB, _two / 255.0);
		public static Color operator +(Color _one, Color _two) => new Color(true, _one.DR + _two.DR, _one.DG + _two.DG, _one.DB + _two.DB, _one.DA);
		public static Color operator -(Color _one, Color _two) => new Color(true, _one.DR - _two.DR, _one.DG - _two.DG, _one.DB - _two.DB, _one.DA);
		
		public static bool operator ==(Color _one, Color _two) => _one.R == _two.R && _one.G == _two.G && _one.B == _two.B && _one.A == _two.A;
		public static bool operator !=(Color _one, Color _two) => !(_one == _two);

		public override bool Equals(object _object) => _object is Color && (this == (Color)_object);
	}
}