using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;

namespace Mekanik
{
	class AreasetEditor : Alignable
	{
		public Alignment AlignmentButtons;
		public AreasetEditorHidden Editor;

		private bool _EditorShown;
		public bool EditorShown
		{
			get { return this._EditorShown; }

			set
			{
				if (value != this._EditorShown)
				{
					this._EditorShown = value;
					if (value)
					{
						this.Children.Remove(this.AlignmentButtons);
						this.Children.Add(this.Editor);
					}
					else
					{
						this.Children.Remove(this.Editor);
						this.Children.Add(this.AlignmentButtons);
					}
				}
			}
		}

		public override void OnInitialization()
		{
			this.Children.Add(this.Editor = new AreasetEditorHidden());
			this.Children.Remove(this.Editor);

			this.Children.Add(this.AlignmentButtons = new Alignment
				(
					new Label("Areaset:"),
					new Button("Add", () =>
						{
							this.Parent.ReleaseKey(Key.MouseLeft);
							File f = this.Parent.OpenFile("png");

							if (f != null)
							{
								this.Editor.TilesetSource = GameBase.LoadImageSource(f.Bytes);
								this.EditorShown = true;
							}
						}),
					new Button("Open", () =>
						{
							this.Parent.ReleaseKey(Key.MouseLeft);
							File f = this.Parent.OpenFile("meka");

							if (f != null)
							{
								this.Editor.Load(MekaItem.FromBytesEncrypted(f.Bytes));
								this.EditorShown = true;
							}
						})
				) { Spacing = 3 });
		}

		internal override double _GetRectWidth() => this.EditorShown ? this.Editor.RectSize.X : this.AlignmentButtons.RectSize.X;
		internal override double _GetRectHeight() => this.EditorShown ? this.Editor.RectSize.Y : this.AlignmentButtons.RectSize.Y;
	}

	class AreasetEditorHidden : Alignable
	{
		private ImageSource _TilesetSource;
		public ImageSource TilesetSource
		{
			get { return this._TilesetSource; }

			set
			{
				this._TilesetSource = value;
				this.TileCount = this._TilesetSource.Size / this.Parent.Tilesize;
				
				this.Graphics.Remove(this.Tileset);
				this.Graphics.Add(this.Tileset = new Image(this.TilesetSource) { Position = new Vector(this.AlignmentZoom.RectSize.X, 0), Color = Color.White ^ 170 });

				this.Colset = new byte[this.TileCount.X, this.TileCount.Y][,];
				for (int x = 0; x < this.TileCount.X; x++)
				{
					for (int y = 0; y < this.TileCount.Y; y++)
						this.Colset[x, y] = new byte[this.Parent.TileCollisionResolution.X, this.Parent.TileCollisionResolution.Y];
				}

				this.ColPreviewSource = new ImageSource(this.TileCount * this.Parent.TileCollisionResolution);
				this.Graphics.Remove(this.ColPreview);
				this.Graphics.Add(this.ColPreview = new Image(this.ColPreviewSource) { Position = new Vector(this.AlignmentZoom.RectSize.X, 0), Scale = this.Parent.Tilesize / this.Parent.TileCollisionResolution });

				this.Zoom = (this._TilesetSource.Height < 200) ? 2 : 1;
				this.UpdateZoom(0);
			}
		}
		public Image Tileset;
		public Point TileCount;
		public byte[,][,] Colset;
		public ImageSource ColPreviewSource;
		public Image ColPreview;
		public MouseArea MouseArea;
		public Point LastPosition;
		public Alignment AlignmentZoom;
		public int Zoom = 1;
		public Alignment AlignmentButtons;

		public AreasetEditorHidden() { this.Interfacial = true; }

		public override void OnInitialization()
		{
			this.AddMouseArea(this.MouseArea = new MouseArea(new Rectangle(0, 1)) { ClickableBy = new Bunch<Key>(Key.MouseLeft, Key.MouseRight), OnClick = key =>
				{
					this.LastPosition = (this.LocalMousePosition - new Vector(this.AlignmentZoom.RectSize.X, 0)) / (this.Parent.Tilesize / this.Parent.TileCollisionResolution) / this.Zoom;
				} });
			
			this.Children.Add(this.AlignmentZoom = new Alignment(new Button("+", () => this.UpdateZoom(1)), new Button("-", () => this.UpdateZoom(-1))) { Vertical = true });
			this.Children.Add(this.AlignmentButtons = new Alignment(new Button("Add", () => { }), new Button("Export", this.Save), new Button("Cancel", () => ((AreasetEditor)this.Parents[0]).EditorShown = false)) { Spacing = 3 });
		}

		public void UpdateZoom(int _offset)
		{
			this.Zoom = Meth.Limit(1, this.Zoom + _offset, 4);

			this.Tileset.Scale = this.Zoom;
			this.ColPreview.Scale = (this.Parent.Tilesize / this.Parent.TileCollisionResolution) * this.Zoom;

			this.UpdateMouseArea();
		}

		//public Tuple<Point, Point> GetMousePosition()
		//{
		//	Point tile = this.LocalMousePosition / this.Parent.Tilesize;
		//	Point col = (this.LocalMousePosition - tile * this.Parent.Tilesize) / (this.Parent.Tilesize / this.Parent.TileCollisionResolution);
		//	return new Tuple<Point, Point>(tile, col);
		//}

		public override void Update()
		{
			if (this.MouseArea.IsClicked)
			{
				bool key = this.MouseArea.ClickedBy(Key.MouseLeft);
				
				Point m = (this.LocalMousePosition - new Vector(this.AlignmentZoom.RectSize.X, 0)) / (this.Parent.Tilesize / this.Parent.TileCollisionResolution) / this.Zoom;
				foreach (Point p in Line.Trace(this.LastPosition, m).Where(item => (new Rect(0, this.TileCount * this.Parent.TileCollisionResolution)).Contains(item)))
				{
					Point tile = p / this.Parent.TileCollisionResolution;
					Point col = p % this.Parent.TileCollisionResolution;

					this.Colset[tile.X, tile.Y][col.X, col.Y] = (byte)(key ? 1 : 0);
					this.ColPreviewSource[tile.X * this.Parent.TileCollisionResolution.X + col.X, tile.Y * this.Parent.TileCollisionResolution.Y + col.Y] = key ? Color.White : Color.Transparent;
				}
				this.LastPosition = m;
			}

			byte b = (byte)((Meth.Sin(this.Runtime / 120.0 * Meth.Tau) + 1) / 2 * 85);
			this.ColPreview.Color = new Color(b) ^ 170;
		}

		public void UpdateMouseArea()
		{
			this.MouseArea.Shape = new Rectangle(new Vector(this.AlignmentZoom.RectSize.X, 0), this.TilesetSource.Size * this.Zoom);
			this.AlignmentButtons.Position = new Vector(0, Meth.Max(this.AlignmentZoom.RectSize.Y, this.TilesetSource.Height * this.Zoom));
		}

		public ImageSource GetColset()
		{
			int pixelcount = Meth.Up(this.Parent.TileCollisionResolution.X * this.Parent.TileCollisionResolution.Y / 4);
			ImageSource @out = new ImageSource(this.TileCount.X * pixelcount, this.TileCount.Y);
			for (int x = 0; x < this.TileCount.X; x++)
			{
				for (int y = 0; y < this.TileCount.Y; y++)
				{
					Bunch<byte> bs = new Bunch<byte>();
					for (int ty = 0; ty < this.Parent.TileCollisionResolution.Y; ty++)
					{
						for (int tx = 0; tx < this.Parent.TileCollisionResolution.X; tx++)
							bs.Add(this.Colset[x, y][tx, ty]);
					}
					Bunch<Color> ps = new Bunch<Color>();
					while (bs.Count > 0)
					{
						while (bs.Count < 4)
							bs.Add(0);
						ps.Add(new Color(bs.SubBunch(0, 4)));
						bs = bs.SubBunch(4);
					}
					for (int i = 0; i < pixelcount; i++)
						@out[x * pixelcount + i, y] = ps[i];
				}
			}
			return @out;
		}

		public MekaItem Export()
		{
			MekaItem @out = new MekaItem("Areaset", new List<MekaItem>());
			@out.Children.Add(new MekaItem("Info", new List<MekaItem>() { new MekaItem("Tilesize", this.Parent.Tilesize.ToString()), new MekaItem("Colsize", this.Parent.TileCollisionResolution.ToString()) }));
			@out.Children.Add(new MekaItem("Tileset", this.TilesetSource.Bytes));
			@out.Children.Add(new MekaItem("Colset", this.GetColset().Bytes));
			return @out;
		}

		public void Save()
		{
			this.Parent.ReleaseKey(Key.MouseLeft);
			File f = this.Parent.SaveFile(this.Export().ToBytesEncrypted(), "meka");
			if (f != null)
				((AreasetEditor)this.Parents[0]).EditorShown = false;
		}

		public void Load(MekaItem _file)
		{
			Areaset a = new Areaset(_file, this.Parent.Tilesize, this.Parent.TileCollisionResolution);

			this.TilesetSource = GameBase.LoadImageSource(_file["Tileset"].Data);
			this.Colset = new byte[this.TileCount.X * this.Parent.TileCollisionResolution.X, this.TileCount.Y * this.Parent.TileCollisionResolution.Y][,];

			for (int i = 0; i < a.Cols.Count; i++)
			{
				Point p = new Point(i % this.TileCount.X, Meth.Down(i / this.TileCount.X));
				this.Colset[p.X, p.Y] = a.Cols[i].Select(item => (byte)(item ? 1 : 0));

				for (int cx = 0; cx < this.Parent.TileCollisionResolution.X; cx++)
				{
					for (int cy = 0; cy < this.Parent.TileCollisionResolution.Y; cy++)
						this.ColPreviewSource[p.X * this.Parent.TileCollisionResolution.X + cx, p.Y * this.Parent.TileCollisionResolution.Y + cy] = (this.Colset[p.X, p.Y][cx, cy] == 1) ? Color.White : Color.Transparent;
				}
			}
		}

		internal override double _GetRectWidth() => Meth.Max(this.AlignmentZoom.RectSize.X + this.Tileset.Width * this.Zoom, this.AlignmentButtons.RectSize.X);
		internal override double _GetRectHeight() => Meth.Max(this.AlignmentZoom.RectSize.Y, this.Tileset.Height * this.Zoom) + this.AlignmentButtons.RectSize.Y;
	}
}