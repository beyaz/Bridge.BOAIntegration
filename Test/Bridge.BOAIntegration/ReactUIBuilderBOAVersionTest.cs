using Bridge.Html5;
using Bridge.QUnit;

namespace Bridge.BOAIntegration
{
    class ReactUIBuilderBOAVersionTest
    {
        #region Public Methods
        public static void Register()
        {
            QUnit.QUnit.Test(nameof(BOAIntegration) + "->" + nameof(False_attributes_mut_be_handle_correctly), False_attributes_mut_be_handle_correctly);
            QUnit.QUnit.Test(nameof(BOAIntegration) + "->" + nameof(Numeric_attributes_mut_be_handle_correctly), Numeric_attributes_mut_be_handle_correctly);
        }
        #endregion

        #region Methods
        internal static void False_attributes_mut_be_handle_correctly(Assert assert)
        {
            var componentProp = JSON.Parse("{\"isVisibleBalance\":\"false\"}");

            // ACT
            ReactUIBuilderBOAVersion.EvaluateBooleanValues("BAccountComponent", componentProp);

            // ASSERT
            assert.Equal("boolean", Script.Write<string>(" typeof ( componentProp.isVisibleBalance )"));
        }

        internal static void Numeric_attributes_mut_be_handle_correctly(Assert assert)
        {
            var componentProp = JSON.Parse("{\"rows\":\"5\"}");

            // ACT
            ReactUIBuilderBOAVersion.EvaluateNumberValues("BInput", componentProp);

            // ASSERT
            assert.Equal("number", Script.Write<string>(" typeof ( componentProp.rows )"));
        }
        #endregion
    }
}