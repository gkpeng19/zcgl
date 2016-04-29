define("widget/rcLayer",["dojo/_base/declare", "dojo/io-query", "esri/layers/DynamicMapServiceLayer"],
function (declare, ioQuery, DynamicMapServiceLayer) {
    "use strict";
    return declare([DynamicMapServiceLayer], {
        constructor: function (extent) {
            this.initialExtent = this.fullExtent = extent;
            this.loaded = true;
            this.onLoad(this);
        },
        getImageUrl: function (extent, width, height, callback) {
            var params = {
                request: "GetMap",
                format: "JPEG",
                layers: "41",
                styles: "default",
                bbox: extent.xmin + "," + extent.ymin + "," + extent.xmax + "," + extent.ymax,
                srs: '',
                width: width,
                height: height
            };
            callback("http://172.24.57.47/service/RSImage/wms?username=syllhjxxjx&password=syllhjxxjx123&" + ioQuery.objectToQuery(params));
        }
    });
});
