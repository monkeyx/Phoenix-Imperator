##Ubertesters SDK

##### 
Ensure your app quality with the Ubertesters SDK.

###Features

###### 
+ **Bug submission** - submit a bug with your comments, description and real-time screenshots directly from within your app. With the Ubertesters SDK bug reporting is easy.


+ **Build distribution** - distribute builds to your team members over-the-air, manage build permissions, enable and disable a specific build for testing. The Ubertesters SDK automatically keeps your testers up-to-date with the latest build of your app.

+ **Crash reporting**- get automatic and instant crash logs and reports that include all essential data such as stack traces, type of crash, device and OS version. 


+ **Test cases support** - run test cases from your mobile application with Ubertesters SDK.

+ **Session Tracking** - track all testing sessions in real-time sorted by devices or testers. Use your Ubertesters dashboard to review your device status in real-time.

+ **Integration with external Bug Tracking Systems** - integrate and publish your bugs, features or feedback into Jira, YouTrack, Mantis or RedMine.


###For iOS:

##### 

```
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;

using UbertestersCrashHandler;
using UbertestersSDK;


namespace UbertestersXamarinAppTest
{
//...
 public override bool FinishedLaunching (UIApplication app, NSDictionary options)
  {
	//...
	Ubertesters.Shared.Initialize ();
	AppDomain.CurrentDomain.UnhandledException += 
	(object sender, UnhandledExceptionEventArgs e) => {
			
			CrashHandler.PostCrash (e);
	   };
			return true;
	}
 //...
	}
 //....
}
```

###For Android:

##### 
```    
//...

using Com.Ubertesters.Sdk;

namespace TestAndroid
{
	[Application(Icon = "@drawable/icon")]
	public class TestAndroidApp : Application
	{
		public TestAndroidApp(IntPtr handle, JniHandleOwnership transfer)
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
```
     
     
