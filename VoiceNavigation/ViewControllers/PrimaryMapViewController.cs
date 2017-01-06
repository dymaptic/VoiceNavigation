using System;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using UIKit;

namespace VoiceNavigation
{
	public partial class PrimaryMapViewController : UIViewController
	{
		public ViewModels.ChatViewModel ChatViewModel { get; private set; }
		public ViewModels.PrimaryMapViewModel ViewModel { get; private set; }

		protected PrimaryMapViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			ChatViewModel = new ViewModels.ChatViewModel();
			ViewModel = new ViewModels.PrimaryMapViewModel();

			//Tie events between the two view models together.
			ChatViewModel.Destination.SubscribePropertyChanged((oldVal, newVal) =>
			{
				this.ViewModel.ComputeDirections(newVal);
			});

			// Perform any additional setup after loading the view, typically from a nib.
			MyMapView.Map = new Esri.ArcGISRuntime.Mapping.Map(Esri.ArcGISRuntime.Mapping.BasemapType.DarkGrayCanvasVector, 0, 0, 100);
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, Foundation.NSObject sender)
		{
			if (segue.Identifier == "ShowChatViewSegue")
			{
				var cvc = segue.DestinationViewController as ChatViewController;
				if (cvc != null)
				{
					cvc.ViewModel = this.ChatViewModel;
				}
			}
		}


	}
}
