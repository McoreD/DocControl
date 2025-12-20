using System.IO;
using System.Text.RegularExpressions;
using DocControl.AI;
using DocControl.Configuration;
using DocControl.Data;
using DocControl.Forms;
using DocControl.Models;
using DocControl.Services;

namespace DocControl
{
    public partial class Form1
    {
        private readonly DocumentService? documentService;
        private readonly ImportService? importService;
        private readonly NlqService? nlqService;
        private readonly ConfigService? configService;
        private readonly DocumentConfig? documentConfig;
        private readonly AiSettings? aiSettings;
        private readonly AiClientOptions? aiOptions;
        private readonly AuditRepository? auditRepository;
        private readonly RecommendationService? recommendationService;
        private readonly DocumentRepository? documentRepository;
        private readonly CodeImportService? codeImportService;
        private readonly CodeSeriesRepository? codeSeriesRepository;
        private int? lastSuggested;
        private CodeSeriesKey? lastSuggestedKey;
        private int auditPage = 1;
        private const int AuditPageSize = 50;
        private IReadOnlyList<ImportSeriesSummary> lastSummaries = Array.Empty<ImportSeriesSummary>();

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(DocumentService documentService, ImportService importService, NlqService nlqService, ConfigService configService, DocumentConfig documentConfig, AiSettings aiSettings, AiClientOptions aiOptions, AuditRepository auditRepository, RecommendationService recommendationService, DocumentRepository documentRepository, CodeImportService codeImportService, CodeSeriesRepository codeSeriesRepository)
            : this()
        {
            this.documentService = documentService;
            this.importService = importService;
            this.nlqService = nlqService;
            this.configService = configService;
            this.documentConfig = documentConfig;
            this.aiSettings = aiSettings;
            this.aiOptions = aiOptions;
            this.auditRepository = auditRepository;
            this.recommendationService = recommendationService;
            this.documentRepository = documentRepository;
            this.codeImportService = codeImportService;
            this.codeSeriesRepository = codeSeriesRepository;

            LoadSettingsToUi();
            AddCodeImportButton();
            LoadCodesAsync();
        }

        private async void LoadCodesAsync()
        {
            if (codeSeriesRepository is null) return;

            try
            {
                await PopulateLevel1CodesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load codes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task PopulateLevel1CodesAsync()
        {
            if (codeSeriesRepository is null) return;

            var codes = await codeSeriesRepository.GetLevel1CodesAsync();
            cmbLevel1.Items.Clear();
            cmbLevel1.Items.Add(""); // Add empty option for manual entry
            foreach (var code in codes)
            {
                cmbLevel1.Items.Add(code);
            }
        }

        private async void cmbLevel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (codeSeriesRepository is null) return;

            var selectedLevel1 = string.IsNullOrWhiteSpace(cmbLevel1.Text) ? null : cmbLevel1.Text.Trim();
            
            // Clear and repopulate Level2
            var level2Codes = await codeSeriesRepository.GetLevel2CodesAsync(selectedLevel1);
            cmbLevel2.Items.Clear();
            cmbLevel2.Items.Add(""); // Add empty option
            foreach (var code in level2Codes)
            {
                cmbLevel2.Items.Add(code);
            }

            // Clear dependent levels
            cmbLevel3.Items.Clear();
            cmbLevel3.Items.Add("");
            cmbLevel4.Items.Clear();
            cmbLevel4.Items.Add("");
        }

        private async void cmbLevel2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (codeSeriesRepository is null) return;

            var selectedLevel1 = string.IsNullOrWhiteSpace(cmbLevel1.Text) ? null : cmbLevel1.Text.Trim();
            var selectedLevel2 = string.IsNullOrWhiteSpace(cmbLevel2.Text) ? null : cmbLevel2.Text.Trim();
            
            // Clear and repopulate Level3
            var level3Codes = await codeSeriesRepository.GetLevel3CodesAsync(selectedLevel1, selectedLevel2);
            cmbLevel3.Items.Clear();
            cmbLevel3.Items.Add(""); // Add empty option
            foreach (var code in level3Codes)
            {
                cmbLevel3.Items.Add(code);
            }

            // Clear dependent level
            cmbLevel4.Items.Clear();
            cmbLevel4.Items.Add("");
        }

        private async void cmbLevel3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (codeSeriesRepository is null || !chkEnableLevel4.Checked) return;

            var selectedLevel1 = string.IsNullOrWhiteSpace(cmbLevel1.Text) ? null : cmbLevel1.Text.Trim();
            var selectedLevel2 = string.IsNullOrWhiteSpace(cmbLevel2.Text) ? null : cmbLevel2.Text.Trim();
            var selectedLevel3 = string.IsNullOrWhiteSpace(cmbLevel3.Text) ? null : cmbLevel3.Text.Trim();
            
            // Clear and repopulate Level4
            var level4Codes = await codeSeriesRepository.GetLevel4CodesAsync(selectedLevel1, selectedLevel2, selectedLevel3);
            cmbLevel4.Items.Clear();
            cmbLevel4.Items.Add(""); // Add empty option
            foreach (var code in level4Codes)
            {
                cmbLevel4.Items.Add(code);
            }
        }

        private async void cmbLevel1_TextChanged(object sender, EventArgs e)
        {
            // Debounce to avoid too many database calls
            await Task.Delay(300);
            if (codeSeriesRepository is null) return;

            var selectedLevel1 = string.IsNullOrWhiteSpace(cmbLevel1.Text) ? null : cmbLevel1.Text.Trim();
            
            // Clear and repopulate Level2
            var level2Codes = await codeSeriesRepository.GetLevel2CodesAsync(selectedLevel1);
            cmbLevel2.Items.Clear();
            cmbLevel2.Items.Add(""); // Add empty option
            foreach (var code in level2Codes)
            {
                cmbLevel2.Items.Add(code);
            }

            // Clear dependent levels
            cmbLevel3.Items.Clear();
            cmbLevel3.Items.Add("");
            cmbLevel4.Items.Clear();
            cmbLevel4.Items.Add("");
        }

        private async void cmbLevel2_TextChanged(object sender, EventArgs e)
        {
            // Debounce to avoid too many database calls
            await Task.Delay(300);
            if (codeSeriesRepository is null) return;

            var selectedLevel1 = string.IsNullOrWhiteSpace(cmbLevel1.Text) ? null : cmbLevel1.Text.Trim();
            var selectedLevel2 = string.IsNullOrWhiteSpace(cmbLevel2.Text) ? null : cmbLevel2.Text.Trim();
            
            // Clear and repopulate Level3
            var level3Codes = await codeSeriesRepository.GetLevel3CodesAsync(selectedLevel1, selectedLevel2);
            cmbLevel3.Items.Clear();
            cmbLevel3.Items.Add(""); // Add empty option
            foreach (var code in level3Codes)
            {
                cmbLevel3.Items.Add(code);
            }

            // Clear dependent level
            cmbLevel4.Items.Clear();
            cmbLevel4.Items.Add("");
        }

        private async void cmbLevel3_TextChanged(object sender, EventArgs e)
        {
            // Debounce to avoid too many database calls
            await Task.Delay(300);
            if (codeSeriesRepository is null || !chkEnableLevel4.Checked) return;

            var selectedLevel1 = string.IsNullOrWhiteSpace(cmbLevel1.Text) ? null : cmbLevel1.Text.Trim();
            var selectedLevel2 = string.IsNullOrWhiteSpace(cmbLevel2.Text) ? null : cmbLevel2.Text.Trim();
            var selectedLevel3 = string.IsNullOrWhiteSpace(cmbLevel3.Text) ? null : cmbLevel3.Text.Trim();
            
            // Clear and repopulate Level4
            var level4Codes = await codeSeriesRepository.GetLevel4CodesAsync(selectedLevel1, selectedLevel2, selectedLevel3);
            cmbLevel4.Items.Clear();
            cmbLevel4.Items.Add(""); // Add empty option
            foreach (var code in level4Codes)
            {
                cmbLevel4.Items.Add(code);
            }
        }

        private void AddCodeImportButton()
        {
            // Add a new button for code import next to the existing CSV import button
            var btnImportCodes = new Button
            {
                Text = "Import Codes",
                Size = new Size(135, 27),
                Location = new Point(180, 99),
                UseVisualStyleBackColor = true
            };
            btnImportCodes.Click += BtnImportCodes_Click;
            
            tabImport.Controls.Add(btnImportCodes);
        }

        private async void BtnImportCodes_Click(object? sender, EventArgs e)
        {
            if (codeImportService is null)
            {
                MessageBox.Show("Code import service unavailable");
                return;
            }

            using var importForm = new CodeImportForm(codeImportService);
            if (importForm.ShowDialog(this) == DialogResult.OK)
            {
                MessageBox.Show("Codes imported successfully. You can now use them in document generation.", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Refresh ALL combo boxes with new data
                await PopulateLevel1CodesAsync();
                await PopulateLevel2CodesAsync();
                await PopulateLevel3CodesAsync();
                
                // Show debug info
                await ShowDebugInfo();
            }
        }

        private async Task PopulateLevel2CodesAsync()
        {
            if (codeSeriesRepository is null) return;

            var codes = await codeSeriesRepository.GetLevel2CodesAsync(null);
            cmbLevel2.Items.Clear();
            cmbLevel2.Items.Add(""); // Add empty option
            foreach (var code in codes)
            {
                cmbLevel2.Items.Add(code);
            }
        }

        private async Task PopulateLevel3CodesAsync()
        {
            if (codeSeriesRepository is null) return;

            var codes = await codeSeriesRepository.GetLevel3CodesAsync(null, null);
            cmbLevel3.Items.Clear();
            cmbLevel3.Items.Add(""); // Add empty option
            foreach (var code in codes)
            {
                cmbLevel3.Items.Add(code);
            }
        }

        private async Task ShowDebugInfo()
        {
            if (codeSeriesRepository is null) return;

            try
            {
                var level1Codes = await codeSeriesRepository.GetLevel1CodesAsync();
                var level2Codes = await codeSeriesRepository.GetLevel2CodesAsync(null);
                var level3Codes = await codeSeriesRepository.GetLevel3CodesAsync(null, null);

                var debugMessage = $"Database Query Results:\n\n" +
                                   $"Level 1 Codes ({level1Codes.Count}): {string.Join(", ", level1Codes)}\n\n" +
                                   $"Level 2 Codes ({level2Codes.Count}): {string.Join(", ", level2Codes)}\n\n" +
                                   $"Level 3 Codes ({level3Codes.Count}): {string.Join(", ", level3Codes)}\n\n" +
                                   $"ComboBox Items:\n" +
                                   $"Level1 ComboBox: {cmbLevel1.Items.Count} items\n" +
                                   $"Level2 ComboBox: {cmbLevel2.Items.Count} items\n" +
                                   $"Level3 ComboBox: {cmbLevel3.Items.Count} items";

                MessageBox.Show(debugMessage, "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Debug error: {ex.Message}", "Debug Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool IsAlphanumeric(string value) => Regex.IsMatch(value, "^[A-Za-z0-9_-]+$");

        private bool ValidateLevels(out string message)
        {
            var level1 = cmbLevel1.Text?.Trim();
            var level2 = cmbLevel2.Text?.Trim();
            var level3 = cmbLevel3.Text?.Trim();
            var level4 = cmbLevel4.Text?.Trim();

            if (string.IsNullOrWhiteSpace(level1) || string.IsNullOrWhiteSpace(level2) || string.IsNullOrWhiteSpace(level3))
            {
                message = "Level1-3 are required.";
                return false;
            }
            if (!IsAlphanumeric(level1) || !IsAlphanumeric(level2) || !IsAlphanumeric(level3) || (chkEnableLevel4.Checked && !string.IsNullOrWhiteSpace(level4) && !IsAlphanumeric(level4)))
            {
                message = "Levels must be alphanumeric (A-Z, 0-9, _ or -).";
                return false;
            }
            if (chkEnableLevel4.Checked && string.IsNullOrWhiteSpace(level4))
            {
                message = "Level4 is enabled but empty.";
                return false;
            }
            message = string.Empty;
            return true;
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            if (documentService is null || documentConfig is null) { MessageBox.Show("Service unavailable"); return; }
            if (!ValidateLevels(out var msg)) { MessageBox.Show(msg); return; }

            var key = new CodeSeriesKey
            {
                Level1 = cmbLevel1.Text?.Trim() ?? "",
                Level2 = cmbLevel2.Text?.Trim() ?? "",
                Level3 = cmbLevel3.Text?.Trim() ?? "",
                Level4 = chkEnableLevel4.Checked ? cmbLevel4.Text?.Trim() : null
            };

            var freeText = txtFreeText.Text.Trim();
            var ext = txtExtension.Text.Trim();

            try
            {
                var result = await documentService.CreateAsync(key, freeText, Environment.UserName, null, ext);
                lblGenerateResult.Text = $"Created: {result.FileName} (audited)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Generate failed: {ex.Message}");
            }
        }

        private void PopulateImportResults(ImportResult result)
        {
            lastSummaries = result.Summaries;
            lblImportResult.Text = $"Valid: {result.Valid.Count}, Invalid: {result.Invalid.Count}";
            lstImportInvalid.Items.Clear();
            foreach (var invalid in result.Invalid)
            {
                lstImportInvalid.Items.Add($"{invalid.FileName} - {invalid.Reason}");
            }

            lvImportSummary.Items.Clear();
            foreach (var s in result.Summaries)
            {
                var seriesText = documentConfig?.EnableLevel4 == true && s.SeriesKey.Level4 is not null
                    ? $"{s.SeriesKey.Level1}{documentConfig.Separator}{s.SeriesKey.Level2}{documentConfig.Separator}{s.SeriesKey.Level3}{documentConfig.Separator}{s.SeriesKey.Level4}"
                    : $"{s.SeriesKey.Level1}{documentConfig?.Separator}{s.SeriesKey.Level2}{documentConfig?.Separator}{s.SeriesKey.Level3}";
                var item = new ListViewItem(seriesText) { Tag = s };
                item.SubItems.Add(s.MaxNumber.ToString());
                item.SubItems.Add(s.NextNumber.ToString());
                lvImportSummary.Items.Add(item);
            }
        }

        private async void btnImport_Click(object sender, EventArgs e)
        {
            if (importService is null) { MessageBox.Show("Service unavailable"); return; }
            var folder = txtImportFolder.Text.Trim();
            if (!Directory.Exists(folder)) { MessageBox.Show("Folder not found"); return; }
            var files = Directory.GetFiles(folder);
            var seeded = chkSeedCounters.Checked;
            var result = await importService.ImportFilesAsync(files, seeded);
            PopulateImportResults(result);
            lblImportNote.Text = seeded ? "Counters seeded from imported max values." : "Preview only (counters not seeded).";
        }

        private async void btnImportCsv_Click(object sender, EventArgs e)
        {
            if (importService is null) { MessageBox.Show("Service unavailable"); return; }
            var path = txtImportCsv.Text.Trim();
            if (!File.Exists(path)) { MessageBox.Show("CSV file not found"); return; }
            var lines = await File.ReadAllLinesAsync(path);
            var seeded = chkSeedCounters.Checked;
            var result = await importService.ImportFileNamesAsync(lines, seeded);
            PopulateImportResults(result);
            lblImportNote.Text = seeded ? "Counters seeded from imported max values." : "Preview only (counters not seeded).";
        }

        private async void btnInterpret_Click(object sender, EventArgs e)
        {
            if (nlqService is null) { MessageBox.Show("Service unavailable"); return; }
            var query = txtNlq.Text.Trim();
            if (string.IsNullOrWhiteSpace(query)) return;

            var result = await nlqService.InterpretAsync(query);
            if (result is null)
            {
                txtNlqResult.Text = "No structured result.";
                return;
            }

            txtNlqResult.Text = $"DocType: {result.DocumentType}\r\nOwner: {result.Owner}\r\nLevels: {result.Level1} | {result.Level2} | {result.Level3} | {result.Level4}\r\nFree: {result.FreeText}";
        }

        private async void btnRecommend_Click(object sender, EventArgs e)
        {
            if (recommendationService is null) { MessageBox.Show("Service unavailable"); return; }
            if (!ValidateLevels(out var msg)) { MessageBox.Show(msg); return; }

            var key = new CodeSeriesKey
            {
                Level1 = cmbLevel1.Text?.Trim() ?? "",
                Level2 = cmbLevel2.Text?.Trim() ?? "",
                Level3 = cmbLevel3.Text?.Trim() ?? "",
                Level4 = chkEnableLevel4.Checked ? cmbLevel4.Text?.Trim() : null
            };

            var rec = await recommendationService.RecommendAsync(key);
            lastSuggested = rec.SuggestedNext;
            lastSuggestedKey = rec.SeriesKey;
            lblRecommendResult.Text = rec.IsExisting
                ? $"Existing series. Max: {rec.ExistingMax}. Next suggested: {rec.SuggestedNext}"
                : "New series. Suggested number: 1";
        }

        private async void btnUseSuggested_Click(object sender, EventArgs e)
        {
            if (documentService is null || lastSuggestedKey is null || lastSuggested is null) { MessageBox.Show("No recommendation available."); return; }
            var freeText = txtFreeText.Text.Trim();
            var ext = txtExtension.Text.Trim();
            var confirm = MessageBox.Show($"Create using suggested number (may adjust if already taken)?\nSeries: {lastSuggestedKey.Level1}-{lastSuggestedKey.Level2}-{lastSuggestedKey.Level3}-{lastSuggestedKey.Level4}\nSuggested: {lastSuggested}", "Confirm", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;

            try
            {
                var result = await documentService.CreateAsync(lastSuggestedKey, freeText, Environment.UserName, null, ext);
                lblRecommendResult.Text = $"Created with number {result.Number} (suggested {lastSuggested}). File: {result.FileName}";
                MessageBox.Show($"Created: {result.FileName} (number {result.Number})");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Create failed: {ex.Message}");
            }
        }

        private async void btnCreateRecommended_Click(object sender, EventArgs e)
        {
            if (documentService is null) { MessageBox.Show("Service unavailable"); return; }
            if (!ValidateLevels(out var msg)) { MessageBox.Show(msg); return; }

            var key = lastSuggestedKey ?? new CodeSeriesKey
            {
                Level1 = cmbLevel1.Text?.Trim() ?? "",
                Level2 = cmbLevel2.Text?.Trim() ?? "",
                Level3 = cmbLevel3.Text?.Trim() ?? "",
                Level4 = chkEnableLevel4.Checked ? cmbLevel4.Text?.Trim() : null
            };
            var freeText = txtFreeText.Text.Trim();
            var ext = txtExtension.Text.Trim();

            try
            {
                var result = await documentService.CreateAsync(key, freeText, Environment.UserName, null, ext);
                lblRecommendResult.Text = $"Created with allocator: {result.FileName} (number {result.Number})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Create failed: {ex.Message}");
            }
        }

        private async void btnSaveSettings_Click(object sender, EventArgs e)
        {
            if (configService is null || documentConfig is null || aiSettings is null || aiOptions is null) { MessageBox.Show("Service unavailable"); return; }
            documentConfig.PaddingLength = (int)numPadding.Value;
            documentConfig.Separator = txtSeparator.Text;
            documentConfig.EnableLevel4 = chkSettingsEnableLevel4.Checked;
            documentConfig.LevelCount = documentConfig.EnableLevel4 ? 4 : 3;

            aiSettings.Provider = cmbProvider.SelectedItem?.ToString() == "Gemini" ? AI.AiProvider.Gemini : AI.AiProvider.OpenAi;
            aiSettings.OpenAiModel = txtOpenAiModel.Text.Trim();
            aiSettings.GeminiModel = txtGeminiModel.Text.Trim();

            // rebuild options (refresh keys/models)
            var refreshed = await configService.BuildAiOptionsAsync(aiSettings);
            aiOptions.DefaultProvider = refreshed.DefaultProvider;
            aiOptions.OpenAi.ApiKey = refreshed.OpenAi.ApiKey;
            aiOptions.OpenAi.Model = refreshed.OpenAi.Model;
            aiOptions.OpenAi.Endpoint = refreshed.OpenAi.Endpoint;
            aiOptions.Gemini.ApiKey = refreshed.Gemini.ApiKey;
            aiOptions.Gemini.Model = refreshed.Gemini.Model;
            aiOptions.Gemini.Endpoint = refreshed.Gemini.Endpoint;

            await configService.SaveDocumentConfigAsync(documentConfig);
            await configService.SaveAiSettingsAsync(aiSettings, txtOpenAiKey.Text, txtGeminiKey.Text);

            MessageBox.Show("Settings saved and AI clients refreshed.");
        }

        private async Task LoadAuditAsync(int page, string? action = null, string? user = null)
        {
            if (auditRepository is null) { MessageBox.Show("Service unavailable"); return; }
            if (page < 1) page = 1;
            var skip = (page - 1) * AuditPageSize;
            lvAudit.Items.Clear();
            var totalCount = await auditRepository.GetCountAsync(action, user);
            var entries = await auditRepository.GetPagedAsync(AuditPageSize, skip, action, user);
            foreach (var entry in entries)
            {
                var item = new ListViewItem(entry.CreatedAtUtc.ToLocalTime().ToString("g"));
                item.SubItems.Add(entry.CreatedBy);
                item.SubItems.Add(entry.Action);
                item.SubItems.Add(entry.Payload ?? string.Empty);
                item.SubItems.Add(entry.DocumentId?.ToString() ?? string.Empty);
                item.Tag = entry.DocumentId;
                lvAudit.Items.Add(item);
            }
            auditPage = page;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)AuditPageSize));
            lblAuditPage.Text = $"Page {auditPage} / {totalPages}";
        }

        private async void btnAuditRefresh_Click(object sender, EventArgs e)
        {
            await LoadAuditAsync(1, null, null);
        }

        private async void btnAuditFilter_Click(object sender, EventArgs e)
        {
            var action = string.IsNullOrWhiteSpace(txtAuditAction.Text) ? null : txtAuditAction.Text.Trim();
            var user = string.IsNullOrWhiteSpace(txtAuditUser.Text) ? null : txtAuditUser.Text.Trim();
            await LoadAuditAsync(1, action, user);
        }

        private async void btnAuditPrev_Click(object sender, EventArgs e)
        {
            var action = string.IsNullOrWhiteSpace(txtAuditAction.Text) ? null : txtAuditAction.Text.Trim();
            var user = string.IsNullOrWhiteSpace(txtAuditUser.Text) ? null : txtAuditUser.Text.Trim();
            var target = auditPage > 1 ? auditPage - 1 : 1;
            await LoadAuditAsync(target, action, user);
        }

        private async void btnAuditNext_Click(object sender, EventArgs e)
        {
            var action = string.IsNullOrWhiteSpace(txtAuditAction.Text) ? null : txtAuditAction.Text.Trim();
            var user = string.IsNullOrWhiteSpace(txtAuditUser.Text) ? null : txtAuditUser.Text.Trim();
            await LoadAuditAsync(auditPage + 1, action, user);
        }

        private void LoadSettingsToUi()
        {
            if (documentConfig is null || aiSettings is null) return;
            numPadding.Value = Math.Max(numPadding.Minimum, Math.Min(numPadding.Maximum, documentConfig.PaddingLength));
            txtSeparator.Text = documentConfig.Separator;
            chkEnableLevel4.Checked = documentConfig.EnableLevel4;
            chkSettingsEnableLevel4.Checked = documentConfig.EnableLevel4;
            txtOpenAiModel.Text = aiSettings.OpenAiModel;
            txtGeminiModel.Text = aiSettings.GeminiModel;
            cmbProvider.Items.Clear();
            cmbProvider.Items.AddRange(new object[] { "OpenAI", "Gemini" });
            cmbProvider.SelectedItem = aiSettings.Provider == AI.AiProvider.Gemini ? "Gemini" : "OpenAI";
        }

        private void chkEnableLevel4_CheckedChanged(object sender, EventArgs e)
        {
            cmbLevel4.Enabled = chkEnableLevel4.Checked;
            if (!chkEnableLevel4.Checked)
            {
                cmbLevel4.SelectedIndex = -1;
            }
        }

        private void chkSettingsEnableLevel4_CheckedChanged(object sender, EventArgs e)
        {
            // Mirror setting to generate tab
            chkEnableLevel4.Checked = chkSettingsEnableLevel4.Checked;
        }

        private void btnBrowseImport_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtImportFolder.Text = dlg.SelectedPath;
            }
        }

        private void btnBrowseCsv_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Filter = "CSV files|*.csv|All files|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtImportCsv.Text = dlg.FileName;
            }
        }

        private async void lvAudit_DoubleClick(object sender, EventArgs e)
        {
            if (documentRepository is null) return;
            if (lvAudit.SelectedItems.Count == 0) return;
            var docIdObj = lvAudit.SelectedItems[0].Tag;
            if (docIdObj is int docId)
            {
                var doc = await documentRepository.GetByIdAsync(docId);
                if (doc is null)
                {
                    MessageBox.Show("Document not found.");
                    return;
                }
                var level4 = string.IsNullOrWhiteSpace(doc.Level4) ? string.Empty : $"{documentConfig?.Separator}{doc.Level4}";
                MessageBox.Show($"DocId: {doc.Id}\nCode: {doc.Level1}{documentConfig?.Separator}{doc.Level2}{documentConfig?.Separator}{doc.Level3}{level4}{documentConfig?.Separator}{doc.Number}\nFile: {doc.FileName}\nBy: {doc.CreatedBy}\nAt: {doc.CreatedAtUtc.ToLocalTime():g}");
            }
        }

        private async void btnSeedSelected_Click(object sender, EventArgs e)
        {
            if (importService is null) { MessageBox.Show("Service unavailable"); return; }
            var selectedSummaries = lvImportSummary.SelectedItems
                .Cast<ListViewItem>()
                .Select(i => i.Tag as ImportSeriesSummary)
                .Where(s => s is not null)
                .Cast<ImportSeriesSummary>()
                .ToList();
            if (selectedSummaries.Count == 0)
            {
                MessageBox.Show("Select one or more series to seed.");
                return;
            }
            await importService.SeedAsync(selectedSummaries);
            MessageBox.Show($"Seeded {selectedSummaries.Count} series counters.");
        }

        private async Task LoadDocsAsync()
        {
            if (documentRepository is null) { MessageBox.Show("Service unavailable"); return; }
            lvDocs.Items.Clear();
            var docs = await documentRepository.GetRecentAsync();
            foreach (var doc in docs)
            {
                var code = string.Join(documentConfig?.Separator ?? "-", new[] { doc.Level1, doc.Level2, doc.Level3 }.Concat(string.IsNullOrWhiteSpace(doc.Level4) ? Array.Empty<string>() : new[] { doc.Level4 }).Concat(new[] { doc.Number.ToString() }));
                var item = new ListViewItem(code) { Tag = doc };
                item.SubItems.Add(doc.FileName);
                item.SubItems.Add(doc.CreatedBy);
                item.SubItems.Add(doc.CreatedAtUtc.ToLocalTime().ToString("g"));
                lvDocs.Items.Add(item);
            }
        }

        private async void btnDocsRefresh_Click(object sender, EventArgs e)
        {
            await LoadDocsAsync();
        }

        private void lvDocs_DoubleClick(object sender, EventArgs e)
        {
            if (lvDocs.SelectedItems.Count == 0) return;
            if (lvDocs.SelectedItems[0].Tag is not DocumentRecord doc) return;
            var level4 = string.IsNullOrWhiteSpace(doc.Level4) ? string.Empty : $"{documentConfig?.Separator}{doc.Level4}";
            MessageBox.Show($"DocId: {doc.Id}\nCode: {doc.Level1}{documentConfig?.Separator}{doc.Level2}{documentConfig?.Separator}{doc.Level3}{level4}{documentConfig?.Separator}{doc.Number}\nFile: {doc.FileName}\nBy: {doc.CreatedBy}\nAt: {doc.CreatedAtUtc.ToLocalTime():g}");
        }

        private async Task ShowDatabaseContents()
        {
            if (codeSeriesRepository is null) return;

            try
            {
                // Use the repository methods instead of direct database access
                var level1Codes = await codeSeriesRepository.GetLevel1CodesAsync();
                var level2Codes = await codeSeriesRepository.GetLevel2CodesAsync(null);
                var level3Codes = await codeSeriesRepository.GetLevel3CodesAsync(null, null);
                
                var results = new List<string>();
                
                foreach (var code in level1Codes)
                {
                    results.Add($"Level1: '{code}'");
                }
                
                foreach (var code in level2Codes)
                {
                    results.Add($"Level2: '{code}'");
                }
                
                foreach (var code in level3Codes)
                {
                    results.Add($"Level3: '{code}'");
                }
                
                var message = "Database Contents via Repository:\n\n" + string.Join("\n", results);
                MessageBox.Show(message, "Database Contents", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database check error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
