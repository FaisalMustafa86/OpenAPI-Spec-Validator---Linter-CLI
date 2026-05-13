using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace OpenAPILinter;

class Program
{
    static void Main(string[] args)
    {
        var printer = new ReportPrinter();

        // Validate arguments
        if (args.Length == 0)
        {
            printer.PrintError("No file path provided");
            Console.WriteLine("Usage: openapi-linter <path-to-openapi-spec.json|yaml>");
            Console.WriteLine("Example: openapi-linter ./api-spec.yaml");
            Environment.Exit(1);
        }

        var filePath = args[0];

        // Check if file exists
        if (!File.Exists(filePath))
        {
            printer.PrintError($"File not found: {filePath}");
            Environment.Exit(1);
        }

        try
        {
            // Read the file
            var fileContent = File.ReadAllText(filePath);

            // Parse based on file extension
            OpenAPISpec? spec = ParseOpenAPISpec(filePath, fileContent);

            if (spec == null)
            {
                printer.PrintError($"Invalid OpenAPI specification file");
                Environment.Exit(1);
            }

            // Validate the spec
            var validator = new Validator(spec);
            var report = validator.Validate();

            // Print report
            printer.PrintReport(report, Path.GetFileName(filePath));

            // Exit with appropriate code
            Environment.Exit(report.ErrorCount > 0 ? 1 : 0);
        }
        catch (JsonException ex)
        {
            printer.PrintError($"Invalid JSON format: {ex.Message}");
            Environment.Exit(1);
        }
        catch (YamlDotNet.Core.YamlException ex)
        {
            printer.PrintError($"Invalid YAML format: {ex.Message}");
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            printer.PrintError($"An unexpected error occurred: {ex.Message}");
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Parse OpenAPI spec from JSON or YAML content
    /// </summary>
    static OpenAPISpec? ParseOpenAPISpec(string filePath, string content)
    {
        // Determine file type
        var extension = Path.GetExtension(filePath).ToLower();

        if (extension == ".json")
        {
            return JsonConvert.DeserializeObject<OpenAPISpec>(content, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
        else if (extension == ".yaml" || extension == ".yml")
        {
            var deserializer = new DeserializerBuilder().Build();

            return deserializer.Deserialize<OpenAPISpec>(content);
        }
        else
        {
            throw new ArgumentException($"Unsupported file format: {extension}. Supported: .json, .yaml, .yml");
        }
    }
}
