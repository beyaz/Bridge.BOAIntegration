using System;
using System.Windows.Input;

namespace System.Windows.Input
{
    /// <summary>Represents the method that will handle mouse button related routed events, for example <see cref="E:System.Windows.UIElement.MouseLeftButtonDown" />. </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);
}

namespace System.Windows.Input
{
    /// <summary>Provides data for mouse button related events. </summary>
    public class MouseButtonEventArgs :EventArgs
    {
       
    }
}


namespace BOA.UI
{
    public class BComboEditorMultiSelect
    {
       
    }

    public static class BComboEditorMultiSelect_Extensions
    {
        public static void UpdateText(this BComboEditorMultiSelect multiSelect)
        {

        }
    }

    public class BDataGrid
    {
        // TODO invoke here
        public event MouseButtonEventHandler MouseDoubleClick;

        public T GetActiveDataItem<T>()
        {
            // TODO: implement here
            throw new NotImplementedException();
        }
    }
}


namespace BOA.UI
{
    public class BComboEditorLabeled
    {
        public object SelectedIndex { get; set; }
    }
}