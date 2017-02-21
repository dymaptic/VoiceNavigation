// <copyright file="BubbleCell.cs" company="Moravec Labs, LLC">
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
    using CoreGraphics;
    using Foundation;
    using UIKit;

    /// <summary>
    /// Bubble Cell as a UI TableViewCell
    /// </summary>
    public abstract class BubbleCell : UITableViewCell
    {
        /// <summary>
        /// The message.
        /// </summary>
        private ChatMessage msg;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VoiceNavigation.BubbleCell"/> class.
        /// </summary>
        /// <param name="handle">Handle for cell.</param>
        public BubbleCell(IntPtr handle) : base(handle)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VoiceNavigation.BubbleCell"/> class.
        /// </summary>
        public BubbleCell()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VoiceNavigation.BubbleCell"/> class.
        /// </summary>
        /// <param name="style">Style for the cell.</param>
        /// <param name="reuseIdentifier">Reuse identifier.</param>
        [Export("initWithStyle:reuseIdentifier:")]
        public BubbleCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the bubble image view.
        /// </summary>
        /// <value>The bubble image view.</value>
        public UIImageView BubbleImageView { get; private set; }

        /// <summary>
        /// Gets the message label.
        /// </summary>
        /// <value>The message label.</value>
        public UILabel MessageLabel { get; private set; }

        /// <summary>
        /// Gets or sets the bubble image.
        /// </summary>
        /// <value>The bubble image.</value>
        public UIImage BubbleImage { get; set; }

        /// <summary>
        /// Gets or sets the bubble highlighted image.
        /// </summary>
        /// <value>The bubble highlighted image.</value>
        public UIImage BubbleHighlightedImage { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public ChatMessage Message
        {
            get
            {
                return this.msg;
            }

            set
            {
                this.msg = value;
                this.BubbleImageView.Image = this.BubbleImage;
                this.BubbleImageView.HighlightedImage = this.BubbleHighlightedImage;

                this.MessageLabel.Text = this.msg.Text;

                this.MessageLabel.UserInteractionEnabled = true;
                this.BubbleImageView.UserInteractionEnabled = false;
            }
        }

        /// <summary>
        /// Sets the selected.
        /// </summary>
        /// <param name="selected">If set to <c>true</c> selected.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
            this.BubbleImageView.Highlighted = selected;
        }

        /// <summary>
        /// Creates the colored image.
        /// </summary>
        /// <returns>The colored image.</returns>
        /// <param name="color">new color.</param>
        /// <param name="mask">image mask.</param>
        protected static UIImage CreateColoredImage(UIColor color, UIImage mask)
        {
            var rect = new CGRect(CGPoint.Empty, mask.Size);
            UIGraphics.BeginImageContextWithOptions(mask.Size, false, mask.CurrentScale);
            CGContext context = UIGraphics.GetCurrentContext();
            mask.DrawAsPatternInRect(rect);
            context.SetFillColor(color.CGColor);
            context.SetBlendMode(CGBlendMode.SourceAtop);
            context.FillRect(rect);
            UIImage result = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return result;
        }

        /// <summary>
        /// Creates the bubble with border.
        /// </summary>
        /// <returns>The bubble with border.</returns>
        /// <param name="bubbleImg">Bubble image.</param>
        /// <param name="bubbleColor">Bubble color.</param>
        protected static UIImage CreateBubbleWithBorder(UIImage bubbleImg, UIColor bubbleColor)
        {
            bubbleImg = CreateColoredImage(bubbleColor, bubbleImg);
            CGSize size = bubbleImg.Size;

            UIGraphics.BeginImageContextWithOptions(size, false, 0);
            var rect = new CGRect(CGPoint.Empty, size);
            bubbleImg.Draw(rect);

            var result = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return result;
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        private void Initialize()
        {
            this.BubbleImageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            this.MessageLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Lines = 0,
                PreferredMaxLayoutWidth = 220f,
                LineBreakMode = UILineBreakMode.WordWrap,
                AutoresizingMask = UIViewAutoresizing.FlexibleHeight
            };

            this.ContentView.AddSubviews(this.BubbleImageView, this.MessageLabel);
        }
    }
}