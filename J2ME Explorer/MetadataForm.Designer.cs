namespace J2ME_Explorer
{
    partial class MetadataForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetadataForm));
            this.metadataList = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.metadataList)).BeginInit();
            this.SuspendLayout();
            // 
            // metadataList
            // 
            this.metadataList.BackgroundColor = System.Drawing.SystemColors.Control;
            this.metadataList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.metadataList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metadataList.Location = new System.Drawing.Point(0, 0);
            this.metadataList.Name = "metadataList";
            this.metadataList.Size = new System.Drawing.Size(560, 292);
            this.metadataList.TabIndex = 2;
            // 
            // MetadataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 292);
            this.Controls.Add(this.metadataList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MetadataForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MetadataForm";
            ((System.ComponentModel.ISupportInitialize)(this.metadataList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView metadataList;
    }
}