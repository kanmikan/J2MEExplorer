using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2ME_Explorer
{
    internal class JarItem
    {
        string filename;
        long fileSize;
        DateTime fileDate;
        public JarItem(string filename, long fileSize, DateTime fileDate) 
        {
            this.filename = filename;
            this.fileSize = fileSize;
            this.fileDate = fileDate;
        }

        public string FileName 
        {
            get { return this.filename; }
        }

        public long FileSize 
        {
            get { return fileSize; }
        }

        public DateTime FileDate
        {
            get { return fileDate; }
        }
    }
}
