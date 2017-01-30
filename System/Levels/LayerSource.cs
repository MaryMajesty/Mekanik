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
	}
}