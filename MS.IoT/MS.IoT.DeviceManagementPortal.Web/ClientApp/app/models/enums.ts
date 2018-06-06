export enum ConnectionState {
    Connected = 0,
    Disconnected = 1,
    NotActivated = 2
}

export enum OrderByType {
    Ascending = 0,
    Descending = 1
}

export enum ComparisonOperators {
    Equals = 0,
    NotEquals = 1,
    Contains = 2,
    DoesNotContains = 3,
    StartsWith = 4,
    EndsWith = 5,
    Greater = 6,
    GreaterOrEqual = 7,
    Lesser = 8,
    LesserOrEqual = 9
}

export enum FieldTypes {
    String = 0,
    Number = 1,
    Double = 2,
    Date = 3,
    Boolean = 4,
    Object = 5
}

export enum LogicalOperators {
    And = 0,
    Or = 1
}