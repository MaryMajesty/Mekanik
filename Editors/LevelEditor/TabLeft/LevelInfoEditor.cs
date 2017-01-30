using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;

namespace Mekanik
{
	class LevelInfoEditor : Entity
	{
		public LevelEditor Editor;
		public TextBox BoxTitle;
		public TextBox BoxAuthor;
		public NumBox BoxWidth;
		public NumBox BoxHeight;
		public ScriptBox BoxOnLoad;
		public ScriptBox BoxOnEnter;
		public ScriptBox BoxOnExit;

		public LevelInfoEditor(LevelEditor _editor)
		{
			this.Editor = _editor;
		}

		public override void OnInitialization()
		{
			Bunch<Alignable> als = new Bunch<Alignable>();
			als.Add(new Alignment(new Label("Title:"), this.BoxTitle = new TextBox("untitled")));
			als.Add(new Alignment(new Label("Author:"), this.BoxAuthor = new TextBox("unknown")));
			als.Add(new Alignment(new Label("Width:"), this.BoxWidth = new NumBox(this.Editor.TileEditor.Size.X)));
			als.Add(new Alignment(new Label("Height:"), this.BoxHeight = new NumBox(this.Editor.TileEditor.Size.Y)));
			als.Add(new Alignment(new Label("OnLoad:"), this.BoxOnLoad = new ScriptBox(Zero.Script.Store(""))));
			als.Add(new Alignment(new Label("OnEnter:"), this.BoxOnEnter = new ScriptBox(Zero.Script.Store(""))));
			als.Add(new Alignment(new Label("OnExit:"), this.BoxOnExit = new ScriptBox(Zero.Script.Store(""))));
			this.Children.Add(new Alignment(als) { Vertical = true });
		}

		public void Load(byte[] _bytes)
		{
			MekaItem info = MekaItem.FromBytesEncrypted(_bytes)["Info"];
			this.BoxTitle.Content = info["Title"].Content;
			this.BoxAuthor.Content = info["Author"].Content;
			this.BoxWidth.Value = info["Width"].Content.To<int>();
			this.BoxHeight.Value = info["Height"].Content.To<int>();
			this.BoxOnLoad.Content = info["OnLoad"].Content;
			this.BoxOnEnter.Content = info["OnEnter"].Content;
			this.BoxOnExit.Content = info["OnExit"].Content;
		}

		public MekaItem Export()
		{
			MekaItem @out = new MekaItem("Info", new List<MekaItem>());
			@out.Children.Add(new MekaItem("Title", this.BoxTitle.Content));
			@out.Children.Add(new MekaItem("Author", this.BoxAuthor.Content));
			@out.Children.Add(new MekaItem("Width", this.BoxWidth.Value.ToString()));
			@out.Children.Add(new MekaItem("Height", this.BoxHeight.Value.ToString()));
			@out.Children.Add(new MekaItem("OnLoad", this.BoxOnLoad.Content));
			@out.Children.Add(new MekaItem("OnEnter", this.BoxOnEnter.Content));
			@out.Children.Add(new MekaItem("OnExit", this.BoxOnExit.Content));
			return @out;
		}

		public override void Update()
		{
			Point s = new Point(this.BoxWidth.Value, this.BoxHeight.Value);

			if (!this.BoxWidth.IsEdited && !this.BoxHeight.IsEdited && s != this.Editor.TileEditor.Size)
			{
				this.Editor.TileEditor.Size = s;
				this.Editor.LevelPreview.NeedsUpdate = true;
				this.Editor.LevelPreview.UpdateSize();
			}
		}
	}
}