using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nexus.Blocktrader.Models;

namespace Nexus.Blocktrader.Api
{
    public class TickerJsonConverter : JsonConverter<Ticker>
    {
        public override Ticker Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (Ticker) Enum.Parse(typeof(Ticker), reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, Ticker value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}