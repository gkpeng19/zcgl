/**
* @author wangyafei
* 向地图控件中添加地图和业务图层
* 图层从数据库中动态获取
* 底图切换
*/
define("widget/layer/PrintMap",
[
"dojo/_base/declare",
"dojo/request/script",
"dojo/topic"
],
function (declare, script, topic) {
    "use strict";

    /* 底图切换功能尚未完成 */
    var PrintMap = declare([], {
        constructor: function () {
        },

        getBaseLayer: function () {
            /* 获取数据库中配置的底图 */
            var defer = script.get("../webservice/WebServiceMap.asmx/GetBaseMapLayer", {
                jsonp: "callback",
                timeout: 3000
            });

            /* 请求成功事件 */
            defer.then(function (suc) {
                topic.publish("topic/basemap-complete", suc);
            }, function (err) {
                /* 如果没有登录，则直接跳转到登录页面 */
                console.log("底图服务GetMapLayer()出现错误，请查看后台日志！");
            });
        }
    });

    return PrintMap;
});