using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace J2ME_Explorer
{
    internal class CustomListView : ListView
    {
        public static Size box = new Size(90, 90);
        private List<Item> items = new List<Item>();
        private ContextMenuStrip contextMenuStrip;
        private ImageList imageList;
        private bool DEBUG = false;

        public CustomListView()
        {
            this.DoubleBuffered = true;
            this.OwnerDraw = true;
            this.View = View.Details;
            this.VirtualMode = true;
            this.BorderStyle = BorderStyle.None;
            this.VirtualListSize = this.LargeImageList?.Images.Count ?? 0;
            this.RetrieveVirtualItem += ListView_RetrieveVirtualItem;
            this.DrawItem += CustomListView_DrawItem;
            this.MouseClick += CustomListView_MouseClick;

            //menu contextual
            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add("Abrir", null, Abrir_Click);
            contextMenuStrip.Items.Add("Ver Metadatos", null, VerMetadatos_Click);
            contextMenuStrip.Items.Add("Ver Contenido", null, VerContenido_Click);
            contextMenuStrip.Items.Add("Mostrar en carpeta", null, Mostrar_Click);
            //contextMenuStrip.Items.Add("Experimental", null, Test_Click);

            //imageList
            this.imageList = new ImageList
            {
                ImageSize = new Size(box.Width - 12, box.Height + 6),
                ColorDepth = ColorDepth.Depth32Bit
            };
            this.LargeImageList = imageList;

        }

        private void CustomListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawBackground();
            Graphics g = e.Graphics;

            Rectangle margins = new Rectangle(0, 4, 0, 0);

            //area
            Rectangle boundsWithMargins = new Rectangle(
                e.Bounds.Left + margins.X,
                e.Bounds.Top + margins.Y,
                e.Bounds.Width - (margins.X + margins.Width),
                e.Bounds.Height - (margins.Y + margins.Height)
            );

            var item = items[e.ItemIndex];
            Image icon = item.Icon;

            //dibujar icono
            if (icon != null)
            {
                int iconX = boundsWithMargins.Left + (boundsWithMargins.Width - box.Width) / 2;
                int iconY = boundsWithMargins.Top;
                Rectangle imageRect = new Rectangle(iconX, iconY, box.Width, box.Height);

                //debug
                if (DEBUG)
                {
                    using (Brush bgBrush = new SolidBrush(Color.FromArgb(128, Color.Green)))
                    {
                        g.FillRectangle(bgBrush, new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height));
                    }

                    using (Brush bgBrush = new SolidBrush(Color.FromArgb(128, Color.Red)))
                    {
                        g.FillRectangle(bgBrush, imageRect);
                    }
                }

                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(icon, imageRect);
            }

            //dibujar texto
            if (!string.IsNullOrEmpty(item.Name))
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                StringFormat format = new StringFormat { Alignment = StringAlignment.Center };

                int iconHeight = box.Height;
                int textHeight = 60;

                RectangleF textRect = new RectangleF(boundsWithMargins.Left, boundsWithMargins.Top + iconHeight + 5, boundsWithMargins.Width, textHeight);
                g.DrawString(item.Name, e.Item.Font, Brushes.Black, textRect, format);
            }

            e.DrawFocusRectangle();
        }

        public ImageList ImageList
        {
            get { return this.imageList; }
        }

        public void FocusAndSelect()
        {
            this.BringToFront();

            this.Focus();

            if (this.Items.Count > 0)
            {
                this.Items[0].Selected = true;
                this.Items[0].Focused = true;
                this.EnsureVisible(0);
            }
        }

        private void ShowContextMenu(Point location, Item item)
        {
            contextMenuStrip.Tag = item;
            contextMenuStrip.Show(this, location);
        }

        private void VerMetadatos_Click(object sender, EventArgs e) 
        {
            Item item = (Item)contextMenuStrip.Tag;
            MetadataForm metadataForm = new MetadataForm(item);
            //metadataForm.ShowDialog();
            metadataForm.Show();
        }

        private void VerContenido_Click(object sender, EventArgs e)
        {
            Item item = (Item)contextMenuStrip.Tag;
            ContentForm contentForm = new ContentForm(item);
            contentForm.Show();
        }

        private void Test_Click(object sender, EventArgs e) 
        {
            var item = (Item)contextMenuStrip.Tag;
            //List<string> jfiles = JarManager.ReadJarFilelist(item.Path);
            bool dim = JarManager.CheckImageDims(item.Path, new List<string> { "240x320", "176x208", "208x208" });
            Console.WriteLine(dim);

        }
        private void Abrir_Click(object sender, EventArgs e) 
        {
            var item = (Item) contextMenuStrip.Tag;
            Action_Click(sender, e, item.Path);
        }

        private void Mostrar_Click(object sender, EventArgs e) 
        {
            var item = (Item) contextMenuStrip.Tag;
            //abrir carpeta y marcar el item
            //TODO: menos hardcodeado?
            
            switch (Environment.OSVersion.Platform) 
            {
                case PlatformID.Win32NT:
                    Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{item.Path}\"") { UseShellExecute = true });
                    break;
                default:
                    break;
            }

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Enter && this.SelectedIndices.Count > 0)
            {
                int selectedIndex = this.SelectedIndices[0];
                if (selectedIndex >= 0 && selectedIndex < items.Count)
                {
                    var selectedItem = items[selectedIndex];
                    Action_Click(this, EventArgs.Empty, selectedItem.Path);
                }
            }
        }

        private void CustomListView_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hitTestInfo = this.HitTest(e.Location);
            var item = items[hitTestInfo.Item.Index];

            if (hitTestInfo.Item != null)
            {
                switch (e.Button) 
                {
                    case MouseButtons.Right:
                        ShowContextMenu(e.Location, item);
                        break;
                    case MouseButtons.Left:
                        Action_Click(sender, e, item.Path);
                        break;
                    default:
                        break;
                }
            }

        }

        public void AddItem(Item item)
        {
            items.Add(item);

            if (items.Count <= 1) FocusAndSelect();
            
            this.VirtualListSize = items.Count;
            this.Refresh();
        }

        public void SortByName(SortOrder sortOrder)
        {
            switch (sortOrder) 
            {
                case SortOrder.Ascending:
                    items = items.OrderBy(i => i.Name).ToList();
                    break;
                case SortOrder.Descending:
                    items = items.OrderByDescending(i => i.Name).ToList();
                    break;
            }

            this.VirtualListSize = items.Count;
            this.Refresh();
        }

        public void SortByVendor(SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    items = items.OrderBy(i => i.Vendor).ToList();
                    break;
                case SortOrder.Descending:
                    items = items.OrderByDescending(i => i.Vendor).ToList();
                    break;
            }

            this.VirtualListSize = items.Count;
            this.Refresh();
        }

        public void SortByFile(SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    items = items.OrderBy(i => Path.GetFileName(i.Path)).ToList();
                    break;
                case SortOrder.Descending:
                    items = items.OrderByDescending(i => Path.GetFileName(i.Path)).ToList();
                    break;
            }

            this.VirtualListSize = items.Count;
            this.Refresh();
        }

        private void ListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex >= 0 && e.ItemIndex < items.Count)
            {
                var item = items[e.ItemIndex];
                e.Item = new ListViewItem(item.Name) { ImageIndex = e.ItemIndex };
            }
        }

        public void Action_Click(object sender, EventArgs e, string path)
        {
            try
            {
                string emu = "emu_launch.bat";
                var process = new ProcessStartInfo()
                {
                    FileName = Path.Combine(Application.StartupPath, emu),
                    Arguments = $"\"{path}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process.Start(process);

            }
            catch (Exception e1)
            {
                MessageBox.Show("Error: " + e1.Message);
            }
        }

    }

}
