namespace J2ME_Explorer
{
    partial class ContentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContentForm));
            this.contentList = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.contentList)).BeginInit();
            this.SuspendLayout();
            // 
            // contentList
            // 
            this.contentList.BackgroundColor = System.Drawing.SystemColors.Control;
            this.contentList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.contentList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentList.Location = new System.Drawing.Point(0, 0);
            this.contentList.Name = "contentList";
            this.contentList.Size = new System.Drawing.Size(578, 410);
            this.contentList.TabIndex = 0;
            // 
            // ContentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 410);
            this.Controls.Add(this.contentList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ContentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ContentForm";
            ((System.ComponentModel.ISupportInitialize)(this.contentList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView contentList;
    }
}