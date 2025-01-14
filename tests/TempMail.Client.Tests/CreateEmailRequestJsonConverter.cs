using System.Text.Json;
using System.Text.Json.Serialization;
using TempMail.Client.Models;
using TempMail.Client.Requests;

namespace TempMail.Client.Tests;

internal class CreateEmailRequestJsonConverter : JsonConverter<CreateEmailRequest>
{
    public override CreateEmailRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        if (!reader.Read())
        {
            throw new JsonException();
        }

        if (reader.TokenType == JsonTokenType.EndObject)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }
        
        var propertyName = reader.GetString();

        if (!reader.Read())
        {
            throw new JsonException();
        }
        
        var propertyValue = reader.GetString() ?? throw new JsonException();

        CreateEmailRequest result;
        switch (propertyName)
        {
            case "email":
                result = CreateEmailRequest.ByEmail(propertyValue);
                break;
            case "domain":
                result = CreateEmailRequest.ByDomain(propertyValue);
                break;
            case "domain_type":
                if (!Enum.TryParse<DomainType>(propertyValue, ignoreCase: true, out var domainType))
                {
                    throw new JsonException();
                }
                result = CreateEmailRequest.ByDomainType(domainType);
                break;
            
            default:
                throw new JsonException();
        }

        while (reader.TokenType != JsonTokenType.EndObject)
        {
            reader.Read();
        }
        
        return result ?? throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, CreateEmailRequest value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case CreateEmailByEmailRequest emailRequest:
                JsonSerializer.Serialize(writer, emailRequest, options);
                break;
            case CreateEmailByDomainRequest emailRequest:
                JsonSerializer.Serialize(writer, emailRequest, options);
                break;
            case CreateEmailByDomainTypeRequest emailRequest:
                JsonSerializer.Serialize(writer, emailRequest, options);
                break;
            default:
                throw new JsonException("Invalid CreateEmailRequest");
        }
    }
}