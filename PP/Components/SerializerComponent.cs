namespace PP.Components
{
    using System.Runtime.Serialization;

    [DataContract]
    public class SerializerComponent
    {
        public SerializerComponent(Component component)
        {
            this.ComponentName = component.ComponentName;

            this.Width = component.Width;
            this.Height = component.Height;

            this.Left = component.Left;
            this.Top = component.Top;
        }

        [DataMember]
        public string ComponentName
        {
            get
            {
                return this.componentName;
            }

            set
            {
                this.componentName = value;

                int lastDot = this.componentName.LastIndexOf('.');

                if (lastDot + 1 < this.componentName.Length)
                {
                    this.componentName = this.componentName.Substring(lastDot + 1);
                }
            }
        }

        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public new double Height { get; set; }

        [DataMember]
        public double Left { get; set; }

        [DataMember]
        public double Top { get; set; }

        private string componentName;
    }
}
