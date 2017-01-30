using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	class LayerList : DragList<LayerItem>
	{
		public TileEditor Editor;
		public bool NeedsUpdate = true;
		public Layer NewLayer;
		public bool Changed;

		//public double Width
		//{
		//	get { return ((TabList)this.Parents[1]).InnerSize.X; }
		//}

		public LayerList(TileEditor _editor)
			: base()
		{
			this.Editor = _editor;

			this.MaxHeight = 10000;

			this.OnOrderChange = () =>
				{
					Layer l = this.Editor.Layer;
					this.Editor.Layers = this.Items.OrderBy(item => item._ListIndex).Select(item => item.Layer);

					if (this.NewLayer == null)
						this.Editor.CurLayer = this.Editor.Layers.IndexOf(l);
					else
					{
						this.Editor.SwitchToLayer(this.Editor.Layers.IndexOf(this.NewLayer));
						this.NewLayer = null;
					}

					this.Editor.UpdateOnionSkin();
					this.Changed = true;
				};

			this.OnItemAdd = pos =>
				{
					Layer l = new Layer(_editor.Areasets, this.Parent.Tilesize, _editor.DefaultTile) { Size = _editor.Size };
					this.NewLayer = l;

					return new LayerItem(_editor, l);
				};
		}

		public override void OnInitialization()
		{
			base.OnInitialization();

			this.Width = this.Editor.Editor.TabListRight.InnerSize.X;
			//this.SetItems(this.Editor.Layers.Select(item => new LayerItem(this.Editor, item)));
		}

		public override void Update()
		{
			if (this.NeedsUpdate)
			{
				this.NeedsUpdate = false;
				this.SetItems(this.Editor.Layers.Select(item => new LayerItem(this.Editor, item)));
			}

			base.Update();
		}
	}

	class LayerItem : DragItem
	{
		public TileEditor Editor;
		public Layer Layer;
		public Image[] Images = new Image[2];
		public Checkbox Delete;
		public Checkbox Main;
		public double Side = 40;
		public Rectangle Highlight;
		public MouseArea MouseArea;
		public VertexArray Frame;

		public Vector Size
		{
			get { return new Vector(this.Editor.Editor.TabListRight.InnerSize.X - this.Side, 100); }
		}
		public Rect FitRect
		{
			get { return (new Rect(0, this.Size)).MakeFit(this.Layer.Size); }
		}
		public Vector FitPosition
		{
			get { return this.FitRect.Position; }
		}
		public Vector FitScale
		{
			get { return this.FitRect.Size / (this.Layer.Size * this.Editor.Parent.Tilesize); }
		}

		public LayerItem(TileEditor _editor, Layer _layer)
		{
			this.Interfacial = true;

			this.Editor = _editor;
			this.Layer = _layer;

			this.Images[0] = new Image(new ImageSource(1, 1));
			this.Images[1] = new Image(new ImageSource(1, 1));

			//this.Image.Position = this.FitPosition;
			//this.Image.Scale = this.FitScale;

			this.Highlight = new Rectangle(0, new Vector(this.Editor.Editor.TabListRight.InnerSize.X, 100)) { Color = Color.White * 0.8 };

			this.Frame = new VertexArray(VertexArrayType.LinesStrip) { Color = Color.Black };
			this.Frame.Add(0, new Vector(1, 0), 1, new Vector(0, 1), 0);

			this._UpdateEntities();

			//this.Graphics.Add();
			//this.Graphics.Add(this.Cross = new VertexArray(VertexArrayType.Lines) { Position = new Vector(this.Size.X + this.Side / 2, this.Size.Y / 4), Color = Color.Red } );
			//this.Cross.Add(this.Side / -5);
			//this.Cross.Add(this.Side / 5);
			//this.Cross.Add(new Vector(this.Side / 5, -this.Side / 5));
			//this.Cross.Add(new Vector(-this.Side / 5, this.Side / 5));
		}

		public override double GetHeight() => this.Size.Y;

		public override void OnInitialization()
		{
			this.AddMouseArea(this.MouseArea = new MouseArea(new Rectangle(0, new Vector(this.Editor.Editor.TabListRight.InnerSize.X, 100))) { OnClick = key =>
				{
					this.StartDrag();
					int index = this.Editor.Layers.IndexOf(this.Layer);
					if (this.Editor.CurLayer != index)
					{
						this.Editor.SwitchToLayer(index);
						this.Editor.Editor.EntityEditor.NeedsToDeselect = true;
					}
				}, OnRelease = key =>
				{
					this.StopDrag();
					if (((LayerList)this.Parents[0]).Changed)
					{
						((LayerList)this.Parents[0]).Changed = false;
						this.Editor.Editor.LevelPreview.UpdatePreview();
					}
				} });

			this.Children.Add(this.Delete = new Checkbox(true)
				{
					Z = 1,
					Position = new Vector(this.Size.X + this.Side / 2 - Checkbox.Size / 2, this.Size.Y / 4 - Checkbox.Size / 2),
					OnUncheck = () =>
						{
							if (this.Layer == this.Editor.Layer)
							{
								if (((LayerList)this.Parents[0]).Items.Count <= this._ListIndex + 1)
									this.Editor.SwitchToLayer(this._ListIndex - 1);
								else
									this.Editor.SwitchToLayer(this._ListIndex + 1);
							}
							((LayerList)this.Parents[0]).RemoveItem(this);
						}
				});
			this.Children.Add(this.Main = new Checkbox(this.Editor.MainLayer == this.Layer)
				{
					Z = 1,
					Position = new Vector(this.Size.X + this.Side / 2 - Checkbox.Size / 2, this.Size.Y / 4 * 3 - Checkbox.Size / 2),
					OnCheck = () =>
						{
							foreach (LayerItem item in ((LayerList)this.Parents[0]).Items)
								item.Main.Checked = false;

							this.Editor.MainLayer = this.Layer;
							this.Main.Checked = true;
						}
				});

			Label l = new Label("Main:");
			l.Position = new Vector(this.Size.X + this.Side / 2 - l.Width / 2 + 2, this.Size.Y * 0.55 - l.Height / 2);
			this.Children.Add(l);
		}

		private void _UpdateEntities()
		{
			this.Graphics.Clear();
			this.Graphics.Add(this.Highlight);
			this.Graphics.Add(this.Frame);

			this.Graphics.Add(this.Images[0]);

			Bunch<Image> icons = this.Layer.EntityPreview;
			foreach (Image icon in icons)
			{
				icon.Position = this.FitPosition + (icon.Position * this.FitScale);
				icon.Scale *= this.FitScale;
				this.Graphics.Add(icon);
			}

			this.Graphics.Add(this.Images[1]);
		}

		public override void Update()
		{
			if (this.Layer.SizeChangedLayer/* || this.Layer.TilesChanged*/)
			{
				this.Layer.SizeChangedLayer = false;
				//this.Layer.TilesChanged = false;

				for (int i = 0; i <= 1; i++)
				{
					//PhotoCanvas c = new PhotoCanvas(this.FitRect.Size);
					//c.Draw(new Image(this.Layer.Previews[i]) { Scale = this.FitScale });
					//this.Images[i].Source = c.ImageSource.Clone();//this.Layer.Previews[i];
					this.Images[i].Source = this.Layer.Previews[i];
					this.Images[i].Scale = this.FitRect.Size / (this.Layer.Size * this.Parent.Tilesize);
					this.Images[i].Position = this.FitPosition;
				}

				this.Highlight.Size = new Vector(this.Editor.Editor.TabListRight.InnerSize.X, 100);

				this.Frame.Position = this.FitPosition;
				this.Frame.Scale = this.FitRect.Size;
			}

			if (this.Layer.EntitiesChanged)
			{
				this.Layer.EntitiesChanged = false;
				this._UpdateEntities();
			}
			
			this.Main.Enabled = this.Editor.MainLayer != this.Layer;
			this.Delete.Enabled = this.Editor.MainLayer != this.Layer;

			this.Highlight.Color = (this.Editor.Layer == this.Layer) ? Color.White * 0.9 : (this.MouseArea.IsHovered ? Color.White * 0.95 : Color.Transparent);
		}
	}
}