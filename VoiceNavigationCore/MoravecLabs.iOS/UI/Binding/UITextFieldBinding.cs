// <copyright file="UITextFieldBinding.cs" company="Moravec Labs, LLC">
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

namespace MoravecLabs.UI.Binding
{
    using System;
    using Infrastructure;
    using UIKit;

    /// <summary>
    /// Binding for a UITextField, changes to the specified viewModel are pushed to the UITextfield automatically.
    /// </summary>
    public class UITextFieldBinding : BaseBinding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MoravecLabs.UI.Binding.UITextFieldBinding"/> class.
        /// </summary>
        /// <param name="viewModel">View model.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="textField">Text field.</param>
        public UITextFieldBinding(BaseViewModel viewModel, string propertyName, UITextField textField)
        {
            viewModel.SubscribePropertyChanged(propertyName, () => { textField.Text = Convert.ToString(viewModel.GetPropertyByName(propertyName)); });
        }
    }
}
