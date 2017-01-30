using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	class ScrollBackground : Entity
	{
		public LevelEditor Editor;
		public LevelPreview Preview;
		public Point Size;
		public MouseArea MouseArea;

		public ScrollBackground(LevelEditor _editor)
		{
			this.Z = -10;

			this.Interfacial = true;
			this.Editor = _editor;
			this.Preview = _editor.LevelPreview;
		}

		public override void OnInitialization()
		{
			this.AddMouseArea(this.MouseArea = new MouseArea(new Rectangle(0, 1)) { ClickableBy = new Bunch<Key>(Key.MouseDown, Key.MouseUp, Key.MouseLeft), OnClick = key =>
				{
					if (key == Key.MouseLeft)
						this.Editor.EntityEditor.Select(null);
					else
						this.Preview.Zoom((key == Key.MouseDown) ? 0.5 : 2, 0);
				} });
		}

		public override void Update()
		{
			if (this.Parent.Size != this.Size)
			{
				this.Size = this.Parent.Size;
				this.MouseArea.Shape = new Rectangle(0, this.Size);
			}
		}
	}
}