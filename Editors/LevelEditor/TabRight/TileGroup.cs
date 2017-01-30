using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	class TileGroup : Entity
	{
		public Image Overlay;
		public Areaset Areaset;
		public TileEditor TileEditor;
		public Point Size;
		public int OverlayTime;
		public bool OverlayStatus;

		public TileGroup(ImageSource _overlay, Areaset _areaset, TileEditor _tileeditor, Point _size)
		{
			this.Interfacial = true;
			
			this.Areaset = _areaset;
			this.TileEditor = _tileeditor;
			this.Size = _size;

			this.Graphics.Add(this.Overlay = new Image(_overlay) { Scale = 2, Z = 1 });
		}

		public override void OnInitialization()
		{
			Renderer c = new Renderer(this.Parent.Tilesize * this.Size * 2);
			Bunch<Graphic> gs = new Bunch<Graphic>();
			for (int i = 0; i < this.Areaset.Tiles.Count; i++)
				gs.Add(new Image(this.Areaset.Tiles[i]) { Position = this.Parent.Tilesize * new Vector(i % this.Size.X, Meth.Down(i / (double)this.Size.X)) * 2, Scale = 2 });
			c.Draw(gs);
			this.Graphics.Add(new Image(c.ImageSource));

			this.AddMouseArea(new MouseArea(new Rectangle(0, this.Size * this.Parent.Tilesize * 2))
				{
					OnClick = key =>
						{
							Point p = this.LocalMousePosition / this.Parent.Tilesize / 2;
							int i = p.X + p.Y * this.Size.X;
							if (i < this.Areaset.Tiles.Count)
								this.TileEditor.CurTile = new Tuple<string, int>(this.Areaset.Name, i);
						}
				});
		}

		public override void Update()
		{
			this.Overlay.Visible = this.TileEditor.CurTile.Item1 == this.Areaset.Name;
			if (this.Overlay.Visible)
			{
				this.Overlay.Position = new Vector(this.TileEditor.CurTile.Item2 % this.Size.X, Meth.Down(this.TileEditor.CurTile.Item2 / (double)this.Size.X)) * this.Parent.Tilesize * 2;
				this.OverlayTime++;

				if (this.OverlayTime == 12)
				{
					this.OverlayTime = 0;
					this.OverlayStatus = !this.OverlayStatus;
				}

				if (this.OverlayStatus)
					this.Overlay.Position.X += this.Parent.Tilesize.X * 2;
				this.Overlay.Scale.X = (this.OverlayStatus ? -1 : 1) * 2;
			}
		}
	}
}