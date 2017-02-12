using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;
using Meka.ByteOperators;

namespace Mekanik
{
	public class LayerSource
	{
		public bool Main;
		public Tuple<string, int>[,] Tiles;
		public Bunch<EntityInstance> Entities = new Bunch<EntityInstance>();

		public LayerSource(MekaItem _item, Bunch<string> _areasets)
		{
			this.Main = _item.Contains("Main");

			ImageSource img = GameBase.LoadImageSource(_item["Tiles"].Data);
			Color[,] px = img.Pixels;
			this.Tiles = new Tuple<string, int>[img.Width, img.Height];
			for (int x = 0; x < img.Width; x++)
			{
				for (int y = 0; y < img.Height; y++)
					this.Tiles[x, y] = new Tuple<string, int>(_areasets[(int)Beth.FromEndian(px[x, y].Bytes.Sub(0, 2))], (int)Beth.FromEndian(px[x, y].Bytes.Sub(2, 2)));
			}

			foreach (MekaItem entity in _item["Entities"].Children)
				this.Entities.Add(new EntityInstance(entity));
		}

		internal Bunch<string> _GetAreasets()
		{
			Bunch<string> @out = new Bunch<string>();
			for (int x = 0; x < this.Tiles.GetLength(0); x++)
			{
				for (int y = 0; y < this.Tiles.GetLength(1); y++)
				{
					if (!@out.Contains(this.Tiles[x, y].Item1))
						@out.Add(this.Tiles[x, y].Item1);
				}
			}
			return @out;
		}

		internal MekaItem _Export(Bunch<string> _areasets)
		{
			MekaItem @out = new MekaItem("Layer", new List<MekaItem>());

			ImageSource src = new ImageSource(this.Tiles.GetLength(0), this.Tiles.GetLength(1));
			for (int x = 0; x < this.Tiles.GetLength(0); x++)
			{
				for (int y = 0; y < this.Tiles.GetLength(1); y++)
				{
					Bunch<byte> bs = Beth.ToEndian(_areasets.IndexOf(this.Tiles[x, y].Item1), 2);
					bs.Add(Beth.ToEndian(this.Tiles[x, y].Item2, 2));
					src[x, y] = new Color(bs);
				}
			}
			@out.Children.Add(new MekaItem("Tiles", src.Bytes));

			@out.Children.Add(new MekaItem("Entities", this.Entities.Select(item => item._Export()).ToList()));

			return @out;
		}
	}
}