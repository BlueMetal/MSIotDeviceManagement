using MS.IoT.UWP.CoffeeMaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.UWP.CoffeeMaker.Helpers
{
    public class VariableHelper
    {
        private static Dictionary<string, VariableModel> _Variables;

        public static Dictionary<string, VariableModel> Variables { get { return _Variables; } }

        public static void Init()
        {
            if (_Variables != null)
                return;

            _Variables = new Dictionary<string, VariableModel>();

            //Features
            _Variables.Add(FourByFourConstants.FEATURE_VARNAME_BREW_STRENGTH, new VariableModel(
                FourByFourConstants.FEATURE_VARNAME_BREW_STRENGTH,
                FourByFourConstants.PROP_FEATURE_VARNAME_BREW_STRENGTH,
                VariableType.BOOL));
            _Variables.Add(FourByFourConstants.FEATURE_VARNAME_BREW, new VariableModel(
                FourByFourConstants.FEATURE_VARNAME_BREW,
                FourByFourConstants.PROP_FEATURE_VARNAME_BREW,
                VariableType.BOOL));
            _Variables.Add(FourByFourConstants.FEATURE_VARNAME_GRIND_BREW, new VariableModel(
                FourByFourConstants.FEATURE_VARNAME_GRIND_BREW,
                FourByFourConstants.PROP_FEATURE_VARNAME_GRIND_BREW,
                VariableType.BOOL));
            _Variables.Add(FourByFourConstants.FEATURE_VARNAME_WIFI, new VariableModel(
                FourByFourConstants.FEATURE_VARNAME_WIFI,
                FourByFourConstants.PROP_FEATURE_VARNAME_WIFI,
                VariableType.BOOL));
            _Variables.Add(FourByFourConstants.FEATURE_VARNAME_DEBUG, new VariableModel(
                FourByFourConstants.FEATURE_VARNAME_DEBUG,
                FourByFourConstants.PROP_FEATURE_VARNAME_DEBUG,
                VariableType.BOOL));

            //State
            _Variables.Add(FourByFourConstants.VALUE_STATE_BREW_STRENGTH, new VariableModel(
                FourByFourConstants.VALUE_STATE_BREW_STRENGTH,
                FourByFourConstants.PROP_VALUE_STATE_BREW_STRENGTH,
                VariableType.BYTE));
            _Variables.Add(FourByFourConstants.VALUE_STATE_GRIND, new VariableModel(
                FourByFourConstants.VALUE_STATE_GRIND,
                FourByFourConstants.PROP_VALUE_STATE_GRIND,
                VariableType.BYTE));
            _Variables.Add(FourByFourConstants.VALUE_STATE_BREW, new VariableModel(
                FourByFourConstants.VALUE_STATE_BREW,
                FourByFourConstants.PROP_VALUE_STATE_BREW,
                VariableType.BOOL));
            _Variables.Add(FourByFourConstants.VALUE_STATE_BREW_ETA, new VariableModel(
                FourByFourConstants.VALUE_STATE_BREW_ETA,
                FourByFourConstants.PROP_VALUE_STATE_BREW_ETA,
                VariableType.TIME));
            _Variables.Add(FourByFourConstants.VALUE_STATE_BREW_GRIND, new VariableModel(
                FourByFourConstants.VALUE_STATE_BREW_GRIND,
                FourByFourConstants.PROP_VALUE_STATE_BREW_GRIND,
                VariableType.BOOL));
            _Variables.Add(FourByFourConstants.VALUE_STATE_BREW_GRIND_ETA, new VariableModel(
                FourByFourConstants.VALUE_STATE_BREW_GRIND_ETA,
                FourByFourConstants.PROP_VALUE_STATE_BREW_GRIND_ETA,
                VariableType.TIME));
            _Variables.Add(FourByFourConstants.VALUE_STAT_LAST_BREWED, new VariableModel(
                FourByFourConstants.VALUE_STAT_LAST_BREWED,
                FourByFourConstants.PROP_VALUE_STAT_LAST_BREWED,
                VariableType.TIME));
            _Variables.Add(FourByFourConstants.VALUE_STAT_NBR_BREWED_TODAY, new VariableModel(
                FourByFourConstants.VALUE_STAT_NBR_BREWED_TODAY,
                FourByFourConstants.PROP_VALUE_STAT_NBR_BREWED_TODAY,
                VariableType.SHORT));
            _Variables.Add(FourByFourConstants.VALUE_STAT_NBR_BREWED_WEEKLY, new VariableModel(
                FourByFourConstants.VALUE_STAT_NBR_BREWED_WEEKLY,
                FourByFourConstants.PROP_VALUE_STAT_NBR_BREWED_WEEKLY,
                VariableType.SHORT));
            _Variables.Add(FourByFourConstants.VALUE_STAT_NBR_BREWED_TOTAL, new VariableModel(
                FourByFourConstants.VALUE_STAT_NBR_BREWED_TOTAL,
                FourByFourConstants.PROP_VALUE_STAT_NBR_BREWED_TOTAL,
                VariableType.INT));
            _Variables.Add(FourByFourConstants.VALUE_UI_CONNECTION_STATE, new VariableModel(
                FourByFourConstants.VALUE_UI_CONNECTION_STATE,
                FourByFourConstants.PROP_VALUE_UI_CONNECTION_STATE,
                VariableType.BOOL));
        }

        public static void SetVariableValue(string variableName, string value)
        {
            VariableModel variable = _Variables[variableName];
            SetVariableValue(variable, value);
        }

        public static void SetVariableValue(VariableModel variable, string value)
        {
            switch (variable.PropertyType)
            {
                case VariableType.BOOL:
                    variable.PropertyValue = value == "1";
                    break;
                case VariableType.BYTE:
                case VariableType.SHORT:
                case VariableType.INT:
                    variable.PropertyValue = Convert.ToInt32(value);
                    break;
                case VariableType.TIME:
                    double timeValue = Convert.ToDouble(value);
                    DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(timeValue);
                    variable.PropertyValue = date;
                    break;
            }
        }
    }
}
