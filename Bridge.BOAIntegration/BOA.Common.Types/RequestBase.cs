using System;
using System.Collections.Generic;
using System.ComponentModel;
using Bridge;

namespace BOA.Common.Types
{
    public enum WorkflowResult
    {
        Return             = 1,
        ExecuteMainProcess = 2,
    }
}
namespace BOA.Common.Types
{
    [Serializable]
    public class WorkflowActionReasonContract : ContractBase
    {
        public string ReasonCode    { get; set; }
        public string ReasonName    { get; set; }
        public string Description   { get; set; }
        public int    FlowVerisonId { get; set; }
        public bool   ActiveVersion { get; set; }
        public int    ActionId      { get; set; }
        public int    StateId       { get; set; }
        public int    ResouceId     { get; set; }
        public int    FirstActionId { get; set; }
    }
}

namespace BOA.Common.Types
{
    [Serializable]
    public sealed class WorkFlowRequestInternalData
    {
        public int                                            InstanceId                  { get; set; }
        public string                                         UserDescription             { get; set; }
        public object                                         AttachedFileInformation     { get; set; }
        public int                                            TransactionWorkGroupId      { get; set; }
        public short                                          FirstActionId               { get; set; }
        public List<WorkFlowRequestInternalData.SaveStateBag> SaveStates                  { get; set; }
        public int                                            InstanceUserId              { get; set; }
        public bool                                           LastChanceApproved          { get; set; }
        public int                                            MainInstanceId              { get; set; }
        public int                                            SubInstanceId               { get; set; }
        public List<int>                                      SubInstanceIdList           { get; set; }
        public bool                                           PromoteMainInstance         { get; set; }
        public WorkflowResult                                 SubWorkFlowResult           { get; set; }
        public string                                         PromoteOutSideUserName      { get; set; }
        public List<WorkflowActionReasonContract>             ActionReasons               { get; set; }
        public bool                                           CheckQueryToken             { get; set; }
        public bool?                                          WorkFlowUrgency             { get; set; }
        public short?                                         MoneyTransferType           { get; set; }
        public bool?                                          WorkPriority                { get; set; }
        public bool?                                          IsBoutique                  { get; set; }
        public string                                         PreviousApproverUserList    { get; set; }
        public Dictionary<string, Dictionary<string, object>> ComponentPropertyDictionary { get; set; }
        [Serializable]
        public class SaveStateBag : ContractBase
        {
            public int    StartRunId           { get; set; }
            public short  RouteNumber          { get; set; }
            public string UserDescription      { get; set; }
            public string SaveStateDescription { get; set; }
        }
    }
}


namespace BOA.Common.Types
{
    [Serializable]
    public sealed class WorkFlowRequestData
    {
        public int?         OwnerWorkGroupId                        { get; set; }
        public string       DisplayDescription                      { get; set; }
        public List<string> ApproverUsers                           { get; set; }
        public string       QueryToken                              { get; set; }
        public int          RelationalMainInstanceId                { get; set; }
        public int          RelationalMainStateId                   { get; set; }
        public bool?        IsStateWaitForRelationalFlow            { get; set; }
        public bool?        IsFlowWaitForRelationalFlow             { get; set; }
        public int?         WaitingStateIdForRelationalFlow         { get; set; }
        public bool         ExecuteRelationalFlowMethods            { get; set; }
        public bool         AllowExecutionByMainInstance            { get; set; }
        public string       AllowExecutionByMainInstanceCommandName { get; set; }
        public bool         ResumeMainFlowOnError                   { get; set; }
        public bool         CheckMainInstanceOnCurrentUser          { get; set; }
        public List<string> CompetencyList                          { get; set; }
        public string       Reference                               { get; set; }
        public string       ProductCode                             { get; set; }
        public short?       OperationLocation                       { get; set; }
        public DateTime?    ForwardDate                             { get; set; }
        public string       CorporationName                         { get; set; }
        public int?         PackageId                               { get; set; }
        public string       Counter                                 { get; set; }
        public bool         UpdateWorkflowJournal                   { get; set; }
        public bool         OpenNextAvailableInstance               { get; set; }
        public bool         IsSetOwnerUser                          { get; set; }
        public DateTime?    ValorDate                               { get; set; }
        public string       DivitId                                 { get; set; }
        public string       DivitInstanceId                         { get; set; }
    }
}

namespace BOA.Common.Types
{
    public interface IWorkFlow
    {
        WorkFlowRequestData         WorkFlowData         { get; set; }
        WorkFlowRequestInternalData WorkFlowInternalData { get; set; }
    }
}

namespace BOA.Common.Types
{
    public enum ReverseOperation : byte
    {
        Cancel,
        Delete,
    }
}

namespace BOA.Common.Types
{
    public interface IAccounting
    {
        string           AccountingDescription        { get; set; }
        decimal?         AccountingReverseBusinessKey { get; set; }
        ReverseOperation AccountingReverseOperation   { get; set; }
    }
}
namespace BOA.Common.Types
{
    [Serializable]
    public class RequestBase : BOAMessageBase
    {

        #region INotifyPropertyChanged Members
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
       
        public void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        #region Public Properties
        public short? ActionId { get; set; }

        public int? CustomerId { get; set; }

        public int? LanguageId { get; set; }

        public int? MainAccountNumber { get; set; }

        public short? MainSuffix { get; set; }

        public string MethodName { get; set; }

        public string ResourceCode { get; set; }

        public int? ResourceId { get; set; }

        public string TransactionCode { get; set; }

        public object WindowInstanceId { get; set; }
        #endregion
    }

    [Serializable]
    public class TransactionRequestBase : RequestBase
    {
        #region Public Properties
        public short BranchId { get; set; }

        public bool CallByFutureDated { get; set; }

        public string CustomerName { get; set; }

        public string Description { get; set; }

        public short? FEC { get; set; }

        public bool HasAccounting { get; set; }

        public bool HasAuthorization { get; set; }

        public bool HasCommission { get; set; }

        public bool HasFutureDated { get; set; }

        public bool HasRevokableTransaction { get; set; }

        public bool HasSlip { get; set; }

        public bool HasTellerTransaction { get; set; }

        public bool HasWorkflow { get; set; }

        public decimal? MainAmount { get; set; }

        public int? ToAccountNumber { get; set; }

        public short? ToBranchId { get; set; }

        public short? ToSuffix { get; set; }

        public short TranBranchId { get; set; }
        public DateTime TranDate { get; set; }

        public string TranRef { get; set; }

        public int? TranUnitId { get; set; }
        #endregion
    }
}