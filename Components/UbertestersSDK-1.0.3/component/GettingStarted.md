You have installed Ubertesters component for Xamarin. It will allow you to use Ubertesters platform with both Android and iOS Xamarin projects. For illustrated manual visit http://ubertesters.com/download-sdk/ and select Xamarin in the left menu.

Add Ubertesters project ID to your project:

1. Open [http://beta.ubertesters.com/projects](http://beta.ubertesters.com/projects) and select your project, go to SDK Integration. 
2. Copy your unique "Ubertesters Project ID"
##### 

### Android integration

##### 
1.Edit "AndroidManifest.xml" file of your project:
``` 
<application>
//...
<meta-data android:name="ubertesters_project_id" android:value="your project id"/>
//...
</application>       
```

2.Add Permissions into `<manifest>` tag:

```
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android"
  package="your package"
  android:versionCode="1"
  android:versionName="1.0">

   // ...
  <!-- Ubertesters library user-permisions -->
  <uses-permission android:name="android.permission.GET_TASKS" />
  <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
  <uses-permission android:name="android.permission.SYSTEM_ALERT_WINDOW" />
  <!-- Ubertesters library user-permisions -->
   //...

</manifest>  
```

3.Open "AppDelegate.cs" file. Add using "Com.Ubertesters.Sdk" namespace:

`using Com.Ubertesters.Sdk;`

4.Open your application class and override `OnCreate`  function. Initialize UbertestersSDK and add crash handler in "AppDelegate.cs":

```
public class DemoApp : Application
{
public DemoApp(IntPtr handle, JniHandleOwnership transfer)
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

```
##### 
**NOTE:**
 If you don't have application class, add new class to your project and inherit Application class. 
You have to add attribute `[Application()] `and remove application initialization from file 
Properties/ApplicationInfo.cs:  

```
[assembly: Application (Label = "Demo app", Icon = "@drawable/icon")]
```
##### 

### iOS integration

##### 

1.Edit info.plst of your project and paste project ID with "ubertesters_project_id" key

2.Open "AppDelegate.cs" file. Add "UbertestersSDK" and “UbertestersCrashHandler” namespaces:
```
using UbertestersSDK;
using UbertestersCrashHandler;

```
3.Override UIApplication Delegate's method "FinishedLaunching" (if it isn't already exist).

4.Copy following code to the FinishedLaunching body next lines:

```
     Ubertesters.Shared.Initialize ();
     AppDomain.CurrentDomain.UnhandledException += 
	(object sender, UnhandledExceptionEventArgs e) => {
			
			CrashHandler.PostCrash (e);
	   };
			return true;
     }
```