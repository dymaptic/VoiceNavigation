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
    [Register ("PrimaryMapViewController")]
    partial class PrimaryMapViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Esri.ArcGISRuntime.UI.Controls.MapView MyMapView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (MyMapView != null) {
                MyMapView.Dispose ();
                MyMapView = null;
            }
        }
    }
}