define([
    "dojo/_base/declare",
    "widget/dijit/TabContainerDijit",
    "dojo/domReady!"
], function (declare, MyTabContainer) {
    "use strict";
    var PropertyInfo = declare([], {
        constructor: function (map, graphic, layerIndex) {
            this.map = map;
            this.graphic = graphic;
            this.layerIndex = layerIndex;
        },
        show: function () {
            var centerPoint = null;
            if (this.graphic && this.graphic.geometry) {
                if (this.graphic.geometry.type === "polygon") {
                    centerPoint = this.graphic.geometry.getCentroid();
                } else if (this.graphic.geometry.type === "point") {
                    centerPoint = this.graphic.geometry;
                } else if (this.graphic.geometry.type === "polyline") {
                    centerPoint = this.graphic.geometry.getPoint(0, 0);
                } else if (this.graphic.geometry.type === "multipoint") {
                    centerPoint = this.graphic.geometry.getPoint(0);
                } else if (this.graphic.geometry.type === "extent") {
                    centerPoint = this.graphic.geometry.getCenter();
                }
                if (this.graphic.attributes) {
                    this.map.infoWindow.setTitle("详细属性");
                    this.map.infoWindow.setContent("<div id='propertyWindow'></div>");
                    this.map.infoWindow.show(centerPoint);
                    this.addTabContainer(this.graphic);
                }
                this.map.centerAt(centerPoint);
            }
        },
        addTabContainer: function (gra) {
            var idx = -1;
            var container = new MyTabContainer({
                map: this.map,
                feature: gra,
                index: this.layerIndex
            }, "propertyWindow");
            container.startup();
        }
    });

    return PropertyInfo;
});

