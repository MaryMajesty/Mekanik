using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using Meka;

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Contacts;

using OpenTK.Audio.OpenAL;
using OpenTK.Audio;

namespace Mekanik
{
	public class GameBase
	{
		#region Setup

		public static string TempPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MekanikEngine\\Temp\\";
		
		private Platform _Platform;
		public Platform Platform
		{
			get { return this._Platform; }
		}

		private bool _IsGame;

		public GameBase(int _width, int _height, bool _fixedresolution = false, int _antialias = 0, bool _isgame = true, Platform _platform = Platform.Windows, Type _achievements = null)
		{
			this._Americanize();

			FarseerPhysics.Settings.MaxPolygonVertices = 100;

			this._Platform = _platform;

			this._CreateAudioContext();
			
			this.Node = new Node(this);
			this.LevelBank = new RegionBank(this);
			
			this._IsGame = _isgame;
			if (!_isgame)
			{
				this.MouseLimitedToScreen = false;
				this.ScaleMode = ScaleMode.None;
			}

			if (_achievements != null)
			{
				foreach (FieldInfo f in _achievements.GetFields())
					this._Achievements.Add((Achievement)f.GetValue(null));
			}

			this._AntiAlias = _antialias;

			this._Resolution = new Point(_width, _height);
			if (_fixedresolution)
				this.FixedResolution = true;

			this.Camera = this._Resolution / 2;

			this.SoundDirectionHalfLife = Meth.Root(this.Resolution.X * this.Resolution.Y) / 2;
			this.SoundVolumeHalfLife = Meth.Root(this.Resolution.X * this.Resolution.Y) / 2;
		}

		private void _CreateAudioContext()
		{
			IntPtr d = OpenTK.Audio.OpenAL.Alc.OpenDevice(OpenTK.Audio.AudioContext.DefaultDevice);
			OpenTK.ContextHandle c = OpenTK.Audio.OpenAL.Alc.CreateContext(d, new int[0]);
			OpenTK.Audio.OpenAL.Alc.MakeContextCurrent(c);
			
			AL.Listener(ALListener3f.Position, 0, 0, 0);
			AL.Listener(ALListener3f.Velocity, 0, 0, 0);
			float[] vs = new float[] { 0, 0, -1, 0, 1, 0 };
			AL.Listener(ALListenerfv.Orientation, ref vs);
		}

		protected void _Initialize()
		{
			if (this.FixedResolution)
				this._RendererPixel = new Renderer(this.Resolution.X, this.Resolution.Y);
			this._RendererOutput = new Renderer(this.Resolution.X, this.Resolution.Y, _rendertowindow: true);

			this._DebugOverlay = new Label("") { CharSize = 18, TextColor = Color.White, /*RenderIntermediately = true, RenderGraphicsIntermediately = true, IntermediateSize = new Point(1, 1), IntermediateShader = null,*/ Edge = 5 };
			this._IndicationOverlay = new Label("") { CharSize = 16, /*RenderIntermediately = true, RenderGraphicsIntermediately = true, IntermediateSize = new Point(1, 1), IntermediateShader = null,*/ Edge = 5 };
			
			this._DebugOverlay.Parent = this;
			this._DebugOverlay.OnInitialization();
		}

		internal void _Americanize()
		{
			System.Globalization.CultureInfo murica = new System.Globalization.CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = murica;
			Thread.CurrentThread.CurrentUICulture = murica;
		}

		protected void _OnStart()
		{
			if (!this._DoSettingsExist())
				this._CreateSettings();
			this._LoadSettings();

			if (!File.Exists(this.Path + "\\Internal\\Saves"))
				File.CreateFolder(this.Path + "\\Internal\\Saves");

			this.OnStart();
		}

		public virtual void OnStart() { }

		protected void _OnClose()
		{
			foreach (Entity e in this._RealEntities)
			{
				foreach (Zero.Program p in e._Programs)
					p.UserAbort();
			}
			this._SaveSettings();
			this.OnClose();
		}

		public virtual void OnClose() { }

		#endregion

		#region Settings

		public readonly Keymap DefaultKeymap = new Keymap();
		public readonly Keymap Keymap = new Keymap();
		public readonly MekaItem DefaultSettings = new MekaItem(null, new List<MekaItem>());
		public readonly MekaItem Settings = new MekaItem(null, new List<MekaItem>());

		protected virtual bool _DoSettingsExist() { throw new Exception("Platform settings not implemented yet."); }
		protected virtual List<MekaItem> _GetPlatformSettings() { throw new Exception("Platform settings not implemented yet."); }
		protected virtual List<MekaItem> _GetPlatformSettingsDefault() { throw new Exception("Platform settings not implemented yet."); }
		protected virtual void _SetPlatformSettings(MekaItem _settings) { throw new Exception("Platform settings not implemented yet."); }

		protected virtual MekaItem _LoadSettingsFile() { throw new Exception("Settings not implemented yet."); }
		protected virtual void _SaveSettingsFile(MekaItem _settings) { throw new Exception("Settings not implemented yet."); }

		private List<MekaItem> _GetEngineSettingsDefault()
		{
			List<MekaItem> @out = new List<MekaItem>();
			@out.Add(new MekaItem("ScaleMode", this._IsGame ? "FixedRatio" : "None"));
			return @out;
		}

		private List<MekaItem> _GetEngineSettings()
		{
			List<MekaItem> @out = new List<MekaItem>();
			@out.Add(new MekaItem("ScaleMode", (this._LevelEditor != null ? this._LevelEditor.StartScaleMode.ToString() : this.ScaleMode.ToString())));
			return @out;
		}

		private void _SetEngineSettings(MekaItem _settings)
		{
			this.ScaleMode = (ScaleMode)ScaleMode.Parse(typeof(ScaleMode), _settings["ScaleMode"].Content);
		}

		private void _CreateSettings()
		{
			MekaItem settings = new MekaItem("Settings", new List<MekaItem>());
			settings.Children.Add(new MekaItem("Engine", this._GetEngineSettingsDefault()));
			settings.Children.Add(new MekaItem("Platform", this._GetPlatformSettingsDefault()));
			settings.Children.Add(new MekaItem("Custom", this.DefaultSettings.Children));
			this._SaveSettingsFile(settings);
		}

		private void _LoadSettings()
		{
			MekaItem settings = this._LoadSettingsFile();
			this._SetEngineSettings(settings["Engine"]);
			this._SetPlatformSettings(settings["Platform"]);
			this.OnSettingsLoad(settings["Custom"]);
		}

		private void _SaveSettings()
		{
			MekaItem settings = new MekaItem("Settings", new List<MekaItem>());
			settings.Children.Add(new MekaItem("Engine", this._GetEngineSettings()));
			settings.Children.Add(new MekaItem("Platform", this._GetPlatformSettings()));
			settings.Children.Add(new MekaItem("Custom", this.Settings.Children));
			this._SaveSettingsFile(settings);
		}

		public virtual void OnSettingsLoad(MekaItem _settings) { }

		#endregion

		#region Network

		public Node Node;
		protected Bunch<MekaItem> _Messages = new Bunch<MekaItem>();
		internal Bunch<int> _UserIds = new Bunch<int>();
		internal Bunch<Tuple<User, Dictionary<string, MekaItem>>> _Syncs = new Bunch<Tuple<User, Dictionary<string, MekaItem>>>();
		protected DateTime _LastSync;
		internal MekaItem _ServerConnect;
		internal Bunch<User> _ClientConnects = new Bunch<User>();
		public int LocalPlayerCount = 1;
		public Bunch<int> LocalPlayerIds = new Bunch<int>();
		public Bunch<int> OnlinePlayerIds = new Bunch<int>();
		public Sandbox<int, Controls> LocalPlayerControls = new Sandbox<int, Controls>();
		public Sandbox<int, Controls> OnlinePlayerControls = new Sandbox<int, Controls>();

		protected void _Sync()
		{
			List<MekaItem> items = new List<MekaItem>();

			items.Add(new MekaItem("Custom", (this.OnSyncSend() ?? new Bunch<MekaItem>()).ToList()));

			List<MekaItem> players = new List<MekaItem>();
			foreach (Entity e in this.RealEntities.Where(item => item.PlayerId.HasValue))
				players.Add(new MekaItem(e.PlayerId.Value.ToString(), e.Position.ToString()));
			items.Add(new MekaItem("Players", players));

			//items.Add(new MekaItem("Messages", this._Messages.ToList()));
			//this._Messages.Clear();

			if (this.Node.IsServer)
			{
				items.Add(new MekaItem("Ping", this.Node.Server.GetPingId().ToString()));
				items.Add(new MekaItem("Info", new List<MekaItem>() { new MekaItem("Speed", this.Node.Server.Users[0]._Speed.ToString()), new MekaItem("Latency", this.Node.Server.Users[0].Latency.ToString()) }));
			}
			else if (this.Node.Client._LastPing != null)
			{
				items.Add(new MekaItem("Pong", this.Node.Client._LastPing));
				this.Node.Client._LastPing = null;
			}

			this.Node.Send(new MekaItem("Sync", items));
			this._LastSync = DateTime.Now;
		}

		public void SendMessage(MekaItem _message) => this._Messages.Add(new MekaItem("Custom", new List<MekaItem>() { _message }));

		public virtual void OnUserJoin(int _id) { }
		public virtual List<MekaItem> OnUserConnect(User _user) => new List<MekaItem>();
		public virtual void OnServerConnect(User _user, MekaItem _info) { }
		public virtual void OnClientConnect(User _user) { }
		public virtual void OnMessageFromServer(MekaItem _message) { }
		public virtual void OnMessageFromClient(MekaItem _message) { }
		public virtual void OnUpdate() { }
		public virtual Bunch<MekaItem> OnSyncSend() => null;
		public virtual void OnSyncReceive(User _user, Dictionary<string, MekaItem> _syncs) { }

		#endregion

		#region Visuals

		public ScaleMode ScaleMode = ScaleMode.FitScreen;
		public Color Background = Color.White;
		public Color EmptyBackground = Color.Black;
		public Vector Camera;
		public double Rotation;
		public double Zoom = 0;
		public double ZoomSpeed = 3;
		public bool FixedResolution;
		protected bool _RenderSync;
		protected bool? _RenderSyncLock;
		public bool AutoRegulateRenderSync = true;
		protected bool _ScreenshotRequested;

		protected Point _Resolution;
		public Point Resolution
		{
			get { return this._Resolution; }
		}

		protected int _AntiAlias;
		public int AntiAlias
		{
			get { return this._AntiAlias; }
		}

		public double RealZoom
		{
			get { return Meth.Pow(this.ZoomSpeed, Zoom); }
		}



		public bool UseMultipleRenderers;
		protected Renderer _RendererOutput;
		protected Renderer _RendererPixel;
		protected Renderer[] _Renderers;
		public Renderer[] Renderers
		{
			get { return this._Renderers.ToArray(); }
		}
		private Dictionary<string, int> _RenderedGraphics;

		public void AddRenderers(int _count)
		{
			Bunch<Renderer> rs = new Bunch<Renderer>();
			for (int i = 0; i < _count; i++)
				rs.Add(new Renderer(this.FixedResolution ? this.Resolution : this.Size));
			this._Renderers = rs.ToArray();
		}



		protected Vector _GetScreenScale()
		{
			if (this.ScaleMode == ScaleMode.None)
				return 1;
			else if (this.ScaleMode == ScaleMode.Perfect)
				return Meth.Down(Meth.Min(this.Width / (double)this.Resolution.X, this.Height / (double)this.Resolution.Y));
			else if (this.ScaleMode == ScaleMode.FixedRatio)
				return Meth.Min(this.Width / (double)this.Resolution.X, this.Height / (double)this.Resolution.Y);
			else
				return new Vector(this.Width / (double)this.Resolution.X, this.Height / (double)this.Resolution.Y);
		}

		protected Vector _GetScreenSize() => this.Resolution * this._GetScreenScale();

		protected Point _GetScreenPosition()
		{
			if (this.ScaleMode == ScaleMode.None)
				return new Point(0, 0);
			else
			{
				Vector v = this.Size / 2.0 - this._GetScreenSize() / 2;
				return new Point(Meth.Up(v.X), Meth.Up(v.Y));
			}
		}

		public void Render()
		{
			DateTime start = DateTime.Now;

			this.FrameRuntime++;
			if (this.FrameRuntime % 30 == 0)
			{
				double dif = (DateTime.Now - this._LastFrameTime).TotalMilliseconds;
				this._Frames.Add(1000 / dif * 30);
				if (this._Frames.Count > 2)
					this._Frames.RemoveAt(0);
				if (this._Frames.Count > 1)
					this._Fps = this._Frames.Average();
				this._LastFrameTime = DateTime.Now;
			}

			this.Draw();

			double time = (DateTime.Now - start).TotalMilliseconds;
			if (this.AutoRegulateRenderSync && !this._RenderSyncLock.HasValue)
			{
				if (time > 20)
					this._RenderSync = false;
			}
			else if (this._RenderSyncLock.HasValue)
				this._RenderSync = this._RenderSyncLock.Value;
			else
				this._RenderSync = false;
		}

		public void Draw()
		{
			if (this.FixedResolution)
			{
				this._RendererOutput.Clear(this.EmptyBackground);
				this._RendererPixel.Clear(this.Background);
			}
			else
				this._RendererOutput.Clear(this.Background);

			if (this.UseMultipleRenderers)
			{
				foreach (Renderer r in this.Renderers)
					r.Clear();
			}

			foreach (Entity entity in this._RealEntities)
				entity.OnRender();

			Bunch<Graphic>[] gs = new Bunch<Graphic>[this.UseMultipleRenderers ? this.Renderers.Length : 1];
			for (int i = 0; i < gs.Length; i++)
				gs[i] = new Bunch<Graphic>();
			for (int i = 0; i < 2; i++)
			{
				Bunch<Graphic> graphics = new Bunch<Graphic>();
				foreach (Entity entity in this.Entities.Where(item => (i == 0) ? !item.Interfacial : item.Interfacial))
					graphics.Add(entity._GetGraphics());
				foreach (Graphic g in graphics.Where(item => item.Visible && !item.Disposed && item.Color.A > 0 && (item.Scale.X != 0 || item.Scale.Y != 0)).OrderBy(item => item.Z))
				{
					if (i == 0)
					{
						g.Position = (g.Position - (Point)this.Camera + this._CameraShakeOffset) * this.RealZoom + this.Resolution / 2;
						g.Scale *= this.RealZoom;
					}

					if (!this.FixedResolution)
					{
						g.Position *= this._GetScreenScale();
						g.Position += this._GetScreenPosition();
						g.Scale *= this._GetScreenScale();
					}

					g.Position += new Vector(0.00000001 * Meth.Sign(g.Scale.X), 0.00000001 * Meth.Sign(g.Scale.Y));

					if (g.LockToGrid)
						g.Position = (Point)g.Position;

					gs[(this.UseMultipleRenderers && g.Renderer.HasValue) ? g.Renderer.Value : 0].Add(g);
				}
			}

			this._RenderedGraphics = new Dictionary<string, int>();

			if (!this.UseMultipleRenderers)
			{
				if (this.FixedResolution)
				{
					this._RendererPixel.Draw(gs[0]);
					this._RenderedGraphics["Pixel"] = gs[0].Count;
				}
				else
				{
					this._RendererOutput.Draw(gs[0]);
					this._RenderedGraphics["Output"] = gs[0].Count;
				}
			}
			else
			{
				for (int i = 0; i < this.Renderers.Length; i++)
				{
					this._Renderers[i].Draw(gs[i]);
					this._RenderedGraphics["Custom " + i.ToString()] = gs[i].Count;
				}
			}

			foreach (Entity e in this.Entities)
				e._ResetAll();

			foreach (Entity entity in this._RealEntities)
				entity.AfterRender();



			if (this.UseMultipleRenderers)
				this.FinalDraw(this.FixedResolution ? this._RendererPixel : this._RendererOutput, this._Renderers);



			if (this.FixedResolution)
				this._RendererOutput.Draw(new Image(this._RendererPixel.ImageSource) { Position = this._GetScreenPosition(), Scale = this._GetScreenScale() });


			if (this._ScreenshotRequested)
			{
				if (!File.Exists(this.Path + "\\Screenshots"))
					File.CreateFolder(this.Path + "\\Screenshots");

				(this.FixedResolution ? this._RendererPixel.ImageSource : this._RendererOutput.ImageSource).SaveRgb(this.GetSaveName(this.Path + "\\Screenshots\\", "png"));
				this._ScreenshotRequested = false;
			}

			if (this._Recording && this.FrameRuntime % 3 == 0)
			{
				if (this.FixedResolution)
					this._RecordedFrames.Add(this._RendererPixel.ImageSource.PixelBytes);
				else
				{
					ImageSource img = this._RendererOutput.ImageSource;
					this._RecordedFrames.Add(img.PixelBytes);
					img.Dispose();
				}
				//this._RecordedFrames.Add((this.FixedResolution ? this._RendererPixel.ImageSource : this._RendererOutput.ImageSource).PixelBytes);
				//this._ConvertedFrames.Add((this.FixedResolution ? this._RendererPixel.ImageSource : this._RendererOutput.ImageSource).ToBitmap().GetBytes(System.Drawing.Imaging.ImageFormat.Gif));
				//this.DoRecord();
				this._FrameCount++;
			}

			if (this._DebugOverlayVisible)
			{
				this._DebugOverlay.Content = this._GetDebugText();
				this._RendererOutput.Draw(this._DebugOverlay._GetGraphics());
			}

			this._AfterRender();
		}

		public virtual void FinalDraw(Renderer _output, Renderer[] _layers)
		{
			foreach (Renderer l in _layers)
				_output.Draw(new Image(l.ImageSource));
		}

		protected virtual void _AfterRender() { }

		private bool _Recording;
		private bool _FinishedRecording = true;
		private Bunch<byte[]> _RecordedFrames;
		private Bunch<byte[]> _ConvertedFrames;
		private int _FrameCount;

		public void DoRecord()
		{
			if (!this._Recording && this._FinishedRecording)
			{
				this._Recording = true;
				this._FinishedRecording = false;
				this._RecordedFrames = new Bunch<byte[]>();
				this._ConvertedFrames = new Bunch<byte[]>();
				this._RenderSyncLock = true;

				Func<byte[], Color[,]> convert = bs =>
					{
						Point size = this.FixedResolution ? this.Resolution : this.Size;
						Color[,] @out = new Color[size.X, size.Y];
						for (int i = 0; i < size.X * size.Y; i++)
						{
							Point p = new Point(i % size.X, Meth.Down(i / size.X));
							@out[p.X, p.Y] = new Color(bs.SubArray(i * 4, 4));
						}
						return @out;
					};

				System.Threading.Thread t = new Thread(() =>
					{
						this.Title = "Started!";
						int i = 0;
						while (this._Recording || this._RecordedFrames.Count > 0 && this.IsRunning)
						{
							if (this._RecordedFrames.Count > 0)
							{
								Color[,] cs = convert(this._RecordedFrames[0]);
								//System.Drawing.Bitmap hd = GifEncoder.Unuglify(cs);
								//this._ConvertedFrames.Add(hd.GetBytes(System.Drawing.Imaging.ImageFormat.Gif));
								this._ConvertedFrames.Add(GameBase.GifUnuglify(cs));
								//hd.Dispose();
								lock (this._RecordedFrames)
									this._RecordedFrames.RemoveAt(0);
								i++;
								this.Title = (i / (double)this._FrameCount).ToString();
							}
						}
						if (this.IsRunning)
						{
							if (!File.Exists(this.Path + "\\Recordings"))
								File.CreateFolder(this.Path + "\\Recordings");

							//GifEncoder.Encode(this._ConvertedFrames, 50, this.GetSaveName(this.Path + "\\Recordings\\", "gif"));
							File.Write(this.GetSaveName(this.Path + "\\Recordings\\", "gif"), GameBase.GifEncode(this._ConvertedFrames));

							this._ConvertedFrames = new Bunch<byte[]>();
							this._FinishedRecording = true;
							this.Title = "Finished!";
						}
					});
				t.Start();
			}
			else
			{
				this._Recording = false;
				this._RenderSyncLock = null;
			}
		}

		#endregion

		#region Physics

		protected internal World _World = new World(new Vector(0, 1024));
		protected World _InterfaceWorld = new World(new Vector(0, 0));
		public double PhysicsSpeed = 1;
		public static double Meter = 32;

		public Vector Gravity
		{
			get { return this._World.Gravity; }
			set { this._World.Gravity = value; }
		}

		public MouseArea GetMouseAreaAt(Vector _pos)
		{
			Bunch<MouseArea> ms = this._InterfaceWorld.TestPointAll(_pos).ToBunch().Select(item => (MouseArea)item.UserData).Where(item => item.Entity.Visible).OrderByDescending(item => item.Entity.RealZ + item.Z);
			return (ms.Count > 0) ? ms[0] : null;
		}

		public Entity GetEntityAt(Vector _pos)
		{
			Bunch<Collider> cs = this._World.TestPointAll(_pos).ToBunch().Select(item => (Collider)item.UserData).OrderByDescending(item => item.Entity.RealZ);
			return (cs.Count > 0) ? cs[0].Entity : null;
		}

		#endregion

		#region Interface

		protected virtual Point _GetSize() => this.Resolution;
		protected virtual void _SetSize(Point _size) { throw new Exception("You can't set the size."); }
		public Point Size
		{
			get { return this._GetSize(); }
			//get { return new Point(this.Form.ClientSize.Width, this.Form.ClientSize.Height); }
			set { this._SetSize(value); }
		}
		public int Width
		{
			get { return this.Size.X; }
			set { this.Size = new Point(value, this.Size.Y); }
		}
		public int Height
		{
			get { return this.Size.Y; }
			set { this.Size = new Point(this.Size.X, value); }
		}

		protected bool _Running = true;
		public bool IsRunning
		{
			get { return this._Running; }
		}

		protected virtual void _SetTitle(string _value) { throw new Exception("Title setting not implemented yet."); }
		protected string _Title;
		public string Title
		{
			get { return this._Title; }

			set
			{
				this._SetTitle(value);
				this._Title = value;
			}
		}



		public bool MouseLimitedToScreen = true;

		protected Vector _MousePositionRaw;
		protected Vector _MousePosition;
		public Vector MousePosition
		{
			get { return this._MousePosition; }
			//set { Mouse.SetPosition((Vector2i)this.GetPointOnScreen(value), this._Window); }
		}

		public Vector MousePositionRaw
		{
			get { return this._MousePositionRaw; }
		}

		private bool _Hovered;
		public bool IsHovered
		{
			get { return this._Hovered; }
		}

		#endregion

		#region Entities

		internal Bunch<Entity> _RealEntities = new Bunch<Entity>();
		public Bunch<Entity> RealEntities
		{
			get { return this._RealEntities; }
		}

		protected Bunch<Entity> _Entities;
		public Bunch<Entity> Entities
		{
			get
			{
				if (this._Entities == null)
				{
					this._Entities = new Bunch<Entity>();
					this._Entities._OnAdd = e => this._AddEntities(e, new Bunch<Entity>());
					this._Entities._OnRemove = e => this._RemoveEntity(e, _kill: false);
				}
				return this._Entities;
			}
		}

		internal void _AddEntities(Bunch<Entity> _entities, Bunch<Entity> _parents)
		{
			foreach (Entity entity in _entities.Where(item => !item._Added))
			{
				if (!entity._IsDead)
				{
					entity._Added = true;
					entity.Parent = this;
					entity.Parents = _parents;
					foreach (Analog analog in entity.Analogs)
						analog._Entity = entity;

					this._RealEntities.Add(entity);
					if (!entity._Initialized)
					{
						if (entity.Physical)
							entity._Body = new Body(this._World, entity.Position) { BodyType = BodyType.Dynamic, Rotation = (float)entity.Rotation };
						if (entity.Interfacial)
							entity._InterfaceBody = new Body(this._InterfaceWorld, entity.Position) { BodyType = BodyType.Static };
					}
					else
					{
						if (entity.Physical)
							entity._Body.Enabled = true;
						if (entity.Interfacial)
							entity._InterfaceBody.Enabled = true;
					}

					entity._UpdateScale();

					if (!entity._Initialized)
					{
						if (this.Inputs != null)
						{
							entity.InputsHidden = (Inputs)this.Inputs.GetType().GetConstructor(new Type[0]).Invoke(new object[0]);
							entity.InputsHidden._Entity = entity;
							entity.InputsHidden._Initialize();
						}

						entity._Initialized = true;
						entity.OnInitialization();
					}

					this._AddEntities(entity.Children, entity + _parents);
				}
			}
		}

		internal void _RemoveEntity(Entity _entity, bool _kill = true)
		{
			_entity._Added = false;
			_entity._IsDead = _kill;
			if (_kill)
			{
				_entity.OnKill();

				if (_entity.Physical)
				{
					foreach (Collider c in _entity._Colliders)
						c.Destroy();
					this._World.RemoveBody(_entity._Body);
				}
				if (_entity.Interfacial)
				{
					foreach (MouseArea m in _entity._MouseAreas)
						m.Destroy();
					this._InterfaceWorld.RemoveBody(_entity._InterfaceBody);
				}

				_entity.AfterKill();
			}
			else
			{
				if (_entity.Physical)
					_entity._Body.Enabled = false;
				if (_entity.Interfacial)
					_entity._InterfaceBody.Enabled = false;
			}

			this._RealEntities.Remove(_entity);

			foreach (Entity child in _entity.Children.ToArray())
				this._RemoveEntity(child, _kill);

			if (_entity.Parents.Count == 0)
				this.Entities._Remove(_entity);
			else if (_kill)
				_entity.Parents[0].Children._Remove(_entity);
		}



		public Entity DragEntity
		{
			get { return this.RealEntities.First(item => item.IsDragged); }
		}

		public bool IsDragging
		{
			get { return this.DragEntity != null; }
		}

		#endregion

		#region Editor

		public Point Tilesize;
		public Bunch<Type> EntityTypes = new Bunch<Type>(typeof(Decoration));
		public string LevelFolder;
		public string AreasetFolder;
		public Dictionary<string, Areaset> Areasets = new Dictionary<string, Areaset>();
		internal LevelEditor _LevelEditor;
		public RegionBank LevelBank;
		public Point TileCollisionResolution = new Point(2, 2);

		internal bool _TestStarted;
		public bool TestStarted
		{
			get { return this._TestStarted; }
		}

		public virtual void StartTest(LevelSource _source, string _path, string _entrance) { }

		public void StopTest()
		{
			foreach (Entity e in this.Entities)
				e.Kill();

			this.Entities.Add(this._LevelEditor);

			this.UseMultipleRenderers = false;
			this.FixedResolution = false;
			this.ScaleMode = ScaleMode.None;
			this.MouseLimitedToScreen = false;
			//this.IgnoreMouseWithoutFocus = false;

			this.Camera = this.Resolution / 2;
			this.CameraFocus = null;
			this.Zoom = 0;

			this._TestStarted = false;
		}

		#endregion

		#region Frames

		public Code.Clock Clock = new Code.Clock();
		public int Runtime;
		public int FrameRuntime;
		protected DateTime _LastUpdateTime;
		protected DateTime _LastFrameTime;
		protected Bunch<double> _Updates = new Bunch<double>();
		protected Bunch<double> _Frames = new Bunch<double>();
		
		public int CurrentFrame
		{
			get { return this.Runtime % 60; }
		}

		protected double _Ups = 30;
		public int Ups
		{
			get { return Meth.Round(this._Ups); }
		}
		protected double _Fps = 30;
		public int Fps
		{
			get { return Meth.Round(this._Fps); }
		}

		#endregion

		#region Camera

		protected Entity _CameraFocus;
		protected double _CameraTargetY;
		public Vector CameraThreshold;
		public Vector CameraSpeed = 0.1;
		public double CameraFallFactor = 2;
		public double CameraY = 0.7;
		protected double _CameraDistance;
		public bool CameraPixelCheatX;
		public bool CameraPixelCheatY;
		protected Vector _CameraOffset;
		protected double _CameraTargetYOffset;
		public double CameraSpeedMin;
		public double CameraFixThreshold = 1;
		protected Point _CameraShakeOffset;
		protected bool _CameraShakeActive;
		protected int _CameraShakeCooldown;

		public Entity CameraFocus
		{
			get { return this._CameraFocus; }

			set
			{
				if (this._CameraFocus != value)
				{
					this._CameraFocus = value;
					//if (value != null)
					//{
					//	value._UpdateRealPosition();
					//	this.Camera = value.RealPosition - new Vector(0, this.Resolution.Y * (this.CameraY - 0.5));
					//}
				}
			}
		}

		public void ZoomAt(Vector _position, double _zoom)
		{
			Vector v = (_position - (this.Camera)) / this.Size * this.RealZoom;

			double old = this.RealZoom;
			this.Zoom += _zoom;
			double now = this.RealZoom;

			this.Camera += v / ((_zoom > 0) ? now : old) * this.Size * Meth.Sign(_zoom) * ((_zoom > 0) ? (now / old - 1) : (old / now - 1));
		}

		public void ShakeCamera(int _speed = 5)
		{
			this._CameraShakeActive = true;
			if (this._CameraShakeCooldown == 0)
			{
				this._CameraShakeCooldown = _speed;
				Bunch<Point> ps = new Bunch<Point>(Point.Right, Point.Down, Point.Left, Point.Up);
				if (ps.Contains(this._CameraShakeOffset))
					ps.Remove(this._CameraShakeOffset);
				this._CameraShakeOffset = ps.Random;
			}
		}

		public void CameraFreezeRelative()
		{
			this._CameraOffset = this.Camera - this.CameraFocus.Position;
			this._CameraTargetYOffset = this._CameraTargetY - this.CameraFocus.Y;
		}

		public void CameraUnfreezeRelative()
		{
			this.Camera = this.CameraFocus.Position + this._CameraOffset;
			this._CameraTargetY = this.CameraFocus.Y + this._CameraTargetYOffset;
		}

		private void _UpdateCamera()
		{
			if (this.CameraFocus != null)
			{
				Vector target = this.Camera;

				if (this.CameraFocus.RealPosition.X - this.Camera.X < -this.CameraThreshold.X)
					target.X = this.CameraFocus.RealPosition.X + this.CameraThreshold.X;
				if (this.CameraFocus.RealPosition.X - this.Camera.X > this.CameraThreshold.X)
					target.X = this.CameraFocus.RealPosition.X - this.CameraThreshold.X;

				if (this.CameraFocus.RealPosition.Y - this.Camera.Y < -this.CameraThreshold.Y)
					this._CameraTargetY = this.CameraFocus.RealPosition.Y + this.CameraThreshold.Y;
				if (this.CameraFocus.RealPosition.Y - this.Camera.Y > this.CameraThreshold.Y)
					this._CameraTargetY = this.CameraFocus.RealPosition.Y - this.CameraThreshold.Y;

				if (this.CameraFocus.IsOnGround)
					this._CameraTargetY = this.CameraFocus.RealPosition.Y - this.Resolution.Y * (this.CameraY - 0.5);

				target.Y = this._CameraTargetY;

				Vector c = this.Camera;

				bool b = false;
				Vector off = (target - this.Camera) * this.CameraSpeed * new Vector(1, target.Y > this.Camera.Y ? this.CameraFallFactor : 1);
				if (Meth.Abs(off.X) > this.CameraFixThreshold)
				{
					double m = this.CameraFocus.ActualMotion.X / 60;
					if (Meth.Abs(m) > this.CameraFixThreshold && Meth.Sign(m) == Meth.Sign(off.X))
					{
						off.X = m;
						b = true;
					}
				}

				if (!this.CameraPixelCheatX && Meth.Abs(off.X) < this.CameraSpeedMin)
					off.X = this.CameraSpeedMin * Meth.Sign(off.X);
				if (!this.CameraPixelCheatY && Meth.Abs(off.Y) < this.CameraSpeedMin)
					off.Y = this.CameraSpeedMin * Meth.Sign(off.Y);

				if (!this.CameraPixelCheatX && Meth.Abs(this.Camera.X + off.X - Meth.Sign(off.X) - target.X) < 1)
					off.X = target.X - this.Camera.X;
				if (!this.CameraPixelCheatY && Meth.Abs(this.Camera.Y + off.Y - Meth.Sign(off.Y) - target.Y) < 1)
					off.Y = target.Y - this.Camera.Y;

				this.Camera += off;

				if (!b)
					this._CameraDistance = 0;
				else
				{
					if (this._CameraDistance == 0)
					{
						this._CameraDistance = this.CameraFocus.X - this.Camera.X;
						this._CameraDistance = this._CameraDistance > 0 ? Meth.Down(this._CameraDistance) + 0.5 : Meth.Up(this._CameraDistance) - 0.5;
					}

					this.Camera.X += (this.CameraFocus.X - this.Camera.X) - this._CameraDistance;
				}
			}

			if (this._CameraShakeCooldown > 0)
				this._CameraShakeCooldown--;
			else
			{
				if (!this._CameraShakeActive)
					this._CameraShakeOffset = new Point(0, 0);
			}
		}

		#endregion

		#region Debug

		protected bool _DebugOverlayVisible;
		protected Label _DebugOverlay;
		protected double _IndicationFade;
		protected Label _IndicationOverlay;
		protected int _DebugPage;

		public string IndicationText
		{
			get { return this._IndicationOverlay.Content; }

			set
			{
				this._IndicationOverlay.Content = value;
				this._IndicationFade = 1;
			}
		}

		protected int _DebugPageMax = 3;
		private PerformanceCounter _PerformanceCounter;
		private bool _GettingPerformanceCounter;
		private int _LastRam;

		private string _GetDebugText()
		{
			Bunch<string> @out = new Bunch<string>();

			if (this._DebugPage == 0)
			{
				@out.Add("(1/3) Performance:");
				@out.Add("");
				@out.Add("UPS: " + this.Ups.ToString());
				@out.Add("FPS: " + this.Fps.ToString());
				@out.Add("RenderSync: " + this._RenderSync.ToString());
				@out.Add("");

				if (!this._GettingPerformanceCounter)
				{
					this._GettingPerformanceCounter = true;

					Thread t = new Thread(() =>
						{
							Process proc = Process.GetCurrentProcess();

							this._PerformanceCounter = new PerformanceCounter();
							this._PerformanceCounter.CategoryName = "Process";
							this._PerformanceCounter.CounterName = "Working Set - Private";
							this._PerformanceCounter.InstanceName = proc.ProcessName;
						});
					t.Start();
				}

				if (this._PerformanceCounter != null && this.Runtime % 10 == 0)
					this._LastRam = (int)(this._PerformanceCounter.NextValue() / 1000);
				@out.Add("RAM: " + this._LastRam.ToString("N0") + " MB");

				@out.Add("");
				@out.Add("Graphics: ");
				foreach (KeyValuePair<string, int> gs in this._RenderedGraphics)
					@out.Add(gs.Key + ": " + gs.Value.ToString());
			}
			else if (this._DebugPage == 1)
			{
				@out.Add("(2/3) Inputs:");
				@out.Add("");
				foreach (GamePad g in GamePad.GetConnectedGamePads())
					@out.Add(g.Id.ToString() + ": " + g.Name);
			}
			else if (this._DebugPage == 2)
			{
				@out.Add("(3/3) Network:");
				@out.Add("");

				Func<User, string> usertostring = u =>
					{
						Bunch<string> o = new Bunch<string>();
						o.Add("IP: " + u.IP + ":" + u.Port.ToString());
						o.Add("> Speed: " + Meth.Up(u.Speed).ToString() + " kb/s");
						o.Add("> Latency: " + Meth.Up(u.Latency).ToString() + " ms");
						return string.Join("\n", o);
					};

				@out.Add("Client: " + (this.Node.Client.IsConnected ? "Connected\n" + usertostring(this.Node.Client.Server) : "Disconnected") + "\n");

				@out.Add("Server: " + (this.Node.Server.Started ? "Started" : "Not Started") + ", " + (this.Node.Server.IsConnected ? "Connected" : "Disconnected"));
				if (this.Node.Server.Started)
					@out.Add("IP: " + this.Node.Server.Ip + ":" + this.Node.Server.Port.ToString());
				if (this.Node.Server.IsConnected)
				{
					foreach (User u in this.Node.Server.Users)
						@out.Add(usertostring(u) + "\n");
				}
			}

			return string.Join("\n", @out);
		}

		#endregion

		#region Update

		public double SoundVolumeHalfLife = 1337;
		public double SoundDirectionHalfLife = 1337;
		private double _VolumeMaster = 1;
		public double VolumeMaster
		{
			get { return this._VolumeMaster; }
			set { this._VolumeMaster = Meth.Limit(0, value, 1); }
		}
		private double _VolumeSounds = 1;
		public double VolumeSounds
		{
			get { return this._VolumeSounds; }
			set { this._VolumeSounds = Meth.Limit(0, value, 1); }
		}
		private double _VolumeMusic = 1;
		public double VolumeMusic
		{
			get { return this._VolumeMusic; }
			set { this._VolumeMusic = Meth.Limit(0, value, 1); }
		}

		public void UpdateAll()
		{
			DateTime start = DateTime.Now;

			this.UpdateEvents();
			this.Update();
			this._FinishEvents();

			double time = (DateTime.Now - start).TotalMilliseconds;

			if (this.AutoRegulateRenderSync && !this._RenderSyncLock.HasValue)
				this._RenderSync = time < 20;
		}

		public void DoFrame()
		{
			this.UpdateAll();
			this.Render();
		}

		protected virtual void _UpdateEvents() { }
		public void UpdateEvents()
		{
			this._UpdateEvents();
			this._UpdateMouse();
		}

		private void _UpdateMouse()
		{
			this._MousePositionRaw = this._GetMousePositionRaw();

			Point s = this.FixedResolution ? this.Resolution : this.Size;
			Point c = (this.ScaleMode == ScaleMode.None) ? 0 : this.Camera;
			Point o = (this.ScaleMode == ScaleMode.None) ? new Point(0) : s / 2;
			
			if (!this.MouseLimitedToScreen || (this._MousePositionRaw.X >= 0 && this._MousePositionRaw.X < s.X && this._MousePositionRaw.Y >= 0 && this._MousePositionRaw.Y < s.Y))
			{
				this._MousePosition = c + (this._MousePositionRaw - o) / this.RealZoom;
				this._Hovered = true;
			}
			else
			{
				if (this.IsHovered)
				{
					foreach (Point m in Line.Trace(this._MousePositionRaw, MousePosition))
					{
						if (m.X >= 0 && m.X < s.X && m.Y >= 0 && m.Y < s.Y)
						{
							this._MousePosition = m + c - o;
							break;
						}
					}

					this._Hovered = false;
				}
			}
			
			MouseArea area = this.GetMouseAreaAt(this.MousePositionRaw);

			foreach (Entity e in this._RealEntities.Where(item => item.Interfacial))
			{
				foreach (MouseArea ma in e._MouseAreas)
					ma._IsHovered = (ma == area);
			}
		}



		private void _UpdateOverlapAreas()
		{
			Bunch<OverlapArea> areas = new Bunch<OverlapArea>();
			foreach (Entity e in this.RealEntities)
			{
				foreach (OverlapArea a in e.OverlapAreas)
				{
					a._Parent = e;
					a._OverlappedAreas = new Bunch<OverlapArea>();
					a.Position += e._RealPosition;
					areas.Add(a);
				}
			}

			foreach (OverlapArea a1 in areas)
			{
				foreach (OverlapArea a2 in areas.Where(item => item != a1))
				{
					if (!a1._OverlappedAreas.Contains(a2) && a1.Rect.Overlaps(a2.Rect))
					{
						a1._OverlappedAreas.Add(a2);
						a2._OverlappedAreas.Add(a1);
					}
				}
			}

			foreach (OverlapArea a in areas)
				a.Position -= a.Parent._RealPosition;
		}

		public void Update()
		{
			if (this.CurrentSaveFile != null)
				this.CurrentSaveFile.PlayTime++;
			
			this.Clock.Tick();
			
			this.Runtime++;
			if (this.Runtime % 30 == 0)
			{
				double dif = (DateTime.Now - this._LastUpdateTime).TotalMilliseconds;
				this._Updates.Add(1000 / dif * 30);
				if (this._Updates.Count > 2)
					this._Updates.RemoveAt(0);
				if (this._Updates.Count > 1)
					this._Ups = this._Updates.Average();
				this._LastUpdateTime = DateTime.Now;
			}

			while (this.RealEntities.Any(item => item._IsDead))
			{
				Entity e = this.RealEntities.First(item => item._IsDead);
				this._RemoveEntity(e);
			}

			this._AddEntities(this.Entities, new Bunch<Entity>());

			foreach (Entity entity in this._RealEntities.Where(item => item._IsDragged))
			{
				entity.OnDrag(entity.Position + (entity.LocalMousePosition - entity._DragPoint) * entity.Scale);
				entity._UpdateRealPosition();
			}

			foreach (Entity entity in this._RealEntities.Where(item => item._IsMoved))
			{
				if (entity.Movable/* || Debug.AllMovable*/)
				{
					foreach (FixedMouseJoint j in entity._MouseJoints)
						j.WorldAnchorB = MousePosition;
				}
			}

			foreach (Entity entity in RealEntities.Where(item => !item.Physical && item.Motion != 0))
			{
				entity.Motion += this.Gravity / 60 * entity.GravityFactor;
				entity.Position += entity.Motion / 60;
				entity.Motion *= entity.MotionDecay;
				entity._UpdateRealPosition();
			}

			while (this._UserIds.Count > 0)
			{
				this.OnUserJoin(this._UserIds[0]);
				this._UserIds.RemoveAt(0);
			}

			if (this._ServerConnect != null)
			{
				this.OnServerConnect(this.Node.Client.Server, this._ServerConnect);
				this._ServerConnect = null;
			}

			while (this._ClientConnects.Count > 0)
			{
				this.OnClientConnect(this._ClientConnects[0]);
				this._ClientConnects.RemoveAt(0);
			}

			while (this._Syncs.Count > 0)
			{
				this.OnSyncReceive(this._Syncs[0].Item1, this._Syncs[0].Item2);
				this._Syncs.RemoveAt(0);
			}

			this._UpdateOverlapAreas();

			this.OnUpdate();

			foreach (Entity entity in this._RealEntities.ToArray())
			{
				if (entity.InputsHidden != null)
					entity.InputsHidden._Update();
				if (!entity.PlayerId.HasValue && entity.AiAllowed)
					entity.OnAi();
				entity.Update();
				entity._UpdateRealPosition();
			}

			foreach (Entity entity in this._RealEntities.ToArray())
			{
				foreach (Sound s in entity._Sounds.ToArray())
				{
					if (!s._ToBePlayed && s.IsFinished)
						entity._Sounds.Remove(s);
					else
					{
						Vector p = entity.RealPosition - this.Camera;
						s._Direction = Meth.Sign(p.X) * (1 - Meth.Pow(0.5, Meth.Abs(p.X) / this.SoundDirectionHalfLife));
						s._VolumeDistance = Meth.Pow(0.5, p.Length / this.SoundVolumeHalfLife);
						s._VolumeGame = this._VolumeMaster * this._VolumeSounds;
						s._UpdateVolume();

						if (s._ToBePlayed)
						{
							s.Play();
							s._ToBePlayed = false;
						}
					}
				}
			}

			foreach (Entity entity in this.RealEntities)
				entity.Runtime++;

			foreach (Entity entity in this.RealEntities.Where(item => item._CarriedBy == null && item._Carrying.Count > 0))
			{
				foreach (Entity c in entity._Carrying)
					c._UpdateMotion();
			}

			this._World.Step((float)(this.PhysicsSpeed / 60));
			foreach (Entity entity in this.RealEntities.Where(item => item.Physical))
				entity._Position = entity._Body.Position;

			foreach (Entity entity in this.RealEntities)
			{
				foreach (Graphic g in entity.Graphics)
					g.Update();
			}

			if (this.Node.Connected && this.Node.IsServer && this.Node.HasSent("Sync") && (this._LastSync == null || (DateTime.Now - this._LastSync).TotalMilliseconds > 100))
				this._Sync();

			this._UpdateCamera();
		}

		private void _FinishEvents()
		{
			foreach (KeyValuePair<int, Controls> c in this.LocalPlayerControls)
				c.Value._Update();
			foreach (KeyValuePair<int, Controls> c in this.OnlinePlayerControls)
				c.Value._Update();
		}

		#endregion

		#region Input

		public Inputs Inputs;
		
		protected virtual Point _GetMousePositionRaw() { throw new Exception("Mouse not implemented yet."); }

		protected void _HandleKeyPress(Key _key, bool _pressed, bool _once = false)
		{
			this._KeysPressed[_key] = _pressed;
			this._UpdateMouse();

			foreach (Entity entity in this._RealEntities)
			{
				if (entity.IsFocused)
				{
					if (_pressed)
						entity.OnKeyPress(_key);
					else
						entity.OnKeyRelease(_key);
					break;
				}
			}

			if (!_pressed)
			{
				foreach (Entity e in this._RealEntities.Where(item => item.Interfacial))
				{
					foreach (MouseArea m in e._MouseAreas.Where(item => item._ClickedKey.HasValue && item._ClickedKey.Value == _key))
					{
						m._ClickedKey = null;
						if (m.OnRelease != null)
							m.OnRelease(_key);

						if (e._IsDragged)
						{
							e._IsDragged = false;
							e.OnDragEnd();
						}
					}
				}
			}
			else
			{
				MouseArea m = GetMouseAreaAt(this.MousePositionRaw);

				if (_key == Key.MouseLeft)
				{
					foreach (Entity e in RealEntities.Where(item => m == null || item != m.Entity))
						e.LoseFocus();
					if (m != null)
						m.Entity.GainFocus();
				}

				if ((m != null) && (m._ClickedKey == null) && m.ClickableBy.Contains(_key))
				{
					if (!(_key == Key.MouseDown || _key == Key.MouseUp))
						m._ClickedKey = _key;

					if (m.OnClick != null)
						m.OnClick(_key);

					if (m.Draggable && m.DragKey == _key)
					{
						Entity e = m.Entity;
						e._IsDragged = true;
						//e._DragPosition = e.Position;
						e._DragPoint = e.LocalMousePosition;
						e.OnDragStart();
					}
				}
			}

			//if (_pressed)
			//{
			//	Entity e = GetEntityAt(this.MousePosition);
			//	if (e != null && _key == Key.MouseLeft && (e.Movable/* || Debug.AllMovable*/))
			//	{
			//		FixedMouseJoint j = new FixedMouseJoint(e._Body, e.LocalMousePosition);
			//		j.WorldAnchorA = MousePosition;
			//		j.WorldAnchorB = j.WorldAnchorA;
			//		this._World.AddJoint(j);
			//		e._MouseJoints.Add(j);
			//		e._IsMoved = true;
			//	}
			//}

			if (!_pressed && _key == Key.MouseLeft)
			{
				foreach (Entity entity in this._RealEntities.OrderByDescending(item => item.RealZ))
				{
					if (entity._IsDragged && entity._MouseAreas.Any(item => item.IsDragged && item.DragKey == _key))
					{
						entity._IsDragged = false;
						entity._MouseAreas.First(item => item.IsDragged && item.DragKey == _key)._IsDragged = false;
						entity.OnDragEnd();
					}
					if (entity._IsMoved)
					{
						while (entity._MouseJoints.Count > 0)
						{
							this._World.RemoveJoint(entity._MouseJoints[0]);
							entity._MouseJoints.RemoveAt(0);
						}
						entity._IsMoved = false;
					}
				}
			}

			if (this.Keymap.Contains(_key))
			{
				KeyInfo info = this.Keymap.GetInfo(_key);
				if (info.LocalId < this.LocalPlayerCount)
				{
					if (this.LocalPlayerControls[info.LocalId][info.Name].IsPressed != _pressed)
					{
						this._PressKey(info.Name, info.LocalId, _pressed, _once, _local: true);

						if (this.Node.Connected)
							this.Node.Send(new MekaItem("Key", new MekaItem("Name", info.Name.ToString()), new MekaItem("Pressed", _pressed.ToString()), new MekaItem("ID", this.LocalPlayerIds[info.LocalId].ToString())));
					}
				}
			}
		}

		internal void _PressKey(string _key, int _id, bool _pressed, bool _once, bool _local)
		{
			Sandbox<int, Controls> cs = _local ? this.LocalPlayerControls : this.OnlinePlayerControls;

			if (!_once)
			{
				if (_pressed)
					cs[_id]._Press(_key);
				else
					cs[_id].Release(_key);
			}
			else
				cs[_id].PressOnce(_key);
		}

		public void PressKey(Key _key) => this._HandleKeyPress(_key, true);
		public void ReleaseKey(Key _key) => this._HandleKeyPress(_key, false);

		protected Sandbox<Key, bool> _KeysPressed = new Sandbox<Key, bool>();
		public bool IsKeyPressed(Key _key) =>  this._KeysPressed[_key];

		#endregion

		#region IO
		
		public string Path
		{
			get { return File.GetFolder(Assembly.GetCallingAssembly().Location); }
		}

		public virtual File SaveFile(byte[] _bytes, string _extension) { throw new Exception("IO not implemented yet."); }
		public virtual File OpenFile(params string[] _extensions) { throw new Exception("IO not implemented yet."); }

		public string SavePath;
		public bool UnsavedChanges;

		public File Save(byte[] _bytes, string _extension)
		{
			if (this.SavePath == null)
			{
				File @out = this.SaveFile(_bytes, _extension);
				if (@out != null)
				{
					this.SavePath = @out.Path;
					this.UnsavedChanges = false;
				}
				return @out;
			}
			else
			{
				File.Write(this.SavePath, _bytes);
				this.UnsavedChanges = false;
				return new File(this.SavePath);
			}
		}

		public File SaveAs(byte[] _bytes, string _extension)
		{
			File @out = this.SaveFile(_bytes, _extension);
			if (@out != null)
			{
				this.SavePath = @out.Path;
				this.UnsavedChanges = false;
			}
			return @out;
		}

		public File Open(params string[] _extensions)
		{
			File @out = this.OpenFile(_extensions);
			if (@out != null)
			{
				this.SavePath = @out.Path;
				this.UnsavedChanges = false;
			}
			return @out;
		}

		public virtual object GetClipboard() { throw new Exception("Clipboard not implemented yet."); }
		public virtual void SetClipboard(object _value) { throw new Exception("Clipboard not implemented yet."); }

		protected virtual void OnError(MekanikalError _error) { }

		protected virtual bool ThrowError(Exception _error)
		{
			MekanikalError e = (_error is MekanikalError) ? (MekanikalError)_error : new MekanikalError(_error);
			this.OnError(e);
			return false;
		}

		public string GetTimeStamp()
		{
			DateTime n = DateTime.Now;
			return n.Year.ToString() + "." + n.Month.ToString("00") + "." + n.Day.ToString("00") + " " + n.Hour.ToString("00") + "-" + n.Minute.ToString("00") + "-" + n.Second.ToString("00");
		}

		public string GetSaveName(string _name, string _extension)
		{
			//if (!File.Exists(this.Path + "\\Screenshots"))
			//	File.CreateFolder(this.Path + "\\Screenshots");

			int i = 0;
			string stamp = this.GetTimeStamp();
			string name = _name + stamp;
			while (File.Exists(name + " [" + i.ToString() + "]." + _extension))
				i++;
			return name + " [" + i.ToString() + "]." + _extension;
		}

		public static Func<byte[], ImageSource> LoadImageSource = bytes => { throw new Exception("yo cant do dat"); };
		public static Func<string, double, FontBase> LoadFont = (name, charsize) => { throw new Exception("yo cant do dat"); };
		public static Func<Color[,], byte[]> GifUnuglify = cs => { throw new Exception("yo cant do dat"); };
		public static Func<Bunch<byte[]>, byte[]> GifEncode = fs => { throw new Exception("yo cant do dat"); };
		public static Func<ImageSource, byte[]> ImageToBytes = img => { throw new Exception("yo cant do dat"); };
		public static Func<ImageSource, byte[]> ImageToBytesRgb = img => { throw new Exception("yo cant do dat"); };

		#endregion

		#region Saves

		internal SaveFile _CurrentSaveFile;
		public SaveFile CurrentSaveFile
		{
			get { return this._CurrentSaveFile; }
		}

		internal Bunch<Achievement> _Achievements = new Bunch<Achievement>();

		public Bunch<SaveFile> GetSaveFiles() => File.GetFiles(this.Path + "\\Internal\\Saves").Select(item => new SaveFile(this, item.Path));

		public void CreateNewSaveFile()
		{
			this._CurrentSaveFile = new SaveFile(this);

			Bunch<int> ids = this.GetSaveFiles().Select(item => item._Id).OrderBy(item => item);

			int id = 0;
			while (ids.Contains(id))
				id++;

			this._CurrentSaveFile._Id = id;
		}

		public void LoadSaveFile(SaveFile f) => this._CurrentSaveFile = f;

		public void SaveSaveFile()
		{
			if (this.CurrentSaveFile != null)
				this.CurrentSaveFile._Save();
		}

		#endregion
	}
}