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
    public partial class ContentForm : Form
    {
        private Item item;

        public ContentForm(Item item)
        {
            InitializeComponent();
            this.item = item;

            contentList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            contentList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            contentList.AllowUserToAddRows = false;
            contentList.AllowUserToResizeRows = false;

            this.Text = $"Contenido de {item.Name}";
            
            List<JarItem> list = JarManager.ReadJarFilelist(this.item.Path);

            contentList.ColumnCount = 3;
            contentList.Columns[0].Name = "Archivo";
            contentList.Columns[1].Name = "Tamaño";
            contentList.Columns[2].Name = "Modificado";

            foreach (var file in list)
            {
                contentList.Rows.Add(file.FileName, file.FileSize, file.FileDate);
            }

        }
    }
}
