using System;
using DragonsCrossing.Core;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DragonsCrossing.Api;

public class CustomDocFilter : IDocumentFilter
{

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var schemaGenerator = context.SchemaGenerator;

        foreach (var foundType in PolymorphicBaseJsonConverter.GetPolymorphicTypes())
            schemaGenerator.GenerateSchema(foundType, context.SchemaRepository);
    }
}
