using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DocControl.Infrastructure.Presentation;

namespace DocControl.Wpf
{
    public partial class CodeEditDialog : Window
    {
        private readonly MainController controller;
        private ObservableCollection<CodeEditItem> codes;
        private bool hasUnsavedChanges = false;

        public CodeEditDialog(MainController controller, ObservableCollection<CodeEditItem> codes)
        {
            InitializeComponent();
            this.controller = controller;
            this.codes = codes;
            
            dgCodes.ItemsSource = codes;
            
            // Track changes to the collection
            foreach (var code in codes)
            {
                code.PropertyChanged += Code_PropertyChanged;
            }
        }

        private void Code_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            hasUnsavedChanges = true;
        }

        private async void btnAddCode_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtNewCode.Text))
            {
                MessageBox.Show("Please enter a code.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var level = int.Parse(((ComboBoxItem)cmbNewLevel.SelectedItem).Content.ToString()!);
            var code = txtNewCode.Text.Trim().ToUpper();
            var description = txtNewDescription.Text.Trim();

            // Check for duplicates
            if (codes.Any(c => c.Level == level && c.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"A Level {level} code '{code}' already exists.", "Duplicate Code", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Import the new code using the existing import functionality
                var csvLine = $"{level},{code},{description}";
                var result = await controller.ImportCodesAsync(csvLine);

                if (result.Errors.Count > 0)
                {
                    MessageBox.Show($"Error adding code: {string.Join(", ", result.Errors)}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Add to the collection
                var newCodeItem = new CodeEditItem
                {
                    Level = level,
                    Code = code,
                    Description = description,
                    IsNew = false
                };
                
                newCodeItem.PropertyChanged += Code_PropertyChanged;
                codes.Add(newCodeItem);

                // Clear inputs
                txtNewCode.Clear();
                txtNewDescription.Clear();
                cmbNewLevel.SelectedIndex = 0;

                MessageBox.Show("Code added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add code: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            if (dgCodes.SelectedItem is not CodeEditItem selectedCode)
            {
                MessageBox.Show("Please select a code to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete Level {selectedCode.Level} code '{selectedCode.Code}'?\n\n" +
                $"Description: {selectedCode.Description}\n\n" +
                $"This action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await controller.DeleteCodeAsync(selectedCode.Level, selectedCode.Code);
                
                selectedCode.PropertyChanged -= Code_PropertyChanged;
                codes.Remove(selectedCode);

                MessageBox.Show("Code deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete code: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!hasUnsavedChanges)
            {
                MessageBox.Show("No changes to save.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                // Update descriptions for all modified codes
                foreach (var code in codes.Where(c => c.IsModified))
                {
                    // Re-import the code with updated description
                    var csvLine = $"{code.Level},{code.Code},{code.Description}";
                    await controller.ImportCodesAsync(csvLine);
                }

                hasUnsavedChanges = false;
                
                // Reset modified flags
                foreach (var code in codes)
                {
                    code.IsModified = false;
                }

                MessageBox.Show("Changes saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Are you sure you want to close without saving?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            DialogResult = false;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Are you sure you want to close without saving?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnClosing(e);
        }
    }

    public class CodeEditItem : INotifyPropertyChanged
    {
        private string description = string.Empty;
        private bool isModified = false;

        public int Level { get; set; }
        public string Code { get; set; } = string.Empty;
        
        public string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public bool IsNew { get; set; }
        
        public bool IsModified
        {
            get => isModified;
            set
            {
                isModified = value;
                OnPropertyChanged(nameof(IsModified));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
