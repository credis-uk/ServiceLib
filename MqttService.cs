using Service.Configuration;
using Service.Enums;
using Service.Packets;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Service
{
    public abstract class MqttService
    {
        public delegate void PacketHandler(string message);

        public virtual string Name => throw new NotImplementedException();

        public Dictionary<string, PacketHandler> Subcriptions = new Dictionary<string, PacketHandler>();

        protected MqttClient Client;

        protected Config Config;

        public MqttService(Config config)
        {
            Config = config;
            Client = new MqttClient(IPAddress.Parse(config.MqttIp));
            Client.MqttMsgPublishReceived += HandlePacket;
            Client.Connect(Name);
        }

        ~MqttService()
        {
            Client.Disconnect();
        }

        public void Publish<T>(string topic, T packet, bool retain = false) where T : IPacket
        {
            Client.Publish(topic, Encoding.UTF8.GetBytes(IPacket.Serialize(packet)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, retain);
        }

        public void Log(LogLevel level, string message, string stacktrace = "")
        {
            Publish(LogPacket.Topic, new LogPacket { Level = level, Message = message, StackTrace = stacktrace });
        }

        protected void Subscribe(string topic, PacketHandler handler)
        {
            if (!Subcriptions.ContainsKey(topic))
            {
                Subcriptions.Add(topic, handler);
                Client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
        }

        private void HandlePacket(object sender, MqttMsgPublishEventArgs arguments)
        {
            try
            {
                string msg = Encoding.UTF8.GetString(arguments.Message);
                Subcriptions.Where(s => s.Key == arguments.Topic).FirstOrDefault().Value(msg);
            }
            catch (Exception ex)
            {
                Log(LogLevel.Warning, ex.Message, ex.StackTrace);
            }
        }
    }
}
