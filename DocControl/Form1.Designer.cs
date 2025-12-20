using System.Windows.Forms;

namespace DocControl
{
    partial class Form1 : Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabGenerate = new TabPage();
            lblGenerateResult = new Label();
            btnGenerate = new Button();
            txtExtension = new TextBox();
            label7 = new Label();
            txtFreeText = new TextBox();
            label6 = new Label();
            chkEnableLevel4 = new CheckBox();
            txtLevel4 = new TextBox();
            label5 = new Label();
            txtLevel3 = new TextBox();
            label4 = new Label();
            txtLevel2 = new TextBox();
            label3 = new Label();
            txtLevel1 = new TextBox();
            label2 = new Label();
            tabImport = new TabPage();
            lblImportResult = new Label();
            btnImport = new Button();
            btnBrowseImport = new Button();
            txtImportFolder = new TextBox();
            label8 = new Label();
            lstImportInvalid = new ListBox();
            btnImportCsv = new Button();
            btnBrowseCsv = new Button();
            txtImportCsv = new TextBox();
            label16 = new Label();
            lvImportSummary = new ListView();
            colSeries = new ColumnHeader();
            colMaxNum = new ColumnHeader();
            colNextNum = new ColumnHeader();
            tabRecommend = new TabPage();
            txtNlqResult = new TextBox();
            btnInterpret = new Button();
            btnRecommend = new Button();
            lblRecommendResult = new Label();
            txtNlq = new TextBox();
            label9 = new Label();
            btnUseSuggested = new Button();
            btnCreateRecommended = new Button();
            lblRecommendInfo = new Label();
            tabSettings = new TabPage();
            btnSaveSettings = new Button();
            txtGeminiKey = new TextBox();
            label15 = new Label();
            txtOpenAiKey = new TextBox();
            label14 = new Label();
            txtGeminiModel = new TextBox();
            label13 = new Label();
            txtOpenAiModel = new TextBox();
            label12 = new Label();
            cmbProvider = new ComboBox();
            label11 = new Label();
            numPadding = new NumericUpDown();
            label10 = new Label();
            txtSeparator = new TextBox();
            label1 = new Label();
            chkSettingsEnableLevel4 = new CheckBox();
            tabAudit = new TabPage();
            btnAuditRefresh = new Button();
            lvAudit = new ListView();
            colAction = new ColumnHeader();
            colBy = new ColumnHeader();
            colAt = new ColumnHeader();
            colPayload = new ColumnHeader();
            colDocId = new ColumnHeader();
            btnAuditFilter = new Button();
            txtAuditAction = new TextBox();
            txtAuditUser = new TextBox();
            labelAuditAction = new Label();
            labelAuditUser = new Label();
            chkSeedCounters = new CheckBox();
            btnAuditPrev = new Button();
            btnAuditNext = new Button();
            lblAuditPage = new Label();
            lblNoDelete = new Label();
            lblImportNote = new Label();
            btnSeedSelected = new Button();
            tabDocs = new TabPage();
            btnDocsRefresh = new Button();
            lvDocs = new ListView();
            colDocCode = new ColumnHeader();
            colDocFile = new ColumnHeader();
            colDocBy = new ColumnHeader();
            colDocAt = new ColumnHeader();
            lblImportPerSeries = new Label();
            tabControl1.SuspendLayout();
            tabGenerate.SuspendLayout();
            tabImport.SuspendLayout();
            tabRecommend.SuspendLayout();
            tabSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numPadding).BeginInit();
            tabAudit.SuspendLayout();
            tabDocs.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabGenerate);
            tabControl1.Controls.Add(tabImport);
            tabControl1.Controls.Add(tabRecommend);
            tabControl1.Controls.Add(tabSettings);
            tabControl1.Controls.Add(tabAudit);
            tabControl1.Controls.Add(tabDocs);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(784, 461);
            tabControl1.TabIndex = 0;
            // 
            // tabGenerate
            // 
            tabGenerate.Controls.Add(lblGenerateResult);
            tabGenerate.Controls.Add(btnGenerate);
            tabGenerate.Controls.Add(txtExtension);
            tabGenerate.Controls.Add(label7);
            tabGenerate.Controls.Add(txtFreeText);
            tabGenerate.Controls.Add(label6);
            tabGenerate.Controls.Add(chkEnableLevel4);
            tabGenerate.Controls.Add(txtLevel4);
            tabGenerate.Controls.Add(label5);
            tabGenerate.Controls.Add(txtLevel3);
            tabGenerate.Controls.Add(label4);
            tabGenerate.Controls.Add(txtLevel2);
            tabGenerate.Controls.Add(label3);
            tabGenerate.Controls.Add(txtLevel1);
            tabGenerate.Controls.Add(label2);
            tabGenerate.Controls.Add(lblNoDelete);
            tabGenerate.Location = new Point(4, 24);
            tabGenerate.Name = "tabGenerate";
            tabGenerate.Padding = new Padding(3);
            tabGenerate.Size = new Size(776, 433);
            tabGenerate.TabIndex = 0;
            tabGenerate.Text = "Generate";
            tabGenerate.UseVisualStyleBackColor = true;
            // 
            // lblGenerateResult
            // 
            lblGenerateResult.AutoSize = true;
            lblGenerateResult.Location = new Point(24, 278);
            lblGenerateResult.Name = "lblGenerateResult";
            lblGenerateResult.Size = new Size(0, 15);
            lblGenerateResult.TabIndex = 14;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(24, 239);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(135, 27);
            btnGenerate.TabIndex = 13;
            btnGenerate.Text = "Generate";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // txtExtension
            // 
            txtExtension.Location = new Point(104, 196);
            txtExtension.Name = "txtExtension";
            txtExtension.Size = new Size(200, 23);
            txtExtension.TabIndex = 12;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(24, 199);
            label7.Name = "label7";
            label7.Size = new Size(60, 15);
            label7.TabIndex = 11;
            label7.Text = "Extension";
            // 
            // txtFreeText
            // 
            txtFreeText.Location = new Point(104, 160);
            txtFreeText.Name = "txtFreeText";
            txtFreeText.Size = new Size(400, 23);
            txtFreeText.TabIndex = 10;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(24, 163);
            label6.Name = "label6";
            label6.Size = new Size(55, 15);
            label6.TabIndex = 9;
            label6.Text = "Free Text";
            // 
            // chkEnableLevel4
            // 
            chkEnableLevel4.AutoSize = true;
            chkEnableLevel4.Location = new Point(24, 129);
            chkEnableLevel4.Name = "chkEnableLevel4";
            chkEnableLevel4.Size = new Size(101, 19);
            chkEnableLevel4.TabIndex = 8;
            chkEnableLevel4.Text = "Enable Level4";
            chkEnableLevel4.UseVisualStyleBackColor = true;
            chkEnableLevel4.CheckedChanged += chkEnableLevel4_CheckedChanged;
            // 
            // txtLevel4
            // 
            txtLevel4.Location = new Point(352, 96);
            txtLevel4.Name = "txtLevel4";
            txtLevel4.Size = new Size(152, 23);
            txtLevel4.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(296, 99);
            label5.Name = "label5";
            label5.Size = new Size(44, 15);
            label5.TabIndex = 6;
            label5.Text = "Level4";
            // 
            // txtLevel3
            // 
            txtLevel3.Location = new Point(104, 96);
            txtLevel3.Name = "txtLevel3";
            txtLevel3.Size = new Size(152, 23);
            txtLevel3.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(24, 99);
            label4.Name = "label4";
            label4.Size = new Size(44, 15);
            label4.TabIndex = 4;
            label4.Text = "Level3";
            // 
            // txtLevel2
            // 
            txtLevel2.Location = new Point(352, 57);
            txtLevel2.Name = "txtLevel2";
            txtLevel2.Size = new Size(152, 23);
            txtLevel2.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(296, 60);
            label3.Name = "label3";
            label3.Size = new Size(44, 15);
            label3.TabIndex = 2;
            label3.Text = "Level2";
            // 
            // txtLevel1
            // 
            txtLevel1.Location = new Point(104, 57);
            txtLevel1.Name = "txtLevel1";
            txtLevel1.Size = new Size(152, 23);
            txtLevel1.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(24, 60);
            label2.Name = "label2";
            label2.Size = new Size(44, 15);
            label2.TabIndex = 0;
            label2.Text = "Level1";
            // 
            // lblNoDelete
            // 
            lblNoDelete.AutoSize = true;
            lblNoDelete.ForeColor = Color.Red;
            lblNoDelete.Location = new Point(24, 310);
            lblNoDelete.Name = "lblNoDelete";
            lblNoDelete.Size = new Size(210, 15);
            lblNoDelete.TabIndex = 15;
            lblNoDelete.Text = "No deletions allowed. All actions audited.";
            // 
            // tabImport
            // 
            tabImport.Controls.Add(lblImportResult);
            tabImport.Controls.Add(btnImport);
            tabImport.Controls.Add(btnBrowseImport);
            tabImport.Controls.Add(txtImportFolder);
            tabImport.Controls.Add(label8);
            tabImport.Controls.Add(lstImportInvalid);
            tabImport.Controls.Add(btnImportCsv);
            tabImport.Controls.Add(btnBrowseCsv);
            tabImport.Controls.Add(txtImportCsv);
            tabImport.Controls.Add(label16);
            tabImport.Controls.Add(lvImportSummary);
            tabImport.Controls.Add(lblImportNote);
            tabImport.Controls.Add(chkSeedCounters);
            tabImport.Controls.Add(btnSeedSelected);
            tabImport.Controls.Add(lblImportPerSeries);
            tabImport.Location = new Point(4, 24);
            tabImport.Name = "tabImport";
            tabImport.Padding = new Padding(3);
            tabImport.Size = new Size(776, 433);
            tabImport.TabIndex = 1;
            tabImport.Text = "Import";
            tabImport.UseVisualStyleBackColor = true;
            // 
            // lblImportResult
            // 
            lblImportResult.AutoSize = true;
            lblImportResult.Location = new Point(24, 70);
            lblImportResult.Name = "lblImportResult";
            lblImportResult.Size = new Size(0, 15);
            lblImportResult.TabIndex = 4;
            // 
            // btnImport
            // 
            btnImport.Location = new Point(24, 34);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(135, 27);
            btnImport.TabIndex = 3;
            btnImport.Text = "Import";
            btnImport.UseVisualStyleBackColor = true;
            btnImport.Click += btnImport_Click;
            // 
            // btnBrowseImport
            // 
            btnBrowseImport.Location = new Point(666, 22);
            btnBrowseImport.Name = "btnBrowseImport";
            btnBrowseImport.Size = new Size(75, 23);
            btnBrowseImport.TabIndex = 2;
            btnBrowseImport.Text = "Browse";
            btnBrowseImport.UseVisualStyleBackColor = true;
            btnBrowseImport.Click += btnBrowseImport_Click;
            // 
            // txtImportFolder
            // 
            txtImportFolder.Location = new Point(104, 22);
            txtImportFolder.Name = "txtImportFolder";
            txtImportFolder.Size = new Size(556, 23);
            txtImportFolder.TabIndex = 1;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(24, 25);
            label8.Name = "label8";
            label8.Size = new Size(69, 15);
            label8.TabIndex = 0;
            label8.Text = "Import path";
            // 
            // lstImportInvalid
            // 
            lstImportInvalid.FormattingEnabled = true;
            lstImportInvalid.HorizontalScrollbar = true;
            lstImportInvalid.ItemHeight = 15;
            lstImportInvalid.Location = new Point(24, 134);
            lstImportInvalid.Name = "lstImportInvalid";
            lstImportInvalid.Size = new Size(717, 58);
            lstImportInvalid.TabIndex = 5;
            // 
            // btnImportCsv
            // 
            btnImportCsv.Location = new Point(24, 99);
            btnImportCsv.Name = "btnImportCsv";
            btnImportCsv.Size = new Size(135, 27);
            btnImportCsv.TabIndex = 9;
            btnImportCsv.Text = "Import CSV";
            btnImportCsv.UseVisualStyleBackColor = true;
            btnImportCsv.Click += btnImportCsv_Click;
            // 
            // btnBrowseCsv
            // 
            btnBrowseCsv.Location = new Point(666, 99);
            btnBrowseCsv.Name = "btnBrowseCsv";
            btnBrowseCsv.Size = new Size(75, 23);
            btnBrowseCsv.TabIndex = 8;
            btnBrowseCsv.Text = "Browse";
            btnBrowseCsv.UseVisualStyleBackColor = true;
            btnBrowseCsv.Click += btnBrowseCsv_Click;
            // 
            // txtImportCsv
            // 
            txtImportCsv.Location = new Point(124, 99);
            txtImportCsv.Name = "txtImportCsv";
            txtImportCsv.Size = new Size(536, 23);
            txtImportCsv.TabIndex = 7;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(24, 102);
            label16.Name = "label16";
            label16.Size = new Size(93, 15);
            label16.TabIndex = 6;
            label16.Text = "CSV file (names)";
            // 
            // lvImportSummary
            // 
            lvImportSummary.Columns.AddRange(new ColumnHeader[] { colSeries, colMaxNum, colNextNum });
            lvImportSummary.FullRowSelect = true;
            lvImportSummary.GridLines = true;
            lvImportSummary.Location = new Point(24, 200);
            lvImportSummary.Name = "lvImportSummary";
            lvImportSummary.Size = new Size(717, 163);
            lvImportSummary.TabIndex = 10;
            lvImportSummary.UseCompatibleStateImageBehavior = false;
            lvImportSummary.View = View.Details;
            colSeries.Text = "Series";
            colSeries.Width = 420;
            colMaxNum.Text = "Max";
            colMaxNum.Width = 80;
            colNextNum.Text = "Next";
            colNextNum.Width = 80;
            // 
            // lblImportNote
            // 
            lblImportNote.AutoSize = true;
            lblImportNote.Location = new Point(180, 69);
            lblImportNote.Name = "lblImportNote";
            lblImportNote.Size = new Size(0, 15);
            lblImportNote.TabIndex = 12;
            // 
            // lblImportPerSeries
            // 
            lblImportPerSeries.AutoSize = true;
            lblImportPerSeries.Location = new Point(24, 184);
            lblImportPerSeries.Name = "lblImportPerSeries";
            lblImportPerSeries.Size = new Size(260, 15);
            lblImportPerSeries.TabIndex = 14;
            lblImportPerSeries.Text = "Select series below and click 'Seed selected series'.";
            // 
            // tabRecommend
            // 
            tabRecommend.Controls.Add(lblRecommendResult);
            tabRecommend.Controls.Add(btnRecommend);
            tabRecommend.Controls.Add(txtNlqResult);
            tabRecommend.Controls.Add(btnInterpret);
            tabRecommend.Controls.Add(txtNlq);
            tabRecommend.Controls.Add(label9);
            tabRecommend.Controls.Add(btnUseSuggested);
            tabRecommend.Controls.Add(btnCreateRecommended);
            tabRecommend.Controls.Add(lblRecommendInfo);
            tabRecommend.Location = new Point(4, 24);
            tabRecommend.Name = "tabRecommend";
            tabRecommend.Padding = new Padding(3);
            tabRecommend.Size = new Size(776, 433);
            tabRecommend.TabIndex = 2;
            tabRecommend.Text = "Recommend";
            tabRecommend.UseVisualStyleBackColor = true;
            // 
            // txtNlqResult
            // 
            txtNlqResult.Location = new Point(24, 96);
            txtNlqResult.Multiline = true;
            txtNlqResult.Name = "txtNlqResult";
            txtNlqResult.ReadOnly = true;
            txtNlqResult.ScrollBars = ScrollBars.Vertical;
            txtNlqResult.Size = new Size(717, 311);
            txtNlqResult.TabIndex = 3;
            // 
            // btnInterpret
            // 
            btnInterpret.Location = new Point(666, 54);
            btnInterpret.Name = "btnInterpret";
            btnInterpret.Size = new Size(75, 27);
            btnInterpret.TabIndex = 2;
            btnInterpret.Text = "Interpret";
            btnInterpret.UseVisualStyleBackColor = true;
            btnInterpret.Click += btnInterpret_Click;
            // 
            // btnRecommend
            // 
            btnRecommend.Location = new Point(570, 20);
            btnRecommend.Name = "btnRecommend";
            btnRecommend.Size = new Size(171, 27);
            btnRecommend.TabIndex = 4;
            btnRecommend.Text = "Recommend from levels";
            btnRecommend.UseVisualStyleBackColor = true;
            btnRecommend.Click += btnRecommend_Click;
            // 
            // lblRecommendResult
            // 
            lblRecommendResult.AutoSize = true;
            lblRecommendResult.Location = new Point(24, 76);
            lblRecommendResult.Name = "lblRecommendResult";
            lblRecommendResult.Size = new Size(0, 15);
            lblRecommendResult.TabIndex = 5;
            // 
            // btnUseSuggested
            // 
            btnUseSuggested.Location = new Point(393, 20);
            btnUseSuggested.Name = "btnUseSuggested";
            btnUseSuggested.Size = new Size(171, 27);
            btnUseSuggested.TabIndex = 6;
            btnUseSuggested.Text = "Use suggested (confirm)";
            btnUseSuggested.UseVisualStyleBackColor = true;
            btnUseSuggested.Click += btnUseSuggested_Click;
            // 
            // btnCreateRecommended
            // 
            btnCreateRecommended.Location = new Point(216, 20);
            btnCreateRecommended.Name = "btnCreateRecommended";
            btnCreateRecommended.Size = new Size(171, 27);
            btnCreateRecommended.TabIndex = 7;
            btnCreateRecommended.Text = "Create (allocator)";
            btnCreateRecommended.UseVisualStyleBackColor = true;
            btnCreateRecommended.Click += btnCreateRecommended_Click;
            // 
            // lblRecommendInfo
            // 
            lblRecommendInfo.AutoSize = true;
            lblRecommendInfo.Location = new Point(24, 54);
            lblRecommendInfo.Name = "lblRecommendInfo";
            lblRecommendInfo.Size = new Size(379, 15);
            lblRecommendInfo.TabIndex = 8;
            lblRecommendInfo.Text = "Allocator always picks next available number; suggested is advisory guidance.";
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(btnSaveSettings);
            tabSettings.Controls.Add(txtGeminiKey);
            tabSettings.Controls.Add(label15);
            tabSettings.Controls.Add(txtOpenAiKey);
            tabSettings.Controls.Add(label14);
            tabSettings.Controls.Add(txtGeminiModel);
            tabSettings.Controls.Add(label13);
            tabSettings.Controls.Add(txtOpenAiModel);
            tabSettings.Controls.Add(label12);
            tabSettings.Controls.Add(cmbProvider);
            tabSettings.Controls.Add(label11);
            tabSettings.Controls.Add(numPadding);
            tabSettings.Controls.Add(label10);
            tabSettings.Controls.Add(txtSeparator);
            tabSettings.Controls.Add(label1);
            tabSettings.Controls.Add(chkSettingsEnableLevel4);
            tabSettings.Location = new Point(4, 24);
            tabSettings.Name = "tabSettings";
            tabSettings.Padding = new Padding(3);
            tabSettings.Size = new Size(776, 433);
            tabSettings.TabIndex = 3;
            tabSettings.Text = "Settings";
            tabSettings.UseVisualStyleBackColor = true;
            // 
            // btnSaveSettings
            // 
            btnSaveSettings.Location = new Point(24, 273);
            btnSaveSettings.Name = "btnSaveSettings";
            btnSaveSettings.Size = new Size(135, 27);
            btnSaveSettings.TabIndex = 15;
            btnSaveSettings.Text = "Save";
            btnSaveSettings.UseVisualStyleBackColor = true;
            btnSaveSettings.Click += btnSaveSettings_Click;
            // 
            // txtGeminiKey
            // 
            txtGeminiKey.Location = new Point(120, 232);
            txtGeminiKey.Name = "txtGeminiKey";
            txtGeminiKey.PasswordChar = '*';
            txtGeminiKey.Size = new Size(308, 23);
            txtGeminiKey.TabIndex = 14;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(24, 235);
            label15.Name = "label15";
            label15.Size = new Size(69, 15);
            label15.TabIndex = 13;
            label15.Text = "Gemini Key";
            // 
            // txtOpenAiKey
            // 
            txtOpenAiKey.Location = new Point(120, 195);
            txtOpenAiKey.Name = "txtOpenAiKey";
            txtOpenAiKey.PasswordChar = '*';
            txtOpenAiKey.Size = new Size(308, 23);
            txtOpenAiKey.TabIndex = 12;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(24, 198);
            label14.Name = "label14";
            label14.Size = new Size(76, 15);
            label14.TabIndex = 11;
            label14.Text = "OpenAI Key";
            // 
            // txtGeminiModel
            // 
            txtGeminiModel.Location = new Point(120, 158);
            txtGeminiModel.Name = "txtGeminiModel";
            txtGeminiModel.Size = new Size(200, 23);
            txtGeminiModel.TabIndex = 10;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(24, 161);
            label13.Name = "label13";
            label13.Size = new Size(79, 15);
            label13.TabIndex = 9;
            label13.Text = "Gemini Model";
            // 
            // txtOpenAiModel
            // 
            txtOpenAiModel.Location = new Point(120, 121);
            txtOpenAiModel.Name = "txtOpenAiModel";
            txtOpenAiModel.Size = new Size(200, 23);
            txtOpenAiModel.TabIndex = 8;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(24, 124);
            label12.Name = "label12";
            label12.Size = new Size(83, 15);
            label12.TabIndex = 7;
            label12.Text = "OpenAI Model";
            // 
            // cmbProvider
            // 
            cmbProvider.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProvider.FormattingEnabled = true;
            cmbProvider.Location = new Point(120, 84);
            cmbProvider.Name = "cmbProvider";
            cmbProvider.Size = new Size(151, 23);
            cmbProvider.TabIndex = 6;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(24, 87);
            label11.Name = "label11";
            label11.Size = new Size(54, 15);
            label11.TabIndex = 5;
            label11.Text = "Provider";
            // 
            // numPadding
            // 
            numPadding.Location = new Point(120, 48);
            numPadding.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numPadding.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numPadding.Name = "numPadding";
            numPadding.Size = new Size(60, 23);
            numPadding.TabIndex = 4;
            numPadding.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(24, 50);
            label10.Name = "label10";
            label10.Size = new Size(49, 15);
            label10.TabIndex = 3;
            label10.Text = "Padding";
            // 
            // txtSeparator
            // 
            txtSeparator.Location = new Point(120, 12);
            txtSeparator.Name = "txtSeparator";
            txtSeparator.Size = new Size(60, 23);
            txtSeparator.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(24, 15);
            label1.Name = "label1";
            label1.Size = new Size(60, 15);
            label1.TabIndex = 1;
            label1.Text = "Separator";
            // 
            // chkSettingsEnableLevel4
            // 
            chkSettingsEnableLevel4.AutoSize = true;
            chkSettingsEnableLevel4.Location = new Point(24, 50);
            chkSettingsEnableLevel4.Name = "chkSettingsEnableLevel4";
            chkSettingsEnableLevel4.Size = new Size(101, 19);
            chkSettingsEnableLevel4.TabIndex = 0;
            chkSettingsEnableLevel4.Text = "Enable Level4";
            chkSettingsEnableLevel4.UseVisualStyleBackColor = true;
            chkSettingsEnableLevel4.CheckedChanged += chkSettingsEnableLevel4_CheckedChanged;
            // 
            // tabAudit
            // 
            tabAudit.Controls.Add(labelAuditUser);
            tabAudit.Controls.Add(labelAuditAction);
            tabAudit.Controls.Add(txtAuditUser);
            tabAudit.Controls.Add(txtAuditAction);
            tabAudit.Controls.Add(btnAuditFilter);
            tabAudit.Controls.Add(lvAudit);
            tabAudit.Controls.Add(btnAuditRefresh);
            tabAudit.Controls.Add(lblAuditPage);
            tabAudit.Controls.Add(btnAuditNext);
            tabAudit.Controls.Add(btnAuditPrev);
            tabAudit.Location = new Point(4, 24);
            tabAudit.Name = "tabAudit";
            tabAudit.Padding = new Padding(3);
            tabAudit.Size = new Size(776, 433);
            tabAudit.TabIndex = 4;
            tabAudit.Text = "Audit";
            tabAudit.UseVisualStyleBackColor = true;
            // 
            // lvAudit
            // 
            lvAudit.Columns.AddRange(new ColumnHeader[] { colAt, colBy, colAction, colPayload, colDocId });
            lvAudit.FullRowSelect = true;
            lvAudit.GridLines = true;
            lvAudit.Location = new Point(24, 58);
            lvAudit.Name = "lvAudit";
            lvAudit.Size = new Size(727, 351);
            lvAudit.TabIndex = 1;
            lvAudit.UseCompatibleStateImageBehavior = false;
            lvAudit.View = View.Details;
            lvAudit.DoubleClick += lvAudit_DoubleClick;
            colAt.Text = "At";
            colAt.Width = 160;
            colBy.Text = "By";
            colBy.Width = 100;
            colAction.Text = "Action";
            colAction.Width = 120;
            colPayload.Text = "Payload";
            colPayload.Width = 250;
            colDocId.Text = "DocId";
            colDocId.Width = 80;
            // 
            // btnAuditRefresh
            // 
            btnAuditRefresh.Location = new Point(24, 20);
            btnAuditRefresh.Name = "btnAuditRefresh";
            btnAuditRefresh.Size = new Size(135, 27);
            btnAuditRefresh.TabIndex = 0;
            btnAuditRefresh.Text = "Refresh";
            btnAuditRefresh.UseVisualStyleBackColor = true;
            btnAuditRefresh.Click += btnAuditRefresh_Click;
            // 
            // btnAuditFilter
            // 
            btnAuditFilter.Location = new Point(560, 20);
            btnAuditFilter.Name = "btnAuditFilter";
            btnAuditFilter.Size = new Size(135, 27);
            btnAuditFilter.TabIndex = 3;
            btnAuditFilter.Text = "Filter";
            btnAuditFilter.UseVisualStyleBackColor = true;
            btnAuditFilter.Click += btnAuditFilter_Click;
            // 
            // btnAuditPrev
            // 
            btnAuditPrev.Location = new Point(520, 20);
            btnAuditPrev.Name = "btnAuditPrev";
            btnAuditPrev.Size = new Size(30, 27);
            btnAuditPrev.TabIndex = 8;
            btnAuditPrev.Text = "<";
            btnAuditPrev.UseVisualStyleBackColor = true;
            btnAuditPrev.Click += btnAuditPrev_Click;
            // 
            // btnAuditNext
            // 
            btnAuditNext.Location = new Point(700, 20);
            btnAuditNext.Name = "btnAuditNext";
            btnAuditNext.Size = new Size(30, 27);
            btnAuditNext.TabIndex = 9;
            btnAuditNext.Text = ">";
            btnAuditNext.UseVisualStyleBackColor = true;
            btnAuditNext.Click += btnAuditNext_Click;
            // 
            // lblAuditPage
            // 
            lblAuditPage.AutoSize = true;
            lblAuditPage.Location = new Point(560, 25);
            lblAuditPage.Name = "lblAuditPage";
            lblAuditPage.Size = new Size(39, 15);
            lblAuditPage.TabIndex = 10;
            lblAuditPage.Text = "1 / ?";
            // 
            // labelAuditAction
            // 
            labelAuditAction.AutoSize = true;
            labelAuditAction.Location = new Point(180, 25);
            labelAuditAction.Name = "labelAuditAction";
            labelAuditAction.Size = new Size(44, 15);
            labelAuditAction.TabIndex = 4;
            labelAuditAction.Text = "Action";
            // 
            // txtAuditAction
            // 
            txtAuditAction.Location = new Point(230, 22);
            txtAuditAction.Name = "txtAuditAction";
            txtAuditAction.Size = new Size(120, 23);
            txtAuditAction.TabIndex = 5;
            // 
            // labelAuditUser
            // 
            labelAuditUser.AutoSize = true;
            labelAuditUser.Location = new Point(370, 25);
            labelAuditUser.Name = "labelAuditUser";
            labelAuditUser.Size = new Size(31, 15);
            labelAuditUser.TabIndex = 6;
            labelAuditUser.Text = "User";
            // 
            // txtAuditUser
            // 
            txtAuditUser.Location = new Point(410, 22);
            txtAuditUser.Name = "txtAuditUser";
            txtAuditUser.Size = new Size(120, 23);
            txtAuditUser.TabIndex = 7;
            // 
            // tabDocs
            // 
            tabDocs.Controls.Add(lvDocs);
            tabDocs.Controls.Add(btnDocsRefresh);
            tabDocs.Location = new Point(4, 24);
            tabDocs.Name = "tabDocs";
            tabDocs.Padding = new Padding(3);
            tabDocs.Size = new Size(776, 433);
            tabDocs.TabIndex = 5;
            tabDocs.Text = "Documents";
            tabDocs.UseVisualStyleBackColor = true;
            // 
            // btnDocsRefresh
            // 
            btnDocsRefresh.Location = new Point(24, 20);
            btnDocsRefresh.Name = "btnDocsRefresh";
            btnDocsRefresh.Size = new Size(135, 27);
            btnDocsRefresh.TabIndex = 0;
            btnDocsRefresh.Text = "Refresh";
            btnDocsRefresh.UseVisualStyleBackColor = true;
            btnDocsRefresh.Click += btnDocsRefresh_Click;
            // 
            // lvDocs
            // 
            lvDocs.Columns.AddRange(new ColumnHeader[] { colDocCode, colDocFile, colDocBy, colDocAt });
            lvDocs.FullRowSelect = true;
            lvDocs.GridLines = true;
            lvDocs.Location = new Point(24, 58);
            lvDocs.Name = "lvDocs";
            lvDocs.Size = new Size(727, 351);
            lvDocs.TabIndex = 1;
            lvDocs.UseCompatibleStateImageBehavior = false;
            lvDocs.View = View.Details;
            lvDocs.DoubleClick += lvDocs_DoubleClick;
            colDocCode.Text = "Code";
            colDocCode.Width = 200;
            colDocFile.Text = "File";
            colDocFile.Width = 250;
            colDocBy.Text = "By";
            colDocBy.Width = 80;
            colDocAt.Text = "At";
            colDocAt.Width = 160;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 461);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "DocControl";
            tabControl1.ResumeLayout(false);
            tabGenerate.ResumeLayout(false);
            tabGenerate.PerformLayout();
            tabImport.ResumeLayout(false);
            tabImport.PerformLayout();
            tabRecommend.ResumeLayout(false);
            tabRecommend.PerformLayout();
            tabSettings.ResumeLayout(false);
            tabSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numPadding).EndInit();
            tabAudit.ResumeLayout(false);
            tabAudit.PerformLayout();
            tabDocs.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabGenerate;
        private TabPage tabImport;
        private TabPage tabRecommend;
        private TabPage tabSettings;
        private Label label2;
        private TextBox txtLevel1;
        private TextBox txtLevel3;
        private Label label4;
        private TextBox txtLevel2;
        private Label label3;
        private TextBox txtLevel4;
        private Label label5;
        private CheckBox chkEnableLevel4;
        private TextBox txtFreeText;
        private Label label6;
        private Button btnGenerate;
        private TextBox txtExtension;
        private Label label7;
        private Label lblGenerateResult;
        private Button btnBrowseImport;
        private TextBox txtImportFolder;
        private Label label8;
        private Label lblImportResult;
        private Button btnImport;
        private TextBox txtNlq;
        private Label label9;
        private TextBox txtNlqResult;
        private Button btnInterpret;
        private Button btnRecommend;
        private Label lblRecommendResult;
        private TextBox txtSeparator;
        private Label label1;
        private NumericUpDown numPadding;
        private Label label10;
        private ComboBox cmbProvider;
        private Label label11;
        private TextBox txtGeminiModel;
        private Label label13;
        private TextBox txtOpenAiModel;
        private Label label12;
        private Button btnSaveSettings;
        private TextBox txtGeminiKey;
        private Label label15;
        private TextBox txtOpenAiKey;
        private Label label14;
        private CheckBox chkSettingsEnableLevel4;
        private TabPage tabAudit;
        private Button btnAuditRefresh;
        private ListView lvAudit;
        private ColumnHeader colAction;
        private ColumnHeader colBy;
        private ColumnHeader colAt;
        private ColumnHeader colPayload;
        private ColumnHeader colDocId;
        private ListBox lstImportInvalid;
        private Button btnImportCsv;
        private Button btnBrowseCsv;
        private TextBox txtImportCsv;
        private Label label16;
        private ListView lvImportSummary;
        private ColumnHeader colSeries;
        private ColumnHeader colMaxNum;
        private ColumnHeader colNextNum;
        private Button btnUseSuggested;
        private Button btnAuditFilter;
        private TextBox txtAuditAction;
        private TextBox txtAuditUser;
        private Label labelAuditAction;
        private Label labelAuditUser;
        private CheckBox chkSeedCounters;
        private Button btnAuditPrev;
        private Button btnAuditNext;
        private Label lblAuditPage;
        private Button btnCreateRecommended;
        private Label lblNoDelete;
        private Label lblImportNote;
        private Button btnSeedSelected;
        private TabPage tabDocs;
        private Button btnDocsRefresh;
        private ListView lvDocs;
        private ColumnHeader colDocCode;
        private ColumnHeader colDocFile;
        private ColumnHeader colDocBy;
        private ColumnHeader colDocAt;
        private Label lblRecommendInfo;
        private Label lblImportPerSeries;
    }
}
