using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Mekanik
{
	public class LevelPreview : Entity
	{
		public LevelEditor Editor;
		public Image Image;
		public bool NeedsUpdate;
		public bool Updated;
		public Image UpdatedImage;
		public double Height = 300;
		public int UpdateTimer;
		public VertexArray Background;
		public VertexArray Frame;
		public VertexArray Viewpoint;
		public MouseArea MouseArea;

		public Vector Size
		{
			get { return new Vector(this.Editor.TabListRight.OuterSize.X, this.Height); }
		}
		private Rect _FitRect;
		public Rect FitRect
		{
			get { return this._FitRect; }
			//get { return (new Rect(0, this.Size - 8)).MakeFit(this.Editor.TileEditor.Layer.Preview.Size); }
		}
		public Vector FitPosition
		{
			get { return this.FitRect.Position + 4; }
		}
		public Vector FitScale
		{
			get { return this.FitRect.Size / (this.Editor.TileEditor.Layer.Size * this.Parent.Tilesize); }
		}

		public LevelPreview(LevelEditor _editor)
		{
			this.Editor = _editor;
			this.Interfacial = true;

			this.UpdateSize();

			this.Graphics.Add(this.Image = new Image(new ImageSource(1, 1)));
			this.Graphics.Add(this.Frame = new VertexArray(VertexArrayType.LinesStrip) { Color = Color.Black });
			this.Graphics.Add(this.Viewpoint = new VertexArray(VertexArrayType.LinesStrip) { Color = Color.White * 0.5, Z = 1 });
			this.Frame.Add(this.FitPosition, this.FitPosition + this.FitRect.Size.OnlyX, this.FitPosition + this.FitRect.Size, this.FitPosition + this.FitRect.Size.OnlyY, this.FitPosition);
			this.Viewpoint.Add(0, new Vector(1, 0), 1, new Vector(0, 1), 0);

			Bunch<Vector> vs = new Bunch<Vector>(new Vector(4, 0), new Vector(this.Size.X - 4, 0), new Vector(this.Size.X, 4), new Vector(this.Size.X, this.Size.Y - 4), new Vector(this.Size.X - 4, this.Size.Y), new Vector(4, this.Size.Y), new Vector(0, this.Size.Y - 4), new Vector(0, 4));
			this.Graphics.Add(new VertexArray(VertexArrayType.Polygon) { Vertices = vs.Select(item => (Vertex)item) });
			this.Graphics.Add(new VertexArray(VertexArrayType.LinesStrip) { Vertices = vs.Select(item => (Vertex)item) + (Vertex)vs[0], Color = Color.Black });
		}

		public override void OnInitialization()
		{
			this.AddMouseArea(this.MouseArea = new MouseArea(new Rectangle(0, this.Size))
				{
					ClickableBy = new Bunch<Key>(Key.MouseLeft, Key.MouseDown, Key.MouseUp),
					OnClick = key =>
						{
							if (key == Key.MouseDown)
								this.Zoom(0.5, this.LocalMousePosition / this.Size * this.Editor.TileEditor.Size * this.Parent.Tilesize);
							if (key == Key.MouseUp)
								this.Zoom(2, this.LocalMousePosition / this.Size * this.Editor.TileEditor.Size * this.Parent.Tilesize);
						}
				});

			this.UpdatePreview();
		}

		public void Zoom(double _factor, Vector _position)
		{
			Vector os = this.Editor.TileEditor.Scale;
			Vector s = os * _factor;
			
			if (s.X < 0.125)
				s = 0.125;
			if (s.X > 32)
				s = 32;

			this.Editor.TileEditor.Scale = s;
			this.Editor.TileEditor.Position = this.Editor.TabListLeft.TotalSize.OnlyX + (this.Editor.TileEditor.Position - this.Editor.TabListLeft.TotalSize.OnlyX) * (s / os);
			this.Editor.TileEditor.Position += this.Editor.EditorSize * ((os.X < s.X) ? (os / s) : (s / os) / 2) * Meth.Sign(os.X - s.X);

			//Rect r = new Rect(0, this.Editor.TileEditor.Size * this.Parent.Tilesize * os);
			//Rect n = r.Zoom(_position, (s / os).X);
			//this.Editor.TileEditor.Position += n.Position;
		}

		public void UpdateSize()
		{
			this._FitRect = (new Rect(0, this.Size - 8)).MakeFit(this.Editor.TileEditor.Layer.Size * this.Editor.Parent.Tilesize);
		}

		public override void Update()
		{
			Vector size = this.Editor.TileEditor.Size * this.Parent.Tilesize * this.Editor.TileEditor.Scale;
			Vector esize = this.Editor.EditorSize;

			if (this.MouseArea.IsClicked)
				this.Editor.TileEditor.Position = -(this.LocalMousePosition - this.FitPosition) / this.FitRect.Size * size + this.Editor.EditorSize / 2 + new Vector(this.Editor.TabListLeft.TotalSize.X, 20);

			this.Editor.TileEditor.X = Meth.Limit(this.Editor.TabListLeft.TotalSize.X - size.X + this.Editor.EditorSize.X, this.Editor.TileEditor.X, this.Editor.TabListLeft.TotalSize.X);
			this.Editor.TileEditor.Y = Meth.Limit(-size.Y + this.Editor.EditorSize.Y + 20, this.Editor.TileEditor.Y, 20);
			
			if (size.X < esize.X)
				this.Editor.TileEditor.X = this.Editor.TabListLeft.TotalSize.X + (esize.X - size.X) / 2;
			if (size.Y < esize.Y)
				this.Editor.TileEditor.Y = (esize.Y - size.Y) / 2 + 20;

			if (this.NeedsUpdate && this.UpdateTimer < 10)
			{
				this.NeedsUpdate = false;
				this.UpdateTimer = 10;
				//Thread t = new Thread(this.UpdatePreview);
				//t.Start();
			}

			if (this.UpdateTimer > 0)
			{
				this.UpdateTimer--;
				if (this.UpdateTimer == 0)
				{
					this.UpdatePreview();
					//this.Parent.IndicationText = "Started!";
					//Thread t = new Thread(this.UpdatePreview);
					//t.Start();
				}
			}

			if (this.Updated)
			{
				this.Updated = false;

				this.Graphics.Remove(this.Image);
				this.Image = this.UpdatedImage;
				this.Graphics.Add(this.Image);

				this.Frame.Vertices.Clear();
				this.Frame.Add(this.FitPosition, this.FitPosition + this.FitRect.Size.OnlyX, this.FitPosition + this.FitRect.Size, this.FitPosition + this.FitRect.Size.OnlyY, this.FitPosition);
			}
			
			Vector topleft = this.FitRect.Size * ((this.Editor.TileEditor.Position - new Vector(this.Editor.TabListLeft.TotalSize.X, 20)) / this.Editor.TileEditor.Scale / this.Parent.Tilesize / -this.Editor.TileEditor.Size);
			Vector botright = topleft + this.FitRect.Size * this.Editor.EditorSize / this.Editor.TileEditor.Scale / this.Parent.Tilesize / this.Editor.TileEditor.Size;

			topleft.X = Meth.Max(topleft.X, 0);
			topleft.Y = Meth.Max(topleft.Y, 0);
			botright.X = Meth.Min(botright.X, FitRect.Width);
			botright.Y = Meth.Min(botright.Y, FitRect.Height);

			this.Viewpoint.Position = FitPosition + topleft;
			this.Viewpoint.Scale = botright - topleft;
		}

		public void UpdatePreview()
		{
			this.UpdateSize();

			Point size = this.Editor.TileEditor.Layer.Size * this.Editor.Parent.Tilesize;

			Renderer c = new Renderer(size);

			c.Draw(new Rectangle(0, size) { Color = Color.Black });
			c.Draw(new Rectangle(1, size - 2) { Color = Color.White });
			
			foreach (Layer l in this.Editor.TileEditor.Layers)
			{
				c.Draw(new Image(l.Previews[0]));
				foreach (Image e in l.EntityPreview)
					c.Draw(e);
				c.Draw(new Image(l.Previews[1]));
			}

			this.UpdatedImage = new Image(c.ImageSource);
			this.UpdatedImage.Position = this.FitPosition;
			this.UpdatedImage.Scale = this.FitScale;

			this.Updated = true;
		}
	}
}