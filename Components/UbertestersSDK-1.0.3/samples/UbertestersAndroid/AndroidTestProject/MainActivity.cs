using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.Ubertesters.Sdk;

namespace AndroidTestProject
{
	[Activity (Label = "AndroidTestProject", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;

		protected override void OnCreate (Bundle bundle)
		{
//			base.OnCreate (bundle);
//
//			// Set our view from the "main" layout resource
//			SetContentView (Resource.Layout.Main);
//
//			// Get our button from the layout resource,
//			// and attach an event to it
//			Button button = FindViewById<Button> (Resource.Id.myButton);
//			
//			button.Click += delegate {
//				button.Text = string.Format ("{0} clicks!", count++);
//			};

			base.OnCreate (bundle);

			// Set our view from the "main" layout resource

			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.logInfoButton);

			button.Click += delegate {

				//XUbertesters.LogInfo ("Info log");
				Ubertesters.Logger().Info("Info log");

			};


			button = FindViewById<Button> (Resource.Id.logWarningButton);

			button.Click += delegate {

				//XUbertesters.LogWarn ("Warn log");
				Ubertesters.Logger().Warn("Warn log");
			};

			button = FindViewById<Button> (Resource.Id.logErrorButton);

			button.Click += delegate {

				//XUbertesters.LogInfo ("Error log");

				Ubertesters.Logger().Error("Error log");

			};

			button = FindViewById<Button> (Resource.Id.makeCrashButton);

			button.Click += delegate {

				int a = 1;

				int b = 0;

				int c = a/b;

			};

			button = FindViewById<Button> (Resource.Id.makeScreenshotButton);

			button.Click += delegate {

				Ubertesters.TakeScreenshot();
				//XUbertesters.MakeScreenshot();

			};
		}
	}
}


