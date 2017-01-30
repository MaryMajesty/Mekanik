using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;

namespace Mekanik
{
	class AnimationEditor : Alignable
	{
		private bool _AnimationShown;
		public bool AnimationShown
		{
			get { return this._AnimationShown; }

			set
			{
				if (value != this._AnimationShown)
				{
					this._AnimationShown = value;
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
		public Alignment AlignmentButtons;
		public AnimationEditorHidden Editor;
		
		public override void OnInitialization()
		{
			this.Children.Add(this.Editor = new AnimationEditorHidden());
			this.Children.Remove(this.Editor);
			
			this.Children.Add(this.AlignmentButtons = new Alignment
				(
					new Label("Animation:"),
					new Button("Add", () =>
						{
							this.Parent.ReleaseKey(Key.MouseLeft);
							File f = this.Parent.OpenFile("png");

							if (f != null)
							{
								this.Editor.SpritesheetSource = GameBase.LoadImageSource(f.Bytes);
								this.AnimationShown = true;
							}
						}),
					new Button("Open", () =>
						{
							this.Parent.ReleaseKey(Key.MouseLeft);
							File f = this.Parent.OpenFile("meka");

							if (f != null)
							{
								MekaItem file = MekaItem.FromBytesEncrypted(f.Bytes);
								this.Editor.SpritesheetSource = GameBase.LoadImageSource(file["Spriteset"].Children[0].Data);
								this.Editor.BoxSprites.Value = file["Spriteset"]["Count"].To<int>();
								this.Editor.BoxSpeed.Value = file["Speed"].To<int>();
								this.Editor.BoxRepeated.Checked = file.Contains("Repeated");
								this.AnimationShown = true;
							}
						})
				) { Spacing = 3 });
		}

		internal override double _GetRectWidth() => this.AnimationShown ? this.Editor.RectSize.X : this.AlignmentButtons.RectSize.X;
		internal override double _GetRectHeight() => this.AnimationShown ? this.Editor.RectSize.Y + 3 : this.AlignmentButtons.RectSize.Y;
	}

	class AnimationEditorHidden : Alignable
	{
		private ImageSource _SpritesheetSource;
		public ImageSource SpritesheetSource
		{
			get { return this._SpritesheetSource; }

			set
			{
				this._SpritesheetSource = value;
				this.Graphics.Clear();
				this.Graphics.Add(this.Spritesheet = new Image(this._SpritesheetSource) { Position = new Vector(this.Zooms.RectSize.X, 0) });

				this.BoxSprites.Value = 4;
				this.BoxSpeed.Value = 10;
				this.BoxRepeated.Checked = true;

				this._NeedsUpdate = true;
				this.Update();

				if (this.Spritesheet.Height < 50)
					this.SetZoom(Meth.Up(50.0 / this.Spritesheet.Height));
			}
		}
		public Image Spritesheet;
		public Alignment Zooms;
		public Alignment Bottom;
		public Animation Animation;
		public int Zoom = 1;

		public NumBox BoxSprites;
		public NumBox BoxSpeed;
		public Checkbox BoxRepeated;

		public int AnimSprites;
		public int AnimSpeed;
		public bool AnimRepeated;

		private bool _NeedsUpdate;

		public void SetZoom(int _zoom)
		{
			this.Zoom = Meth.Limit(1, _zoom, 8);

			this.Spritesheet.Scale = this.Zoom;
			this.Animation.Scale = this.Zoom;
			this.Animation.Position = new Vector(this.Bottom.RectSize.X, this.Spritesheet.Height * this.Spritesheet.Scale.Y);
			this.Bottom.Y = Meth.Max(this.Zooms.RectSize.Y, this.Spritesheet.Height * this.Zoom);
		}

		public override void OnInitialization()
		{
			this.Children.Add(this.Zooms = new Alignment(new Button("-", () => this.SetZoom(this.Zoom - 1)), new Button("+", () => this.SetZoom(this.Zoom + 1))) { Vertical = true });

			Bunch<Alignable> als = new Bunch<Alignable>();
			als.Add(new Alignment(new Label("Sprites:"), this.BoxSprites = new NumBox(4) { MinValue = 1, Digits = 2, MaxValue = 99 }));
			als.Add(new Alignment(new Label("Speed:"), this.BoxSpeed = new NumBox(10) { MinValue = 1, Digits = 3, MaxValue = 999 }));
			als.Add(new Alignment(this.BoxRepeated = new Checkbox(true) { Content = "Repeated" }, new Button("►", () => this.Animation.ForcePlay("Start"))));
			als.Add(new Alignment
				(
					new Button("Add", () => { }),
					new Button("Export", () =>
					{
						this.Parent.ReleaseKey(Key.MouseLeft);
						File f = this.Parent.SaveFile(this.GetAnimationFile().ToBytesEncrypted(), "meka");
						if (f != null)
							((AnimationEditor)this.Parents[0]).AnimationShown = false;
					}),
					new Button("Cancel", () => ((AnimationEditor)this.Parents[0]).AnimationShown = false)
				) { Spacing = 3 });
			this.Children.Add(this.Bottom = new Alignment(als) { Vertical = true });
		}

		public override void Update()
		{
			if (BoxSprites.Value != this.AnimSprites)
			{
				this.AnimSprites = BoxSprites.Value;
				this._NeedsUpdate = true;
			}
			if (BoxSpeed.Value != this.AnimSpeed)
			{
				this.AnimSpeed = BoxSpeed.Value;
				this._NeedsUpdate = true;
			}
			if (BoxRepeated.Checked != this.AnimRepeated)
			{
				this.AnimRepeated = BoxRepeated.Checked;
				this._NeedsUpdate = true;
			}

			if (this._NeedsUpdate)
			{
				this._NeedsUpdate = false;

				if (this.Graphics.Contains(this.Animation))
					this.Graphics.Remove(this.Animation);

				this.Animation = new Animation(new AnimationLibrary(new Dictionary<string, AnimationSource>() { { "Start", new AnimationSource(this.GetAnimationFile()) } })) { Position = new Vector(this.Bottom.RectSize.X, this.Spritesheet.Height * this.Spritesheet.Scale.Y), Scale = this.Spritesheet.Scale };
				this.Animation.Play("Start");

				this.Graphics.Add(this.Animation);
			}
		}

		public MekaItem GetAnimationFile()
		{
			MekaItem @out = new MekaItem("Animation", new List<MekaItem>());

			MekaItem spriteset = new MekaItem("Spriteset", new List<MekaItem>());
			spriteset.Children.Add(new MekaItem("Spritesheet", "png", this.SpritesheetSource.Bytes));
			spriteset.Children.Add(new MekaItem("Count", this.AnimSprites.ToString()));
			@out.Children.Add(spriteset);

			@out.Children.Add(new MekaItem("Speed", this.AnimSpeed.ToString()));

			if (this.AnimRepeated)
				@out.Children.Add(new MekaItem("Repeated"));

			return @out;
		}

		internal override double _GetRectWidth() => Meth.Max(this.Bottom.RectSize.X + this.Spritesheet.Width / this.AnimSprites * this.Spritesheet.Scale.X, this.Spritesheet.Width * this.Spritesheet.Scale.X);
		internal override double _GetRectHeight() => Meth.Max(this.Bottom.Y + this.Bottom.RectSize.Y, this.Spritesheet.Height * this.Spritesheet.Scale.Y * 2);
	}
}