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
			ImageSource overlay = new ImageSource(this.Parent.TileSize);
			int b = 0;
			Action<int, int> paint = (x, y) => overlay[x, y] = (new Bunch<Color>(Color.Black, Color.White))[b++ % 2];

			for (int x = 0; x < this.Parent.TileSize.X - 1; x++)
				paint(x, 0);
			for (int y = 0; y < this.Parent.TileSize.Y - 1; y++)
				paint(this.Parent.TileSize.X - 1, y);
			for (int x = this.Parent.TileSize.X - 1; x > 0; x--)
				paint(x, this.Parent.TileSize.Y - 1);
			for (int y = this.Parent.TileSize.Y - 1; y > 0; y--)
				paint(0, y);



			foreach (Areaset a in this.Areasets.Select(item => item.Value))
			{
				CollapseGroup c = new CollapseGroup(FontBase.Consolas, a.Name, new TileGroup(overlay, a, this.Editor.TileEditor, new Point(a.Size.X, a.Size.Y))) { Width = this.Size.X * this.Parent.TileSize.X * 2, InnerHeight = a.Size.Y * this.Parent.TileSize.Y * 2 };
				this.Groups.Add(c);
				this.Children.Add(c);
			}
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