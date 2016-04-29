/**
* @author wangyafei
* 饼状专题图
*/
define("widget/thematic/JZChartGraphics",
    ["dojo/_base/declare",
     "esri/graphic",
     "dojo/dom-style"
    ],
    function (declare, Graphic, domStyle) {

        return declare(Graphic, {
            bindGraphic: null,
            parentDiv: null,
            series: null,
            id: null,
            divHeight: null,
            divWidth: null,
            map: null,
            setId: function (id) {
                this.id = id;
            },
            setSeries: function (series) {
                this.series = series;
            },
            setDivHeight: function (height) {
                this.divHeight = height;
            },
            setDivWidth: function (width) {
                this.divWidth = width;
            },
            getSeries: function () {
                return this.series;
            },
            getDivHeight: function () {
                return this.divHeight;
            },
            getDivWidth: function () {
                return this.divWidth;
            },
            getId: function () {
                return this.id;
            },
            hide: function () {
                if (this.parentDiv) {
                    domStyle.set(this.parentDiv, "display", "none");
                }
            },
            show: function () {
                if (this.parentDiv) {
                    domStyle.set(this.parentDiv, "display", "");
                }
            },
            _getMap: function() {
                var gl = this._graphicsLayer;
                return gl._map;
            },
            constructor: function () {
            }
        });

    });