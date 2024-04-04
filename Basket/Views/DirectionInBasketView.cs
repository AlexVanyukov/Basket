using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basket.Model;

namespace Basket.Views
{
	public class DirectionInBasketView
	{
		public Direction Direction { get; set; }
		public List<ProfileInBasketView> Profiles { get; set; }
		public int Position { get; set; }
		public int PositionParent { get; set; }
		public string VisibilityButtonUp { get; set; }
		public string VisibilityButtonDown { get; set; }

		public DirectionInBasketView(DirectionView direction, int position, int allCountPosition)
		{
			Direction = direction.Direction;
			Profiles = new List<ProfileInBasketView>();
			int countElements = Direction.Profiles.Count;
			for (int i = 0; i < countElements; i++)
			{
				Profiles.Add(new ProfileInBasketView(Direction.Profiles[i], i));
				Profiles[i].RefreshButtons(countElements);
			}
			Position = position;
			PositionParent = direction.Position;
			RefreshButtons(allCountPosition);
		}

		public DirectionInBasketView(DirectionInBasket direction, int allCountPosition)
		{
			Direction = direction.GetDirection();
			Profiles = new List<ProfileInBasketView>();
			int countElements = Direction.Profiles.Count;
			for (int i = 0; i < countElements; i++)
			{
				Profiles.Add(new ProfileInBasketView(Direction.Profiles[i], i));
				Profiles[i].RefreshButtons(countElements);
			}
			Position = direction.Position;
			PositionParent = direction.Position;
			RefreshButtons(allCountPosition);
		}

		public void RefreshParentPositionDirectionInProfiles()
		{
			for (int i=0; i< Profiles.Count; i++)
			{
				Profiles[i].PositionDirection = Position;
			}
		}

		public void RefreshButtons(int allPosition)
		{
			if ((this.Position == 0) && (allPosition == 1))
			{
				this.VisibilityButtonUp = "Collapsed";
				this.VisibilityButtonDown = "Collapsed";
			}
			else if (this.Position == 0)
			{
				this.VisibilityButtonUp = "Collapsed";
				this.VisibilityButtonDown = "Visible";
			}
			else if (this.Position == allPosition-1)
			{
				this.VisibilityButtonUp = "Visible";
				this.VisibilityButtonDown = "Collapsed";
			}
			else
			{
				this.VisibilityButtonUp = "Visible";
				this.VisibilityButtonDown = "Visible";
			}
		}

		public void PositionUp(int allPosition)
		{
			Position--;
			RefreshButtons(allPosition);
		}

		public void PositionDown(int allPosition)
		{
			Position++;
			RefreshButtons(allPosition);
		}


	}
}
