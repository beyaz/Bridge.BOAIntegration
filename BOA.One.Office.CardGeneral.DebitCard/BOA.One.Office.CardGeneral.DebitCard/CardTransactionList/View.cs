using System.Threading.Tasks;
using BOA.Common.Types;
using BOA.Types.CardGeneral.DebitCard;
using BOA.Types.Kernel.DebitCard;
using Bridge;
using Bridge.BOAIntegration;

namespace BOA.One.Office.CardGeneral.DebitCard.CardTransactionList
{
    public class View : BrowsePage
    {

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
        async Task getExternalResponseCodesCommand()
        {
            var proxyRequest = new ProxyRequest<CardTransactionRequest>
            {
                RequestClass = "BOA.Types.CardGeneral.DebitCard.CardTransactionRequest",
                RequestBody = new CardTransactionRequest
                {
                    MethodName = "GetExternalResponseCodes"
                },
                Key = "GetExternalResponseCodes"
            };

            var response = await Execute<GenericResponse<ExternalResponseCodeContract[]>>(proxyRequest);

            if (!response.Success)
            {
                ShowError(Message.ErrorOccurredWhileFetchingData, response.Results);
                return;
            }

            SetState(new ViewState
            {
                ExternalResponseCodeList = response.Value
            });
        }
        #endregion
    }
}