// <copyright file="PrimaryMapViewController.cs" company="Moravec Labs, LLC">
//     MIT License
//
//     Copyright (c) Moravec Labs, LLC.
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// </copyright>

namespace VoiceNavigation
{
    using System;
    using Esri.ArcGISRuntime.Tasks.Geocoding;
    using UIKit;

    /// <summary>
    /// Primary map view controller.
    /// </summary>
    public partial class PrimaryMapViewController : UIViewController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:VoiceNavigation.PrimaryMapViewController"/> class.
        /// </summary>
        /// <param name="handle">Handle for the controller.</param>
        protected PrimaryMapViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        /// <summary>
        /// Gets the chat view model.
        /// </summary>
        /// <value>The chat view model.</value>
        public ViewModels.ChatViewModel ChatViewModel { get; private set; }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public ViewModels.PrimaryMapViewModel ViewModel { get; private set; }

        /// <summary>
        /// Views the did load.
        /// </summary>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.ChatViewModel = new ViewModels.ChatViewModel();
            this.ViewModel = new ViewModels.PrimaryMapViewModel();

            // Tie events between the two view models together.
            this.ChatViewModel.Destination.SubscribePropertyChanged((oldVal, newVal) =>
            {
                this.ViewModel.ComputeDirections(newVal);
            });

            // Perform any additional setup after loading the view, typically from a nib.
            this.MyMapView.Map = new Esri.ArcGISRuntime.Mapping.Map(
                Esri.ArcGISRuntime.Mapping.BasemapType.DarkGrayCanvasVector, 
                0, 
                0, 
                100);
        }

        /// <summary>
        /// Dids the receive memory warning.
        /// </summary>
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        /// <summary>
        /// Prepares for segue.
        /// </summary>
        /// <param name="segue">Segue that will occur.</param>
        /// <param name="sender">Source/sender of the segue.</param>
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
