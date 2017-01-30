using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class LoadingCircle : Entity
	{
		VertexArray OuterRing;
		VertexArray InnerRing = new VertexArray(VertexArrayType.Quads);
		VertexArray Lines = new VertexArray(VertexArrayType.Lines) { Color = Color.Black };
		double RealProgress;
		double TargetProgress;
		double LineProgress;
		public bool Started;
		bool Finished;
		Text Text;
		public bool ProgressIndefinitely;

		public double Progress
		{
			get { return this.TargetProgress; }

			set
			{
				this.TargetProgress = value;
				if (value == 1)
				{
					this.Started = true;
					this.Finished = true;
				}
			}
		}

		public LoadingCircle(string _text = "")
		{
			Bunch<Vector> vs = new Bunch<Vector>();
			for (int i = 0; i <= 20; i++)
				vs.Add(Vector.FromAngle(i / 20.0 * Meth.Tau, 8));
			this.OuterRing = Line.DrawMultiple(vs, Color.Black, 1);
			this.Graphics.Add(this.OuterRing);

			this.Graphics.Add(this.InnerRing, this.Lines);

			this.Text = new Text(FontBase.Consolas) { Position = new Vector(10, -1), Origin = new Vector(0, 0.5), CharSize = 14, Color = Color.Black, Content = _text };
			this.Graphics.Add(this.Text);

			this.Offset = 10;
		}

		public override void Update()
		{
			if (this.ProgressIndefinitely)
				this.Progress += (1 - this.Progress) / 200;

			if (this.Started)
			{
				if (!this.Finished)
				{
					this.RealProgress += (this.TargetProgress - this.RealProgress) * 0.1;
					//if (this.TargetProgress == 1)
					//{
					//	this.Finished = true;
					//	this.Kill();
					//}

					if ((this.Runtime - 20) % 120 < 60)
						this.LineProgress = 0;
					else
						this.LineProgress = Meth.Smooth(((this.Runtime - 20) % 120 - 60) / 60.0);
				}
				else
				{
					this.RealProgress *= 0.9;
					this.LineProgress -= 1 / 10.0;

					this.Graphics.Remove(this.OuterRing);
					Bunch<Vector> vs = new Bunch<Vector>();
					for (int i = 0; i <= 20; i++)
						vs.Add(Vector.FromAngle(i / 20.0 * Meth.Tau, 8 * Meth.Root(this.RealProgress)));
					this.OuterRing = Line.DrawMultiple(vs, Color.Black, 1);
					this.Graphics.Add(this.OuterRing);

					this.Text.Scale = new Vector(Meth.Square(this.RealProgress), this.RealProgress);
					this.Text.Position.X = this.RealProgress * 10;

					if (this.RealProgress < 0.001)
						this.Kill();
				}
			}
			else
			{
				this.LineProgress += (1 - this.LineProgress) * 0.1;

				this.RealProgress += (this.TargetProgress - this.RealProgress) * 0.1;
				this.RealProgress = Meth.Min(this.LineProgress, this.RealProgress);

				this.Graphics.Remove(this.OuterRing);
				Bunch<Vector> vs = new Bunch<Vector>();
				for (int i = 0; i <= 20; i++)
					vs.Add(Vector.FromAngle(i / 20.0 * Meth.Tau, 8 * this.LineProgress));
				this.OuterRing = Line.DrawMultiple(vs, Color.Black, 1);
				this.Graphics.Add(this.OuterRing);

				this.Text.Scale = new Vector(Meth.Square(this.LineProgress), this.LineProgress);
				this.Text.Position.X = this.LineProgress * 10;

				if (this.LineProgress > 0.99)
				{
					this.LineProgress = 1;
					this.Started = true;
				}
			}

			this.Graphics.Remove(this.InnerRing);

			Bunch<Vector> ps = new Bunch<Vector>();
			for (int i = 0; i <= 20; i++)
				ps.Add(Vector.FromAngle(i / 20.0 * Meth.Tau, 8 * this.RealProgress));
			this.InnerRing = Line.DrawMultiple(ps, Color.Black, 1);
			this.Graphics.Add(this.InnerRing);

			this.Lines.Vertices.Clear();
			this.Lines.Add(0);
			this.Lines.Add(Vector.FromAngle(this.LineProgress * Meth.Tau, 8 * this.RealProgress));
			this.Lines.Add(Vector.FromAngle(this.LineProgress * -Meth.Tau - Meth.Tau / 2, 8 * this.RealProgress));
			this.Lines.Add(Vector.FromAngle(this.LineProgress * -Meth.Tau - Meth.Tau / 2, this.Finished ? 8 * Meth.Root(this.RealProgress) : 8));
		}
	}
}