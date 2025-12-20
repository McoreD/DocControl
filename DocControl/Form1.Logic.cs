using System.IO;
using DocControl.AI;
using DocControl.Configuration;
using DocControl.Forms;
using DocControl.Models;
using DocControl.Presentation;
using DocControl.Services;

namespace DocControl
{
    public partial class Form1
    {
        private readonly MainController controller = null!;
        private readonly DocumentConfig documentConfig = null!;
        private readonly AiSettings aiSettings = null!;
        private readonly AiClientOptions aiOptions = null!;
        private int? lastSuggested;
        private CodeSeriesKey? lastSuggestedKey;
        private int auditPage = 1;
        private const int AuditPageSize = 50;
        private IReadOnlyList<ImportSeriesSummary> lastSummaries = Array.Empty<ImportSeriesSummary>();

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(MainController controller, DocumentConfig documentConfig, AiSettings aiSettings, AiClientOptions aiOptions)
            : this()
        {
            this.controller = controller;
            this.documentConfig = documentConfig;
            this.aiSettings = aiSettings;
            this.aiOptions = aiOptions;

            LoadSettingsToUi();
            AddCodeImportButton();
            LoadCodesAsync();
        }

        private async void LoadCodesAsync()
        {
            try
            {
                await PopulateLevel1CodesAsync();
                await PopulateLevel2CodesAsync();
                await PopulateLevel3CodesAsync();

                if (chkEnableLevel4.Checked)
                {
                    await PopulateLevel4CodesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load codes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task PopulateLevel1CodesAsync()
        {
            var codes = await controller.GetLevel1CodesAsync().ConfigureAwait(true);
            cmbLevel1.Items.Clear();
            cmbLevel1.Items.Add(string.Empty);
            foreach (var code in codes)
            {
                cmbLevel1.Items.Add(code);
            }
        }

        private async void cmbLevel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            await PopulateLevel2CodesAsync();
            await PopulateLevel3CodesAsync();
            cmbLevel4.Items.Clear();
            cmbLevel4.Items.Add(string.Empty);
        }

        private async void cmbLevel2_SelectedIndexChanged(object sender, EventArgs e)
        {
            await PopulateLevel3CodesAsync();
            cmbLevel4.Items.Clear();
            cmbLevel4.Items.Add(string.Empty);
        }

        private async void cmbLevel3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!chkEnableLevel4.Checked) return;
            await PopulateLevel4CodesAsync();
        }

        private async void cmbLevel1_TextChanged(object sender, EventArgs e)
        {
            await Task.Delay(300);
            await PopulateLevel2CodesAsync();
            await PopulateLevel3CodesAsync();
            cmbLevel4.Items.Clear();
            cmbLevel4.Items.Add(string.Empty);
        }

        private async void cmbLevel2_TextChanged(object sender, EventArgs e)
        {
            await Task.Delay(300);
            await PopulateLevel3CodesAsync();
            cmbLevel4.Items.Clear();
            cmbLevel4.Items.Add(string.Empty);
        }

        private async void cmbLevel3_TextChanged(object sender, EventArgs e)
        {
            await Task.Delay(300);
            if (!chkEnableLevel4.Checked) return;
            await PopulateLevel4CodesAsync();
        }

        private async Task PopulateLevel2CodesAsync()
        {
            var level1 = cmbLevel1.Text?.Trim();
            var codes = await controller.GetLevel2CodesAsync(level1, CancellationToken.None).ConfigureAwait(true);
            cmbLevel2.Items.Clear();
            cmbLevel2.Items.Add(string.Empty);
            foreach (var code in codes)
            {
                cmbLevel2.Items.Add(code);
            }
        }

        private async Task PopulateLevel3CodesAsync()
        {
            var level1 = cmbLevel1.Text?.Trim();
            var level2 = cmbLevel2.Text?.Trim();
            var codes = await controller.GetLevel3CodesAsync(level1, level2, CancellationToken.None).ConfigureAwait(true);
            cmbLevel3.Items.Clear();
            cmbLevel3.Items.Add(string.Empty);
            foreach (var code in codes)
            {
                cmbLevel3.Items.Add(code);
            }
        }

        private async Task PopulateLevel4CodesAsync()
        {
            var level1 = cmbLevel1.Text?.Trim();
            var level2 = cmbLevel2.Text?.Trim();
            var level3 = cmbLevel3.Text?.Trim();
            var codes = await controller.GetLevel4CodesAsync(level1, level2, level3, CancellationToken.None).ConfigureAwait(true);
            cmbLevel4.Items.Clear();
            cmbLevel4.Items.Add(string.Empty);
            foreach (var code in codes)
            {
                cmbLevel4.Items.Add(code);
            }
        }

        private void AddCodeImportButton()
        {
            var btnImportCodes = new Button
            {
                Text = "Import Codes",
                Size = new Size(135, 27),
                Location = new Point(180, 99),
                UseVisualStyleBackColor = true
            };
            btnImportCodes.Click += BtnImportCodes_Click;

            var btnClearDocuments = new Button
            {
                Text = "Clear documents",
                Size = new Size(135, 27),
                Location = new Point(325, 99),
                UseVisualStyleBackColor = true
            };
            btnClearDocuments.Click += BtnClearDocuments_Click;

            tabImport.Controls.Add(btnImportCodes);
            tabImport.Controls.Add(btnClearDocuments);
        }

        private async void BtnClearDocuments_Click(object? sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "This will delete ALL document entries (file names/history) and audit entries. Codes will NOT be deleted.\n\nContinue?",
                "Confirm clear",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                await controller.ClearDocumentsAsync().ConfigureAwait(true);

                lvDocs.Items.Clear();
                lvAudit.Items.Clear();
                lblImportResult.Text = string.Empty;
                lblImportNote.Text = string.Empty;
                lstImportInvalid.Items.Clear();
                lvImportSummary.Items.Clear();

                MessageBox.Show("Documents/audit history cleared.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Clear failed: {ex.Message}");
            }
        }

        private async void BtnImportCodes_Click(object? sender, EventArgs e)
        {
            using var importForm = new CodeImportForm(controller);
            if (importForm.ShowDialog(this) == DialogResult.OK)
            {
                MessageBox.Show("Codes imported successfully. You can now use them in document generation.", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await PopulateLevel1CodesAsync();
                await PopulateLevel2CodesAsync();
                await PopulateLevel3CodesAsync();
            }
        }

        private CodeSeriesKey BuildKeyFromInputs()
        {
            return new CodeSeriesKey
            {
                Level1 = cmbLevel1.Text?.Trim() ?? string.Empty,
                Level2 = cmbLevel2.Text?.Trim() ?? string.Empty,
                Level3 = cmbLevel3.Text?.Trim() ?? string.Empty,
                Level4 = chkEnableLevel4.Checked ? cmbLevel4.Text?.Trim() : null
            };
        }

        private bool ValidateLevels(out string message)
        {
            var key = BuildKeyFromInputs();
            var result = controller.ValidateLevels(key, chkEnableLevel4.Checked);
            message = result.Message;
            return result.IsValid;
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            if (!ValidateLevels(out var msg))
            {
                MessageBox.Show(msg);
                return;
            }

            var key = BuildKeyFromInputs();

            var freeText = txtFreeText.Text.Trim();
            var ext = txtExtension.Text.Trim();

            try
            {
                var result = await controller.GenerateDocumentAsync(key, freeText, Environment.UserName, ext).ConfigureAwait(true);
                lblGenerateResult.Text = $"Created: {result.FileName} (audited)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Generate failed: {ex.Message}");
            }
        }

        private async void btnImportCsv_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "CSV files|*.csv|Text files|*.txt|All files|*.*",
                Title = "Select CSV file (code + name)"
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            var lines = await File.ReadAllLinesAsync(dlg.FileName).ConfigureAwait(true);
            var outcome = await controller.ImportDocumentsAsync(lines, Environment.UserName).ConfigureAwait(true);

            await LoadDocsAsync();

            lblImportResult.Text = $"Valid: {outcome.ValidCount}, Invalid: {outcome.InvalidCount}";
            lstImportInvalid.Items.Clear();
            foreach (var error in outcome.Errors.Take(50))
            {
                lstImportInvalid.Items.Add(error);
            }

            lastSummaries = outcome.Summaries;
            lvImportSummary.Items.Clear();
            foreach (var s in outcome.Summaries)
            {
                var seriesText = documentConfig.EnableLevel4 && s.SeriesKey.Level4 is not null
                    ? $"{s.SeriesKey.Level1}{documentConfig.Separator}{s.SeriesKey.Level2}{documentConfig.Separator}{s.SeriesKey.Level3}{documentConfig.Separator}{s.SeriesKey.Level4}"
                    : $"{s.SeriesKey.Level1}{documentConfig.Separator}{s.SeriesKey.Level2}{documentConfig.Separator}{s.SeriesKey.Level3}";
                var item = new ListViewItem(seriesText) { Tag = s };
                item.SubItems.Add(s.MaxNumber.ToString());
                item.SubItems.Add(s.NextNumber.ToString());
                lvImportSummary.Items.Add(item);
            }

            var msg = $"Imported {outcome.ValidCount} rows. Invalid: {outcome.InvalidCount}.";
            if (outcome.Errors.Count > 0)
            {
                msg += "\n\nFirst errors:\n" + string.Join("\n", outcome.Errors.Take(10));
                if (outcome.Errors.Count > 10) msg += $"\n...and {outcome.Errors.Count - 10} more.";
            }

            MessageBox.Show(msg);
        }

        private async void btnInterpret_Click(object sender, EventArgs e)
        {
            var query = txtNlq.Text.Trim();
            if (string.IsNullOrWhiteSpace(query)) return;

            var result = await controller.InterpretAsync(query).ConfigureAwait(true);
            if (result is null)
            {
                txtNlqResult.Text = "No structured result.";
                return;
            }

            txtNlqResult.Text = $"DocType: {result.DocumentType}\r\nOwner: {result.Owner}\r\nLevels: {result.Level1} | {result.Level2} | {result.Level3} | {result.Level4}\r\nFree: {result.FreeText}";
        }

        private async void btnRecommend_Click(object sender, EventArgs e)
        {
            if (!ValidateLevels(out var msg))
            {
                MessageBox.Show(msg);
                return;
            }

            var key = BuildKeyFromInputs();

            var rec = await controller.RecommendAsync(key).ConfigureAwait(true);
            lastSuggested = rec.SuggestedNext;
            lastSuggestedKey = rec.SeriesKey;
            lblRecommendResult.Text = rec.IsExisting
                ? $"Existing series. Max: {rec.ExistingMax}. Next suggested: {rec.SuggestedNext}"
                : "New series. Suggested number: 1";
        }

        private async void btnUseSuggested_Click(object sender, EventArgs e)
        {
            if (lastSuggestedKey is null || lastSuggested is null)
            {
                MessageBox.Show("No recommendation available.");
                return;
            }

            var freeText = txtFreeText.Text.Trim();
            var ext = txtExtension.Text.Trim();
            var confirm = MessageBox.Show($"Create using suggested number (may adjust if already taken)?\nSeries: {lastSuggestedKey.Level1}-{lastSuggestedKey.Level2}-{lastSuggestedKey.Level3}-{lastSuggestedKey.Level4}\nSuggested: {lastSuggested}", "Confirm", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;

            try
            {
                var result = await controller.GenerateDocumentAsync(lastSuggestedKey, freeText, Environment.UserName, ext).ConfigureAwait(true);
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
            if (!ValidateLevels(out var msg))
            {
                MessageBox.Show(msg);
                return;
            }

            var key = lastSuggestedKey ?? BuildKeyFromInputs();
            var freeText = txtFreeText.Text.Trim();
            var ext = txtExtension.Text.Trim();

            try
            {
                var result = await controller.GenerateDocumentAsync(key, freeText, Environment.UserName, ext).ConfigureAwait(true);
                lblRecommendResult.Text = $"Created with allocator: {result.FileName} (number {result.Number})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Create failed: {ex.Message}");
            }
        }

        private async void btnSaveSettings_Click(object sender, EventArgs e)
        {
            var provider = cmbProvider.SelectedItem?.ToString() == "Gemini" ? AiProvider.Gemini : AiProvider.OpenAi;

            var state = new SettingsState(
                (int)numPadding.Value,
                txtSeparator.Text,
                chkSettingsEnableLevel4.Checked,
                provider,
                txtOpenAiModel.Text.Trim(),
                txtGeminiModel.Text.Trim());

            try
            {
                await controller.SaveSettingsAsync(state, txtOpenAiKey.Text, txtGeminiKey.Text).ConfigureAwait(true);
                MessageBox.Show("Settings saved and AI clients refreshed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}");
            }
        }

        private async Task LoadAuditAsync(int page, string? action = null, string? user = null)
        {
            var result = await controller.LoadAuditPageAsync(page, AuditPageSize, action, user).ConfigureAwait(true);

            lvAudit.Items.Clear();
            foreach (var entry in result.Entries)
            {
                var item = new ListViewItem(entry.CreatedAtUtc.ToLocalTime().ToString("g"));
                item.SubItems.Add(entry.CreatedBy);
                item.SubItems.Add(entry.Action);
                item.SubItems.Add(entry.Payload ?? string.Empty);
                item.SubItems.Add(entry.DocumentId?.ToString() ?? string.Empty);
                item.Tag = entry.DocumentId;
                lvAudit.Items.Add(item);
            }

            auditPage = result.Page;
            lblAuditPage.Text = $"Page {result.Page} / {result.TotalPages}";
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
            numPadding.Value = Math.Max(numPadding.Minimum, Math.Min(numPadding.Maximum, documentConfig.PaddingLength));
            txtSeparator.Text = documentConfig.Separator;
            chkEnableLevel4.Checked = documentConfig.EnableLevel4;
            chkSettingsEnableLevel4.Checked = documentConfig.EnableLevel4;
            txtOpenAiModel.Text = aiSettings.OpenAiModel;
            txtGeminiModel.Text = aiSettings.GeminiModel;
            cmbProvider.Items.Clear();
            cmbProvider.Items.AddRange(new object[] { "OpenAI", "Gemini" });
            cmbProvider.SelectedItem = aiSettings.Provider == AiProvider.Gemini ? "Gemini" : "OpenAI";
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
            chkEnableLevel4.Checked = chkSettingsEnableLevel4.Checked;
        }

        private async void lvAudit_DoubleClick(object sender, EventArgs e)
        {
            if (lvAudit.SelectedItems.Count == 0) return;
            var docIdObj = lvAudit.SelectedItems[0].Tag;
            if (docIdObj is int docId)
            {
                var doc = await controller.GetDocumentAsync(docId).ConfigureAwait(true);
                if (doc is null)
                {
                    MessageBox.Show("Document not found.");
                    return;
                }

                var level4 = string.IsNullOrWhiteSpace(doc.Level4) ? string.Empty : $"{documentConfig.Separator}{doc.Level4}";
                MessageBox.Show($"DocId: {doc.Id}\nCode: {doc.Level1}{documentConfig.Separator}{doc.Level2}{documentConfig.Separator}{doc.Level3}{level4}{documentConfig.Separator}{doc.Number}\nFile: {doc.FileName}\nBy: {doc.CreatedBy}\nAt: {doc.CreatedAtUtc.ToLocalTime():g}");
            }
        }

        private async void btnSeedSelected_Click(object sender, EventArgs e)
        {
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

            await controller.SeedSeriesAsync(selectedSummaries).ConfigureAwait(true);
            MessageBox.Show($"Seeded {selectedSummaries.Count} series counters.");
        }

        private async Task LoadDocsAsync()
        {
            lvDocs.Items.Clear();
            var docs = await controller.LoadRecentDocumentsAsync().ConfigureAwait(true);
            foreach (var doc in docs)
            {
                var sep = documentConfig.Separator;
                var pad = documentConfig.PaddingLength;
                var num = doc.Number.ToString().PadLeft(pad, '0');

                var parts = new List<string> { doc.Level1, doc.Level2, doc.Level3 };
                if (documentConfig.EnableLevel4 && !string.IsNullOrWhiteSpace(doc.Level4))
                {
                    parts.Add(doc.Level4);
                }
                parts.Add(num);

                var code = string.Join(sep, parts);

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
            var level4 = string.IsNullOrWhiteSpace(doc.Level4) ? string.Empty : $"{documentConfig.Separator}{doc.Level4}";
            MessageBox.Show($"DocId: {doc.Id}\nCode: {doc.Level1}{documentConfig.Separator}{doc.Level2}{documentConfig.Separator}{doc.Level3}{level4}{documentConfig.Separator}{doc.Number}\nFile: {doc.FileName}\nBy: {doc.CreatedBy}\nAt: {doc.CreatedAtUtc.ToLocalTime():g}");
        }

        private async void btnCodesImportCsv_Click(object sender, EventArgs e)
        {
            using var importForm = new CodeImportForm(controller);
            if (importForm.ShowDialog(this) == DialogResult.OK)
            {
                await LoadCodesDisplayAsync();
                MessageBox.Show("Codes imported successfully.", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await PopulateLevel1CodesAsync();
                await PopulateLevel2CodesAsync();
                await PopulateLevel3CodesAsync();
            }
        }

        private async void btnCodesRefresh_Click(object sender, EventArgs e)
        {
            await LoadCodesDisplayAsync();
        }

        private async Task LoadCodesDisplayAsync()
        {
            lvCodes.Items.Clear();

            try
            {
                var items = await controller.LoadCodesDisplayAsync(documentConfig.EnableLevel4).ConfigureAwait(true);
                foreach (var item in items)
                {
                    var listViewItem = new ListViewItem(item.Level.ToString());
                    listViewItem.SubItems.Add(item.Code);
                    listViewItem.SubItems.Add(item.Description);
                    lvCodes.Items.Add(listViewItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load codes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
