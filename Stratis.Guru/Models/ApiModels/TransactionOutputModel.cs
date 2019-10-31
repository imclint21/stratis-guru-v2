using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.Models.ApiModels
{
    public class TransactionOutputModel
    {
        public int Index { get; set; }

        public string Address { get; set; }

        public long Balance { get; set; }

        public string OutputType { get; set; }

        public string ScriptPubKey { get; set; }

        public string ScriptPubKeyAsm { get; set; }

        public string SpentInTransaction { get; set; }
    }
}
