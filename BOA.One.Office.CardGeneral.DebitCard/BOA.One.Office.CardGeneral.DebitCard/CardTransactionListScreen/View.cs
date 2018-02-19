using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BOA.Common.Types;
using BOA.Messaging;
using BOA.Types.CardGeneral.DebitCard;
using BOA.Types.Kernel.DebitCard;
using BOA.UI.CardGeneral.DebitCard.CardTransactionListScreen;
using BOA.UI.CardGeneral.DebitCard.Common.Data;
using BOA.UI.Types;
using Bridge;
using Bridge.BOAIntegration;
using Bridge.Html5;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BOA.One.Office.CardGeneral.DebitCard.CardTransactionListScreen
{
    public class View : BrowsePage
    { /// <summary>
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
            

            return true;
        }


        /// <summary>
        ///     Searches the specified search values.
        /// </summary>
        async Task<IReadOnlyCollection<DebitTransactionSearchResultContract>> Search(DebitTransactionSearchContract searchValues)
        {
            var request = new CardTransactionRequest
            {
                MethodName     = "Search",
                SearchContract = searchValues
            };
            return await ExecuteAsync<List<DebitTransactionSearchResultContract>>(request);
        }

        /// <summary>
        ///     Gets the information execute.
        /// </summary>
        async Task GetInfoExecute()
        {
            var searchValues = Model.SearchContract;
            



            searchValues.ExternalResponseCodes = Model.SelectedExternalResponseCodes.Where(x => x.IsSelected).Select(x => ((ExternalResponseCodeContract)x).ExternalResponseCode).ToList();

            Model.TransactionList = await Search(searchValues);

            Model.TransactionList = new[]
            {
                new DebitTransactionSearchResultContract
                {
                    InternalResponseCodeDescription = "Aloha",
                    DebitTransaction = new DebitTransactionContract
                    {
                        CardNumber = "hhhtö9"
                    }
                },
                new DebitTransactionSearchResultContract
                {
                    InternalResponseCodeDescription = "Aloha3",
                    DebitTransaction = new DebitTransactionContract
                    {
                        CardNumber = "y62"
                    }
                }
            };


            var viewState = new ViewState();
            viewState["dataSource"] = Model.TransactionList.ToArray();

            SetState(viewState);

            
        }

        /// <summary>
        ///     Gets the get information command.
        /// </summary>
        public ICommand GetInfoCommand
        {
            get { return new DelegateCommand(async () => { await GetInfoExecute(); }, CanGetInfoExecute); }
        }

        /// <summary>
        ///     Gets the clear command.
        /// </summary>
        public ICommand ClearCommand
        {
            get { return new DelegateCommand(async () => { await CleanExecute(); }, () => true); }
        }

        async Task clearCommand()
        {
            await CleanExecute();
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
                    TransactionDateBegin        = DateTime.Today.AddYears(-5),
                    TransactionDateEnd          = DateTime.Today,
                    ProcessTransactionTimeBegin = "00:00",
                    ProcessTransactionTimeEnd   = "23:59",
                    ExternalResponseCodes       = new List<int>()
                },
                TransactionList               = new List<DebitTransactionSearchResultContract>(),
                ExternalResponseCodes         = (await GetExternalResponseCodes()).ToArray(),
                SelectedExternalResponseCodes = new List<ContractBase>()
            };

            model.ExternalResponseCodes.ToList().ForEach(x => x.IsSelected = true);

            Model = model;

            
            
        }


        Model _model;

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

        public Message Message { get; set; } = new Message();

        #region Constructors

        public View()
        {
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Model")
                {
                    forceUpdate();
                }

            };

            this.ConfigureColumns(GetColumnInformations());
        }
        #endregion

        #region Public Methods


        public ReactElement render()
        {
            var dataContext = this;

            var xmlUi = @" 
<BGridSection>

    <BGridRow>
        <BAccountComponent  
                AccountNumber    = '{Binding Model.SearchContract.AccountNumber, Mode=TwoWay}'
                isVisibleBalance = 'false' 
                isVisibleIBAN    = 'false' />
    </BGridRow>

     <BGridRow>
        <BInputMask type              = 'CreditCard'
                    Value             = '{Binding Model.SearchContract.CardNumber, Mode=TwoWay}'
                    hintText          = '{Model.Label.CardNumber}'
                    floatingLabelText = '{Model.Label.CardNumber}'  />
    </BGridRow>

    <BGridRow>
        <BDateTimePicker Value                  = '{Binding Model.SearchContract.TransactionDateBegin, Mode=TwoWay}' 
                        floatingLabelTextDate   = '{Binding Model.Label.TransactionStartDate}' />
    </BGridRow>

    <BGridRow>
        
        <BDateTimePicker Value                  = '{Binding Model.SearchContract.TransactionDateEnd, Mode=TwoWay}' 
                        floatingLabelTextDate   = '{Binding Model.Label.TransactionDeadline}' />
    </BGridRow>

    <BGridRow>

        <BComboBox
            labelText       = '{Binding Model.Label.CodeOfActionAnswer}'
            dataSource      = '{Binding Model.ExternalResponseCodes, Mode=TwoWay}'
            SelectedItems   = '{Binding Model.SelectedExternalResponseCodes, Mode=TwoWay}'

            displayLabelSeperator   =','
            multiSelect             ='true'
            multiColumn             ='true'
            isAllOptionIncluded     ='true'
            valueMemberPath         ='ExternalResponseCode'
            displayMemberPath       = 'Description'>
            <BComboBox.Columns>
                <ComboBoxColumn key = 'Description'  Name='{Binding Model.Label.ResponseCodeNumber}'  />
            </BComboBox.Columns>


        </BComboBox>

    </BGridRow>

</BGridSection>
";

           
            return BuildUI(xmlUi, dataContext);
        }


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


        #endregion

        #region Methods
        static T ConvertToBridgeGeneratedType<T>(object jsonValue)
        {
              var jsonString = JSON.Stringify(jsonValue);

            return JsonConvert.DeserializeObject<T>(jsonString,new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

        }
        async Task<TResponseValueType> ExecuteAsync<TResponseValueType>(RequestBase request)
        {
            var response = await base.ExecuteAsync<RequestBase, GenericResponse<TResponseValueType>>(request);
            if (response.Success)
            {
                var responseValue = response.Value;

                responseValue =  ConvertToBridgeGeneratedType<TResponseValueType>(responseValue);

                return responseValue;
            }

            throw new InvalidOperationException(string.Join(Environment.NewLine, response.Results.Select(r => r.ErrorMessage)));
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


        async Task getExternalResponseCodesCommand()
        {
            SetState(new ViewState
            {
                ExternalResponseCodeList = (await GetExternalResponseCodes()).ToArray()
            });
        }
        #endregion
    }
}