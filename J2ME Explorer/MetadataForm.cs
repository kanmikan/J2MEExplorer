using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace J2ME_Explorer
{
    public partial class MetadataForm : Form
    {
        private Item item;

        public MetadataForm(Item item)
        {
            InitializeComponent();
            this.item = item;

            metadataList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            metadataList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            metadataList.AllowUserToAddRows = false;
            metadataList.AllowUserToResizeRows = false;

            this.Text = $"Manifest de {item.Name}";
            
            //ICollection<string> keys = this.item.Manifest.Keys;
            Dictionary<string, string> manifest = this.item.Manifest;

            metadataList.ColumnCount = 2;
            metadataList.Columns[0].Name = "ID";
            metadataList.Columns[1].Name = "Valor";

            foreach (var kvp in manifest)
            {
                metadataList.Rows.Add(kvp.Key, kvp.Value);
            }

        }
    }
}
