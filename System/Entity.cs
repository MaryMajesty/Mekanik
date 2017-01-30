using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Contacts;

namespace Mekanik
{
	public class Entity
	{
		[Editable(_default: "[none]")]public string Identifier;
		public double Z;
		public Bunch<Graphic> Graphics = new Bunch<Graphic>();
		public Bunch<Analog> Analogs = new Bunch<Analog>();
		public GameBase Parent;
		public bool Paused;
		public Analog XAnalog;
		public Bunch<Entity> Parents = new Bunch<Entity>();
		public bool AttachedToScreen;
		public Vector Offset;
		public bool Zoomable = true;
		internal Body _Body;
		internal Body _InterfaceBody;
		public bool Physical;
		public bool Interfacial;
		public int Runtime;
		internal bool _IsMoved;
		public string CollisionGroup;
		public Bunch<string> CollidesWith;
		public bool ApplyRotationToGraphics = true;
		public bool AiAllowed = true;
		public double MotionDecay = 1;
		public bool Visible = true;

		internal bool _Initialized;
		internal bool _Added;
		internal Vector _RealPosition;
		internal Vector _RealScale = 1;
		internal Vector _DragPoint;
		internal Bunch<Collider> _Colliders = new Bunch<Collider>();
		internal Bunch<MouseArea> _MouseAreas = new Bunch<MouseArea>();
		public ulong _Id;
		public bool Movable;
		internal Bunch<FixedMouseJoint> _MouseJoints = new Bunch<FixedMouseJoint>();
		public Inputs InputsHidden;
		public Bunch<OverlapArea> OverlapAreas = new Bunch<OverlapArea>();
		internal Bunch<Sound> _Sounds = new Bunch<Sound>();

		public int? LocalPlayerId;
		public int? Renderer;
		internal Bunch<Program> _Programs = new Bunch<Program>();
		private Variable _WrappedThisPrivate;
		internal Variable _WrappedThis
		{
			get
			{
				if (this._WrappedThisPrivate == null)
					this._WrappedThisPrivate = new Variable("this", Wrapper.WrapObject(this, _onlyzero: false));
				return this._WrappedThisPrivate;
			}
		}

		#region Properties

		internal Entity _CarriedBy;
		public Entity CarriedBy
		{
			get { return this._CarriedBy; }
		}
		public bool IsBeingCarried
		{
			get { return this._CarriedBy != null; }
		}
		internal Bunch<Entity> _Carrying = new Bunch<Entity>();
		public Bunch<Entity> Carrying
		{
			get { return this._Carrying.Clone(); }
		}
		public bool IsCarrying
		{
			get { return this._Carrying.Count > 0; }
		}
		//public Point IntermediateSize
		//{
		//	get { return (Point)this._IntermediateTexture.Size; }

		//	set
		//	{
		//		if (this._IntermediateTexture == null || (Point)this._IntermediateTexture.Size != value)
		//		{
		//			this._IntermediateTexture = new SFML.Graphics.RenderTexture((uint)value.X, (uint)value.Y);

		//			Shader s = null;
		//			if (this._IntermediateArray?.Shader != null)
		//				s = this._IntermediateArray.Shader;

		//			this._IntermediateArray = new VertexArray(VertexArrayType.Quads) { Shader = s };
		//			this._IntermediateArray.Add(0, 0);
		//			this._IntermediateArray.Add(value.OnlyX, value.OnlyX);
		//			this._IntermediateArray.Add(value, value);
		//			this._IntermediateArray.Add(value.OnlyY, value.OnlyY);
		//			this._IntermediateArray.Texture = new ImageSource(this._IntermediateTexture.Texture);

		//			this.UpdateIntermediate = true;
		//		}
		//	}
		//}
		//private Vector _IntermediatePosition;
		//public Vector IntermediatePosition
		//{
		//	get { return this._IntermediatePosition; }
		//	set { this._IntermediatePosition = value; }
		//}
		//public Color IntermediateColor
		//{
		//	get { return this._IntermediateArray.Color; }
		//	set { this._IntermediateArray.Color = value; }
		//}
		private int? _PlayerId;
		public int? PlayerId
		{
			get
			{
				if (this.LocalPlayerId.HasValue)
				{
					if (this.Parent.Node.Connected)
						return this.Parent.LocalPlayerIds[this.LocalPlayerId.Value];
					else
						return this.LocalPlayerId;
				}
				else
					return this._PlayerId;
			}
			
			set { this._PlayerId = value; }
		}
		internal Vector _Position;
		public Vector Position
		{
			get { return this._Position; }

			set
			{
				if (this._Position != value)
				{
					this._Position = value;
					this._RenderUpdateNeeded = true;

					if (this.Physical && this._Initialized)
					{
						bool s = this._Body.IsStatic;
						this._Body.IsStatic = true;
						this._Body.Position = this._Position;
						this._Body.IsStatic = s;
					}
				}
			}
		}
		private Vector _Scale = 1;
		public Vector Scale
		{
			get { return this._Scale; }

			set
			{
				if (this._Scale != value)
				{
					this._Scale = value;

					this._UpdateScale();

					foreach (Entity c in this.Children)
						c._UpdateScale();
				}
			}
		}
		private Color _Color = Color.White;
		public Color Color
		{
			get { return this._Color; }

			set
			{
				if (this._Color != value)
				{
					this._Color = value;
					this._RenderUpdateNeeded = true;
				}
			}
		}
		internal bool _RenderUpdateNeeded = true;
		internal bool _IsDead;
		public bool IsDead
		{
			get { return this._IsDead; }
		}
		//private Controls _Controls = new Controls();
		//public Controls Controls
		//{
		//	get { return this._Controls; }
		//}
		private Controls _Controls;
		public Controls Controls
		{
			get
			{
				if (this.LocalPlayerId.HasValue)
					return this.Parent.LocalPlayerControls[this.LocalPlayerId.Value];
				else if (this.PlayerId.HasValue)
					return this.Parent.OnlinePlayerControls[this.PlayerId.Value];
				else
				{
					if (this._Controls == null)
						this._Controls = new Controls();
					return this._Controls;
				}
			}
		}
		private Collider _GroundSensor;
		public Collider GroundSensor
		{
			get { return this._GroundSensor; }

			set
			{
				this._GroundSensor = value;
				this.AddCollider(value);
			}
		}
		public bool IsOnGround
		{
			get { return this.GroundSensor.IsTouchedSolid; }
		}
		public double X
		{
			get { return this.Position.X; }
			set { this.Position = new Vector(value, this.Position.Y); }
		}
		public double Y
		{
			get { return Position.Y; }
			set { Position = new Vector(this.Position.X, value); }
		}
		internal bool _Focused;
		public bool IsFocused
		{
			get { return this._Focused; }
		}
		public Vector LocalMousePosition
		{
			get
			{
				return this.IsAttachedToScreen
					? Parent.MousePositionRaw - this._RealPosition
					: (Parent.MousePosition - this._RealPosition) / this._RealScale;// * (this.IsZoomable ? 1 : Parent.RealZoom * this._RealScale);
			}
		}
		public Vector RealPosition
		{
			get { return this._RealPosition; }
		}
		public Vector ScreenPosition
		{
			get { return this._RealPosition - (Parent.Camera - Parent.Resolution / 2); }
		}
		internal bool _IsDragged;
		public bool IsDragged
		{
			get { return this._IsDragged; }
		}
		public double RealZ
		{
			get { return this.Parents.Sum(item => item.Z) + this.Z; }
		}
		public bool IsPaused
		{
			get { return this.Paused || this.Parents.Any(item => item.Paused); }
		}
		public bool IsZoomable
		{
			get { return !(!this.Zoomable || this.Parents.Any(item => !item.Zoomable)); }
		}
		public bool IsAttachedToScreen
		{
			get { return this.AttachedToScreen || this.Parents.Any(item => item.AttachedToScreen); }
		}
		public bool Rotatable
		{
			get { return !this._Body.FixedRotation; }
			set { this._Body.FixedRotation = !value; }
		}
		private double _GravityFactor = 0;
		public double GravityFactor
		{
			get { return this.Physical ? this._Body.GravityScale : this._GravityFactor; }

			set
			{
				if (this.Physical)
					this._Body.GravityScale = (float)value;
				else
					this._GravityFactor = value;
			}
		}
		private double _Rotation;
		public double Rotation
		{
			get
			{
				if (this.Physical && this._Initialized)
					this._Rotation = this._Body.Rotation;
				return this._Rotation;
			}

			set
			{
				this._Rotation = value;
				if (this.Physical && this._Initialized)
					this._Body.Rotation = (float)value;
				if (this.Interfacial)
					this._InterfaceBody.Rotation = (float)value;
			}
		}
		internal Vector _Motion;
		public Vector Motion
		{
			get { return (this.Physical && this._CarriedBy == null) ? (Vector)this._Body.LinearVelocity : this._Motion; }
			
			set
			{
				if (this.Physical && this._CarriedBy == null)
					this._Body.LinearVelocity = value;
				else
					this._Motion = value;
			}
		}
		public Vector ActualMotion
		{
			get { return this._Body.LinearVelocity; }
			set { this._Body.LinearVelocity = value; }
		}
		public double AngularMotion
		{
			get { return this._Body.AngularVelocity; }
			set { this._Body.AngularVelocity = (float)value; }
		}
		public bool Static
		{
			get { return this._Body.BodyType == BodyType.Static; }
			set { this._Body.BodyType = (value ? BodyType.Static : (this.Kinematic ? BodyType.Kinematic : BodyType.Dynamic)); }
		}
		public bool Kinematic
		{
			get { return this._Body.BodyType == BodyType.Kinematic; }
			set { this._Body.BodyType = (value ? BodyType.Kinematic : (this.Static ? BodyType.Static : BodyType.Dynamic)); }
		}
		private bool _ChildrenInitialized;
		private Bunch<Entity> _Children = new Bunch<Entity>();
		public Bunch<Entity> Children
		{
			get
			{
				if (!this._ChildrenInitialized)
				{
					this._Children._OnAdd = (Entity item) => Parent._AddEntities(item, this + this.Parents);
					this._Children._OnRemove = (Entity item) => Parent._RemoveEntity(item, false);
					this._ChildrenInitialized = true;
				}
				return this._Children;
			}
		}
		public double Friction
		{
			get { return this._Body.Friction; }
			set { this._Body.Friction = (float)value; }
		}
		public bool IsInitialized
		{
			get { return this._Initialized; }
		}

		#endregion

		public Entity() { }
		public Entity(params Graphic[] _graphics) { this.Graphics = _graphics; }

		public virtual void OnInitialization() { }
		public virtual void Update() { }
		public virtual void OnDrag(Vector _position) { this.Position = _position; }
		public virtual void OnDragStart() { }
		public virtual void OnDragEnd() { }
		public virtual void OnMouseClick(Key _key, Vector _position) { }
		public virtual void OnMouseRelease(Key _key, Vector _position) { }
		public virtual void OnKeyPress(Key _key) { }
		public virtual void OnKeyRelease(Key _key) { }
		public virtual void OnTextInput(string _text) { }
		public virtual void OnFocusGain() { }
		public virtual void OnFocusLoss() { }
		public virtual void OnRender() { }
		public virtual void AfterRender() { }
		public virtual void OnKill() { }
		public virtual void AfterKill() { }
		public virtual void OnAi() { }

		public void Kill()
		{
			this._IsDead = true;
			this._Added = false;

			foreach (Program p in this._Programs)
				p.UserAbort();
		}

		public void Focus()
		{
			foreach (Entity entity in Parent.RealEntities)
				entity.LoseFocus();

			if (!this._Focused)
			{
				this._Focused = true;
				this.OnFocusGain();
			}
		}
		public void LoseFocus()
		{
			if (this._Focused)
			{
				this._Focused = false;
				this.OnFocusLoss();
			}
		}
		public void GainFocus()
		{
			if (!this._Focused)
			{
				this._Focused = true;
				this.OnFocusGain();
			}
		}

		public bool CarriedByAny(Entity _entity)
		{
			if (this.IsBeingCarried)
			{
				if (this.CarriedBy == _entity)
					return true;
				else
					return this.CarriedBy.CarriedByAny(_entity);
			}
			else
				return false;
		}

		public void StartCarrying(Entity _entity)
		{
			if (!this._Carrying.Contains(_entity) && !this.CarriedByAny(_entity))
			{
				this._Carrying.Add(_entity);

				if (_entity.IsBeingCarried)
					_entity._CarriedBy.StopCarrying(_entity);
				_entity._CarriedBy = this;

				_entity._Motion = _entity.ActualMotion;
			}
		}

		public void StopCarrying(Entity _entity)
		{
			if (_entity._CarriedBy == this)
			{
				this._Carrying.Remove(_entity);
				_entity._CarriedBy = null;

				//_entity.ActualMotion = _entity._Motion;
			}
		}

		public void StopBeingCarried()
		{
			if (this._CarriedBy != null)
				this._CarriedBy.StopCarrying(this);
		}

		internal void _UpdateMotion()
		{
			if (this._Motion.Y > 0)
				this._Motion.Y = 0;
			this.ActualMotion = (this._CarriedBy != null ? this._CarriedBy.ActualMotion : 0) + this.Motion;
			foreach (Entity c in this._Carrying)
				c._UpdateMotion();
		}

		internal void _UpdateRealPosition()
		{
			Vector p = 0;
			Vector s = 1;
			foreach (Entity entity in Parents)
			{
				p += (entity.Position + entity.Offset) * s;
				s *= entity.Scale;
			}

			Vector rp = p + this.Position * s + this.Offset * s * this.Scale;//p + tp;
			if (this._RealPosition != rp)
			{
				this._RenderUpdateNeeded = true;
				this._RealPosition = rp;

				if (this.Interfacial && this._Initialized)
					this._InterfaceBody.Position = this._RealPosition;
			}
		}

		public void Clear()
		{
			foreach (Entity child in this.Children)
				child.Kill();
		}

		public void AddCollider(Collider _collider)
		{
			if (!this.Physical)
				throw new Exception("This entity was not declared physical.");
			else
			{
				_collider.BindToEntity(this);
				this._Colliders.Add(_collider);
			}
		}

		public void AddMouseArea(MouseArea _mousearea)
		{
			if (!this.Interfacial)
				throw new Exception("This entity was not declared interfacial.");
			else
			{
				_mousearea.BindToEntity(this);
				this._MouseAreas.Add(_mousearea);
			}
		}

		public void ApplyImpulse(Vector _impulse) { this._Body.ApplyLinearImpulse(_impulse); }
		public void ApplyImpulse(double _x, double _y) { this.ApplyImpulse(new Vector(_x, _y)); }

		public void ApplyMasslessImpulse(Vector _impulse) { this._Body.ApplyLinearImpulse(_impulse * this._Body.Mass); }
		public void ApplyMasslessImpulse(double _x, double _y) { this.ApplyMasslessImpulse(new Vector(_x, _y)); }

		public void ApplyAngularImpulse(double _strength) { this._Body.ApplyAngularImpulse((float)_strength); }

		internal void _UpdateScale()
		{
			Vector s = this._RealScale;

			this._RealScale = this.Scale;
			foreach (Entity parent in this.Parents)
				this._RealScale *= parent.Scale;

			if (s != this._RealScale)
			{
				this._RenderUpdateNeeded = true;
				this._UpdateRealPosition();

				foreach (MouseArea m in this._MouseAreas)
					m.Rebind();
			}

			//foreach (Entity c in this.Children)
			//	c._UpdateScale();
		}

		//public void PlaySound(SoundSource _sound)
		//{
			//Parent.PlaySound(_sound, this.Position - Parent.Camera);
		//}

		//public Connection Connect(Entity _entity, Vector _pos0, Vector _pos1)
		//{
		//	RevoluteJoint j = new RevoluteJoint(this._Body, _entity._Body, _pos0, _pos1);
		//	j.CollideConnected = true;
		//	//j.LimitEnabled = true;
		//	//j.LowerLimit = (float)(Meth.Tau / -32);
		//	//j.UpperLimit = (float)(Meth.Tau / 32);
		//	this.Parent._World.AddJoint(j);

		//	return new Connection(this.Parent._World, j, this, _entity);
		//}

		//public Connection ConnectAlt(Entity _entity, Vector _pos0, Vector _pos1, Vector _axis)
		//{
		//	PrismaticJoint j = new PrismaticJoint(this._Body, _entity._Body, _pos0, _pos1, _axis);
		//	j.ReferenceAngle = (float)(0);
		//	j.CollideConnected = true;

		//	this.Parent._World.AddJoint(j);

		//	return new Connection(this.Parent._World, j, this, _entity);
		//}

		//public void LoadScript(Script _script)
		//{
		//	this.MekaComp = new MekaComp(this, _script);
		//}

		public void ExecuteScript(Script _script, bool _includeentities, params Variable[] _vars)
		{
			Bunch<Variable> vars = new Bunch<Variable>(this._WrappedThis, Misc.Mekanik);
			vars.Add(_vars);

			if (_includeentities)
			{
				foreach (Entity e in this.Parent.RealEntities.Where(item => item.Identifier != null))
					vars.Add(new Variable(new string(e.Identifier.Select(item => (item == ' ') ? '_' : item).ToArray()), e._WrappedThis.Value));
			}

			Program p = new Program(_script) { OnCrash = cr =>
				{
					if (cr.Error.ErrorCode != Error.Execution.AbortedByUser.ErrorCode)
						throw cr.Error;
				} };
			p.Start(new Permissions(Permission.DllUsage), vars);
			this._Programs.Add(p);
		}

		//public void Connect(Vector _pos)
		//{
		//	JointFactory.CreateRevoluteJoint(
		//	this.Parent._World.AddJoint(j);
		//}

		internal Bunch<Graphic> _GetGraphics()
		{
			if (!this.Visible)
				return new Bunch<Graphic>();
			else
			{
				this._UpdateRealPosition();
				this._UpdateScale();

				foreach (Graphic g in this.Graphics)
					g._Set(this);

				//if (this.RenderIntermediately)
				//{
				//	Bunch<Graphic> @out = this.RenderGraphicsIntermediately ? new Bunch<Graphic>() : this.Graphics.Clone();
				//	Bunch<Graphic> gs = this.RenderGraphicsIntermediately ? this.Graphics.Clone() : new Bunch<Graphic>();

				//	if (this.Renderer.HasValue)
				//	{
				//		foreach (Graphic g in @out)
				//			g.Renderer = this.Renderer.Value;
				//	}

				//	foreach (Entity c in this.Children)
				//		gs.Add(c._GetGraphics());

				//	foreach (Graphic g in @out)
				//		this._UpdateGraphic(g);

				//	//if (this.AlwaysUpdateIntermediate || this.UpdateIntermediate)
				//	//{
				//	//	this.UpdateIntermediate = false;

				//	//	this._IntermediateTexture.Clear(Color.Transparent);
				//	//	foreach (Graphic g in gs.OrderBy(item => item.Z))
				//	//	{
				//	//		g.Position -= this._IntermediatePosition;
				//	//		g.Draw(this._IntermediateTexture);
				//	//	}
				//	//	this._IntermediateTexture.Display();
				//	//}

				//	////this._RenderArray.Texture = new ImageSource(this._RenderTexture.Texture);
				//	//this._IntermediateArray.Z = this.Z;
				//	//this._IntermediateArray.Scale = this.Scale;
				//	//this._IntermediateArray.Position = this.Position + this._IntermediatePosition;

				//	//@out.Add(this._IntermediateArray);
				//	return @out;
				//}
				//else
				{
					Bunch<Graphic> @out = this.Graphics.Where(item => item.Visible);

					foreach (Entity c in this.Children)
						@out.Add(c._GetGraphics());

					foreach (Graphic g in @out)
						this._UpdateGraphic(g);

					return @out;
				}
			}
		}

		internal void _UpdateGraphic(Graphic _graphic)
		{
			_graphic.Z += this.Z;

			_graphic.Position *= this.Scale;
			if (this.ApplyRotationToGraphics)
			{
				_graphic.Position = Vector.FromAngle(this.Rotation + _graphic.Position.Angle, _graphic.Position.Length);
				_graphic.Rotation += this.Rotation;
			}
			_graphic.Position += this.Position + this.Offset;

			_graphic.Scale *= this.Scale;
			
			_graphic.Color *= this.Color;

			if (this.Renderer.HasValue && !_graphic.Renderer.HasValue)
				_graphic.Renderer = this.Renderer;
		}

		internal void _ResetAll()
		{
			if (this.Visible)
			{
				foreach (Graphic g in this.Graphics)
					g._Reset();
				foreach (Entity e in this.Children)
					e._ResetAll();
			}
		}

		public Sound PlaySound(SoundSource _source)
		{
			Sound s = new Sound(_source) { _ToBePlayed = true };
			this._Sounds.Add(s);
			return s;
		}
	}
}