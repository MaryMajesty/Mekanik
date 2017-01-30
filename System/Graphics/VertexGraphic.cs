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
	public abstract class VertexGraphic : Graphic
	{
		public ImageSource Texture;
		public bool SmoothTexture;
		public Shader Shader;
		public Vector Offset;
		public Func<Vertex, Vertex> VertexModifyer;
		
		protected internal abstract VertexArray _ToVertexArray();
		internal VertexArray ToVertexArray()
		{
			VertexArray v = this._ToVertexArray();

			v.Position = this.Position;
			//v._OffsetPosition = this._OffsetPosition;
			v.Scale = this.Scale;
			v.Rotation = this.Rotation;
			v.Origin = this.Origin;
			v.Z = this.Z;
			v.Runtime = this.Runtime;
			v.Renderer = this.Renderer;

			v.Texture = this.Texture;
			v.SmoothTexture = this.SmoothTexture;
			v.Color = this.Color;
			v.Shader = this.Shader;
			v.BlendMode = this.BlendMode;
			v.Offset = this.Offset;
			v.VertexModifyer = this.VertexModifyer;

			return v;
		}

		internal virtual VertexArray _ToShapeVertexArray() { return this._ToVertexArray(); }
		internal VertexArray ToShapeVertexArray()
		{
			VertexArray v = this._ToShapeVertexArray();

			v.Position = this.Position;
			v.Scale = this.Scale;
			v.Rotation = this.Rotation;
			v.Origin = this.Origin;

			return v;
		}

		//protected internal override void Draw(SFML.Graphics.RenderTarget _target)
		//{
		//	if (this.Color.A > 0)
		//		this.ToVertexArray().DrawVertex(_target);
		//}

		internal Vertices _ToVertices(Vector _offset, Vector _scale)
		{
			//VertexArray v = this.ToShapeVertexArray();
			//return new Vertices(v.Vertices.Select(item => (Microsoft.Xna.Framework.Vector2)((item.Position - v.Size * this.Origin).AddAngle(this.Rotation) + this.Position)));
			Vector s = this.Scale;
			Vector p = this.Position;

			this.Position *= _scale;
			this.Position += _offset;
			this.Scale *= _scale;
			VertexArray v = this.ToShapeVertexArray();

			this.Scale = s;
			this.Position = p;
			//v.Position += _offset;
			//v.Scale *= _scale;
			return new Vertices(v._GetTransformedVertices().Select(item => (Microsoft.Xna.Framework.Vector2)item.Position));
		}

		internal Shape _ToShape(Vector _offset, Vector _scale, double _density = 1)
		{
			PolygonShape @out = new PolygonShape(this._ToVertices(_offset, _scale), (float)_density);
			return @out;
		}
	}
}