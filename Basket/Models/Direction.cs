using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Model
{
	public class Direction
	{
		public int ID { get; set; }
		public int Number { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
		public List<Profile> Profiles { get; set; }
	}

}
