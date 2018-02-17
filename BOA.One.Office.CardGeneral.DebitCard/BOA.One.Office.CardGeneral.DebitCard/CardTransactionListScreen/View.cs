using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOA.Common.Types;
using BOA.Types.CardGeneral.DebitCard;
using BOA.Types.Kernel.DebitCard;
using BOA.UI.CardGeneral.DebitCard.CardTransactionListScreen;
using Bridge;
using Bridge.BOAIntegration;

namespace BOA.One.Office.CardGeneral.DebitCard.CardTransactionListScreen
{
    public class View : BrowsePage
    {


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
                    TransactionDateBegin        = DateTime.Today.AddDays(-10),
                    TransactionDateEnd          = DateTime.Today,
                    ProcessTransactionTimeBegin = "00:00",
                    ProcessTransactionTimeEnd   = "23:59",
                    ExternalResponseCodes       = new List<int>()
                },
                TransactionList               = new List<DebitTransactionSearchResultContract>(),
                ExternalResponseCodes         = await GetExternalResponseCodes(),
                SelectedExternalResponseCodes = new List<ContractBase>()
            };

            model.ExternalResponseCodes.ForEach(x => x.IsSelected = true);

            Model = model;

            SetState(new ViewState{ExternalResponseCodeList = model.ExternalResponseCodes.ToArray() });
            
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
        public View(object props) : base(props)
        {
        }
        #endregion

        #region Public Methods
        public GridColumnInfo[] getGridColumns()
        {
            return new[]
            {
                new GridColumnInfo
                {
                    Key       = "cardNumber",
                    Name      = GetMessage("CardGeneral", "CardNumber"),
                    Width     = 140,
                    Resizable = true
                }
            };
        }

        public ReactElement render()
        {
            var viewState = new ViewState
            {
                ExternalResponseCodeList = new[]
                {
                    new ExternalResponseCodeContract
                    {
                        ExternalResponseCode = 3,
                        Description          = "hh"
                    }
                },
                externalResponseCodeListColumns = new[]
                {
                    new ComboBoxColumn
                    {
                        Key   = "externalResponseCode",
                        Name  = "ResponseCodeNumber",
                        Width = 60,
                        type  = "number"
                    }
                }
            };

            var prop = Script.Write<object>("this.state");

            
            var newProp = ObjectLiteral.Create<object>();

            foreach (var key in Keys(prop))
            {
                newProp[key] = prop[key];
            }

            newProp["externalResponseCodeList"]        = viewState.ExternalResponseCodeList;
            newProp["externalResponseCodeListColumns"] = viewState.externalResponseCodeListColumns;
            prop                                       = newProp;

            return BuildUI(@"
<BGridSection>

    <BGridRow>
        <BAccountComponent  
                accountNumber    = '{windowRequest.searchContract.accountNumber}' 
                isVisibleBalance = 'false' 
                isVisibleIBAN    = 'false'
                onCustomerSelect = 'this.onCustomerSelect'
                />
    </BGridRow>

     <BGridRow>
        <BInputMask  
                type = 'CreditCard' 
                hintText    = 'TODO:KartNumber'                
                />
    </BGridRow>

    <BGridRow>
        <BDateTimePicker  value = '{windowRequest.searchContract.processTransactionTimeBegin}' />
    </BGridRow>

    <BGridRow>
        <BDateTimePicker  />
    </BGridRow>

 <BGridRow>

        <BComboBox
            labelText='commm' 
            dataSource='{externalResponseCodeList}'
            defaultValue = '{windowRequest.searchContract.externalResponseCodes}'
            columns      = '{externalResponseCodeListColumns}'
            displayLabelSeperator=','
            multiSelect='true'
            multiColumn='true'
            isAllOptionIncluded='true'
            valueMemberPath='externalResponseCode'
            displayMemberPath = 'description'
        />

    </BGridRow>  

</BGridSection>
", prop);
        }


        #endregion

        #region Methods


        async Task<TResponseValueType> ExecuteAsync<TResponseValueType>(RequestBase request)
        {
            var response = await base.ExecuteAsync<RequestBase, GenericResponse<TResponseValueType>>(request);
            if (response.Success)
            {
                return response.Value;
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