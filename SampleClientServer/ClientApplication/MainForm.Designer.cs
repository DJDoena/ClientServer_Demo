namespace ClientApplication
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.GetAllIdsButton = new System.Windows.Forms.Button();
            this.IPAddressTextBox = new System.Windows.Forms.TextBox();
            this.ProfileIdListView = new System.Windows.Forms.ListView();
            this.TitleTextBox = new System.Windows.Forms.TextBox();
            this.SetTitleButton = new System.Windows.Forms.Button();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.SetTitleAndOriginalTitleButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GetAllIdsButton
            // 
            this.GetAllIdsButton.Location = new System.Drawing.Point(12, 38);
            this.GetAllIdsButton.Name = "GetAllIdsButton";
            this.GetAllIdsButton.Size = new System.Drawing.Size(288, 23);
            this.GetAllIdsButton.TabIndex = 0;
            this.GetAllIdsButton.Text = "Get All IDs";
            this.GetAllIdsButton.UseVisualStyleBackColor = true;
            this.GetAllIdsButton.Click += new System.EventHandler(this.OnGetAllIdsButtonClick);
            // 
            // IPAddressTextBox
            // 
            this.IPAddressTextBox.Location = new System.Drawing.Point(12, 12);
            this.IPAddressTextBox.Name = "IPAddressTextBox";
            this.IPAddressTextBox.Size = new System.Drawing.Size(288, 20);
            this.IPAddressTextBox.TabIndex = 1;
            this.IPAddressTextBox.Text = "127.0.0.1";
            // 
            // ProfileIdListView
            // 
            this.ProfileIdListView.FullRowSelect = true;
            this.ProfileIdListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.ProfileIdListView.Location = new System.Drawing.Point(12, 67);
            this.ProfileIdListView.MultiSelect = false;
            this.ProfileIdListView.Name = "ProfileIdListView";
            this.ProfileIdListView.Size = new System.Drawing.Size(288, 322);
            this.ProfileIdListView.TabIndex = 2;
            this.ProfileIdListView.UseCompatibleStateImageBehavior = false;
            this.ProfileIdListView.View = System.Windows.Forms.View.List;
            this.ProfileIdListView.SelectedIndexChanged += new System.EventHandler(this.OnProfileIdListViewSelectedIndexChanged);
            // 
            // TitleTextBox
            // 
            this.TitleTextBox.Location = new System.Drawing.Point(306, 69);
            this.TitleTextBox.Name = "TitleTextBox";
            this.TitleTextBox.Size = new System.Drawing.Size(184, 20);
            this.TitleTextBox.TabIndex = 3;
            // 
            // SetTitleButton
            // 
            this.SetTitleButton.Location = new System.Drawing.Point(496, 67);
            this.SetTitleButton.Name = "SetTitleButton";
            this.SetTitleButton.Size = new System.Drawing.Size(75, 23);
            this.SetTitleButton.TabIndex = 4;
            this.SetTitleButton.Text = "Set Title";
            this.SetTitleButton.UseVisualStyleBackColor = true;
            this.SetTitleButton.Click += new System.EventHandler(this.OnSetTitleButtonClick);
            // 
            // SetTitleAndOriginalTitleButton
            // 
            this.SetTitleAndOriginalTitleButton.Location = new System.Drawing.Point(357, 193);
            this.SetTitleAndOriginalTitleButton.Name = "SetTitleAndOriginalTitleButton";
            this.SetTitleAndOriginalTitleButton.Size = new System.Drawing.Size(184, 23);
            this.SetTitleAndOriginalTitleButton.TabIndex = 5;
            this.SetTitleAndOriginalTitleButton.Text = "Set a Title and an Original Title";
            this.SetTitleAndOriginalTitleButton.UseVisualStyleBackColor = true;
            this.SetTitleAndOriginalTitleButton.Click += new System.EventHandler(this.OnSetTitleAndOriginalTitleButtonClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 532);
            this.Controls.Add(this.SetTitleAndOriginalTitleButton);
            this.Controls.Add(this.SetTitleButton);
            this.Controls.Add(this.TitleTextBox);
            this.Controls.Add(this.ProfileIdListView);
            this.Controls.Add(this.IPAddressTextBox);
            this.Controls.Add(this.GetAllIdsButton);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnMainFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GetAllIdsButton;
        private System.Windows.Forms.TextBox IPAddressTextBox;
        private System.Windows.Forms.ListView ProfileIdListView;
        private System.Windows.Forms.TextBox TitleTextBox;
        private System.Windows.Forms.Button SetTitleButton;
        private System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.Button SetTitleAndOriginalTitleButton;
    }
}

