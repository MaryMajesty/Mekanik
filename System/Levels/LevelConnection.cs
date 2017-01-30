using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class LevelConnection : Entity
	{
		[Editable]public string DestinationLevel;
		[Editable]public string DestinationEntrance;
	}
}