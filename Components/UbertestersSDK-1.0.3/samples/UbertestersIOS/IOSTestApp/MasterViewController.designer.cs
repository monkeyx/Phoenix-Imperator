// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace IOSTestApp
{
	[Register ("MasterViewController")]
	partial class MasterViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ErrorLogButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton InfoLogoButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton MakeCrashButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton WarningLogButton { get; set; }

		[Action ("ErrorLogButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void ErrorLogButton_TouchUpInside (UIButton sender);

		[Action ("InfoLogoButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void InfoLogoButton_TouchUpInside (UIButton sender);

		[Action ("MakeCrashButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void MakeCrashButton_TouchUpInside (UIButton sender);

		[Action ("WarningLogButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void WarningLogButton_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (ErrorLogButton != null) {
				ErrorLogButton.Dispose ();
				ErrorLogButton = null;
			}
			if (InfoLogoButton != null) {
				InfoLogoButton.Dispose ();
				InfoLogoButton = null;
			}
			if (MakeCrashButton != null) {
				MakeCrashButton.Dispose ();
				MakeCrashButton = null;
			}
			if (WarningLogButton != null) {
				WarningLogButton.Dispose ();
				WarningLogButton = null;
			}
		}
	}
}
