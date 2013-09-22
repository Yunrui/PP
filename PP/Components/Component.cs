namespace PP
{
    using PP.Components;
    using PP.Components.Interface;
    using System.Runtime.Serialization;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Imaging;

    public abstract class Component : UserControl, IComponent
    {
        protected const int DeltaPixel = 2;

        public string Text { get; set; }
        
        public string ComponentName
        {
            get { return this.GetType().FullName; }
        }

        public double ComponentMinWidth { get { return this.MinWidth; } }

        public double ComponentMinHeight { get { return this.MinHeight; } }

        public new double Width
        {
            get
            {
                return base.Width;
            }

            set
            {
                base.Width = value;
            }
        }

        public new double Height 
        { 
            get 
            { 
                return base.Height; 
            }

            set
            {
                base.Height = value;
            }
        }

        public double Left { get; set; }
        
        public double Top { get; set; }

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

        public static Component CreateComponent(SerializerComponent serivalizerComponent)
        {
            Component component;
            switch (serivalizerComponent.ComponentName)
            {
                case "Icon" :
                    component = new Icon();
                    break;
                case "Button" :
                    component = new PP.Components.Button();
                    break;
                case "CheckBox" :
                    component = new PP.Components.CheckBox();
                    break;
                case "ComboBox":
                    component = new PP.Components.ComboBox();
                    break;
                case "HorizontalLine":
                    component = new HorizontalLine();
                    break;
                case "HyperLink":
                    component = new HyperLink();
                    break;
                case "Label":
                    component = new Label();
                    break;
                case "RadioButton":
                    component = new PP.Components.RadioButton();
                    break;
                case "TextBox":
                    component = new PP.Components.TextBox();
                    break;
                case "VerticalLine":
                    component = new VerticalLine();
                    break;
                default:
                    component = null;
                    break;
            }

            if (component != null)
            {
                component.Width = serivalizerComponent.Width;
                component.Height = serivalizerComponent.Height;
                
                component.Left = serivalizerComponent.Left;
                component.Top = serivalizerComponent.Top;

                component.SetText(serivalizerComponent.Text);
            }
            else
            {
                Instrumentation.Current.Log(string.Format("Unexpected Component Name : {0}", serivalizerComponent.ComponentName));
            }

            return component;
        }

        public virtual void SetText(string text)
        {
            this.Text = text;
        }
    }
}
