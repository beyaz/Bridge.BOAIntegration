﻿using System;
using System.Threading.Tasks;
using BOA.One.Office.CardGeneral.DebitCard;

namespace Bridge.BOAIntegration
{
    public class BasePage
    {
        #region Events
        internal event Action<string, object> OnProxyDidRespond;
        #endregion

        #region Public Methods
        public Task<TResponse> Execute<TResponse>(object proxyRequest)
        {
            return ServiceCallExecuter.Call(proxyRequest, this).As<Task<TResponse>>();
        }

        [Name("proxyExecute")]
        public extern void ProxyExecute(object requestContainer);
        #endregion

        #region Methods
        void proxyDidRespond(dynamic proxyResponse)
        {
            string key = proxyResponse.key;

            var response = proxyResponse.response;

            OnProxyDidRespond?.Invoke(key, response);
        }
        #endregion

        class ServiceCallExecuter : IPromise
        {
            #region Fields
            dynamic _request;
            dynamic _view;
            dynamic _response;

            Delegate _fulfilledHandler;
            #endregion

            #region Properties
            CardTransactionListView View
            {
                get { return _view; }
            }
            #endregion

            #region Public Methods
            public void Then(Delegate fulfilledHandler, Delegate errorHandler = null, Delegate progressHandler = null)
            {
                _fulfilledHandler = fulfilledHandler;

                View.OnProxyDidRespond += proxyDidRespondHandler;

                View.ProxyExecute(_request);
            }
            #endregion

            #region Methods
            internal static async Task<object> Call(object request, object view)
            {
                var promise = new ServiceCallExecuter
                {
                    _request = request,
                    _view    = view
                };

                var resultHandler = (Func<ServiceCallExecuter, object>) (r => r._response);

                // TODO: Error message management ? 
                var errorHandler = (Func<ServiceCallExecuter, Exception>) (me => new ArgumentException(me.ToString()));

                var task = Task.FromPromise<string>(promise, resultHandler, errorHandler);

                await task;

                return task.Result;
            }

            void proxyDidRespondHandler(string key, object response)
            {
                if (key == _request.key)
                {
                    _response = response;
                    if (View.OnProxyDidRespond != null)
                    {
                        View.OnProxyDidRespond -= proxyDidRespondHandler;
                    }

                    _fulfilledHandler.Call(null, this);
                }
            }
            #endregion
        }
    }
}