using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace J2ME_Explorer
{
    class Item
    {
        protected Image icon;
        protected string name;
        protected string path;
        protected string vendor;

        public Item(Image icon, string name, string path, string vendor)
        {
            this.icon = icon;
            this.name = name;
            this.path = path;
            this.vendor = vendor;
        }

        public Image Icon
        {
            get { return icon; }
        }

        public string Name
        {
            get { return name;}
            set { name = value; }
        }

        public string Path
        {
            get { return path; }
        }

        public string Vendor
        {
            get { return vendor; }
        }

    }
}
