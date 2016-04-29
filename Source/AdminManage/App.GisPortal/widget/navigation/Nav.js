/**
* @author wangyafei
*/
define("widget/navigation/Nav", ["dojo/_base/declare", "dojo/_base/lang", "dojox/gfx", "dojo/dom", "dojo/dom-style",
    "dojo/on", "dojo/mouse", "esri/toolbars/navigation"],
function (declare, lang, gfx, dom, domStyle, on, mouse, EsriNavigation) {
    "use strict";
    var lg1 = {
        type: "linear",
        x1: 50,
        y1: 10,
        x2: 50,
        y2: 110,
        colors: [
			{ offset: 0, color: [0, 150, 0, 1] },
			{ offset: 0.1, color: [0, 170, 0, 1] },
			{ offset: 0.2, color: [0, 190, 0, 1] },
			{ offset: 0.3, color: [0, 210, 0, 1] },
			{ offset: 0.4, color: [0, 230, 0, 1] },
			{ offset: 0.5, color: [0, 250, 0, 1] },
			{ offset: 0.6, color: [0, 230, 0, 1] },
			{ offset: 0.7, color: [0, 220, 0, 1] },
			{ offset: 0.8, color: [0, 210, 0, 1] },
			{ offset: 0.9, color: [0, 190, 0, 1] },
			{ offset: 1, color: [0, 170, 0, 1] }
		]
    };

    var lt1, lt2, lt3, lt4, tr1, tr2, tr3, tr4, rb1, rb2, rb3, rb4, lb1, lb2, lb3, lb4, n1, n2, n3, n4, p1, p2, p3, p4;

    /* 八方向导航控制点 */
    lt1 = { x: 13.04482, y: 34.69266 };
    lt2 = { x: 31.52241, y: 42.34633 };
    lt4 = { x: 34.69266, y: 13.04482 };
    lt3 = { x: 42.34633, y: 31.52241 };
    tr1 = { x: 65.30734, y: 13.04482 };
    tr2 = { x: 57.65367, y: 31.52241 };
    tr3 = { x: 68.47759, y: 42.34633 };
    tr4 = { x: 86.95518, y: 34.69926 };
    rb1 = { x: 86.95518, y: 65.30734 };
    rb2 = { x: 68.47759, y: 57.65367 };
    rb3 = { x: 57.65367, y: 68.47759 };
    rb4 = { x: 65.30734, y: 86.95518 };
    lb1 = { x: 34.69266, y: 86.95518 };
    lb2 = { x: 42.34633, y: 68.47759 };
    lb3 = { x: 31.52241, y: 57.65367 };
    lb4 = { x: 13.04482, y: 65.30734 };

    /* 上下视图控制点 */
    n1 = { x: 71.95518, y: 94.69926 };
    n2 = { x: 53.47759, y: 102.34633 };
    n3 = { x: 53.47759, y: 117.65367 };
    n4 = { x: 71.95518, y: 125.30734 };
    p1 = { x: 28.04482, y: 125.30734 };
    p2 = { x: 46.52241, y: 117.65367 };
    p3 = { x: 46.52241, y: 102.34633 };
    p4 = { x: 28.04482, y: 94.69266 };

    var Nav = declare([], {
        defaultOptions: {
            radius: 30,
            width: 100,
            height: 500,
            fillColor: [0, 153, 180, 1],
            hoverColor: [0, 153, 220, 1]
        },

        constructor: function (options) {
            this.options = lang.mixin(lang.clone(this.defaultOptions), (options || {}));

            //计算转换参数
            this.getTransferParams();
            if (this.options["map"]) {
                //on(this.options["map"], "load", function () {
                    this.draw();
                //});
            }
        },

        getTransferParams: function () {
            /* 半径不同时的矩阵转换参数 */
            this.tp = this.options["radius"] / 40;
        },

        draw: function () {
            var that = this;
            var surface = gfx.createSurface(this.options['domNode'], 80, 400);
            var surfaceSize = { width: 80, height: 400 };
            var esriNavigation = this.options['en'];
            var map = this.options["map"];
            var domNode = this.options["domNode"];
            var initExtent = this.options["initExtent"] ? this.options["initExtent"] : JZ.ie;

            on(domNode, mouse.enter, function (e) {
                domStyle.set(this, "opacity", 1);
            });

            on(domNode, mouse.leave, function (e) {
                domStyle.set(this, "opacity", 0.9);
            })

            var fillColor = this.options["fillColor"];
            var hoverColor = this.options["hoverColor"];

            /* 地图放大到初始范围 */
            var centerImg = surface.createImage({ width: 26, height: 26, src: "data:image/png;base64,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" }).setTransform({ dx: 25, dy: 25 });
            var initCenter1 = surface.createCircle({ cx: 50, cy: 50, r: 20 }).setTransform({ xx: that.tp, yy: that.tp });
            initCenter1.setStroke(fillColor);
            var initCenter = null;
            on(centerImg, mouse.enter, function (e) {
                initCenter = surface.createCircle({ cx: 50, cy: 50, r: 19 }).setTransform({ xx: that.tp, yy: that.tp });
                initCenter.setStroke({ color: "yellow", width: 1 }).setTransform({ xx: that.tp, yy: that.tp });
            });
            on(centerImg, mouse.leave, function (e) {
                initCenter.setStroke({ color: "yellow", width: 0 }).setTransform({ xx: that.tp, yy: that.tp });
            });
            on(centerImg, "click", function (e) {
                map.setExtent(initExtent);
            });

            //向上移动
            var tPath = surface.createPath("");
            tPath.moveTo(lt4).lineTo(lt3).arcTo(15, 15, 45, false, true, tr2)
	        .lineTo(tr1).arcTo(40, 40, 45, false, false, lt4).closePath();
            tPath.setFill(fillColor).setStroke(fillColor).setTransform({ xx: that.tp, yy: that.tp });

            var t1Line = surface.createLine({ x1: 50, y1: 15, x2: 43, y2: 23 });
            t1Line.setStroke({ color: "white", width: 2 }).setTransform({ xx: that.tp, yy: that.tp });
            var t2Line = surface.createLine({ x1: 50, y1: 15, x2: 57, y2: 23 });
            t2Line.setStroke({ color: "white", width: 2 }).setTransform({ xx: that.tp, yy: that.tp });

            on(tPath, mouse.enter, function (e) {
                tPath.setFill(hoverColor);
                t1Line.setStroke({ color: "yellow", width: 2 });
                t2Line.setStroke({ color: "yellow", width: 2 });
            });

            on(tPath, mouse.leave, function (e) {
                tPath.setFill(fillColor);
                t1Line.setStroke({ color: "white", width: 2 });
                t2Line.setStroke({ color: "white", width: 2 });
            });

            on(tPath, "click", function (e) {
                map.panUp();
            });

            //右上移动
            var trPath = surface.createPath("");
            trPath.moveTo(tr1).lineTo(tr2).arcTo(15, 15, 45, false, true, tr3)
	        .lineTo(tr4).arcTo(40, 40, 45, false, false, tr1).closePath();
            trPath.setFill(fillColor).setStroke(fillColor).setTransform({ xx: that.tp, yy: that.tp });

            on(trPath, mouse.enter, function (e) {
                trPath.setFill(hoverColor);
            });

            on(trPath, mouse.leave, function (e) {
                trPath.setFill(fillColor);
            });

            on(trPath, "click", function (e) {
                map.panUpperRight();
            });

            //向右移动
            var rPath = surface.createPath("");
            rPath.moveTo(tr4).lineTo(tr3).arcTo(15, 15, 45, false, true, rb2)
	        .lineTo(rb1).arcTo(40, 40, 45, false, false, tr4).closePath();
            rPath.setFill(fillColor).setStroke(fillColor).setTransform({ xx: that.tp, yy: that.tp });

            var r1Line = surface.createLine({ x1: 85, y1: 50, x2: 77, y2: 58 });
            r1Line.setStroke({ color: "white", width: 2 }).setTransform({ xx: that.tp, yy: that.tp });
            var r2Line = surface.createLine({ x1: 85, y1: 50, x2: 77, y2: 42 });
            r2Line.setStroke({ color: "white", width: 2 }).setTransform({ xx: that.tp, yy: that.tp });

            on(rPath, mouse.enter, function (e) {
                rPath.setFill(hoverColor);

                r1Line.setStroke({ color: "yellow", width: 2 });
                r2Line.setStroke({ color: "yellow", width: 2 });
            });

            on(rPath, mouse.leave, function (e) {
                rPath.setFill(fillColor);

                r1Line.setStroke({ color: "white", width: 2 });
                r2Line.setStroke({ color: "white", width: 2 });
            });

            on(rPath, "click", function (e) {
                map.panRight();
            });

            //右下移动
            var rbPath = surface.createPath("");
            rbPath.moveTo(rb1).lineTo(rb2).arcTo(15, 15, 45, false, true, rb3)
	        .lineTo(rb4).arcTo(40, 40, 45, false, false, rb1).closePath();
            rbPath.setFill(fillColor).setStroke(fillColor).setTransform({ xx: that.tp, yy: that.tp });

            on(rbPath, mouse.enter, function (e) {
                rbPath.setFill(hoverColor);
            });

            on(rbPath, mouse.leave, function (e) {
                rbPath.setFill(fillColor);
            });

            on(rbPath, "click", function (e) {
                map.panLowerRight();
            });

            //向下移动
            var bPath = surface.createPath("");
            bPath.moveTo(rb4).lineTo(rb3).arcTo(15, 15, 45, false, true, lb2)
	        .lineTo(lb1).arcTo(40, 40, 45, false, false, rb4).closePath();
            bPath.setFill(fillColor).setStroke(fillColor).setTransform({ xx: that.tp, yy: that.tp });

            var b1Line = surface.createLine({ x1: 50, y1: 85, x2: 42, y2: 77 });
            b1Line.setStroke({ color: "white", width: 2 }).setTransform({ xx: that.tp, yy: that.tp });
            var b2Line = surface.createLine({ x1: 50, y1: 85, x2: 58, y2: 77 });
            b2Line.setStroke({ color: "white", width: 2 }).setTransform({ xx: that.tp, yy: that.tp });

            on(bPath, mouse.enter, function (e) {
                bPath.setFill(hoverColor);

                b1Line.setStroke({ color: "yellow", width: 2 });
                b2Line.setStroke({ color: "yellow", width: 2 });
            });

            on(bPath, mouse.leave, function (e) {
                bPath.setFill(fillColor);

                b1Line.setStroke({ color: "white", width: 2 });
                b2Line.setStroke({ color: "white", width: 2 });
            });

            on(bPath, "click", function (e) {
                map.panDown();
            });

            //左下移动
            var lbPath = surface.createPath("");
            lbPath.moveTo(lb1).lineTo(lb2).arcTo(15, 15, 45, false, true, lb3)
	        .lineTo(lb4).arcTo(40, 40, 45, false, false, lb1).closePath();
            lbPath.setFill(fillColor).setStroke(fillColor).setTransform({ xx: that.tp, yy: that.tp });

            on(lbPath, mouse.enter, function (e) {
                lbPath.setFill(hoverColor);
            });

            on(lbPath, mouse.leave, function (e) {
                lbPath.setFill(fillColor);
            });

            on(lbPath, "click", function (e) {
                map.panLowerLeft();
            });

            //向左移动
            var lPath = surface.createPath("");
            lPath.moveTo(lb4).lineTo(lb3).arcTo(15, 15, 45, false, true, lt2)
	        .lineTo(lt1).arcTo(40, 40, 45, false, false, lb4).closePath();
            lPath.setFill(fillColor).setStroke(fillColor).setTransform({ xx: that.tp, yy: that.tp });

            var left1Line = surface.createLine({ x1: 15, y1: 50, x2: 23, y2: 42 });
            left1Line.setStroke({ color: "white", width: 2 }).setTransform({ xx: that.tp, yy: that.tp });
            var left2Line = surface.createLine({ x1: 15, y1: 50, x2: 23, y2: 58 });
            left2Line.setStroke({ color: "white", width: 2 }).setTransform({ xx: that.tp, yy: that.tp });

            on(lPath, mouse.enter, function (e) {
                lPath.setFill(hoverColor);

                left1Line.setStroke({ color: "yellow", width: 2 });
                left2Line.setStroke({ color: "yellow", width: 2 });
            });

            on(lPath, mouse.leave, function (e) {
                lPath.setFill(fillColor);

                left1Line.setStroke({ color: "white", width: 2 });
                left2Line.setStroke({ color: "white", width: 2 });
            });

            on(lPath, "click", function (e) {
                map.panLeft();
            });

            //左上移动
            var ltPath = surface.createPath("");
            ltPath.moveTo(lt1).lineTo(lt2).arcTo(15, 15, 45, false, true, lt3)
	        .lineTo(lt4).arcTo(40, 40, 45, false, false, lt1).closePath();
            ltPath.setFill(fillColor).setStroke(fillColor).setTransform({ xx: that.tp, yy: that.tp });

            on(ltPath, mouse.enter, function (e) {
                ltPath.setFill(hoverColor);
            });

            on(ltPath, mouse.leave, function (e) {
                ltPath.setFill(fillColor);
            });

            on(ltPath, "click", function (e) {
                map.panUpperLeft();
            });

            //上一视图
            var previousPath = surface.createPath("");
            previousPath.moveTo(p1).lineTo(p2).arcTo(15, 15, 45, false, true, p3)
	        .lineTo(p4).arcTo(40, 40, 45, false, false, p1).closePath();
            previousPath.setFill(fillColor).setStroke(fillColor).setTransform({ xx: that.tp, yy: that.tp });

            var p1Line = surface.createLine({ x1: 30, y1: 110, x2: 38, y2: 118 });
            p1Line.setStroke({ color: "white", width: 2 }).setTransform({ xx: that.tp, yy: that.tp });
            var p2Line = surface.createLine({ x1: 30, y1: 110, x2: 38, y2: 102 });
            p2Line.setStroke({ color: "white", width: 2 }).setTransform({ xx: that.tp, yy: that.tp });

            on(previousPath, mouse.enter, function (e) {
                previousPath.setFill(hoverColor);
                p1Line.setStroke({ color: "yellow", width: 2 });
                p2Line.setStroke({ color: "yellow", width: 2 });
            });

            on(previousPath, mouse.leave, function (e) {
                previousPath.setFill(fillColor);
                p1Line.setStroke({ color: "white", width: 2 });
                p2Line.setStroke({ color: "white", width: 2 });
            });

            on(previousPath, "click", function (e) {
                if (esriNavigation) {
                    esriNavigation.zoomToPrevExtent();
                }
            });

            //下一视图
            var nextPath = surface.createPath("");
            nextPath.moveTo(n1).lineTo(n2).arcTo(15, 15, 45, false, true, n3)
	        .lineTo(n4).arcTo(40, 40, 45, false, false, n1).closePath();
            nextPath.setFill(fillColor).setStroke(fillColor).setTransform({ xx: that.tp, yy: that.tp });

            var n1Line = surface.createLine({ x1: 70, y1: 110, x2: 62, y2: 118 });
            n1Line.setStroke({ color: "white", width: 2 }).setTransform({ xx: that.tp, yy: that.tp });
            var n2Line = surface.createLine({ x1: 70, y1: 110, x2: 62, y2: 102 });
            n2Line.setStroke({ color: "white", width: 2 }).setTransform({ xx: that.tp, yy: that.tp });

            on(nextPath, mouse.enter, function (e) {
                nextPath.setFill(hoverColor);
                n1Line.setStroke({ color: "yellow", width: 2 });
                n2Line.setStroke({ color: "yellow", width: 2 });
            });

            on(nextPath, mouse.leave, function (e) {
                nextPath.setFill(fillColor);
                n1Line.setStroke({ color: "white", width: 2 });
                n2Line.setStroke({ color: "white", width: 2 });
            });

            on(nextPath, "click", function (e) {
                if (esriNavigation) {
                    esriNavigation.zoomToNextExtent();
                }
            });

            var panRect = surface.createRect({ x: 35, y: 410, width: 30, height: 30 });
            panRect.setFill(fillColor).setStroke(hoverColor).setTransform({ xx: that.tp, yy: that.tp });
            var zoominRect = surface.createRect({ x: 35, y: 490, width: 30, height: 30 });
            zoominRect.setFill(fillColor).setStroke(hoverColor).setTransform({ xx: that.tp, yy: that.tp });
            var zoomoutRect = surface.createRect({ x: 35, y: 450, width: 30, height: 30 });
            zoomoutRect.setFill(fillColor).setStroke(hoverColor).setTransform({ xx: that.tp, yy: that.tp });

            var panImg = surface.createImage({ width: 16, height: 16, src: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAEVSURBVDhPjdNPK0RRGMfxi0KZWPiz0yxMWCBTrAgjGyVpmqxsFGFH0cRCIkUSsbKx8xq8Pd/v3FncnHvuzK8+zZnbnGfOec65SYcs4gAT6PNBt5nFEb5wgx/cYwYdM4QXXOMDc3jFPh4whcLVjOERruIO87DYEn7xiXVEMw4LuNxsgWVcYgsniKaowDk2cIYgPehFtsAt7EEThQWcvIcnNGCBUdiHYUyjDCfXEBTwR+9YwTc8tkHkZRNBAZfuea+iCs+8H3nJLWAW4Ba8BwM+iMQCp+kwzAXsRVF2cJgOw0zirf0ZyxXW0mF+tuEpeJz/48RnlFrfIrGhbsP7b7Pq8Gj9Z3tUQVdxG7s4hm+mV3gEmSTJH6GBJvBJKvVSAAAAAElFTkSuQmCC" })
	        .setTransform({ dx: 29, dy: 310 });

            var zoomoutImg = surface.createImage({ width: 16, height: 16, src: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAJXSURBVDhPY8AH4ltfa6S0vZ6b3Pr6PhD/AtEgPkgcqgQ3iKi6Hxhd8/B+0/ynJ7acfLvrzrPPJ45c+Xigd9nLUyBxkDxUKSZwyzmj4Z178f7UjQ92HL7++tCLD99vffz26/nrTz9vP3rz7ez8bY93+QDlQeqgWlCBVcz+uXm9F0/svvjiwJVHH06ANFvFHPgPoh+/+Xbu2pNPJ0r6L54CqYNqQQXavpvuz912d/vpu28v3Hn5+cG7Lz8/6Pht+g+iQXyQ+NI99/aA1EG1oAJlt9W/dpx/fgCkWMVt9X90/OHbr2eHrr46BlIH1YIKlJzX3J+/7d5ukE0gxT9+/fms5LL2P4gGh8WHn7dW7ntwCKQOqgUVqHlun5vVdu7svquvDt9/8eU0SJO65w54GAAD8lxx14VLIHVQLajA0jHTyjD40Iu5m++fvfn002lQ6INtBtLP3n+7uH7fy1PGwSfv6wQdxYwFS0tLeWtr6+KOyXv+W0ZeeNUw/dmlE1c+n37+7vv1U1c+n2mf+/Kadeyt+6YR1zDTgY2NjZarq3vNjh17/ouKijarWjbbOSQ/m+uY/Oy+Y9KzXyAaxLePf4ppM9BWi8DA4LYjR878V1BQ6mFjY1OFShEGtra23vHxyRMuXrz7X1dXfxorK6sxVIo4YGZmVnLr1qv/dnbO85iZmV2gwkQDDjExMRdhYeEWFhaWbCBfH4h1gBjkTxUglgdiaSAWA2JBIOYBYg4gZgViJiBmYAFikKQUEEsCsQSULwLEQkAsAMR8QMwNxJxAzAbEID3MDAwMjABTEVKEuKeyEgAAAABJRU5ErkJggg==" })
	        .setTransform({ dx: 29, dy: 340 });

            var zoominImg = surface.createImage({ width: 16, height: 16, src: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAIvSURBVDhPY8AH4ltfa6S0vZ6b3Pr6PhD/AtEgPkgcqgQ3iKi6Hxhd8/B+0/ynJ7acfLvrzrPPJ45c+Xigd9nLUyBxkDxUKSZwyzmj4Z178f7UjQ92HL7++tCjN9/OPn7z7RwIg9jztz3e5QOUB6mDakEFVjH75+b1Xjyx++KLA1cefTgB0wzD1558OlHSf/EUSB1UCyrQ9t10f+62u9tP33174c7Lzw/QMUh86Z57e0DqoFpQgbLb6l87zj8/AFKs4rb6Pzr+8O3Xs0NXXx0DqYNqQQVKzmvuz992bzfIJpDij99+PUfGrz/8vLVy34NDIHVQLahAzXP73Ky2c2f3XX11+P6LL6fRwwAYkOeKuy5cAqmDakEFlo6ZVobBh17M3Xz/7M2nn1AMePb+28X1+16eMg4+eV8n6ChmLFhaWspbW1sXd0ze898y8sKrhunPLp248vn083ffr5+68vlM+9yX16xjb903jbiGmQ5sbGy0XF3da3bs2PNfVFS0WdWy2c4h+dlcx+Rn9x2Tnv0C0SC+ffxTTJuBtloEBga3HTly5r+CglIPGxubKlSKMLC1tfWOj0+ecPHi3f+6uvrTWFlZjaFSxAEzM7OSW7de/bezc57HzMzsAhUmGnCIiYm5CAsLt7CwsGQD+fpArAPEIH+qALE8EEsDsRgQCwIxDxBzADErEDMBMQMLEIMkpYBYEogloHwRIBYCYgEg5gNibiDmBGI2IAbpYWZgYGAEAGZ0Vgkfk9UxAAAAAElFTkSuQmCC" })
	        .setTransform({ dx: 29, dy: 370 });

            on(panRect, mouse.enter, function (e) {
                panRect.setFill(hoverColor);
            });

            on(panRect, mouse.leave, function (e) {
                panRect.setFill(fillColor);
            });

            on(panImg, mouse.enter, function (e) {
                panRect.setFill(hoverColor);
            });

            on(panImg, mouse.leave, function (e) {
                panRect.setFill(fillColor);
            });

            on(panImg, "click", function (e) {
                esriNavigation.activate(EsriNavigation.PAN);
                map.setCursor("default");
            });

            on(zoominRect, mouse.enter, function (e) {
                zoominRect.setFill(hoverColor);
            });

            on(zoominRect, mouse.leave, function (e) {
                zoominRect.setFill(fillColor);
            });

            on(zoominImg, mouse.enter, function (e) {
                zoominRect.setFill(hoverColor);
            });

            on(zoominImg, mouse.leave, function (e) {
                zoominRect.setFill(fillColor);
            });

            on(zoominImg, "click", function (e) {
                esriNavigation.activate(EsriNavigation.ZOOM_OUT);
                map.setCursor("crosshair");
            });

            on(zoomoutRect, mouse.enter, function (e) {
                zoomoutRect.setFill(hoverColor);
            });

            on(zoomoutRect, mouse.leave, function (e) {
                zoomoutRect.setFill(fillColor);
            });

            on(zoomoutImg, mouse.enter, function (e) {
                zoomoutRect.setFill(hoverColor);
            });

            on(zoomoutImg, mouse.leave, function (e) {
                zoomoutRect.setFill(fillColor);
            });

            on(zoomoutImg, "click", function (e) {
                esriNavigation.activate(EsriNavigation.ZOOM_IN);
                map.setCursor("crosshair");
            });
        }
    });
    return Nav;
});