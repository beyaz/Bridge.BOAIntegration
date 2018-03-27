using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BOA.Common.Types;
using BOA.UI.Utils;
using Bridge;
using Bridge.BOAIntegration;
using Bridge.jQuery2;

namespace BOA.UI
{
    public class WindowBase : INotifyPropertyChanged
    {
        public FormManager FormManager
        {
            get { return Utils.FormManager.Instance; }
        }

        public void SetState<T>(T state) where T : BState
        {
            setState(state);
        }
        protected void ForceRender()
        {
            if (TypeScriptVersion == null)
            {
                return;
            }

            forceUpdate();
        }

        [Template("$TypeScriptVersion.forceUpdate()")]
        protected extern void forceUpdate();

        [Template("$TypeScriptVersion.setState({0})")]
        protected extern void setState(object state);

        public bool CanExecuteAction(string commandName)
        {
            return true;
        }
        public object Data => State.PageParams.Data;

         public ApplicationContext ApplicationContext => State?.Context?.ApplicationContext;

        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
        public BState State { [Template("$TypeScriptVersion.state")] get; }

        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
        public object TypeScriptVersion { [Template("$TypeScriptVersion")] get; }


        public event EventHandler LoadCompleted;
        protected string XmlUI { get; set; }
        public virtual void LoadData() 
        {
        }
        public int RenderCount { get; private set; }
        public ReactElement render()
        {
            if (RenderCount == 0)
            {
                jQuery.Ready(() =>
                {
                    LoadData();
                    LoadCompleted?.Invoke(this, null);
                });
            }

            RenderCount++;

            return BuildUI(XmlUI);
        }
        protected ReactElement BuildUI(string xmlUI)
        {
            var reactUiBuilder = new ReactUIBuilderBOAVersion
            {
                XmlUI                     = xmlUI,
                DataContext               = this,
                Caller                    = this,
                TypeScriptWrittenJsObject = TypeScriptVersion
            };

            return reactUiBuilder.Build();
        }

        #region Events
        internal event Action<string, object> OnProxyDidRespond;
        #endregion

        #region Public Methods

        public async Task<Response> ExecuteAsync<Request, Response>(Request request) where Request : RequestBase, new() where Response : ResponseBase
        {
            var responseType = typeof(Response);

            var proxyRequest = new ProxyRequest
            {
                RequestClass = request.GetType().FullName,
                RequestBody  = request,
                Key          = request.MethodName + "-" + Guid.NewGuid()
            };

            var response = await ServiceCallExecuter.Call(proxyRequest, this);

            var responseAsDotnetInstance = Utility.ConvertBOAJsonObjectToDotnetInstance(response, responseType);


            return responseAsDotnetInstance.As<Response>();
        }

        public async Task<TResponseValueType> ExecuteAsync<TResponseValueType>(RequestBase request)
        {
            var response = await ExecuteAsync<RequestBase, GenericResponse<TResponseValueType>>(request);
            if (response.Success)
            {
                return response.Value;
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

        [Template("$TypeScriptVersion.proxyTransactionExecute({0})")]
        public extern void ProxyTransactionExecute(object requestContainer);

        
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
            Delegate _fulfilledHandler;
            ProxyRequest _request;
            dynamic  _response;
            dynamic  _view;
            #endregion

            #region Properties
            WindowBase View
            {
                get { return _view; }
            }
            #endregion

            #region Public Methods
            public void Then(Delegate fulfilledHandler, Delegate errorHandler = null, Delegate progressHandler = null)
            {
                _fulfilledHandler = fulfilledHandler;

                View.OnProxyDidRespond += proxyDidRespondHandler;


                var boaJsonObject = Utility.ConvertDotnetInstanceToBOAJsonObject(_request);

                if (_request.RequestBody is TransactionRequestBase)
                {
                    View.ProxyTransactionExecute(boaJsonObject);
                    return;
                }

                View.ProxyExecute(boaJsonObject);
            }
            #endregion

            #region Methods
            internal static async Task<object> Call(ProxyRequest request, object view)
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
                if (key == _request.Key)
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