using System;

namespace Bridge.BOAIntegration
{
    class Component_1 : Component
    {
        #region Properties
        Component_1_Prop Props
        {
            get { return this["props"].As<Component_1_Prop>(); }
        }
        #endregion

        #region Public Methods
        public override ReactElement render()
        {
            var prop = ObjectLiteral.Create<object>();
            prop["href"] = Props.Name5;

            return ReactElement.Create("a", prop);
        }
        #endregion
    }
}