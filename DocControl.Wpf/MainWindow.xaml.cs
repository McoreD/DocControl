using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DocControl.AI;
using DocControl.Core.Configuration;
using DocControl.Core.Models;
using DocControl.Infrastructure.Presentation;
using DocControl.Infrastructure.Services;
using Microsoft.Win32;

namespace DocControl.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly MainController controller;
        private readonly DocumentConfig documentConfig;
        private readonly AiSettings aiSettings;
        private readonly AiClientOptions aiOptions;
        private List<ImportSeriesSummary> lastSummaries = new();
        private int auditPage = 1;
        private const int AuditPageSize = 50;
        private string? auditFilterUser;
        private string? auditFilterAction;
        private bool isPopulatingDropdowns = false;

        public MainWindow(MainController controller, DocumentConfig documentConfig, AiSettings aiSettings, AiClientOptions aiOptions)
        {
            InitializeComponent();
            this.controller = controller;
            this.documentConfig = documentConfig;
            this.aiSettings = aiSettings;
            this.aiOptions = aiOptions;
            
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            chkEnableLevel4.IsChecked = documentConfig.EnableLevel4;
            chkSettingsEnableLevel4.IsChecked = documentConfig.EnableLevel4;
            txtSeparator.Text = documentConfig.Separator;
            txtPadding.Text = documentConfig.PaddingLength.ToString();
            
            cmbProvider.SelectedIndex = aiSettings.Provider == AiProvider.Gemini ? 1 : 0;
            txtOpenAiModel.Text = aiSettings.OpenAiModel;
            txtGeminiModel.Text = aiSettings.GeminiModel;

            await PopulateLevel1CodesAsync();
            await PopulateLevel2CodesAsync();
            await PopulateLevel3CodesAsync();
            await LoadCodesDisplayAsync();
            await LoadDocsAsync();
            await LoadAuditAsync();
        }

        private async Task PopulateLevel1CodesAsync()
        {
            var codes = await controller.GetLevel1CodesAsync();
            cmbLevel1.Items.Clear();
            foreach (var code in codes)
            {
                cmbLevel1.Items.Add(code);
            }
        }

        private async Task PopulateLevel2CodesAsync()
        {
            var level1 = cmbLevel1.Text?.Trim();
            var currentSelection = cmbLevel2.Text?.Trim();
            
            var codes = await controller.GetLevel2CodesAsync(level1);
            
            isPopulatingDropdowns = true;
            cmbLevel2.Items.Clear();
            foreach (var code in codes)
            {
                cmbLevel2.Items.Add(code);
            }
            
            if (!string.IsNullOrWhiteSpace(currentSelection) && cmbLevel2.Items.Contains(currentSelection))
            {
                cmbLevel2.Text = currentSelection;
            }
            isPopulatingDropdowns = false;
        }

        private async Task PopulateLevel3CodesAsync()
        {
            var level1 = cmbLevel1.Text?.Trim();
            var level2 = cmbLevel2.Text?.Trim();
            var currentSelection = cmbLevel3.Text?.Trim();

            var codes = await controller.GetLevel3CodesAsync(level1, level2);
            
            isPopulatingDropdowns = true;
            cmbLevel3.Items.Clear();
            foreach (var code in codes)
            {
                cmbLevel3.Items.Add(code);
            }
            
            if (!string.IsNullOrWhiteSpace(currentSelection) && cmbLevel3.Items.Contains(currentSelection))
            {
                cmbLevel3.Text = currentSelection;
            }
            isPopulatingDropdowns = false;
        }

        private async Task PopulateLevel4CodesAsync()
        {
            var level1 = cmbLevel1.Text?.Trim();
            var level2 = cmbLevel2.Text?.Trim();
            var level3 = cmbLevel3.Text?.Trim();
            var currentSelection = cmbLevel4.Text?.Trim();

            var codes = await controller.GetLevel4CodesAsync(level1, level2, level3);
            
            isPopulatingDropdowns = true;
            cmbLevel4.Items.Clear();
            foreach (var code in codes)
            {
                cmbLevel4.Items.Add(code);
            }
            
            if (!string.IsNullOrWhiteSpace(currentSelection) && cmbLevel4.Items.Contains(currentSelection))
            {
                cmbLevel4.Text = currentSelection;
            }
            isPopulatingDropdowns = false;
        }

        private async void cmbLevel1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isPopulatingDropdowns) return;
            await PopulateLevel2CodesAsync();
        }

        private async void cmbLevel2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isPopulatingDropdowns) return;
            await PopulateLevel3CodesAsync();
        }

        private async void cmbLevel3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isPopulatingDropdowns) return;
            if (chkEnableLevel4.IsChecked == true)
            {
                await PopulateLevel4CodesAsync();
            }
        }

        private void chkEnableLevel4_Changed(object sender, RoutedEventArgs e)
        {
            cmbLevel4.IsEnabled = chkEnableLevel4.IsChecked == true;
        }

        private CodeSeriesKey BuildKeyFromInputs()
        {
            return new CodeSeriesKey
            {
                Level1 = cmbLevel1.Text?.Trim() ?? string.Empty,
                Level2 = cmbLevel2.Text?.Trim() ?? string.Empty,
                Level3 = cmbLevel3.Text?.Trim() ?? string.Empty,
                Level4 = chkEnableLevel4.IsChecked == true ? cmbLevel4.Text?.Trim() : null
            };
        }

        private bool ValidateLevels(out string message)
        {
            var key = BuildKeyFromInputs();
            var result = controller.ValidateLevels(key, chkEnableLevel4.IsChecked == true);
            message = result.Message;
            return result.IsValid;
        }

        private async void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateLevels(out var msg))
            {
                MessageBox.Show(msg, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var key = BuildKeyFromInputs();
            var freeText = txtFreeText.Text.Trim();
            var ext = txtExtension.Text.Trim();

            try
            {
                var result = await controller.GenerateDocumentAsync(key, freeText, Environment.UserName, ext);
                lblGenerateResult.Text = $"Created: {result.FileName} (audited)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Generate failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadCodesDisplayAsync()
        {
            lvCodes.Items.Clear();

            try
            {
                var items = await controller.LoadCodesDisplayAsync(documentConfig.EnableLevel4);
                foreach (var item in items)
                {
                    lvCodes.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load codes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnCodesImportCsv_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "CSV files|*.csv|Text files|*.txt|All files|*.*",
                Title = "Select CSV file with codes"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                var csvContent = await File.ReadAllTextAsync(dialog.FileName);
                var result = await controller.ImportCodesAsync(csvContent);
                
                var errorMsg = result.HasErrors ? $"\nErrors: {result.Errors.Count}" : "";
                MessageBox.Show($"Codes imported successfully.\nSuccess: {result.SuccessCount}{errorMsg}", 
                    "Import Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                
                await LoadCodesDisplayAsync();
                await PopulateLevel1CodesAsync();
                await PopulateLevel2CodesAsync();
                await PopulateLevel3CodesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Import failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnCodesRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadCodesDisplayAsync();
        }

        private async void btnImportCsv_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "CSV files|*.csv|Text files|*.txt|All files|*.*",
                Title = "Select CSV file (code + name)"
            };

            if (dialog.ShowDialog() != true) return;

            var lines = await File.ReadAllLinesAsync(dialog.FileName);
            var outcome = await controller.ImportDocumentsAsync(lines, Environment.UserName);

            await LoadDocsAsync();

            lblImportResult.Text = $"Valid: {outcome.ValidCount}, Invalid: {outcome.InvalidCount}";
            lstImportInvalid.Items.Clear();
            foreach (var error in outcome.Errors.Take(50))
            {
                lstImportInvalid.Items.Add(error);
            }

            lastSummaries = outcome.Summaries.ToList();
            lvImportSummary.Items.Clear();
            foreach (var s in outcome.Summaries)
            {
                var seriesText = documentConfig.EnableLevel4 && s.SeriesKey.Level4 is not null
                    ? $"{s.SeriesKey.Level1}{documentConfig.Separator}{s.SeriesKey.Level2}{documentConfig.Separator}{s.SeriesKey.Level3}{documentConfig.Separator}{s.SeriesKey.Level4}"
                    : $"{s.SeriesKey.Level1}{documentConfig.Separator}{s.SeriesKey.Level2}{documentConfig.Separator}{s.SeriesKey.Level3}";
                
                lvImportSummary.Items.Add(new
                {
                    SeriesText = seriesText,
                    MaxNumber = s.MaxNumber,
                    NextNumber = s.NextNumber,
                    Summary = s
                });
            }

            var msg = $"Imported {outcome.ValidCount} rows. Invalid: {outcome.InvalidCount}.";
            if (outcome.Errors.Count > 0)
            {
                msg += $"\nFirst {Math.Min(50, outcome.Errors.Count)} errors listed below.";
            }
            MessageBox.Show(msg, "Import Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void btnSeedSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = lvImportSummary.SelectedItems.Cast<dynamic>().ToList();
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Select one or more series to seed.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedSummaries = selectedItems.Select(i => (ImportSeriesSummary)i.Summary).ToList();
            await controller.SeedSeriesAsync(selectedSummaries);
            MessageBox.Show($"Seeded {selectedSummaries.Count} series counters.", "Seed Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void btnInterpret_Click(object sender, RoutedEventArgs e)
        {
            var query = txtNlq.Text.Trim();
            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Please enter a natural language query.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var result = await controller.InterpretAsync(query);
                if (result != null)
                {
                    txtNlqResult.Text = $"Document Type: {result.DocumentType}\nOwner: {result.Owner}\n" +
                                       $"Levels: {result.Level1}/{result.Level2}/{result.Level3}/{result.Level4}\n" +
                                       $"Free Text: {result.FreeText}";
                }
                else
                {
                    txtNlqResult.Text = "No interpretation available.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Interpretation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnRecommend_Click(object sender, RoutedEventArgs e)
        {
            var query = txtNlq.Text.Trim();
            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Please enter a natural language query.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var key = BuildKeyFromInputs();
                var recommendation = await controller.RecommendAsync(key);
                
                var recKey = recommendation.SeriesKey;
                lblRecommendResult.Text = $"Recommended: {recKey.Level1}-{recKey.Level2}-{recKey.Level3} (Next: {recommendation.SuggestedNext})";
                
                cmbLevel1.Text = recKey.Level1;
                await PopulateLevel2CodesAsync();
                cmbLevel2.Text = recKey.Level2;
                await PopulateLevel3CodesAsync();
                cmbLevel3.Text = recKey.Level3;
                
                if (documentConfig.EnableLevel4 && !string.IsNullOrWhiteSpace(recKey.Level4))
                {
                    await PopulateLevel4CodesAsync();
                    cmbLevel4.Text = recKey.Level4;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Recommendation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUseSuggested_Click(object sender, RoutedEventArgs e)
        {
            btnRecommend_Click(sender, e);
        }

        private void btnCreateRecommended_Click(object sender, RoutedEventArgs e)
        {
            btnGenerate_Click(sender, e);
        }

        private async void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var state = new SettingsState(
                    int.TryParse(txtPadding.Text, out var padding) ? padding : documentConfig.PaddingLength,
                    txtSeparator.Text,
                    chkSettingsEnableLevel4.IsChecked == true,
                    cmbProvider.SelectedIndex == 0 ? AiProvider.OpenAi : AiProvider.Gemini,
                    txtOpenAiModel.Text,
                    txtGeminiModel.Text);

                await controller.SaveSettingsAsync(state, string.Empty, string.Empty);
                
                documentConfig.Separator = state.Separator;
                documentConfig.PaddingLength = state.PaddingLength;
                documentConfig.EnableLevel4 = state.EnableLevel4;
                
                MessageBox.Show("Settings saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadAuditAsync()
        {
            lvAudit.Items.Clear();
            
            try
            {
                var result = await controller.LoadAuditPageAsync(auditPage, AuditPageSize, auditFilterAction, auditFilterUser);
                foreach (var entry in result.Entries)
                {
                    lvAudit.Items.Add(new
                    {
                        Timestamp = entry.CreatedAtUtc.ToLocalTime().ToString("g"),
                        User = entry.CreatedBy,
                        Action = entry.Action,
                        DocumentId = entry.DocumentId?.ToString() ?? "",
                        Payload = entry.Payload ?? ""
                    });
                }
                
                lblAuditPage.Text = $"Page {result.Page} of {result.TotalPages}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load audit entries: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnAuditRefresh_Click(object sender, RoutedEventArgs e)
        {
            auditPage = 1;
            await LoadAuditAsync();
        }

        private async void btnAuditFilter_Click(object sender, RoutedEventArgs e)
        {
            auditFilterUser = string.IsNullOrWhiteSpace(txtAuditUser.Text) ? null : txtAuditUser.Text.Trim();
            auditFilterAction = string.IsNullOrWhiteSpace(txtAuditAction.Text) ? null : txtAuditAction.Text.Trim();
            auditPage = 1;
            await LoadAuditAsync();
        }

        private async void btnAuditPrev_Click(object sender, RoutedEventArgs e)
        {
            if (auditPage > 1)
            {
                auditPage--;
                await LoadAuditAsync();
            }
        }

        private async void btnAuditNext_Click(object sender, RoutedEventArgs e)
        {
            auditPage++;
            await LoadAuditAsync();
        }

        private async Task LoadDocsAsync()
        {
            lvDocs.Items.Clear();
            
            string? level1Filter = string.IsNullOrWhiteSpace(txtDocsFilterLevel1.Text) ? null : txtDocsFilterLevel1.Text.Trim();
            string? level2Filter = string.IsNullOrWhiteSpace(txtDocsFilterLevel2.Text) ? null : txtDocsFilterLevel2.Text.Trim();
            string? level3Filter = string.IsNullOrWhiteSpace(txtDocsFilterLevel3.Text) ? null : txtDocsFilterLevel3.Text.Trim();
            string? fileNameFilter = string.IsNullOrWhiteSpace(txtDocsFilterFileName.Text) ? null : txtDocsFilterFileName.Text.Trim();
            
            var docs = await controller.LoadFilteredDocumentsAsync(level1Filter, level2Filter, level3Filter, fileNameFilter);
            
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

                lvDocs.Items.Add(new
                {
                    Code = code,
                    FileName = doc.FileName,
                    CreatedBy = doc.CreatedBy,
                    CreatedAt = doc.CreatedAtUtc.ToLocalTime().ToString("g"),
                    Document = doc
                });
            }
            
            // Update result count label
            if (level1Filter != null || level2Filter != null || level3Filter != null || fileNameFilter != null)
            {
                lblDocsResultCount.Text = $"Found {docs.Count} document(s)";
            }
            else
            {
                lblDocsResultCount.Text = $"Showing {docs.Count} recent document(s)";
            }
        }

        private async void btnDocsRefresh_Click(object sender, RoutedEventArgs e)
        {
            // Clear filters and reload all recent documents
            txtDocsFilterLevel1.Text = string.Empty;
            txtDocsFilterLevel2.Text = string.Empty;
            txtDocsFilterLevel3.Text = string.Empty;
            txtDocsFilterFileName.Text = string.Empty;
            await LoadDocsAsync();
        }

        private async void btnDocsFilter_Click(object sender, RoutedEventArgs e)
        {
            await LoadDocsAsync();
        }

        private void lvDocs_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = lvDocs.SelectedItem;
            if (item == null) return;
            
            var itemType = item.GetType();
            var docProperty = itemType.GetProperty("Document");
            if (docProperty == null) return;
            
            var doc = docProperty.GetValue(item) as DocumentRecord;
            if (doc == null) return;
            
            var level4 = string.IsNullOrWhiteSpace(doc.Level4) ? string.Empty : $"{documentConfig.Separator}{doc.Level4}";
            var code = $"{doc.Level1}{documentConfig.Separator}{doc.Level2}{documentConfig.Separator}{doc.Level3}{level4}{documentConfig.Separator}{doc.Number}";
            
            MessageBox.Show(
                $"DocId: {doc.Id}\nCode: {code}\nFile: {doc.FileName}\nBy: {doc.CreatedBy}\nAt: {doc.CreatedAtUtc.ToLocalTime():g}",
                "Document Details",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}