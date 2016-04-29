/**
* @author wangyafei
* 饼状专题图，继承GraphicsLayer
*/
define("widget/thematic/JZChartLayer", ["dojo/_base/declare",
    "esri/layers/GraphicsLayer",
    "dojo/_base/lang",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/_base/array",
    "dojo/_base/window",
    "widget/thematic/JZPieChart"
],
    function (declare, GraphicsLayer, lang, dom, domStyle, Array, win, JZPieChart) {
        var JZChartLayer = declare(GraphicsLayer, {
            divid: null,
            layerUrl: null,

            setDivId: function (id) {
                this.divid = id;
            },

            constructor: function (layerurl, params) {
                this.layerUrl = layerurl;
                lang.mixin(this, params);
            },

            _draw: function (graphic, redraw, zoomFlag) {
                var that = this;
                if (!that._map) {
                    return;
                }
                if (graphic instanceof JZPieChart) {
                    that._drawChart(graphic, zoomFlag);
                }
            },
            _drawChart: function (pieGraphic, zoomFlag) {
                if (!pieGraphic.bindGraphic) {
                    return;
                }
                if (zoomFlag) {
                    dom.byId(this.divid).removeChild(pieGraphic.parentDiv);
                }
                if (pieGraphic.bindGraphic._extent) {
                    if (pieGraphic.bindGraphic._shape) {
                        var svgDojoShape = pieGraphic.bindGraphic.getDojoShape();
                        var svgl = svgDojoShape.bbox.l;
                        var svgt = svgDojoShape.bbox.t;
                        var svgr = svgDojoShape.bbox.r;
                        var svgb = svgDojoShape.bbox.b;

                        var svgWidth = svgr - svgl;
                        var svgHeight = svgb - svgt;
                        var value = 0;
                        var dw = 0;
                        var dh = 0;
                        if (svgWidth >= svgHeight) {
                            value = svgHeight;
                        } else {
                            value = svgWidth;
                        }

                        if (value > 82) {
                            value = 82;
                        }

                        pieGraphic.divWidth = value;
                        pieGraphic.divHeight = value;
                        var svgTransform = svgDojoShape.parent.matrix;
                        var pieDivX = (svgWidth - value) / 2 + svgl + svgTransform.dx;
                        var pieDivY = (svgHeight - value) / 2 + svgt + svgTransform.dy;

                        if (!pieGraphic.parentDiv || zoomFlag) {
                            var pieDiv = win.doc.createElement("div");
                            domStyle.set(pieDiv, {
                                "left": pieDivX + "px",
                                "top": pieDivY + "px",
                                "position": "absolute",
                                "width": pieGraphic.getDivWidth() + "px",
                                "height": pieGraphic.getDivHeight() + "px",
                                "margin": "0",
                                "padding": "0",
                                "z-index": 2
                            });
                            dom.byId(this.divid).appendChild(pieDiv);

                            if (value / 2 > 40) {
                                value = 40;
                            } else {
                                value = value / 2 - 1;
                            }
                            pieGraphic._draw(pieDiv, value);
                            pieGraphic.parentDiv = pieDiv;
                        } else if (pieGraphic.parentDiv) {
                            domStyle.set(pieGraphic.parentDiv, {
                                "left": pieDivX + "px",
                                "top": pieDivY + "px",
                                "position": "absolute",
                                "width": pieGraphic.getDivWidth() + "px",
                                "height": pieGraphic.getDivHeight() + "px",
                                "margin": "0",
                                "padding": "0",
                                "z-index": 2
                            });
                        }
                    }
                } else {
                    dom.byId(this.divid).removeChild(pieGraphic.parentDiv);
                    pieGraphic.parentDiv = null;
                }
            },

            show: function () {
                var length = this.bindGraphicLayer.graphics.length;
                var graphics = this.bindGraphicLayer.graphics;
                Array.forEach(graphics, function (graphic) {
                    if (graphic.parentDiv) {
                        domStyle.set(graphic.parentDiv, "display", "");
                    }
                });
            },

            hide: function () {
                var length = this.bindGraphicLayer.graphics.length;
                var graphics = this.bindGraphicLayer.graphics;
                Array.forEach(graphics, function (graphic) {
                    if (graphic.parentDiv) {
                        domStyle.set(graphic.parentDiv, "display", "none");
                    }
                });
            },
            _onPanStartHandler: function () {
                this.hide();
            },
            _onPanEndHandler: function () {
                this._refresh(false);
            },
            _onZoomStartHandler: function () {
                this.hide();
            },
            _refresh: function (redraw, zoomFlag) {
                var graphics = this.bindGraphicLayer.graphics;
                var length = graphics.length;
                var _draw = this._draw;
                var i = 0;
                if (!redraw) {
                    Array.forEach(graphics, function (graphic) {
                        _draw(graphic, redraw, zoomFlag);
                    });
                } else {
                    Array.forEach(graphics, function (graphic) {
                        _draw(graphic, redraw, zoomFlag);
                    });
                }
                this.show();
            },
            _onExtentChangeHandler: function (extent, delta, levelChange, lod) {
                if (levelChange) {
                    this._refresh(true, levelChange);
                }
            }
        });

        return JZChartLayer;
    });