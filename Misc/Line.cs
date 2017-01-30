using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public static class Line
	{
		public static Bunch<Point> Trace(Point _start, Point _end)
		{
			Bunch<Point> @out = new Bunch<Point>();

			Point dif = _end - _start;
			if (dif.X != 0 || dif.Y != 0)
			{
				if (Meth.Abs(dif.X) > Meth.Abs(dif.Y))
				{
					for (int x = 0; x <= Meth.Abs(dif.X); x++)
					{
						int nx = _start.X + x * Meth.Sign(dif.X);
						int ny = Meth.Round(_start.Y + dif.Y * (x * Math.Sign(dif.X) / (float)dif.X));
						@out.Add(new Point(nx, ny));
					}
				}
				else
				{
					for (int y = 0; y <= Meth.Abs(dif.Y); y++)
					{
						int nx = Meth.Round(_start.X + dif.X * (y * Meth.Sign(dif.Y) / (float)dif.Y));
						int ny = _start.Y + y * Meth.Sign(dif.Y);
						@out.Add(new Point(nx, ny));
					}
				}
			}
			else
				@out.Add(_start);

			return @out;
		}

		public static VertexArray Draw(Vector _start, Vector _end, double _thickness, Color _color)
		{
			VertexArray @out = new VertexArray(VertexArrayType.Quads);

			double a = (_end - _start).Angle;
			for (int i = -1; i <= 1; i += 2)
				@out.Add(_start + Vector.FromAngle(a + Meth.Tau / 4 * i, _thickness / 2), _color);
			for (int i = 1; i >= -1; i -= 2)
				@out.Add(_end + Vector.FromAngle(a + Meth.Tau / 4 * i, _thickness / 2), _color);

			return @out;
		}

		public static VertexArray DrawMultiple(Vector[] _points, double _thickness, FrameCurve _curve, FrameCurve _r, FrameCurve _g, FrameCurve _b)
		{
			Bunch<Vector>[] npoints = new Bunch<Vector>[] { new Bunch<Vector>(), new Bunch<Vector>() };

			for (int i = 0; i < _points.Length - 1; i++)
			{
				double a = (_points[i + 1] - _points[i]).Angle;
				for (int x = 0; x < 2; x++)
				{
					for (int y = 0; y < 2; y++)
						npoints[y].Add(_points[i + x] + Vector.FromAngle(a + Meth.Tau / 4 + Meth.Tau / 2 * y, _curve.GetValue((i + x) / (double)(_points.Length - 1)) / 2 * _thickness));
				}
			}
			
			//VertexArray v = new VertexArray(VertexArrayType.Quads);
			//for (int i = 0; i < npoints[0].LastIndex; i++)
			//{
			//	Color c1 = GetColor(i / (double)npoints[0].LastIndex, _r, _g, _b);
			//	Color c2 = GetColor((i + 1) / (double)npoints[0].LastIndex, _r, _g, _b);

			//	v.Add(npoints[0][i], c1);
			//	v.Add(npoints[1][i], c1);
			//	v.Add(npoints[1][i + 1], c2);
			//	v.Add(npoints[0][i + 1], c2);
			//}

			VertexArray v = new VertexArray(VertexArrayType.Triangles) { Color = Color.Black };
			for (int i = 0; i < npoints[0].LastIndex; i++)
			{
				Color c1 = GetColor(i / (double)npoints[0].LastIndex, _r, _g, _b);
				Color c2 = GetColor((i + 1) / (double)npoints[0].LastIndex, _r, _g, _b);

				v.Add(npoints[0][i], c1);
				v.Add(npoints[1][i], c1);
				v.Add(npoints[1][i + 1], c2);

				v.Add(npoints[1][i + 1], c2);
				v.Add(npoints[0][i + 1], c2);
				v.Add(npoints[0][i], c1);
			}

			if (_points.Length > 1)
			{
				for (int n = 0; n < 2; n++)
				{
					Vector p = _points[n * (_points.Length - 1)];
					int q = Meth.Up(_points.Length / 2.0);

					Color c = GetColor(n, _r, _g, _b);
					double t = _thickness * _curve.GetValue(n) / 2;

					double a = ((n == 0) ? (_points[0] - _points[1]).Angle : (_points[_points.Length - 1] - _points[_points.Length - 2]).Angle) - Meth.Tau / 4;

					for (int i = 0; i < q; i++)
					{
						v.Add(p, c);
						v.Add(p + Vector.FromAngle(a + i / (double)q * Meth.Tau / 2, t), c);
						v.Add(p + Vector.FromAngle(a + (i + 1) / (double)q * Meth.Tau / 2, t), c);
					}
				}
			}

			return v;
		}

		public static Color GetColor(double _p, FrameCurve _r, FrameCurve _g, FrameCurve _b)
		{
			Color @out = Color.Black;
			@out.DR = _r.GetValue(_p);
			@out.DG = _g.GetValue(_p);
			@out.DB = _b.GetValue(_p);
			return @out;
		}

		public static VertexArray DrawMultiple(Vector[] _points, Color _color, double _thickness) { return DrawMultiple(_points, _thickness, new FrameCurve(1), new FrameCurve(_color.DR), new FrameCurve(_color.DG), new FrameCurve(_color.DB)); }

		public static Vector GetIntersection(Vector _start1, Vector _end1, Vector _start2, Vector _end2)
		{
			double a1 = Meth.Tan((_end1 - _start1).Angle);
			double a2 = Meth.Tan((_end2 - _start2).Angle);

			if (a1 != a2)
			{
				double x = (a1 * _start1.X - a2 * _start2.X - _start1.Y + _start2.Y) / (a1 - a2);

				Vector v = new Vector(x, _start1.Y + (x - _start1.X) * a1);
				if ((v - _start1).Length <= (_end1 - _start1).Length)
					return v;
			}

			return (_start1 + _start2 + _end1 + _end2) / 4;
		}
	}
	//public struct Line : IEnumerable<Point>
	//{
	//	public Point Start;
	//	public Point End;

	//	public Line(Point _start, Point _end)
	//	{
	//		this.Start = _start;
	//		this.End = _end;
	//	}

	//	public Line(int _startx, int _starty, int _endx, int _endy)
	//	{
	//		this.Start = new Point(_startx, _starty);
	//		this.End = new Point(_endx, _endy);
	//	}

	//	public IEnumerator<Point> GetEnumerator()
	//	{
	//		Point dif = End - Start;
	//		if (dif.X != 0 || dif.Y != 0)
	//		{
	//			if (Meth.Abs(dif.X) > Meth.Abs(dif.Y))
	//			{
	//				for (int x = 0; x <= Meth.Abs(dif.X); x++)
	//				{
	//					int nx = Start.X + x * Meth.Sign(dif.X);
	//					int ny = Meth.Round(Start.Y + dif.Y * (x * Math.Sign(dif.X) / (float)dif.X));
	//					yield return new Point(nx, ny);
	//				}
	//			}
	//			else
	//			{
	//				for (int y = 0; y <= Meth.Abs(dif.Y); y++)
	//				{
	//					int nx = Meth.Round(Start.X + dif.X * (y * Meth.Sign(dif.Y) / (float)dif.Y));
	//					int ny = Start.Y + y * Meth.Sign(dif.Y);
	//					yield return new Point(nx, ny);
	//				}
	//			}
	//		}
	//		else
	//			yield return Start;
	//	}

	//	IEnumerator IEnumerable.GetEnumerator() { return (IEnumerator)GetEnumerator(); }
	//}
}