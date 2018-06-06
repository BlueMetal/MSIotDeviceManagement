import { FieldTypes, LogicalOperators, ComparisonOperators, OrderByType, ConnectionState } from './enums';

declare module MSIoTDeviceManagementPortalModels {
    export type NotificationType = 'info' | 'warning' | 'error';

    export type NotificationMessage = {
		Message: string;
		Type: NotificationType
    }

    export type Alert = {
        code: number;
        message: string;
    }

	export type User = {
		readonly id: string;
		readonly name: string;
	}

	export type Device = {
		readonly id: string;
		readonly deviceId: string;
		productModel: string;
		productLine: string;
		location: Location;
		retailer: string;
		retailerLocation: Location;
		activationDate: Date;
		shipDate: Date;
		creationDate: Date;
		modifiedDate: Date;
		createdBy: string;
		modifiedBy: string;
		properties: CustomProperty[]
	}

    export type DeviceTwin = {
        readonly id: string;
        readonly deviceId: string;
        tags: Tags;
        desired: DesiredProperty;
        reported: ReportedProperty;
    }

    export type DatatableConfig = {
        itemsPerPage: number;
        paginationMaxSize: number;
        tabs: TabConfig[];
        searchFields: string[];
    };

    export type DatatableField = {
        name: string;
        displayName: string;
        class: string;
        rendering?: Function;
    }

    export type TabConfig = {
        title: string;
        fields: DatatableField[];
        orderByDefault: string;
        orderBySortingDefault?: OrderByType;
        groupBy?: string;
        content?: string;
        disabled?: boolean;
        rendering?: Function;
    }

    export type TopActivatedGroup = {
        title: string;
        groupBy: string;
    }

    export type DatatableState = {
        isDatabaseLoaded: boolean;
        isDatabaseLoading: boolean;
        isDatabaseLocationUpdating: boolean;
        itemsPerPage: number;
        tab: TabConfig;
        orderBy: string;
        orderBySorting: OrderByType;
        searchTerms: string[];
        root: NodeState;
    }

    export type NodeState = {
        isOpened: boolean;
        isLoaded: boolean;
        pageIndex: number;
        depth: number;
        selectedItems: string[];
        items: DeviceInfoEntity[];
        itemsCount: number;
        groups: DeviceGroup[];
        pageItemsCount: number;
        filters: DeviceQueryRule[];
        parent?: NodeState;
    }

    export type DeviceQueryConfiguration = {
        pageIndex: number;
        itemsPerPage: number;
        orderBy?: string;
        orderBySorting?: OrderByType;
        where: DeviceQueryRuleGroup;
        groupBy?: string;
    }

    export type DeviceField = {
        name: string;
        type: FieldTypes;
    }

    export type DeviceMapQueryConfiguration = {
        viewId: string;
        filters?: DeviceQueryRule[];
    }

    export type DeviceQueryRuleGroup = {
        operator: LogicalOperators;
        rules: DeviceQueryRule[];
        groups: DeviceQueryRuleGroup[];
        depth?: number;
    }

    export type DeviceQueryRule = {
        field?: string;
        value?: string;
        operator?: ComparisonOperators;
    }

    export type DeviceQueryResponse = {
        itemsCount: number;
        items: DeviceInfoEntity[];
        groupsCount: number;
        groups: DeviceGroup[];
        isDatabaseLoaded: boolean;
        isDatabaseLoading: boolean;
        isDatabaseLocationUpdating: boolean;
    }

    export type DeviceMapQueryResponse = {
        pushpins: DeviceMapEntity[];
        isDatabaseLoaded: boolean;
        isDatabaseLoading: boolean;
        isDatabaseLocationUpdating: boolean;
    }

    export type DeviceMapAreaQueryResponse = {
        areaItems: any[];
        isDatabaseLoaded: boolean;
        isDatabaseLoading: boolean;
        isDatabaseLocationUpdating: boolean;
    }

    export type DeviceMapGroupsInformation = {
        retailers: MapFilterItem[];
        productFamilies: MapFilterItem[];
        connectionStates: MapFilterItem[];
        count: number;
    }

    export type DeviceGroup = {
        name: string;
        displayName: string;
        itemsIds: string[];
        //parent: DeviceGroup;
        state: NodeState
    }

    export type DeviceInfoEntity = {
		readonly deviceId: string;
		readonly productName: string;
        readonly productFamily: string;
        readonly connectionState: ConnectionState;
        readonly retailerName: string;
        readonly retailerRegion: string;
        readonly statusCode: number;
        readonly productRegion: string;
        readonly productCity: string;
        readonly productCountry: string;
    }

    export type DeviceMapEntity = {
        readonly name: string;
        readonly count: number;
        readonly lat: string;
        readonly lon: string;
    }

	export type Tags = {
		productName: string;
		productFamily: boolean;
		manufacturedDate: Date;
		retailerName: string;
        shippedDate: Date;
		location: DeviceIpLocation;
	}

	export type DesiredProperty = {
        features: any[];
	}

	export type ReportedProperty = {
		statusCode: number;
		isConnected: boolean;
		firmwareVersion: number;
		heartbeat: Date;
		ipAddress: string;
        featuresDefinitions : FeatureDefinition[]
    }

    export type DeviceTwinUpdateAsyncModel = {
        deviceIds: string[];
        jsonDesired?: string;
        jsonTags?: string;
    }

    export type DeviceTwinUpdateSyncModel = {
        deviceId: string;
        jsonDesired?: string;
        jsonTags?: string;
    }

    export type CustomProperty = {
        readonly name: string;
        readonly value: string;
    }

    export type Location = {
        addressLine1: string;
        addressLine2: string;
        city: string;
        state: string;
        zipcode: string;
        additionalNotes: string;
        formattedAddress: string;
        latitude: string;
        longitude: string;
	}

	export type DeviceIpLocation = {
		ip: string;
		country_code: string;
		country_name: string;
		region_code: string;
		region_name: string;
		city: string;
		zip_code: string;
		time_zone: string;
		latitude: string;
		longitude: string;
		metro_code: string;
	}

	export type DeviceTwinMetaData = {
		$lastUpdated: string;	
    }

    export type DevicePerGroupActivated = {
        name: string;
        percentageActivated: number;
    }

    export type DeviceAlertCount = {
        alert: number;
        count: number;
        description?: string;
    }

	export type DeviceTwinSummary = {
		totalDevicesCount: number;
		activatedDevicesCount: number;
		notActivatedDevicesCount: number;
		connectedDevicesCount: number;
        disconnectedDevicesCount: number;
        alertCounts: DeviceAlertCount[];
        devicePerGroupActivated: DevicePerGroupActivated[];
    }

	export type DeviceTwinDesiredFeatures = {
		deviceId: string;
        features: FeatureDefinition[];
    }

    export type FeatureDefinition = {
        name: string;
        displayName: string;
        isActivated: boolean;
    }

    export type MapConfig = {
        initConfig: Microsoft.Maps.IMapLoadOptions;
        zoomAreasConfig: ZoomAreaConfig[];
        colorsAreaCount: ColorsAreasConfig[];
        colorsAreaActivationPercentage: ColorsAreasConfig[];
        deviceCountPinsColor: string;
        dateOptions: any;
    }

    export type ZoomAreaConfig = {
        id: number;
        zoomLevelMax: number;
        entityType: string;
    }

    export type ColorsAreasConfig = {
        color: string;
    }

    export type ZoomPushpinsConfig = {
        id: number;
        zoomLevelMax: number;
        entityType: string;
    }

    export type MapState = {
        initialized: boolean;
        currentZoom: number;
        currentBoundaries: number[];
        //eventViewChanged: Microsoft.Maps.IHandlerId;
        area: MapAreaState;
        pushpins: MapPushpinsState;
        filters: MapFiltersState;
        pinViewId: string;
        areaViewId: string;
        retailers: MapFilterItem[];
        productFamilies: MapFilterItem[];
        connectionStates: MapFilterItem[];
        legend: string;
    }

    export type MapFiltersState = {
        filterRetailer: MapFilterItem;
        filterProductFamily: MapFilterItem;
        filterConnectionStatus: MapFilterItem;
        filterActivationDateMin?: number;
        filterActivationDateMax?: number;
        filterShippingDateMin?: number;
        filterShippingDateMax?: number;
        isActivationDateMinVisible: boolean;
        isActivationDateMaxVisible: boolean;
        isShippingDateMinVisible: boolean;
        isShippingDateMaxVisible: boolean;
    }

    export type MapAreaState = {
        listObjectsArea: Microsoft.Maps.IPrimitive[];
        listHandlerIdsArea: Microsoft.Maps.IHandlerId[];
        infoboxArea: Microsoft.Maps.Infobox;
        lastZoomAreaConfigId: number;
        isLoading: boolean;
    }

    export type MapPushpinsState = {
        listHandlerIdsPushpins: Microsoft.Maps.IHandlerId[];
        infoboxPushpins: Microsoft.Maps.Infobox;
        pushpinsLayer?: Microsoft.Maps.ClusterLayer;
        lastZoomPushpinsConfigId: number;
        isLoading: boolean;
    }

    export type MapFilterItem = {
        name: string;
        displayName: string;
        count: number;
	}

	export type PublishFeatureType = {
		name: string;
		value: string;
		methodName: string;
    }

    export type CustomGroupItem = {
        id?: string;
        name: string;
        where: DeviceQueryRuleGroup;
        count: number;
        order: number;
    }

    export type UpdatePropertyDefinition = {
        name: string;
        type: FieldTypes;
        value?: any;
        isNull?: boolean;
        children?: UpdatePropertyDefinition[];
        allowedChildrenTypes?: FieldTypes[];
    }

    export type DeviceUpdateResult = {
        hasSucceeded: boolean;
        jobId?: string;
        errorReason?: string;
    }
} 

export = MSIoTDeviceManagementPortalModels;