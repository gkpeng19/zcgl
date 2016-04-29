define([
    'dojo/_base/declare',
    'dojo/_base/array',
    'dojo/_base/lang',
    'dojo/_base/Color',
    'dojo/_base/connect',
    'dojo/on',
    'dojo/promise/all',
    'esri/SpatialReference',
    'esri/geometry/Point',
    'esri/geometry/Multipoint',
    'esri/geometry/Extent',
    'esri/graphic',
    "esri/config",
    "esri/geometry/normalizeUtils",
    'esri/symbols/SimpleMarkerSymbol',
    'esri/symbols/SimpleLineSymbol',
    'esri/symbols/SimpleFillSymbol',
    'esri/symbols/TextSymbol',
    'esri/symbols/Font',
    'esri/renderers/ClassBreaksRenderer',
    'esri/request',
    'esri/symbols/jsonUtils',
    'esri/renderers/jsonUtils',
    'esri/dijit/PopupTemplate',
    'esri/layers/GraphicsLayer',
    'esri/tasks/query',
    'esri/tasks/QueryTask'
], function (
    declare, arrayUtils, lang, Color, connect, on, all,
    SpatialReference, Point, Multipoint, Extent, Graphic,
    config, normalizeUtils,
    SimpleMarkerSymbol, SimpleLineSymbol, SimpleFillSymbol, TextSymbol, Font,
    ClassBreaksRenderer,
    esriRequest, symbolJsonUtils, rendererJsonUtil,
    PopupTemplate, GraphicsLayer, Query, QueryTask
) {

    function concat(a1, a2) {
        return a1.concat(a2);
    }

    function merge(arrs) {
        var len = arrs.length, target = [];
        while (len--) {
            var o = arrs[len];
            if (o.constructor === Array) {
                target = concat(target, o);
            } else {
                target.push(o);
            }
        }
        return target;
    }

    function difference(arr1, cacheCount, hash) {

        var len = arr1.length, diff = [];
        if (!cacheCount) {
            diff = arr1;
            while (len--) {
                var value = arr1[len];
                if (!hash[value]) {
                    hash[value] = value;
                }
            }
            return diff;
        }
        while (len--) {
            var val = arr1[len];
            if (!hash[val]) {
                hash[val] = val;
                diff.push(val);
            }
        }
        return diff;
    }

    function toPoints(features) {
        var len = features.length;
        var points = [];
        while (len--) {
            var g = features[len];
            points.push(
                new Graphic(
                    g.geometry.getCentroid(),
                    g.symbol, g.attributes,
                    g.infoTemplate
            ));
        }
        return points;
    }

    (function () {
        if (!window.console) {
            window.console = {};
        }
        var m = [
          "log", "info", "warn", "error", "debug", "trace", "dir", "group",
          "groupCollapsed", "groupEnd", "time", "timeEnd", "profile", "profileEnd",
          "dirxml", "assert", "count", "markTimeline", "timeStamp", "clear"
        ];
        for (var i = 0; i < m.length; i++) {
            if (!window.console[m[i]]) {
                window.console[m[i]] = function () { };
            }
        }
    })();

    return declare([GraphicsLayer], {
        constructor: function (options) {

            this._clusterTolerance = options.distance || 50;
            this._clusterData = [];
            this._clusters = [];
            this._clusterLabelColor = options.labelColor || '#000';
            this._clusterLabelOffset = (options.hasOwnProperty('labelOffset')) ? options.labelOffset : -5;
            this._singles = [];
            this._showSingles = options.hasOwnProperty('showSingles') ? options.showSingles : true;
            this._zoomOnClick = options.hasOwnProperty('zoomOnClick') ? options.zoomOnClick : true;
            this._singleSym = options.singleSymbol || new SimpleMarkerSymbol("circle", 16,
                                    new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([85, 125, 140, 1]), 3),
                                    new Color([255, 255, 255, 1]));
            this._singleTemplate = options.singleTemplate || new PopupTemplate({ 'title': '', 'description': '{*}' });
            this._maxSingles = options.maxSingles || 10000;
            this._font = options.font || new Font('10pt').setFamily('Arial');
            this._sr = options.spatialReference || new SpatialReference({ 'wkid': 102100 });
            this._zoomEnd = null;
            this.url = options.url || null;
            this._outFields = options.outFields || ['*'];
            this.queryTask = new QueryTask(this.url);
            this._where = options.where || null;
            this._useDefaultSymbol = options.hasOwnProperty('useDefaultSymbol') ? options.useDefaultSymbol : false;
            this._returnLimit = options.returnLimit || 1000;
            this._singleRenderer = options.singleRenderer;
            this._objectIdField = options.objectIdField || 'OBJECTID';
            if (!this.url) {
                throw new Error('请传递FeatureLayer的URL路径！');
            }
            this._clusterCache = {};
            this._objectIdCache = [];
            this._objectIdHash = {};
            this._currentClusterGraphic = null;
            this._currentClusterLabel = null;
            this._visitedExtent = null;
            this.detailsLoaded = false;
            this._query = new Query();
            this.MODE_SNAPSHOT = options.hasOwnProperty('MODE_SNAPSHOT') ? options.MODE_SNAPSHOT : true;
            this._getServiceDetails();

        },

        _getServiceDetails: function () {
            esriRequest({
                url: this.url,
                content: {
                    f: 'json'
                },
                handleAs: 'json'
            }).then(lang.hitch(this, function (response) {
                this._defaultRenderer = this._singleRenderer ||
                    rendererJsonUtil.fromJson(response.drawingInfo.renderer);
                this.native_geometryType = response.geometryType;
                if (response.geometryType === 'esriGeometryPolygon') {
                    this._useDefaultSymbol = false;
                    console.info('polygon geometry will be converted to points');
                }
                this.emit('details-loaded', response);
            }));
        },

        _getDefaultSymbol: function (g) {
            var rend = this._defaultRenderer;
            if (!this._useDefaultSymbol || !rend) {
                return this._singleSym;
            } else {
                return rend.getSymbol(g);
            }
        },

        _getRenderedSymbol: function (feature) {
            var attr = feature.attributes;
            if (attr.clusterCount === 1) {
                if (!this._useDefaultSymbol) {
                    return this._singleSym;
                }
                var rend = this._defaultRenderer;
                if (!rend) {
                    return null;
                } else {
                    return rend.getSymbol(feature);
                }
            } else {
                return null;
            }
        },

        _reCluster: function () {
            this._clusterResolution = this._map.extent.getWidth() / this._map.width;
            if (!this._visitedExtent) {
                this._getObjectIds(this._map.extent);
            } else if (!this._visitedExtent.contains(this._map.extent)) {
                this._getObjectIds(this._map.extent);
            } else {
                this._clusterGraphics();
            }
            this._visitedExtent = this._visitedExtent ? this._visitedExtent.union(this._map.extent) : this._map.extent;
        },

        _setClickedClusterGraphics: function (g) {
            if (g === null) {
                this._currentClusterGraphic = null;
                this._currentClusterLabel = null;
                return;
            }
            if (g.symbol === null) {
                this._currentClusterLabel = this._getCurrentLabelGraphic(g);
                this._currentClusterGraphic = g;
            } else if (g.symbol.declaredClass === 'esri.symbol.TextSymbol') {
                this._currentClusterLabel = g;
                this._currentClusterGraphic = this._getCurrentClusterGraphic(g);
            }
        },

        _getCurrentClusterGraphic: function (c) {
            var gArray = arrayUtils.filter(this.graphics, function (g) {
                return (g.attributes.clusterId === c.attributes.clusterId);
            });
            return gArray[0];
        },
        _getCurrentLabelGraphic: function (c) {
            var gArray = arrayUtils.filter(this.graphics, function (g) {
                return (g.symbol &&
                    g.symbol.declaredClass === 'esri.symbol.TextSymbol' &&
                    g.attributes.clusterId === c.attributes.clusterId);
            });
            return gArray[0];
        },
        _popupVisibilityChange: function (e) {
            if (this && this._map) {
                var show = this._map.infoWindow.isShowing;
                this._showClickedCluster(!show);
                if (!show) {
                    this.clearSingles();
                }
            }
        },
        _showClickedCluster: function (show) {
            if (this._currentClusterGraphic && this._currentClusterLabel) {
                if (show) {
                    this._currentClusterGraphic.show();
                    this._currentClusterLabel.show();
                } else {
                    this._currentClusterGraphic.hide();
                    this._currentClusterLabel.hide();
                }
            }
        },
        _setMap: function (map, surface) {
            this._query.outSpatialReference = map.spatialReference;
            this._query.returnGeometry = true;
            this._query.outFields = this._outFields;
            this._extentChange = on(map, 'extent-change', lang.hitch(this, '_reCluster'));

            map.infoWindow.on('hide', lang.hitch(this, '_popupVisibilityChange'));
            map.infoWindow.on('show', lang.hitch(this, '_popupVisibilityChange'));

            var layerAdded = on(map, 'layer-add', lang.hitch(this, function (e) {
                if (e.layer === this) {
                    layerAdded.remove();
                    if (!this.detailsLoaded) {
                        on.once(this, 'details-loaded', lang.hitch(this, function () {
                            if (!this.renderer) {

                                this._singleSym = this._singleSym || new SimpleMarkerSymbol("circle", 16,
                                    new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([85, 125, 140, 1]), 3),
                                    new Color([255, 255, 255, .5]));

                                var renderer = new ClassBreaksRenderer(this._singleSym, 'clusterCount');

                                small = new SimpleMarkerSymbol("circle", 25,
                                            new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([140, 177, 210, 0.35]), 15),
                                            new Color([140, 177, 210, 0.75]));
                                medium = new SimpleMarkerSymbol("circle", 50,
                                            new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([97, 147, 179, 0.35]), 15),
                                            new Color([97, 147, 179, 0.75]));
                                large = new SimpleMarkerSymbol("circle", 80,
                                            new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([59, 110, 128, 0.35]), 15),
                                            new Color([59, 110, 128, 0.75]));
                                xlarge = new SimpleMarkerSymbol("circle", 110,
                                            new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([20, 72, 77, 0.35]), 15),
                                            new Color([20, 72, 77, 0.75]));

                                renderer.addBreak(2, 10, small);
                                renderer.addBreak(10, 25, medium);
                                renderer.addBreak(25, 100, large);
                                renderer.addBreak(100, Infinity, xlarge);
                                this.setRenderer(renderer);
                            }
                            this._reCluster();
                        }));
                    }
                }
            }));

            var div = this.inherited(arguments);
            return div;
        },

        _unsetMap: function () {
            this.inherited(arguments);
            this._extentChange.remove();
        },

        _onClusterClick: function (e) {
            var attr = e.graphic.attributes;
            if (attr && attr.clusterCount) {
                var source = arrayUtils.filter(this._clusterData, function (g) {
                    return attr.clusterId === g.attributes.clusterId;
                }, this);
                this.emit('cluster-click', source);
            }
        },

        _getObjectIds: function (extent) {
            if (this.url) {
                var ext = extent || this._map.extent;
                this._query.objectIds = null;
                if (this._where) {
                    this._query.where = this._where;
                }
                if (!this.MODE_SNAPSHOT) {
                    this._query.geometry = ext;
                }
                if (!this._query.geometry && !this._query.where) {
                    this._query.where = '1=1';
                }
                this.queryTask.executeForIds(this._query).then(
                    lang.hitch(this, '_onIdsReturned'), this._onError
                );
            }
        },

        _onError: function (err) {
            //console.warn('ReturnIds Error', err);
        },

        _onIdsReturned: function (results) {
            var uncached = difference(results, this._objectIdCache.length, this._objectIdHash);
            this._objectIdCache = concat(this._objectIdCache, uncached);
            if (uncached && uncached.length) {
                this._query.where = null;
                this._query.geometry = null;
                var queries = [];
                if (uncached.length > this._returnLimit) {
                    while (uncached.length) {
                        this._query.objectIds = uncached.splice(0, this._returnLimit - 1);
                        queries.push(this.queryTask.execute(this._query));
                    }
                    all(queries).then(lang.hitch(this, function (res) {
                        var features = arrayUtils.map(res, function (r) {
                            return r.features;
                        });
                        this._onFeaturesReturned({
                            features: merge(features)
                        });
                    }));
                } else {
                    this._query.objectIds = uncached.splice(0, this._returnLimit - 1);
                    this.queryTask.execute(this._query).then(
                        lang.hitch(this, '_onFeaturesReturned'), this._onError
                    );
                }
            } else if (this._objectIdCache.length) {
                this._onFeaturesReturned({
                    features: []
                });
            } else {
                this.clear();
            }
        },
        _inExtent: function () {
            var ext = this._map.extent;
            var len = this._objectIdCache.length;
            var valid = [];
            while (len--) {
                var oid = this._objectIdCache[len];
                var cached = this._clusterCache[oid];
                if (cached && ext.contains(cached.geometry)) {
                    valid.push(cached);
                }
            }
            return valid;
        },

        _onFeaturesReturned: function (results) {
            var inExtent = this._inExtent();
            var features;
            if (this.native_geometryType === 'esriGeometryPolygon') {
                features = toPoints(results.features);
            } else {
                features = results.features;
            }
            var len = features.length;
            if (len) {
                this._clusterData.length = 0;
                this.clear();
                arrayUtils.forEach(features, function (feat) {
                    this._clusterCache[feat.attributes[this._objectIdField]] = feat;
                }, this);
                this._clusterData = concat(features, inExtent);
            }
            this._clusterGraphics();
        },
        updateClusters: function () {
            this.clearCache();
            this._reCluster();
        },

        clearCache: function () {
            arrayUtils.forEach(this._objectIdCache, function (oid) {
                delete this._objectIdCache[oid];
            }, this);
            this._objectIdCache.length = 0;
            this._clusterCache = {};
            this._objectIdHash = {};
        },

        add: function (p) {
            if (p.declaredClass) {
                this.inherited(arguments);
                return;
            }
            this._clusterData.push(p);
            var clustered = false;
            for (var i = 0; i < this._clusters.length; i++) {
                var c = this._clusters[i];
                if (this._clusterTest(p, c)) {
                    this._clusterAddPoint(p, c);
                    this._updateClusterGeometry(c);
                    this._updateLabel(c);
                    clustered = true;
                    break;
                }
            }

            if (!clustered) {
                this._clusterCreate(p);
                p.attributes.clusterCount = 1;
                this._showCluster(p);
            }
        },

        clear: function () {
            this.inherited(arguments);
            this._clusters.length = 0;
        },

        clearSingles: function (singles) {
            var s = singles || this._singles;
            arrayUtils.forEach(s, function (g) {
                this.remove(g);
            }, this);
            this._singles.length = 0;
        },

        onClick: function (e) {
            e.stopPropagation();
            if (e.graphic.attributes.clusterCount === 1) {
                this._showClickedCluster(true);
                this._setClickedClusterGraphics(null);
                this.clearSingles(this._singles);
                var singles = this._getClusterSingles(e.graphic.attributes.clusterId);
                arrayUtils.forEach(singles, function (g) {
                    g.setSymbol(this._getDefaultSymbol(g));
                    g.setInfoTemplate(this._singleTemplate);
                }, this);

                this._addSingleGraphics(singles);
                this._map.infoWindow.setFeatures(singles);
                this._map.infoWindow.show(e.graphic.geometry);
                this._map.infoWindow.show(e.graphic.geometry);
            }
            else if (this._zoomOnClick && e.graphic.attributes.clusterCount > 1 && this._map.getZoom() !== this._map.getMaxZoom()) {
                var extent = this._getClusterExtent(e.graphic);
                if (extent.getWidth()) {
                    this._map.setExtent(extent.expand(1.5), true);
                } else {
                    this._map.centerAndZoom(e.graphic.geometry, this._map.getMaxZoom());
                }
            } else {
                this.clearSingles(this._singles);
                var singles = this._getClusterSingles(e.graphic.attributes.clusterId);
                if (singles.length > this._maxSingles) {
                    //alert("对不起，聚合点个数超过了" + this._maxSingles + "限制，请缩放后查看更多信息！");
                    return;
                } else {
                    this._showClickedCluster(true);
                    this._setClickedClusterGraphics(e.graphic);
                    this._showClickedCluster(false);
                    this._addSingleGraphics(singles);
                    this._map.infoWindow.setFeatures(this._singles);
                    this._map.infoWindow.show(e.graphic.geometry);
                    this._map.infoWindow.show(e.graphic.geometry);
                }
            }
        },

        _clusterGraphics: function () {
            this.clear();
            for (var j = 0, jl = this._clusterData.length; j < jl; j++) {
                var point = this._clusterData[j].geometry || this._clusterData[j];
                if (!this._map.extent.contains(point)) {
                    this._clusterData[j].attributes.clusterId = -1;
                    continue;
                }
                var feature = this._clusterData[j];
                var clustered = false;
                for (var i = 0; i < this._clusters.length; i++) {
                    var c = this._clusters[i];
                    if (this._clusterTest(point, c)) {
                        this._clusterAddPoint(feature, point, c);
                        clustered = true;
                        break;
                    }
                }
                if (!clustered) {
                    this._clusterCreate(feature, point);
                }
            }

            this._showAllClusters();
        },

        _clusterTest: function (p, cluster) {
            var distance = (
                Math.sqrt(
                    Math.pow((cluster.x - p.x), 2) + Math.pow((cluster.y - p.y), 2)
            ) / this._clusterResolution
            );
            return (distance <= this._clusterTolerance);
        },

        _clusterAddPoint: function (feature, p, cluster) {
            var count, x, y;
            count = cluster.attributes.clusterCount;
            x = (p.x + (cluster.x * count)) / (count + 1);
            y = (p.y + (cluster.y * count)) / (count + 1);
            cluster.x = x;
            cluster.y = y;

            if (p.x < cluster.attributes.extent[0]) {
                cluster.attributes.extent[0] = p.x;
            } else if (p.x > cluster.attributes.extent[2]) {
                cluster.attributes.extent[2] = p.x;
            }
            if (p.y < cluster.attributes.extent[1]) {
                cluster.attributes.extent[1] = p.y;
            } else if (p.y > cluster.attributes.extent[3]) {
                cluster.attributes.extent[3] = p.y;
            }

            cluster.attributes.clusterCount++;
            if (!p.hasOwnProperty('attributes')) {
                p.attributes = {};
            }
            feature.attributes.clusterId = p.attributes.clusterId = cluster.attributes.clusterId;
        },

        _clusterCreate: function (feature, p) {
            var clusterId = this._clusters.length + 1;
            if (!p.attributes) {
                p.attributes = {};
            }
            feature.attributes.clusterId = p.attributes.clusterId = clusterId;
            var cluster = {
                'x': p.x,
                'y': p.y,
                'attributes': {
                    'clusterCount': 1,
                    'clusterId': clusterId,
                    'extent': [p.x, p.y, p.x, p.y]
                }
            };
            this._clusters.push(cluster);
        },

        _showAllClusters: function () {
            var len = this._clusters.length;

            for (var i = 0, il = this._clusters.length; i < il; i++) {
                this._showCluster(this._clusters[i]);
            }
            this.emit('clusters-shown', this._clusters);
        },

        _showCluster: function (c) {
            var point = new Point(c.x, c.y, this._sr);
            var count = c.attributes.clusterCount;

            var g = new Graphic(point, null, c.attributes);
            g.setSymbol(this._getRenderedSymbol(g));
            this.add(g);

            if (c.attributes.clusterCount < 2) {
                return;
            }

            var label = new TextSymbol(c.attributes.clusterCount)
                .setColor(new Color(this._clusterLabelColor))
                .setOffset(0, this._clusterLabelOffset)
                .setFont(this._font);
            this.add(
                new Graphic(
                    point,
                    label,
                    c.attributes
                )
            );
        },
        _findCluster: function (id) {
            var cg = arrayUtils.filter(this.graphics, function (g) {
                return !g.symbol &&
                    g.attributes.clusterId == c.attributes.clusterId;
            });
        },

        _getClusterExtent: function (cluster) {
            var ext;
            ext = cluster.attributes.extent;
            return new Extent(ext[0], ext[1], ext[2], ext[3], this._map.spatialReference);
        },

        _getClusteredExtent: function () {
            var extent, clusteredExtent;
            for (var i = 0; i < this._clusters.length; i++) {
                extent = this._getClusteredExtent(this._clusters[i]);
                if (!clusteredExtent) {
                    clusteredExtent = extent;
                } else {
                    clusteredExtent = clusteredExtent.union(extent);
                }
            }
            return clusteredExtent;
        },

        _getClusterSingles: function (id) {
            var singles = [];
            for (var i = 0, il = this._clusterData.length; i < il; i++) {
                if (id === this._clusterData[i].attributes.clusterId) {
                    singles.push(this._clusterData[i]);
                }
            }
            return singles;
        },

        _addSingleGraphics: function (singles) {
            arrayUtils.forEach(singles, function (g) {
                g.setSymbol(this._getDefaultSymbol(g));
                g.setInfoTemplate(this._singleTemplate);
                this._singles.push(g);
                if (this._showSingles) {
                    this.add(g);
                }
            }, this);
        },

        _updateClusterGeometry: function (c) {
            var cg = arrayUtils.filter(this.graphics, function (g) {
                return !g.symbol &&
                    g.attributes.clusterId == c.attributes.clusterId;
            });
            if (cg.length == 1) {
                cg[0].geometry.update(c.x, c.y);
            } else {
                //console.log('didn not find exactly one cluster geometry to update: ', cg);
            }
        },

        _updateLabel: function (c) {
            var label = arrayUtils.filter(this.graphics, function (g) {
                return g.symbol &&
                    g.symbol.declaredClass == 'esri.symbol.TextSymbol' &&
                    g.attributes.clusterId == c.attributes.clusterId;
            });
            if (label.length == 1) {
                this.remove(label[0]);
                var newLabel = new TextSymbol(c.attributes.clusterCount)
                    .setColor(new Color(this._clusterLabelColor))
                    .setOffset(0, this._clusterLabelOffset)
                    .setFont(this._font);
                this.add(
                    new Graphic(
                        new Point(c.x, c.y, this._sr),
                        newLabel,
                        c.attributes)
                );
            } else {
                //console.log('didn not find exactly one label: ', label);
            }
        },

        _clusterMeta: function () {
            console.log('聚合点的总个数:', this._clusterData.length);
            var count = 0;
            arrayUtils.forEach(this._clusters, function (c) {
                count += c.attributes.clusterCount;
            });
        }
    });
});
