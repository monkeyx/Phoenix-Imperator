using System;
using System.IO;

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.DL;

namespace PhoenixImperator
{
	public class App : Application, IDatabase
	{
		public static NavigationPage NavigationPage { get; set; }

		public static User User { get; set; }

		public static UserManager UserManager { get; set; }

		public App ()
		{
			var sqliteFilename = "Phoenix.db3";
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
			var path = Path.Combine(libraryPath, sqliteFilename);
			_dbConnection = new SQLite.SQLiteConnection (path);

			PhoenixDatabase.DatabaseProvider = this;
			PhoenixDatabase.CreateTables ();

			App.UserManager = new UserManager ();
			int userCount = App.UserManager.Count ();
			Console.WriteLine ("Users: " + userCount);
			UserForm userForm = new UserForm();
			MainPage = new NavigationPage(userForm);
			if (userCount > 0) {
				App.UserManager.First ((user) => {
					// The root page of your application
					userForm.UserCode = user.Code;
					userForm.UserId = user.Id;
				});
			}
		}

		public SQLite.SQLiteConnection GetConnection()
		{
			return _dbConnection;
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

		private SQLite.SQLiteConnection _dbConnection;
	}
}

