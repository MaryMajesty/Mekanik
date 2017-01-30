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
	public class MouseArea
	{
		private Fixture _Fixture;
		public bool Draggable;
		public Key DragKey = Key.MouseLeft;
		public Bunch<Key> ClickableBy = new Bunch<Key>(Key.MouseLeft);
		public Action<Key> OnClick;
		public Action<Key> OnRelease;
		//public Func<DropData, bool> OnTryDrop = data => false;
		//public Action<DropData> OnDrop = data => { };
		internal Key? _ClickedKey;
		public double Z;

		//private bool _Enabled = true;
		//internal bool Enabled
		//{
		//	get { return this._Enabled; }

		//	set
		//	{
		//		this._Enabled = value;

		//		if (value)
		//			this._Fixture.Dispose();
		//		else
		//			this.BindToEntity(this.Entity);
		//	}
		//}
		private Entity _Entity;
		public Entity Entity
		{
			get { return this._Entity; }
		}
		internal bool _IsHovered;
		public bool IsHovered
		{
			get { return this._IsHovered; }
		}
		internal bool _IsDragged;
		public bool IsDragged
		{
			get { return this._IsDragged; }
		}
		private VertexGraphic _Shape;
		public VertexGraphic Shape
		{
			get { return this._Shape; }

			set
			{
				this._Shape = value;
				this.Rebind();
			}
		}
		public bool IsClicked
		{
			get { return this._ClickedKey != null; }
		}
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
						this.Entity._InterfaceBody.DestroyFixture(this._Fixture);
					else
						this.BindToEntity(this.Entity);
				}
			}
		}

		public MouseArea(VertexGraphic _shape)
		{
			this._Shape = _shape;
		}

		internal void BindToEntity(Entity _entity)
		{
			this._Entity = _entity;
			
			if (_entity._RealScale.X == 0 || _entity._RealScale.Y == 0)
				this._Fixture = null;
			else
			{
				this._Fixture = _entity._InterfaceBody.CreateFixture(this._Shape._ToShape(new Vector(-0.5, -0.5), _entity._RealScale), this);
				this._Fixture.IsSensor = true;
			}
		}

		internal void Rebind()
		{
			bool enabled = this.Enabled;

			if (!enabled)
				this.Enabled = true;

			if (this._Fixture != null)
				this.Entity._InterfaceBody.DestroyFixture(this._Fixture);
			
			this.BindToEntity(this._Entity);

			this.Enabled = enabled;
		}

		public bool ClickedBy(Key _key)
		{
			return this._ClickedKey == _key;
		}

		public void Destroy()
		{
			if (this._Enabled && this._Fixture != null)
				this.Entity._InterfaceBody.DestroyFixture(this._Fixture);
		}
	}
}