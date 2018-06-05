using MS.IoT.Common;
using MS.IoT.Domain.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Repositories.Helpers
{
    public static class InMemoryLambdaExpressionsHelper
    {
        private static bool Initialized = false;
        private static Dictionary<string, Func<DeviceTwinFlatModel, string>> _FieldsString;
        private static Dictionary<string, Func<DeviceTwinFlatModel, int>> _FieldsInteger;
        private static Dictionary<string, Func<DeviceTwinFlatModel, double>> _FieldsDouble;
        private static Dictionary<string, Func<DeviceTwinFlatModel, DateTime>> _FieldsDate;
        private static Dictionary<string, Func<DeviceTwinFlatModel, bool>> _FieldsBool;
        private static List<DeviceFieldModel> _Fields;

        public static Dictionary<string, Func<DeviceTwinFlatModel, string>> FieldsStringLambdas
        {
            get
            {
                InitExpressions();
                return _FieldsString;
            }
        }
        public static Dictionary<string, Func<DeviceTwinFlatModel, int>> FieldsIntegerLambdas
        {
            get
            {
                InitExpressions();
                return _FieldsInteger;
            }
        }
        public static Dictionary<string, Func<DeviceTwinFlatModel, DateTime>> FieldsDateLambdas
        {
            get
            {
                InitExpressions();
                return _FieldsDate;
            }
        }
        public static Dictionary<string, Func<DeviceTwinFlatModel, double>> FieldsDoubleLambdas
        {
            get
            {
                InitExpressions();
                return _FieldsDouble;
            }
        }
        public static Dictionary<string, Func<DeviceTwinFlatModel, bool>> FieldsBoolLambdas
        {
            get
            {
                InitExpressions();
                return _FieldsBool;
            }
        }

        public static List<DeviceFieldModel> Fields
        {
            get
            {
                InitExpressions();
                return _Fields;
            }
        }

        public static Func<DeviceTwinFlatModel, string, bool> Test = null;

        public static void InitExpressions()
        {
            if (Initialized)
                return;

            _FieldsString = new Dictionary<string, Func<DeviceTwinFlatModel, string>>();
            _FieldsInteger = new Dictionary<string, Func<DeviceTwinFlatModel, int>>();
            _FieldsDouble = new Dictionary<string, Func<DeviceTwinFlatModel, double>>();
            _FieldsDate = new Dictionary<string, Func<DeviceTwinFlatModel, DateTime>>();
            _FieldsBool = new Dictionary<string, Func<DeviceTwinFlatModel, bool>>();
            _Fields = new List<DeviceFieldModel>();

            try
            {
                InitFieldsByObject(new DeviceTwinFlatModel().GetType(), new List<string>(), new List<string>());
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            Initialized = true;
        }

        private static void InitFieldsByObject(Type type, List<string> parentProperties, List<string> parentPropertiesJSON)
        {
            foreach (PropertyInfo property in type.GetProperties())
            {
                var jsonAttributes = property.GetCustomAttributes<JsonPropertyAttribute>(false);
                if (jsonAttributes.Count() > 0)
                {
                    string jsonField = (parentPropertiesJSON.Count > 0 ? string.Join(".", parentPropertiesJSON) + "." : string.Empty) + jsonAttributes.ToArray()[0].PropertyName;
                    List<string> subProperties = new List<string>(parentProperties);
                    subProperties.Add(property.Name);
                    if (property.PropertyType == typeof(string))
                    {
                        _FieldsString.Add(jsonField, GetLambdaExpressionForGroupByString(subProperties).Compile());
                        _Fields.Add(new DeviceFieldModel(jsonField, FieldTypes.String));
                    }
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        _FieldsDate.Add(jsonField, GetLambdaExpressionForGroupByDate(subProperties).Compile());
                        _Fields.Add(new DeviceFieldModel(jsonField, FieldTypes.Date));
                    }
                    else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(DeviceConnectionStatus))
                    {
                        _FieldsInteger.Add(jsonField, GetLambdaExpressionForGroupByInteger(subProperties).Compile());
                        _Fields.Add(new DeviceFieldModel(jsonField, FieldTypes.Integer));
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        _FieldsDouble.Add(jsonField, GetLambdaExpressionForGroupByDouble(subProperties).Compile());
                        _Fields.Add(new DeviceFieldModel(jsonField, FieldTypes.Double));
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        _FieldsBool.Add(jsonField, GetLambdaExpressionForGroupByBool(subProperties).Compile());
                        _Fields.Add(new DeviceFieldModel(jsonField, FieldTypes.Boolean));
                    }
                    else
                    {
                        List<string> subPropertiesJSON = new List<string>(parentPropertiesJSON);
                        subPropertiesJSON.Add(jsonAttributes.ToArray()[0].PropertyName);
                        InitFieldsByObject(property.PropertyType, subProperties, subPropertiesJSON);
                    }
                }
            };
        }

        /// <summary>
        /// Generate a lambda expression for group by
        /// </summary>
        /// <param name="propertyName">Field</param>
        /// <returns></returns>
        private static Expression<Func<DeviceTwinFlatModel, string>> GetLambdaExpressionForGroupByString(List<string> propertyNames)
        {
            var parameterExp = Expression.Parameter(typeof(DeviceTwinFlatModel), "x");
            Expression body = parameterExp;
            for (var i = 0; i < propertyNames.Count; i++)
                body = Expression.Property(body, propertyNames[i]);
            body = Expression.Condition(Expression.NotEqual(body, Expression.Constant(null)), body, Expression.Constant(string.Empty));
            return Expression.Lambda<Func<DeviceTwinFlatModel, string>>(body, parameterExp);
        }

        /// <summary>
        /// Generate a lambda expression for group by
        /// </summary>
        /// <param name="propertyName">Field</param>
        /// <returns></returns>
        private static Expression<Func<DeviceTwinFlatModel, int>> GetLambdaExpressionForGroupByInteger(List<string> propertyNames)
        {
            var parameterExp = Expression.Parameter(typeof(DeviceTwinFlatModel), "x");
            Expression body = parameterExp;
            for (var i = 0; i < propertyNames.Count; i++)
                body = Expression.Property(body, propertyNames[i]);
            var cast = Expression.Convert(body, typeof(int));
            return Expression.Lambda<Func<DeviceTwinFlatModel, int>>(cast, parameterExp);
        }

        /// <summary>
        /// Generate a lambda expression for group by
        /// </summary>
        /// <param name="propertyName">Field</param>
        /// <returns></returns>
        private static Expression<Func<DeviceTwinFlatModel, DateTime>> GetLambdaExpressionForGroupByDate(List<string> propertyNames)
        {
            var parameterExp = Expression.Parameter(typeof(DeviceTwinFlatModel), "x");
            Expression body = parameterExp;
            for (var i = 0; i < propertyNames.Count; i++)
                body = Expression.Property(body, propertyNames[i]);
            return Expression.Lambda<Func<DeviceTwinFlatModel, DateTime>>(body, parameterExp);
        }

        /// <summary>
        /// Generate a lambda expression for group by
        /// </summary>
        /// <param name="propertyName">Field</param>
        /// <returns></returns>
        private static Expression<Func<DeviceTwinFlatModel, double>> GetLambdaExpressionForGroupByDouble(List<string> propertyNames)
        {
            var parameterExp = Expression.Parameter(typeof(DeviceTwinFlatModel), "x");
            Expression body = parameterExp;
            for (var i = 0; i < propertyNames.Count; i++)
                body = Expression.Property(body, propertyNames[i]);
            return Expression.Lambda<Func<DeviceTwinFlatModel, double>>(body, parameterExp);
        }

        /// <summary>
        /// Generate a lambda expression for group by
        /// </summary>
        /// <param name="propertyName">Field</param>
        /// <returns></returns>
        private static Expression<Func<DeviceTwinFlatModel, bool>> GetLambdaExpressionForGroupByBool(List<string> propertyNames)
        {
            var parameterExp = Expression.Parameter(typeof(DeviceTwinFlatModel), "x");
            Expression body = parameterExp;
            for (var i = 0; i < propertyNames.Count; i++)
                body = Expression.Property(body, propertyNames[i]);
            return Expression.Lambda<Func<DeviceTwinFlatModel, bool>>(body, parameterExp);
        }
    }
}
