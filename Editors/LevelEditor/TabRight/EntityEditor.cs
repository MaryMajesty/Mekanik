using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;

namespace Mekanik
{
	class EntityEditor : Entity
	{
		public EntityIcon Icon;
		//public Dictionary<string, Func<string>> Properties;
		public PropertyEditor PropertyEditor;
		public bool NeedsToDeselect;
		public NumBox XBox;
		public NumBox YBox;
		public NumBox ZBox;
		public bool ChangedPosition;

		public void Select(EntityIcon _entity)
		{
			if (this.Icon != _entity)
			{
				if (this.Icon != null)
				{
					this.UpdateValues();
					this.Icon.Selected = false;
				}

				this.Icon = _entity;
				if (this.Icon != null)
					this.Icon.Selected = true;

				foreach (Entity c in this.Children)
					c.Kill();
				//this.Properties = new Dictionary<string, Func<string>>();

				Bunch<Alignable> als = new Bunch<Alignable>();

				if (_entity != null)
				{
					als.Add(new Alignment(new Label("X:"), this.XBox = new NumBox(Meth.Down(_entity.X))));
					als.Add(new Alignment(new Label("Y:"), this.YBox = new NumBox(Meth.Down(_entity.Y))));
					als.Add(new Alignment(new Label("Z:"), this.ZBox = new NumBox(Meth.Down(_entity.EntityZ)) { MinValue = -9999 }));
				}

				if (_entity != null && _entity.Type.Name != "Decoration")
					als.Add(this.PropertyEditor = new PropertyEditor(this.Icon.Properties.ToDictionary(item => item.Name, item => item.Type), this.Icon.Properties.ToDictionary(item => item.Name, item => (object)item.Value)));

				this.Children.Add(new Alignment(als) { Vertical = true });

				if (_entity != null)
				{
					Button b = new Button("Delete", () => this.Icon.RemoveFromLevel());
					b.Position = new Vector(((TabList)this.Parents[2]).InnerSize.X - b.RectSize.X, 0);
					this.Children.Add(b);
				}
			}
		}

		public override void Update()
		{
			if (this.Icon != null)
			{
				double tx = this.XBox.Value/* + this.Icon.Type.Origin.X * this.Icon.Icon.Width % 1*/;
				double ty = this.YBox.Value/* + this.Icon.Type.Origin.Y * this.Icon.Icon.Height % 1*/;

				if (!this.XBox._TextBox.IsEdited && !this.Icon.IsDragged && this.Icon.X != tx)
				{
					this.Icon.X = tx;
					this.ChangedPosition = true;
				}
				if (!this.YBox._TextBox.IsEdited && !this.Icon.IsDragged && this.Icon.Y != ty)
				{
					this.Icon.Y = ty;
					this.ChangedPosition = true;
				}

				if (!this.XBox.IsEdited)
					this.XBox.Value = Meth.Down(this.Icon.X);
				if (!this.YBox.IsEdited && this.YBox.Value != Meth.Down(this.Icon.Y))
					this.YBox.Value = Meth.Down(this.Icon.Y);

				if (!this.XBox.IsEdited && !this.YBox.IsEdited && this.ChangedPosition)
				{
					this.ChangedPosition = false;
					this.Icon.LevelEditor.LevelPreview.NeedsUpdate = true;
				}
				
				this.Icon.EntityZ = this.ZBox.Value;
			}

			if (this.Icon != null && this.Icon.Type.Name != "Decoration" && !this.PropertyEditor.IsEdited)
				this.UpdateValues();

			if (this.NeedsToDeselect)
			{
				this.NeedsToDeselect = false;
				this.Select(null);
			}
		}

		public void UpdateValues()
		{
			if (this.PropertyEditor != null)
			{
				foreach (KeyValuePair<string, object> p in this.PropertyEditor.GetProperties())
					this.Icon.Properties.First(item => item.Name == p.Key).Value = p.Value.ToString();
			}
		}
	}
}