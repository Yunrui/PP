﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace PP
{
    public abstract class Component : UserControl, IComponent
    {
        protected const int DeltaPixel = 2;

        public string ComponentName
        {
            get { return this.GetType().FullName; }
        }

        public double ComponentMinWidth { get { return this.MinWidth; } }

        public double ComponentMinHeight { get { return this.MinHeight; } }

        public double InitialWidth { get { return this.Width; } }

        public double InitialHeight { get { return this.Height; } }

        public ResizeAnchorMode AnchorMode 
        { 
            get 
            { 
                return this.anchorMode; 
            }
 
            set
            {
                this.anchorMode = value;
            }
        }

        private ResizeAnchorMode anchorMode = ResizeAnchorMode.Full;

        public virtual void Resize(double percentageWidth, double percentageHeight)
        {
            this.Width *= percentageWidth;
            this.Height *= percentageHeight;
        }

        public virtual void Draw(WriteableBitmap bitmap, int left, int top)
        {
        }
    }
}