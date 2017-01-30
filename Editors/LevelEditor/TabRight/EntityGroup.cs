using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	class EntityGroup : Entity
	{
		public Bunch<EntityTile> Types;
		public TileEditor TileEditor;
		public EntityEditor EntityEditor;
		public Point Size;

		public EntityGroup(Bunch<EntityTile> _types, TileEditor _tileeditor, EntityEditor _entityeditor, Point _size)
		{
			this.Interfacial = true;

			this.Types = _types;
			this.TileEditor = _tileeditor;
			this.EntityEditor = _entityeditor;
			this.Size = _size;
		}

		public override void OnInitialization()
		{
			//for (int i = 0; i < this.Types.Count; i++)
			//	this.Graphics.Add(new Image(this.Types[i].Icon) { Origin = 0.5, Position = this.Parent.Tilesize / 2 + this.Parent.Tilesize * new Vector(i % this.Size.X, Meth.Down(i / (double)this.Size.X)), Scale = Meth.Min(this.Parent.Tilesize.X / (double)this.Types[i].Icon.Width, this.Parent.Tilesize.Y / (double)this.Types[i].Icon.Height) });

			Renderer r = new Renderer(this.Parent.Tilesize * this.Size * 2);
			Bunch<Graphic> gs = new Bunch<Graphic>();
			for (int i = 0; i < this.Types.Count; i++)
				gs.Add(new Image(this.Types[i].Icon) { Origin = 0.5, Position = (this.Parent.Tilesize / 2 + this.Parent.Tilesize * new Vector(i % this.Size.X, Meth.Down(i / (double)this.Size.X))) * 2, Scale = Meth.Min(this.Parent.Tilesize.X / (double)this.Types[i].Icon.Width, this.Parent.Tilesize.Y / (double)this.Types[i].Icon.Height) * 2 });
			r.Draw(gs);
			this.Graphics.Add(new Image(r.ImageSource));

			this.AddMouseArea(new MouseArea(new Rectangle(0, this.Size * this.Parent.Tilesize * 2))
				{
					OnClick = key =>
						{
							Point p = this.LocalMousePosition / this.Parent.Tilesize / 2;
							int i = p.X + p.Y * this.Size.X;
							if (i < this.Types.Count)
							{
								EntityIcon icon = this.Types[i].GetIcon(this.TileEditor.Layer);
								icon.Position = this.TileEditor.LocalMousePosition;
								this.TileEditor.Layer.Entities.Add(icon);
								this.TileEditor.Children.Add(icon);

								Vector s = this.Types[i].Icon.Size;
								Rect n = (new Rect(0, this.Parent.Tilesize)).MakeFit(s);
								icon._DragPoint = ((this.LocalMousePosition % ((Vector)this.Parent.Tilesize * 2)) / 2 - n.Position) / n.Size * s - s * icon.Type.Origin;

								this.EntityEditor.Select(icon);
							}
						}
				});
		}
	}
}