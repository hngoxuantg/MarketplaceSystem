using System.Text.Json;
using System.Text.Json.Serialization;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Shared;

namespace MarketplaceSystem.Web.UI.Admin.Converters
{
    /// <summary>
    /// Custom converter ?? convert AttributeType t? string/number sang object
    /// Backend tr? v?: "Text", "Number", "Select", etc.
    /// Frontend expect: { Id: 1, Name: "Text" }
    /// </summary>
    public class AttributeTypeConverter : JsonConverter<AttributeType>
    {
        public override AttributeType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                // Backend tr? v? string: "Text", "Number", etc.
                var value = reader.GetString();
                return MapStringToAttributeType(value);
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                // Backend tr? v? number: 1, 2, 3, etc.
                var value = reader.GetInt32();
                return MapNumberToAttributeType(value);
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Backend tr? v? object: { "id": 1, "name": "Text" }
                using var doc = JsonDocument.ParseValue(ref reader);
                var root = doc.RootElement;

                var id = root.TryGetProperty("id", out var idProp) ? idProp.GetInt32() : 0;
                var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "";

                return new AttributeType
                {
                    Id = id,
                    Name = name ?? ""
                };
            }

            // Fallback: return Text type
            return new AttributeType { Id = 1, Name = "Text" };
        }

        public override void Write(Utf8JsonWriter writer, AttributeType value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("id", value.Id);
            writer.WriteString("name", value.Name);
            writer.WriteEndObject();
        }

        private AttributeType MapStringToAttributeType(string? value)
        {
            return value?.ToLower() switch
            {
                "text" => new AttributeType { Id = 1, Name = "Text" },
                "number" => new AttributeType { Id = 2, Name = "Number" },
                "select" => new AttributeType { Id = 3, Name = "Select" },
                "multiselect" => new AttributeType { Id = 4, Name = "MultiSelect" },
                "boolean" => new AttributeType { Id = 5, Name = "Boolean" },
                "date" => new AttributeType { Id = 6, Name = "Date" },
                "datetime" => new AttributeType { Id = 7, Name = "DateTime" },
                "email" => new AttributeType { Id = 8, Name = "Email" },
                "textarea" => new AttributeType { Id = 11, Name = "TextArea" },
                _ => new AttributeType { Id = 1, Name = "Text" }
            };
        }

        private AttributeType MapNumberToAttributeType(int value)
        {
            return value switch
            {
                1 => new AttributeType { Id = 1, Name = "Text" },
                2 => new AttributeType { Id = 2, Name = "Number" },
                3 => new AttributeType { Id = 3, Name = "Select" },
                4 => new AttributeType { Id = 4, Name = "MultiSelect" },
                5 => new AttributeType { Id = 5, Name = "Boolean" },
                6 => new AttributeType { Id = 6, Name = "Date" },
                7 => new AttributeType { Id = 7, Name = "DateTime" },
                8 => new AttributeType { Id = 8, Name = "Email" },
                11 => new AttributeType { Id = 11, Name = "TextArea" },
                _ => new AttributeType { Id = 1, Name = "Text" }
            };
        }
    }
}
