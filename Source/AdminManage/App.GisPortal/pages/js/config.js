/**
* @author wangyafei
*/
var dojoConfig = {
    async: 1,
    isDebug: true,
    parseOnLoad: true,
    tlmSiblingOfDojo: false,
    packages: [
        { name: "widget", location: location.pathname.replace(/\/[^/]+$/, "") + "/../widget" },
    ]
};