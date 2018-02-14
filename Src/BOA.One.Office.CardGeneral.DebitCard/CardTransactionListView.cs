﻿using System.Threading.Tasks;
using BOA.Common.Types;
using BOA.Types.CardGeneral.DebitCard;
using Bridge;
using Bridge.BOAIntegration;

namespace BOA.One.Office.CardGeneral.DebitCard
{
    [ObjectLiteral]
    public class CardTransactionListViewState
    {
        #region Public Properties
        [Name("externalResponseCodeList")]
        public ExternalResponseCodeContract[] ExternalResponseCodeList { get; set; }
        #endregion
    }

    public class CardTransactionListView : BrowsePage
    {
        #region Constructors
        public CardTransactionListView(object props) : base(props)
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

        public void SetState(CardTransactionListViewState state)
        {
            base.SetState(state);
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
                // TODO: buralar messages yapısından alınmalı
                ShowError("Veriler getirilirken hata oluştu", response.Results);
                return;
            }

            SetState(new CardTransactionListViewState
            {
                ExternalResponseCodeList = response.Value
            });
            
        }
        #endregion
    }
}