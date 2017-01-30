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

using OpenTK.Graphics.OpenGL;

namespace Mekanik
{
	public class VertexArray : VertexGraphic// : ICollider
	{
		public Bunch<Vertex> Vertices = new Bunch<Vertex>();
		public VertexArrayType Type;
		private int _LastHash;

		private Vector _Size;
		public Vector Size
		{
			get
			{
				if (this.Vertices.GetHashCode() != this._LastHash)
				{
					this._LastHash = this.Vertices.GetHashCode();
					this._Size = new Vector(this.Vertices.Max(item => item.Position.X), this.Vertices.Max(item => item.Position.Y))/* - new Vector(this.Vertices.Min(item => item.Position.X), this.Vertices.Min(item => item.Position.Y))*/;
				}
				return this._Size;
			}
			//get { return  * this.Scale; }
		}

		public VertexArray(VertexArrayType _type)
		{
			this.Type = _type;
		}

		public override int GetHashCode()
		{
			return (new int[] { this.Position.GetHashCode(), this.Scale.GetHashCode(), this.Origin.GetHashCode(), this.Vertices.GetHashCode() }).GetHashCode();
		}

		public void Add(Vertex _vertex) { this.Vertices.Add(_vertex); }
		public void Add(Vector _position, Color _color) { this.Vertices.Add(new Vertex(_position, _color)); }
		public void Add(Vector _position, Vector _imageposition) { this.Vertices.Add(new Vertex(_position, _imageposition)); }

		public void Add(params Vector[] _positions)
		{
			foreach (Vector p in _positions)
				this.Vertices.Add(p);
		}

		//public void DrawVertex(SFML.Graphics.RenderTarget _target)
		//{
		//	if (this.Shader != null)
		//	{
		//		this.Shader["Runtime"] = this.Runtime;

		//		if (this.Texture != null)
		//		{
		//			this.Shader["Texture"] = this.Texture;
		//			this.Shader["Width"] = this.Texture.Width;
		//			this.Shader["Height"] = this.Texture.Height;
		//		}
		//	}

		//	//if (this.VertexModifyer != null || this._VertexArray == null || this.GetHashCode() != this._LastHash)
		//	{
		//		//Dictionary<string, SFML.Graphics.PrimitiveType> conv = new Dictionary<string, SFML.Graphics.PrimitiveType>();
		//		//for (int i = 0; i < 7; i++)
		//		//	conv[((SFML.Graphics.PrimitiveType)i).ToString()] = (SFML.Graphics.PrimitiveType)i;

		//		//this._VertexArray = new SFML.Graphics.VertexArray((this.Type == VertexArrayType.Polygon) ? SFML.Graphics.PrimitiveType.TrianglesFan : conv[this.Type.ToString()]);
		//		//SFML.Graphics.VertexArray v = new SFML.Graphics.VertexArray((this.Type == VertexArrayType.Polygon) ? SFML.Graphics.PrimitiveType.TrianglesFan : conv[this.Type.ToString()]);

		//		//this._VertexArray = this._GetPrimitive();

		//		//this._VertexArray = new SFML.Graphics.VertexArray(this._GetType());
		//		//this._AddTransformedVertices(this._VertexArray);

		//		//Bunch<Vertex> vs = this._GetTransformedVertices();
		//		//if (this.Type == VertexArrayType.Polygon)
		//		//	vs = new Vertex(new Vector(vs.Sum(item => item.Position.X), vs.Sum(item => item.Position.Y)) / vs.Count, vs[0].Color) + vs + vs[0];
		//		//foreach (Vertex vx in vs)
		//		//	v.Append((SFML.Graphics.Vertex)vx);

		//		//if (Texture == null)
		//		//	_target.Draw(v, new SFML.Graphics.RenderStates(this.BlendMode.ToSfml()) { Shader = (this.Shader == null) ? null : this.Shader._Shader });
		//		//else
		//		//	_target.Draw(v, new SFML.Graphics.RenderStates(this.Texture) { BlendMode = this.BlendMode.ToSfml(), Shader = (this.Shader == null) ? null : this.Shader._Shader });
		//	}

		//	if (Texture == null)
		//		_target.Draw(this._VertexArray, new SFML.Graphics.RenderStates(this.BlendMode.ToSfml()) { Shader = (this.Shader == null) ? null : this.Shader._Shader });
		//	else
		//	{
		//		this.Texture._Smooth = this.SmoothTexture;
		//		_target.Draw(this._VertexArray, new SFML.Graphics.RenderStates(this.Texture) { BlendMode = this.BlendMode.ToSfml(), Shader = (this.Shader == null) ? null : this.Shader._Shader });
		//	}

		//	//if (Debug)
		//	//	Log.Trace(this._LastHash.ToString());
		//}

		internal VertexArray _GetPrimitive()
		{
			VertexArray v = new VertexArray(this._GetType()) { Texture = this.Texture, BlendMode = this.BlendMode, Runtime = this.Runtime, SmoothTexture = this.SmoothTexture, Color = this.Color, Shader = this.Shader };
			foreach (Vertex vx in this._GetTransformedVertices())
				v.Add(vx);
			return v;
		}

		private VertexArrayType _GetType()
		{
			if (this.Type == VertexArrayType.Lines || this.Type == VertexArrayType.LinesStrip)
				return VertexArrayType.Lines;
			else
				return VertexArrayType.Triangles;
		}

		private Bunch<Vertex> _GetVertices()
		{
			Bunch<Vertex> @out = new Bunch<Vertex>();
			if (this.Type == VertexArrayType.Lines)
				@out = this.Vertices;
			else if (this.Type == VertexArrayType.LinesStrip)
			{
				for (int i = 0; i < this.Vertices.Count - 1; i++)
					@out.Add(this.Vertices[i], this.Vertices[i + 1]);
			}
			else if (this.Type == VertexArrayType.Triangles)
				@out = this.Vertices;
			else if (this.Type == VertexArrayType.TrianglesFan)
			{
				for (int i = 1; i < this.Vertices.Count - 1; i++)
					@out.Add(this.Vertices[0], this.Vertices[i], this.Vertices[i + 1]);
			}
			else if (this.Type == VertexArrayType.TrianglesStrip)
			{
				for (int i = 1; i < this.Vertices.Count - 2; i++)
					@out.Add(this.Vertices[0], this.Vertices[i], this.Vertices[i + 1]);
			}
			else if (this.Type == VertexArrayType.Polygon)
			{
				Vector p = 0;
				Vector ip = 0;
				Bunch<int> c = new Bunch<int>(0, 0, 0, 0);

				foreach (Vertex v in this.Vertices)
				{
					p += v.Position;
					ip += v.ImagePosition;
					for (int i = 0; i < 4; i++)
						c[i] += v.Color.Bytes[i];
				}

				Bunch<byte> bs = c.Select(item => (byte)(item / (double)this.Vertices.Count));

				Vertex m = new Vertex(p / this.Vertices.Count, new Color(bs)) { ImagePosition = ip / this.Vertices.Count };
				for (int i = 0; i < this.Vertices.Count; i++)
					@out.Add(m, this.Vertices[i], this.Vertices[(i + 1) % this.Vertices.Count]);
			}
			else if (this.Type == VertexArrayType.Quads)
			{
				for (int i = 0; i < this.Vertices.Count; i += 4)
					@out.Add(this.Vertices[i], this.Vertices[i + 1], this.Vertices[i + 2], this.Vertices[i + 2], this.Vertices[i + 3], this.Vertices[i]);
			}
			return @out;
			//else if (this.Type == 
		}

		private Vertex _TransformVertex(Vertex _vertex)
		{
			Vector p = ((_vertex.Position + this.Offset) - (this.Origin == 0 ? 0 : this.Size * this.Origin)) * this.Scale;
			p.Angle += this.Rotation;
			return new Vertex(this.Position + p, this.Color * _vertex.Color) { ImagePosition = _vertex.ImagePosition };
		}

		//internal void _AddTransformedVertices(SFML.Graphics.VertexArray _vertexarray)
		//{
		//	Vertex[] vs = this._GetVertices().ToArray();
		//	for (int i = 0; i < vs.Length; i++)
		//	{
		//		Vertex vertex = vs[i];
		//		if (this.VertexModifyer != null)
		//			vertex = this.VertexModifyer(vertex);
		//		//foreach (Vertex vertex in this.Vertices.ToArray())
		//		//{

		//		//Vector p = vertex.Position * this.Scale;
		//		//p -= this.Size * this.Origin * this.Scale;

		//		_vertexarray.Append((SFML.Graphics.Vertex)this._TransformVertex(vertex));
		//		//_vertexarray.Append((SFML.Graphics.Vertex)(new Vertex(this.Position + p, this.Color * vertex.Color) { ImagePosition = vertex.ImagePosition }));
		//	}
		//}

		internal Bunch<Vertex> _GetTransformedVertices() => this._GetVertices().Select(item => this._TransformVertex(item));
		//{
		//Bunch<Vertex> @out = new Bunch<Vertex>();

		//foreach (Vertex vertex in this._GetVertices())
		//{
		//	//@out.Add(vertex);
		//	Vector p = (vertex.Position + this._Offset) * this.Scale;
		//	p -= this.Size * this.Origin * this.Scale;
		//	p.Angle += this.Rotation;

		//	@out.Add(new Vertex(this.Position + p, this.Color * vertex.Color) { ImagePosition = vertex.ImagePosition });
		//}

		//return @out;
		//}

		protected internal override VertexArray _ToVertexArray()
		{
			VertexArray @out = new VertexArray(this.Type);

			@out.Position = this.Position;
			@out.Scale = this.Scale;
			@out.Rotation = this.Rotation;
			@out.Origin = this.Origin;
			@out.Z = this.Z;
			@out.Runtime = this.Runtime;
			@out.Renderer = this.Renderer;
			@out.Vertices = this.Vertices.Clone();

			@out.Texture = this.Texture;
			@out.SmoothTexture = this.SmoothTexture;
			@out.Color = this.Color;
			@out.Shader = this.Shader;
			@out.BlendMode = this.BlendMode;
			@out.Offset = this.Offset;
			@out.VertexModifyer = this.VertexModifyer;

			return @out;
		}
	}
}