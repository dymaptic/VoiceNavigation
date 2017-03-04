// <copyright file="Utilities.cs" company="Moravec Labs, LLC">
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

namespace MoravecLabs
{
    using System;

    /// <summary>
    /// Contains utility methods.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Converts a number to a string that means the order, 1 => first, 2 => seconds, etc.
        /// </summary>
        /// <returns>A string representation of the number.</returns>
        /// <param name="number">The number to convert.</param>
        public static int StringNumberToInteger(string number)
        {
            try
            {
                return Convert.ToInt32(number);
            }
            catch
            {
            }

            switch (number)
            {
                case "first":
                    return 1;
                case "second":
                    return 2;
                case "third":
                    return 3;
                case "one":
                    return 1;
                case "two":
                    return 2;
                case "three":
                    return 3;
                default:
                    throw new InvalidCastException("Unable to convert string to number");
            }
        }
    }
}
