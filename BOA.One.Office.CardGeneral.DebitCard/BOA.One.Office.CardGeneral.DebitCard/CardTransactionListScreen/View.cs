using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BOA.Common.Types;
using BOA.Types.CardGeneral.DebitCard;
using BOA.Types.Kernel.DebitCard;
using BOA.UI.CardGeneral.DebitCard.CardTransactionListScreen;
using Bridge;
using Bridge.BOAIntegration;
using Bridge.Html5;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

        public View(object props) : base(props)
        {
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Model")
                {
                    forceUpdate();
                }

            };
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
        <BDateTimePicker  Value = '{Binding Model.SearchContract.TransactionDateBegin, Mode=TwoWay}' />
    </BGridRow>

    <BGridRow>
        <BDateTimePicker  />
    </BGridRow>

    <BGridRow>

        <BComboBox
            labelText       ='{Binding Model.Label.CodeOfActionAnswer}' 
            dataSource      ='{Binding Model.ExternalResponseCodes, Mode=TwoWay}'
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