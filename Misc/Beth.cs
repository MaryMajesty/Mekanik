using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public static class Beth
	{
		public static byte[] ToEndian(int _number)
		{
			Bunch<byte> @out = new Bunch<byte>();
			while (_number > 0)
			{
				byte b = (byte)(_number % 256);
				@out.Add(b);
				_number -= b;
				_number /= 256;
			}

			return @out.ToArray();
		}

		public static byte[] ToEndian(int _number, int _bytes)
		{
			byte[] bs = ToEndian(_number);
			Bunch<byte> @out = new Bunch<byte>() { bs };
			while (@out.Count < _bytes)
				@out.Add(0);
			return @out.ToArray();
		}

		public static long FromEndian(byte[] _bytes)
		{
			long @out = 0;
			long f = 1;
			for (int i = 0; i < _bytes.Length; i++)
			{
				@out += _bytes[i] * f;
				f *= 256;
			}
			return @out;
		}

		public static T[] Override<T>(this T[] @this, int _index, T[] _data)
		{
			T[] @out = new T[@this.Length];
			for (int i = 0; i < @this.Length; i++)
			{
				if (i < _index || i >= _index + _data.Length)
					@out[i] = @this[i];
				else
					@out[i] = _data[i - _index];
			}
			return @out;
		}
	}
}