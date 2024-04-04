using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Basket.Model;
using Basket.Views;

namespace Basket
{
	public class SQLContext
	{
		private SqlConnection connection;

		public SQLContext()
		{
			string datasource = @"AVV_HomePC\SQLEXPRESS";
			string database = "THDB";
			string connString = $"Data Source=\"{datasource}\";Initial Catalog=\"{database}\";Integrated Security=SSPI;";
			connection = new SqlConnection(connString);
		}

		private DataTable Execute(string command)
		{
			DataTable Table = new DataTable();
			try
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand
				{
					CommandText = command,
					CommandType = CommandType.Text,
					Connection = connection
				};
				Table.Load(cmd.ExecuteReader());
				cmd.Dispose();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message.ToString());
			}
			finally
			{
				connection.Close();
			}
			return Table;
		}

		public List<User> GetUsers()
		{
			string sqlString = "SELECT * FROM Users";
			List<User> users = new List<User>();
			using (DataTable dt = Execute(sqlString))
			{
				for (int i=0; i<dt.Rows.Count; i++)
				{
					User user = new User();
					user.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
					user.Name = dt.Rows[i]["Name"].ToString();
					user.Surname = dt.Rows[i]["Surname"].ToString();
					user.Patronymic = dt.Rows[i]["Patronymic"].ToString();
					users.Add(user);
				}
				dt.Dispose();
			}
			return users;
		}

		
		public List<DirectionInBasket> GetDirectionInBasket(int userId)
		{
			string sqlString = $"SELECT dr.*, d.Id ID ,d.Name, d.Number, d.Code FROM DirectionsInBasket dr LEFT JOIN Directions AS d ON dr.ID_Direction=d.id WHERE ID_User={userId}";
			List<DirectionInBasket> directions = new List<DirectionInBasket>();
			List<ProfileInBasket> profiles;

			using (DataTable dt = Execute(sqlString))
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					DirectionInBasket direction = new DirectionInBasket();

					direction.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
					direction.UserID = Convert.ToInt32(dt.Rows[i]["ID_User"]);
					direction.DirectionID = Convert.ToInt32(dt.Rows[i]["ID_Direction"]);
					direction.Position = Convert.ToInt32(dt.Rows[i]["Position"]);
					direction.Name = dt.Rows[i]["Name"].ToString();
					direction.Number = Convert.ToInt32(dt.Rows[i]["Number"]);
					direction.Code = dt.Rows[i]["Code"].ToString();

					directions.Add(direction);
				}
				dt.Dispose();
			}
			
			for (int i = 0; i < directions.Count; i++)
			{
				sqlString = 
					"SELECT pr.*, p.Name, p.ID_Direction " +
					"FROM ProfilesInBasket pr " +
						"LEFT JOIN Profiles AS p ON pr.ID_Profile=p.id " +
					$"WHERE ID_Direction={directions[i].DirectionID} and ID_User={userId} " +
					"ORDER BY Position ASC";
				profiles = new List<ProfileInBasket>();
				using (DataTable dt = Execute(sqlString))
				{
					for (int j = 0; j < dt.Rows.Count; j++)
					{
						ProfileInBasket profile = new ProfileInBasket();
						profile.ID = Convert.ToInt32(dt.Rows[j]["ID_Profile"]);
						profile.UserID = Convert.ToInt32(dt.Rows[j]["ID_User"]);
						profile.ProfileID = Convert.ToInt32(dt.Rows[j]["ID"]);
						profile.Position = Convert.ToInt32(dt.Rows[j]["Position"]);
						profile.Name = dt.Rows[j]["Name"].ToString();
						profile.Position = j;
						profiles.Add(profile);
					}
					dt.Dispose();
				}
				directions[i].Profiles = profiles;
			}
			return directions;
		}
		
		public List<Direction> GetDirection()
		{
			string sqlString = "SELECT * FROM Directions";
			List<Direction> directions = new List<Direction>();
			List<Profile> profiles;
			List<int> directionsID = new List<int>();

			using (DataTable dt = Execute(sqlString))
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					Direction direction = new Direction();
					direction.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
					direction.Name = dt.Rows[i]["Name"].ToString();
					direction.Number = Convert.ToInt32(dt.Rows[i]["Number"]);
					direction.Code = dt.Rows[i]["Code"].ToString();
					directions.Add(direction);

					directionsID.Add(direction.ID);
				}
				dt.Dispose();
			}
			
			for (int i = 0; i < directions.Count; i++)
			{
				sqlString = $"SELECT pr.* FROM Profiles pr WHERE pr.ID_Direction = ({directions[i].ID})";
				profiles = new List<Profile>();
				using (DataTable dt = Execute(sqlString))
				{
					for (int j = 0; j < dt.Rows.Count; j++)
					{
						Profile profile = new Profile();
						profile.ID = Convert.ToInt32(dt.Rows[j]["ID"]);
						profile.DirectionID = Convert.ToInt32(dt.Rows[j]["ID_DIrection"]);
						profile.Name = dt.Rows[j]["Name"].ToString();
						profiles.Add(profile);
					}
					dt.Dispose();
				}
				directions[i].Profiles = profiles;
			}

			return directions;
		}

		public void ClearDirectioninBasket(int userID)
		{
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			connection.Open();
			command.CommandText = $"DELETE FROM DirectionsInBasket WHERE ID_User = {userID}";
			command.ExecuteNonQuery();

			command.CommandText = $"DELETE FROM ProfilesInBasket WHERE ID_User = {userID}";
			command.ExecuteNonQuery();
			connection.Close();
		}

		public void SaveDirections(List<DirectionInBasketView> directions, int userID)
		{
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			connection.Open();
			for (int i=0; i< directions.Count; i++)
			{
				command.CommandText = @"INSERT INTO DirectionsInBasket(ID_User, ID_Direction, Position) VALUES (@ID_User, @ID_Direction, @Position)";
				command.Parameters.Clear();

				command.Parameters.AddWithValue("@ID_Direction", directions[i].Direction.ID);
				command.Parameters.AddWithValue("@ID_User", userID);
				command.Parameters.AddWithValue("@Position", directions[i].Position);

				command.ExecuteNonQuery();

				for (int j = 0; j < directions[i].Profiles.Count; j++)
				{
					ProfileInBasketView profile = directions[i].Profiles[j];
					command.CommandText = @"INSERT INTO ProfilesInBasket(ID_User, ID_Profile, Position) VALUES (@ID_User, @ID_Profile, @Position)";
					command.Parameters.Clear();

					command.Parameters.AddWithValue("@ID_Profile", profile.Profile.ID);
					command.Parameters.AddWithValue("@ID_User", userID);
					command.Parameters.AddWithValue("@Position", profile.Position);

					command.ExecuteNonQuery();
				}
			}
			connection.Close();
		}
			
	}
}
