// <copyright file="Context.cs" company="Moravec Labs, LLC">
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

namespace VoiceNavigation.Context
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using MoravecLabs.Atom;

    /// <summary>
    /// Context - A list of subjects that must be collected, and actions to help collect them, and to do when the context is complete.
    /// </summary>
    public class Context
    {

        private List<ActionHolder> ActionList;

        /// <summary>
        /// Gets the subjects.
        /// </summary>
        /// <value>The subjects.</value>
        public List<Subject> Subjects { get; private set;}

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:VoiceNavigation.Context.Context"/> is ready.
        /// </summary>
        /// <value><c>true</c> if is ready; otherwise, <c>false</c>.</value>
        public bool IsReady
        {
            get
            {
                // If all of the Subjects are ready (none are false) then the context is ready.
                if (this.Subjects.Count(i => i.IsReady.Value == false) == 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VoiceNavigation.Context.Context"/> class.
        /// </summary>
        public Context()
        {
            this.Subjects = new List<Subject>();
            this.ActionList = new List<ActionHolder>();
        }

        public void AddSubject(Subject newSubject)
        {
            newSubject.Context.Value = this;
            this.Subjects.Add(newSubject);
        }

        /// <summary>
        /// Gets the subject by name.  Null is returned if no subject by that name is found.  Duplicate names will have unexpected results.
        /// </summary>
        /// <returns>The subject by name.</returns>
        /// <param name="name">Name to search for.</param>
        public Subject GetSubjectByName(string name)
        {
            return this.Subjects.FirstOrDefault(i => i.Name.Value.Equals(name));
        }

        /// <summary>
        /// Adds the action for the given conditions
        /// </summary>
        /// <param name="newAction">New action.</param>
        /// <param name="conditions">Conditions.</param>
        public void AddAction(Func<Context, List<string>> newAction, IEnumerable<Subject> conditions)
        {
            this.ActionList.Add(new ActionHolder { Action = newAction, Conditions = conditions });
        }

        /// <summary>
        /// Evalulate the specified query.  Returns a list of "things to say."  Each of the items in the list should be 
        /// displayed to the user as output from the bot.
        /// </summary>
        /// <param name="query">Query.</param>
        public async Task<List<string>> Evalulate(string query)
        {
            var queryConsumed = false;
            foreach (var s in this.Subjects)
            {
                if (s.IsReady.Value == false)
                {
                    var retValue = new List<string>();

                    if (!queryConsumed)
                    {
                        retValue = await s.Next(query);
                    }
                    else 
                    {
                        retValue = await s.Next(null);
                    }

                    // If the return value is either null or an empty list, and the subject is ready, skip to the next one.
                    // Otherwise just return whatever the subject returned.
                    if ((retValue == null || retValue.Count() == 0) & s.IsReady.Value)
                    {
                        queryConsumed = true;
                        continue;
                    }
                    retValue.AddRange(this.EvaluateActions());
                    return retValue;
                }
            }
            return this.EvaluateActions();
        }

        /// <summary>
        /// Evaluates the actions.
        /// </summary>
        /// <returns>The actions.</returns>
        private List<string> EvaluateActions()
        {
            var retValue = new List<string>();
            foreach (var action in this.ActionList)
            {
                // If all actions are true (none are false)
                if (action.Conditions.Count(c => c.IsReady.Value == false) == 0)
                {
                    var temp = action.Action(this);
                    if (temp != null)
                    {
                        retValue.AddRange(temp);
                    }
                }
            }
            return retValue;
        }
    }

    /// <summary>
    /// Action holder.
    /// </summary>
    internal struct ActionHolder
    {
        /// <summary>
        /// The action.
        /// </summary>
        public Func<Context, List<string>> Action;

        /// <summary>
        /// The conditions.
        /// </summary>
        public IEnumerable<Subject> Conditions;
    }
}
