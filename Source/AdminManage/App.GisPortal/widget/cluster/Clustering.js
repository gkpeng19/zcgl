/**
* @author wangyafei
* 多点聚合效果
*/
define([
  "dojo/_base/declare",
  "dojo/_base/array",
  "esri/Color",
  "dojo/_base/connect",
  "esri/SpatialReference",
  "esri/geometry/Point",
  "esri/graphic",
  "esri/symbols/SimpleMarkerSymbol",
  "esri/symbols/TextSymbol",
  "esri/dijit/PopupTemplate",
  "esri/layers/GraphicsLayer"
], function (
  declare, arrayUtils, Color, connect,
  SpatialReference, Point, Graphic, SimpleMarkerSymbol, TextSymbol,
  PopupTemplate, GraphicsLayer
) {
    return declare([GraphicsLayer], {
        constructor: function (options) {

            this._clusterTolerance = options.distance || 50;
            this._clusterData = options.data || [];
            this._clusters = [];
            this._clusterLabelColor = options.labelColor || "#000";
            this._clusterLabelOffset = (options.hasOwnProperty("labelOffset")) ? options.labelOffset : -5;
            this._singles = []; 
            this._showSingles = options.hasOwnProperty("showSingles") ? options.showSingles : true;
            var SMS = SimpleMarkerSymbol;
            this._singleSym = options.singleSymbol || new SMS("circle", 6, null, new Color("#888"));
            this._singleTemplate = options.singleTemplate || new PopupTemplate({ "title": "", "description": "{*}" });
            this._maxSingles = options.maxSingles || 1000;

            this._webmap = options.hasOwnProperty("webmap") ? options.webmap : false;

            this._sr = options.spatialReference || new SpatialReference({ "wkid": 102100 });

            this._zoomEnd = null;
        },

        _setMap: function (map, surface) {
            this._clusterResolution = map.extent.getWidth() / map.width; 
            this._clusterGraphics();

            this._zoomEnd = connect.connect(map, "onZoomEnd", this, function () {
                this._clusterResolution = this._map.extent.getWidth() / this._map.width;
                this.clear();
                this._clusterGraphics();
            });

            var div = this.inherited(arguments);
            return div;
        },

        _unsetMap: function () {
            this.inherited(arguments);
            connect.disconnect(this._zoomEnd);
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
            this.clearSingles(this._singles);

            var singles = [];
            for (var i = 0, il = this._clusterData.length; i < il; i++) {
                if (e.graphic.attributes.clusterId == this._clusterData[i].attributes.clusterId) {
                    singles.push(this._clusterData[i]);
                }
            }
            if (singles.length > this._maxSingles) {
                return;
            } else {
                e.stopPropagation();
                this._map.infoWindow.show(e.graphic.geometry);
                this._addSingles(singles);
            }
        },

        _clusterGraphics: function () {
            for (var j = 0, jl = this._clusterData.length; j < jl; j++) {
                var point = this._clusterData[j];
                var clustered = false;
                var numClusters = this._clusters.length;
                for (var i = 0; i < this._clusters.length; i++) {
                    var c = this._clusters[i];
                    if (this._clusterTest(point, c)) {
                        this._clusterAddPoint(point, c);
                        clustered = true;
                        break;
                    }
                }

                if (!clustered) {
                    this._clusterCreate(point);
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

        _clusterAddPoint: function (p, cluster) {
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
            if (!p.hasOwnProperty("attributes")) {
                p.attributes = {};
            }
            p.attributes.clusterId = cluster.attributes.clusterId;
        },

        _clusterCreate: function (p) {
            var clusterId = this._clusters.length + 1;
            if (!p.attributes) {
                p.attributes = {};
            }
            p.attributes.clusterId = clusterId;
            var cluster = {
                "x": p.x,
                "y": p.y,
                "attributes": {
                    "clusterCount": 1,
                    "clusterId": clusterId,
                    "extent": [p.x, p.y, p.x, p.y]
                }
            };
            this._clusters.push(cluster);
        },

        _showAllClusters: function () {
            for (var i = 0, il = this._clusters.length; i < il; i++) {
                var c = this._clusters[i];
                this._showCluster(c);
            }
        },

        _showCluster: function (c) {
            var point = new Point(c.x, c.y, this._sr);
            this.add(
              new Graphic(
                point,
                null,
                c.attributes
              )
            );
            if (c.attributes.clusterCount == 1) {
                return;
            }

            var label = new TextSymbol(c.attributes.clusterCount)
              .setColor(new Color(this._clusterLabelColor))
              .setOffset(0, this._clusterLabelOffset);
            this.add(
              new Graphic(
                point,
                label,
                c.attributes
              )
            );
        },

        _addSingles: function (singles) {
            arrayUtils.forEach(singles, function (p) {
                var g = new Graphic(
                  new Point(p.x, p.y, this._sr),
                  this._singleSym,
                  p.attributes,
                  this._singleTemplate
                );
                this._singles.push(g);
                if (this._showSingles) {
                    this.add(g);
                }
            }, this);
            this._map.infoWindow.setFeatures(this._singles);
        },

        _updateClusterGeometry: function (c) {
            var cg = arrayUtils.filter(this.graphics, function (g) {
                return !g.symbol &&
                       g.attributes.clusterId == c.attributes.clusterId;
            });
            if (cg.length == 1) {
                cg[0].geometry.update(c.x, c.y);
            } else {
                console.log("didn't find exactly one cluster geometry to update: ", cg);
            }
        },

        _updateLabel: function (c) {
            var label = arrayUtils.filter(this.graphics, function (g) {
                return g.symbol &&
                       g.symbol.declaredClass == "esri.symbol.TextSymbol" &&
                       g.attributes.clusterId == c.attributes.clusterId;
            });
            if (label.length == 1) {
                this.remove(label[0]);
                var newLabel = new TextSymbol(c.attributes.clusterCount)
                  .setColor(new Color(this._clusterLabelColor))
                  .setOffset(0, this._clusterLabelOffset);
                this.add(
                  new Graphic(
                    new Point(c.x, c.y, this._sr),
                    newLabel,
                    c.attributes
                  )
                );
            } else {
                console.log("didn't find exactly one label: ", label);
            }
        },

        _clusterMeta: function () {
            console.log("Total:  ", this._clusterData.length);

            var count = 0;
            arrayUtils.forEach(this._clusters, function (c) {
                count += c.attributes.clusterCount;
            });
            console.log("In clusters:  ", count);
        }

    });
});

