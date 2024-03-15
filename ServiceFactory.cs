using Service.Configuration;

namespace Service
{
    public static class ServiceFactory
    {
        public static T Create<T>(Config config) where T : MqttService
        {
            return (T)Activator.CreateInstance(typeof(T), config);
        }

        public static void ServiceRunner<T>(Config config) where T : MqttService
        {
            var service = Create<T>(config);

            Console.WriteLine($"{service.Name} Service Started! Type exit to quit the application.");
            string input = "";
            while (input != "exit")
            {
                input = Console.ReadLine();
            }

            Environment.Exit(0);
        }
    }
}
