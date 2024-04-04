using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Model
{
	public class ProfileInBasket
	{
		public int ID { get; set; }
		public int DirectionID { get; set; }
		public string Name { get; set; }
		public int ProfileID { get; set; }
		public int UserID { get; set; }
		public int Position { get; set; }

		public Profile GetProfile()
		{
			Profile profile = new Profile();
			profile.ID = ID;
			profile.Name = Name;
			profile.DirectionID = DirectionID;
			return profile;
		}
	}
}
