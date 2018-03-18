using System;
using BOA.Common.Types;
using Bridge;

namespace BOA.Types.Kernel.Account
{
    [ObjectLiteral]
    [Serializable]
    public class AccountComponentAccountsContract : ContractBase
    {
        #region Public Properties
        [Name("accountNumber")]
        public int AccountNumber { get; set; }

        [Name("accountSuffix")]
        public short AccountSuffix { get; set; }
        #endregion
    }
}