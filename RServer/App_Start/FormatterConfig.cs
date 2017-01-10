using Newtonsoft.Json;
using System.Net.Http.Formatting;

namespace RServer
{
    class FormatterConfig
    {
        public static void ConfigureFormatters(MediaTypeFormatterCollection formatters)
        {
            formatters.Remove(formatters.XmlFormatter);

            JsonSerializerSettings settings = formatters.JsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.None;
        }
    }
}
