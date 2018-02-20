using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BOA.UI.Types
{
    public enum DialogTypes
    {
        Info,
        Error,
        Warning,
        Question,
    }

    public class BOADelegateCommand
    {
        internal bool CanRun = true;
        internal event EventHandler AfterExecuteCommand;

        internal event EventHandler BeforeExecuteCommand;
        public DateTime? StartTime
        {
            get;
            protected set;
        }
        public DateTime? FinishTime
        {
            get;
            protected set;
        }
        public string CommandName
        {
            get;
            set;
        }
        public string ActionName
        {
            get;
            internal set;
        }
        public short? ActionId
        {
            get;
            internal set;
        }
        public int ResourceId
        {
            get;
            internal set;
        }
        public double? ExecutionDuration
        {
            get
            {
                if (this.StartTime.HasValue && this.FinishTime.HasValue)
                {
                    return new double?((this.FinishTime - this.StartTime).Value.TotalMilliseconds);
                }
                return null;
            }
        }
        protected internal void BeforeExecuteCommandMethod()
        {
            if (this.BeforeExecuteCommand != null)
            {
                this.BeforeExecuteCommand(this, null);
            }
        }
        protected internal void AfterExecuteCommandMethod()
        {
            if (this.AfterExecuteCommand != null)
            {
                this.AfterExecuteCommand(this, null);
            }
            this.StartTime = null;
            this.FinishTime = null;
        }
    }
    public class DelegateCommandBase: BOADelegateCommand
    {
        private readonly Func<bool> _canExecuteMethod;
        public bool CanExecute()
        {
            return this._canExecuteMethod == null || this._canExecuteMethod();
        }
        public DelegateCommandBase(Func<bool> canExecuteMethod) 
        {
            this._canExecuteMethod = canExecuteMethod;
        }
    }
    public class DelegateCommand: DelegateCommandBase,ICommand
    {

        bool ICommand.CanExecute(object parameter)
        {
            return base.CanExecute();
        }
        void ICommand.Execute(object parameter)
        {
            this.Execute();
        }

        private readonly Action _executeMethod;
        public DelegateCommand(Action executeMethod)
            : this(executeMethod, null)
        {
        }

       

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base(canExecuteMethod)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod));
            }
            this._executeMethod = executeMethod;
        }

        public void Execute()
        {
            if (!this.CanRun)
            {
                this.CanRun = true;
                return;
            }
            base.BeforeExecuteCommandMethod();
            base.StartTime = new DateTime?(DateTime.Now);
            if (this._executeMethod != null)
            {
                this._executeMethod();
            }
            base.FinishTime = new DateTime?(DateTime.Now);
            base.AfterExecuteCommandMethod();
        }
    }
}
