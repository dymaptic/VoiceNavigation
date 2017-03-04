// // <copyright file="ContextUtils.cs" company="Moravec Labs, LLC">
// //     MIT License
// //
// //     Copyright (c) Moravec Labs, LLC.
// //
// //     Permission is hereby granted, free of charge, to any person obtaining a copy
// //     of this software and associated documentation files (the "Software"), to deal
// //     in the Software without restriction, including without limitation the rights
// //     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// //     copies of the Software, and to permit persons to whom the Software is
// //     furnished to do so, subject to the following conditions:
// //
// //     The above copyright notice and this permission notice shall be included in all
// //     copies or substantial portions of the Software.
// //
// //     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// //     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// //     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// //     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// //     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// //     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// //     SOFTWARE.
// // </copyright>

namespace VoiceNavigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Esri.ArcGISRuntime.Geometry;
    using Esri.ArcGISRuntime.Tasks.Geocoding;

    /// <summary>
    /// Context utils.
    /// </summary>
    public static class ContextFactory
    {
        #region Repeater
        public static Context.Context GetRepeater()
        {
            var c = new Context.Context();
            c.AddSubject(new Context.Subject<string>("Pete"));
            return c;
        }
        #endregion

        #region Simple Interview
        public static Context.Context GetSimpleInterview()
        {
            var c = new Context.Context();

            var name = new Context.Subject<string>("Name");

            name.NextAction = (subject, query) =>
            {
                
                var answer = new List<string>();
                if (string.IsNullOrEmpty(query))
                {
                    answer.Add("What is your name?");
                    return answer;
                }

                //Assume that whatever they told is is their name
                subject.SetValue(query);

                //Return an empty list so that the context moves to the next subject.
                return answer;
            };

            var age = new Context.Subject<int>("Age");
            age.NextAction = (subject, query) =>
            {
                var answer = new List<string>();
                if (string.IsNullOrEmpty(query))
                {
                    answer.Add("How old are you?");
                    return answer;
                }

                // Test to ensure that the age is a number
                try
                {
                    var userAge = Convert.ToInt32(query);
                    subject.SetValue(userAge);
                }
                catch
                {
                    // It wasn't a number, so provide some feedback
                    answer.Add("Sorry, I didn't get that.  Please enter a number");
                    answer.Add("How old are you?");
                }

                return answer;
            };

            c.AddSubject(name);
            c.AddSubject(age);
            c.AddAction((context) =>
            {
                return new List<string> { "Thanks!" };
            }, 
                        new List<Context.Subject> { name, age });

            return c;
        }
        #endregion

        #region GetDirections
        public static Context.Context GetDirectionsContext(Func<Context.Context, List<string>> action)
        {
            var c = new Context.Context();
            var requestedDestination = new Context.Subject<string>("RequestedDestination");
            var destinationOptions = new Context.Subject<List<GeocodeResult>>("DestinationOptions");
            var finalDestination = new Context.Subject<GeocodeResult>("FinalDestination");
            var currentLocation = new Context.Subject<MapPoint>("CurrentLocation");

            requestedDestination.NextActionAsync = async (subject, query) =>
            {
                var answer = new List<string>();
                if (string.IsNullOrEmpty(query))
                {
                    answer.Add("Where To?");
                    return answer;
                }

                // This means we got an answer, so we are going to Pass this off to Luis.as to process it 
                // and then we can make a decision about what to do.

                var luisResult = await Context.LuisProcessing.PredictText(query);

                // If it was not identified as the correct intent
                if (luisResult.TopScoringIntent.Name != "Get Directions")
                {
                    answer.Add("Sorry, I didn't get that.");
                    return answer;
                }

                // It was the right intent, but we are not very sure about it.
                if (luisResult.TopScoringIntent.Score < 0.50)
                { 
                    answer.Add("Try just telling me the address.");
                    return answer;
                }

                // This means that Luis want's us to ask the user a specifc question
                if (luisResult.DialogResponse.Status != "Finished")
                {
                    answer.Add(luisResult.DialogResponse.Prompt);
                    return answer;
                }

                // Once we get the answer from luis.ai, we need to figure out if we understand the location
                // So we pass the "destination" to the Esri Geocoder and see what happens.
                // Note that we search nearby, assuming that the user is not going that far.

                var geocodeServiceUrl = @"https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer";
                LocatorTask geocodeTask = await LocatorTask.CreateAsync(new Uri(geocodeServiceUrl));

                var geocodeProperties = new GeocodeParameters
                {
                    PreferredSearchLocation = subject.Context.Value.GetSubjectByName("CurrentLocation").GetValue() as MapPoint
                };
                var suggResults = await geocodeTask.GeocodeAsync(luisResult.Entities.First(i => i.Key == "Destination").Value.First().Value,
                                                                 geocodeProperties);

                // If we have no idea what we just did
                if (suggResults.Count == 0)
                {
                    answer.Add("Nope, that's not a location, I just checked.");
                }

                // If we fully understand what happened, we call SetValue on finalDestination to alert 
                // anyone listening that we know where to go.
                else if (suggResults.Count == 1)
                {
                    finalDestination.SetValue(suggResults.First());
                    answer.Add($"Getting Directions to: {suggResults.First().Label}");
                }

                // If the first result has a score over 90, then just go for it
                else if (suggResults.First().Score > 90)
                {
                    finalDestination.SetValue(suggResults.First());
                    answer.Add($"Getting Directions to: {suggResults.First().Label}");
                }

                // If we don't fully understand it, and we want to ask the user to pick from a list,
                // we can set the value on DestinationOptions and on this, and let the next code block handle it.
                else
                {
                    subject.SetValue(luisResult.Entities.First(i => i.Key == "Destination").Value.First().Value);
                    answer.Add("I found multiple locations, which one?");
                    answer.AddRange(suggResults.Take(3).Select(s => s.Label));
                    destinationOptions.SetValue(suggResults.Take(3).ToList(), false);
                }

                return answer;
            };

            destinationOptions.NextActionAsync = async (subject, query) =>
            {
                var answer = new List<string>();
                if (string.IsNullOrEmpty(query))
                {
                    return answer;
                }

                // In this case, we should already have a list of items, and the user just displayed the list 
                // so we are expecting them to select one.  But it might not be one, they might say repeat or
                // something like that so we use luis.ai to help us out.

                var luisResult = await Context.LuisProcessing.PredictText(query);

                if (luisResult.TopScoringIntent.Name != "Menu Selection")
                {
                    return answer;
                }

                // Try to parse it directly
                try
                {
                    var position = MoravecLabs.Utilities.StringNumberToInteger(query);
                    // position is now a valid in, so get the item from the internal list and set the final value
                    var destination = ((List<GeocodeResult>)subject.GetValue())[position - 1];
                    finalDestination.SetValue(destination);
                    answer.Add($"Getting Directions to: {destination.Label}");
                    return answer;
                }
                catch (InvalidCastException)
                { }

                // No idea what it was...
                answer.Add("Say what?! Please say one, two or three");

                return answer;
            };

            c.AddSubject(requestedDestination);
            c.AddSubject(destinationOptions);
            c.AddSubject(finalDestination);
            // This is actually set by the ChatViewModel so it is always ready and will jsut be skipped.
            c.AddSubject(currentLocation);

            c.AddAction(action, new List<Context.Subject> { finalDestination });

            return c;
        }
        #endregion
    }
}
