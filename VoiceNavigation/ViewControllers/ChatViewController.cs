// <copyright file="ChatViewController.cs" company="Moravec Labs, LLC">
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Foundation;
    using MoravecLabs.UI;

    using UIKit;

    /// <summary>
    /// Chat view controller.
    /// </summary>
    public partial class ChatViewController : UITableViewController
    {
        /// <summary>
        /// The chat bubble incoming.
        /// </summary>
        private static readonly string ChatBubbleIncomingIdentifier = "CHAT_BUBBLE_INCOMING";

        /// <summary>
        /// The chat bubble outgoing.
        /// </summary>
        private static readonly string ChatBubbleOutgoingIdentifier = "CHAT_BUBBLE_OUTGOING";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VoiceNavigation.ChatViewController"/> class.
        /// </summary>
        /// <param name="handle">Handle for the controller.</param>
        public ChatViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public ViewModels.ChatViewModel ViewModel { get; set; }

        /// <summary>
        /// Views the will appear.
        /// </summary>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.ChatTableView.RowHeight = UITableView.AutomaticDimension;
            this.ChatTableView.EstimatedRowHeight = 40f;
        }

        /// <summary>
        /// Gets the height for row.
        /// </summary>
        /// <returns>The height for row.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var data = this.ViewModel.ChatData[indexPath.Row];
            return Math.Max(data.Text.Length, 26) / 25f * 40f;
        }

        /// <summary>
        /// Views the did load.
        /// </summary>
        public override void ViewDidLoad()
        {
            this.ViewModel = new ViewModels.ChatViewModel();

            // Connect Data source to TableView
            this.ChatTableView.DataSource = new BindableUITableViewDataSource<ChatMessage>(
                this.ChatTableView, 
                this.ViewModel.ChatData, 
                (tableView, indexPath, dataSource) => 
            {
                var message = dataSource[indexPath.Row];
                BubbleCell cell = null;
                if (message.Type == ChatMessageType.Incoming)
                {
                    cell = (BubbleCell)tableView.DequeueReusableCell(ChatBubbleIncomingIdentifier);
                }
                else // outgoing
                {
                    cell = (BubbleCell)tableView.DequeueReusableCell(ChatBubbleOutgoingIdentifier);
                }

                cell.Message = message;
                return cell;
            });

            // Force the keyboard to go away.
            this.MessageTextField.ShouldReturn += (txtField) => 
            {
                txtField.ResignFirstResponder();
                this.ViewModel.AddMessage(txtField.Text);
                txtField.Text = string.Empty;
                return true;
            };

            // Whenever the destination is set on the view model, it is time to go away.
            this.ViewModel.Destination.SubscribePropertyChanged((oldVal, newVal) =>
            {
                UIApplication.SharedApplication.InvokeOnMainThread(delegate
                {
                    this.NavigationController.PopViewController(true);
                });
            });
        }
    }
}