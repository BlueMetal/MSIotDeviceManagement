using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.UWP.CoffeeMaker.Models
{

    public enum VariableType
    {
        BYTE = 0,
        BOOL = 1,
        SHORT = 2,
        INT = 3,
        TIME = 4
    };

    public class VariableModel
    {
        public string VariableName { get; set; }
        public string PropertyName { get; set; }
        public VariableType PropertyType { get; set; }
        public object PropertyValue { get; set; }

        public VariableModel(string variableName, string propertyName, VariableType propertyType)
        {
            VariableName = variableName;
            PropertyName = propertyName;
            PropertyType = propertyType;
        }
    }
}
