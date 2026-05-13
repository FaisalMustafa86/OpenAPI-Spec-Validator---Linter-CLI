namespace OpenAPILinter;

/// <summary>
/// Formats and prints validation reports to console with colors
/// </summary>
public class ReportPrinter
{
    private const string ResetColor = "\u001b[0m";
    private const string Red = "\u001b[31m";
    private const string Yellow = "\u001b[33m";
    private const string Cyan = "\u001b[36m";
    private const string Green = "\u001b[32m";
    private const string BoldWhite = "\u001b[1;37m";

    /// <summary>
    /// Print the validation report to console
    /// </summary>
    public void PrintReport(ValidationReport report, string fileName)
    {
        Console.WriteLine();
        PrintHeader();
        PrintFileInfo(fileName);
        PrintSummary(report);
        PrintIssues(report);
        PrintScore(report);
        Console.WriteLine();
    }

    /// <summary>
    /// Print errors for fatal failures
    /// </summary>
    public void PrintError(string message)
    {
        Console.WriteLine($"{Red}✗ ERROR:{ResetColor} {message}");
    }
    
    /// <summary>
    /// Print header
    /// </summary>
    private void PrintHeader()
    {
        Console.WriteLine($"{BoldWhite}╔════════════════════════════════════════════════════════════╗{ResetColor}");
        Console.WriteLine($"{BoldWhite}║         OpenAPI Linter - Specification Validator            ║{ResetColor}");
        Console.WriteLine($"{BoldWhite}╚════════════════════════════════════════════════════════════╝{ResetColor}");
    }

    /// <summary>
    /// Print file information
    /// </summary>
    private void PrintFileInfo(string fileName)
    {
        Console.WriteLine();
        Console.WriteLine($"{BoldWhite}File:{ResetColor} {fileName}");
    }

    /// <summary>
    /// Print summary statistics
    /// </summary>
    private void PrintSummary(ValidationReport report)
    {
        Console.WriteLine();
        Console.WriteLine($"{BoldWhite}Summary:{ResetColor}");
        Console.WriteLine($"  • Total Paths:      {report.TotalPaths}");
        Console.WriteLine($"  • Total Operations: {report.TotalOperations}");
        Console.WriteLine($"  • Total Issues:     {report.TotalIssues}");

        if (report.TotalIssues > 0)
        {
            var severityLine = "  • Issues by type:  ";
            if (report.ErrorCount > 0)
                severityLine += $"{Red}Errors: {report.ErrorCount}{ResetColor} ";
            if (report.WarningCount > 0)
                severityLine += $"{Yellow}Warnings: {report.WarningCount}{ResetColor} ";
            if (report.InfoCount > 0)
                severityLine += $"{Cyan}Info: {report.InfoCount}{ResetColor}";

            Console.WriteLine(severityLine);
        }
    }

    /// <summary>
    /// Print all issues grouped by severity
    /// </summary>
    private void PrintIssues(ValidationReport report)
    {
        if (report.TotalIssues == 0)
        {
            Console.WriteLine();
            Console.WriteLine($"{Green}✓ No issues found! Your OpenAPI spec looks great.{ResetColor}");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"{BoldWhite}Issues:{ResetColor}");
        Console.WriteLine();

        // Errors
        var errors = report.Issues.Where(i => i.Severity == IssueSeverity.Error).ToList();
        if (errors.Any())
        {
            Console.WriteLine($"{Red}[ERROR - Blocks generation]{ResetColor}");
            foreach (var issue in errors)
            {
                PrintIssue(issue, Red);
            }
            Console.WriteLine();
        }

        // Warnings
        var warnings = report.Issues.Where(i => i.Severity == IssueSeverity.Warning).ToList();
        if (warnings.Any())
        {
            Console.WriteLine($"{Yellow}[WARNING - Should fix]{ResetColor}");
            foreach (var issue in warnings)
            {
                PrintIssue(issue, Yellow);
            }
            Console.WriteLine();
        }

        // Info
        var infos = report.Issues.Where(i => i.Severity == IssueSeverity.Info).ToList();
        if (infos.Any())
        {
            Console.WriteLine($"{Cyan}[INFO - Nice to have]{ResetColor}");
            foreach (var issue in infos)
            {
                PrintIssue(issue, Cyan);
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Print a single issue
    /// </summary>
    private void PrintIssue(ValidationIssue issue, string colorCode)
    {
        Console.WriteLine($"  {colorCode}●{ResetColor} {issue.Location}");
        Console.WriteLine($"    → {issue.Message}");
    }

    /// <summary>
    /// Print quality score
    /// </summary>
    private void PrintScore(ValidationReport report)
    {
        var score = report.CalculateScore();
        var scoreColor = GetScoreColor(score);

        Console.WriteLine($"{BoldWhite}Quality Score:{ResetColor} {scoreColor}{score}/100{ResetColor}");

        // Print score bar
        var filledLength = score / 10;
        var emptyLength = 10 - filledLength;
        var bar = new string('█', filledLength) + new string('░', emptyLength);
        Console.WriteLine($"  [{scoreColor}{bar}{ResetColor}]");
    }

    /// <summary>
    /// Get color based on score
    /// </summary>
    private string GetScoreColor(int score)
    {
        if (score >= 80)
            return Green;
        if (score >= 60)
            return Yellow;
        return Red;
    }
}
