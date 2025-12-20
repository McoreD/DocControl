using DocControl.Infrastructure.Presentation;

namespace DocControl.Forms;

public partial class CodeImportForm : Form
{
    private readonly MainController mainController;
    private TextBox csvTextBox = null!;
    private Button importButton = null!;
    private Button selectFileButton = null!;
    private Label statusLabel = null!;
    private TableLayoutPanel mainPanel;
    private Panel filePanel;
    private Label csvLabel;
    private Panel buttonPanel;
    private Button cancelButton;
    private ProgressBar progressBar = null!;

    public CodeImportForm(MainController mainController)
    {
        this.mainController = mainController;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        mainPanel = new TableLayoutPanel();
        filePanel = new Panel();
        selectFileButton = new Button();
        csvLabel = new Label();
        csvTextBox = new TextBox();
        buttonPanel = new Panel();
        importButton = new Button();
        cancelButton = new Button();
        statusLabel = new Label();
        progressBar = new ProgressBar();
        mainPanel.SuspendLayout();
        filePanel.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // mainPanel
        // 
        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        mainPanel.Controls.Add(filePanel);
        mainPanel.Controls.Add(csvLabel);
        mainPanel.Controls.Add(csvTextBox);
        mainPanel.Controls.Add(buttonPanel);
        mainPanel.Controls.Add(statusLabel);
        mainPanel.Controls.Add(progressBar);
        mainPanel.Location = new Point(0, 0);
        mainPanel.Name = "mainPanel";
        mainPanel.RowStyles.Add(new RowStyle());
        mainPanel.RowStyles.Add(new RowStyle());
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainPanel.RowStyles.Add(new RowStyle());
        mainPanel.RowStyles.Add(new RowStyle());
        mainPanel.RowStyles.Add(new RowStyle());
        mainPanel.Size = new Size(200, 100);
        mainPanel.TabIndex = 0;
        // 
        // filePanel
        // 
        filePanel.Controls.Add(selectFileButton);
        filePanel.Location = new Point(3, 3);
        filePanel.Name = "filePanel";
        filePanel.Size = new Size(194, 100);
        filePanel.TabIndex = 0;
        // 
        // selectFileButton
        // 
        selectFileButton.Location = new Point(0, 0);
        selectFileButton.Name = "selectFileButton";
        selectFileButton.Size = new Size(75, 23);
        selectFileButton.TabIndex = 0;
        selectFileButton.Click += SelectFileButton_Click;
        // 
        // csvLabel
        // 
        csvLabel.Location = new Point(3, 106);
        csvLabel.Name = "csvLabel";
        csvLabel.Size = new Size(100, 23);
        csvLabel.TabIndex = 1;
        // 
        // csvTextBox
        // 
        csvTextBox.Location = new Point(3, 132);
        csvTextBox.Name = "csvTextBox";
        csvTextBox.Size = new Size(100, 27);
        csvTextBox.TabIndex = 2;
        // 
        // buttonPanel
        // 
        buttonPanel.Controls.Add(importButton);
        buttonPanel.Controls.Add(cancelButton);
        buttonPanel.Location = new Point(3, -55);
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Size = new Size(194, 100);
        buttonPanel.TabIndex = 3;
        // 
        // importButton
        // 
        importButton.Location = new Point(0, 0);
        importButton.Name = "importButton";
        importButton.Size = new Size(75, 23);
        importButton.TabIndex = 0;
        importButton.Click += ImportButton_Click;
        // 
        // cancelButton
        // 
        cancelButton.Location = new Point(0, 0);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(75, 23);
        cancelButton.TabIndex = 1;
        // 
        // statusLabel
        // 
        statusLabel.Location = new Point(3, 48);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(100, 23);
        statusLabel.TabIndex = 4;
        // 
        // progressBar
        // 
        progressBar.Location = new Point(3, 74);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(100, 23);
        progressBar.TabIndex = 5;
        // 
        // CodeImportForm
        // 
        AcceptButton = importButton;
        CancelButton = cancelButton;
        ClientSize = new Size(782, 553);
        Controls.Add(mainPanel);
        Name = "CodeImportForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Import Codes from CSV";
        mainPanel.ResumeLayout(false);
        mainPanel.PerformLayout();
        filePanel.ResumeLayout(false);
        buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    private void SelectFileButton_Click(object? sender, EventArgs e)
    {
        using var openFileDialog = new OpenFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            Title = "Select CSV File"
        };

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var content = File.ReadAllText(openFileDialog.FileName);
                csvTextBox.Text = content;
                statusLabel.Text = $"Loaded {openFileDialog.FileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void ImportButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(csvTextBox.Text))
        {
            MessageBox.Show("Please provide CSV content to import.", "No Content", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        importButton.Enabled = false;
        progressBar.Visible = true;
        statusLabel.Text = "Importing codes...";

        try
        {
            var result = await mainController.ImportCodesAsync(csvTextBox.Text).ConfigureAwait(true);

            progressBar.Visible = false;

            if (result.HasErrors)
            {
                var message = $"Import completed with {result.SuccessCount} successful imports and {result.Errors.Count} errors:\n\n" +
                              string.Join("\n", result.Errors.Take(10));

                if (result.Errors.Count > 10)
                {
                    message += $"\n... and {result.Errors.Count - 10} more errors.";
                }

                MessageBox.Show(message, "Import Results", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show($"Successfully imported {result.SuccessCount} codes.", "Import Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }

            statusLabel.Text = $"Import completed: {result.SuccessCount} successful, {result.Errors.Count} errors";
        }
        catch (Exception ex)
        {
            progressBar.Visible = false;
            statusLabel.Text = "Import failed";
            MessageBox.Show($"Import failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            importButton.Enabled = true;
        }
    }
}
