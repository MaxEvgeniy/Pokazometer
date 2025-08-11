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

        #region Код, автоматически созданный конструктором Windows Form

        /// <summary>
        /// Требуемый метод для поддержки конструктора - не изменяйте
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.pdfViewerSource = new PdfiumViewer.PdfViewer();
            this.label2 = new System.Windows.Forms.Label();
            this.pdfViewerResult = new PdfiumViewer.PdfViewer();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.buttonNextFile = new System.Windows.Forms.Button();
            this.buttonProcessAll = new System.Windows.Forms.Button();
            this.buttonPauseResume = new System.Windows.Forms.Button();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonBrowseFont = new System.Windows.Forms.Button();
            this.textBoxFontPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonBrowseFolder = new System.Windows.Forms.Button();
            this.textBoxPdfFolderPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 488);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(974, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(959, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "Готово.";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 130);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.pdfViewerSource);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.pdfViewerResult);
            this.splitContainer1.Size = new System.Drawing.Size(974, 298);
            this.splitContainer1.SplitterDistance = 484;
            this.splitContainer1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Исходный PDF:";
            // 
            // pdfViewerSource
            // 
            this.pdfViewerSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pdfViewerSource.Location = new System.Drawing.Point(0, 16);
            this.pdfViewerSource.Name = "pdfViewerSource";
            this.pdfViewerSource.Size = new System.Drawing.Size(484, 282);
            this.pdfViewerSource.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Результирующий PDF:";
            // 
            // pdfViewerResult
            // 
            this.pdfViewerResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pdfViewerResult.Location = new System.Drawing.Point(0, 16);
            this.pdfViewerResult.Name = "pdfViewerResult";
            this.pdfViewerResult.Size = new System.Drawing.Size(486, 282);
            this.pdfViewerResult.TabIndex = 0;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.buttonNextFile);
            this.panelBottom.Controls.Add(this.buttonProcessAll);
            this.panelBottom.Controls.Add(this.buttonPauseResume);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 428);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(974, 60);
            this.panelBottom.TabIndex = 2;
            // 
            // buttonNextFile
            // 
            this.buttonNextFile.Location = new System.Drawing.Point(12, 19);
            this.buttonNextFile.Name = "buttonNextFile";
            this.buttonNextFile.Size = new System.Drawing.Size(120, 23);
            this.buttonNextFile.TabIndex = 0;
            this.buttonNextFile.Text = "Обзор пути файла";
            this.buttonNextFile.UseVisualStyleBackColor = true;
            this.buttonNextFile.Click += new System.EventHandler(this.buttonNextFile_Click);
            // 
            // buttonProcessAll
            // 
            this.buttonProcessAll.Location = new System.Drawing.Point(138, 19);
            this.buttonProcessAll.Name = "buttonProcessAll";
            this.buttonProcessAll.Size = new System.Drawing.Size(150, 23);
            this.buttonProcessAll.TabIndex = 1;
            this.buttonProcessAll.Text = "Обработать все";
            this.buttonProcessAll.UseVisualStyleBackColor = true;
            this.buttonProcessAll.Click += new System.EventHandler(this.buttonProcessAll_Click);
            // 
            // buttonPauseResume
            // 
            this.buttonPauseResume.Location = new System.Drawing.Point(294, 19);
            this.buttonPauseResume.Name = "buttonPauseResume";
            this.buttonPauseResume.Size = new System.Drawing.Size(100, 23);
            this.buttonPauseResume.TabIndex = 2;
            this.buttonPauseResume.Text = "Пауза";
            this.buttonPauseResume.UseVisualStyleBackColor = true;
            // 
            // textBoxLog
            // 
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBoxLog.Location = new System.Drawing.Point(0, 510);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(974, 100);
            this.textBoxLog.TabIndex = 3;
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.textBox1);
            this.groupBoxSettings.Controls.Add(this.label5);
            this.groupBoxSettings.Controls.Add(this.buttonBrowseFont);
            this.groupBoxSettings.Controls.Add(this.textBoxFontPath);
            this.groupBoxSettings.Controls.Add(this.label4);
            this.groupBoxSettings.Controls.Add(this.buttonBrowseFolder);
            this.groupBoxSettings.Controls.Add(this.textBoxPdfFolderPath);
            this.groupBoxSettings.Controls.Add(this.label3);
            this.groupBoxSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxSettings.Location = new System.Drawing.Point(0, 0);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(974, 130);
            this.groupBoxSettings.TabIndex = 4;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Настройки";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 99);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(149, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Дата передачи документов:";
            // 
            // buttonBrowseFont
            // 
            this.buttonBrowseFont.Location = new System.Drawing.Point(701, 58);
            this.buttonBrowseFont.Name = "buttonBrowseFont";
            this.buttonBrowseFont.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseFont.TabIndex = 5;
            this.buttonBrowseFont.Text = "...";
            this.buttonBrowseFont.UseVisualStyleBackColor = true;
            this.buttonBrowseFont.Click += new System.EventHandler(this.buttonBrowseFont_Click);
            // 
            // textBoxFontPath
            // 
            this.textBoxFontPath.Location = new System.Drawing.Point(12, 60);
            this.textBoxFontPath.Name = "textBoxFontPath";
            this.textBoxFontPath.Size = new System.Drawing.Size(683, 20);
            this.textBoxFontPath.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Путь к шрифту:";
            // 
            // buttonBrowseFolder
            // 
            this.buttonBrowseFolder.Location = new System.Drawing.Point(701, 16);
            this.buttonBrowseFolder.Name = "buttonBrowseFolder";
            this.buttonBrowseFolder.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseFolder.TabIndex = 2;
            this.buttonBrowseFolder.Text = "Обзор...";
            this.buttonBrowseFolder.UseVisualStyleBackColor = true;
            this.buttonBrowseFolder.Click += new System.EventHandler(this.buttonBrowseFolder_Click);
            // 
            // textBoxPdfFolderPath
            // 
            this.textBoxPdfFolderPath.Location = new System.Drawing.Point(12, 18);
            this.textBoxPdfFolderPath.Name = "textBoxPdfFolderPath";
            this.textBoxPdfFolderPath.Size = new System.Drawing.Size(683, 20);
            this.textBoxPdfFolderPath.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Путь к папке PDF:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(974, 610);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.textBoxLog);
            this.Name = "Form1";
            this.Text = "DocumentMark";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private PdfiumViewer.PdfViewer pdfViewerSource;
        private System.Windows.Forms.Label label2;
        private PdfiumViewer.PdfViewer pdfViewerResult;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button buttonNextFile;
        private System.Windows.Forms.Button buttonProcessAll;
        private System.Windows.Forms.Button buttonPauseResume;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.Button buttonBrowseFont;
        private System.Windows.Forms.TextBox textBoxFontPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonBrowseFolder;
        private System.Windows.Forms.TextBox textBoxPdfFolderPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
    }
}