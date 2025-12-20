using DocControl.Presentation;

namespace DocControl.Forms;

public partial class CodeImportForm : Form
{
    private readonly MainController mainController;
    private TextBox csvTextBox = null!;
    private Button importButton = null!;
    private Button selectFileButton = null!;
    private Label statusLabel = null!;
    private ProgressBar progressBar = null!;

    public CodeImportForm(MainController mainController)
    {
        this.mainController = mainController;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        Text = "Import Codes from CSV";
        Size = new Size(800, 600);
        StartPosition = FormStartPosition.CenterParent;

        var mainPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            Padding = new Padding(10)
        };

        // File selection
        var filePanel = new Panel { Height = 35 };
        selectFileButton = new Button
        {
            Text = "Select CSV File...",
            Size = new Size(120, 25),
            Location = new Point(0, 5)
        };
        selectFileButton.Click += SelectFileButton_Click;
        filePanel.Controls.Add(selectFileButton);

        // CSV content
        var csvLabel = new Label
        {
            Text = "CSV Content (Level,Code,Description):",
            AutoSize = true
        };

        csvTextBox = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 9),
            Text = "Level,Code,Code Description\n1,DFS,Delpach Family Solutions Pty Ltd\n1,DFT,Delpach Family Trust\n1,DSF,DFS Superannuation Fund\n2,BNK,Bank\n2,EDU,Education\n3,AGE,Agenda\n3,ANL,Analysis"
        };

        // Buttons
        var buttonPanel = new Panel { Height = 35 };
        importButton = new Button
        {
            Text = "Import Codes",
            Size = new Size(100, 25),
            Location = new Point(0, 5)
        };
        importButton.Click += ImportButton_Click;

        var cancelButton = new Button
        {
            Text = "Cancel",
            Size = new Size(75, 25),
            Location = new Point(110, 5),
            DialogResult = DialogResult.Cancel
        };

        buttonPanel.Controls.AddRange(new Control[] { importButton, cancelButton });

        // Status
        statusLabel = new Label
        {
            Text = "Ready to import",
            AutoSize = true
        };

        progressBar = new ProgressBar
        {
            Visible = false,
            Style = ProgressBarStyle.Marquee,
            Height = 20
        };

        mainPanel.Controls.Add(filePanel);
        mainPanel.Controls.Add(csvLabel);
        mainPanel.Controls.Add(csvTextBox);
        mainPanel.Controls.Add(buttonPanel);
        mainPanel.Controls.Add(statusLabel);
        mainPanel.Controls.Add(progressBar);

        mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        Controls.Add(mainPanel);
        CancelButton = cancelButton;
        AcceptButton = importButton;

        ResumeLayout();
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