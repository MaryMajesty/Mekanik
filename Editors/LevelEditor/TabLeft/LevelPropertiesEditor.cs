using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;

namespace Mekanik
{
	class LevelPropertiesEditor : Entity
	{
		public LevelEditor Editor;
		public Type LevelProperties;
		public PropertyEditor PropertyEditor;
		public Dictionary<string, object> Properties;

		public LevelPropertiesEditor(LevelEditor _editor, Type _levelproperties)
		{
			this.Editor = _editor;
			this.LevelProperties = _levelproperties;

			if (_levelproperties != null)
				this.Properties = PropertyReflector.GetPropertyValues(_levelproperties.GetConstructor(new Type[0]).Invoke(new object[0]));
		}

		public override void OnInitialization()
		{
			Bunch<Alignable> als = new Bunch<Alignable>();

			//als.Add(new Alignment(new Label("Width:"), this.WidthBox = new NumBox(this.Editor.TileEditor.Size.X) { Digits = 3, MinValue = 1 }));
			//als.Add(new Alignment(new Label("Height:"), this.HeightBox = new NumBox(this.Editor.TileEditor.Size.Y) { Digits = 3, MinValue = 1 }));

			if (this.LevelProperties != null)
				als.Add(this.PropertyEditor = new PropertyEditor(this.LevelProperties));

			this.Children.Add(new Alignment(als) { Vertical = true });
		}

		public void Load(byte[] _bytes)
		{
			if (this.LevelProperties != null)
			{
				MekaItem file = MekaItem.FromBytesEncrypted(_bytes);
				this.Properties = PropertySaver.Load(PropertyReflector.GetPropertyTypes(this.LevelProperties), file["Properties"].Children/*file.Contains("Info") ? file["Info"].Children : new List<MekaItem>()*/);
			}
		}

		public MekaItem Export() => new MekaItem("Properties", this.LevelProperties != null ? (PropertySaver.Save(this.IsInitialized ? this.PropertyEditor.GetProperties() : this.Properties)) : new List<MekaItem>());

		public override void Update()
		{
			//Point s = new Point(this.WidthBox.Value, this.HeightBox.Value);

			//if (!this.WidthBox.IsEdited() && !this.HeightBox.IsEdited() && s != this.Editor.TileEditor.Size)
			//{
			//	this.Editor.TileEditor.Size = s;
			//	this.Editor.LevelPreview.NeedsUpdate = true;
			//	this.Editor.LevelPreview.UpdateSize();
			//}

			if (this.Properties != null)
			{
				this.PropertyEditor.SetProperties(this.Properties);
				this.Properties = null;
			}
		}
	}
}