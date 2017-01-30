using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mekanik;

namespace Mekanik
{
	public class FrameCurve
	{
		public Bunch<Vector> Points;

		private Bunch<Vector> _InitialPoints;
		public Bunch<Vector> InitialPoints
		{
			get { return this._InitialPoints; }
		}

		public FrameCurve(Bunch<Vector> _points) { this.Update(_points); }

		public FrameCurve(double _v = 1) : this(new Bunch<Vector>() { new Vector(0, _v), new Vector(0.25, _v), new Vector(0.5, _v), new Vector(0.75, _v), new Vector(1, _v) }) { }

		public double GetValue(double _pos)
		{
			Vector v = Points[0];
			for (int i = 1; i < Points.Count; i++)
			{
				Vector p = Points[i];
				if (_pos >= v.X && _pos <= p.X)
				{
					double f = (_pos - v.X) / (p.X - v.X);
					return v.Y + (p.Y - v.Y) * f;
				}
				else
					v = p;
			}
			
			return 0;
		}

		public void Update(Bunch<Vector> _points)
		{
			this._InitialPoints = _points;
			this.Points = new Bunch<Vector>();
			for (int i = 0; i <= 20; i++)
				this.Points.Add(Bezier.GetSingle(_points, i / 20.0));
		}
	}
}