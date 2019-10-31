namespace Stratis.Guru.Models.ApiModels
{
    public class NetworkInfo
    {
        public int Version { get; set; }

        public string SubVersion { get; set; }

        public int ProtocolVersion { get; set; }

        public string LocalServices { get; set; }

        public bool IsLocalRelay { get; set; }

        public int TimeOffset { get; set; }

        public int Connections { get; set; }

        public bool IsNetworkActive { get; set; }

        public decimal RelayFee { get; set; }

        public decimal IncrementalFee { get; set; }
    }
}