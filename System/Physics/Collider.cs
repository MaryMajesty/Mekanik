using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	public class Collider
	{
		public bool IsSensor;
		public Func<Collider, Collider, bool> OnCollision;
		public Action<Collider, Collider> OnSeparation;
		public string Tag;
		public double Density = 1;
		private Fixture _Fixture;

		public Bunch<Collider> ContactColliders = new Bunch<Collider>();
		public Bunch<Collider> SolidColliders = new Bunch<Collider>();

		private Vector _Scale = 1;
		public Vector Scale
		{
			get { return this._Scale; }

			set
			{
				this._Scale = value;
				if (this._Fixture != null)
				{
					this.Entity._Body.DestroyFixture(this._Fixture);
					this.BindToEntity(this._Entity);
				}
			}
		}
		public Bunch<Entity> Contacts
		{
			get
			{
				Bunch<Entity> @out = new Bunch<Entity>();
				foreach (Collider c in this.ContactColliders)
				{
					if (!@out.Contains(c.Entity))
						@out.Add(c.Entity);
				}
				return @out;
			}
		}
		public Bunch<Entity> SolidContacts
		{
			get
			{
				Bunch<Entity> @out = new Bunch<Entity>();
				foreach (Collider c in this.SolidColliders.Where(item => !item.IsSensor))
				{
					if (!@out.Contains(c.Entity))
						@out.Add(c.Entity);
				}
				return @out;
			}
		}
		public bool IsTouched
		{
			get { return this.ContactColliders.Count > 0; }
		}
		public bool IsTouchedSolid
		{
			get { return this.SolidColliders.Count > 0; }
		}
		private Entity _Entity;
		public Entity Entity
		{
			get { return this._Entity; }
		}
		private VertexGraphic _Shape;
		public VertexGraphic Shape
		{
			get { return this._Shape; }

			set
			{
				if (this._Fixture != null)
				{
					this.Entity._Body.DestroyFixture(this._Fixture);
					this._Shape = value;
					this.BindToEntity(this._Entity);
				}
			}
		}
		//private double _Friction = 1;
		//public double Friction
		//{
		//	get { return this._Friction; }

		//	set
		//	{
		//		if (this._Fixture != null)
		//		{
		//			this._Fixture.Friction = (float)value;
		//			this._Friction = value;
		//		}
		//	}
		//}
		private bool _Enabled = true;
		public bool Enabled
		{
			get { return this._Enabled; }

			set
			{
				if (this._Enabled != value)
				{
					this._Enabled = value;

					if (!value)
						this.Entity._Body.DestroyFixture(this._Fixture);
					else
						this.BindToEntity(this.Entity);
				}
			}
		}

		public Collider(VertexGraphic _shape) { this._Shape = _shape; }

		internal void BindToEntity(Entity _entity)
		{
			this._Entity = _entity;

			//try
			//{
			FarseerPhysics.Collision.Shapes.Shape s = this.Shape._ToShape(new Vector(0, 0), this.Scale, _density: this.Density);
			this._Fixture = _entity._Body.CreateFixture(s, this);
			//this._Fixture.Friction = (float)this.Friction;
			//this._Fixture = FixtureFactory.AttachPolygon(this.Shape.ToVertices(), 1, this.Entity.Body);

			this._Fixture.IsSensor = this.IsSensor;

			this._Fixture.OnCollision += (Fixture _me, Fixture _you, Contact _contact) =>
				{
					Collider me = (Collider)_me.UserData;
					Collider you = (Collider)_you.UserData;

					if ((me.Entity.CollidesWith == null) || me.Entity.CollidesWith.Contains(you.Entity.CollisionGroup)
						|| (you.Entity.CollidesWith == null) || you.Entity.CollidesWith.Contains(me.Entity.CollisionGroup))
					//if (me.Entity.CollidesWith.Contains(you.Entity.CollisionGroups) || you.Entity.CollidesWith.Contains(me.Entity.CollisionGroups))
					{
						bool b = (this.OnCollision == null) ? true : this.OnCollision(me, you);

						if (b)
						{
							if (!you.IsSensor)
								this.SolidColliders.Add(you);
							this.ContactColliders.Add(you);

							if (!me.IsSensor && you.IsSensor && you.Entity.GroundSensor == you/* && you.Entity.CarriedBy == null*/)
								me.Entity.StartCarrying(you.Entity);
						}

						return b;
					}
					else
						return false;
				};

			this._Fixture.OnSeparation += (Fixture _me, Fixture _you) =>
				{
					Collider me = (Collider)_me.UserData;
					Collider you = (Collider)_you.UserData;

					if ((me.Entity.CollidesWith == null) || me.Entity.CollidesWith.Contains(you.Entity.CollisionGroup)
						|| ((you.Entity.CollidesWith == null) || you.Entity.CollidesWith.Contains(me.Entity.CollisionGroup)))
					//if (me.Entity.CollidesWith.Contains(you.Entity.CollisionGroups) || you.Entity.CollidesWith.Contains(me.Entity.CollisionGroups))
					{
						//this.ContactColliders.Remove(you);
						if (!you.IsSensor)
							this.SolidColliders.Remove(you);
						this.ContactColliders.Remove(you);

						this.OnSeparation?.Invoke(me, you);

						if (!me.IsSensor && you.IsSensor && you.Entity.GroundSensor == you)
							me.Entity.StopCarrying(you.Entity);
					}
				};
				//if (this.OnCollision != null)
				//	this._Fixture.OnCollision += (Fixture _me, Fixture _you, Contact _contact) => this.OnCollision((Collider)_me.UserData, (Collider)_you.UserData);

				//this._Fixture.OnSeparation += (Fixture _me, Fixture _you) => { /*this._TouchNumber--; */this.ContactColliders.Remove((Collider)_you.UserData); };
				//if (this.OnSeparation != null)
				//	this._Fixture.OnSeparation += (Fixture _me, Fixture _you) => this.OnSeparation((Collider)_me.UserData, (Collider)_you.UserData);
			//}
			//catch { }
		}

		public bool Touches<T>() where T : Entity => this.Contacts.Any(item => item is T);
		public bool TouchesSolid<T>() where T : Entity => this.SolidContacts.Any(item => item is T);

		public bool Touches(string _collisiongroup) => this.Contacts.Any(item => item.CollisionGroup == _collisiongroup);
		public bool TouchesSolid(string _collisiongroup) => this.SolidContacts.Any(item => item.CollisionGroup == _collisiongroup);

		public bool Touches(Entity _entity) => this.Contacts.Any(item => item == _entity);
		public bool TouchesSolid(Entity _entity) => this.SolidContacts.Any(item => item == _entity);

		public T GetContact<T>() where T : Entity => (T)this.Contacts.First(item => item is T);

		public void Destroy()
		{
			if (this._Enabled && this._Fixture.Body != null)
				this.Entity._Body.DestroyFixture(this._Fixture);
		}
	}

	//public delegate void TouchDelegate(TouchEventArgs _args);
	//public delegate void CollisionDelegate(CollisionEventArgs _args);

	//public class Collider
	//{
	//	public bool IsObstacle;
	//	public Rect Rect;

	//	public double X
	//	{
	//		get { return Rect.X; }
	//		set { Rect.X = value; }
	//	}
	//	public double Y
	//	{
	//		get { return Rect.Y; }
	//		set { Rect.Y = value; }
	//	}
	//	public double Width
	//	{
	//		get { return Rect.Width; }
	//		set { Rect.Width = value; }
	//	}
	//	public double Height
	//	{
	//		get { return Rect.Height; }
	//		set { Rect.Height = value; }
	//	}
	//	public Vector Position
	//	{
	//		get { return Rect.Position; }
	//		set { Rect.Position = value; }
	//	}
	//	public Vector Size
	//	{
	//		get { return Rect.Size; }
	//		set { Rect.Size = value; }
	//	}

	//	public event TouchDelegate Touched;
	//	public event CollisionDelegate Collided;

	//	public Collider(Rect _rect, bool _isobstacle = false)
	//	{
	//		this.Rect = _rect;
	//		this.IsObstacle = _isobstacle;
	//	}

	//	public Collider(Vector _position, Vector _size, bool _isobstacle = false)
	//	{
	//		this.Rect = new Rect(_position, _size);
	//		this.IsObstacle = _isobstacle;
	//	}

	//	public Collider(double _x, double _y, double _width, double _height, bool _isobstacle = false)
	//	{
	//		this.Rect = new Rect(_x, _y, _width, _height);
	//		this.IsObstacle = _isobstacle;
	//	}

	//	//internal void Invoke(double _start, double _length, bool _positive, bool _vertical)
	//	//{
	//	//	if (Touched != null)
	//	//		Touched(_start, _length, _positive, _vertical);
	//	//}

	//	internal void Invoke(TouchEventArgs _args)
	//	{
	//		if (Touched != null)
	//			Touched(_args);
	//	}

	//	internal void Invoke(CollisionEventArgs _args)
	//	{
	//		if (Collided != null)
	//			Collided(_args);
	//	}

	//	public static Bunch<Collider> GetCircle(Vector _position, double _radius, int _quality, bool _isobstacle = false)
	//	{
	//		Bunch<Collider> @out = new Bunch<Collider>();
	//		for (int i = 0; i < _quality; i++)
	//		{
	//			double y = (i + 0.5) / _quality * 2 - 1;
	//			double angle = Meth.Asin(y);
	//			double x = Meth.Cos(angle);
	//			@out.Add(new Collider(_position + new Vector(x * -1 * _radius, y * _radius - _radius / _quality), new Vector(Meth.Abs(x) * _radius * 2, _radius * 2 / _quality), _isobstacle));
	//		}
	//		return @out;
	//	}

	//	public static Bunch<Collider> GetCircle(double _radius, int _quality, bool _isobstacle = false) { return GetCircle(new Point(0, 0), _radius, _quality, _isobstacle); }



	//	public static Collider operator *(Collider _one, Vector _two)
	//	{
	//		Collider @out = new Collider(_one.Position * _two, _one.Size * _two, _one.IsObstacle);
	//		if (_two.X < 0)
	//		{
	//			double x = @out.X;
	//			@out.X = @out.Width + x;
	//			@out.Width *= -1;
	//		}
	//		if (_two.Y < 0)
	//		{
	//			double y = @out.Y;
	//			@out.Y = @out.Height + y;
	//			@out.Height *= -1;
	//		}
	//		@out.Collided += _one.Collided;
	//		return @out;
	//	}
	//}
}