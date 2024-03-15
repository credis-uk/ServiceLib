namespace Service.Packets
{
    public class TransactionPacket : IPacket
    {
        public static string Topic => "TRANSACTION";

        public string Transaction { get; set; }
    }
}
