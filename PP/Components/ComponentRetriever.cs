﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PP
{
    class ComponentRetriever
    {
        internal static Component Retrieve(string componentUri)
        {
            // get the component name from the uri
            // the uri may be like : /Assets/Label.png

            int startPos = componentUri.LastIndexOf("/");
            int endPos = componentUri.LastIndexOf(".");

            string componentName = componentUri.Substring(startPos + 1, endPos - startPos - 1).ToLower();

            if (componentName.Equals("textbox"))
            {
                return new Components.TextBox();
            }
            else if (componentName.Equals("label"))
            {
                return new Components.Label();
            }
            else if (componentName.Equals("hyperlink"))
            {
                return new Components.HyperLink();
            }
            else if (componentName.Equals("button"))
            {
                return new Components.Button();
            }
            else if (componentName.Equals("icon"))
            {
                return new Components.Icon();
            }
            else if (componentName.Equals("radiobutton"))
            {
                return new Components.RadioButton();
            }
            else if (componentName.Equals("checkbox"))
            {
                return new  Components.CheckBox();
            }
            else if (componentName.Equals("combobox"))
            {
                return new Components.ComboBox();
            }
            else if (componentName.Equals("horizontalline"))
            {
                return new Components.HorizontalLine();
            }
            else if (componentName.Equals("verticalline"))
            {
                return new Components.VerticalLine();
            }
            else
            {
                throw new ArgumentException(string.Format("{0} can not be found."), componentName);
            }
        }
    }
}
