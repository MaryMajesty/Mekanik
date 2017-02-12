using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public abstract class Graphic
	{
		public bool LockToGrid;
		public Vector Position;
		public Vector Offset;
		public Vector Scale = 1;
		public double Rotation;
		public double Z;
		public Color Color = Color.White;
		public Vector Origin;
		public bool Visible = true;
		public BlendMode BlendMode = BlendMode.Alpha;
		public int Runtime;
		public int? Renderer;
		
		internal Vector _OriginalPosition;
		internal Vector _OriginalOffset;
		internal Vector _OriginalScale;
		internal double _OriginalRotation;
		internal double _OriginalZ;
		internal Color _OriginalColor;
		internal int? _OriginalRenderer;

		internal Entity _Parent;

		protected bool _Disposed;
		public bool Disposed
		{
			get { return this._Disposed; }
		}
		
		internal void _Set(Entity _entity)
		{
			this._OriginalPosition = this.Position;
			this._OriginalOffset = this.Offset;
			this._OriginalRotation = this.Rotation;
			this._OriginalScale = this.Scale;
			this._OriginalZ = this.Z;
			this._OriginalColor = this.Color;
			this._OriginalRenderer = this.Renderer;

			this.Position += this.Offset;

			this._Parent = _entity;
		}

		internal void _Reset()
		{
			this.Position = this._OriginalPosition;
			this.Offset = this._OriginalOffset;
			this.Rotation = this._OriginalRotation;
			this.Scale = this._OriginalScale;
			this.Z = this._OriginalZ;
			this.Color = this._OriginalColor;
			this.Renderer = this._OriginalRenderer;
		}

		public virtual void Update() { }

		public virtual void Dispose() { }
	}
}