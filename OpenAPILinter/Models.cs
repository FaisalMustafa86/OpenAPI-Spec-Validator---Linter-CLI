using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace OpenAPILinter;

/// <summary>
/// OpenAPI Specification root object
/// </summary>
public class OpenAPISpec
{
    [JsonProperty("openapi")]
    [YamlMember(Alias = "openapi")]
    public string? OpenAPI { get; set; }

    [JsonProperty("info")]
    [YamlMember(Alias = "info")]
    public InfoObject? Info { get; set; }

    [JsonProperty("paths")]
    [YamlMember(Alias = "paths")]
    public Dictionary<string, PathItem>? Paths { get; set; }

    [JsonProperty("components")]
    [YamlMember(Alias = "components")]
    public ComponentsObject? Components { get; set; }

    public Dictionary<string, object> AdditionalProperties { get; set; } = new();
}

/// <summary>
/// Info object containing metadata about the API
/// </summary>
public class InfoObject
{
    [JsonProperty("title")]
    [YamlMember(Alias = "title")]
    public string? Title { get; set; }

    [JsonProperty("version")]
    [YamlMember(Alias = "version")]
    public string? Version { get; set; }

    [JsonProperty("description")]
    [YamlMember(Alias = "description")]
    public string? Description { get; set; }
}

/// <summary>
/// Path item describing operations for a specific path
/// </summary>
public class PathItem
{
    [JsonProperty("get")]
    [YamlMember(Alias = "get")]
    public Operation? Get { get; set; }

    [JsonProperty("post")]
    [YamlMember(Alias = "post")]
    public Operation? Post { get; set; }

    [JsonProperty("put")]
    [YamlMember(Alias = "put")]
    public Operation? Put { get; set; }

    [JsonProperty("delete")]
    [YamlMember(Alias = "delete")]
    public Operation? Delete { get; set; }

    [JsonProperty("patch")]
    [YamlMember(Alias = "patch")]
    public Operation? Patch { get; set; }

    [JsonProperty("options")]
    [YamlMember(Alias = "options")]
    public Operation? Options { get; set; }

    [JsonProperty("head")]
    [YamlMember(Alias = "head")]
    public Operation? Head { get; set; }

    [JsonProperty("trace")]
    [YamlMember(Alias = "trace")]
    public Operation? Trace { get; set; }

    [JsonProperty("parameters")]
    [YamlMember(Alias = "parameters")]
    public List<Parameter>? Parameters { get; set; }

    public List<Operation> GetAllOperations()
    {
        var operations = new List<Operation>();
        if (Get != null) operations.Add(Get);
        if (Post != null) operations.Add(Post);
        if (Put != null) operations.Add(Put);
        if (Delete != null) operations.Add(Delete);
        if (Patch != null) operations.Add(Patch);
        if (Options != null) operations.Add(Options);
        if (Head != null) operations.Add(Head);
        if (Trace != null) operations.Add(Trace);
        return operations;
    }

    public string? GetOperationMethod(Operation operation)
    {
        if (Get == operation) return "GET";
        if (Post == operation) return "POST";
        if (Put == operation) return "PUT";
        if (Delete == operation) return "DELETE";
        if (Patch == operation) return "PATCH";
        if (Options == operation) return "OPTIONS";
        if (Head == operation) return "HEAD";
        if (Trace == operation) return "TRACE";
        return null;
    }
}

/// <summary>
/// Operation object describing an API operation
/// </summary>
public class Operation
{
    [JsonProperty("summary")]
    [YamlMember(Alias = "summary")]
    public string? Summary { get; set; }

    [JsonProperty("description")]
    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [JsonProperty("operationId")]
    [YamlMember(Alias = "operationId")]
    public string? OperationId { get; set; }

    [JsonProperty("parameters")]
    [YamlMember(Alias = "parameters")]
    public List<Parameter>? Parameters { get; set; }

    [JsonProperty("requestBody")]
    [YamlMember(Alias = "requestBody")]
    public RequestBody? RequestBody { get; set; }

    [JsonProperty("responses")]
    [YamlMember(Alias = "responses")]
    public Dictionary<string, ResponseObject>? Responses { get; set; }

    [JsonProperty("tags")]
    [YamlMember(Alias = "tags")]
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Parameter object describing a parameter
/// </summary>
public class Parameter
{
    [JsonProperty("name")]
    [YamlMember(Alias = "name")]
    public string? Name { get; set; }

    [JsonProperty("in")]
    [YamlMember(Alias = "in")]
    public string? In { get; set; }

    [JsonProperty("description")]
    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [JsonProperty("schema")]
    [YamlMember(Alias = "schema")]
    public Schema? Schema { get; set; }

    [JsonProperty("required")]
    [YamlMember(Alias = "required")]
    public bool? Required { get; set; }
}

/// <summary>
/// Request body object describing the request body
/// </summary>
public class RequestBody
{
    [JsonProperty("required")]
    [YamlMember(Alias = "required")]
    public bool? Required { get; set; }

    [JsonProperty("content")]
    [YamlMember(Alias = "content")]
    public Dictionary<string, object>? Content { get; set; }

    [JsonProperty("description")]
    [YamlMember(Alias = "description")]
    public string? Description { get; set; }
}

/// <summary>
/// Response object describing a response
/// </summary>
public class ResponseObject
{
    [JsonProperty("description")]
    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [JsonProperty("content")]
    [YamlMember(Alias = "content")]
    public Dictionary<string, object>? Content { get; set; }
}

/// <summary>
/// Schema object describing data structures
/// </summary>
public class Schema
{
    [JsonProperty("type")]
    [YamlMember(Alias = "type")]
    public string? Type { get; set; }

    [JsonProperty("format")]
    [YamlMember(Alias = "format")]
    public string? Format { get; set; }

    [JsonProperty("description")]
    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [JsonProperty("properties")]
    [YamlMember(Alias = "properties")]
    public Dictionary<string, Schema>? Properties { get; set; }

    [JsonProperty("$ref")]
    [YamlMember(Alias = "$ref")]
    public string? Ref { get; set; }

    [JsonProperty("items")]
    [YamlMember(Alias = "items")]
    public Schema? Items { get; set; }

    [JsonProperty("required")]
    [YamlMember(Alias = "required")]
    public List<string>? Required { get; set; }
}

/// <summary>
/// Components object containing reusable schema definitions
/// </summary>
public class ComponentsObject
{
    [JsonProperty("schemas")]
    [YamlMember(Alias = "schemas")]
    public Dictionary<string, Schema>? Schemas { get; set; }
}

/// <summary>
/// Issue found during validation
/// </summary>
public class ValidationIssue
{
    public IssueSeverity Severity { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Issue severity level
/// </summary>
public enum IssueSeverity
{
    Error,
    Warning,
    Info
}

/// <summary>
/// Validation report containing results
/// </summary>
public class ValidationReport
{
    public int TotalPaths { get; set; }
    public int TotalOperations { get; set; }
    public List<ValidationIssue> Issues { get; set; } = new();

    public int ErrorCount => Issues.Count(i => i.Severity == IssueSeverity.Error);
    public int WarningCount => Issues.Count(i => i.Severity == IssueSeverity.Warning);
    public int InfoCount => Issues.Count(i => i.Severity == IssueSeverity.Info);
    public int TotalIssues => Issues.Count;

    public int CalculateScore()
    {
        if (TotalOperations == 0) return 100;

        int checks = 0;
        int passes = 0;

        // Each operation should have a summary (TotalOperations checks)
        checks += TotalOperations;
        passes += TotalOperations - Issues.Count(i => i.Message.Contains("missing a summary"));

        // Each operation should have a description (TotalOperations checks)
        checks += TotalOperations;
        passes += TotalOperations - Issues.Count(i => i.Message.Contains("missing a description") && i.Severity == IssueSeverity.Error);

        // Each operation should have responses (TotalOperations checks)
        checks += TotalOperations;
        passes += TotalOperations - Issues.Count(i => i.Message.Contains("missing response"));

        // Path operations check
        checks += TotalPaths;
        passes += TotalPaths - Issues.Count(i => i.Message.Contains("no operations"));

        if (checks == 0) return 100;

        return Math.Max(0, (passes * 100) / checks);
    }
}
