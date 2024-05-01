using System;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MediaFileMetadataCheckerAPI.Filters;
public class FileUploadFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var formParameters = context.ApiDescription.ParameterDescriptions
            .Where(paramDesc => paramDesc.IsFromForm());

        if (formParameters.Any())
        {
            return;
        }
        if (operation.RequestBody != null)
        {
            return;
        }
        if (context.ApiDescription.HttpMethod == HttpMethod.Post.Method)
        {
            var uploadFileMediaType = new OpenApiMediaType()
            {
                Schema = new OpenApiSchema()
                {
                    Type = "object",
                    Properties =
                    {
                        ["files"] = new OpenApiSchema()
                        {
                            Type = "array",
                            Items = new OpenApiSchema()
                            {
                                Type = "string",
                                Format = "binary"
                            }
                        }
                    },
                    Required = new HashSet<string>() { "files" }
                }
            };

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = { ["multipart/form-data"] = uploadFileMediaType }
            };
        }
    }
}

public static class Helper
{
    internal static bool IsFromForm(this ApiParameterDescription apiParameter)
    {
        var source = apiParameter.Source;
        var elementType = apiParameter.ModelMetadata?.ElementType;

        return (elementType != null && typeof(IFormFile).IsAssignableFrom(elementType));
    }
}
