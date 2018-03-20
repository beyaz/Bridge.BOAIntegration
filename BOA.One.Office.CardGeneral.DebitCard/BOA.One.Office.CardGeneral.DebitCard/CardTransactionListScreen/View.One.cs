using System.Linq;
using BOA.Common.Helpers;
using Bridge.BOAIntegration;

namespace BOA.UI.CardGeneral.DebitCard.CardTransactionListScreen
{
    public partial class View : BrowsePage
    {
        BOA.UI.BComboEditorMultiSelect MultiSelect;

        object _TransactionDateBeginComponent;

        #region Methods
        

        void EvaluateInWhichCaseRenderMethodWillBeCall()
        {
             this.OnPropertyChanged(nameof(Model), ForceRender);

            this.OnPropertyChanged(nameof(Model), () =>
            {
                if (Model != null)
                {
                    Model.OnPropertyChanged(nameof(Model.TransactionList), () =>
                    {
                        ControlGridDataSource = Model.TransactionList.ToArray();
                    });

                    Model.SearchContract.OnPropertyChanged(nameof(Model.SearchContract.CardNumber), () =>
                    {
                        Model.SearchContract.TransactionDateBegin = Model.SearchContract.TransactionDateBegin.AddDays(-1);

                        

                    });
                }
            });
        }

        void InitializeComponent()
        {
            XmlUI = @" 
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
        <BDateTimePicker 
                        x.Name = '_TransactionDateBeginComponent'
                        Value                   = '{Binding Model.SearchContract.TransactionDateBegin, Mode=TwoWay}' 
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
            EvaluateInWhichCaseRenderMethodWillBeCall();
        }
        #endregion
    }
}