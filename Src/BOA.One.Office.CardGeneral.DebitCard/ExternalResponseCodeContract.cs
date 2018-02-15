﻿using BOA.Common.Types;
using Bridge;

namespace BOA.One.Office.CardGeneral.DebitCard
{
    [ObjectLiteral]
    public class ExternalResponseCodeContract : ContractBase
    {
        public int    externalResponseCode { get; set; }
        public string description          { get; set; }
    }
}