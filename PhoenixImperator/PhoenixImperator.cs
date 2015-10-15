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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using ModernHttpClient;

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

using PhoenixImperator.Pages;

namespace PhoenixImperator
{
	public class App : Application, Phoenix.IDatabase, Phoenix.ILogger, Phoenix.IDocumentFolder, Phoenix.IRestClient
	{
		public static NavigationPage NavigationPage { get; set; }

		public App ()
		{
			var sqliteFilename = "Phoenix.db3";
			string documentsPath = GetDocumentPath ();
			#if __ANDROID__
			var path = Path.Combine(documentsPath, sqliteFilename);
			#else
			string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
			var path = Path.Combine(libraryPath, sqliteFilename);
			#endif
			_dbConnection = new SQLite.SQLiteConnection (path);

			Phoenix.Application.Initialize (this, this, this, this);

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

		/// <summary>
		/// Gets the document path.
		/// </summary>
		/// <returns>The document path.</returns>
		public string GetDocumentPath()
		{
			#if __ANDROID__
			string documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal); // Documents folder
			#else
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			#endif
			return documentsPath;
		}

		/// <summary>
		/// Writes the file.
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="content">Content.</param>
		/// <param name="fileame">Fileame.</param>
		public void WriteFile(string filename, string content)
		{
			string filePath = Path.Combine (GetDocumentPath (), filename);
			File.WriteAllText (filePath, content);
		}

		/// <summary>
		/// Reads the file.
		/// </summary>
		/// <returns>The file.</returns>
		/// <param name="filename">Filename.</param>
		public string ReadFile(string filename)
		{
			string filePath = Path.Combine (GetDocumentPath (), filename);
			return File.ReadAllText (filePath);
		}

		/// <summary>
		/// Gets the connection.
		/// </summary>
		/// <returns>The connection.</returns>
		public SQLite.SQLiteConnection GetConnection()
		{
			return _dbConnection;
		}

		/// <summary>
		/// Writes the line.
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="arg">Argument.</param>
		public void WriteLine(string format, params object[] arg)
		{
			Console.WriteLine (format, arg);
		}

		/// <summary>
		/// Async GET method
		/// </summary>
		/// <returns>Stream</returns>
		/// <param name="url">URL.</param>
		public Task<Stream> GetAsync(string url)
		{
			var httpClient = new HttpClient(new NativeMessageHandler());
			httpClient.Timeout = TimeSpan.FromSeconds(30);
			return httpClient.GetStreamAsync (new Uri (url));
		}

		/// <summary>
		/// Async POST method
		/// </summary>
		/// <returns>Stream</returns>
		/// <param name="url">URL.</param>
		/// <param name="dto">Dto.</param>
		public Task<HttpResponseMessage> PostAsync(string url, object dto)
		{
			var httpClient = new HttpClient(new NativeMessageHandler());
			httpClient.Timeout = TimeSpan.FromSeconds(30);
			return httpClient.PostAsync(new Uri(url),new StringContent(dto.ToString()));
		}

		/// <summary>
		/// Application developers override this method to perform actions when the application starts.
		/// </summary>
		/// <remarks>To be added.</remarks>
		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		/// <summary>
		/// Application developers override this method to perform actions when the application enters the sleeping state.
		/// </summary>
		/// <remarks>To be added.</remarks>
		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		/// <summary>
		/// Application developers override this method to perform actions when the application resumes from a sleeping state.
		/// </summary>
		/// <remarks>To be added.</remarks>
		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

		private SQLite.SQLiteConnection _dbConnection;
	}
}

