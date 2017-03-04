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

namespace VoiceNavigation
{
    [Register ("ChatViewController")]
    partial class ChatViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView ChatTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField MessageTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MaterialControls.MDButton SpeakButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ChatTableView != null) {
                ChatTableView.Dispose ();
                ChatTableView = null;
            }

            if (MessageTextField != null) {
                MessageTextField.Dispose ();
                MessageTextField = null;
            }

            if (SpeakButton != null) {
                SpeakButton.Dispose ();
                SpeakButton = null;
            }
        }
    }
}