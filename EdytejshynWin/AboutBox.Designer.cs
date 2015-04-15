namespace Edytejshyn
{
    partial class AboutBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.AppPicture = new System.Windows.Forms.PictureBox();
            this.authorsBox = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.auth1 = new System.Windows.Forms.Label();
            this.infoBox = new System.Windows.Forms.GroupBox();
            this.cprtLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.descriptBox = new System.Windows.Forms.GroupBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AppPicture)).BeginInit();
            this.authorsBox.SuspendLayout();
            this.infoBox.SuspendLayout();
            this.descriptBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(9, 9);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.descriptBox);
            this.splitContainer1.Size = new System.Drawing.Size(581, 404);
            this.splitContainer1.SplitterDistance = 238;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.AppPicture);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.authorsBox);
            this.splitContainer2.Panel2.Controls.Add(this.infoBox);
            this.splitContainer2.Size = new System.Drawing.Size(581, 238);
            this.splitContainer2.SplitterDistance = 195;
            this.splitContainer2.TabIndex = 0;
            // 
            // AppPicture
            // 
            this.AppPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.AppPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AppPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AppPicture.Image = ((System.Drawing.Image)(resources.GetObject("AppPicture.Image")));
            this.AppPicture.Location = new System.Drawing.Point(0, 0);
            this.AppPicture.Name = "AppPicture";
            this.AppPicture.Size = new System.Drawing.Size(195, 238);
            this.AppPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.AppPicture.TabIndex = 0;
            this.AppPicture.TabStop = false;
            // 
            // authorsBox
            // 
            this.authorsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.authorsBox.AutoSize = true;
            this.authorsBox.Controls.Add(this.label3);
            this.authorsBox.Controls.Add(this.label2);
            this.authorsBox.Controls.Add(this.auth1);
            this.authorsBox.Location = new System.Drawing.Point(3, 133);
            this.authorsBox.Name = "authorsBox";
            this.authorsBox.Size = new System.Drawing.Size(376, 102);
            this.authorsBox.TabIndex = 1;
            this.authorsBox.TabStop = false;
            this.authorsBox.Text = "Authors";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Patryk Kochański";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(129, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Piotr Marcińczyk";
            // 
            // auth1
            // 
            this.auth1.AutoSize = true;
            this.auth1.Location = new System.Drawing.Point(15, 26);
            this.auth1.Name = "auth1";
            this.auth1.Size = new System.Drawing.Size(93, 13);
            this.auth1.TabIndex = 0;
            this.auth1.Text = "Krzysztof Szczech";
            // 
            // infoBox
            // 
            this.infoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoBox.AutoSize = true;
            this.infoBox.Controls.Add(this.cprtLabel);
            this.infoBox.Controls.Add(this.versionLabel);
            this.infoBox.Location = new System.Drawing.Point(3, 3);
            this.infoBox.Name = "infoBox";
            this.infoBox.Size = new System.Drawing.Size(376, 149);
            this.infoBox.TabIndex = 0;
            this.infoBox.TabStop = false;
            this.infoBox.Text = "General Info";
            // 
            // cprtLabel
            // 
            this.cprtLabel.AutoSize = true;
            this.cprtLabel.Location = new System.Drawing.Point(20, 52);
            this.cprtLabel.Name = "cprtLabel";
            this.cprtLabel.Size = new System.Drawing.Size(145, 13);
            this.cprtLabel.TabIndex = 1;
            this.cprtLabel.Text = "Copyrights: All rights reserved";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(20, 29);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(117, 13);
            this.versionLabel.TabIndex = 0;
            this.versionLabel.Text = "Application version: 1.0";
            // 
            // descriptBox
            // 
            this.descriptBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptBox.AutoSize = true;
            this.descriptBox.Controls.Add(this.descriptionLabel);
            this.descriptBox.Location = new System.Drawing.Point(3, 3);
            this.descriptBox.Name = "descriptBox";
            this.descriptBox.Size = new System.Drawing.Size(578, 155);
            this.descriptBox.TabIndex = 0;
            this.descriptBox.TabStop = false;
            this.descriptBox.Text = "Description";
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descriptionLabel.Location = new System.Drawing.Point(3, 16);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(570, 78);
            this.descriptionLabel.TabIndex = 0;
            this.descriptionLabel.Text = resources.GetString("descriptionLabel.Text");
            // 
            // AboutBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 422);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edytejszyn Info";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AppPicture)).EndInit();
            this.authorsBox.ResumeLayout(false);
            this.authorsBox.PerformLayout();
            this.infoBox.ResumeLayout(false);
            this.infoBox.PerformLayout();
            this.descriptBox.ResumeLayout(false);
            this.descriptBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PictureBox AppPicture;
        private System.Windows.Forms.GroupBox authorsBox;
        private System.Windows.Forms.GroupBox infoBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label auth1;
        private System.Windows.Forms.Label cprtLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.GroupBox descriptBox;
        private System.Windows.Forms.Label descriptionLabel;

    }
}
