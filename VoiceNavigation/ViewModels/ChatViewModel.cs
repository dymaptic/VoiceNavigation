using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AVFoundation;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Microsoft.Cognitive.LUIS;
using MoravecLabs.Atom;
using MoravecLabs.Infrastructure;
namespace VoiceNavigation.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        public ObservableCollection<ChatMessage> ChatData
        {
            get { return _chatData; }
            set { SetPropertyValue(ref _chatData, value); }
        }
        private ObservableCollection<ChatMessage> _chatData;

        public Atom<GeocodeResult> Destination { get; set; }

        public ChatViewModel()
        {
            ChatData = new ObservableCollection<ChatMessage>();
            Destination = new Atom<GeocodeResult>(null, this, nameof(Destination));
        }

        public async void AddMessage(string message, ChatMessageType messageType = ChatMessageType.Outgoing)
        {
            ChatData.Add(new ChatMessage { Text = message, Type = messageType });
            //If the user created the message, figure out what they mean...
            if (messageType == ChatMessageType.Outgoing)
                await Predict(_chatData.Last().Text);


            //if (messageType == ChatMessageType.Incoming)
            //{
            //    var speechSynthesizer = new AVSpeechSynthesizer();

            //    var speechUtterance = new AVSpeechUtterance(message)
            //    {
            //        Rate = AVSpeechUtterance.MaximumSpeechRate / 2,
            //        Voice = AVSpeechSynthesisVoice.FromLanguage("en-AU"),
            //        Volume = 0.5F,
            //        PitchMultiplier = 1
            //    };
            //    speechSynthesizer.SpeakUtterance(speechUtterance);
            //}

        }

        //For handling when the user selects from a menu of options.
        private List<string> _menuItems = new List<string>();
        private Action<string> _menuSelectionCallback;

        private async Task Predict(string textToPredict)
        {
            try
            {
                using (var router = IntentRouter.Setup("fda94187-6a76-4a69-bc18-efa17f7e1a52", "d2e15ac749254c06a4a29096cfa660d2", this, true))
                {
                    var handled = await router.Route(textToPredict, this);
                    if (!handled)
                        AddMessage("Sorry, I didn't get that", ChatMessageType.Incoming);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public Task FindLocation(string searchString)
        {
            return Task.Run(async () =>
            {
                var geocodeServiceUrl = @"https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer";
                LocatorTask geocodeTask = await LocatorTask.CreateAsync(new Uri(geocodeServiceUrl));
                var suggResults = await geocodeTask.SuggestAsync(searchString);
                if (suggResults.Count == 0)
                {
                    this.AddMessage("Sorry, I couldn't find that!", ChatMessageType.Incoming);
                    return;
                }
                if (suggResults.Count == 1)
                {
                    this.AddMessage("Finding directions to...", ChatMessageType.Incoming);
                }
                else 
                {
                    this.AddMessage("I found multiple locations, which one?", ChatMessageType.Incoming);
                    _menuItems = suggResults.Take(3).Select(i => i.Label).ToList();

                    foreach (var x in _menuItems.Select((value, index) => new { value, index }))
                    {
                        this.AddMessage($"{x.index + 1}. {x.value}", ChatMessageType.Incoming);
                    }

                    _menuSelectionCallback = async (selection) =>
                    {
                        var intSelection = MoravecLabs.Utilities.StringNumberToInteger(selection);
                        if (_menuItems.Count < intSelection)
                        {
                            this.AddMessage($"Please use a number between 1 and {_menuItems.Count}", ChatMessageType.Incoming);
                            return;
                        }
                        var destination = _menuItems[intSelection - 1];
                        this.AddMessage($"Getting directions to {destination}", ChatMessageType.Incoming);
                        var gcResults = await geocodeTask.GeocodeAsync(destination);
                        this.Destination.Value = gcResults.FirstOrDefault();
                        if (this.Destination.Value == null)
                        {
                            this.AddMessage("Sorry, I wasn't able to find that location.", ChatMessageType.Incoming);
                            return;
                        }
                        _menuSelectionCallback = null;
                    };
                }

            });
        }

        [IntentHandler(0.65, Name = "Get Directions")]
        public Task<bool> GetDirections(LuisResult result, object context)
        {
            return Task.Run(async () => 
            {
                if (result.DialogResponse.Status != "Finished")
                {
                    //Need to ask the user something
                    this.AddMessage(result.DialogResponse.Prompt, ChatMessageType.Incoming);
                    return true;
                } 
                else
                {
                    //Else, we should already have a destination
                    try
                    {
                        var destination = result.Entities.First(i => i.Key == "Destination").Value.First();
                        this.AddMessage($"Ok, looking for {destination.Value}", ChatMessageType.Incoming);
                        await FindLocation(destination.Value);
                        return true;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return false;
            });

        }

        [IntentHandler(0.65, Name = "Menu Selection")]
        public Task<bool> SelectMenuItem(LuisResult result, object context)
        {
            return Task.Run( () =>
            {
                if (this._menuSelectionCallback == null)
                {
                    return false;
                }
                //First try to pull out the MenuItem entity, failing that we try for the first numeric entity.
                var menuItemEntity = result.Entities.FirstOrDefault(i => i.Key == "Menu Item").Value;
                if (menuItemEntity == null || menuItemEntity.Count == 0)
                {
                }
                else
                {
                    //Got the path value,so try to decode it.
                    var temp = menuItemEntity[0].Value;
                    this._menuSelectionCallback(temp);
                }


                return true;
            });
        }

        [IntentHandler(0.7, Name = "None")]
        public Task<bool> None(LuisResult result, object context)
        {
            return Task.FromResult(false);
        }
    }
}
