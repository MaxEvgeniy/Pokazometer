namespace DocumentMark
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonProcessAll = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFontPath = new System.Windows.Forms.TextBox();
            this.buttonBrowseFont = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonBrowseFolder = new System.Windows.Forms.Button();
            this.textBoxPdfFolderPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonNextFile = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.buttonProcessCurrent = new System.Windows.Forms.Button();
            this.pdfViewerSource = new PdfiumViewer.PdfViewer();
            this.pdfViewerResult = new PdfiumViewer.PdfViewer();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonProcessAll
            // 
            this.buttonProcessAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonProcessAll.Location = new System.Drawing.Point(10, 265);
            this.buttonProcessAll.Name = "buttonProcessAll";
            this.buttonProcessAll.Size = new System.Drawing.Size(236, 23);
            this.buttonProcessAll.TabIndex = 0;
            this.buttonProcessAll.Text = "Обработать все PDF";
            this.buttonProcessAll.UseVisualStyleBackColor = true;
            this.buttonProcessAll.Click += new System.EventHandler(this.buttonProcessAll_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Шрифт";
            // 
            // textBoxFontPath
            // 
            this.textBoxFontPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFontPath.Location = new System.Drawing.Point(6, 93);
            this.textBoxFontPath.Name = "textBoxFontPath";
            this.textBoxFontPath.Size = new System.Drawing.Size(417, 20);
            this.textBoxFontPath.TabIndex = 2;
            this.textBoxFontPath.Text = "C:\\Users\\maksimov\\AppData\\Local\\Microsoft\\Windows\\Fonts\\GOST_2_304-81_TYPE.TTF";
            // 
            // buttonBrowseFont
            // 
            this.buttonBrowseFont.Location = new System.Drawing.Point(444, 93);
            this.buttonBrowseFont.Name = "buttonBrowseFont";
            this.buttonBrowseFont.Size = new System.Drawing.Size(79, 23);
            this.buttonBrowseFont.TabIndex = 3;
            this.buttonBrowseFont.Text = "Обзор";
            this.buttonBrowseFont.UseVisualStyleBackColor = true;
            this.buttonBrowseFont.Click += new System.EventHandler(this.buttonBrowseFont_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 447);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1381, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // textBoxLog
            // 
            this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.textBoxLog.Location = new System.Drawing.Point(12, 12);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(541, 247);
            this.textBoxLog.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.buttonBrowseFolder);
            this.groupBox1.Controls.Add(this.textBoxPdfFolderPath);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.buttonBrowseFont);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxFontPath);
            this.groupBox1.Location = new System.Drawing.Point(10, 323);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(543, 121);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(161, 13);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 5;
            this.textBox1.Text = "08.08.2025";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Дата передачи документов:";
            // 
            // buttonBrowseFolder
            // 
            this.buttonBrowseFolder.Location = new System.Drawing.Point(444, 52);
            this.buttonBrowseFolder.Name = "buttonBrowseFolder";
            this.buttonBrowseFolder.Size = new System.Drawing.Size(79, 23);
            this.buttonBrowseFolder.TabIndex = 2;
            this.buttonBrowseFolder.Text = "Обзор";
            this.buttonBrowseFolder.UseVisualStyleBackColor = true;
            this.buttonBrowseFolder.Click += new System.EventHandler(this.buttonBrowseFolder_Click);
            // 
            // textBoxPdfFolderPath
            // 
            this.textBoxPdfFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPdfFolderPath.Location = new System.Drawing.Point(6, 54);
            this.textBoxPdfFolderPath.Name = "textBoxPdfFolderPath";
            this.textBoxPdfFolderPath.Size = new System.Drawing.Size(417, 20);
            this.textBoxPdfFolderPath.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Путь к папке PDF:";
            // 
            // buttonNextFile
            // 
            this.buttonNextFile.Location = new System.Drawing.Point(3, 0);
            this.buttonNextFile.Name = "buttonNextFile";
            this.buttonNextFile.Size = new System.Drawing.Size(106, 23);
            this.buttonNextFile.TabIndex = 11;
            this.buttonNextFile.Text = "Следующий файл";
            this.buttonNextFile.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(559, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pdfViewerSource);
            this.splitContainer1.Panel1.Controls.Add(this.panelBottom);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pdfViewerResult);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Size = new System.Drawing.Size(810, 432);
            this.splitContainer1.SplitterDistance = 269;
            this.splitContainer1.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Исходный PDF:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Результирующий PDF:";
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.buttonProcessCurrent);
            this.panelBottom.Controls.Add(this.buttonNextFile);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 410);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(269, 22);
            this.panelBottom.TabIndex = 1;
            // 
            // buttonProcessCurrent
            // 
            this.buttonProcessCurrent.Location = new System.Drawing.Point(155, -1);
            this.buttonProcessCurrent.Name = "buttonProcessCurrent";
            this.buttonProcessCurrent.Size = new System.Drawing.Size(111, 23);
            this.buttonProcessCurrent.TabIndex = 12;
            this.buttonProcessCurrent.Text = "Обработать текущий файл";
            this.buttonProcessCurrent.UseVisualStyleBackColor = true;
            // 
            // pdfViewerSource
            // 
            this.pdfViewerSource.Location = new System.Drawing.Point(7, 21);
            this.pdfViewerSource.Name = "pdfViewerSource";
            this.pdfViewerSource.Size = new System.Drawing.Size(259, 383);
            this.pdfViewerSource.TabIndex = 2;
            // 
            // pdfViewerResult
            // 
            this.pdfViewerResult.Location = new System.Drawing.Point(7, 21);
            this.pdfViewerResult.Name = "pdfViewerResult";
            this.pdfViewerResult.Size = new System.Drawing.Size(527, 408);
            this.pdfViewerResult.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1381, 469);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonProcessAll);
            this.Name = "Form1";
            this.Text = "Form1";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonProcessAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFontPath;
        private System.Windows.Forms.Button buttonBrowseFont;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonBrowseFolder;
        private System.Windows.Forms.TextBox textBoxPdfFolderPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonNextFile;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button buttonProcessCurrent;
        private PdfiumViewer.PdfViewer pdfViewerSource;
        private PdfiumViewer.PdfViewer pdfViewerResult;
    }
}

