using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	class EntitySelector : Entity
	{
		public Bunch<EntityType> Entities;
		public Point Size = new Point(12, 4);
		public Bunch<CollapseGroup> Groups = new Bunch<CollapseGroup>();
		public EntityEditor EntityEditor;

		public LevelEditor Editor
		{
			get { return (LevelEditor)this.Parents[2]; }
		}

		public EntitySelector(Bunch<EntityType> _entities, LevelEditor _editor)
		{
			//this.Interfacial = true;
			this.Entities = _entities;
			_editor.EntityEditor = this.EntityEditor = new EntityEditor();
		}

		public override void OnInitialization()
		{
			this.Children.Add(this.EntityEditor);

			Dictionary<string, Bunch<EntityTile>> groups = new Dictionary<string, Bunch<EntityTile>>();
			foreach (EntityType t in this.Entities.Where(item => item.Name != "Decoration"))
			{
				if (!groups.ContainsKey(t.Group))
					groups[t.Group] = new Bunch<EntityTile>();
				groups[t.Group].Add(new EntityTileNormal(t) { Types = this.Entities });
			}

			foreach (Areaset a in this.Parent.Areasets.Select(item => item.Value).Where(item => item.Decorations.Count > 0))
			{
				groups["Decoration " + a.Name] = new Bunch<EntityTile>();
				foreach (KeyValuePair<string, ImageSource> decoration in a.Decorations)
					groups["Decoration " + a.Name].Add(new EntityTileDecoration(a, decoration.Key) { Types = this.Entities });
			}

			foreach (KeyValuePair<string, Bunch<EntityTile>> g in groups.OrderBy(item => item.Key.GetAlphabetIndex()))
			{
				int height = Meth.Up(g.Value.Count / (double)this.Size.X);
				CollapseGroup c = new CollapseGroup(FontBase.Consolas, g.Key, new EntityGroup(g.Value, this.Editor.TileEditor, this.EntityEditor, new Point(this.Size.X, height))) { InnerHeight = height * this.Parent.Tilesize.Y * 2, Width = this.Size.X * this.Parent.Tilesize.X * 2 };
				this.Groups.Add(c);
				this.Children.Add(c);
			}

			//for (int i = 0; i < this.Entities.Count; i++)
			//	this.Graphics.Add(new Image(this.Entities[i].Icon) { Origin = 0.5, Position = this.Parent.Tilesize / 2 + this.Parent.Tilesize * new Vector(i % this.Size.X, Meth.Down(i / (double)this.Size.X)), Scale = Meth.Min(this.Parent.Tilesize.X / (double)this.Entities[i].Icon.Width, this.Parent.Tilesize.Y / (double)this.Entities[i].Icon.Height) });

			//this.Scale = 2;

			////this.Offset.X = -this.TileSize.X * this.Size.X;

			//this.AddMouseArea(this.MouseArea = new MouseArea(new Rectangle(0, this.Size * this.Parent.Tilesize))
			//	{
			//		OnClick = key =>
			//			{
			//				Point p = this.LocalMousePosition / this.Parent.Tilesize;
			//				int i = p.X + p.Y * this.Size.X;
			//				if (i < this.Entities.Count)
			//				{
			//					EntityIcon icon = new EntityIcon(this.Editor.TileEditor.Layer, this.Entities[i], true) { Position = this.Editor.TileEditor.LocalMousePosition };
			//					this.Editor.TileEditor.Layer.Entities.Add(icon);
			//					this.Editor.TileEditor.Children.Add(icon);

			//					this.Editor.EntityEditor.Select(null);
			//				}
			//			}
			//	});
		}

		public override void Update()
		{
			double y = 0;
			foreach (CollapseGroup g in this.Groups)
			{
				g.Y = y;
				y += g.Height;
			}

			this.EntityEditor.Y = y;
		}

		//public override void Update()
		//{
		//	this.X = Parent.Size.X;
		//}
	}
}