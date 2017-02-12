using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Logo : LoadingScreen
	{
		private Gear _Middle;
		private Rack[] _Sides;
		private Rectangle[] _Cutoffs;
		private Text[] _Letters = new Text[6];
		private bool _Done;
		private int _DoneProgress0;
		private int _DoneProgress1;
		private int _DoneProgress2;
		private Bunch<ILoadable> _Loadables;
		private bool _DoneLoading;

		public Logo(params ILoadable[] _loadables)
		{
			this.Skippable = false;
			this._Loadables = _loadables;

			if (_loadables.Length == 0)
			{
				this._DoneLoading = true;
				this.Skippable = true;
			}
		}

		public override void OnInitialization()
		{
			base.OnInitialization();

			this.Parent.Background = Color.Black;
			
			this.Scale = Meth.Min(this.Parent.Resolution.X / 960.0, this.Parent.Resolution.Y / 480.0);

			this.Children.Add(this._Sides = new Rack[] { new Rack(this.Parent.FixedResolution) { Position = new Vector(-75, -75) }, new Rack(this.Parent.FixedResolution) { Position = new Vector(75, -75), Scale = new Vector(-1, 1) } });
			this.Children.Add(this._Middle = new Gear(this.Parent.FixedResolution) { X = -1 });

			this.Graphics.Add(this._Cutoffs = new Rectangle[] { new Rectangle(-80, 70, 160, 200) { Color = Color.Black, Z = 1 }, new Rectangle(-80, -70, 160, -200) { Color = Color.Black, Z = 1 } });

			FontBase f = FontBase.Consolas;
			f.CharSize = 150;
			for (int i = 0; i < 6; i++)
				this.Graphics.Add(this._Letters[i] = new Text(f) { Content = "ekanik"[i].ToString(), TextColor = Color.Black, Position = new Vector(50, -5) + new Vector(70, 0) * i });

			this.Position = this.Parent.Resolution / 2;
		}

		public override void Update()
		{
			base.Update();

			this._Sides[0].Y = -75 - Meth.Up((this._Middle.Rotation / Meth.Tau * 10 * 30 + 5) % 30 - 5);
			this._Sides[1].Y = -75 + Meth.Up((this._Middle.Rotation / Meth.Tau * 10 * 30 + 25) % 30 - 25);

			DateTime start = DateTime.Now;
			while ((DateTime.Now - start).TotalMilliseconds < 3 && this._Loadables.Count > 0)
			{
				if (this._Loadables[0].FinishedLoading)
				{
					this._Loadables.RemoveAt(0);
					if (this._Loadables.Count == 0)
					{
						this._DoneLoading = true;
						this.Skippable = true;
					}
				}
				else
					this._Loadables[0].LoadStep();
			}

			if (this.Runtime % 110 == 0 && this._DoneLoading)
			{
				this._Done = true;
				this._Middle.Done = true;
				this.Skippable = true;
			}

			if (this._Done && this._DoneProgress0 < 100)
			{
				this._DoneProgress0++;

				this._Cutoffs[0].Position.Y += 1;
				this._Cutoffs[1].Position.Y -= 0.5;

				this.Position = this.Parent.Resolution / 2 - (Point)(new Vector(220, 70) * Meth.Smooth(this._DoneProgress0 / 100.0) * this.Scale);
			}

			if (this._Done && this._DoneProgress0 >= 50 && this._DoneProgress1 < 80)
			{
				this._DoneProgress1++;

				for (int i = 0; i < 6; i++)
					this._Letters[i].TextColor = Color.White * Meth.Limit(0, Meth.Smooth(this._DoneProgress1 / 80.0) * 6 - i, 1);
			}

			if (this._DoneProgress1 >= 80 && this._DoneProgress1 < 120)
				this._DoneProgress1++;

			if (this._DoneProgress1 == 120)
			{
				this._DoneProgress2++;
				this.Color = Color.White * Meth.Smooth(1 - this._DoneProgress2 * 3 / 255.0);
				if (this._DoneProgress2 == 85)
					this.Kill();
			}
		}

		class Gear : Entity
		{
			public double TargetRotation;
			public double RotationSpeed;
			public bool Done;

			public Gear(bool _fixedres)
			{
				double[] ls = new double[] { 20, 20, 20, 15 };
				double[] fs = new double[] { 0.1, 0.15 };

				VertexArray lines0 = new VertexArray(VertexArrayType.LinesStrip) { Color = Color.White * 0.2, LockToGrid = _fixedres };
				VertexArray lines1 = new VertexArray(VertexArrayType.LinesStrip) { Color = Color.White * 0.2, LockToGrid = _fixedres };

				for (int i = 0; i < 10; i++)
				{
					Bunch<Vector> vs = new Bunch<Vector>();

					double a0 = Meth.Tau / 10 * i;
					double a1 = Meth.Tau / 10 * (i + 1);
					
					Action<Vector> add = v =>
						{
							vs.Add(v);
							lines1.Add(v);
						};

					for (int n = 0; n <= 2; n++)
						vs.Add(Vector.FromAngle(a1 - (a1 - a0) / 2 * n, ls[0]));
					for (int n = 2; n >= 0; n--)
						lines0.Add(Vector.FromAngle(a1 - (a1 - a0) / 2 * n, ls[0]));

					Vector va0 = Vector.FromAngle(a0, ls[0] + ls[1]);
					Vector va1 = Vector.FromAngle(a1, ls[0] + ls[1]);

					Vector dif = Vector.FromAngle(a1, ls[0] + ls[1]) - Vector.FromAngle(a0, ls[0] + ls[1]);

					for (int n = 0; n <= 2; n++)
						add(Vector.FromAngle(a0 + ((va0 + dif * fs[0]).Angle - a0) / 2 * n, ls[0] + ls[1]));

					for (int n = 0; n <= 3; n++)
						add(va0 + dif * fs[0] + dif * (n / 3.0) * fs[1] + Vector.FromAngle(dif.Angle - Meth.Tau / 4, Meth.Smooth(n / 3.0) * ls[3]));
					for (int n = 0; n <= 3; n++)
						add(va0 + dif * (1 - fs[0] - fs[1]) + dif * (n / 3.0) * fs[1] + Vector.FromAngle(dif.Angle - Meth.Tau / 4, Meth.Smooth((3 - n) / 3.0) * ls[3]));
					
					for (int n = 0; n <= 2; n++)
						add(Vector.FromAngle((va0 + dif * (1 - fs[0])).Angle + (a1 - (va0 + dif * (1 - fs[0])).Angle) / 2 * n, ls[0] + ls[1]));
					
					this.Graphics.Add(new VertexArray(VertexArrayType.Polygon) { Vertices = vs.Select(item => (Vertex)item), Color = Color.White, LockToGrid = _fixedres });
				}

				this.Graphics.Add(lines0, lines1);
			}
			
			public override void Update()
			{
				if (!this.Done)
				{
					if (this.Runtime % 110 == 0)
					{
						this.TargetRotation += Meth.Tau / 10;
						this.RotationSpeed = 0;
					}

					this.RotationSpeed += (this.TargetRotation - this.Rotation) / 200;

					if (this.Rotation > this.TargetRotation)
						this.RotationSpeed = this.RotationSpeed * 0.3 + (this.TargetRotation - this.Rotation) * 0.7;

					this.Rotation += this.RotationSpeed;
				}
				else
					this.Rotation += (this.TargetRotation - this.Rotation) * 0.1;
			}
		}

		class Rack : Entity
		{
			public Rack(bool _fixedres)
			{
				double[] ls = new double[] { 15, 15, 30 };
				double[] ds = new double[] { 0.2, 0.1 };

				VertexArray lines = new VertexArray(VertexArrayType.LinesStrip) { Color = Color.White * 0.2, LockToGrid = _fixedres };
				lines.Add(new Vector(0, ls[2] * 7), new Vector(0, 0), new Vector(0, 0));

				for (int i = 0; i < 7; i++)
				{
					Bunch<Vector> vs = new Bunch<Vector>();

					vs.Add(new Vector(0, i * ls[2]));
					
					Action<Vector> add = v =>
						{
							vs.Add(v);
							lines.Add(v);
						};

					add(new Vector(ls[0], i * ls[2]));
					
					for (int n = 0; n <= 3; n++)
						add(new Vector(ls[0] + Meth.Smooth(n / 3.0) * ls[1], i * ls[2] + ls[2] * ds[0] + ls[2] * ds[1] * (n / 3.0)));
					for (int n = 0; n <= 3; n++)
						add(new Vector(ls[0] + Meth.Smooth((3 - n) / 3.0) * ls[1], i * ls[2] + ls[2] * (1 - ds[0] - ds[1]) + ls[2] * ds[1] * (n / 3.0)));

					add(new Vector(ls[0], (i + 1) * ls[2]));

					vs.Add(new Vector(0, (i + 1) * ls[2]));

					this.Graphics.Add(new VertexArray(VertexArrayType.TrianglesFan) { Vertices = new Vertex(new Vector(0, (i + 0.5) * ls[2]), Color.White) + vs.Select(item => (Vertex)item), Color = Color.White, LockToGrid = _fixedres });
				}

				lines.Add(new Vector(0, ls[2] * 7));

				this.Graphics.Add(lines);
			}
		}
	}
}