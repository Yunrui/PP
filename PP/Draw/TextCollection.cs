namespace PP.Draw
{
    using System.Collections.Generic;

    /// <summary>
    /// The collection of the text waiting to be drawn
    /// </summary>
    public class TextCollection
    {
        private TextCollection() 
        {
            this.Collection = new List<TextItem>();
        }

        private static TextCollection instance;

        public static TextCollection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TextCollection();
                }

                return instance;
            }

            set
            {
                instance = value;
            }
        }

        public List<TextItem> Collection { get; set; }
    }

    /// <summary>
    /// Text items will be drawn to the bitmap once to improve the performance
    /// </summary>
    public class TextItem
    {
        public TextItem()
        {
            this.IsHyperLink = false;
        }

        /// <summary>
        /// Gets or sets the left corner's x-pos
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// Gets or sets the left corner's y-pos
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// Gets or sets the context of the item
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets whether the item is a hyperlink
        /// </summary>
        public bool IsHyperLink { get; set; }
    }
}
