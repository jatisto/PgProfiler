namespace PgProfiler.Data
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class SettingPg
    {
        [JsonProperty("Path")]
        public string? Path { get; set; }

        [JsonProperty("CodeEditor")]
        public string? CodeEditor { get; set; }

        [JsonProperty("Settings")]
        public List<Setting>? Settings { get; set; }
    }

    public class Setting
    {
        [JsonProperty("SettingPg")]
        public ConnectionModel? SettingPg { get; set; }
    }

    public class ConnectionModel
    {
        [JsonProperty("Uid")]
        public string? Uid { get; set; }

        [JsonProperty("ServerDb")]
        public string? ServerDb { get; set; }

        [JsonProperty("NameDb")]
        public string? NameDb { get; set; }

        [JsonProperty("PortDb")]
        public string? PortDb { get; set; }

        [JsonProperty("Login")]
        public string? Login { get; set; }

        [JsonProperty("Password")]
        public string? Password { get; set; }

        [JsonProperty ("IsActive")]
        public bool? IsActive { get; set; } = false;
        
        public bool Control_() => Control_(this);

        private static bool Control_(ConnectionModel? bean)
        {
            if (bean?.ServerDb == null || bean.ServerDb.Trim().Equals(""))
                return false;
            if (bean.PortDb == null || bean.PortDb.Trim().Equals(""))
                return false;
            if (bean.NameDb == null || bean.NameDb.Trim().Equals(""))
                return false;
            if (bean.Login == null || bean.Login.Trim().Equals(""))
                return false;
            if (bean.Password == null || bean.Password.Trim().Equals(""))
                return false;
            if (bean.ServerDb == null || bean.ServerDb.Trim().Equals(""))
                return false;
            return true;
        }
    }

    public partial class SettingPg
    {
        public static SettingPg FromJson(string json) => JsonConvert.DeserializeObject<SettingPg>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SettingPg self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object? ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (long.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
