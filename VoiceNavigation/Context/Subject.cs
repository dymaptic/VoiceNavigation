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
    using System.Collections.Generic;
    using MoravecLabs.Atom;

    /// <summary>
    /// Subject abstract class to supprt the generic type.
    /// </summary>
    public abstract class Subject
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public Atom<string> Name { get; internal set; }

        /// <summary>
        /// Gets or sets the is ready.  Set to true when the subject is ready and the context can move to the next subject.
        /// </summary>
        /// <value>The is ready.</value>
        public Atom<bool> IsReady { get; internal set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public Atom<Context> Context { get; set; }

        /// <summary>
        /// Gets or sets the next action.
        /// </summary>
        /// <value>The next action.</value>
        public Func<Subject, string, List<string>> NextAction {get; set;}

        /// <summary>
        /// Next the specified result.
        /// </summary>
        /// <param name="result">Result.</param>
        public abstract List<string> Next(string result);

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">Value.</param>
        public abstract void SetValue(object value);


    }

    /// <summary>
    /// Subject - What a context is about.
    /// </summary>
    public class Subject<T> : Subject
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public Atom<T> Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VoiceNavigation.Context.Subject"/> class.
        /// </summary>
        public Subject(string name)
        {
            this.Name = new Atom<string>(name);
            this.Value = new Atom<T>(default(T));
            this.IsReady = new Atom<bool>(false);
            this.Context = new Atom<Context>(default(Context));
        }

        public override void SetValue(object value)
        {
            this.Value.Value = (T)value;
            this.IsReady.Value = true;
        }

        public override List<string> Next(string result)
        {
            if (this.NextAction != null)
            {
                return this.NextAction(this, result);
            }

            var r = new List<string>();
            r.Add(result);
            return r;
        }
    }
}
