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
    using AVFoundation;
    using Esri.ArcGISRuntime.Geometry;
    using System.Linq;
    using Foundation;
    using MoravecLabs.UI;

    using UIKit;
    using Speech;

    /// <summary>
    /// Chat view controller.
    /// </summary>
    public partial class ChatViewController : UIViewController
    {
        #region Private Variables for Speech
        private AVAudioEngine AudioEngine = new AVAudioEngine();
        private SFSpeechRecognizer SpeechRecognizer = new SFSpeechRecognizer();
        private SFSpeechAudioBufferRecognitionRequest LiveSpeechRequest = new SFSpeechAudioBufferRecognitionRequest();
        private SFSpeechRecognitionTask RecognitionTask;
        #endregion

        /// <summary>
        /// The chat bubble incoming.
        /// </summary>
        private static readonly string ChatBubbleIncomingIdentifier = "CHAT_BUBBLE_INCOMING";

        /// <summary>
        /// The chat bubble outgoing.
        /// </summary>
        private static readonly string ChatBubbleOutgoingIdentifier = "CHAT_BUBBLE_OUTGOING";

        /// <summary>
        /// The speech syntesizer.
        /// </summary>
        private AVSpeechSynthesizer SpeechSyntesizer;

        /// <summary>
        /// Chat view table delegate.
        /// </summary>
        private class ChatViewTableDelegate: UITableViewDelegate
        {
            /// <summary>
            /// Gets or sets the view model.
            /// </summary>
            /// <value>The view model.</value>
            public ViewModels.ChatViewModel ViewModel { get; set; }

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
        }

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
            var del = new ChatViewTableDelegate();
            del.ViewModel = this.ViewModel;
            this.ChatTableView.Delegate = del;
            this.SpeakButton.TouchUpInside += (sender, e) =>
            {
                if (this.SpeechSyntesizer.Speaking)
                {
                    this.SpeechSyntesizer.StopSpeaking(AVSpeechBoundary.Immediate);
                }

                //if (this.RecognitionTask == null)
                //{
                //    StartRecording();
                //}
                //else if (this.RecognitionTask.State == SFSpeechRecognitionTaskState.Running)
                //{
                //    StopRecording();
                //}
                //else 
                //{
                //    StartRecording();
                //}
            };
        }

        /// <summary>
        /// Views the did load.
        /// </summary>
        public override void ViewDidLoad()
        {
            // Configure the button
            this.SpeakButton.Type = 2;
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

            //Configure the Speech Synthesizer
            this.SpeechSyntesizer = new AVSpeechSynthesizer();
            this.ViewModel.ChatData.CollectionChanged += (sender, e) =>
            {
                // if there are new items, and they are outgoing, the user said something, so stop speaking if we are.
                if (e.NewItems.Count > 0)
                {
                    if (e.NewItems.OfType<object>().Cast<ChatMessage>().Count(i => i.Type == ChatMessageType.Outgoing) > 0)
                    {
                        if (this.SpeechSyntesizer.Speaking)
                        {
                            this.SpeechSyntesizer.StopSpeaking(AVSpeechBoundary.Word);
                        }
                    }
                }
                foreach (var item in e.NewItems)
                {
                    var chat = item as ChatMessage;
                    if (chat != null & chat.Type == ChatMessageType.Incoming)
                    {

                        var speechUtterance = new AVSpeechUtterance(chat.Text)
                        {
                            Rate = AVSpeechUtterance.MaximumSpeechRate / 1.5f,
                            Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
                            Volume = 0.5f,
                            PitchMultiplier = 1.0f
                        };
                        this.SpeechSyntesizer.SpeakUtterance(speechUtterance);
                    }
                }

            };
        }

        private void StartRecording()
        {
            // Setup audio session
            var node = AudioEngine.InputNode;
            var recordingFormat = node.GetBusOutputFormat(0);
            node.InstallTapOnBus(0, 1024, recordingFormat, (AVAudioPcmBuffer buffer, AVAudioTime when) =>
            {
                // Append buffer to recognition request
                LiveSpeechRequest.Append(buffer);
            });

            // Start recording
            AudioEngine.Prepare();
            NSError error;
            AudioEngine.StartAndReturnError(out error);

            // Did recording start?
            if (error != null)
            {
                return;
            }

            // Start recognition
            RecognitionTask = SpeechRecognizer.GetRecognitionTask(LiveSpeechRequest, async (SFSpeechRecognitionResult result, NSError err) =>
            {
                // Was there an error?
                if (err != null)
                {
                    await this.ViewModel.AddMessage("Sorry, Voice Control is currently disabled", ChatMessageType.Incoming);
                }
                else 
                {
                    // Is this the final translation?
                    if (result.Final)
                    {
                        Console.WriteLine("You said \"{0}\".", result.BestTranscription.FormattedString);
                    }
                }
            });
        }

        public void StopRecording()
        {
            AudioEngine.Stop();
            LiveSpeechRequest.EndAudio();
        }

        public void CancelRecording()
        {
            AudioEngine.Stop();
            RecognitionTask.Cancel();
        }
    }
}