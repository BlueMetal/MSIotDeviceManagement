using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Repositories.Helpers
{
    public static class InMemoryLinqDeviceQueryHelper
    {
        private static Dictionary<string, Func<DeviceTwinFlatModel, string>> _StringFields;
        private static Dictionary<string, Func<DeviceTwinFlatModel, int>> _IntegerFields;
        private static Dictionary<string, Func<DeviceTwinFlatModel, double>> _DoubleFields;
        private static Dictionary<string, Func<DeviceTwinFlatModel, DateTime>> _DateFields;
        private static Dictionary<string, Func<DeviceTwinFlatModel, bool>> _BoolFields;

        public static IEnumerable<DeviceGroup> GetDevicesGroups(IEnumerable<DeviceTwinFlatModel> filteredQuery, string groupByField, OrderBySort sortBy)
        {
            IEnumerable<DeviceGroup> groups = null;

            if (InMemoryLambdaExpressionsHelper.FieldsStringLambdas.ContainsKey(groupByField))
                groups = filteredQuery.GroupBy(InMemoryLambdaExpressionsHelper.FieldsStringLambdas[groupByField]).Select(p => new DeviceGroup() { Name = p.Key, ItemsCount = p.Count(), ItemsIds = p.Select(x => x.DeviceId) });
            else if (InMemoryLambdaExpressionsHelper.FieldsIntegerLambdas.ContainsKey(groupByField))
                groups = filteredQuery.GroupBy(InMemoryLambdaExpressionsHelper.FieldsIntegerLambdas[groupByField]).Select(p => new DeviceGroup() { Name = p.Key.ToString(), ItemsCount = p.Count(), ItemsIds = p.Select(x => x.DeviceId) });
            else if (InMemoryLambdaExpressionsHelper.FieldsDateLambdas.ContainsKey(groupByField))
                groups = filteredQuery.GroupBy(InMemoryLambdaExpressionsHelper.FieldsDateLambdas[groupByField]).Select(p => new DeviceGroup() { Name = p.Key.ToShortDateString(), ItemsCount = p.Count(), ItemsIds = p.Select(x => x.DeviceId) });
            else if (InMemoryLambdaExpressionsHelper.FieldsDoubleLambdas.ContainsKey(groupByField))
                groups = filteredQuery.GroupBy(InMemoryLambdaExpressionsHelper.FieldsDoubleLambdas[groupByField]).Select(p => new DeviceGroup() { Name = p.Key.ToString(), ItemsCount = p.Count(), ItemsIds = p.Select(x => x.DeviceId) });
            else if (InMemoryLambdaExpressionsHelper.FieldsBoolLambdas.ContainsKey(groupByField))
                groups = filteredQuery.GroupBy(InMemoryLambdaExpressionsHelper.FieldsBoolLambdas[groupByField]).Select(p => new DeviceGroup() { Name = p.Key.ToString(), ItemsCount = p.Count(), ItemsIds = p.Select(x => x.DeviceId) });
            else
                return new List<DeviceGroup>();

            groups = sortBy == OrderBySort.Ascending ? groups.OrderBy(p => p.Name) : groups.OrderByDescending(p => p.Name);

            return groups;
        }

        public static IEnumerable<IGrouping<string, DeviceTwinFlatModel>> GetGroupByString(IEnumerable<DeviceTwinFlatModel> filteredQuery, string groupByField)
        {
            if (InMemoryLambdaExpressionsHelper.FieldsStringLambdas.ContainsKey(groupByField))
                return filteredQuery.GroupBy(InMemoryLambdaExpressionsHelper.FieldsStringLambdas[groupByField]);

            return null;
        }

        public static IEnumerable<DeviceTwinFlatModel> GetOrderedDevices(IEnumerable<DeviceTwinFlatModel> devices, string orderByField, OrderBySort orderBySort)
        {
            if (string.IsNullOrEmpty(orderByField))
                return devices;

            switch (orderBySort)
            {
                case OrderBySort.Ascending:
                    if (InMemoryLambdaExpressionsHelper.FieldsStringLambdas.ContainsKey(orderByField))
                        return devices.OrderBy(InMemoryLambdaExpressionsHelper.FieldsStringLambdas[orderByField]);
                    else if (InMemoryLambdaExpressionsHelper.FieldsIntegerLambdas.ContainsKey(orderByField))
                        return devices.OrderBy(InMemoryLambdaExpressionsHelper.FieldsIntegerLambdas[orderByField]);
                    else if (InMemoryLambdaExpressionsHelper.FieldsDateLambdas.ContainsKey(orderByField))
                        return devices.OrderBy(InMemoryLambdaExpressionsHelper.FieldsDateLambdas[orderByField]);
                    else if (InMemoryLambdaExpressionsHelper.FieldsDoubleLambdas.ContainsKey(orderByField))
                        return devices.OrderBy(InMemoryLambdaExpressionsHelper.FieldsDoubleLambdas[orderByField]);
                    else if (InMemoryLambdaExpressionsHelper.FieldsBoolLambdas.ContainsKey(orderByField))
                        return devices.OrderBy(InMemoryLambdaExpressionsHelper.FieldsBoolLambdas[orderByField]);
                    else
                        return devices;
                case OrderBySort.Descending:
                    if (InMemoryLambdaExpressionsHelper.FieldsStringLambdas.ContainsKey(orderByField))
                        return devices.OrderByDescending(InMemoryLambdaExpressionsHelper.FieldsStringLambdas[orderByField]);
                    else if (InMemoryLambdaExpressionsHelper.FieldsIntegerLambdas.ContainsKey(orderByField))
                        return devices.OrderByDescending(InMemoryLambdaExpressionsHelper.FieldsIntegerLambdas[orderByField]);
                    else if (InMemoryLambdaExpressionsHelper.FieldsDateLambdas.ContainsKey(orderByField))
                        return devices.OrderByDescending(InMemoryLambdaExpressionsHelper.FieldsDateLambdas[orderByField]);
                    else if (InMemoryLambdaExpressionsHelper.FieldsDoubleLambdas.ContainsKey(orderByField))
                        return devices.OrderByDescending(InMemoryLambdaExpressionsHelper.FieldsDoubleLambdas[orderByField]);
                    else if (InMemoryLambdaExpressionsHelper.FieldsBoolLambdas.ContainsKey(orderByField))
                        return devices.OrderByDescending(InMemoryLambdaExpressionsHelper.FieldsBoolLambdas[orderByField]);
                    else
                        return devices;

            }
            return devices;
        }

        public static IEnumerable<DeviceTwinFlatModel> GetFilteredDevicesGroup(IEnumerable<DeviceTwinFlatModel> devices, DeviceQueryRuleGroup filterGroup)
        {
            _StringFields = InMemoryLambdaExpressionsHelper.FieldsStringLambdas;
            _IntegerFields = InMemoryLambdaExpressionsHelper.FieldsIntegerLambdas;
            _DoubleFields = InMemoryLambdaExpressionsHelper.FieldsDoubleLambdas;
            _DateFields = InMemoryLambdaExpressionsHelper.FieldsDateLambdas;
            _BoolFields = InMemoryLambdaExpressionsHelper.FieldsBoolLambdas;

            if (filterGroup.Operator == LogicalOperators.And)
                return devices.Where(GetFilteredDevicesAnd(devices, filterGroup));
            return devices.Where(GetFilteredDevicesOr(devices, filterGroup));
        }

        public static IEnumerable<string> GetFilteredDevicesIds(IEnumerable<DeviceTwinFlatModel> devices, DeviceQueryRuleGroup filterGroup)
        {
            _StringFields = InMemoryLambdaExpressionsHelper.FieldsStringLambdas;
            _IntegerFields = InMemoryLambdaExpressionsHelper.FieldsIntegerLambdas;
            _DoubleFields = InMemoryLambdaExpressionsHelper.FieldsDoubleLambdas;
            _DateFields = InMemoryLambdaExpressionsHelper.FieldsDateLambdas;
            _BoolFields = InMemoryLambdaExpressionsHelper.FieldsBoolLambdas;

            if (filterGroup.Operator == LogicalOperators.And)
                return devices.Where(GetFilteredDevicesAnd(devices, filterGroup)).Select(p => p.DeviceId);
            return devices.Where(GetFilteredDevicesOr(devices, filterGroup)).Select(p => p.DeviceId);
        }

        private static Func<DeviceTwinFlatModel,bool> GetFilteredDevicesAnd(IEnumerable<DeviceTwinFlatModel> devices, DeviceQueryRuleGroup filterGroup)
        {
            var predicate = PredicateBuilder.True<DeviceTwinFlatModel>();
            if (filterGroup != null)
            {
                //First, dealing with groups
                foreach (var subFilterGroup in filterGroup.Groups)
                {
                    if (subFilterGroup == null || (subFilterGroup.Rules.Count == 0 && subFilterGroup.Groups.Count == 0))
                        continue;
                    if (subFilterGroup.Operator == LogicalOperators.And)
                        predicate = predicate.And(GetFilteredDevicesAnd(devices, subFilterGroup));
                    else
                        predicate = predicate.And(GetFilteredDevicesOr(devices, subFilterGroup));
                }

                foreach (DeviceQueryRule rule in filterGroup.Rules)
                {
                    predicate = predicate.And(GetFilteredDevices(devices, rule));
                }
            }

            return predicate;
        }

        private static Func<DeviceTwinFlatModel, bool> GetFilteredDevicesOr(IEnumerable<DeviceTwinFlatModel> devices, DeviceQueryRuleGroup filterGroup)
        {
            var predicate = PredicateBuilder.False<DeviceTwinFlatModel>();
            if (filterGroup != null)
            {
                //First, dealing with groups
                foreach (var subFilterGroup in filterGroup.Groups)
                {
                    if (subFilterGroup == null || (subFilterGroup.Rules.Count == 0 && subFilterGroup.Groups.Count == 0))
                        continue;
                    if (subFilterGroup.Operator == LogicalOperators.And)
                        predicate = predicate.Or(GetFilteredDevicesAnd(devices, subFilterGroup));
                    else
                        predicate = predicate.Or(GetFilteredDevicesOr(devices, subFilterGroup));
                }

                foreach (DeviceQueryRule rule in filterGroup.Rules)
                {
                    if(rule != null)
                        predicate = predicate.Or(GetFilteredDevices(devices, rule));
                }
            }

            return predicate;
        }

        private static Func<DeviceTwinFlatModel, bool> GetFilteredDevices(IEnumerable<DeviceTwinFlatModel> devices, DeviceQueryRule filter)
        {
            //String filter
            Func<DeviceTwinFlatModel, bool> predicate = null;

            if (_StringFields.ContainsKey(filter.Field))
            {
                string value = filter.Value.ToLower();
                switch (filter.ComparisonOperator)
                {
                    case ComparisonOperators.Contains:
                        predicate = p => _StringFields[filter.Field](p).ToLower().Contains(value);
                        break;
                    case ComparisonOperators.DoesNotContains:
                        predicate = p => !_StringFields[filter.Field](p).ToLower().Contains(value);
                        break;
                    case ComparisonOperators.Equals:
                        predicate = p => _StringFields[filter.Field](p).ToLower() == value;
                        break;
                    case ComparisonOperators.NotEquals:
                        predicate = p => _StringFields[filter.Field](p).ToLower() != value;
                        break;
                    case ComparisonOperators.StartsWith:
                        predicate = p => _StringFields[filter.Field](p).ToLower().StartsWith(value);
                        break;
                    case ComparisonOperators.EndsWith:
                        predicate = p => _StringFields[filter.Field](p).ToLower().EndsWith(value);
                        break;
                }
            }
            else if (_IntegerFields.ContainsKey(filter.Field))
            {
                int value = 0;
                if (int.TryParse(filter.Value, out value))
                {
                    switch (filter.ComparisonOperator)
                    {
                        case ComparisonOperators.Equals:
                            predicate = p => _IntegerFields[filter.Field](p) == value;
                            break;
                        case ComparisonOperators.NotEquals:
                            predicate = p => _IntegerFields[filter.Field](p) != value;
                            break;
                        case ComparisonOperators.Greater:
                            predicate = p => _IntegerFields[filter.Field](p) > value;
                            break;
                        case ComparisonOperators.GreaterOrEqual:
                            predicate = p => _IntegerFields[filter.Field](p) >= value;
                            break;
                        case ComparisonOperators.Lesser:
                            predicate = p => _IntegerFields[filter.Field](p) < value;
                            break;
                        case ComparisonOperators.LesserOrEqual:
                            predicate = p => _IntegerFields[filter.Field](p) <= value;
                            break;
                    }
                }
            }
            else if (_DoubleFields.ContainsKey(filter.Field))
            {
                double value = 0;
                if (double.TryParse(filter.Value, out value))
                {
                    switch (filter.ComparisonOperator)
                    {

                        case ComparisonOperators.Equals:
                            predicate = p => _DoubleFields[filter.Field](p) == value;
                            break;
                        case ComparisonOperators.NotEquals:
                            predicate = p => _DoubleFields[filter.Field](p) != value;
                            break;
                        case ComparisonOperators.Greater:
                            predicate = p => _DoubleFields[filter.Field](p) > value;
                            break;
                        case ComparisonOperators.GreaterOrEqual:
                            predicate = p => _DoubleFields[filter.Field](p) >= value;
                            break;
                        case ComparisonOperators.Lesser:
                            predicate = p => _DoubleFields[filter.Field](p) < value;
                            break;
                        case ComparisonOperators.LesserOrEqual:
                            predicate = p => _DoubleFields[filter.Field](p) <= value;
                            break;
                    }
                }
            }
            else if (_DateFields.ContainsKey(filter.Field))
            {
                double ticksDate = 0;
                if (double.TryParse(filter.Value, out ticksDate))
                {
                    DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds(ticksDate).ToLocalTime();
                    switch (filter.ComparisonOperator)
                    {
                        case ComparisonOperators.Equals:
                            predicate = p => _DateFields[filter.Field](p) == date;
                            break;
                        case ComparisonOperators.NotEquals:
                            predicate = p => _DateFields[filter.Field](p) != date;
                            break;
                        case ComparisonOperators.Greater:
                            predicate = p => _DateFields[filter.Field](p) > date;
                            break;
                        case ComparisonOperators.GreaterOrEqual:
                            predicate = p => _DateFields[filter.Field](p) >= date;
                            break;
                        case ComparisonOperators.Lesser:
                            predicate = p => _DateFields[filter.Field](p) < date;
                            break;
                        case ComparisonOperators.LesserOrEqual:
                            predicate = p => _DateFields[filter.Field](p) <= date;
                            break;
                    }
                }
            }
            else if (_BoolFields.ContainsKey(filter.Field))
            {
                bool boolValue = false;
                if (bool.TryParse(filter.Value, out boolValue))
                {
                    switch (filter.ComparisonOperator)
                    {
                        case ComparisonOperators.Equals:
                            predicate = p => _BoolFields[filter.Field](p) == boolValue;
                            break;
                        case ComparisonOperators.NotEquals:
                            predicate = p => _BoolFields[filter.Field](p) != boolValue;
                            break;
                    }
                }
            }
            return predicate;
        }

        public static List<DeviceFieldModel> Fields
        {
            get
            {
                return InMemoryLambdaExpressionsHelper.Fields;
            }
        }
    }
}
