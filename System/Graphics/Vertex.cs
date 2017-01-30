using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public struct Vertex
	{
		public Vector Position;
		public Vector ImagePosition;
		public Color Color;

		public Vertex(Vector _position, Color _color)
		{
			this.Position = _position;
			this.ImagePosition = new Vector();
			this.Color = _color;
		}

		public Vertex(Vector _position, Vector _imageposition)
		{
			this.Position = _position;
			this.ImagePosition = _imageposition;
			this.Color = Color.White;
		}

		public Vertex(Vector _position, Vector _imageposition, Color _color)
		{
			this.Position = _position;
			this.ImagePosition = _imageposition;
			this.Color = _color;
		}

		public static bool operator ==(Vertex _one, Vertex _two)
		{
			return (_one.Position == _two.Position) && (_one.Color == _two.Color) && (_one.ImagePosition == _two.ImagePosition);
		}

		public static bool operator !=(Vertex _one, Vertex _two)
		{
			return !(_one == _two);
		}

		public static implicit operator Vertex(Vector @this) { return new Vertex(@this, Color.White); }
		//public static explicit operator SFML.Graphics.Vertex(Vertex @this) { return new SFML.Graphics.Vertex((SFML.System.Vector2f)@this.Position, (SFML.Graphics.Color)@this.Color, (SFML.System.Vector2f)@this.ImagePosition); }
	}
}