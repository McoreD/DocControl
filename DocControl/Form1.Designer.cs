using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DocControl
{
    partial class Form1
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabGenerate = new TabPage();
            lblNoDelete = new Label();
            lblGenerateResult = new Label();
            btnGenerate = new Button();
            txtExtension = new TextBox();
            label7 = new Label();
            txtFreeText = new TextBox();
            label6 = new Label();
            chkEnableLevel4 = new CheckBox();
            cmbLevel4 = new ComboBox();
            label5 = new Label();
            cmbLevel3 = new ComboBox();
            label4 = new Label();
            cmbLevel2 = new ComboBox();
            label3 = new Label();
            cmbLevel1 = new ComboBox();
            label2 = new Label();

            tabCodes = new TabPage();
            btnCodesImportCsv = new Button();
            btnCodesRefresh = new Button();
            lvCodes = new ListView();
            colCodeLevel = new ColumnHeader();
            colCodeValue = new ColumnHeader();
            colCodeDescription = new ColumnHeader();
            lblCodesInfo = new Label();

            tabImport = new TabPage();
            lblImportPerSeries = new Label();
            lblImportResult = new Label();
            lblImportNote = new Label();
            chkSeedCounters = new CheckBox();
            btnSeedSelected = new Button();
            lvImportSummary = new ListView();
            colSeries = new ColumnHeader();
            colMaxNum = new ColumnHeader();
            colNextNum = new ColumnHeader();
            lstImportInvalid = new ListBox();

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
            btnAuditFilter = new Button();
            btnAuditPrev = new Button();
            btnAuditNext = new Button();
            lblAuditPage = new Label();
            txtAuditAction = new TextBox();
            txtAuditUser = new TextBox();
            labelAuditAction = new Label();
            labelAuditUser = new Label();
            lvAudit = new ListView();
            colAt = new ColumnHeader();
            colBy = new ColumnHeader();
            colAction = new ColumnHeader();
            colPayload = new ColumnHeader();
            colDocId = new ColumnHeader();

            tabDocs = new TabPage();
            btnDocsRefresh = new Button();
            lvDocs = new ListView();
            colDocCode = new ColumnHeader();
            colDocFile = new ColumnHeader();
            colDocBy = new ColumnHeader();
            colDocAt = new ColumnHeader();
            lblImportPerSeriesDocs = new Label();
            btnImportCsv = new Button();

            tabControl1.SuspendLayout();
            tabGenerate.SuspendLayout();
            tabCodes.SuspendLayout();
            tabImport.SuspendLayout();
            tabRecommend.SuspendLayout();
            tabSettings.SuspendLayout();
            ((ISupportInitialize)numPadding).BeginInit();
            tabAudit.SuspendLayout();
            tabDocs.SuspendLayout();
            SuspendLayout();

            // tabControl1
            tabControl1.Controls.Add(tabGenerate);
            tabControl1.Controls.Add(tabCodes);
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

            // tabGenerate
            tabGenerate.Controls.Add(lblNoDelete);
            tabGenerate.Controls.Add(lblGenerateResult);
            tabGenerate.Controls.Add(btnGenerate);
            tabGenerate.Controls.Add(txtExtension);
            tabGenerate.Controls.Add(label7);
            tabGenerate.Controls.Add(txtFreeText);
            tabGenerate.Controls.Add(label6);
            tabGenerate.Controls.Add(chkEnableLevel4);
            tabGenerate.Controls.Add(cmbLevel4);
            tabGenerate.Controls.Add(label5);
            tabGenerate.Controls.Add(cmbLevel3);
            tabGenerate.Controls.Add(label4);
            tabGenerate.Controls.Add(cmbLevel2);
            tabGenerate.Controls.Add(label3);
            tabGenerate.Controls.Add(cmbLevel1);
            tabGenerate.Controls.Add(label2);
            tabGenerate.Location = new Point(4, 24);
            tabGenerate.Name = "tabGenerate";
            tabGenerate.Padding = new Padding(3);
            tabGenerate.Size = new Size(776, 433);
            tabGenerate.TabIndex = 0;
            tabGenerate.Text = "Generate";
            tabGenerate.UseVisualStyleBackColor = true;

            // label2
            label2.AutoSize = true;
            label2.Location = new Point(24, 60);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 0;
            label2.Text = "Level1";

            // cmbLevel1
            cmbLevel1.DropDownStyle = ComboBoxStyle.DropDown;
            cmbLevel1.FormattingEnabled = true;
            cmbLevel1.Location = new Point(104, 57);
            cmbLevel1.Name = "cmbLevel1";
            cmbLevel1.Size = new Size(152, 23);
            cmbLevel1.TabIndex = 1;
            cmbLevel1.SelectedIndexChanged += cmbLevel1_SelectedIndexChanged;
            cmbLevel1.TextChanged += cmbLevel1_TextChanged;

            // label3
            label3.AutoSize = true;
            label3.Location = new Point(296, 60);
            label3.Name = "label3";
            label3.Size = new Size(42, 15);
            label3.TabIndex = 2;
            label3.Text = "Level2";

            // cmbLevel2
            cmbLevel2.DropDownStyle = ComboBoxStyle.DropDown;
            cmbLevel2.FormattingEnabled = true;
            cmbLevel2.Location = new Point(352, 57);
            cmbLevel2.Name = "cmbLevel2";
            cmbLevel2.Size = new Size(152, 23);
            cmbLevel2.TabIndex = 3;
            cmbLevel2.SelectedIndexChanged += cmbLevel2_SelectedIndexChanged;
            cmbLevel2.TextChanged += cmbLevel2_TextChanged;

            // label4
            label4.AutoSize = true;
            label4.Location = new Point(24, 99);
            label4.Name = "label4";
            label4.Size = new Size(42, 15);
            label4.TabIndex = 4;
            label4.Text = "Level3";

            // cmbLevel3
            cmbLevel3.DropDownStyle = ComboBoxStyle.DropDown;
            cmbLevel3.FormattingEnabled = true;
            cmbLevel3.Location = new Point(104, 96);
            cmbLevel3.Name = "cmbLevel3";
            cmbLevel3.Size = new Size(152, 23);
            cmbLevel3.TabIndex = 5;
            cmbLevel3.SelectedIndexChanged += cmbLevel3_SelectedIndexChanged;
            cmbLevel3.TextChanged += cmbLevel3_TextChanged;

            // label5
            label5.AutoSize = true;
            label5.Location = new Point(296, 99);
            label5.Name = "label5";
            label5.Size = new Size(42, 15);
            label5.TabIndex = 6;
            label5.Text = "Level4";

            // cmbLevel4
            cmbLevel4.DropDownStyle = ComboBoxStyle.DropDown;
            cmbLevel4.FormattingEnabled = true;
            cmbLevel4.Location = new Point(352, 96);
            cmbLevel4.Name = "cmbLevel4";
            cmbLevel4.Size = new Size(152, 23);
            cmbLevel4.TabIndex = 7;

            // chkEnableLevel4
            chkEnableLevel4.AutoSize = true;
            chkEnableLevel4.Location = new Point(24, 129);
            chkEnableLevel4.Name = "chkEnableLevel4";
            chkEnableLevel4.Size = new Size(99, 19);
            chkEnableLevel4.TabIndex = 8;
            chkEnableLevel4.Text = "Enable Level4";
            chkEnableLevel4.UseVisualStyleBackColor = true;
            chkEnableLevel4.CheckedChanged += chkEnableLevel4_CheckedChanged;

            // label6
            label6.AutoSize = true;
            label6.Location = new Point(24, 163);
            label6.Name = "label6";
            label6.Size = new Size(55, 15);
            label6.TabIndex = 9;
            label6.Text = "Free Text";

            // txtFreeText
            txtFreeText.Location = new Point(104, 160);
            txtFreeText.Name = "txtFreeText";
            txtFreeText.Size = new Size(400, 23);
            txtFreeText.TabIndex = 10;

            // label7
            label7.AutoSize = true;
            label7.Location = new Point(24, 199);
            label7.Name = "label7";
            label7.Size = new Size(60, 15);
            label7.TabIndex = 11;
            label7.Text = "Extension";

            // txtExtension
            txtExtension.Location = new Point(104, 196);
            txtExtension.Name = "txtExtension";
            txtExtension.Size = new Size(200, 23);
            txtExtension.TabIndex = 12;

            // btnGenerate
            btnGenerate.Location = new Point(24, 239);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(135, 27);
            btnGenerate.TabIndex = 13;
            btnGenerate.Text = "Generate";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;

            // lblGenerateResult
            lblGenerateResult.AutoSize = true;
            lblGenerateResult.Location = new Point(24, 278);
            lblGenerateResult.Name = "lblGenerateResult";
            lblGenerateResult.Size = new Size(0, 15);
            lblGenerateResult.TabIndex = 14;

            // lblNoDelete
            lblNoDelete.AutoSize = true;
            lblNoDelete.Location = new Point(24, 24);
            lblNoDelete.Name = "lblNoDelete";
            lblNoDelete.Size = new Size(0, 15);
            lblNoDelete.TabIndex = 15;

            // tabCodes
            tabCodes.Controls.Add(btnCodesRefresh);
            tabCodes.Controls.Add(btnCodesImportCsv);
            tabCodes.Controls.Add(lvCodes);
            tabCodes.Controls.Add(lblCodesInfo);
            tabCodes.Location = new Point(4, 24);
            tabCodes.Name = "tabCodes";
            tabCodes.Padding = new Padding(3);
            tabCodes.Size = new Size(776, 433);
            tabCodes.TabIndex = 6;
            tabCodes.Text = "Codes";
            tabCodes.UseVisualStyleBackColor = true;

            lblCodesInfo.AutoSize = true;
            lblCodesInfo.Location = new Point(24, 24);
            lblCodesInfo.Name = "lblCodesInfo";
            lblCodesInfo.Size = new Size(400, 15);
            lblCodesInfo.TabIndex = 0;
            lblCodesInfo.Text = "Import and manage code series (Level1, Level2, Level3, Level4).";

            btnCodesImportCsv.Location = new Point(24, 54);
            btnCodesImportCsv.Name = "btnCodesImportCsv";
            btnCodesImportCsv.Size = new Size(135, 27);
            btnCodesImportCsv.TabIndex = 1;
            btnCodesImportCsv.Text = "Import CSV";
            btnCodesImportCsv.UseVisualStyleBackColor = true;
            btnCodesImportCsv.Click += btnCodesImportCsv_Click;

            btnCodesRefresh.Location = new Point(165, 54);
            btnCodesRefresh.Name = "btnCodesRefresh";
            btnCodesRefresh.Size = new Size(135, 27);
            btnCodesRefresh.TabIndex = 2;
            btnCodesRefresh.Text = "Refresh";
            btnCodesRefresh.UseVisualStyleBackColor = true;
            btnCodesRefresh.Click += btnCodesRefresh_Click;

            lvCodes.Columns.AddRange(new ColumnHeader[] { colCodeLevel, colCodeValue, colCodeDescription });
            lvCodes.FullRowSelect = true;
            lvCodes.GridLines = true;
            lvCodes.HideSelection = false;
            lvCodes.Location = new Point(24, 90);
            lvCodes.Name = "lvCodes";
            lvCodes.Size = new Size(728, 319);
            lvCodes.TabIndex = 3;
            lvCodes.UseCompatibleStateImageBehavior = false;
            lvCodes.View = View.Details;

            colCodeLevel.Text = "Level";
            colCodeLevel.Width = 80;

            colCodeValue.Text = "Code";
            colCodeValue.Width = 150;

            colCodeDescription.Text = "Description";
            colCodeDescription.Width = 450;

            // tabImport
            tabImport.Controls.Add(lblImportPerSeries);
            tabImport.Controls.Add(lvImportSummary);
            tabImport.Controls.Add(btnSeedSelected);
            tabImport.Controls.Add(chkSeedCounters);
            tabImport.Controls.Add(lblImportNote);
            tabImport.Controls.Add(lblImportResult);
            tabImport.Controls.Add(lstImportInvalid);
            tabImport.Location = new Point(4, 24);
            tabImport.Name = "tabImport";
            tabImport.Padding = new Padding(3);
            tabImport.Size = new Size(776, 433);
            tabImport.TabIndex = 2;
            tabImport.Text = "Import";
            tabImport.UseVisualStyleBackColor = true;

            // lblImportPerSeries
            lblImportPerSeries.AutoSize = true;
            lblImportPerSeries.Location = new Point(24, 190);
            lblImportPerSeries.Name = "lblImportPerSeries";
            lblImportPerSeries.Size = new Size(0, 15);
            lblImportPerSeries.TabIndex = 0;

            // lvImportSummary
            lvImportSummary.Columns.AddRange(new ColumnHeader[] { colSeries, colMaxNum, colNextNum });
            lvImportSummary.FullRowSelect = true;
            lvImportSummary.GridLines = true;
            lvImportSummary.HideSelection = false;
            lvImportSummary.Location = new Point(24, 24);
            lvImportSummary.Name = "lvImportSummary";
            lvImportSummary.Size = new Size(728, 158);
            lvImportSummary.TabIndex = 4;
            lvImportSummary.UseCompatibleStateImageBehavior = false;
            lvImportSummary.View = View.Details;

            colSeries.Text = "Series";
            colSeries.Width = 120;

            colMaxNum.Text = "Max Number";
            colMaxNum.Width = 120;

            colNextNum.Text = "Next Number";
            colNextNum.Width = 120;

            // btnSeedSelected
            btnSeedSelected.Location = new Point(165, 220);
            btnSeedSelected.Name = "btnSeedSelected";
            btnSeedSelected.Size = new Size(135, 27);
            btnSeedSelected.TabIndex = 6;
            btnSeedSelected.Text = "Seed Selected";
            btnSeedSelected.UseVisualStyleBackColor = true;
            btnSeedSelected.Click += btnSeedSelected_Click;

            // chkSeedCounters
            chkSeedCounters.AutoSize = true;
            chkSeedCounters.Location = new Point(24, 225);
            chkSeedCounters.Name = "chkSeedCounters";
            chkSeedCounters.Size = new Size(109, 19);
            chkSeedCounters.TabIndex = 5;
            chkSeedCounters.Text = "Seed Counters";
            chkSeedCounters.UseVisualStyleBackColor = true;

            // lblImportNote
            lblImportNote.AutoSize = true;
            lblImportNote.Location = new Point(24, 255);
            lblImportNote.Name = "lblImportNote";
            lblImportNote.Size = new Size(0, 15);
            lblImportNote.TabIndex = 7;

            // lblImportResult
            lblImportResult.AutoSize = true;
            lblImportResult.Location = new Point(24, 278);
            lblImportResult.Name = "lblImportResult";
            lblImportResult.Size = new Size(0, 15);
            lblImportResult.TabIndex = 8;

            // lstImportInvalid
            lstImportInvalid.FormattingEnabled = true;
            lstImportInvalid.ItemHeight = 15;
            lstImportInvalid.Location = new Point(24, 311);
            lstImportInvalid.Name = "lstImportInvalid";
            lstImportInvalid.Size = new Size(728, 109);
            lstImportInvalid.TabIndex = 9;

            // tabRecommend
            tabRecommend.Controls.Add(txtNlqResult);
            tabRecommend.Controls.Add(btnInterpret);
            tabRecommend.Controls.Add(btnRecommend);
            tabRecommend.Controls.Add(lblRecommendResult);
            tabRecommend.Controls.Add(txtNlq);
            tabRecommend.Controls.Add(label9);
            tabRecommend.Controls.Add(btnUseSuggested);
            tabRecommend.Controls.Add(btnCreateRecommended);
            tabRecommend.Controls.Add(lblRecommendInfo);
            tabRecommend.Location = new Point(4, 24);
            tabRecommend.Name = "tabRecommend";
            tabRecommend.Size = new Size(776, 433);
            tabRecommend.TabIndex = 3;
            tabRecommend.Text = "Recommend";
            tabRecommend.UseVisualStyleBackColor = true;

            txtNlqResult.Location = new Point(24, 194);
            txtNlqResult.Multiline = true;
            txtNlqResult.Name = "txtNlqResult";
            txtNlqResult.Size = new Size(728, 158);
            txtNlqResult.TabIndex = 8;

            btnInterpret.Location = new Point(24, 165);
            btnInterpret.Name = "btnInterpret";
            btnInterpret.Size = new Size(135, 27);
            btnInterpret.TabIndex = 7;
            btnInterpret.Text = "Interpret NLQ";
            btnInterpret.UseVisualStyleBackColor = true;
            btnInterpret.Click += btnInterpret_Click;

            btnRecommend.Location = new Point(165, 165);
            btnRecommend.Name = "btnRecommend";
            btnRecommend.Size = new Size(135, 27);
            btnRecommend.TabIndex = 6;
            btnRecommend.Text = "Get Recommendation";
            btnRecommend.UseVisualStyleBackColor = true;
            btnRecommend.Click += btnRecommend_Click;

            lblRecommendResult.AutoSize = true;
            lblRecommendResult.Location = new Point(24, 278);
            lblRecommendResult.Name = "lblRecommendResult";
            lblRecommendResult.Size = new Size(0, 15);
            lblRecommendResult.TabIndex = 5;

            txtNlq.Location = new Point(104, 24);
            txtNlq.Multiline = true;
            txtNlq.Name = "txtNlq";
            txtNlq.Size = new Size(648, 72);
            txtNlq.TabIndex = 4;

            label9.AutoSize = true;
            label9.Location = new Point(24, 27);
            label9.Name = "label9";
            label9.Size = new Size(34, 15);
            label9.TabIndex = 3;
            label9.Text = "NLQ:";

            btnUseSuggested.Location = new Point(24, 72);
            btnUseSuggested.Name = "btnUseSuggested";
            btnUseSuggested.Size = new Size(135, 27);
            btnUseSuggested.TabIndex = 2;
            btnUseSuggested.Text = "Use Suggested";
            btnUseSuggested.UseVisualStyleBackColor = true;
            btnUseSuggested.Click += btnUseSuggested_Click;

            btnCreateRecommended.Location = new Point(165, 72);
            btnCreateRecommended.Name = "btnCreateRecommended";
            btnCreateRecommended.Size = new Size(135, 27);
            btnCreateRecommended.TabIndex = 1;
            btnCreateRecommended.Text = "Create Recommended";
            btnCreateRecommended.UseVisualStyleBackColor = true;
            btnCreateRecommended.Click += btnCreateRecommended_Click;

            lblRecommendInfo.AutoSize = true;
            lblRecommendInfo.Location = new Point(24, 109);
            lblRecommendInfo.Name = "lblRecommendInfo";
            lblRecommendInfo.Size = new Size(0, 15);
            lblRecommendInfo.TabIndex = 0;

            // tabSettings
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
            tabSettings.Size = new Size(776, 433);
            tabSettings.TabIndex = 4;
            tabSettings.Text = "Settings";
            tabSettings.UseVisualStyleBackColor = true;

            cmbProvider.DropDownStyle = ComboBoxStyle.DropDown;
            cmbProvider.FormattingEnabled = true;
            cmbProvider.Location = new Point(104, 24);
            cmbProvider.Name = "cmbProvider";
            cmbProvider.Size = new Size(152, 23);
            cmbProvider.TabIndex = 6;

            label11.AutoSize = true;
            label11.Location = new Point(24, 27);
            label11.Name = "label11";
            label11.Size = new Size(57, 15);
            label11.TabIndex = 5;
            label11.Text = "Provider:";

            chkSettingsEnableLevel4.AutoSize = true;
            chkSettingsEnableLevel4.Location = new Point(24, 85);
            chkSettingsEnableLevel4.Name = "chkSettingsEnableLevel4";
            chkSettingsEnableLevel4.Size = new Size(99, 19);
            chkSettingsEnableLevel4.TabIndex = 0;
            chkSettingsEnableLevel4.Text = "Enable Level4";
            chkSettingsEnableLevel4.UseVisualStyleBackColor = true;

            txtSeparator.Location = new Point(104, 124);
            txtSeparator.Name = "txtSeparator";
            txtSeparator.Size = new Size(200, 23);
            txtSeparator.TabIndex = 2;

            label1.AutoSize = true;
            label1.Location = new Point(24, 127);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 1;
            label1.Text = "Separator:";

            txtOpenAiModel.Location = new Point(104, 232);
            txtOpenAiModel.Name = "txtOpenAiModel";
            txtOpenAiModel.Size = new Size(648, 23);
            txtOpenAiModel.TabIndex = 8;

            label12.AutoSize = true;
            label12.Location = new Point(24, 235);
            label12.Name = "label12";
            label12.Size = new Size(83, 15);
            label12.TabIndex = 7;
            label12.Text = "OpenAI Model";

            txtGeminiModel.Location = new Point(104, 268);
            txtGeminiModel.Name = "txtGeminiModel";
            txtGeminiModel.Size = new Size(648, 23);
            txtGeminiModel.TabIndex = 10;

            label13.AutoSize = true;
            label13.Location = new Point(24, 271);
            label13.Name = "label13";
            label13.Size = new Size(87, 15);
            label13.TabIndex = 9;
            label13.Text = "Gemini Model";

            txtOpenAiKey.Location = new Point(104, 304);
            txtOpenAiKey.Name = "txtOpenAiKey";
            txtOpenAiKey.Size = new Size(648, 23);
            txtOpenAiKey.TabIndex = 12;

            label14.AutoSize = true;
            label14.Location = new Point(24, 307);
            label14.Name = "label14";
            label14.Size = new Size(69, 15);
            label14.TabIndex = 11;
            label14.Text = "OpenAI Key";

            txtGeminiKey.Location = new Point(104, 340);
            txtGeminiKey.Name = "txtGeminiKey";
            txtGeminiKey.Size = new Size(648, 23);
            txtGeminiKey.TabIndex = 14;

            label15.AutoSize = true;
            label15.Location = new Point(24, 343);
            label15.Name = "label15";
            label15.Size = new Size(71, 15);
            label15.TabIndex = 13;
            label15.Text = "Gemini Key";

            label10.AutoSize = true;
            label10.Location = new Point(24, 391);
            label10.Name = "label10";
            label10.Size = new Size(55, 15);
            label10.TabIndex = 3;
            label10.Text = "Padding:";

            numPadding.Location = new Point(104, 389);
            numPadding.Name = "numPadding";
            numPadding.Size = new Size(120, 23);
            numPadding.TabIndex = 4;

            btnSaveSettings.Location = new Point(24, 385);
            btnSaveSettings.Name = "btnSaveSettings";
            btnSaveSettings.Size = new Size(135, 27);
            btnSaveSettings.TabIndex = 15;
            btnSaveSettings.Text = "Save Settings";
            btnSaveSettings.UseVisualStyleBackColor = true;
            btnSaveSettings.Click += btnSaveSettings_Click;

            // tabAudit
            tabAudit.Controls.Add(btnAuditRefresh);
            tabAudit.Controls.Add(btnAuditFilter);
            tabAudit.Controls.Add(btnAuditPrev);
            tabAudit.Controls.Add(btnAuditNext);
            tabAudit.Controls.Add(lblAuditPage);
            tabAudit.Controls.Add(txtAuditAction);
            tabAudit.Controls.Add(txtAuditUser);
            tabAudit.Controls.Add(labelAuditAction);
            tabAudit.Controls.Add(labelAuditUser);
            tabAudit.Controls.Add(lvAudit);
            tabAudit.Location = new Point(4, 24);
            tabAudit.Name = "tabAudit";
            tabAudit.Size = new Size(776, 433);
            tabAudit.TabIndex = 5;
            tabAudit.Text = "Audit";
            tabAudit.UseVisualStyleBackColor = true;

            btnAuditRefresh.Location = new Point(24, 24);
            btnAuditRefresh.Name = "btnAuditRefresh";
            btnAuditRefresh.Size = new Size(135, 27);
            btnAuditRefresh.TabIndex = 0;
            btnAuditRefresh.Text = "Refresh";
            btnAuditRefresh.UseVisualStyleBackColor = true;

            btnAuditFilter.Location = new Point(165, 24);
            btnAuditFilter.Name = "btnAuditFilter";
            btnAuditFilter.Size = new Size(135, 27);
            btnAuditFilter.TabIndex = 1;
            btnAuditFilter.Text = "Filter";
            btnAuditFilter.UseVisualStyleBackColor = true;

            labelAuditUser.AutoSize = true;
            labelAuditUser.Location = new Point(24, 60);
            labelAuditUser.Name = "labelAuditUser";
            labelAuditUser.Size = new Size(30, 15);
            labelAuditUser.TabIndex = 2;
            labelAuditUser.Text = "User:";

            txtAuditUser.Location = new Point(104, 57);
            txtAuditUser.Name = "txtAuditUser";
            txtAuditUser.Size = new Size(200, 23);
            txtAuditUser.TabIndex = 3;

            labelAuditAction.AutoSize = true;
            labelAuditAction.Location = new Point(320, 60);
            labelAuditAction.Name = "labelAuditAction";
            labelAuditAction.Size = new Size(45, 15);
            labelAuditAction.TabIndex = 4;
            labelAuditAction.Text = "Action:";

            txtAuditAction.Location = new Point(380, 57);
            txtAuditAction.Name = "txtAuditAction";
            txtAuditAction.Size = new Size(200, 23);
            txtAuditAction.TabIndex = 5;

            lvAudit.Columns.AddRange(new ColumnHeader[] { colAt, colBy, colAction, colPayload, colDocId });
            lvAudit.FullRowSelect = true;
            lvAudit.GridLines = true;
            lvAudit.HideSelection = false;
            lvAudit.Location = new Point(24, 90);
            lvAudit.Name = "lvAudit";
            lvAudit.Size = new Size(728, 250);
            lvAudit.TabIndex = 6;
            lvAudit.UseCompatibleStateImageBehavior = false;
            lvAudit.View = View.Details;

            colAt.Text = "Timestamp";
            colAt.Width = 150;

            colBy.Text = "User";
            colBy.Width = 100;

            colAction.Text = "Action";
            colAction.Width = 100;

            colPayload.Text = "Payload";
            colPayload.Width = 250;

            colDocId.Text = "Doc ID";
            colDocId.Width = 100;

            btnAuditPrev.Location = new Point(24, 350);
            btnAuditPrev.Name = "btnAuditPrev";
            btnAuditPrev.Size = new Size(100, 27);
            btnAuditPrev.TabIndex = 7;
            btnAuditPrev.Text = "< Previous";
            btnAuditPrev.UseVisualStyleBackColor = true;

            lblAuditPage.AutoSize = true;
            lblAuditPage.Location = new Point(350, 356);
            lblAuditPage.Name = "lblAuditPage";
            lblAuditPage.Size = new Size(50, 15);
            lblAuditPage.TabIndex = 8;
            lblAuditPage.Text = "Page 1/1";
            lblAuditPage.TextAlign = ContentAlignment.MiddleCenter;

            btnAuditNext.Location = new Point(652, 350);
            btnAuditNext.Name = "btnAuditNext";
            btnAuditNext.Size = new Size(100, 27);
            btnAuditNext.TabIndex = 9;
            btnAuditNext.Text = "Next >";
            btnAuditNext.UseVisualStyleBackColor = true;

            // tabDocs
            tabDocs.Controls.Add(btnDocsRefresh);
            tabDocs.Controls.Add(btnImportCsv);
            tabDocs.Controls.Add(lvDocs);
            tabDocs.Controls.Add(lblImportPerSeriesDocs);
            tabDocs.Location = new Point(4, 24);
            tabDocs.Name = "tabDocs";
            tabDocs.Size = new Size(776, 433);
            tabDocs.TabIndex = 7;
            tabDocs.Text = "Documents";
            tabDocs.UseVisualStyleBackColor = true;

            lblImportPerSeriesDocs.AutoSize = true;
            lblImportPerSeriesDocs.Location = new Point(24, 24);
            lblImportPerSeriesDocs.Name = "lblImportPerSeriesDocs";
            lblImportPerSeriesDocs.Size = new Size(150, 15);
            lblImportPerSeriesDocs.TabIndex = 0;
            lblImportPerSeriesDocs.Text = "Manage document records";

            btnImportCsv.Location = new Point(24, 54);
            btnImportCsv.Name = "btnImportCsv";
            btnImportCsv.Size = new Size(135, 27);
            btnImportCsv.TabIndex = 1;
            btnImportCsv.Text = "Import CSV";
            btnImportCsv.UseVisualStyleBackColor = true;

            btnDocsRefresh.Location = new Point(165, 54);
            btnDocsRefresh.Name = "btnDocsRefresh";
            btnDocsRefresh.Size = new Size(135, 27);
            btnDocsRefresh.TabIndex = 2;
            btnDocsRefresh.Text = "Refresh";
            btnDocsRefresh.UseVisualStyleBackColor = true;

            lvDocs.Columns.AddRange(new ColumnHeader[] { colDocCode, colDocFile, colDocBy, colDocAt });
            lvDocs.FullRowSelect = true;
            lvDocs.GridLines = true;
            lvDocs.HideSelection = false;
            lvDocs.Location = new Point(24, 90);
            lvDocs.Name = "lvDocs";
            lvDocs.Size = new Size(728, 319);
            lvDocs.TabIndex = 3;
            lvDocs.UseCompatibleStateImageBehavior = false;
            lvDocs.View = View.Details;

            colDocCode.Text = "Document Code";
            colDocCode.Width = 150;

            colDocFile.Text = "File Name";
            colDocFile.Width = 250;

            colDocBy.Text = "Created By";
            colDocBy.Width = 100;

            colDocAt.Text = "Created At";
            colDocAt.Width = 150;

            tabControl1.ResumeLayout(false);
            tabGenerate.ResumeLayout(false);
            tabGenerate.PerformLayout();
            tabCodes.ResumeLayout(false);
            tabCodes.PerformLayout();
            tabImport.ResumeLayout(false);
            tabImport.PerformLayout();
            tabRecommend.ResumeLayout(false);
            tabRecommend.PerformLayout();
            tabSettings.ResumeLayout(false);
            tabSettings.PerformLayout();
            ((ISupportInitialize)numPadding).EndInit();
            tabAudit.ResumeLayout(false);
            tabAudit.PerformLayout();
            tabDocs.ResumeLayout(false);
            tabDocs.PerformLayout();

            Controls.Add(tabControl1);
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 461);
            Name = "Form1";
            Text = "DocControl";

            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabGenerate;
        private TabPage tabCodes;
        private TabPage tabImport;
        private TabPage tabRecommend;
        private TabPage tabSettings;
        private TabPage tabAudit;
        private TabPage tabDocs;

        private Label label2;
        private ComboBox cmbLevel1;
        private ComboBox cmbLevel2;
        private ComboBox cmbLevel3;
        private ComboBox cmbLevel4;
        private Label label3;
        private Label label4;
        private Label label5;
        private CheckBox chkEnableLevel4;
        private TextBox txtFreeText;
        private Label label6;
        private Button btnGenerate;
        private TextBox txtExtension;
        private Label label7;
        private Label lblGenerateResult;
        private Label lblNoDelete;

        private Button btnCodesImportCsv;
        private Button btnCodesRefresh;
        private ListView lvCodes;
        private ColumnHeader colCodeLevel;
        private ColumnHeader colCodeValue;
        private ColumnHeader colCodeDescription;
        private Label lblCodesInfo;

        private Label lblImportResult;
        private ListBox lstImportInvalid;
        private ListView lvImportSummary;
        private ColumnHeader colSeries;
        private ColumnHeader colMaxNum;
        private ColumnHeader colNextNum;
        private Label lblImportNote;
        private CheckBox chkSeedCounters;
        private Button btnSeedSelected;
        private Label lblImportPerSeries;

        private TextBox txtNlqResult;
        private Button btnInterpret;
        private Button btnRecommend;
        private Label lblRecommendResult;
        private TextBox txtNlq;
        private Label label9;
        private Button btnUseSuggested;
        private Button btnCreateRecommended;
        private Label lblRecommendInfo;

        private Button btnSaveSettings;
        private TextBox txtGeminiKey;
        private Label label15;
        private TextBox txtOpenAiKey;
        private Label label14;
        private TextBox txtGeminiModel;
        private Label label13;
        private TextBox txtOpenAiModel;
        private Label label12;
        private ComboBox cmbProvider;
        private Label label11;
        private NumericUpDown numPadding;
        private Label label10;
        private TextBox txtSeparator;
        private Label label1;
        private CheckBox chkSettingsEnableLevel4;

        private Button btnAuditRefresh;
        private ListView lvAudit;
        private ColumnHeader colAt;
        private ColumnHeader colBy;
        private ColumnHeader colAction;
        private ColumnHeader colPayload;
        private ColumnHeader colDocId;
        private Button btnAuditFilter;
        private TextBox txtAuditAction;
        private TextBox txtAuditUser;
        private Label labelAuditAction;
        private Label labelAuditUser;
        private Button btnAuditPrev;
        private Button btnAuditNext;
        private Label lblAuditPage;

        private Button btnDocsRefresh;
        private ListView lvDocs;
        private ColumnHeader colDocCode;
        private ColumnHeader colDocFile;
        private ColumnHeader colDocBy;
        private ColumnHeader colDocAt;
        private Label lblImportPerSeriesDocs;
        private Button btnImportCsv;
    }
}
