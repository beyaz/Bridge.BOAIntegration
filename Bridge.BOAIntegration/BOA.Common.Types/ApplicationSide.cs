﻿using Bridge;

namespace BOA.Common.Types
{
    public enum DialogResponses
    {
        None,
        Ok,
        Yes,
        No,
        Cancel,
    }

    public enum ApplicationSide
    {
        Client                   = 0,
        Server                   = 1,
        Database                 = 2,
        DatabaseConnection       = 3,
        Transportation           = 4,
        Dispatcher               = 5,
        Pipeline                 = 6,
        BroadcastTransportation  = 7,
        Orchestration            = 8,
        AccountingEngine         = 9,
        CommissionEngine         = 10,
        SlipEngine               = 11,
        TellerEngine             = 12,
        AuthorizationEngine      = 13,
        FutureTransactionEngine  = 14,
        RevokeEngine             = 15,
        WorkflowEngine           = 16,
        AccountControl           = 17,
        ReverseAccountControl    = 18,
        RuleEngine               = 19,
        FraudEngine              = 20,
        TransactionAuthorization = 21
    }

    [External]
    [ObjectLiteral]
    public class ApplicationContext
    {
        [Name("user")]
        public UserContract User { get; set; }
    }


    [External]
    [ObjectLiteral]
    public class UserContract
    {
        [Name("branchId")]
        public short BranchId { get; set; }
    }

    [External]
    [ObjectLiteral]
    public class ResourceActionContract
    {
        [Name("commandName")]
        public string CommandName { get; set; }

        [Name("actionId")]
        public short ActionId { get; set; }
        
    }

    

}