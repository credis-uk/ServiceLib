using Service.Enums;

namespace Service.Packets
{
    public class TransactionAuthPacket : TransactionPacket
    {
        public static string Topic => "TRANSACTION_AUTH";
        public TransactionAuthStatus Status { get; set; }
        public float ConfidenceScore { get; set; }
    }
}
