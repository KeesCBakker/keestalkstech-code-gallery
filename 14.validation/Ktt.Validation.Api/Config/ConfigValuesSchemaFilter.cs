using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Services.Validation.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Some fields are decorated with validation attributes. We can use configuration
/// to determine the enum values of those fields.
/// </summary>
public class ConfigValuesSchemaFilter(
    ProvisioningOptions config
) : ISchemaFilter
{
    private readonly Dictionary<Type, Func<IEnumerable<string>>> _valueMap = new()
    {
        [typeof(LabelAttribute)] = () => config.Labels,
        [typeof(EnvironmentAttribute)] = () => config.Environments,
    };

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var member = context.MemberInfo;
        if (member == null)
        {
            return;
        }

        var attributes = member.GetCustomAttributes(inherit: true);

        foreach (var (attrType, getValues) in _valueMap)
        {
            if (!attributes.Any(attrType.IsInstanceOfType))
            {
                continue;
            }

            var schemaName = ToEnumSchemaName(attrType);

            if (!context.SchemaRepository.Schemas.ContainsKey(schemaName))
            {
                var enumValues = getValues().Select(v => new OpenApiString(v)).Cast<IOpenApiAny>().ToList();

                context.SchemaRepository.Schemas[schemaName] = new OpenApiSchema
                {
                    Type = "string",
                    Enum = enumValues,
                    Title = schemaName
                };
            }

            schema.Reference = new OpenApiReference
            {
                Type = ReferenceType.Schema,
                Id = schemaName
            };

            schema.Type = null;
            schema.Enum = null;
            schema.Title = null;

            break;
        }
    }

    private static string ToEnumSchemaName(Type attributeType)
    {
        const string suffix = "Attribute";
        var name = attributeType.Name;
        return name.EndsWith(suffix)
            ? name[..^suffix.Length] + "Enum"
            : name + "Enum";
    }
}
