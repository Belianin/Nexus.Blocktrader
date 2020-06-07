using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nexus.Blocktrader.Domain;

namespace Nexus.Blocktrader.Api
{
    public class ExchangeTitleJsonConverter : JsonConverter<ExchangeTitle>
    {
        public override ExchangeTitle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (ExchangeTitle) Enum.Parse(typeof(ExchangeTitle), reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, ExchangeTitle value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}