using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BOA.Common.Types;
using Bridge.Html5;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bridge.BOAIntegration
{
    public class BasePage : INotifyPropertyChanged
    {
        #region Events
        internal event Action<string, object> OnProxyDidRespond;
        #endregion

        #region Public Methods
        public Task<TResponse> Execute<TResponse>(object proxyRequest)
        {
            return ServiceCallExecuter.Call(proxyRequest, this).As<Task<TResponse>>();
        }

        public Task<Response> ExecuteAsync<Request, Response>(Request request) where Request : RequestBase, new() where Response : ResponseBase
        {
            var proxyRequest = new ProxyRequest<Request>
            {
                RequestClass = request.GetType().FullName,
                RequestBody  = request,
                Key          = request.MethodName + "-" + Guid.NewGuid()
            };

            return ServiceCallExecuter.Call(proxyRequest, this).As<Task<Response>>();
        }

        public async Task<TResponseValueType> ExecuteAsync<TResponseValueType>(RequestBase request)
        {
            var response = await ExecuteAsync<RequestBase, GenericResponse<TResponseValueType>>(request);
            if (response.Success)
            {
                return ConvertToBridgeGeneratedType<TResponseValueType>(response.Value);
            }

            throw new InvalidOperationException(string.Join(Environment.NewLine, response.Results.Select(r => r.ErrorMessage)));
        }

        public void onActionClick(ResourceActionContract resourceAction)
        {
            var propertyName = resourceAction.CommandName + "Command";

            var propertyInfo = GetType().GetProperty(propertyName);

            if (propertyInfo == null)
            {
                throw new ArgumentException(propertyName + " command not found.");
            }

            var command = (ICommand) propertyInfo.GetValue(this);

            command.Execute(null);
        }

        [Template("$TypeScriptVersion.proxyExecute({0})")]
        public extern void ProxyExecute(object requestContainer);
        #endregion

        #region Methods
        static T ConvertToBridgeGeneratedType<T>(object jsonValue)
        {
            var jsonString = JSON.Stringify(jsonValue);

            return JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

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
            Delegate _fulfilledHandler;
            dynamic  _request;
            dynamic  _response;
            dynamic  _view;
            #endregion

            #region Properties
            BasePage View
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

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}