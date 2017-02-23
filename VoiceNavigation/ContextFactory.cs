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
        public static Context.Context GetDirectionsContext()
        {
            var c = new Context.Context();
            var requestedDestination = new Context.Subject<string>("RequestedDestination");
            var destinationOptions = new Context.Subject<List<string>>("DestinationOptions");
            var finalDestination = new Context.Subject<string>("FinalDestination");
            requestedDestination.NextAction = (subject, query) =>
            {
                var answer = new List<string>();
                if (string.IsNullOrEmpty(query))
                {
                    answer.Add("Where To?");
                }

                // This means we got an answer, so we are going to Pass this off to Luis.as to process it 
                // and then we can make a decision about what to do.

                // Once we get the answer from luis.ai, we need to figure out if we understand the location
                // So we pass the "destination" to the Esri Geocoder and see what happens.

                // If we fully understand what happened, we call SetValue on finalDestination to alert 
                // anyone listening that we know where to go.

                // If we don't fully understand it, and we want to ask the user to pick from a list,
                // we can set the value on DestinationOptions and on this, and let the next code block handle it.

                return answer;
            };


            c.AddSubject(requestedDestination);
            c.AddSubject(destinationOptions);
            c.AddSubject(finalDestination);

            return c;
        }
        #endregion
    }
}
