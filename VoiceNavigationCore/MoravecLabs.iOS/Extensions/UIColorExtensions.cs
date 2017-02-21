// <copyright file="UICOlorExtensions.cs" company="Moravec Labs, LLC">
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

namespace MoravecLabs.Extensions
{
    using System;
    using UIKit;

    /// <summary>
    /// Extensions to UIColor
    /// </summary>
    public static class UIColorExtensions
    {
        /// <summary>
        /// UIColor from Hex (as int).  Originally from: http://stackoverflow.com/questions/10310917/uicolor-from-hex-in-monotouch
        /// </summary>
        /// <returns>a UIColor for the given hex code</returns>
        /// <param name="color">The UIColor (this) for the extension.</param>
        /// <param name="hexValue">The hex value.</param>
        public static UIColor FromHex(this UIColor color, int hexValue)
        {
            return UIColor.FromRGB(
                ((float)((hexValue & 0xFF0000) >> 16)) / 255.0f,
                ((float)((hexValue & 0xFF00) >> 8)) / 255.0f,
                ((float)(hexValue & 0xFF)) / 255.0f);
        }
    }
}
