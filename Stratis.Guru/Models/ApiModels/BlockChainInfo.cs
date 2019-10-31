namespace Stratis.Guru.Models.ApiModels
{
    public class BlockChainInfo
    {
        public string Chain { get; set; }

        public long Blocks { get; set; }

        public long Headers { get; set; }

        public string BestBlockHash { get; set; }

        public decimal Difficulty { get; set; }

        public long MedianTime { get; set; }

        public decimal VerificationProgress { get; set; }

        public bool IsInitialBlockDownload { get; set; }

        public string Chainwork { get; set; }

        public bool IsPruned { get; set; }
    }
}