using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basket.Model;

namespace Basket.Views
{
	public class DirectionView
	{
		public Direction Direction { get; set; }
		public int Position { get; set; }
		public string VisibilityButtonAdd { get; set; }

		public DirectionView(Direction direction, int position)
		{
			Direction = direction;
			VisibilityButtonAdd = "Visible";
			Position = position;
		}
	}
}
