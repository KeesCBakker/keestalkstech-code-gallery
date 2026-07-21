using System.Text.Json.Nodes;
using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Services.Validation.Attributes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ktt.Validation.Api.Config;

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

    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
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
            var enumValues = getValues().Select(v => (JsonNode)JsonValue.Create(v)).ToList();

            if (!context.SchemaRepository.Schemas.ContainsKey(schemaName))
            {
                context.SchemaRepository.Schemas[schemaName] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Enum = enumValues,
                    Title = schemaName
                };
            }

            if (schema is OpenApiSchema concreteSchema)
            {
                concreteSchema.Enum = enumValues;
                concreteSchema.Type = JsonSchemaType.String;
            }

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
