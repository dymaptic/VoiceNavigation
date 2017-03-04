// <copyright file="ChatViewModel.cs" company="Moravec Labs, LLC">
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

namespace VoiceNavigation.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Esri.ArcGISRuntime.Geometry;
    using Esri.ArcGISRuntime.Tasks.Geocoding;
    using MoravecLabs.Atom;
    using MoravecLabs.Infrastructure;

    /// <summary>
    /// Chat view model.
    /// </summary>
    public class ChatViewModel : BaseViewModel
    {
        /// <summary>
        /// The chat data.
        /// </summary>
        private ObservableCollection<ChatMessage> _chatData;

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        private Atom<Context.Context> Context { get; set; }

        /// <summary>
        /// Gets or sets the chat data.
        /// </summary>
        /// <value>The chat data.</value>
        public ObservableCollection<ChatMessage> ChatData
        {
            get { return _chatData; }
            set { SetPropertyValue(ref _chatData, value); }
        }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public Atom<GeocodeResult> Destination { get; set; }

        /// <summary>
        /// Gets or sets the current location.
        /// </summary>
        /// <value>The current location.</value>
        public Atom<MapPoint> CurrentLocation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VoiceNavigation.ViewModels.ChatViewModel"/> class.
        /// </summary>
        public ChatViewModel()
        {
            // this.Context = new Atom<Context.Context>(ContextFactory.GetRepeater());
            // this.Context = new Atom<Context.Context>(ContextFactory.GetSimpleInterview());
            this.Context = new Atom<Context.Context>(ContextFactory.GetDirectionsContext(this.DestinationSet));
            this.ChatData = new ObservableCollection<ChatMessage>();
            this.Destination = new Atom<GeocodeResult>(null, this, nameof(this.Destination));
            this.CurrentLocation = new Atom<MapPoint>(default(MapPoint), this, nameof(this.CurrentLocation));
            this.CurrentLocation.SubscribePropertyChanged((oldValue, newValue) => 
            {
                var s = this.Context.Value.GetSubjectByName("CurrentLocation");
                s.SetValue(newValue); 
            });
        }

        /// <summary>
        /// Destination was set on the context
        /// </summary>
        /// <returns>The set.</returns>
        /// <param name="context">Context.</param>
        public List<string> DestinationSet(Context.Context context)
        {
            this.Destination.Value = context.GetSubjectByName("FinalDestination").GetValue() as GeocodeResult;
            return new List<string>();
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        public async Task InitializeAsync()
        {
            var result = await this.Context.Value.Evalulate("");
            foreach (var m in result)
            {
                await this.AddMessage(m, ChatMessageType.Incoming);
            }
        }

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="messageType">Message type.</param>
        public async Task AddMessage(string message, ChatMessageType messageType = ChatMessageType.Outgoing)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            ChatData.Add(new ChatMessage { Text = message, Type = messageType });
            //If the user created the message, figure out what they mean...
            if (messageType == ChatMessageType.Outgoing)
            {
                var result = await this.Context.Value.Evalulate(message);
                foreach (var m in result)
                {
                    await this.AddMessage(m, ChatMessageType.Incoming);
                }
            }


        }
    }
}
