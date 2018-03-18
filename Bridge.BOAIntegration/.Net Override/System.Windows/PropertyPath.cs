using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace System.Windows
{
    public class PropertyPath
    {
        #region Fields
        internal readonly List<Trigger> Triggers = new List<Trigger>();

        bool _pathLastNodeIsReachable = true;
        #endregion

        #region Constructors
        public PropertyPath(string path)
        {
            Path = path;
        }
        #endregion

        #region Public Properties
        public string Path { get; }
        #endregion

        #region Properties
        internal bool IsNotReadyToUpdate => !_pathLastNodeIsReachable;
        Trigger       LastTrigger        => Triggers[Triggers.Count - 1];
        #endregion

        #region Public Methods
        public static implicit operator PropertyPath(string path)
        {
            return new PropertyPath(path);
        }

        public void Clear()
        {
            Triggers.ForEach(t => t.StopListen());
            Triggers.Clear();
            _pathLastNodeIsReachable = true;
        }

        public object GetPropertyValue()
        {
            if (Triggers.Count == 0)
            {
                throw new InvalidOperationException("PropertyPathProblem:" + Path);
            }

            var lastTrigger  = LastTrigger;
            var instance     = lastTrigger.Instance;
            var propertyName = lastTrigger.PropertyName;

            return GetPropertyValue(instance, propertyName);
        }

        public void Listen(object instance, Action onPropertyValueChanged)
        {
            Walk(instance);

            var len = Triggers.Count;

            for (var i = 0; i < len; i++)
            {
                var trigger = Triggers[i];

                trigger.OnPropertyValueChanged = () =>
                {
                    Listen(instance, onPropertyValueChanged);
                    onPropertyValueChanged();
                };
                trigger.Listen();
            }
        }

        public void SetPropertyValue(object value)
        {
            var lastTrigger  = LastTrigger;
            var instance     = lastTrigger.Instance;
            var propertyName = lastTrigger.PropertyName;

            var propertyType = ReflectionHelper.FindProperty(instance, propertyName)?.PropertyType;
            if (propertyType != null)
            {
                value = Cast.To(value, propertyType, CultureInfo.CurrentCulture);
            }

            SetPropertyValue(instance, propertyName, value);
        }

        public void Walk(object instance)
        {
            Clear();

            ParsePath(instance, Path);
        }
        #endregion

        #region Methods
        internal void ParsePath(object instance, string path)
        {
            while (true)
            {
                if (instance == null)
                {
                    _pathLastNodeIsReachable = false;
                    return;
                }

                var firstDat = path.IndexOf('.');

                if (firstDat < 0)
                {
                    Triggers.Add(new Trigger
                    {
                        Instance     = instance,
                        PropertyName = path
                    });
                    return;
                }

                var propertyName = path.Substring(0, firstDat);

                Triggers.Add(new Trigger
                {
                    Instance     = instance,
                    PropertyName = propertyName
                });

                instance = GetPropertyValue(instance, propertyName);

                path = path.Substring(firstDat + 1);
            }
        }

        static object GetPropertyValue(object instance, string propertyName)
        {
            if (instance.GetType() == typeof(object))
            {
                return instance[propertyName];
            }

            return ReflectionHelper.GetPropertyValue(instance, propertyName,BindingFlags.Public|BindingFlags.Instance|BindingFlags.IgnoreCase);
        }

        static void SetPropertyValue(object instance, string propertyName, object value)
        {
            if (instance.GetType() == typeof(object))
            {
                instance[propertyName] = value;
                return;
            }

            ReflectionHelper.SetPropertyValue(instance, propertyName, value);
        }
        #endregion

        internal class Trigger
        {
            #region Fields
            public object Instance;

            public Action OnPropertyValueChanged;
            public string PropertyName;
            #endregion

            #region Properties
            INotifyPropertyChanged InstanceAsNotifyPropertyChanged => Instance as INotifyPropertyChanged;
            #endregion

            #region Public Methods
            public void Listen()
            {
                if (InstanceAsNotifyPropertyChanged == null)
                {
                    return;
                }

                InstanceAsNotifyPropertyChanged.PropertyChanged += OnChange;
            }

            public void StopListen()
            {
                if (InstanceAsNotifyPropertyChanged == null)
                {
                    return;
                }

                InstanceAsNotifyPropertyChanged.PropertyChanged -= OnChange;
            }

            public override string ToString()
            {
                return Instance + "->" + PropertyName;
            }
            #endregion

            #region Methods
            void OnChange(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == PropertyName)
                {
                    OnPropertyValueChanged();
                }
            }
            #endregion
        }
    }
}