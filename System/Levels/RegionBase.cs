using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;
using Zero;

namespace Mekanik
{
	public class RegionBase : Entity
	{
		internal Dictionary<string, Point> _LevelPositions = new Dictionary<string, Point>();
		internal Dictionary<string, LevelSource> _LevelSources = new Dictionary<string, LevelSource>();
		internal Dictionary<string, LevelBase> _Levels = new Dictionary<string, LevelBase>();
		internal Action<LevelBase> _OnLevelLoad;
		internal Action<LevelBase> _OnLevelEnter;
		internal Action<LevelBase> _OnLevelExit;
		internal bool[,] _Collisions;
		internal GameBase _Game;
		internal Bunch<Collider> Colliders = new Bunch<Collider>();
		public Entity Player;
		private Bunch<Program> _Programs = new Bunch<Program>();

		public Point Size
		{
			get { return new Point(this._Collisions.GetLength(0), this._Collisions.GetLength(1)); }
		}
		internal LevelBase _CurrentLevel;
		public LevelBase CurrentLevel
		{
			get { return this._CurrentLevel; }
		}

		public RegionBase(GameBase _game, string _path)
		{
			this.Physical = true;

			this._Game = _game;

			Point? topleft = null;
			Point? botright = null;

			MekaItem file = MekaItem.LoadFromFile(_path);
			foreach (MekaItem level in file.Children)
			{
				Point pos = this._LevelPositions[level.Name] = level.Content.To<Point>();
				LevelSource s = this._LevelSources[level.Name] = new LevelSource(File.GetFolder(_path) + "\\" + level.Name + ".meka", _game);

				if (!topleft.HasValue)
				{
					topleft = pos;
					botright = pos + s.Size;
				}
				else
				{
					if (pos.X < topleft.Value.X)
						topleft = new Point(pos.X, topleft.Value.Y);
					if (pos.Y < topleft.Value.Y)
						topleft = new Point(topleft.Value.X, pos.Y);

					if (pos.X + s.Size.X > botright.Value.X)
						botright = new Point(pos.X + s.Size.X, botright.Value.Y);
					if (pos.Y + s.Size.Y > botright.Value.Y)
						botright = new Point(botright.Value.X, pos.Y + s.Size.Y);
				}
			}

			topleft = topleft.Value * this._Game.TileCollisionResolution;
			botright = botright.Value * this._Game.TileCollisionResolution;

			this._Collisions = new bool[botright.Value.X - topleft.Value.X + 2, botright.Value.Y - topleft.Value.Y + 2];
			topleft = topleft.Value - 1;
			foreach (KeyValuePair<string, LevelSource> s in this._LevelSources)
			{
				Point p = this._LevelPositions[s.Key];
				for (int x = 0; x < s.Value.Collisions.GetLength(0); x++)
				{
					for (int y = 0; y < s.Value.Collisions.GetLength(1); y++)
						this._Collisions[x + p.X * this._Game.TileCollisionResolution.X - topleft.Value.X, y + p.Y * this._Game.TileCollisionResolution.Y - topleft.Value.Y] = s.Value.Collisions[x, y];
				}
			}

			this.CreateColliders();

			foreach (KeyValuePair<string, Point> p in this._LevelPositions.ToArray())
				this._LevelPositions[p.Key] -= (topleft.Value + 1) / this._Game.TileCollisionResolution;
		}

		private void CreateColliders()
		{
			int width = this.Size.X - 2;// * this._Game.TileCollisionResolution.X;
			int height = this.Size.Y - 2;// * this._Game.TileCollisionResolution.Y;

			Point dir = Point.Right;
			for (int d = 0; d < 4; d++)
			{
				dir = dir.NextClockwise;
				Point up = dir.NextAntiClockwise;

				bool[,] done = new bool[width + 2, height + 2];

				while (true)
				{
					bool b = true;
					for (int x = 0; x < width + 2; x++)
					{
						for (int y = 0; y < height + 2; y++)
						{
							if (!done[x, y])
								b = false;
						}
					}

					if (b)
						break;
					else
					{
						for (int x = 0; x < width + 2; x++)
						{
							for (int y = 0; y < height + 2; y++)
							{
								if (!done[x, y] && this._Collisions[x, y] && !this._Collisions[x + up.X, y + up.Y])
								{
									int wr = 0;
									int wl = 0;
									bool er = false;
									bool el = false;

									while (true)
									{
										wr++;

										if (!this._Collisions[x + dir.X * wr, y + dir.Y * wr])
										{
											wr--;
											er = true;
											break;
										}
										else if (this._Collisions[x + dir.X * wr + up.X, y + dir.Y * wr + up.Y])
											break;
									}
									while (true)
									{
										wl++;

										if (!this._Collisions[x - dir.X * wl, y - dir.Y * wl])
										{
											wl--;
											el = true;
											break;
										}
										else if (this._Collisions[x - dir.X * wl + up.X, y - dir.Y * wl + up.Y])
											break;
									}

									VertexArray v = new VertexArray(VertexArrayType.Quads) { Color = Color.Green ^ 170, Scale = this._Game.Tilesize / this._Game.TileCollisionResolution, Z = 1000 };

									Vector endlu = new Vector(el ? -0.5 : 0.5, -0.5);
									endlu.Angle += dir.Angle;

									Vector endru = new Vector(er ? 0.5 : -0.5, -0.5);
									endru.Angle += dir.Angle;

									Vector p = new Vector(x + 0.5, y + 0.5);
									v.Add(p - wl * (Vector)dir + endlu - 1);
									v.Add(p - wl * (Vector)dir - 1);
									v.Add(p + wr * (Vector)dir - 1);
									v.Add(p + wr * (Vector)dir + endru - 1);

									//v.Add(p - wl * 0.9 * (Vector)dir + endlu - 1);
									//v.Add(p - wl * (Vector)dir + endlu * 0.9 - 1);
									//v.Add(p - wl * (Vector)dir + endlu * 0.1 - 1);
									//v.Add(p - wl * 0.9 * (Vector)dir + endlu * 0.1 - 1);
									//v.Add(p + wr * 0.9 * (Vector)dir - 1);
									//v.Add(p + wr * (Vector)dir + endru * 0.1 - 1);
									//v.Add(p + wr * (Vector)dir + endru * 0.9 - 1);
									//v.Add(p + wr * 0.9 * (Vector)dir + endru - 1);

									this.Colliders.Add(new Collider(v));

									for (int i = -wl; i <= wr; i++)
										done[x + i * dir.X, y + i * dir.Y] = true;
								}
								else
									done[x, y] = true;
							}
						}
					}
				}
			}
		}

		public override void OnInitialization()
		{
			this.Static = true;
			
			this._WrappedThis.Name = "region";

			foreach (Collider c in this.Colliders)
				this.AddCollider(c);
			foreach (KeyValuePair<string, LevelSource> s in this._LevelSources)
			{
				LevelBase b = new LevelBase(s.Value, 0) { Position = this._LevelPositions[s.Key] * this._Game.Tilesize };
				this._Levels[s.Key] = b;
				this.Children.Add(b);
			}
			foreach (KeyValuePair<string, LevelBase> l in this._Levels)
				this._OnLevelLoad?.Invoke(l.Value);
			foreach (KeyValuePair<string, LevelBase> l in this._Levels)
			{
				//l.Value._Wrap = new Variable("this", Wrapper.WrapObject(l.Value, _onlyzero: false));
				l.Value.Source.OnLoad.Execute(new Permissions(Permission.DllUsage), Misc.Mekanik, this._WrappedThis);
			}
		}

		public override void Update()
		{
			if (this.Player != null)
			{
				foreach (KeyValuePair<string, Point> p in this._LevelPositions)
				{
					Rect r = new Rect(p.Value * this._Game.Tilesize, this._LevelSources[p.Key].Size * this._Game.Tilesize);
					if (r.Contains(this.Player.Position))
					{
						LevelBase b = this._Levels[p.Key];
						if (this._CurrentLevel != b)
						{
							if (this._CurrentLevel != null)
							{
								this._OnLevelExit?.Invoke(this._CurrentLevel);
								this._CurrentLevel.ExecuteScript(this._CurrentLevel.Source.OnExit, false, this._WrappedThis);
							}
							this._CurrentLevel = b;
							this._OnLevelEnter?.Invoke(this._CurrentLevel);

							this._CurrentLevel.ExecuteScript(this._CurrentLevel.Source.OnEnter, false, this._WrappedThis);
							//{
							//	Program pr = new Program(this._CurrentLevel.Source.OnEnter);
							//	this._Programs.Add(pr);
							//	pr.Start(new Permissions(Permission.DllUsage), Misc.Mekanik, this._CurrentLevel._Wrap, this._Wrap);
							//}
						}
					}
				}
			}
		}
	}
}