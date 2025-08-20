using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

/// <summary>
/// Swagger-OperationFilter für Datei-Uploads.
/// Passt die Swagger-Dokumentation an, sodass Upload-Endpunkte
/// (z. B. mit <see cref="IFormFile"/> oder Recording-Model) 
/// korrekt als <c>multipart/form-data</c> angezeigt werden.
/// </summary>
public class FileUploadOperationFilter : IOperationFilter
{
    /// <summary>
    /// Wendet die Anpassung an einer Swagger-Operation an.
    /// Wenn Endpunkt Datei-Uploads akzeptiert, wird ein entsprechender 
    /// <c>multipart/form-data</c>-RequestBody erzeugt.
    /// </summary>
    /// <param name="operation">Die Swagger-Operation, die angepasst wird.</param>
    /// <param name="context">Swagger-Kontext mit API-Infos.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Parameter suchen, die IFormFile oder ein Upload-Model enthalten
        var fileParams = context.ApiDescription.ParameterDescriptions
            .Where(p => p.ModelMetadata?.ModelType == typeof(IFormFile) ||
                        (p.ModelMetadata?.ModelType != null &&
                         p.ModelMetadata.ModelType.Namespace == "BandCloudBackend.Models"));

        // Wenn solche Parameter vorhanden sind, RequestBody für multipart/form-data definieren
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
