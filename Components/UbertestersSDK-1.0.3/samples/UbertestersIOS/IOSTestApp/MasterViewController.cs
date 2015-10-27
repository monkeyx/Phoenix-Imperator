using System;
using System.Drawing;
using System.Collections.Generic;

using Foundation;
using UIKit;

using UbertestersSDK;

namespace IOSTestApp
{
	public partial class MasterViewController : UIViewController
	{

		public MasterViewController (IntPtr handle) : base (handle)
		{
			Title = NSBundle.MainBundle.LocalizedString ("Master", "Master");

			// Custom initialization
		}



		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.


		}
			


		partial void InfoLogoButton_TouchUpInside (UIButton sender)
		{

			Ubertesters.Shared.UTLog("Info log", UTLogLevel.Info);
		}

		partial void WarningLogButton_TouchUpInside (UIButton sender)
		{
			Ubertesters.Shared.UTLog("Warning log", UTLogLevel.Warning);

		}

		partial void ErrorLogButton_TouchUpInside (UIButton sender)
		{
			Ubertesters.Shared.UTLog("Error log", UTLogLevel.Error);

		}

		partial void MakeCrashButton_TouchUpInside (UIButton sender)
		{
			int a = 1-1;
			int b = 1/a;
		}
	}
}

