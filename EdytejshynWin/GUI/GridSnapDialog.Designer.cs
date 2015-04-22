namespace Edytejshyn.GUI
{
    partial class GridSnapDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.moveSnapTextBox = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.gridStepTextBox = new System.Windows.Forms.TextBox();
            this.scaleSnapTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rotateSnapTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Move snap";
            // 
            // moveSnapTextBox
            // 
            this.moveSnapTextBox.Location = new System.Drawing.Point(99, 10);
            this.moveSnapTextBox.Name = "moveSnapTextBox";
            this.moveSnapTextBox.Size = new System.Drawing.Size(100, 20);
            this.moveSnapTextBox.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(13, 148);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(124, 148);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Grid step";
            // 
            // gridStepTextBox
            // 
            this.gridStepTextBox.Location = new System.Drawing.Point(99, 36);
            this.gridStepTextBox.Name = "gridStepTextBox";
            this.gridStepTextBox.Size = new System.Drawing.Size(100, 20);
            this.gridStepTextBox.TabIndex = 1;
            // 
            // scaleSnapTextBox
            // 
            this.scaleSnapTextBox.Location = new System.Drawing.Point(99, 88);
            this.scaleSnapTextBox.Name = "scaleSnapTextBox";
            this.scaleSnapTextBox.Size = new System.Drawing.Size(100, 20);
            this.scaleSnapTextBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Scale snap";
            // 
            // rotateSnapTextBox
            // 
            this.rotateSnapTextBox.Location = new System.Drawing.Point(99, 62);
            this.rotateSnapTextBox.Name = "rotateSnapTextBox";
            this.rotateSnapTextBox.Size = new System.Drawing.Size(100, 20);
            this.rotateSnapTextBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Rotate snap";
            // 
            // GridSnapDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(220, 183);
            this.Controls.Add(this.scaleSnapTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rotateSnapTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.gridStepTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.moveSnapTextBox);
            this.Controls.Add(this.label1);
            this.Name = "GridSnapDialog";
            this.Text = "Grid & Snapping";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox moveSnapTextBox;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox gridStepTextBox;
        private System.Windows.Forms.TextBox scaleSnapTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox rotateSnapTextBox;
        private System.Windows.Forms.Label label2;
    }
}