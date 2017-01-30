using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	class TileSelector : Entity
	{
		public Dictionary<string, Areaset> Areasets;
		public Point Size = new Point(12, 4);
		//public MouseArea MouseArea;
		//public ImageSource TileOverlaySource;
		//public Image TileOverlay;
		public Bunch<CollapseGroup> Groups = new Bunch<CollapseGroup>();

		public LevelEditor Editor
		{
			get { return (LevelEditor)this.Parents[2]; }
		}

		public TileSelector(Dictionary<string, Areaset> _areasets)
		{
			//this.Interfacial = true;
			this.Areasets = _areasets;

			//PhotoCanvas c = new PhotoCanvas(this.TileSize * this.Size);
			//for (int i = 0; i < this.Tiles.Count; i++)
			//	c.Draw(new Image(this.Tiles[i]) { Position = new Vector(i % this.Size.X, Meth.Down(i / this.Size.X)) * this.TileSize });
			//this.Graphics.Add(new Image(c.ImageSource.Clone()));
		}

		public override void OnInitialization()
		{
			ImageSource overlay = new ImageSource(this.Parent.Tilesize);
			int b = 0;
			Action<int, int> paint = (x, y) => overlay[x, y] = (new Bunch<Color>(Color.Black, Color.White))[b++ % 2];

			for (int x = 0; x < this.Parent.Tilesize.X - 1; x++)
				paint(x, 0);
			for (int y = 0; y < this.Parent.Tilesize.Y - 1; y++)
				paint(this.Parent.Tilesize.X - 1, y);
			for (int x = this.Parent.Tilesize.X - 1; x > 0; x--)
				paint(x, this.Parent.Tilesize.Y - 1);
			for (int y = this.Parent.Tilesize.Y - 1; y > 0; y--)
				paint(0, y);



			foreach (Areaset a in this.Areasets.Select(item => item.Value))
			{
				int height = Meth.Up(a.Tiles.Count / (double)this.Size.X);
				CollapseGroup c = new CollapseGroup(FontBase.Consolas, a.Name, new TileGroup(overlay, a, this.Editor.TileEditor, new Point(this.Size.X, height))) { Width = this.Size.X * this.Parent.Tilesize.X * 2, InnerHeight = height * this.Parent.Tilesize.Y * 2 };
				this.Groups.Add(c);
				this.Children.Add(c);
			}

			//this.AddMouseArea(this.MouseArea = new MouseArea(new Rectangle(0, this.Size * this.TileSize))
			//	{
			//		OnClick = key =>
			//			{
			//				int t = Meth.Down(this.LocalMousePosition.X / this.TileSize.X) + this.Size.X * Meth.Down(this.LocalMousePosition.Y / this.TileSize.Y);
			//				if (t < this.Tiles.Count)
			//					this.Editor.CurTile = t;
			//			}
			//	});

			//this.Graphics.Add(this.TileOverlay = new Image(this.TileOverlaySource));

			//this.Scale = 2;
			//this.Offset.X = -this.Size.X * this.TileSize.X;
		}

		public override void Update()
		{
			double y = 0;
			foreach (CollapseGroup g in this.Groups)
			{
				g.Y = y;
				y += g.Height;
			}
			//this.TileOverlay.Position = new Vector(this.Editor.CurTile % this.Size.X, Meth.Down(this.Editor.CurTile / this.Size.X)) * this.TileSize;
		}
	}
}