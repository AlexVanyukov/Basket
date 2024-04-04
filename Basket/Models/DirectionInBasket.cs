using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Model
{
	public class DirectionInBasket
	{
		public int ID { get; set; }
		public int UserID { get; set; }
		public int DirectionID { get; set; }
		public int Number { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
		public List<ProfileInBasket> Profiles { get; set; }
		public int Position { get; set; }

		public Direction GetDirection()
		{
			Direction direction = new Direction();
			direction.ID = DirectionID;
			direction.Name = Name;
			direction.Code = Code;
			direction.Number = Number;
			direction.Profiles = new List<Profile>();
			for (int i=0; i< Profiles.Count; i++)
			{
				direction.Profiles.Add(Profiles[i].GetProfile());
			}
			return direction;
		}

	}
}
