function GetProjectPath() {
    var path, fullPath = window.location.pathname;
    if (fullPath === "/" || fullPath.substr(fullPath.length - 1) === "/") {
        path = fullPath;
    } else if (/\.html$/.test(fullPath.split("/").pop())) {
        var sections = fullPath.split("/");
        sections.pop();
        path = sections.join("/") + "/";
    } else if (/\.aspx/.test(fullPath.split("/").pop())) {
        var sections = fullPath.split("/");
        sections.pop();
        path = sections.join("/") + "/";
    } else {
        return false;
    }
    return path;
}

var apiUrl = "http://10.246.62.11/arcgis_js_api/jsapi/",
    path = GetProjectPath();



