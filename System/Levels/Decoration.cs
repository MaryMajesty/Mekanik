using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Decoration : Entity
	{
		[Editable]public string Areaset;
		[Editable]public string Name;
		public Image Image;
		
		public override void OnInitialization()
		{
			this.Graphics.Add(this.Image = new Image(this.Parent.Areasets.First(item => item.Value.Name == this.Areaset).Value.Decorations[this.Name]) { Position = 0.000000001 });
		}
	}
}