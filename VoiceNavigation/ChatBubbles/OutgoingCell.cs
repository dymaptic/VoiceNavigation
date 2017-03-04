// <copyright file="OutgoingCell.cs" company="Moravec Labs, LLC">
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

namespace VoiceNavigation
{
    using System;
    using Foundation;
    using UIKit;

    /// <summary>
    /// Outgoing cell.
    /// </summary>
    [Register("OutgoingCell")]
    public class OutgoingCell : BubbleCell
    {
        /// <summary>
        /// The cell identifier.
        /// </summary>
        public static readonly NSString CellId = new NSString("Outgoing");

        /// <summary>
        /// The normal bubble image.
        /// </summary>
        private static readonly UIImage NormalBubbleImage;

        /// <summary>
        /// The highlighted bubble image.
        /// </summary>
        private static readonly UIImage HighlightedBubbleImage;

        /// <summary>
        /// Initializes static members of the <see cref="T:VoiceNavigation.OutgoingCell"/> class.
        /// </summary>
        static OutgoingCell()
        {
            UIImage mask = UIImage.FromBundle("ChatBubbleOutgoing");

            var cap = new UIEdgeInsets 
            {
                Top = 17f,
                Left = 21f,
                Bottom = 17f,
                Right = 26f
            };

            var normalColor = UIColor.FromRGB(43, 119, 250);
            var highlightedColor = UIColor.FromRGB(32, 96, 200);

            NormalBubbleImage = CreateColoredImage(normalColor, mask).CreateResizableImage(cap);
            HighlightedBubbleImage = CreateColoredImage(highlightedColor, mask).CreateResizableImage(cap);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VoiceNavigation.OutgoingCell"/> class.
        /// </summary>
        /// <param name="handle">Handle of the cell.</param>
        public OutgoingCell(IntPtr handle) : base(handle)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VoiceNavigation.OutgoingCell"/> class.
        /// </summary>
        public OutgoingCell()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VoiceNavigation.OutgoingCell"/> class.
        /// </summary>
        /// <param name="style">cell style.</param>
        /// <param name="reuseIdentifier">Reuse identifier.</param>
        [Export("initWithStyle:reuseIdentifier:")]
        public OutgoingCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        private void Initialize()
        {
            this.BubbleHighlightedImage = HighlightedBubbleImage;
            this.BubbleImage = NormalBubbleImage;

            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(
                "H:[bubble]|",
                0, 
                "bubble", 
                this.BubbleImageView));

            BubbleImageView.AddConstraints(NSLayoutConstraint.FromVisualFormat(
                "H:[bubble(>=48)]",
                0,
                "bubble", 
                this.BubbleImageView));

            var spaceTop = NSLayoutConstraint.Create(
                this.MessageLabel, 
                NSLayoutAttribute.Top, 
                NSLayoutRelation.Equal, 
                this.BubbleImageView, 
                NSLayoutAttribute.Top, 
                1, 
                10);
            this.ContentView.AddConstraint(spaceTop);

            var spaceBottom = NSLayoutConstraint.Create(
                this.MessageLabel, 
                NSLayoutAttribute.Bottom, 
                NSLayoutRelation.Equal, 
                this.BubbleImageView, 
                NSLayoutAttribute.Bottom, 
                1, 
                -10);
            this.ContentView.AddConstraint(spaceBottom);

            var msgTrailing = NSLayoutConstraint.Create(
                this.MessageLabel, 
                NSLayoutAttribute.Trailing, 
                NSLayoutRelation.LessThanOrEqual, 
                this.BubbleImageView, 
                NSLayoutAttribute.Trailing, 
                1, 
                -16);
            this.ContentView.AddConstraint(msgTrailing);

            var msgCenter = NSLayoutConstraint.Create(
                this.MessageLabel, 
                NSLayoutAttribute.CenterX, 
                NSLayoutRelation.Equal, 
                this.BubbleImageView, 
                NSLayoutAttribute.CenterX, 
                1, 
                -3);
            ContentView.AddConstraint(msgCenter);

            this.MessageLabel.TextColor = UIColor.White;
        }
    }
}