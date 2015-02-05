using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Inside
{
    public class JsonUtcConverter : IJsonConverter
    {
        private JsonSerializerSettings setting;

        public JsonUtcConverter()
        {
            setting = new JsonSerializerSettings();
            setting.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        }

        public string Serilize(object value)
        {
            return JsonConvert.SerializeObject(value, setting);
        }

        public string Serilize(object value, IsoDateTimeConverter converter)
        {
            return JsonConvert.SerializeObject(value, converter);
        }

        public T Deserilize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public T Deserilize<T>(string value, IsoDateTimeConverter converter)
        {
            return JsonConvert.DeserializeObject<T>(value, converter);
        }
    }
}