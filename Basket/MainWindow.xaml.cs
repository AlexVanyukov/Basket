using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using Basket.Model;
using Basket.Views;

namespace Basket
{
	public partial class MainWindow : Window
	{
		public int UserId;
		public List<User> Users;
		public List<Profile> Profiles; // исп. только для ручного заполнения
		public List<Direction> Directions;

		public List<DirectionView> DirectionsView;
		public List<DirectionInBasketView> DirectionsInBasketView;

		public SQLContext sqlContext;

		public void InitLocalData()
		{
			Users = new List<User>()
			{
				new User() { ID = 1, Name="Александр", Surname="Александров", Patronymic="Александрович"},
				new User() { ID = 2, Name="Владимир", Surname="Владимиров", Patronymic="Владимирович"},
				new User() { ID = 4, Name="Елена", Surname="Еленова", Patronymic="Еленовна"},
			};

			Profiles = new List<Profile>()
			{
				new Profile() { Name="00" },
				new Profile() { Name="11" },
				new Profile() { Name="22" },
				new Profile() { Name="33" },
				new Profile() { Name="44" },
				new Profile() { Name="55" },
				new Profile() { Name="66" },
				new Profile() { Name="77" },
				new Profile() { Name="88" },
				new Profile() { Name="99" },
			};

			Directions = new List<Direction>()
			{
				new Direction() { Number=1, Name="111", Code="1.1", Profiles = new List<Profile> {Profiles[0], Profiles[1], Profiles[2]} },
				new Direction() { Number=2, Name="222", Code="2.2", Profiles = new List<Profile> {Profiles[3]} },
				new Direction() { Number=3, Name="333", Code="3.3", Profiles = new List<Profile> {Profiles[4], Profiles[5]} },
				new Direction() { Number=4, Name="444", Code="4.4", Profiles = new List<Profile> {Profiles[6], Profiles[7], Profiles[8], Profiles[9]} },
			};

			DirectionsView = new List<DirectionView>();

			for (int i=0; i< Directions.Count; i++)
			{
				DirectionView directionView = new DirectionView(Directions[i], i);
				DirectionsView.Add(directionView);
			}
			
			DirectionsInBasketView = new List<DirectionInBasketView>();
		}

		public void InitUsers()
		{
			Dictionary<int, string> users = new Dictionary<int, string>();
			
			for (int i = 0; i < Users.Count; i++)
			{
				users.Add(Users[i].ID, $"{Users[i].Surname} {Users[i].Name} {Users[i].Patronymic}");
			}

			comboBox_User.ItemsSource = users;
			//Directions = sqlContext.GetDirection();
		}

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			InitLocalData(); // Использовать без базы

			sqlContext = new SQLContext();
			//Users = sqlContext.GetUsers(); // Использовать с базой
			InitUsers();

			//Directions = sqlContext.GetDirection(); // Использовать с базой
			DirectionsView = new List<DirectionView>();

			for (int i = 0; i < Directions.Count; i++)
			{
				DirectionView directionView = new DirectionView(Directions[i], i);
				DirectionsView.Add(directionView);
			}

			DirectionsInBasketView = new List<DirectionInBasketView>();

			list_Direction.ItemsSource = DirectionsView;
		}

		private void Button_Add_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			DirectionView dataObject = btn.DataContext as DirectionView;
			int countPositions = DirectionsInBasketView.Count;

			DirectionsView[dataObject.Position].VisibilityButtonAdd = "Collapsed";
			list_Direction.Items.Refresh();

			DirectionInBasketView direct = new DirectionInBasketView(dataObject, countPositions++, countPositions);
			direct.RefreshParentPositionDirectionInProfiles();
			DirectionsInBasketView.Add(direct);

			if (countPositions > 1)
			{
				DirectionsInBasketView[countPositions - 2].RefreshButtons(countPositions);
			}

			list_DirectionInBasket.ItemsSource = DirectionsInBasketView;
			list_DirectionInBasket.Items.Refresh();
		}

		private void button_Clear_Click(object sender, RoutedEventArgs e)
		{
			Clear();
		}

		private void Clear()
		{
			DirectionsInBasketView.Clear();

			for (int i = 0; i < DirectionsView.Count; i++)
			{
				DirectionsView[i].VisibilityButtonAdd = "Visible";
			}

			list_Direction.ItemsSource = DirectionsView;
			list_Direction.Items.Refresh();
			list_DirectionInBasket.ItemsSource = DirectionsInBasketView;
			list_DirectionInBasket.Items.Refresh();
		}

		private void button_Sub_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			DirectionInBasketView dataObject = btn.DataContext as DirectionInBasketView;

			DirectionsInBasketView.RemoveAt(dataObject.Position);
			DirectionsView[dataObject.PositionParent].VisibilityButtonAdd = "Visible";
			int countPos = DirectionsInBasketView.Count;

			for (int i = dataObject.Position; i < countPos; i++)
			{
				DirectionsInBasketView[i].Position = i;
				DirectionsInBasketView[i].RefreshButtons(countPos);
				DirectionsInBasketView[i].RefreshParentPositionDirectionInProfiles();
			}

			list_Direction.ItemsSource = DirectionsView;
			list_Direction.Items.Refresh();

			list_DirectionInBasket.ItemsSource = DirectionsInBasketView;
			list_DirectionInBasket.Items.Refresh();
		}

		private void button_Save_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Невозможно сохранить данные. Используется версия без подключения к базе данных"); // Использовать без базы
			return;

			sqlContext.ClearDirectioninBasket(UserId);
			sqlContext.SaveDirections(DirectionsInBasketView, UserId);
		}

		private void Button_ProfileUp_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			ProfileInBasketView dataObject = btn.DataContext as ProfileInBasketView;
			int positionDirection = dataObject.PositionDirection;
			int positionProfile = dataObject.Position;

			List<ProfileInBasketView> profiles = DirectionsInBasketView[positionDirection].Profiles;
			int countPosition = profiles.Count;

			ProfileInBasketView temp;

			if (positionProfile > 0)
			{
				profiles[positionProfile].PositionUp(countPosition);
				profiles[positionProfile - 1].PositionDown(countPosition);
				temp = profiles[positionProfile];
				profiles[positionProfile] = profiles[positionProfile-1];
				profiles[positionProfile-1] = temp;
			}
			else
			{
				temp = profiles[positionProfile];

				for (int i = 0; i < countPosition-1; i++)
				{
					profiles[i] = profiles[i + 1];
					profiles[i].PositionUp(countPosition);
				}

				profiles[countPosition-1] = temp;
				profiles[countPosition-1].PositionUpFirst(countPosition);
			}

			DirectionsInBasketView[positionDirection].Profiles = profiles;

			list_DirectionInBasket.ItemsSource = DirectionsInBasketView;
			list_DirectionInBasket.Items.Refresh();
		}

		private void Button_ProfileUp_Click1(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			ProfileInBasketView dataObject = btn.DataContext as ProfileInBasketView;
			int posDirection = dataObject.PositionDirection;
			int posProfile = dataObject.Position;

			List<ProfileInBasketView> profiles = DirectionsInBasketView[posDirection].Profiles;
			int countPos = profiles.Count;

			profiles[posProfile].PositionUp(countPos);
			profiles[posProfile - 1].PositionDown(countPos);

			ProfileInBasketView temp = profiles[posProfile - 1];
			profiles[posProfile - 1] = profiles[posProfile];
			profiles[posProfile] = temp;

			DirectionsInBasketView[posDirection].Profiles = profiles;

			list_DirectionInBasket.ItemsSource = DirectionsInBasketView;
			list_DirectionInBasket.Items.Refresh();
		}

		private void Button_ProfileDown_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			ProfileInBasketView dataObject = btn.DataContext as ProfileInBasketView;
			int positionDirection = dataObject.PositionDirection;
			int positionProfile = dataObject.Position;

			List<ProfileInBasketView> profiles = DirectionsInBasketView[positionDirection].Profiles;
			int countPosition = profiles.Count;

			ProfileInBasketView temp = profiles[positionProfile];

			if (positionProfile < countPosition-1)
			{
				profiles[positionProfile].PositionDown(countPosition);
				profiles[positionProfile + 1].PositionUp(countPosition);
				temp = profiles[positionProfile];
				profiles[positionProfile] = profiles[positionProfile+1];
				profiles[positionProfile+1] = temp;
			}
			else
			{
				temp = profiles[positionProfile];

				for (int i = countPosition-2; i >= 0; i--)
				{
					profiles[i + 1] = profiles[i];
					profiles[i + 1].PositionDown(countPosition);
				}
				profiles[0] = temp;
				profiles[0].PositionDownLast(countPosition);
			}
			DirectionsInBasketView[positionDirection].Profiles = profiles;

			list_DirectionInBasket.ItemsSource = DirectionsInBasketView;
			list_DirectionInBasket.Items.Refresh();
		}

		private void Button_ProfileDown_Click1(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			ProfileInBasketView dataObject = btn.DataContext as ProfileInBasketView;
			int posDirection = dataObject.PositionDirection;
			int posProfile = dataObject.Position;

			List<ProfileInBasketView> profiles = DirectionsInBasketView[posDirection].Profiles;
			int countPos = profiles.Count;

			profiles[posProfile].PositionDown(countPos);
			profiles[posProfile + 1].PositionUp(countPos);

			ProfileInBasketView temp = profiles[posProfile + 1];
			profiles[posProfile + 1] = profiles[posProfile];
			profiles[posProfile] = temp;

			DirectionsInBasketView[posDirection].Profiles = profiles;

			list_DirectionInBasket.ItemsSource = DirectionsInBasketView;
			list_DirectionInBasket.Items.Refresh();
		}

		private void Button_DirectionUp_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			DirectionInBasketView dataObject = btn.DataContext as DirectionInBasketView;
			int PosDirection = dataObject.Position;
			int countPos = DirectionsInBasketView.Count;

			DirectionsInBasketView[PosDirection].PositionUp(countPos);
			DirectionsInBasketView[PosDirection-1].PositionDown(countPos);
			DirectionsInBasketView[PosDirection].RefreshParentPositionDirectionInProfiles();
			DirectionsInBasketView[PosDirection-1].RefreshParentPositionDirectionInProfiles();

			DirectionInBasketView temp = DirectionsInBasketView[PosDirection-1];
			DirectionsInBasketView[PosDirection-1] = DirectionsInBasketView[PosDirection];
			DirectionsInBasketView[PosDirection] = temp;

			list_DirectionInBasket.ItemsSource = DirectionsInBasketView;
			list_DirectionInBasket.Items.Refresh();
		}

		private void Button_DirectionDown_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			DirectionInBasketView dataObject = btn.DataContext as DirectionInBasketView;
			int currentPos = dataObject.Position;
			int countPos = DirectionsInBasketView.Count;

			DirectionsInBasketView[currentPos].PositionDown(countPos);
			DirectionsInBasketView[currentPos+1].PositionUp(countPos);
			DirectionsInBasketView[currentPos].RefreshParentPositionDirectionInProfiles();
			DirectionsInBasketView[currentPos+1].RefreshParentPositionDirectionInProfiles();

			DirectionInBasketView temp = DirectionsInBasketView[currentPos+1];
			DirectionsInBasketView[currentPos+1] = DirectionsInBasketView[currentPos];
			DirectionsInBasketView[currentPos] = temp;

			list_DirectionInBasket.ItemsSource = DirectionsInBasketView;
			list_DirectionInBasket.Items.Refresh();
		}

		private void comboBox_User_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UserId = Convert.ToInt32(comboBox_User.SelectedValue);
			Clear();

			//List<DirectionInBasket> dir = sqlContext.GetDirectionInBasket(UserId); // Использовать с базой
			List<DirectionInBasket> dir = new List<DirectionInBasket>();

			for (int i = 0; i < dir.Count; i++)
			{
				DirectionInBasketView direction = new DirectionInBasketView(dir[i], dir.Count);
				DirectionsInBasketView.Add(direction);

				for (int j=0; j < DirectionsView.Count; j++)
				{
					int id1 = DirectionsView[j].Direction.ID;
					int id2 = direction.Direction.ID;

					if (id1 == id2)
					{
						DirectionsView[j].VisibilityButtonAdd = "Collapsed";
						break;
					}
				}
			}

			list_Direction.ItemsSource = DirectionsView;
			list_Direction.Items.Refresh();
			list_DirectionInBasket.ItemsSource = DirectionsInBasketView;
			list_DirectionInBasket.Items.Refresh();
		}
	}
}
