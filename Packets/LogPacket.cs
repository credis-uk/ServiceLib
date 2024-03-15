using Service.Enums;

namespace Service.Packets
{
    public struct LogPacket : IPacket
    {
        public static string Topic => "LOG";

        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
