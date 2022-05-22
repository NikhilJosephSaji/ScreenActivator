using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ScreenActivator.Buisness
{
    public class XmlHelper
    {
        private XElement xml;
        private string path;

        public XElement Xml { get { return xml != null ? xml : null; } }
        public XmlHelper()
        {
            path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            xml = XElement.Load(path + "\\Buisness\\ScreenActData.xml");
        }

        public bool XmlStringToBool(string str)
        {
            return Convert.ToBoolean(Split(Encryption.Decrypt(str)));
        }

        public int SaveXml()
        {
            try
            {
                var path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                path = path + "\\Buisness\\ScreenActData.xml";
                xml.Save(path);
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        private string Split(string str)
        {
            int index = str.LastIndexOf("-");
            str = str.Substring(index);
            if (str.Contains("False"))
                str = "False";
            else
                str = "True";
            return str;
        }
    }
}
