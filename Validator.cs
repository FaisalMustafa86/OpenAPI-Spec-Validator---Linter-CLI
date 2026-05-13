namespace OpenAPILinter;

/// <summary>
/// Validator for OpenAPI specifications
/// </summary>
public class Validator
{
    private readonly OpenAPISpec _spec;

    public Validator(OpenAPISpec spec)
    {
        _spec = spec;
    }

    /// <summary>
    /// Run all validation checks and return a report
    /// </summary>
    public ValidationReport Validate()
    {
        var report = new ValidationReport();

        // Get path count
        report.TotalPaths = _spec.Paths?.Count ?? 0;

        // Get operation count
        report.TotalOperations = _spec.Paths?.Values
            .SelectMany(p => p.GetAllOperations())
            .Count() ?? 0;

        // Run validation checks
        ValidateInfoObject(report);
        ValidatePaths(report);
        ValidateComponents(report);

        return report;
    }

    /// <summary>
    /// Validate info object
    /// </summary>
    private void ValidateInfoObject(ValidationReport report)
    {
        if (_spec.Info == null)
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Error,
                Location = "Info",
                Message = "Missing info object - spec must have title, version, and description"
            });
            return;
        }

        if (string.IsNullOrWhiteSpace(_spec.Info.Title))
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Error,
                Location = "Info",
                Message = "Info object is missing required title"
            });
        }

        if (string.IsNullOrWhiteSpace(_spec.Info.Version))
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Error,
                Location = "Info",
                Message = "Info object is missing required version"
            });
        }

        if (string.IsNullOrWhiteSpace(_spec.Info.Description))
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Warning,
                Location = "Info",
                Message = "Info object should have a description explaining the API"
            });
        }
    }

    /// <summary>
    /// Validate all paths and their operations
    /// </summary>
    private void ValidatePaths(ValidationReport report)
    {
        if (_spec.Paths == null || _spec.Paths.Count == 0)
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Error,
                Location = "Paths",
                Message = "Spec must have at least one path defined"
            });
            return;
        }

        var allOperationIds = new HashSet<string>();

        foreach (var (path, pathItem) in _spec.Paths)
        {
            ValidatePath(path, pathItem, report, allOperationIds);
        }
    }

    /// <summary>
    /// Validate a specific path
    /// </summary>
    private void ValidatePath(string path, PathItem pathItem, ValidationReport report, HashSet<string> allOperationIds)
    {
        var operations = pathItem.GetAllOperations();

        // Check if path has any operations
        if (operations.Count == 0)
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Warning,
                Location = path,
                Message = "Path has no operations defined (GET, POST, etc.)"
            });
            return;
        }

        // Path-level parameters should have descriptions
        if (pathItem.Parameters != null)
        {
            foreach (var param in pathItem.Parameters)
            {
                ValidateParameter(param, $"{path} > parameter", report);
            }
        }

        // Validate each operation
        foreach (var operation in operations)
        {
            var method = pathItem.GetOperationMethod(operation);
            ValidateOperation(method ?? "UNKNOWN", path, operation, report, allOperationIds);
        }
    }

    /// <summary>
    /// Validate a specific operation
    /// </summary>
    private void ValidateOperation(string method, string path, Operation operation, ValidationReport report, HashSet<string> allOperationIds)
    {
        var operationLocation = $"{method} {path}";

        // Check for summary
        if (string.IsNullOrWhiteSpace(operation.Summary))
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Error,
                Location = operationLocation,
                Message = "Operation is missing a summary - should be a short description"
            });
        }

        // Check for description
        if (string.IsNullOrWhiteSpace(operation.Description))
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Warning,
                Location = operationLocation,
                Message = "Operation should have a description with detailed information"
            });
        }

        // Check for duplicate operation IDs
        if (!string.IsNullOrWhiteSpace(operation.OperationId))
        {
            if (allOperationIds.Contains(operation.OperationId))
            {
                report.Issues.Add(new ValidationIssue
                {
                    Severity = IssueSeverity.Error,
                    Location = operationLocation,
                    Message = $"Duplicate operation ID: '{operation.OperationId}' - each operation must have a unique ID"
                });
            }
            else
            {
                allOperationIds.Add(operation.OperationId);
            }
        }

        // Validate parameters
        if (operation.Parameters != null)
        {
            foreach (var param in operation.Parameters)
            {
                ValidateParameter(param, $"{operationLocation} > parameter", report);
            }
        }

        // Validate responses
        ValidateResponses(operationLocation, operation.Responses, report);
    }

    /// <summary>
    /// Validate parameter
    /// </summary>
    private void ValidateParameter(Parameter parameter, string location, ValidationReport report)
    {
        if (string.IsNullOrWhiteSpace(parameter.Description))
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Warning,
                Location = $"{location} '{parameter.Name}'",
                Message = "Parameter should have a clear description"
            });
        }

        if (parameter.Schema == null)
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Error,
                Location = $"{location} '{parameter.Name}'",
                Message = "Parameter must have a schema defined"
            });
        }
    }

    /// <summary>
    /// Validate operation responses
    /// </summary>
    private void ValidateResponses(string operationLocation, Dictionary<string, ResponseObject>? responses, ValidationReport report)
    {
        if (responses == null || responses.Count == 0)
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Error,
                Location = $"{operationLocation} > responses",
                Message = "Operation must define at least one response"
            });
            return;
        }

        // Check for 200 response
        var hasSuccessResponse = responses.Keys.Any(k => k.StartsWith("2"));
        if (!hasSuccessResponse)
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Error,
                Location = $"{operationLocation} > responses",
                Message = "Operation should have at least one success response (2xx status code)"
            });
        }

        // Check for error response
        var hasErrorResponse = responses.Keys.Any(k => k.StartsWith("4") || k.StartsWith("5"));
        if (!hasErrorResponse)
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Warning,
                Location = $"{operationLocation} > responses",
                Message = "Operation should define at least one error response (4xx or 5xx) for handling failures"
            });
        }

        // Validate each response
        foreach (var (statusCode, response) in responses)
        {
            if (response != null && string.IsNullOrWhiteSpace(response.Description))
            {
                report.Issues.Add(new ValidationIssue
                {
                    Severity = IssueSeverity.Info,
                    Location = $"{operationLocation} > response {statusCode}",
                    Message = "Response should have a description explaining when it occurs"
                });
            }
        }
    }

    /// <summary>
    /// Validate components/schemas
    /// </summary>
    private void ValidateComponents(ValidationReport report)
    {
        if (_spec.Components?.Schemas == null || _spec.Components.Schemas.Count == 0)
        {
            return;
        }

        foreach (var (schemaName, schema) in _spec.Components.Schemas)
        {
            ValidateSchema(schemaName, schema, report);
        }
    }

    /// <summary>
    /// Validate a schema definition
    /// </summary>
    private void ValidateSchema(string schemaName, Schema schema, ValidationReport report)
    {
        var location = $"Components > Schemas > {schemaName}";

        if (schema == null)
        {
            return;
        }

        // Check for description
        if (string.IsNullOrWhiteSpace(schema.Description) && !string.IsNullOrWhiteSpace(schema.Ref))
        {
            // Referenced schemas don't need descriptions
            return;
        }

        if (string.IsNullOrWhiteSpace(schema.Description))
        {
            report.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Info,
                Location = location,
                Message = "Schema should have a description explaining its purpose"
            });
        }

        // Validate nested properties
        if (schema.Properties != null)
        {
            foreach (var (propName, prop) in schema.Properties)
            {
                if (prop != null && string.IsNullOrWhiteSpace(prop.Description) && string.IsNullOrWhiteSpace(prop.Ref))
                {
                    report.Issues.Add(new ValidationIssue
                    {
                        Severity = IssueSeverity.Info,
                        Location = $"{location} > {propName}",
                        Message = "Property should have a description"
                    });
                }
            }
        }
    }
}
