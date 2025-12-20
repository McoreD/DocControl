using System.Text;
using DocControl.Configuration;
using DocControl.Models;

namespace DocControl.Services;

public sealed class CodeGenerator
{
    private readonly DocumentConfig config;

    public CodeGenerator(DocumentConfig config)
    {
        this.config = config;
    }

    public string BuildCode(CodeSeriesKey key, int number)
    {
        var parts = new List<string> { key.Level1, key.Level2, key.Level3 };
        if (config.EnableLevel4 && !string.IsNullOrWhiteSpace(key.Level4))
        {
            parts.Add(key.Level4);
        }
        parts.Add(number.ToString().PadLeft(config.PaddingLength, '0'));
        return string.Join(config.Separator, parts);
    }

    public string BuildFileName(CodeSeriesKey key, int number, string freeText, string? extension)
    {
        var baseCode = BuildCode(key, number);
        var sb = new StringBuilder(baseCode);
        if (!string.IsNullOrWhiteSpace(freeText))
        {
            sb.Append(config.Separator);
            sb.Append(freeText.Trim());
        }
        if (!string.IsNullOrWhiteSpace(extension))
        {
            var sanitized = extension.StartsWith('.') ? extension : $".{extension}";
            sb.Append(sanitized);
        }
        return sb.ToString();
    }
}
