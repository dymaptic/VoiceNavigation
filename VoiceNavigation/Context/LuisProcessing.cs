// // <copyright file="LuisProcessing.cs" company="Moravec Labs, LLC">
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

namespace VoiceNavigation.Context
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Cognitive.LUIS;

    /// <summary>
    /// Luis processing.
    /// </summary>
    public static class LuisProcessing
    {
        /// <summary>
        /// Predict from text.
        /// </summary>
        /// <param name="text">Text.</param>
        public static async Task<LuisResult> PredictText(string text)
        {
            try
            {
                var client = new LuisClient("fda94187-6a76-4a69-bc18-efa17f7e1a52", 
                                            "1e9b1ceb8bc844ce8eec97e41b67319b", 
                                            true, 
                                            "westus");
                var result = await client.Predict(text);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
