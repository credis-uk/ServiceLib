using Newtonsoft.Json;

namespace Service.Packets
{
    public interface IPacket
    {
        public static string Topic { get; }

        public static string Serialize<T>(T packet) where T : IPacket
        {
            return JsonConvert.SerializeObject(packet);
        }
    }
}
