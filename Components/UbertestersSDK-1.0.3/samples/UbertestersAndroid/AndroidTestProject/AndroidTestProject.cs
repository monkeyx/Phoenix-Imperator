using System;
using Android.Runtime;
using Android.App;
using Com.Ubertesters.Sdk;
using System.Text;

namespace AndroidTestProject
{
	[Application(Icon = "@drawable/icon")]
	public class AndroidTestProject : Application
	{
		public AndroidTestProject(IntPtr handle, JniHandleOwnership transfer)
			: base(handle,transfer)
		{
		}

		public override void OnCreate ()
		{
			base.OnCreate ();
			Ubertesters.Initialize (this);

			AndroidEnvironment.UnhandledExceptionRaiser += OnException;
		}

		private static void OnException(object sender, RaiseThrowableEventArgs e)
		{
			Ubertesters.SendException(e);
			throw e.Exception;
		}

	}
}

