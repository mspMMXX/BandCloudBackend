using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParams = context.ApiDescription.ParameterDescriptions
            .Where(p => p.ModelMetadata?.ModelType == typeof(IFormFile) ||
                        (p.ModelMetadata?.ModelType != null &&
                         p.ModelMetadata.ModelType.Namespace == "BandCloudBackend.Models"));

        if (fileParams.Any())
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                ["file"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                },
                                ["comment"] = new OpenApiSchema
                                {
                                    Type = "string"
                                }
                            },
                            Required = new HashSet<string> { "file" }
                        }
                    }
                }
            };
        }
    }
}
