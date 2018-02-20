using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BOA.Common.Types;
using BOA.Messaging;
using BOA.Types.CardGeneral.DebitCard;
using BOA.Types.Kernel.DebitCard;
using BOA.UI.CardGeneral.DebitCard.Common.Data;
using BOA.UI.Types;
using Task = System.Threading.Tasks.Task;

namespace BOA.UI.CardGeneral.DebitCard.CardTransactionListScreen
{
    /// <summary>
    ///     The view
    /// </summary>
    public sealed partial class View
    {
        #region Fields
        /// <summary>
        ///     The model
        /// </summary>
        Model _model;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="View" /> class.
        /// </summary>
        public View()
        {
            InitializeComponent();

            this.ConfigureColumns(GetColumnInformations());

            LoadCompleted += async (s, e) => { await OnLoadCompleted(); };
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets the clear command.
        /// </summary>
        public ICommand ClearCommand
        {
            get { return new DelegateCommand(async () => { await CleanExecute(); }, () => true); }
        }

        /// <summary>
        ///     Gets the get information command.
        /// </summary>
        public ICommand GetInfoCommand
        {
            get { return new DelegateCommand(async () => { await GetInfoExecute(); }, CanGetInfoExecute); }
        }

        /// <summary>
        ///     Gets or sets the model.
        /// </summary>
        public Model Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                {
                    _model = value;
                    OnPropertyChanged("Model");
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Gets the column informations.
        /// </summary>
        static IEnumerable<DataGridColumnInfoContract> GetColumnInformations()
        {
            var type = typeof(DebitTransactionContract);
            var resultContractType = typeof(DebitTransactionSearchResultContract);

            return new List<DataGridColumnInfoContract>
            {
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.CardNumber",
                    DataType    = type.TryGetProperType("CardNumber"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "CardNumber")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.AccountNumber",
                    DataType    = type.TryGetProperType("AccountNumber"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "CustomerNumber")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.AccountSuffix",
                    DataType    = type.TryGetProperType("AccountSuffix"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "AnnexNo")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.TransactionDate",
                    DataType    = type.TryGetProperType("TransactionDate"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "TransactionHistory")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.ProcessTransactionTime",
                    DataType    = type.TryGetProperType("ProcessTransactionTime"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "ProcessingTime")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "ExternalTransactionCodeDescription",
                    DataType    = resultContractType.TryGetProperType("ExternalTransactionCodeDescription"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "ProcessName")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.OriginalCurrencyCode",
                    DataType    = type.TryGetProperType("OriginalCurrencyCode"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "OriginalCurrency")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.OriginalAmount",
                    DataType    = type.TryGetProperType("OriginalAmount"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "OriginalAmount")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.BillingCurrencyCode",
                    DataType    = type.TryGetProperType("BillingCurrencyCode"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "TransactionCurrencyType")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.BillingAmount",
                    DataType    = type.TryGetProperType("BillingAmount"),
                    Label       = "İşlem Tutar"
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.Location",
                    DataType    = type.TryGetProperType("Location"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "TransactionLocation")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.ExternalResponseCode",
                    DataType    = type.TryGetProperType("ExternalResponseCode"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "ResponseCodeNumber")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "InternalResponseCodeDescription",
                    DataType    = resultContractType.TryGetProperType("InternalResponseCodeDescription"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "ProcessingStatus")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.IsReversed",
                    DataType    = type.TryGetProperType("IsReversed"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "CancelMi")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.AuthenticationId",
                    DataType    = type.TryGetProperType("AuthenticationId"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "CodeOfProvision")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.SystemTraceAuditNumber",
                    DataType    = type.TryGetProperType("SystemTraceAuditNumber"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "Stan")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.MerchantCode",
                    DataType    = type.TryGetProperType("MerchantCode"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "MCC")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.AcquirerId",
                    DataType    = type.TryGetProperType("AcquirerId"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "CodeOfBank")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "AcquirerIdDescription",
                    DataType    = resultContractType.TryGetProperType("AcquirerIdDescription"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "Bank")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.SettlementAmount",
                    DataType    = type.TryGetProperType("SettlementAmount"),
                    Label       = "İşlem Tutar(USD)"
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.EntryMode",
                    DataType    = type.TryGetProperType("EntryMode"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "PosInputMode")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.TerminalId",
                    DataType    = type.TryGetProperType("TerminalId"),
                    Label       = MessagingHelper.GetMessage("CardGeneral", "TerminalId")
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.BKMUniqueMerchantId",
                    DataType    = type.TryGetProperType("BKMUniqueMerchantId"),
                    Label       = "BKM Id"
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.TransmissionDTS",
                    DataType    = type.TryGetProperType("TransmissionDTS"),
                    Label       = "DTS"
                },
                new DataGridColumnInfoContract
                {
                    BindingPath = "DebitTransaction.RetrievalReferenceNumber",
                    DataType    = type.TryGetProperType("RetrievalReferenceNumber"),
                    Label       = "RRN"
                }
            };
        }

        /// <summary>
        ///     Determines whether this instance [can get information execute].
        /// </summary>
        bool CanGetInfoExecute()
        {
            if (Model == null)
            {
                return false;
            }

            if (Model.IsFreeSearchEnabled)
            {
                return true;
            }

            if (Model.SearchContract.AccountNumber == null &&
                string.IsNullOrWhiteSpace(Model.SearchContract.CardNumber))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Cleans the execute.
        /// </summary>
        async Task CleanExecute()
        {
            var model = new Model
            {
                LabelWidth = 125,

                SearchContract = new DebitTransactionSearchContract
                {
                    TransactionDateBegin = DateTime.Today.AddDays(-10),
                    TransactionDateEnd = DateTime.Today,
                    ProcessTransactionTimeBegin = "00:00",
                    ProcessTransactionTimeEnd = "23:59",
                    ExternalResponseCodes = new List<int>()
                },
                TransactionList = new List<DebitTransactionSearchResultContract>(),
                ExternalResponseCodes = (await GetExternalResponseCodes()).ToArray(),
                IsFreeSearchEnabled = CanExecuteAction("FreeSearch"),
                SelectedExternalResponseCodes = new List<ContractBase>()
            };

            model.ExternalResponseCodes.ToList().ForEach(x => x.IsSelected = true);

            Model = model;

            ClearMultiSelectComponent();
        }

        /// <summary>
        ///     Gets the external response codes.
        /// </summary>
        async Task<List<ExternalResponseCodeContract>> GetExternalResponseCodes()
        {
            var request = new CardTransactionRequest
            {
                MethodName = "GetExternalResponseCodes"
            };
            var list = await ExecuteAsync<List<ExternalResponseCodeContract>>(request);

            list.ForEach(x => x.Description = x.ExternalResponseCode.ToString().PadLeft(2, '0'));

            return list;
        }

        /// <summary>
        ///     Gets the information execute.
        /// </summary>
        async Task GetInfoExecute()
        {
            var searchValues = Model.SearchContract;
            if (string.IsNullOrWhiteSpace(searchValues.CardNumber))
            {
                searchValues.CardNumber = null;
            }

            searchValues.ExternalResponseCodes = Model.SelectedExternalResponseCodes.Where(x => x.IsSelected).Select(x => ((ExternalResponseCodeContract)x).ExternalResponseCode).ToList();

            Model.TransactionList = await Search(searchValues);

            ShowStatusMessage(Model.TransactionList.Count + " adet kayıt getirildi.", DialogTypes.Info);
        }

        async Task OnLoadCompleted()
        {
            await CleanExecute();

            await ProcessDataProperty();
        }

        /// <summary>
        ///     Processes the data property.
        /// </summary>
        async Task ProcessDataProperty()
        {
            var cardNumber = Data as string;
            if (cardNumber != null)
            {
                Model.SearchContract.CardNumber = cardNumber;
                await GetInfoExecute();
                return;
            }

            var searchContract = Data as DebitTransactionSearchContract;

            if (searchContract == null)
            {
                return;
            }

            searchContract.CopyNotNullValues(Model.SearchContract);

            if (searchContract.CardNumber != null)
            {
                await GetInfoExecute();
            }
        }

        /// <summary>
        ///     Searches the specified search values.
        /// </summary>
        async Task<IReadOnlyCollection<DebitTransactionSearchResultContract>> Search(DebitTransactionSearchContract searchValues)
        {
            var request = new CardTransactionRequest
            {
                MethodName = "Search",
                SearchContract = searchValues
            };
            return await ExecuteAsync<List<DebitTransactionSearchResultContract>>(request);
        }
        #endregion
    }
}