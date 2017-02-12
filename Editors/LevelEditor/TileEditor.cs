using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;
using Meka.ByteOperators;

namespace Mekanik
{
	class TileEditor : Entity
	{
		public Bunch<Layer> Layers = new Bunch<Layer>();
		public int CurLayer;
		public Layer MainLayer;
		public Dictionary<string, Areaset> Areasets;
		public Image Grid;
		public Image OnionSkinBelow;
		public Image OnionSkinAbove;
		public Image[] Backgrounds = new Image[2];
		public MouseArea MouseArea;
		public Point DrawStart;
		public Tuple<string, int> CurTile;
		public bool Enabled = true;
		public Tuple<string, int> DefaultTile;
		public VertexArray Frame;
		
		public Layer Layer
		{
			get { return this.Layers[this.CurLayer]; }
		}
		public LevelEditor Editor
		{
			get { return (LevelEditor)this.Parents[0]; }
		}
		public Point Size
		{
			get { return this.Layer.Size; }

			set
			{
				foreach (Layer l in this.Layers)
					l.Size = value;
				this.UpdateFrame();
			}
		}

		public TileEditor(Dictionary<string, Areaset> _areasets, Tuple<string, int> _defaulttile, Point _tilesize)
		{
			this.Interfacial = true;

			this.Areasets = _areasets;
			this.DefaultTile = _defaulttile;
			this.CurTile = _defaulttile;

			this.Layers.Add(this.MainLayer = new Layer(this.Areasets, _tilesize, _defaulttile));
			
			this.Graphics.Add(this.Frame = new VertexArray(VertexArrayType.LinesStrip) { Color = Color.Black });
			this.Graphics.Add(this.OnionSkinBelow = new Image(new ImageSource(1, 1)) { Color = Color.White ^ 85, Z = 1 });
			this.Graphics.Add(this.Backgrounds[0] = new Image(new ImageSource(1, 1)) { Z = 2 });
			this.Graphics.Add(this.Backgrounds[1] = new Image(new ImageSource(1, 1)) { Z = 5 });
			this.Graphics.Add(this.OnionSkinAbove = new Image(new ImageSource(1, 1)) { Color = Color.White ^ 85, Z = 6 });
			this.Graphics.Add(this.Grid = new Image(new ImageSource(1, 1)) { Shader = Shader.Grid, Z = 7, Visible = false });

			this.Grid.Shader["Color"] = Color.White * 0.5;
			this.Grid.Shader["TileSize"] = _tilesize;
		}

		public void UpdateFrame()
		{
			this.Frame.Vertices.Clear();
			this.Frame.Add(0, (this.Size * this.Parent.TileSize).OnlyX, this.Size * this.Parent.TileSize, (this.Size * this.Parent.TileSize).OnlyY, 0);
		}

		public void AddLayer(int _pos)
		{
			Layer l = new Layer(this.Areasets, this.Parent.TileSize, this.DefaultTile) { Size = this.Size };
			this.Layers = this.Layers.SubBunch(0, _pos) + l + this.Layers.SubBunch(_pos);
			this.CurLayer = _pos;
		}

		public void SwitchToLayer(int _pos)
		{
			foreach (EntityIcon icon in this.Children.GetTypes<EntityIcon>())
				this.Children.Remove(icon);

			this.CurLayer = _pos;
			this.Layer.SizeChangedEditor = true;

			foreach (EntityIcon icon in this.Layer.Entities)
				this.Children.Add(icon);

			this.UpdateOnionSkin();
		}

		public void UpdateOnionSkin()
		{
			for (int i = -1; i <= 1; i += 2)
			{
				if (this.CurLayer + i >= 0 && this.CurLayer + i < this.Layers.Count)
				{
					Renderer c = new Renderer(this.Size * this.Parent.TileSize);
					c.Draw(new Image(this.Layers[this.CurLayer + i].Previews[0]));
					c.Draw(new Image(this.Layers[this.CurLayer + i].Previews[1]));
					
					(new Bunch<Image>(this.OnionSkinBelow, this.OnionSkinAbove))[(i == -1) ? 0 : 1].Source = c.ImageSource;
				}
				else
					(new Bunch<Image>(this.OnionSkinBelow, this.OnionSkinAbove))[(i == -1) ? 0 : 1].Source = new ImageSource(1, 1);
			}
		}

		public override void OnInitialization()
		{
			this.Size = this.Editor.LevelSize;

			this.AddMouseArea(this.MouseArea = new MouseArea(new Rectangle(0, this.Size * this.Parent.TileSize))
				{
					ClickableBy = new Bunch<Key>(Key.MouseLeft, Key.MouseRight, Key.MouseMiddle, Key.MouseDown, Key.MouseUp),
					Draggable = true,
					DragKey = Key.MouseMiddle,
					OnClick = key =>
						{
							if (Editor.TabListRight.CurName == "Tiles")
							{
								if (key == Key.MouseLeft || key == Key.MouseRight)
								{
									if (this.Enabled)
										DrawStart = this.LocalMousePosition / this.Parent.TileSize;
								}
							}
							else if (key == Key.MouseLeft)
								Editor.EntityEditor.Select(null);

							if (key == Key.MouseDown || key == Key.MouseUp)
								this.Editor.LevelPreview.Zoom((key == Key.MouseDown) ? 0.5 : 2, this.LocalMousePosition);
						}
				});

			this.Scale = 2;
			this.Z = -4;
		}

		public override void Update()
		{
			if ((this.MouseArea.ClickedBy(Key.MouseLeft) || this.MouseArea.ClickedBy(Key.MouseRight)) && Editor.TabListRight.CurName == "Tiles")
			{
				Point c = this.LocalMousePosition / this.Parent.TileSize;
				foreach (Point p in Line.Trace(this.DrawStart, c))
					this.Layer[p.X, p.Y] = this.MouseArea.ClickedBy(Key.MouseLeft) ? this.CurTile : this.DefaultTile;
				this.DrawStart = c;

				this.Editor.LevelPreview.NeedsUpdate = true;
			}
			
			if (this.Layer.SizeChangedEditor)
			{
				this.Layer.SizeChangedEditor = false;
				this.Backgrounds[0].Source = this.Layer.Previews[0];
				this.Backgrounds[1].Source = this.Layer.Previews[1];
				this.Grid.Source = new ImageSource(this.Backgrounds[0].Source.Size);

				this.MouseArea.Shape = new Rectangle(0, this.Layer.Size * this.Parent.TileSize);
			}
			
			this.OnionSkinBelow.Visible = this.OnionSkinAbove.Visible = this.Editor.OnionSkin;
		}

		internal void _Load(byte[] _bytes)
		{
			foreach (Entity c in this.Children)
				c.Kill();

			MekaItem level = MekaItem.FromBytesEncrypted(_bytes);

			this.Layers = new Bunch<Layer>();

			Bunch<string> areasets = level["Areasets"].Children.Select(item => item.Content).ToBunch();

			foreach (MekaItem layer in level["Layers"].Children)
			{
				Layer l = new Layer(this.Areasets, this.Parent.TileSize, this.DefaultTile) { SizeChangedLayer = true };
				l.LoadFromImage(GameBase.LoadImageSource(layer["Tiles"].Data), areasets);

				foreach (MekaItem entity in layer["Entities"].Children)
				{
					EntityIcon i = new EntityIcon(l, Editor.EntityTypes.First(item => item.Name == entity.Name), _dragged: false) { LoadInfo = entity };
					//i.LoadFromItem(entity);
					l.Entities.Add(i);
					this.Children.Add(i);
				}

				this.Layers.Add(l);

				if (layer.Contains("Main"))
					this.MainLayer = l;
			}

			this.SwitchToLayer(this.Layers.IndexOf(this.MainLayer));
				
			this.Editor.LayerList.NeedsUpdate = true;
			this.Editor.LevelPreview.NeedsUpdate = true;

			this.UpdateFrame();

			//ImageSource tiles = new ImageSource(level["Tiles"].Data);
			//this.Size = tiles.Size;

			//for (int x = 0; x < tiles.Size.X; x++)
			//{
			//	for (int y = 0; y < tiles.Size.Y; y++)
			//		//this.Draw(new Point(x, y), Beth.FromEndian(tiles[x, y].Bytes.Sub(0, 3)));
			//		this.Layer[x, y] = Beth.FromEndian(tiles[x, y].Bytes.Sub(0, 3));
			//}

			//this.Editor.EntityEditor.Select(null);

			//foreach (MekaItem entity in level["Entities"].Children)
			//{
			//	EntityIcon e = new EntityIcon(Editor.EntityTypes.First(item => item.Name == entity.Name), _dragged: false);

			//	e.Position = entity["Settings"]["Position"].To<Vector>();

			//	foreach (MekaItem property in entity["Properties"].Children)
			//		e.Properties.First(item => item.Name == property.Name).Value = property.Content;

			//	this.Children.Add(e);
			//}
		}
	}
}