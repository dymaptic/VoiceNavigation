using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using MoravecLabs.UI;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace VoiceNavigation
{
	public partial class ChatViewController : UITableViewController
	{
		public ViewModels.ChatViewModel ViewModel {get; set;}

		private static readonly string CHAT_BUBBLE_INCOMING = "CHAT_BUBBLE_INCOMING";
		private static readonly string CHAT_BUBBLE_OUTGOING = "CHAT_BUBBLE_OUTGOING";


        public ChatViewController (IntPtr handle) : base (handle)
        {
			ViewModel = new ViewModels.ChatViewModel();
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			ChatTableView.RowHeight = UITableView.AutomaticDimension;
			ChatTableView.EstimatedRowHeight = 40f;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var data = this.ViewModel.ChatData[indexPath.Row];
			return Math.Max(data.Text.Length, 26) / 25f * 40f;

		}

		public override void ViewDidLoad()
		{

			//Connect Data source to TableView
			ChatTableView.DataSource = new BindableUITableViewDataSource<ChatMessage>(this.ChatTableView, 
			                                                                          this.ViewModel.ChatData, 
			                                                                          (tableView, indexPath, dataSource) => 
			{
				var message = dataSource[indexPath.Row];
				BubbleCell cell = null;
				if (message.Type == ChatMessageType.Incoming)
				{
					cell = (BubbleCell)tableView.DequeueReusableCell(CHAT_BUBBLE_INCOMING);
				}
				else //outgoing
				{
					cell = (BubbleCell)tableView.DequeueReusableCell(CHAT_BUBBLE_OUTGOING);
				}
				cell.Message = message;
				return cell;
			});

			//Force the keyboard to go away.
			MessageTextField.ShouldReturn += (txtField) => {
				txtField.ResignFirstResponder();
				this.ViewModel.AddMessage(txtField.Text);
				txtField.Text = "";
				return true;
			};

			//Whenever the destination is set on the view model, it is time to go away.
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