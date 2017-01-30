using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Bezier
	{
		public static Bunch<Vector> GetPoints(Bunch<Vector> _points, int _quality)
		{
			Bunch<Vector> @out = new Bunch<Vector>();
			for (int i = 0; i <= _quality; i++)
			{
				double f = i / (double)_quality;
				Bunch<Vector> ps = _points.Clone();
				while (ps.Count > 1)
				{
					Bunch<Vector> nps = new Bunch<Vector>();
					for (int x = 0; x < ps.Count - 1; x++)
						nps.Add(ps[x] + (ps[x + 1] - ps[x]) * f);
					ps = nps;
				}
				@out.Add(ps[0]);
			}
			return @out;
		}

		public static Vector GetSingle(Bunch<Vector> _points, double _progress)
		{
			Bunch<Vector> ps = _points.Clone();
			while (ps.Count > 1)
			{
				Bunch<Vector> nps = new Bunch<Vector>();
				for (int x = 0; x < ps.Count - 1; x++)
					nps.Add(ps[x] + (ps[x + 1] - ps[x]) * _progress);
				ps = nps;
			}
			return ps[0];
		}

		public static double GetLength(Bunch<Vector> _points, int _quality)
		{
			double @out = 0;
			Bunch<Vector> ps = GetPoints(_points, _quality);
			for (int i = 0; i < ps.LastIndex; i++)
				@out += (ps[i + 1] - ps[i]).Length;
			return @out;
		}
	}
}