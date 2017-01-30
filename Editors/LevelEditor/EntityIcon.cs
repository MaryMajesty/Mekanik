using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;

namespace Mekanik
{
	class EntityIcon : Entity
	{
		public EntityType Type;
		public MouseArea Area;
		public Image Icon;
		public Image IconOverlay;
		public Bunch<EntityPropertyInstance> Properties = new Bunch<EntityPropertyInstance>();
		public bool Dragged;
		public Layer Layer;
		public bool Selected;
		public bool StartedDragging;
		public int EntityZ;
		//public bool Loaded;
		public VertexArray TestButton;
		public MouseArea TestArea;
		public MekaItem LoadInfo;

		public LevelEditor LevelEditor
		{
			get { return (LevelEditor)this.Parents[1]; }
		}
		public TileEditor TileEditor
		{
			get { return (TileEditor)this.Parents[0]; }
		}
		public bool IsInsideLevel
		{
			get
			{
				Vector topleft = this.Position - this.Icon.Size * this.Type.Origin;
				Vector botright = this.Position + this.Icon.Size * (1 - this.Type.Origin);
				Vector size = this.TileEditor.Size * this.Parent.Tilesize;

				return !(botright.X < 0 || botright.Y < 0 || topleft.X > size.X || topleft.Y > size.Y);
			}
			//get { return (new Rect(0, this.TileEditor.Size * this.Parent.Tilesize)).ContainsE(this.Position); }
		}

		public EntityIcon(Layer _layer, EntityType _type, bool _dragged)
		{
			this.Interfacial = true;

			this.Layer = _layer;
			this.Layer.EntitiesChanged = true;
			this.Type = _type;
			this.Dragged = _dragged;
			
			this.Properties = _type.Properties.Select(item => new EntityPropertyInstance(item));
		}

		public void LoadFromItem(MekaItem _item)
		{
			this.Position = new Vector(_item["X"].To<double>(), _item["Y"].To<double>());
			this.EntityZ = _item["Z"].To<int>();

			foreach (MekaItem property in _item["Properties"].Children)
			{
				if (this.Properties.Any(item => item.Name == property.Name))
					this.Properties.First(item => item.Name == property.Name).Value = property.Content;
			}

			//this.Loaded = true;
		}

		public override void OnInitialization()
		{
			ImageSource src = this.Type.Name == "Decoration" ? this.Parent.Areasets.First(item => item.Value.Name == this.Properties.First(prop => prop.Name == "Areaset").Value).Value.Decorations[this.Properties.First(item => item.Name == "Name").Value] : this.Type.Icon;

			Image img0 = (new Image(src) { Origin = this.Type.Origin }).Extend();
			img0.Shader = Shader.Outline;
			this.Graphics.Add(this.Icon = img0);

			Image img1 = (new Image(src) { Origin = this.Type.Origin }).Extend();
			img1.Shader = Shader.Outline;
			img1.Z = 8;
			this.Graphics.Add(this.IconOverlay = img1);
			
			if (this.Type.Entrance)
			{
				VertexArray v = new VertexArray(VertexArrayType.Triangles);
				v.Add(new Vector(0, -8), new Vector(6, -4), new Vector(0, 0));
				v.Position = new Vector(this.Icon.Scale.X * this.Icon.Width * this.Icon.Origin.X * -1, this.Icon.Scale.Y * this.Icon.Height * (1 - this.Icon.Origin.Y)) + new Vector(1, -1);
				v.Z = 8;

				this.Graphics.Add(this.TestButton = v);
				this.AddMouseArea(this.TestArea = new MouseArea(v) { Z = 8, OnClick = key => { this.LevelEditor.Test(this.Properties.First(item => item.Name == "Identifier").Value); this.TestArea._ClickedKey = null; } });
			}



			this.AddMouseArea(this.Area = new MouseArea(this.Icon) { _IsDragged = this.Dragged, Draggable = true, ClickableBy = new Bunch<Key>(Key.MouseLeft, Key.MouseDown, Key.MouseUp), OnClick = key =>
				{
					if (key == Key.MouseLeft)
						this.LevelEditor.EntityEditor.Select(this);
					else
						this.LevelEditor.LevelPreview.Zoom((key == Key.MouseDown) ? 0.5 : 2, this.TileEditor.LocalMousePosition);
				} });

			this._IsDragged = this.Dragged;
			//this.Area._ClickedKey = Key.MouseLeft;

			if (this.LoadInfo == null)
			{
				this.EntityZ = (this.Type.Name == "Decoration") ? -1 : 0;

				if (this.Type.Name != "Decoration")
					this.Properties.First(item => item.Name == "Identifier").Value = this.Type.Name + this.Parents[0].Children.GetTypes<EntityIcon>().Count(item => item.Type.Name == this.Type.Name).ToString();
			}
			else
				this.LoadFromItem(this.LoadInfo);
		}

		public override void Update()
		{
			this.Z = this.StartedDragging ? 10 : (3.5 + Meth.InfToOne(this.EntityZ) / 2);
			
			if (TileEditor.Editor.TabListRight.CurName == "Entities")
			{
				this.Area.Enabled = true;

				this.Icon.Color = this.IsInsideLevel ? Color.White : Color.White ^ 170;
				this.Icon.Shader["Selected"] = (this.Selected && !this.StartedDragging) ? 1 : 0;
				this.Icon.Shader["Color0"] = Meth.Down(this.Runtime / 20.0) % 2 == 0 ? Color.Black : Color.White;
				this.Icon.Shader["Color1"] = Meth.Down(this.Runtime / 20.0) % 2 == 1 ? Color.Black : Color.White;

				this.IconOverlay.Color = Color.White ^ 85;
				this.IconOverlay.Shader["Selected"] = (this.Selected && !this.StartedDragging) ? 1 : 0;
				this.IconOverlay.Shader["Color0"] = Meth.Down(this.Runtime / 20.0) % 2 == 0 ? Color.Black : Color.White;
				this.IconOverlay.Shader["Color1"] = Meth.Down(this.Runtime / 20.0) % 2 == 1 ? Color.Black : Color.White;

				if (this.Type.Entrance)
				{
					this.TestArea.Enabled = true;
					this.TestButton.Visible = true;
					this.TestButton.Color = Color.Green * (this.TestArea.IsHovered ? 1 : 0.7);
				}
			}
			else
			{
				this.Area.Enabled = false;

				this.Icon.Color = Color.Transparent;
				this.Icon.Shader["Selected"] = 0;

				this.IconOverlay.Color = Color.White ^ 85;
				this.IconOverlay.Shader["Selected"] = 0;

				if (this.Type.Entrance)
				{
					this.TestArea.Enabled = false;
					this.TestButton.Visible = false;
				}
			}
		}

		public override void OnDrag(Vector _position)
		{
			if (!this.StartedDragging && _position != this.Position)
				this.StartedDragging = true;

			if (this.StartedDragging)
			{
				if (this.Type.Lock.HasValue == !Parent.IsKeyPressed(Key.LControl))
				{
					Vector l = (this.Type.Lock.HasValue ? this.Type.Lock.Value : (Vector)this.Parent.Tilesize);
					Point t = new Point(Meth.Round(_position.X / Parent.Tilesize.X - l.X), Meth.Round(_position.Y / Parent.Tilesize.Y - l.Y));
					this.Position = (t + l) * Parent.Tilesize;
				}
				else
					this.Position = (Point)_position;
				//this.Position = _position;

				this.Layer.EntitiesChanged = true;
			}
		}

		public override void OnDragEnd()
		{
			if (this.StartedDragging)
			{
				this.StartedDragging = false;

				if (!this.IsInsideLevel)
					this.RemoveFromLevel();
				this.LevelEditor.LevelPreview.NeedsUpdate = true;
			}
		}

		public void RemoveFromLevel()
		{
			this.Kill();
			this.LevelEditor.EntityEditor.Select(null);
			this.Layer.Entities.Remove(this);
			this.Layer.EntitiesChanged = true;
			this.LevelEditor.LevelPreview.NeedsUpdate = true;
		}

		public MekaItem Export()
		{
			MekaItem @out = new MekaItem(this.Type.Name, new List<MekaItem>());
			@out.Children.Add(new MekaItem("X", this.Position.X.ToString()));
			@out.Children.Add(new MekaItem("Y", this.Position.Y.ToString()));
			@out.Children.Add(new MekaItem("Z", this.EntityZ.ToString()));
			//@out.Children.Add(new MekaItem("Settings", new List<MekaItem>() { new MekaItem("Position", this.Position.ToString()), new MekaItem("Z", this.EntityZ.ToString()) }));
			@out.Children.Add(new MekaItem("Properties", this.Properties.Select(item => new MekaItem(item.Name, item.Value)).ToList()));
			return @out;
		}
	}
}