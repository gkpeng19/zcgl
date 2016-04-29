/**
* @author wangyafei
* 功能菜单
*/
define('widget/dijit/ContextMenu',
['dojo/_base/declare',
'esri/toolbars/navigation',
'dijit/Menu',
'dijit/MenuItem'],
function (declare, EsriNavigation, Menu, MenuItem) {
    "use strict";
    var instance = null, clazz;

    /* 根据用户的权限从数据库中获取用户的功能权限 */
    clazz = declare(null, {
        constructor: function () {

        },
        show: function () {
            var menu = new Menu({
                targetNodeIds: ["mapDiv"],
                style: {
                    display: "none",
                    width: "130px"
                }
            });

            menu.addChild(new MenuItem({
                label: "放大",
                iconClass: "dijitIcon dijitZoomInIcon",
                onClick: function (evt) {
                    JZ.mc.setMapCursor("crosshair");
                    JZ.en.activate(EsriNavigation.ZOOM_IN);
                }
            }));

            menu.addChild(new MenuItem({
                label: "缩小",
                iconClass: "dijitIcon dijitZoomOutIcon",
                onClick: function (evt) {
                    JZ.mc.setMapCursor("crosshair");
                    JZ.en.activate(EsriNavigation.ZOOM_OUT);
                }
            }));

            menu.addChild(new MenuItem({
                label: "平移",
                iconClass: "dijitIcon dijitPanIcon",
                onClick: function (evt) {
                    JZ.mc.setMapCursor("default");
                    JZ.en.activate(EsriNavigation.PAN);
                }
            }));

            menu.addChild(new MenuItem({
                label: "上一视图",
                iconClass: "dijitIcon dijitPrevIcon",
                onClick: function (evt) {
                    JZ.en.zoomToPrevExtent();
                }
            }));

            menu.addChild(new MenuItem({
                label: "下一视图",
                iconClass: "dijitIcon dijitNextIcon",
                onClick: function (evt) {
                    JZ.en.zoomToNextExtent();
                }
            }));

            menu.addChild(new MenuItem({
                label: "初始范围",
                iconClass: "dijitIcon dijitFullIcon",
                onClick: function (evt) {
                    JZ.mc.setExtent(JZ.ie);
                }
            }));

            menu.addChild(new MenuItem({
                label: "清除痕迹",
                iconClass: "dijitIcon dijitClearAllIcon",
                onClick: function (evt) {
                    JZ.mc.graphics.clear();
                    JZ.mc.infoWindow.hide();
                }
            }));
        }
    });

    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz();
        }
        return instance;
    };
    return clazz;
});