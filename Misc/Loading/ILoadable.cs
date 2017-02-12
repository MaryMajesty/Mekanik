using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public interface ILoadable
	{
		bool FinishedLoading
		{
			get;
		}
		void LoadStep();
	}
}