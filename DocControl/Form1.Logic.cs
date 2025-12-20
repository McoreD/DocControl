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
        private int? lastSuggested;
        private CodeSeriesKey? lastSuggestedKey;
        private int auditPage = 1;
        private const int AuditPageSize = 50;
        private IReadOnlyList<ImportSeriesSummary> lastSummaries = Array.Empty<ImportSeriesSummary>();

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(DocumentService documentService, ImportService importService, NlqService nlqService, ConfigService configService, DocumentConfig documentConfig, AiSettings aiSettings, AiClientOptions aiOptions, AuditRepository auditRepository, RecommendationService recommendationService, DocumentRepository documentRepository, CodeImportService codeImportService)
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

            LoadSettingsToUi();
            AddCodeImportButton();
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

        private void BtnImportCodes_Click(object? sender, EventArgs e)
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
            }
        }

        private static bool IsAlphanumeric(string value) => Regex.IsMatch(value, "^[A-Za-z0-9_-]+$");

        private bool ValidateLevels(out string message)
        {
            if (string.IsNullOrWhiteSpace(txtLevel1.Text) || string.IsNullOrWhiteSpace(txtLevel2.Text) || string.IsNullOrWhiteSpace(txtLevel3.Text))
            {
                message = "Level1-3 are required.";
                return false;
            }
            if (!IsAlphanumeric(txtLevel1.Text) || !IsAlphanumeric(txtLevel2.Text) || !IsAlphanumeric(txtLevel3.Text) || (chkEnableLevel4.Checked && !string.IsNullOrWhiteSpace(txtLevel4.Text) && !IsAlphanumeric(txtLevel4.Text)))
            {
                message = "Levels must be alphanumeric (A-Z, 0-9, _ or -).";
                return false;
            }
            if (chkEnableLevel4.Checked && string.IsNullOrWhiteSpace(txtLevel4.Text))
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
                Level1 = txtLevel1.Text.Trim(),
                Level2 = txtLevel2.Text.Trim(),
                Level3 = txtLevel3.Text.Trim(),
                Level4 = chkEnableLevel4.Checked ? txtLevel4.Text.Trim() : null
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
                Level1 = txtLevel1.Text.Trim(),
                Level2 = txtLevel2.Text.Trim(),
                Level3 = txtLevel3.Text.Trim(),
                Level4 = chkEnableLevel4.Checked ? txtLevel4.Text.Trim() : null
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
                Level1 = txtLevel1.Text.Trim(),
                Level2 = txtLevel2.Text.Trim(),
                Level3 = txtLevel3.Text.Trim(),
                Level4 = chkEnableLevel4.Checked ? txtLevel4.Text.Trim() : null
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
            txtLevel4.Enabled = chkEnableLevel4.Checked;
            if (!chkEnableLevel4.Checked)
            {
                txtLevel4.Text = string.Empty;
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
    }
}
