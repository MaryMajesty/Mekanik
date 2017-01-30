using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;

namespace Mekanik
{
	public class AnimationSource
	{
		public Bunch<ImageSource> Sprites = new Bunch<ImageSource>();
		public Bunch<Vector> Offsets = new Bunch<Vector>();
		public int Speed;
		public bool Repeated;

		public AnimationSource(MekaItem _item)
		{
			if (_item.Contains("Sprites"))
			{
				foreach (MekaItem frame in _item["Sprites"].Children)
				{
					this.Sprites.Add(GameBase.LoadImageSource(frame.Data));
					this.Offsets.Add(frame.Contains("Offset") ? frame["Offset"].To<Vector>() : 0);
				}
			}
			else
			{
				ImageSource src = GameBase.LoadImageSource(_item["Spriteset"].Children[0].Data);
				this.Sprites.Add(src.Split(_item["Spriteset"]["Count"].To<int>()));
				for (int i = 0; i < this.Sprites.Count; i++)
					this.Offsets.Add(0);
			}

			if (_item.Contains("Offset"))
			{
				Vector offset = _item["Offset"].To<Vector>();
				for (int i = 0; i < this.Sprites.Count; i++)
					this.Offsets[i] = offset;
			}

			this.Speed = _item["Speed"].To<int>();
			this.Repeated = _item.Contains("Repeated");
		}

		public AnimationSource(string _path) : this(MekaItem.LoadFromFile(_path)) { }
	}
}

//namespace Mekanik
//{
//	public class AnimationSource
//	{
//		public readonly int Speed;
//		//public readonly ImageSource[] Sprites;
//		public readonly ImageSource[] Frames;
//		public readonly bool Repeated;
//		public readonly Vector Scale;
//		public readonly Vector Origin;
//		public readonly Vector Offset;

//		public AnimationSource(MekaItem _file)
//		{
//			MekaItem sprites = _file["Sprites"];
//			//ImageSource img = new ImageSource(sprites.Children[0].Data);
//			this.Frames = new ImageSource[sprites.Children.Count];
//			//if (sheet.Contains("Count"))
//			//{
//			//	this.Sprites = img.Split(int.Parse(sheet["Count"].Content));
//			//	img.Dispose();
//			//}
//			//else
//			//	this.Sprites = new ImageSource[] { img };

//			this.Scale = _file.Contains("Scale") ? Vector.Parse(_file["Scale"].Content) : 1;
//			this.Speed = _file.Contains("Speed") ? int.Parse(_file["Speed"].Content) : 20;
//			//this.Reversed = _file.Contains("Reversed");
//			this.Repeated = _file.Contains("Repeated");
//			//this.Busy = _file.Contains("Busy");
//			//this.ReleaseWhenFinished = _file.Contains("ReleaseWhenFinished");

//			this.Origin = _file.Contains("Origin") ? Vector.Parse(_file["Origin"].Content) : new Vector(0.5, 1);
//			this.Offset = _file.Contains("Offset") ? Vector.Parse(_file["Offset"].Content) : 0;
//			//this.Start = _file.Contains("Start") ? int.Parse(_file["Start"].Content) : 0;

//			//this.DirectionTiedToInput = _file.Contains("DirectionTiedToInput");
//			//this.SpeedTiedToInput = _file.Contains("SpeedTiedToInput");

//			for (int i = 0; i < sprites.Children.Count; i++)
//				this.Frames[i] = new ImageSource(sprites.Children[i].Data);

//			//Func<string, int> getindex = (string text) => int.Parse(text.Substring(text.IndexOf(' ') + 1));

//			//if (!_file.Contains("Order"))
//			//	this.Frames = this.Sprites;
//			//else
//			//{
//			//	MekaItem order = _file["Order"];
//			//	this.Frames = new ImageSource[int.Parse(order["Length"].Content)];

//			//	foreach (MekaItem frame in order.Children.Where(item => item.Name != "Length"))
//			//	{
//			//		int index = getindex(frame.Name);

//			//		for (int i = index; i < this.Frames.Length; i++)
//			//			this.Frames[i] = this.Sprites[getindex(frame.Content)];
//			//	}
//			//}

//			//Colliders = new Dictionary<string, Bunch<Collider>[]>();
//			//if (_file.Contains("Colliders"))
//			//{
//			//	foreach (MekaItem group in _file["Colliders"].Children)
//			//	{
//			//		//Colliders[group.Name] = new Bunch<Collider>[this.Frames.Length];

//			//		//for (int i = 0; i < this.Frames.Length; i++)
//			//		//Colliders[group.Name][i] = new Bunch<Collider>();

//			//		foreach (MekaItem frame in group.Children)
//			//		{
//			//			int index = getindex(frame.Name);

//			//			//Bunch<Collider> cs = new Bunch<Collider>();
//			//			foreach (MekaItem collider in (frame.Children ?? new List<MekaItem>()))
//			//			{
//			//				int x = int.Parse(collider["X"].Content);
//			//				int y = int.Parse(collider["Y"].Content);
//			//				int width = int.Parse(collider["Width"].Content);
//			//				int height = int.Parse(collider["Height"].Content);
//			//				bool solid = collider.Contains("Solid");
//			//				//Collider c = new Collider(x, y, width, height, solid);
//			//				//c.Collided += (CollisionEventArgs _args) =>
//			//				//	{
//			//				//		if (_args.VsCollider.IsObstacle)
//			//				//			_args.VsEntity.Motion.X = -2000;
//			//				//	};
//			//				//cs.Add(c);
//			//				//cs.Add(new Collider(x, y, width, height, solid));
//			//			}

//			//			//for (int i = index; i < Frames.Length; i++)
//			//			//Colliders[group.Name][i] = cs;
//			//		}
//			//	}
//			//}

//			//Colors = new Color[Frames.Length];
//			//for (int i = 0; i < Frames.Length; i++)
//			//	Colors[i] = Color.White;
//			//if (_file.Contains("Effects"))
//			//{
//			//	foreach (MekaItem frame in _file["Effects"].Children)
//			//	{
//			//		int index = getindex(frame.Name);

//			//		if (frame.Contains("Color"))
//			//		{
//			//			Color c = Color.Parse(frame["Color"].Content);
//			//			for (int i = index; i < Frames.Length; i++)
//			//				Colors[i] = c;
//			//		}
//			//	}
//			//}
//		}
//	}
//}

////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;
////using Meka;
////using Meka.Data;
//////using Cml;

////namespace Mekanik
////{
////	public class AnimationSource
////	{
////		public readonly int Speed;
////		public readonly ImageSource[] Sprites;
////		public readonly ImageSource[] Frames;
////		public readonly bool Reversed;
////		public readonly bool Repeated;
////		public readonly bool Busy;
////		public readonly bool ReleaseWhenFinished;
////		public readonly Vector Scale;
////		public readonly Vector Origin;
////		public readonly Vector Offset;
////		public readonly int Start;
////		public readonly bool DirectionTiedToInput;
////		public readonly bool SpeedTiedToInput;
////		//public readonly Dictionary<string, Bunch<Collider>[]> Colliders;
////		public readonly Color[] Colors;

////		public AnimationSource(MekaItem _file)
////		{
////			MekaItem sheet = _file["Sprites"];
////			ImageSource img = new ImageSource(sheet.Children[0].Data);
////			if (sheet.Contains("Count"))
////			{
////				this.Sprites = img.Split(int.Parse(sheet["Count"].Content));
////				img.Dispose();
////			}
////			else
////				this.Sprites = new ImageSource[] { img };

////			this.Scale = _file.Contains("Scale") ? Vector.Parse(_file["Scale"].Content) : 1;
////			this.Speed = _file.Contains("Speed") ? int.Parse(_file["Speed"].Content) : 1;
////			this.Reversed = _file.Contains("Reversed");
////			this.Repeated = _file.Contains("Repeated");
////			this.Busy = _file.Contains("Busy");
////			this.ReleaseWhenFinished = _file.Contains("ReleaseWhenFinished");

////			this.Origin = _file.Contains("Origin") ? Vector.Parse(_file["Origin"].Content) : new Vector(0.5, 1);
////			this.Offset = _file.Contains("Offset") ? Vector.Parse(_file["Offset"].Content) : 0;
////			this.Start = _file.Contains("Start") ? int.Parse(_file["Start"].Content) : 0;

////			this.DirectionTiedToInput = _file.Contains("DirectionTiedToInput");
////			this.SpeedTiedToInput = _file.Contains("SpeedTiedToInput");

////			Func<string, int> getindex = (string text) => int.Parse(text.Substring(text.IndexOf(' ') + 1));

////			if (!_file.Contains("Order"))
////				this.Frames = this.Sprites;
////			else
////			{
////				MekaItem order = _file["Order"];
////				this.Frames = new ImageSource[int.Parse(order["Length"].Content)];

////				foreach (MekaItem frame in order.Children.Where(item => item.Name != "Length"))
////				{
////					int index = getindex(frame.Name);

////					for (int i = index; i < this.Frames.Length; i++)
////						this.Frames[i] = this.Sprites[getindex(frame.Content)];
////				}
////			}

////			//Colliders = new Dictionary<string, Bunch<Collider>[]>();
////			if (_file.Contains("Colliders"))
////			{
////				foreach (MekaItem group in _file["Colliders"].Children)
////				{
////					//Colliders[group.Name] = new Bunch<Collider>[this.Frames.Length];

////					//for (int i = 0; i < this.Frames.Length; i++)
////						//Colliders[group.Name][i] = new Bunch<Collider>();

////					foreach (MekaItem frame in group.Children)
////					{
////						int index = getindex(frame.Name);

////						//Bunch<Collider> cs = new Bunch<Collider>();
////						foreach (MekaItem collider in (frame.Children ?? new List<MekaItem>()))
////						{
////							int x = int.Parse(collider["X"].Content);
////							int y = int.Parse(collider["Y"].Content);
////							int width = int.Parse(collider["Width"].Content);
////							int height = int.Parse(collider["Height"].Content);
////							bool solid = collider.Contains("Solid");
////							//Collider c = new Collider(x, y, width, height, solid);
////							//c.Collided += (CollisionEventArgs _args) =>
////							//	{
////							//		if (_args.VsCollider.IsObstacle)
////							//			_args.VsEntity.Motion.X = -2000;
////							//	};
////							//cs.Add(c);
////							//cs.Add(new Collider(x, y, width, height, solid));
////						}

////						//for (int i = index; i < Frames.Length; i++)
////							//Colliders[group.Name][i] = cs;
////					}
////				}
////			}

////			Colors = new Color[Frames.Length];
////			for (int i = 0; i < Frames.Length; i++)
////				Colors[i] = Color.White;
////			if (_file.Contains("Effects"))
////			{
////				foreach (MekaItem frame in _file["Effects"].Children)
////				{
////					int index = getindex(frame.Name);

////					if (frame.Contains("Color"))
////					{
////						Color c = Color.Parse(frame["Color"].Content);
////						for (int i = index; i < Frames.Length; i++)
////							Colors[i] = c;
////					}
////				}
////			}
////		}
////	}
////}