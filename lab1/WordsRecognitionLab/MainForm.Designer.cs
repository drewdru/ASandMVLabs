namespace WordsRecognition
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
            this.tcRoot = new System.Windows.Forms.TabControl();
            this.lab4 = new System.Windows.Forms.TabPage();
            this.pbSource = new System.Windows.Forms.PictureBox();
            this.llPath = new System.Windows.Forms.LinkLabel();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnResolve = new System.Windows.Forms.Button();
            this.llResult = new System.Windows.Forms.Label();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.ilWordsImagesSource = new System.Windows.Forms.ImageList(this.components);
            this.lvLetters = new System.Windows.Forms.ListView();
            this.tcRoot.SuspendLayout();
            this.lab4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tcRoot
            // 
            this.tcRoot.Controls.Add(this.lab4);
            this.tcRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcRoot.Location = new System.Drawing.Point(0, 0);
            this.tcRoot.Name = "tcRoot";
            this.tcRoot.SelectedIndex = 0;
            this.tcRoot.Size = new System.Drawing.Size(755, 472);
            this.tcRoot.TabIndex = 3;
            // 
            // lab4
            // 
            this.lab4.Controls.Add(this.pbSource);
            this.lab4.Controls.Add(this.llPath);
            this.lab4.Controls.Add(this.btnOpen);
            this.lab4.Controls.Add(this.btnResolve);
            this.lab4.Controls.Add(this.llResult);
            this.lab4.Controls.Add(this.tbResult);
            this.lab4.Controls.Add(this.lvLetters);
            this.lab4.Location = new System.Drawing.Point(4, 22);
            this.lab4.Name = "lab4";
            this.lab4.Padding = new System.Windows.Forms.Padding(3);
            this.lab4.Size = new System.Drawing.Size(747, 446);
            this.lab4.TabIndex = 3;
            this.lab4.Text = "Lab_4";
            this.lab4.UseVisualStyleBackColor = true;
            // 
            // pbSource
            // 
            this.pbSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbSource.Location = new System.Drawing.Point(3, 39);
            this.pbSource.Name = "pbSource";
            this.pbSource.Size = new System.Drawing.Size(383, 325);
            this.pbSource.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSource.TabIndex = 4;
            this.pbSource.TabStop = false;
            // 
            // llPath
            // 
            this.llPath.AutoSize = true;
            this.llPath.Dock = System.Windows.Forms.DockStyle.Top;
            this.llPath.Location = new System.Drawing.Point(3, 26);
            this.llPath.Name = "llPath";
            this.llPath.Size = new System.Drawing.Size(137, 13);
            this.llPath.TabIndex = 6;
            this.llPath.TabStop = true;
            this.llPath.Text = "Path to image will be here...";
            // 
            // btnOpen
            // 
            this.btnOpen.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOpen.Location = new System.Drawing.Point(3, 3);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(383, 23);
            this.btnOpen.TabIndex = 8;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            // 
            // btnResolve
            // 
            this.btnResolve.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnResolve.Location = new System.Drawing.Point(3, 364);
            this.btnResolve.Name = "btnResolve";
            this.btnResolve.Size = new System.Drawing.Size(383, 23);
            this.btnResolve.TabIndex = 5;
            this.btnResolve.Text = "Resolve";
            this.btnResolve.UseVisualStyleBackColor = true;
            // 
            // llResult
            // 
            this.llResult.AutoSize = true;
            this.llResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.llResult.Location = new System.Drawing.Point(3, 387);
            this.llResult.Name = "llResult";
            this.llResult.Size = new System.Drawing.Size(40, 13);
            this.llResult.TabIndex = 9;
            this.llResult.Text = "Result:";
            // 
            // tbResult
            // 
            this.tbResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbResult.Location = new System.Drawing.Point(3, 400);
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.Size = new System.Drawing.Size(383, 43);
            this.tbResult.TabIndex = 7;
            // 
            // ilWordsImagesSource
            // 
            this.ilWordsImagesSource.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilWordsImagesSource.ImageSize = new System.Drawing.Size(16, 16);
            this.ilWordsImagesSource.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lvLetters
            // 
            this.lvLetters.Dock = System.Windows.Forms.DockStyle.Right;
            this.lvLetters.LargeImageList = this.ilWordsImagesSource;
            this.lvLetters.Location = new System.Drawing.Point(386, 3);
            this.lvLetters.Name = "lvLetters";
            this.lvLetters.Size = new System.Drawing.Size(358, 440);
            this.lvLetters.TabIndex = 4;
            this.lvLetters.UseCompatibleStateImageBehavior = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 472);
            this.Controls.Add(this.tcRoot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tcRoot.ResumeLayout(false);
            this.lab4.ResumeLayout(false);
            this.lab4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcRoot;
        private System.Windows.Forms.TabPage lab4;
        private System.Windows.Forms.PictureBox pbSource;
        private System.Windows.Forms.LinkLabel llPath;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnResolve;
        private System.Windows.Forms.Label llResult;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.ListView lvLetters;
        private System.Windows.Forms.ImageList ilWordsImagesSource;
    }
}