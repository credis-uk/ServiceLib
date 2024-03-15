using Newtonsoft.Json;

namespace Service.Configuration
{
    public class Config
    {
        public string MqttIp { get; set; } = "127.0.0.1";

        public static T Load<T>(string path) where T : Config
        {
            if (!File.Exists(path)) // If the file does not exist, create it with default values
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(Activator.CreateInstance<T>(), Formatting.Indented));
            }

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}
