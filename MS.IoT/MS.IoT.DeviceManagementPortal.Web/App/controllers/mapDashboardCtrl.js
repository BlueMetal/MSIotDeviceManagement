var MSIoTDeviceManagementPortal;
(function (MSIoTDeviceManagementPortal) {
    var app = angular.module("MSIoTDeviceManagementPortal");
    //Class for the view Home
    var MapDashboardCtrl = /** @class */ (function () {
        //Main Constructor
        function MapDashboardCtrl(currentUser, deviceDBService, $timeout, $sce) {
            this.currentUser = currentUser;
            this.deviceDBService = deviceDBService;
            this.$timeout = $timeout;
            this.initMap();
            this.$sce = $sce;
        }
        MapDashboardCtrl.prototype.initMap = function () {
            var _this = this;
            this.deviceDBService.getMapGroupsInformations().then(function (response) {
                response.connectionStates.forEach(function (filter) {
                    filter.displayName = _this.getConnectionStatusString(filter.name);
                });
                Microsoft.Maps.loadModule('Microsoft.Maps.SpatialDataService', function () {
                    //Create a legend. 
                    //this.createLegend(40)
                    Microsoft.Maps.loadModule("Microsoft.Maps.Clustering", function () {
                        _this.mapConfig = {
                            initConfig: {
                                credentials: "Am-PemccWaPtGyGw36n1eGqtKJmjK7MAbuEQhemkTRvDyibkYPzGcw_g2NDl12aQ",
                                center: new Microsoft.Maps.Location(38.68551, -96.503906),
                                zoom: 4,
                                allowHidingLabelsOfRoad: true,
                                disableStreetside: true,
                                disableStreetsideAutoCoverage: true,
                                showMapTypeSelector: false,
                                disableKeyboardInput: true
                            },
                            zoomAreasConfig: [
                                { id: 1, zoomLevelMax: 20, entityType: 'AdminDivision1' }
                            ],
                            colorsAreaCount: [
                                { color: "#72def1" },
                                { color: "#73CFF2" },
                                { color: "#007EA7" },
                                { color: "#003459" },
                                { color: "#2E4147" }
                            ],
                            colorsAreaActivationPercentage: [
                                { color: "#ff0000" },
                                { color: "#ff8000" },
                                { color: "#ffff00" },
                                { color: "#bfff00" },
                                { color: "#80ff00" }
                            ],
                            dateOptions: {
                                formatYear: 'yy',
                                maxDate: new Date(2030, 12, 31),
                                minDate: new Date(2000, 1, 1),
                                startingDay: 1
                            },
                            deviceCountPinsColor: 'rgba(20, 180, 20, 0.5)'
                        };
                        _this.map = new Microsoft.Maps.Map("#myMap", _this.mapConfig.initConfig);
                        //Init infobox
                        var infoboxArea = new Microsoft.Maps.Infobox(_this.map.getCenter(), {
                            title: 'Title',
                            description: 'Description',
                            visible: false
                        });
                        var infoboxPushpin = new Microsoft.Maps.Infobox(_this.map.getCenter(), {
                            title: 'Title',
                            description: 'Description',
                            visible: false
                        });
                        //Init state
                        var retailerAll = { name: "", displayName: "All Retailers", count: 0 };
                        var productFamilyAll = { name: "", displayName: "All Product Families", count: 0 };
                        var connectionStateAll = { name: "", displayName: "All devices", count: 0 };
                        _this.mapState = {
                            initialized: false,
                            currentZoom: _this.mapConfig.initConfig.zoom,
                            currentBoundaries: _this.map.getBounds().bounds,
                            eventViewChanged: Microsoft.Maps.Events.addHandler(_this.map, "viewchangeend", function (args) { _this.eventViewChanged(args, _this); }),
                            area: { listHandlerIdsArea: [], listObjectsArea: [], lastZoomAreaConfigId: -1, infoboxArea: infoboxArea, isLoading: false },
                            pushpins: { listHandlerIdsPushpins: [], lastZoomPushpinsConfigId: -1, infoboxPushpins: infoboxPushpin, isLoading: false, pushpinsLayer: null },
                            pinViewId: "devices",
                            areaViewId: "count",
                            filters: {
                                filterRetailer: retailerAll,
                                filterProductFamily: productFamilyAll,
                                filterConnectionStatus: connectionStateAll,
                                filterActivationDateMin: null,
                                filterActivationDateMax: null,
                                filterShippingDateMin: null,
                                filterShippingDateMax: null,
                                isActivationDateMinVisible: false,
                                isActivationDateMaxVisible: false,
                                isShippingDateMinVisible: false,
                                isShippingDateMaxVisible: false
                            },
                            retailers: [retailerAll],
                            productFamilies: [productFamilyAll],
                            connectionStates: [connectionStateAll],
                            legend: ""
                        };
                        //Set up dropdown
                        _this.mapState.retailers[0].count = response.count;
                        _this.mapState.productFamilies[0].count = response.count;
                        _this.mapState.connectionStates[0].count = response.count;
                        (_a = _this.mapState.retailers).push.apply(_a, response.retailers);
                        (_b = _this.mapState.productFamilies).push.apply(_b, response.productFamilies);
                        (_c = _this.mapState.connectionStates).push.apply(_c, response.connectionStates);
                        //Set up pins arrays
                        _this.mapState.pushpins.pushpinsLayer = new Microsoft.Maps.ClusterLayer([], {
                            clusteredPinCallback: function (args) { _this.eventGenerateCustomPin(args, _this); },
                            gridSize: 90
                        });
                        _this.map.layers.insert(_this.mapState.pushpins.pushpinsLayer);
                        //Loading Pushpins and areas
                        _this.mapState.pushpins.infoboxPushpins.setMap(_this.map);
                        _this.mapState.area.infoboxArea.setMap(_this.map);
                        _this.refreshMap();
                        _this.mapState.initialized = true;
                        var _a, _b, _c;
                    });
                });
            });
        };
        ;
        MapDashboardCtrl.prototype.getMapQueryConfiguration = function (viewId) {
            //generate filters
            var filters = [];
            if (this.mapState.filters.filterRetailer != null && this.mapState.filters.filterRetailer.name != "")
                filters.push({ field: "retailerName", operator: MSIoTDeviceManagementPortal.ComparisonOperators.Equals, value: this.mapState.filters.filterRetailer.name });
            if (this.mapState.filters.filterProductFamily != null && this.mapState.filters.filterProductFamily.name != "")
                filters.push({ field: "productFamily", operator: MSIoTDeviceManagementPortal.ComparisonOperators.Equals, value: this.mapState.filters.filterProductFamily.name });
            if (this.mapState.filters.filterConnectionStatus != null && this.mapState.filters.filterConnectionStatus.name != "")
                filters.push({ field: "status", operator: MSIoTDeviceManagementPortal.ComparisonOperators.Equals, value: this.mapState.filters.filterConnectionStatus.name });
            if (this.mapState.filters.filterActivationDateMin)
                filters.push({ field: "activationDate", operator: MSIoTDeviceManagementPortal.ComparisonOperators.GreaterOrEqual, value: String(this.mapState.filters.filterActivationDateMin.valueOf()) });
            if (this.mapState.filters.filterActivationDateMax != null)
                filters.push({ field: "activationDate", operator: MSIoTDeviceManagementPortal.ComparisonOperators.LesserOrEqual, value: String(this.mapState.filters.filterActivationDateMax.valueOf()) });
            if (this.mapState.filters.filterShippingDateMin)
                filters.push({ field: "shippedDate", operator: MSIoTDeviceManagementPortal.ComparisonOperators.GreaterOrEqual, value: String(this.mapState.filters.filterShippingDateMin.valueOf()) });
            if (this.mapState.filters.filterShippingDateMax != null)
                filters.push({ field: "shippedDate", operator: MSIoTDeviceManagementPortal.ComparisonOperators.LesserOrEqual, value: String(this.mapState.filters.filterShippingDateMax.valueOf()) });
            var query = {
                viewId: viewId,
                filters: filters
            };
            return query;
        };
        MapDashboardCtrl.prototype.refreshMap = function () {
            this.refreshAreas();
            this.refreshPushpins();
        };
        MapDashboardCtrl.prototype.refreshAreas = function () {
            this.refreshAreasInternal(this.getMapQueryConfiguration(this.mapState.areaViewId));
        };
        MapDashboardCtrl.prototype.refreshPushpins = function () {
            this.refreshPushpinsInternal(this.getMapQueryConfiguration(this.mapState.pinViewId));
        };
        MapDashboardCtrl.prototype.refreshAreasInternal = function (query) {
            var _this = this;
            if (this.mapState.area.isLoading)
                return;
            //Get Area config
            var requestOptions = this.getCurrentAreaConfig();
            this.mapState.area.isLoading = true;
            //request
            var stateRequestOptions = {
                entityType: requestOptions.entityType,
                getAllPolygons: false,
            };
            //Remove all previous handlers and areas
            if (this.mapState.area.listHandlerIdsArea != null && this.mapState.area.listHandlerIdsArea.length > 0) {
                this.mapState.area.listHandlerIdsArea.forEach(function (handlerID) {
                    Microsoft.Maps.Events.removeHandler(handlerID);
                });
                this.mapState.area.listHandlerIdsArea.splice(0, this.mapState.area.listHandlerIdsArea.length);
            }
            if (this.mapState.area.listObjectsArea != null && this.mapState.area.listObjectsArea.length > 0) {
                this.mapState.area.listObjectsArea.forEach(function (entity) {
                    _this.map.entities.remove(entity);
                });
                this.mapState.area.listObjectsArea.splice(0, this.mapState.area.listObjectsArea.length);
            }
            this.deviceDBService.getDevicesTwinMapArea(query).then(function (response) {
                var areaItems = response.areaItems;
                var areaGroups = [];
                var totalDevicesShipped = 0;
                var minValue = -1;
                var maxValue = -1;
                var colorSystem = null;
                if (_this.mapState.areaViewId == "count")
                    colorSystem = _this.mapConfig.colorsAreaCount;
                else if (_this.mapState.areaViewId == "activated" || _this.mapState.areaViewId == "retailerName" || _this.mapState.areaViewId == "productFamily")
                    colorSystem = _this.mapConfig.colorsAreaActivationPercentage;
                for (var key in areaItems) {
                    if (minValue < 0 || minValue > areaItems[key])
                        minValue = areaItems[key];
                    if (maxValue < 0 || maxValue < areaItems[key])
                        maxValue = areaItems[key];
                    totalDevicesShipped += areaItems[key];
                    areaGroups.push(key);
                }
                //Percentage boundaries > The legend will always be between 0% to 100%
                if (_this.mapState.areaViewId == "activated" || _this.mapState.areaViewId == "retailerName" || _this.mapState.areaViewId == "productFamily") {
                    minValue = 0;
                    maxValue = 100;
                }
                //Legend
                var initLegend = [];
                var currentValue = minValue;
                var divisionNbrs = colorSystem.length;
                if (maxValue - minValue < divisionNbrs)
                    divisionNbrs = (maxValue - minValue);
                if (divisionNbrs == 0)
                    divisionNbrs++;
                var divisionsValues = _this.getLegendMatrix(minValue, maxValue, divisionNbrs);
                for (var i = 0; i < divisionNbrs; i++) {
                    initLegend.push('<svg width="12" height="12"><rect width="12" height="12" style="fill:' + colorSystem[i].color + '"></rect></svg> ');
                    if (_this.mapState.areaViewId == "activated" || _this.mapState.areaViewId == "retailerName" || _this.mapState.areaViewId == "productFamily")
                        initLegend.push(Math.round(divisionsValues[i]) + '%-' + Math.round(divisionsValues[i + 1]), '%<br/>');
                    else
                        initLegend.push(Math.round(divisionsValues[i]) + '-' + Math.round(divisionsValues[i + 1]), '<br/>');
                }
                _this.mapState.legend = _this.$sce.trustAsHtml(initLegend.join(""));
                //Use the GeoData API manager to get the State. 
                Microsoft.Maps.SpatialDataService.GeoDataAPIManager.getBoundary(areaGroups, requestOptions, _this.map, function (data) {
                    if (data.results && data.results.length > 0) {
                        var count = areaItems[data.location];
                        for (var i = 0; i < divisionNbrs; i++) {
                            if ((divisionsValues[i] <= count && count < divisionsValues[i + 1]) || (count == maxValue && count <= divisionsValues[i + 1])) {
                                data.results[0].Polygons[0].setOptions({
                                    fillColor: colorSystem[i].color
                                });
                                break;
                            }
                        }
                        data.results[0].Polygons[0].metadata = {
                            title: data.location,
                            description: _this.mapState.areaViewId == "count" ? "Devices count: " + count : _this.mapState.areaViewId == "activated" ? "% Activated: " + count : _this.mapState.areaViewId == "retailerName" ? "% belonging to " + _this.mapState.filters.filterRetailer.displayName + ": " + count : ""
                        };
                        _this.mapState.area.listObjectsArea.push(data.results[0].Polygons[0]);
                        _this.map.entities.push(data.results[0].Polygons[0]);
                        _this.mapState.area.listHandlerIdsArea.push(Microsoft.Maps.Events.addHandler(data.results[0].Polygons[0], 'click', function (args) { _this.eventSetInfoboxArea(args, _this); }));
                        _this.mapState.area.lastZoomAreaConfigId = requestOptions.id;
                    }
                });
                _this.mapState.area.isLoading = false;
            });
        };
        MapDashboardCtrl.prototype.getLegendMatrix = function (min, max, divisions) {
            var numberOfZeroes = String((max - min)).length - 2;
            var multiplier = Math.pow(10, numberOfZeroes);
            var difference = 0;
            var interval = 0;
            var division = [];
            min = Math.floor(min / multiplier) * multiplier;
            max = Math.ceil(max / multiplier) * multiplier;
            difference = max - min;
            interval = difference / divisions;
            for (var i = 0; i < divisions; i++) {
                division.push(Math.round((interval * i + min) / multiplier) * multiplier);
            }
            division.push(max);
            return division;
        };
        MapDashboardCtrl.prototype.refreshPushpinsInternal = function (query) {
            var _this = this;
            if (this.mapState.pushpins.isLoading)
                return;
            this.mapState.pushpins.isLoading = true;
            //Remove all previous handlers and areas
            if (this.mapState.pushpins.listHandlerIdsPushpins != null && this.mapState.pushpins.listHandlerIdsPushpins.length > 0) {
                this.mapState.pushpins.listHandlerIdsPushpins.forEach(function (handlerID) {
                    Microsoft.Maps.Events.removeHandler(handlerID);
                });
                this.mapState.pushpins.listHandlerIdsPushpins.splice(0, this.mapState.pushpins.listHandlerIdsPushpins.length);
            }
            /*if (this.mapState.pushpins.listObjectsPushpins != null && this.mapState.pushpins.listObjectsPushpins.length > 0) {
                this.mapState.pushpins.listObjectsPushpins.forEach((entity: Microsoft.Maps.IPrimitive) => {
                    this.map.entities.remove(entity);
                });
                this.mapState.pushpins.listObjectsPushpins.splice(0, this.mapState.area.listObjectsArea.length);
            }*/
            this.mapState.pushpins.pushpinsLayer.clear();
            this.deviceDBService.getDevicesTwinMap(query).then(function (response) {
                var pushpinsItems = response.pushpins;
                var totalItems = pushpinsItems.length;
                var pushpins = [];
                if (pushpinsItems != null) {
                    for (var i = 0; i < pushpinsItems.length; i++) {
                        if (pushpinsItems[i].lat !== null) {
                            var latLon = new Microsoft.Maps.Location(pushpinsItems[i].lat, pushpinsItems[i].lon);
                            var iconPins = "/Content/images/icon-pin-count.svg";
                            if (_this.mapState.pinViewId == "alerts")
                                iconPins = "/Content/images/icon-alert.svg";
                            var pushpin = new Microsoft.Maps.Pushpin(latLon, { color: _this.mapConfig.deviceCountPinsColor, icon: iconPins }); //, text: '', title: pushpinsItems[i].name, subTitle: '' });
                            //pushpin.metadata = {
                            //    title: pushpinsItems[i].name,
                            //    description: "Devices count= " + pushpinsItems[i].count
                            //};
                            //Add an click event handler to the pushpin.
                            //this.mapState.area.listHandlerIdsArea.push(Microsoft.Maps.Events.addHandler(pushpin, 'mouseover', (args: any) => { this.eventSetInfoboxPushpins(args, this); }));
                            pushpins.push(pushpin);
                        }
                    }
                    _this.mapState.pushpins.pushpinsLayer.setPushpins(pushpins);
                }
                _this.mapState.pushpins.isLoading = false;
            });
        };
        MapDashboardCtrl.prototype.getCurrentAreaConfig = function () {
            for (var i = 0; i < this.mapConfig.zoomAreasConfig.length; i++) {
                if (this.mapState.currentZoom <= this.mapConfig.zoomAreasConfig[i].zoomLevelMax)
                    return this.mapConfig.zoomAreasConfig[i];
            }
            return null;
        };
        MapDashboardCtrl.prototype.eventGenerateCustomPin = function (cluster, c) {
            //Define variables for minimum cluster radius, and how wide the outline area of the circle should be.
            var minRadius = 12;
            var outlineWidth = 7;
            //Get the number of pushpins in the cluster
            var clusterSize = cluster.containedPushpins.length;
            //Calculate the radius of the cluster based on the number of pushpins in the cluster, using a logarithmic scale.
            var radius = Math.log(clusterSize) / Math.log(10) * 5 + minRadius;
            //Default cluster color is green.
            var fillColor = c.mapConfig.deviceCountPinsColor;
            //Create an SVG string of two circles, one on top of the other, with the specified radius and color.
            var svg = [];
            if (this.mapState.pinViewId == "devices") {
                svg = ['<svg xmlns="http://www.w3.org/2000/svg" width="', (radius * 2), '" height="', (radius * 2), '">',
                    '<circle cx="', radius, '" cy="', radius, '" r="', radius, '" fill="', fillColor, '"/>',
                    '<circle cx="', radius, '" cy="', radius, '" r="', radius - outlineWidth, '" fill="', fillColor, '"/>',
                    '</svg>'];
            }
            else if (this.mapState.pinViewId == "alerts") {
                svg = ['<svg xmlns= "http://www.w3.org/2000/svg" width="', (radius * 2), '" height="', (radius * 2), '" viewBox= "0 0 18 17">',
                    '<g id="Welcome" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd"><g id="001-device-dashboard-all-UI" transform="translate(-1186.000000, -741.000000)"><g id="Group-11" transform="translate(135.000000, 368.000000)">',
                    '<g id="Group-4"><g id="table" transform="translate(0.000000, 42.000000)"><g id="Alerts" transform="translate(1051.000000, 131.000000)"><g id="warning" transform="translate(0.000000, 200.000000)"><g id="Group">',
                    '<path d="M17.3406122,12.3061224 L17.3406122,12.3061224 L11.0957143,1.52081633 C10.5189796,0.538163265 9.74755102,0 8.91,0 C8.07244898,0 7.30102041,0.538163265 6.73346939,1.51530612 L0.479387755,12.2877551 C-0.0955102041,13.2869388 -0.156122449,14.3265306 0.312244898,15.1346939 C0.756734694,15.9061224 1.62918367,16.3469388 2.70734694,16.3469388 L15.1144898,16.3469388 C16.1926531,16.3469388 17.0669388,15.902449 17.5022449,15.1328571 C17.9761224,14.3265306 17.9136735,13.2997959 17.3406122,12.3061224 Z" id="Path-Copy" fill="#FFCC00"></path>',
                    '<path d="M8.91,10.2361224 C9.06216008,10.2361224 9.1855102,10.1127723 9.1855102,9.96061224 L9.1855102,4.86367347 C9.18551021,4.76524315 9.13299826,4.67428978 9.04775511,4.62507462 C8.96251195,4.57585946 8.85748805,4.57585946 8.77224489,4.62507462 C8.68700174,4.67428978 8.63448979,4.76524315 8.6344898,4.86367347 L8.6344898,9.96061224 C8.6344898,10.0336821 8.66351668,10.1037592 8.71518487,10.1554274 C8.76685305,10.2070956 8.83693015,10.2361224 8.91,10.2361224 L8.91,10.2361224 Z" id="Shape" fill="#000000" fill-rule="nonzero"></path>',
                    '<circle id="Oval" fill="#000000" fill-rule="nonzero" cx="8.87693878" cy="12.5081633" r="1"></circle>',
                    '</g></g></g></g></g></g></g></g></svg>'];
            }
            //Customize the clustered pushpin using the generated SVG and anchor on its center.
            var config = {
                icon: svg.join(''),
                anchor: new Microsoft.Maps.Point(radius, radius),
                textOffset: new Microsoft.Maps.Point(0, radius - 8) //Subtract 8 to compensate for height of text.
            };
            if (this.mapState.pinViewId == "alerts")
                config.text = "";
            cluster.setOptions(config);
        };
        MapDashboardCtrl.prototype.eventViewChanged = function (e, c) {
            if (c.mapState == null || !c.mapState.initialized)
                return;
            c.mapState.currentZoom = e.target.getZoom();
            c.mapState.currentBoundaries = e.target.getBounds().bounds;
            //refresh
            //c.refreshAreas();
            //c.refreshPushpins();
        };
        MapDashboardCtrl.prototype.eventSetInfoboxArea = function (args, c) {
            this.mapState.area.infoboxArea.setOptions({
                location: args.location,
                title: args.target.metadata.title,
                description: args.target.metadata.description,
                visible: true
            });
        };
        MapDashboardCtrl.prototype.eventSetInfoboxPushpins = function (args, c) {
            this.mapState.pushpins.infoboxPushpins.setOptions({
                location: args.target.getLocation(),
                title: args.target.metadata.title,
                description: args.target.metadata.description,
                visible: true
            });
        };
        MapDashboardCtrl.prototype.getConnectionStatusString = function (e) {
            switch (e) {
                case "0":
                    return "Connected";
                case "1":
                    return "Disconnected";
                case "2":
                    return "Not Activated";
            }
        };
        MapDashboardCtrl.$inject = ['currentUser', 'DeviceDBService', '$timeout', '$sce'];
        return MapDashboardCtrl;
    }());
    app.controller('MapDashboardCtrl', MapDashboardCtrl);
})(MSIoTDeviceManagementPortal || (MSIoTDeviceManagementPortal = {}));
//# sourceMappingURL=mapDashboardCtrl.js.map