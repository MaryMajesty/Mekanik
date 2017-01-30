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
	public class Rectangle : Sizable
	{
		//private Color _Color;
		//private Vector _Size;

		//public new Color Color
		//{
		//	get { return _Color; }
		//	set { _Color = value; }
		//}
		//public new Vector Size
		//{
		//	get { return _Size; }
		//	set { _Size = value; } 
		//}
		public Rect Rect
		{
			get { return new Rect(this.Position, this.Size); }

			set
			{
				this.Position = value.Position;
				this.Size = value.Size;
			}
		}

		public Rectangle(double _x, double _y, double _width, double _height)
		{
			this.Position = new Vector(_x, _y);
			this.Size = new Vector(_width, _height);
		}

		public Rectangle(Vector _position, Vector _size)
		{
			this.Position = _position;
			this.Size = _size;
		}

		public Rectangle(Rect _rect)
		{
			this.Position = _rect.Position;
			this.Size = _rect.Size;
		}

		//public override void Draw(SFML.Graphics.RenderTarget _target, SFML.Graphics.RenderStates _states)
		//{
		//	_target.Draw(new SFML.Graphics.RectangleShape((SFML.System.Vector2f)this._Size) { Position = (SFML.System.Vector2f)this.Position, FillColor = this.Color, Rotation = (float)(this.Rotation / Meth.AngleUnit * 360), Scale = (SFML.System.Vector2f)this.Scale, Origin = (SFML.System.Vector2f)(this.Size * this.Origin) }, new SFML.Graphics.RenderStates(this.BlendMode.ToSfml()));
		//}

		protected internal override VertexArray _ToVertexArray() { return new VertexArray(VertexArrayType.Quads) { Vertices = new Bunch<Vertex>() { new Vector(0), new Vector(this.Width, 0), this.Size, new Vector(0, this.Height) } }; }
		//internal override VertexArray ToShapeVertexArray() { return this.ToVertexArray(); }

		//public override Shape ToShape()
		//{
		//	Vertices vs = new Vertices(4);
		//	vs.Add(this.Position / Game.Meter);
		//	vs.Add((this.Position + new Vector(this.Size.X, 0)) / Game.Meter);
		//	vs.Add((this.Position + this.Size) / Game.Meter);
		//	vs.Add((this.Position + new Vector(0, this.Size.Y)) / Game.Meter);

		//	return new PolygonShape(vs, 1);
		//	//return new PolygonShape(new Vertices(new Microsoft.Xna.Framework.Vector2[] { new Vector(0, 0), new Vector(100, 0), new Vector(100, 10), new Vector(0, 10) }), 1);
		//	//return new PolygonShape(new Vertices(vs.Select(item => (Microsoft.Xna.Framework.Vector2)(item / Game.Meter))), 1);
		//}
	}
}