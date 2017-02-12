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
		internal Dictionary<string, Level> _Levels = new Dictionary<string, Level>();
		private string _ThisLevel;
		internal Action<Level> _OnLevelLoad;
		internal Action<Level> _OnLevelEnter;
		internal Action<Level> _OnLevelExit;
		internal Action<Level> _OnLevelUpdate;
		internal Func<List<MekaItem>, object> _LoadInfo;
		public bool[,] Collisions;
		internal GameBase _Game;
		internal Bunch<Collider> Colliders = new Bunch<Collider>();
		public Entity Player;
		private Bunch<Program> _Programs = new Bunch<Program>();
		private bool _Loaded;
		private string _Path;

		public Point Size
		{
			get { return new Point(this.Collisions.GetLength(0), this.Collisions.GetLength(1)); }
		}
		public int Width
		{
			get { return this.Size.X; }
		}
		public int Height
		{
			get { return this.Size.Y; }
		}
		internal Level _CurrentLevel;
		public Level CurrentLevel
		{
			get { return this._CurrentLevel; }
		}

		public RegionBase(GameBase _game, string _path, bool _usebank = true)
		{
			this.Physical = true;

			this._Game = _game;
			this._Path = _path.Substring(_game.LevelFolder.Length + 1);

			Point? topleft = null;
			Point? botright = null;

			if (File.Exists(_path + "\\_Region.meka"))
			{
				MekaItem file = MekaItem.LoadFromFile(_path + "\\_Region.meka");
				foreach (MekaItem level in file.Children)
				{
					Point pos = this._LevelPositions[level.Name] = level.Content.To<Point>();
					LevelSource s = this._LevelSources[level.Name] = (_usebank ? _game.LevelBank.GetLevel(_path.Substring(_game.LevelFolder.Length) + "\\" + level.Name) : new LevelSource(_game, _path + "\\" + level.Name + ".meka"));
					
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
			}
			else
			{
				this._ThisLevel = File.GetName(_path);
				this._LevelPositions["this"] = new Point(0, 0);
				LevelSource s = this._LevelSources["this"] = (_usebank ? _game.LevelBank.GetLevel(File.GetFolder(_path.Substring(_game.LevelFolder.Length)) + "\\" + File.GetName(_path)) : new LevelSource(_game, _path));
				topleft = new Point(0, 0);
				botright = s.Size;
			}

			topleft = topleft.Value * this._Game.TileCollisionResolution;
			botright = botright.Value * this._Game.TileCollisionResolution;

			this.Collisions = new bool[botright.Value.X - topleft.Value.X + 2, botright.Value.Y - topleft.Value.Y + 2];
			topleft = topleft.Value - 1;
			foreach (KeyValuePair<string, LevelSource> s in this._LevelSources)
			{
				Point p = this._LevelPositions[s.Key];
				for (int x = 0; x < s.Value.Collisions.GetLength(0); x++)
				{
					for (int y = 0; y < s.Value.Collisions.GetLength(1); y++)
						this.Collisions[x + p.X * this._Game.TileCollisionResolution.X - topleft.Value.X, y + p.Y * this._Game.TileCollisionResolution.Y - topleft.Value.Y] = s.Value.Collisions[x, y];
				}
			}
			
			this.CreateColliders();

			foreach (KeyValuePair<string, Point> p in this._LevelPositions.ToArray())
				this._LevelPositions[p.Key] -= (topleft.Value + 1) / this._Game.TileCollisionResolution;
		}

		private void CreateColliders()
		{
			int width = this.Size.X - 2;
			int height = this.Size.Y - 2;

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
								if (!done[x, y] && this.Collisions[x, y] && !this.Collisions[x + up.X, y + up.Y])
								{
									int wr = 0;
									int wl = 0;
									bool er = false;
									bool el = false;

									while (true)
									{
										wr++;

										if (!this.Collisions[x + dir.X * wr, y + dir.Y * wr])
										{
											wr--;
											er = true;
											break;
										}
										else if (this.Collisions[x + dir.X * wr + up.X, y + dir.Y * wr + up.Y])
											break;
									}
									while (true)
									{
										wl++;

										if (!this.Collisions[x - dir.X * wl, y - dir.Y * wl])
										{
											wl--;
											el = true;
											break;
										}
										else if (this.Collisions[x - dir.X * wl + up.X, y - dir.Y * wl + up.Y])
											break;
									}

									VertexArray v = new VertexArray(VertexArrayType.Quads) { Color = Color.Green ^ 170, Scale = this._Game.TileSize / this._Game.TileCollisionResolution, Z = 1000 };

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
				Level b = new Level(s.Value/*, 0*/) { Position = this._LevelPositions[s.Key] * this._Game.TileSize, _RegionBase = this };
				this._Levels[s.Key] = b;
				this.Children.Add(b);
			}
			foreach (KeyValuePair<string, Level> l in this._Levels)
			{
				//l.Value._Wrap = new Variable("this", Wrapper.WrapObject(l.Value, _onlyzero: false));
				l.Value.Source.OnLoad.Execute(new Permissions(Permission.DllUsage), Misc.Mekanik, this._WrappedThis);
			}

			foreach (KeyValuePair<string, Level> p in this._Levels)
				p.Value._Properties = this._LoadInfo(p.Value.Source._Properties);

			this.Friction = 0;
		}

		public override void Update()
		{
			if (!this._Loaded)
			{
				this._Loaded = true;
				foreach (KeyValuePair<string, Level> l in this._Levels)
					this._OnLevelLoad?.Invoke(l.Value);
			}

			if (this.Player != null)
			{
				foreach (KeyValuePair<string, Point> p in this._LevelPositions)
				{
					Rect r = new Rect(p.Value * this._Game.TileSize, this._LevelSources[p.Key].Size * this._Game.TileSize);
					if (r.Contains(this.Player.Position))
					{
						Level b = this._Levels[p.Key];
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
				
				this._OnLevelUpdate?.Invoke(this.CurrentLevel);
			}
		}

		public void SwitchLevel(Entity _player, string _level, string _entrance)
		{
			RegionBase r = this.Parent.LevelBank._GetRegion(_level);
			r.Position = this.Position;
			r._OnLevelLoad = this._OnLevelLoad;
			r._OnLevelEnter = this._OnLevelEnter;
			r._OnLevelExit = this._OnLevelExit;
			r._OnLevelUpdate = this._OnLevelUpdate;
			r._LoadInfo = this._LoadInfo;

			this.Children.Remove(_player);
			this.Kill();
			this.Parents[0].Children.Add(r);
			r.SpawnPlayer(_player, _level, _entrance);
		}

		public void SpawnPlayer(Entity _player, string _level, string _entrance)
		{
			Level b;
			if (this._Levels.Count == 1)
				b = this._Levels.ToArray()[0].Value;
			else
				b = this._Levels[_level];

			b.SpawnPlayer(_player, _entrance);
			this.Children.Add(this.Player = _player);

			if (this.Parent.CurrentSaveFile != null)
			{
				this.Parent.CurrentSaveFile._PositionRegion = this._Path;
				this.Parent.CurrentSaveFile._PositionEntrance = _entrance;
			}
		}
	}
}