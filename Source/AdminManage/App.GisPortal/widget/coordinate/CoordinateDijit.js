define([
    'dojo/_base/declare',
    'dijit/_WidgetBase',
    'dijit/_TemplatedMixin',
    'dojo/text!./Coordinate.html',
    'dojo/on',
    'dojo/_base/lang',
    'dojo/_base/html',
    'dojo/dom-class',
    'esri/tasks/ProjectParameters',
    'esri/SpatialReference'
],
  function (
    declare,
    _WidgetBase,
    _TemplatedMixin,
    template,
    on,
    lang,
    html,
    domClass,
    ProjectParameters,
    SpatialReference
  ) {
      var clazz = declare([_WidgetBase, _TemplatedMixin], {
          templateString: template,
          baseClass: 'widget-coordinate',
          _mapWkt: null,
          _parentNode: null,
          map: null,
          gs: null,
          enableRealtime: false,

          startup: function () {
              this.inherited(arguments);
              this._mapWkt = this.map.spatialReference.wkt;
              this.onOpen();
          },

          onOpen: function () {
              domClass.add(this.coordinateBackground, "coordinate-background");
              this.own(on(this.map, "mouse-move", lang.hitch(this, this.onMouseMove)));
              this.own(on(this.map, "click", lang.hitch(this, this.onMapClick)));
              this.own(on(this.coordinateInfo, "click", lang.hitch(this, this.onGetRealTimeCoord)));
          },

          onGetRealTimeCoord: function (event) {
              if (this.enableRealtime) {
                  this.enableRealtime = false;
                  html.setAttr(this.coordinateInfo, 'title', '点击获取实时坐标');
              } else {
                  this.enableRealtime = true;
                  html.setAttr(this.coordinateInfo, 'title', '点击关闭实时坐标');
              }
          },
          onMapClick: function (evt) {
              this._displayOnClient(evt.mapPoint);
              this.coordinateInfo.innerHTML = "正在计算中...";
              if (!this.enableRealtime) {
                  html.setAttr(this.coordinateInfo, 'title', '点击获取实时坐标');
              }
          },

          _displayOnClient: function (point) {
              var params = new ProjectParameters();
              params.geometries = [point];
              params.outSR = new SpatialReference({
                  wkid: 4326
              });
              this.gs.project(params, lang.hitch(this, this.onProjectComplete), lang.hitch(this, this.onError));
          },

          onProjectComplete: function (geometries) {
              var point = geometries[0], x = point.x, y = point.y;
              this._displayProject(y, x);
          },

          onMouseMove: function (evt) {
              if (!this.enableRealtime) {
                  return;
              }

              this._displayOnClient(evt.mapPoint);
          },

          _displayProject: function (y, x) {
              this.coordinateInfo.innerHTML = "经度:" + x.toFixed(3) + ", 纬度" + y.toFixed(3);
          },

          /**
           * Helper function to prettify decimal degrees into DMS (degrees-minutes-seconds).
           *
           * @param {number} decDeg The decimal degree number
           * @param {string} decDir LAT or LON
           *
           * @return {string} Human-readable representation of decDeg.
           */
          degToDMS: function (decDeg, decDir) {
              /** @type {number} */
              var d = Math.abs(decDeg);
              /** @type {number} */
              var deg = Math.floor(d);
              d = d - deg;
              /** @type {number} */
              var min = Math.floor(d * 60);
              /** @type {number} */
              var sec = Math.floor((d - min / 60) * 60 * 60);
              if (sec === 60) { // can happen due to rounding above
                  min++;
                  sec = 0;
              }
              if (min === 60) { // can happen due to rounding above
                  deg++;
                  min = 0;
              }
              /** @type {string} */
              var min_string = min < 10 ? "0" + min : min;
              /** @type {string} */
              var sec_string = sec < 10 ? "0" + sec : sec;
              /** @type {string} */
              var dir = (decDir === 'LAT') ? (decDeg < 0 ? "S" : "N") : (decDeg < 0 ? "W" : "E");

              return (decDir === 'LAT') ?
                deg + "&deg;" + min_string + "&prime;" + sec_string + "&Prime;" + dir :
                deg + "&deg;" + min_string + "&prime;" + sec_string + "&Prime;" + dir;
          },

          separator: function (nStr, places) {
              if (this.config.addSeparator && JSON.parse(this.config.addSeparator)) {
                  return utils.localizeNumber(nStr, {
                      places: places
                  });
              }
              return nStr;
          }
      });

      return clazz;
  });