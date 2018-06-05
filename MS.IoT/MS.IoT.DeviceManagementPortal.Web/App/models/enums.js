var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var ConnectionStatus;
    (function (ConnectionStatus) {
        ConnectionStatus[ConnectionStatus["Connected"] = 0] = "Connected";
        ConnectionStatus[ConnectionStatus["Disconnected"] = 1] = "Disconnected";
        ConnectionStatus[ConnectionStatus["NotActivated"] = 2] = "NotActivated";
    })(ConnectionStatus = MSIoTDeviceManagementPortal.ConnectionStatus || (MSIoTDeviceManagementPortal.ConnectionStatus = {}));
    var OrderByType;
    (function (OrderByType) {
        OrderByType[OrderByType["Ascending"] = 0] = "Ascending";
        OrderByType[OrderByType["Descending"] = 1] = "Descending";
    })(OrderByType = MSIoTDeviceManagementPortal.OrderByType || (MSIoTDeviceManagementPortal.OrderByType = {}));
    var ComparisonOperators;
    (function (ComparisonOperators) {
        ComparisonOperators[ComparisonOperators["Equals"] = 0] = "Equals";
        ComparisonOperators[ComparisonOperators["NotEquals"] = 1] = "NotEquals";
        ComparisonOperators[ComparisonOperators["Contains"] = 2] = "Contains";
        ComparisonOperators[ComparisonOperators["DoesNotContains"] = 3] = "DoesNotContains";
        ComparisonOperators[ComparisonOperators["StartsWith"] = 4] = "StartsWith";
        ComparisonOperators[ComparisonOperators["EndsWith"] = 5] = "EndsWith";
        ComparisonOperators[ComparisonOperators["Greater"] = 6] = "Greater";
        ComparisonOperators[ComparisonOperators["GreaterOrEqual"] = 7] = "GreaterOrEqual";
        ComparisonOperators[ComparisonOperators["Lesser"] = 8] = "Lesser";
        ComparisonOperators[ComparisonOperators["LesserOrEqual"] = 9] = "LesserOrEqual";
    })(ComparisonOperators = MSIoTDeviceManagementPortal.ComparisonOperators || (MSIoTDeviceManagementPortal.ComparisonOperators = {}));
    var FieldTypes;
    (function (FieldTypes) {
        FieldTypes[FieldTypes["String"] = 0] = "String";
        FieldTypes[FieldTypes["Number"] = 1] = "Number";
        FieldTypes[FieldTypes["Double"] = 2] = "Double";
        FieldTypes[FieldTypes["Date"] = 3] = "Date";
        FieldTypes[FieldTypes["Boolean"] = 4] = "Boolean";
        FieldTypes[FieldTypes["Object"] = 5] = "Object";
    })(FieldTypes = MSIoTDeviceManagementPortal.FieldTypes || (MSIoTDeviceManagementPortal.FieldTypes = {}));
    var LogicalOperators;
    (function (LogicalOperators) {
        LogicalOperators[LogicalOperators["And"] = 0] = "And";
        LogicalOperators[LogicalOperators["Or"] = 1] = "Or";
    })(LogicalOperators = MSIoTDeviceManagementPortal.LogicalOperators || (MSIoTDeviceManagementPortal.LogicalOperators = {}));
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=enums.js.map