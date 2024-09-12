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
        protected Dictionary<string, string> manifest;

        public Item(Image icon, string name, string path, Dictionary<string, string> manifest)
        {
            this.icon = icon;
            this.name = name;
            this.path = path;
            this.manifest = manifest;
        }

        public Image Icon
        {
            get { return this.icon; }
        }

        public string Name
        {
            get { return this.name;}
            set { this.name = value; }
        }

        public string Path
        {
            get { return this.path; }
        }

        public string Vendor
        {
            get { return this.manifest["MIDlet-Vendor"]; }
        }

        public Dictionary<string, string> Manifest
        {
            get { return this.manifest; }
        }

    }
}
