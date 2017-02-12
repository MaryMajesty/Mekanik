using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class LoadingScreen : Entity
	{
		public bool Skippable = true;

		public override void Update()
		{
			this.Focus();
		}

		public override void OnKeyPress(Key _key)
		{
			if (this.Skippable)
				this.Kill();
		}
	}
}