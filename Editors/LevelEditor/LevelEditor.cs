using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;

namespace Mekanik
{
	public class LevelEditor : Entity
	{
		public Action OnExit;
		public Tuple<string, int> DefaultTile;
		public Point LevelSize;
		
		internal TileEditor TileEditor;
		internal Bunch<EntityType> EntityTypes;
		TileSelector TileSelector;
		internal EntitySelector EntitySelector;
		FileMenu Menu;
		internal TabList TabListLeft;
		internal TabList TabListRight;
		internal EntityEditor EntityEditor;
		internal LayerList LayerList;
		public bool OnionSkin;
		public LevelPreview LevelPreview;
		public Type LevelProperties;
		internal LevelInfoEditor LevelInfoEditor;
		internal LevelPropertiesEditor LevelPropertiesEditor;

		internal bool StartFixedResolution;
		public ScaleMode StartScaleMode;
		internal bool StartMouseLimitedToScreen;
		internal bool StartUseMultipleRenderers;

		public Tuple<string, int> CurTile
		{
			get { return this.TileEditor.CurTile; }
			set { this.TileEditor.CurTile = value; }
		}
		public Vector EditorSize
		{
			get { return new Vector(this.Parent.Width - this.TabListLeft.TotalSize.X - this.TabListRight.TotalSize.X, this.Parent.Height - 22); }
		}

		public LevelEditor(Point _levelsize, Tuple<string, int> _defaulttile, Type _levelproperties = null)
		{
			this.LevelSize = _levelsize;
			this.DefaultTile = _defaulttile;
			this.LevelProperties = _levelproperties;

			this.AttachedToScreen = true;
		}
		
		public override void OnInitialization()
		{
			this.Parent._LevelEditor = this;
			
			this.EntityTypes = this.Parent.EntityTypes.Select(item => new EntityType(item));

			this.Children.Add(this.Menu = new FileMenu
				(
					new Selectable("Exit", () => this.Exit()),
					new Selectable("Save", () => this.Parent.Save(this._Export(), "meka")),
					new Selectable("Save As", () => this.Parent.SaveAs(this._Export(), "meka")),
					new Selectable("Open", () =>
						{
							File f = this.Parent.Open("meka");
							if (f != null)
							{
								byte[] bs = f.Bytes;

								this.TileEditor._Load(bs);
								this.LevelInfoEditor.Load(bs);
								this.LevelPropertiesEditor.Load(bs);
								this.EntityEditor.Select(null);
							}
						}),
					new Selectable("Onion Skin", () => this.OnionSkin = !this.OnionSkin),
					new Selectable("Grid", () => this.TileEditor.Grid.Visible = !this.TileEditor.Grid.Visible)
				) { Z = 8 });
			this.Children.Add(this.TileEditor = new TileEditor(this.Parent.Areasets, this.DefaultTile, this.Parent.TileSize) { Parent = this.Parent });

			this.Children.Add(this.TabListLeft = new TabList
				(
					new TabInfo("Info", this.LevelInfoEditor = new LevelInfoEditor(this)),
					new TabInfo("Properties", this.LevelPropertiesEditor = new LevelPropertiesEditor(this, this.LevelProperties)),
					new TabInfo("Resources", new Alignment(new AnimationEditor(), new AreasetEditor()) { Vertical = true })
				) { InnerSize = new Vector(32 * 12, 32 * 16), Position = new Vector(0, 22), Z = 8 });

			//this.Parent.Clock.Add(1, () => this.TabListLeft.SelectTab(1));
			//this.Parent.Clock.Add(3, () => this.TabListLeft.SelectTab(0));

			this.Children.Add(this.TabListRight = new TabList
				(
					new TabInfo("Tiles", this.TileSelector = new TileSelector(this.Parent.Areasets)),
					new TabInfo("Entities", this.EntitySelector = new EntitySelector(this.EntityTypes, this)),
					new TabInfo("Layers", this.LayerList = new LayerList(this.TileEditor))
				) { InnerSize = new Vector(32 * 12, 32 * 16), AlignRight = true, Z = 8 });

			this.Children.Add(this.LevelPreview = new LevelPreview(this) { Z = 8 });

			this.Children.Add(new ScrollBackground(this));
			
			this.StartFixedResolution = this.Parent.FixedResolution;
			this.StartScaleMode = this.Parent.ScaleMode;
			this.StartMouseLimitedToScreen = this.Parent.MouseLimitedToScreen;
			this.StartUseMultipleRenderers = this.Parent.UseMultipleRenderers;
			//this.StartIgnoreMouseWithoutFocus = this.Parent.IgnoreMouseWithoutFocus;

			this.Parent.FixedResolution = false;
			this.Parent.ScaleMode = ScaleMode.None;
			this.Parent.MouseLimitedToScreen = false;
			this.Parent.UseMultipleRenderers = false;
			//this.Parent.IgnoreMouseWithoutFocus = false;
		}

		public void Test(string _entrance)
		{
			File f = this.Parent.Save(this._Export(), "meka");

			if (f != null)
			{
				this.Parent._TestStarted = true;

				this.Parent.FixedResolution = this.StartFixedResolution;
				//this.Parent.IgnoreMouseWithoutFocus = this.StartIgnoreMouseWithoutFocus;
				this.Parent.MouseLimitedToScreen = this.StartMouseLimitedToScreen;
				this.Parent.ScaleMode = this.StartScaleMode;
				this.Parent.UseMultipleRenderers = this.StartUseMultipleRenderers;

				this.Parent.Entities.Remove(this);

				this.Parent.StartTest(new LevelSource(this.Parent, MekaItem.FromBytesEncrypted(this._Export())), this.Parent.SavePath, _entrance);
			}
		}

		public override void Update()
		{
			this.TabListLeft.OuterSize = new Vector(this.TabListLeft.OuterSize.X, this.Parent.Size.Y - 22);

			this.TabListRight.Position = new Vector(this.Parent.Size.X - this.TabListRight.OuterSize.X - 1, 1);
			this.TabListRight.OuterSize = new Vector(this.TabListRight.OuterSize.X, this.Parent.Size.Y - 1 - this.LevelPreview.Height);

			this.LevelPreview.X = this.Parent.Width - this.TabListRight.OuterSize.X - 1;
			this.LevelPreview.Y = this.Parent.Height - this.LevelPreview.Height - 1;
		}

		public void Exit()
		{
			this.Kill();

			this.Parent._LevelEditor = null;

			this.Parent.FixedResolution = this.StartFixedResolution;
			this.Parent.ScaleMode = this.StartScaleMode;
			this.Parent.MouseLimitedToScreen = this.StartMouseLimitedToScreen;
			this.Parent.UseMultipleRenderers = this.StartUseMultipleRenderers;
			//Parent.IgnoreMouseWithoutFocus = this.StartIgnoreMouseWithoutFocus;

			this.OnExit();
		}

		internal byte[] _Export()
		{
			Bunch<string> _areasets = new Bunch<string>();

			foreach (Layer l in this.TileEditor.Layers)
			{
				foreach (string areaset in l.GetAreasets())
				{
					if (!_areasets.Contains(areaset))
						_areasets.Add(areaset);
				}
			}

			MekaItem file = new MekaItem("File", new List<MekaItem>());

			file.Children.Add(this.LevelInfoEditor.Export());
			file.Children.Add(this.LevelPropertiesEditor.Export());

			file.Children.Add(new MekaItem("Areasets", _areasets.Select(item => new MekaItem("Areaset", item))));

			MekaItem layers = new MekaItem("Layers", new List<MekaItem>());
			foreach (Layer l in this.TileEditor.Layers)
			{
				MekaItem item = l._Export(_areasets);
				if (this.TileEditor.MainLayer == l)
					item.Children.Add(new MekaItem("Main"));
				layers.Children.Add(item);
			}
			file.Children.Add(layers);

			return file.ToBytesEncrypted();
		}
	}
}