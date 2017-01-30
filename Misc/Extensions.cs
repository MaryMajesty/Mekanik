using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;

namespace Mekanik
{
	public static class Extensions
	{
		//internal static Styles ToSfml(this WindowStyle _style) { return (new Dictionary<WindowStyle, Styles>() { { WindowStyle.None, Styles.None }, { WindowStyle.Default, Styles.Titlebar }, { WindowStyle.Fullscreen, Styles.Fullscreen } })[_style]; }

		//public static List<T> GetTypes<T>(this IList @this)
		//{
		//	List<T> @out = new List<T>();
		//	foreach (object o in @this)
		//	{
		//		if (o.GetType() == typeof(T) || o.GetType().IsSubclassOf(typeof(T)))
		//			@out.Add((T)o);
		//	}
		//	return @out;
		//}

		//public static List<T> GetStrictTypes<T>(this IList @this)
		//{
		//	List<T> @out = new List<T>();
		//	foreach (object o in @this)
		//	{
		//		if (o.GetType() == typeof(T))
		//			@out.Add((T)o);
		//	}
		//	return @out;
		//}

		public static string[] Split(this string @this, string _splitter) { return @this.Split(new string[] { _splitter }, StringSplitOptions.None); }
		
		internal static Key ToKey(string _string)
		{
			for (int i = 0; i < (int)Key.KeyCount; i++)
			{
				Key k = (Key)i;
				if (k.ToString() == _string)
					return k;
			}
			return Key.Unknown;
		}

		internal static bool IsMouse(this Key @this)
		{
			return (int)@this > (int)Key.KeyCount;
		}
		
		public static T[] Add<T>(this T[] @this, T _object)
		{
			T[] @out = new T[@this.Length + 1];
			for (int i = 0; i < @this.Length; i++)
				@out[i] = @this[i];
			@out[@this.Length] = _object;
			return @out;
		}
		
		//internal static TextStyle ToMekanik(this SFML.Graphics.Text.Styles @this) { return (TextStyle)((int)@this); }
		////internal static SFML.Graphics.Text.Styles ToSfml(this TextStyle @this) { return (SFML.Graphics.Text.Styles)((int)@this); }
		//internal static SFML.Graphics.Text.Styles ToSfml(this TextStyle @this)
		//{
		//	SFML.Graphics.Text.Styles @out = SFML.Graphics.Text.Styles.Regular;
		//	for (int i = 0; i < 4; i++)
		//	{
		//		if (@this.HasFlag((TextStyle)i))
		//			@out |= (SFML.Graphics.Text.Styles)i;
		//	}
		//	return @out;
		//}

		public static Bunch<T> ToBunch<T>(this System.Linq.IOrderedEnumerable<T> @this)
		{
			return new Bunch<T>() { @this.ToArray() };
		}

		//internal static double ToFrames(this SFML.System.Time @this, Game _game)
		//{
		//	return @this.AsSeconds() * _game.FramerateLimit;
		//}

		public static T[] SubArray<T>(this T[] @this, int _start, int _length)
		{
			T[] @out = new T[_length];
			for (int x = _start; x < _start + _length; x++)
				@out[x - _start] = @this[x];
			return @out;
		}

		public static T[] SubArray<T>(this T[] @this, int _start) { return @this.SubArray(_start, @this.Length - _start); }

		public static T[,] SubArray<T>(this T[,] @this, int _x, int _y, int _width, int _height)
		{
			T[,] @out = new T[_width, _height];
			for (int x = _x; x < _x + _width; x++)
			{
				for (int y = _y; y < _y + _height; y++)
					@out[x - _x, y - _y] = @this[x, y];
			}
			return @out;
		}

		public static double GetAlphabetIndex(this string @this)
		{
			double @out = 0;
			double n = 1;
			foreach (char c in @this)
			{
				@out += (ushort)c / (double)ushort.MaxValue / Meth.Pow(2, n);
				n++;
			}
			return @out;
		}

		//public static byte[] GetBytes(this System.Drawing.Bitmap @this, System.Drawing.Imaging.ImageFormat _format)
		//{
		//	System.IO.MemoryStream s = new System.IO.MemoryStream();
		//	@this.Save(s, _format);
		//	return s.ToArray();
		//}

		public static T2[,] To<T1, T2>(this T1[,] @this, Func<T1, T2> _func) where T2 : T1
		{
			int width = @this.GetLength(0);
			int height = @this.GetLength(1);

			T2[,] @out = new T2[width, height];

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
					@out[x, y] = _func(@this[x, y]);
			}

			return @out;
		}

		//public static bool Contains(this Bunch<Rect> @this, Vector _vector)
		//{
		//	foreach (Rect rect in @this)
		//	{
		//		if (rect.Contains(_vector))
		//			return true;
		//	}
		//	return false;
		//}

		//public static bool Contains(this Bunch<Collider> @this, Vector _vector)
		//{
		//	return @this.Select(item => item.Rect).Contains(_vector);
		//}

		//public static bool IntersectsWith(this Bunch<Rect> @this, Bunch<Rect> _rects)
		//{
		//	foreach (Rect rect1 in _rects)
		//	{
		//		foreach (Rect rect2 in @this)
		//		{
		//			if (rect2.IntersectsWith(rect1))
		//				return true;
		//		}
		//	}
		//	return false;
		//}

		//public static bool IntersectsWith(this Bunch<Collider> @this, Bunch<Collider> _colliders) { return @this.Select(item => item.Rect).IntersectsWith(_colliders.Select(item => item.Rect)); }

		//public static bool Contains(this Bunch<Rect> @this, Bunch<Rect> _rects)
		//{
		//	foreach (Rect rect1 in _rects)
		//	{
		//		bool contains = false;
		//		foreach (Rect rect2 in @this)
		//		{
		//			if (rect2.Contains(rect1))
		//			{
		//				contains = true;
		//				break;
		//			}
		//		}
		//		if (!contains)
		//			return false;
		//	}
		//	return true;
		//}

		//public static bool Contains(this Bunch<Collider> @this, Bunch<Collider> _colliders)
		//{
		//	return @this.Select(item => item.Rect).Contains(_colliders.Select(item => item.Rect));
		//}

		//public static bool Contains(this Bunch<Area> @this, Vector _point)
		//{
		//	return @this.WhereFirst(item => item.Contains(_point)) != null;
		//}

		//public static bool Contains(this Bunch<Rect> @this, Bunch<Rect> _rects)
		//{
		//	foreach (Rect rect1 in _rects)
		//	{
		//		bool contains = false;
		//		foreach (Rect rect2 in @this)
		//		{
		//			if (rect2.Contains(rect1))
		//			{
		//				contains = true;
		//				break;
		//			}
		//		}
		//		if (!contains)
		//			return false;
		//	}
		//	return true;
		//}



		public static Rect GetRect(this Bunch<Rect> @this)
		{
			Vector topleft = new Vector(double.MaxValue, double.MaxValue);
			Vector botright = new Vector(double.MinValue, double.MinValue);
			foreach (Rect rect in @this)
			{
				if (rect.X < topleft.X)
					topleft.X = rect.X;
				if (rect.Y < topleft.Y)
					topleft.Y = rect.Y;

				if (rect.X + rect.Width > botright.X)
					botright.X = rect.X + rect.Width;
				if (rect.Y + rect.Height > botright.Y)
					botright.Y = rect.Y + rect.Height;
			}
			return new Rect(topleft, botright - topleft);
		}

		//public static Rect GetRect(this Bunch<Collider> @this)
		//{
		//	return @this.Select(item => item.Rect).GetRect();
		//}

		public static MekaItem Convert(this MekaItem @this)
		{
			if (@this.HasChildren)
			{
				MekaItem @out = new MekaItem(@this.Name, new List<MekaItem>());

				foreach (MekaItem child in @this.Children)
				{
					if (child.Name[0] == '#')
					{
						foreach (MekaItem c in child.Convert().Children)
							@out.Children.Add(c);
					}
					else
						@out.Children.Add(child.Convert());
				}

				return @out;
			}
			else
				return @this;
		}

		public static bool Any<T>(this T[,] @this, Func<T, bool> _func)
		{
			foreach (T t in @this)
			{
				if (_func(t))
					return true;
			}
			return false;
		}

		public static TV[,] Select<T, TV>(this T[,] @this, Func<T, TV> _func)
		{
			TV[,] @out = new TV[@this.GetLength(0), @this.GetLength(1)];
			for (int x = 0; x < @this.GetLength(0); x++)
			{
				for (int y = 0; y < @this.GetLength(1); y++)
					@out[x, y] = _func(@this[x, y]);
			}
			return @out;
		}

		public static Bunch<T> ToBunch<T>(this IEnumerable<T> @this)
		{
			Bunch<T> @out = new Bunch<T>();
			foreach (T t in @this)
				@out.Add(t);
			return @out;
		}

		public static object To(this string @this, Type _type)
		{
			if (_type == typeof(string))
				return @this;
			else if (_type.IsEnum)
				return Enum.Parse(_type, @this);
			else
			{
				//try
				//{
				//if (_type == typeof(double))
				//{
				//	System.Reflection.MethodInfo parse = _type.GetMethod("Parse", new Type[] { typeof(string), typeof(IFormatProvider) });
				//	return parse.Invoke(null, new object[] { @this, new System.Globalization.CultureInfo("en-US") });
				//}
				//else
					return _type.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { @this });
				//}
				//catch { return _type.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { @this }); }
			}
		}

		public static T To<T>(this string @this) { return (T)@this.To(typeof(T)); }

		//public static string ToMurica(this object @this)
		//{
		//	System.Reflection.MethodInfo tostring = @this.GetType().GetMethod("ToString", new Type[] { typeof(IFormatProvider) });
		//	return (string)tostring.Invoke(@this, new object[] { new System.Globalization.CultureInfo("en-US") });
		//}

		//public static T To<T>(this MekaItem @this) { return @this.Content.To<T>(); }

		//public static Key ToMekanik(this MouseButtons @this)
		//{
		//	switch (@this)
		//	{
		//		case MouseButtons.Left:
		//			return Key.MouseLeft;
		//		case MouseButtons.Middle:
		//			return Key.MouseMiddle;
		//		case MouseButtons.Right:
		//			return Key.MouseRight;
		//		case MouseButtons.XButton1:
		//			return Key.MouseForward;
		//		case MouseButtons.XButton2:
		//			return Key.MouseBackward;
		//	}
		//	throw new Exception("Unknown key.");
		//}

		public static Key ToMekanik(this OpenTK.Input.Key @this) => (Key)((int)@this);

		//public static Point ToMekanik(this System.Drawing.Point @this) => new Point(@this.X, @this.Y);

		//public static System.Drawing.Point ToOpenTK(this Point @this) => new System.Drawing.Point(@this.X, @this.Y);

		public static double ToFullTau(this double _rot)
		{
			if (_rot < 0)
				return Meth.Tau + _rot;
			else
				return _rot;
		}

		public static double ToHalfTau(this double _rot)
		{
			if (_rot > Meth.Tau / 2)
				return -(Meth.Tau - _rot);
			else
				return _rot;
		}

		//public static object Create(this Type @this, params object[] _parameters)
		//{
		//	return @this.GetConstructor(_parameters.Select(item => item.GetType()).ToArray()).Invoke(_parameters);
		//}

		public static string[] ToLines(this string @this) { return @this.Split('\n'); }

		public static Point GetCharPos(this string @this, int _char)
		{
			int pos = 0;
			int line = 0;
			for (int i = 0; i < _char; i++)
			{
				//if (i == @this.Length - 1)
				//{
				//	pos--;
				//	break;
				//}
				if (@this[i] == '\n')
				{
					pos = i + 1;
					line++;
				}
			}
			return new Point(_char - pos, line);
		}

		public static string Cut(this string @this, int _start, int _end) { return @this.Substring(_start, _end - _start + 1); }

		public static Color ToColor(this string @this)
		{
			string abc = "abcdefghijklmnopqrstuvwxyz";
			bool black = true;
			double rot = 0;

			for (int i = 0; i < @this.Length; i++)
			{
				char c = @this.ToLower()[i];
				if (abc.Contains(c))
				{
					black = false;
					rot += (abc.IndexOf(c) - 13) / 26.0 * Meth.Tau / Meth.Pow(5, i);
				}
			}

			if (black)
				return Color.Black;
			else
				return Color.FromHsv(rot, 1, 1);
		}

		public static Key ToMekanik(this OpenTK.Input.MouseButton @this)
		{
			if (@this == OpenTK.Input.MouseButton.Left)
				return Key.MouseLeft;
			else if (@this == OpenTK.Input.MouseButton.Middle)
				return Key.MouseMiddle;
			else if (@this == OpenTK.Input.MouseButton.Right)
				return Key.MouseRight;
			else
				throw new Exception();
		}

		public static OpenTK.Graphics.Color4 ToOpenTK(this Color @this) => new OpenTK.Graphics.Color4(@this.R, @this.G, @this.B, @this.A);

		//public static Point ToMekanik(this System.Drawing.Size @this) => new Point(@this.Width, @this.Height);

		public static OpenTK.Vector2 ToOpenTK(this Vector @this) => new OpenTK.Vector2((float)@this.X, (float)@this.Y);
	}
}