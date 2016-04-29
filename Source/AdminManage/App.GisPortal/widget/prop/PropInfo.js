define([
    'dojo/_base/declare',
    'widget/dijit/TabContainer',
    'dojo/domReady!'
], function (declare, MyTabContainer) {
    "use strict";
    var index = 0;
    var PropertyInfo = declare([], {
        constructor: function (map, graphic, layerID) {
            this.map = map;
            this.graphic = graphic;
            this.layerID = layerID;
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
                    if (this.map.getLevel() < 6) {
                        this.map.setLevel(5);
                    }
                    var name = this.graphic.attributes["OBJNAME"] || this.graphic.attributes["MC"] || this.graphic.attributes["名称"];
                    if (name == " ") {
                        name = "要素信息";
                    }
                    this.map.infoWindow.setTitle(name);
                    this.map.infoWindow.setContent("<div id='propertyWindow_" + index + "'></div>");
                    this.map.infoWindow.show(centerPoint);
                    this.addTabContainer(this.graphic);
                }
                this.map.centerAt(centerPoint);
            }
        },
        addTabContainer: function (gra) {
            var idx = -1;
            var container = new MyTabContainer({
                map: JZ.mc,
                feature: gra,
                layerID: this.layerID,
                type: 0
            }, "propertyWindow_" + index);
            index = index + 1;
            container.startup();
        }
    });

    return PropertyInfo;
});

