/*
<link rel="stylesheet" type="text/css" href="../esri/dijit/themes/claro/claro.css"/>
    <link rel="stylesheet" type="text/css" href="../esri/esri/css/esri.css" />
    <link rel="stylesheet" href="../css/thematic.css" />
        -->
    <script type="text/javascript">
        var JZ = {
            mc: null,
            ie: null,
            en: null
        };
        */
path = path.substr(0, path.length - 1);
path = path.substr(0, path.lastIndexOf("/") + 1);

var resources = [], dojoConfig = {
    parseOnLoad: false,
    async: true,
    tlmSiblingOfDojo: false,
    has: {
        'extend-esri': 1
    },
    locale: "zh-cn",
    baseUrl: apiUrl + 'dojo',
    packages: [{
        name: "widget",
        location: path + "widget"
    }]
}, JZ = {
    config: null,
    mc: null, //地图控件
    ie: null, //地图初始范围
    gs: null, //几何地图服务
    bms: null,
    district: null,
    //bms: [],//底图集合
    //bml: [],//当前地图的底图
    //cb: -1,//当前底图索引
    en: null, //地图导航
    ec: '', //当前地图范围的编码
    ztlayer: null, //当前的专题图
    smc: null//街景地图中的地图控件
};


function loadResources(ress, onOneBeginLoad, onOneLoad, onLoad) {
    var loaded = [];
    function _onOneLoad(url) {
        if (checkHaveLoaded(url)) {
            return;
        }
        loaded.push(url);
        if (onOneLoad) {
            onOneLoad(url, loaded.length);
        }
        if (loaded.length === ress.length) {
            if (onLoad) {
                onLoad();
            }
        }
    }
    for (var i = 0; i < ress.length; i++) {
        loadResource(ress[i], onOneBeginLoad, _onOneLoad);
    }
    function checkHaveLoaded(url) {
        for (var i = 0; i < loaded.length; i++) {
            if (loaded[i] === url) {
                return true;
            }
        }
        return false;
    }
}
function getExtension(url) {
    url = url || "";
    var items = url.split("?")[0].split(".");
    return items[items.length - 1].toLowerCase();
}
function loadResource(url, onBeginLoad, onLoad) {
    if (onBeginLoad) {
        onBeginLoad(url);
    }
    var type = getExtension(url);
    if (type.toLowerCase() === 'css') {
        loadCss(url);
    } else {
        loadJs(url);
    }
    function createElement(config) {
        var e = document.createElement(config.element);
        for (var i in config) {
            if (i !== 'element' && i !== 'appendTo') {
                e[i] = config[i];
            }
        }
        var root = document.getElementsByTagName(config.appendTo)[0];
        return (typeof root.appendChild(e) === 'object');
    }
    function loadCss(url) {
        var result = createElement({
            element: 'link',
            rel: 'stylesheet',
            type: 'text/css',
            href: url,
            onload: function () {
                elementLoaded(url);
            },
            appendTo: 'head'
        });

        var ti = setInterval(function () {
            var styles = document.styleSheets;
            for (var i = 0; i < styles.length; i++) {
                if (styles[i].href && styles[i].href.substr(styles[i].href.indexOf(url), styles[i].href.length) === url) {
                    clearInterval(ti);
                    elementLoaded(url);
                }
            }
        }, 500);

        return (result);
    }
    function loadJs(url) {
        var result = createElement({
            element: 'script',
            type: 'text/javascript',
            onload: function () {
                elementLoaded(url);
            },
            onreadystatechange: function () {
                elementReadyStateChanged(url, this);
            },
            src: url,
            appendTo: 'body'
        });
        return (result);
    }
    function elementLoaded(url) {
        if (onLoad) {
            onLoad(url);
        }
    }
    function elementReadyStateChanged(url, thisObj) {
        if (thisObj.readyState === 'loaded' || thisObj.readyState === 'complete') {
            elementLoaded(url);
        }
    }
}
resources = resources.concat([
    apiUrl + 'dijit/themes/claro/claro.css',
    apiUrl + 'esri/css/esri.css',
    path + "css/thematic.css"
]);
resources.push(apiUrl + 'init.js');
loadResources(resources, null, function (url, loaded) {
    if (typeof loadingCallback === 'function') {
        loadingCallback(url, loaded, resources.length);
    }
}, function () {
    continueLoad();
    function continueLoad() {
        if (typeof require === 'undefined' || typeof define === 'undefined') {
            if (window.console) {
                console.log('等待加载地图API...');
            }
            setTimeout(continueLoad, 100);
            return;
        } else {
            var script = document.createElement("script");
            script.type = "text/javascript";
            script.src = "thematic.js";
            document.body.appendChild(script);
        }
    }
});


function layercols(layerid, cols) {
    this.lid = layerid;
    this.lcos = cols;

}
function AllCols() {
    this.data = new Array();
    this.add = function layercols_add(layercols) {
        this.data[this.data.length] = layercols;
    }
    this.get = function layer_elementAt(layerid) {
        for (var i = 0; i < this.data.length; i++) {
            var laycos = this.data[i];
            if (laycos.lid == layerid) {
                return laycos.lcos;
            }
        }
        return null;
    }
}
window.AllLayerCols = new AllCols();





