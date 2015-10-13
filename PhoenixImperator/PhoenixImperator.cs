//
// PhoenixImperator.cs
//
// Author:
//       Seyed Razavi <monkeyx@gmail.com>
//
// Copyright (c) 2015 Seyed Razavi 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IO;

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.DL;
using Phoenix.Util;

using PhoenixImperator.Pages;

namespace PhoenixImperator
{
	public class App : Application, IDatabase, ILogger
	{
		public static NavigationPage NavigationPage { get; set; }

		public App ()
		{
			var sqliteFilename = "Phoenix.db3";
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
			var path = Path.Combine(libraryPath, sqliteFilename);
			_dbConnection = new SQLite.SQLiteConnection (path);

			Phoenix.Application.Initialize (this, this);

			int userCount = Phoenix.Application.UserManager.Count ();
			Log.WriteLine (Log.Layer.AL, typeof(App), "Users: " + userCount);
			UserLoginPage userLoginPage = new UserLoginPage();
			App.NavigationPage = new NavigationPage(userLoginPage);
			MainPage = App.NavigationPage;
			if (userCount > 0) {
				Phoenix.Application.UserManager.First ((user) => {
					// The root page of your application
					userLoginPage.UserCode = user.Code;
					userLoginPage.UserId = user.Id;
					Phoenix.Application.UserLoggedIn(user);
				});
			}
		}

		public SQLite.SQLiteConnection GetConnection()
		{
			return _dbConnection;
		}

		public void WriteLine(string format, params object[] arg)
		{
			Console.WriteLine (format, arg);
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

