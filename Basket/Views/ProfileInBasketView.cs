using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basket.Model;

namespace Basket.Views
{
	public class ProfileInBasketView
	{
		public Profile Profile { get; set; }
		public int Position { get; set; }
		public int PositionDirection { get; set; }
		public string VisibilityButtonUp { get; set; }
		public string VisibilityButtonDown { get; set; }

		public ProfileInBasketView(Profile profile, int countPosition)
		{
			this.Profile = profile;
			this.Position = countPosition;
			RefreshButtons(countPosition+1);
		}

		public ProfileInBasketView(ProfileInBasket profile, int countPosition)
		{
			this.Profile = profile.GetProfile();
			this.Position = countPosition;
			RefreshButtons(countPosition + 1);
		}

		public void RefreshButtons(int allPosition)
		{
			if ((this.Position == 0) && (allPosition == 1))
			{
				this.VisibilityButtonUp = "Collapsed";
				this.VisibilityButtonDown = "Collapsed";
			}
			else
			{
				this.VisibilityButtonUp = "Visible";
				this.VisibilityButtonDown = "Visible";
			}
		}

		public void RefreshButtons_OLD(int allPosition)
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
			else if (this.Position == allPosition - 1)
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

		public void PositionUpFirst(int countPosition)
		{
			Position = countPosition-1;
			RefreshButtons(countPosition);
		}

		public void PositionDownLast(int countPosition)
		{
			Position = 0;
			RefreshButtons(countPosition);
		}

		public void PositionUp(int countPosition)
		{
			Position--;
			RefreshButtons(countPosition);
		}

		public void PositionDown(int countPosition)
		{
			Position++;
			RefreshButtons(countPosition);
		}


	}
}
