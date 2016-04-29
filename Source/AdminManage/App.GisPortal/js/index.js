/**
 * @author wangyafei
 */
function callback() {
    require(["widget/dijit/EditFeature"], function (clazz) {
        clazz.getInstance().onCancel();
    });
}
//格式化时间
Date.prototype.Format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "h+": this.getHours(),
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        "S": this.getMilliseconds()
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        } 
    }
    return fmt;
}
var ie = (function () {
    var undef, v = 3, div = document.createElement('div'), all = div.getElementsByTagName('i');
    div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->';
    while (all[0]) {
        div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->';
    }
    return v > 4 ? v : undef;
}());

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

/* 阻止浏览器的默认行为 */
function stopEvent(e) {
    if (e && e.preventDefault) {
        e.preventDefault();
    } else {
        window.event.returnValue = false;
    }
}

/* 阻止事件冒泡 */
function stopBubble(e) {
    if (e && e.stopPropagation) {
        e.stopPropagation();
    } else {
        window.event.cancelBubble = true;
    }
}

var dojoConfig,
    JZ = {
        privilegeLayers:[],//用户权限下的所有的图层组及图层
        loading: null,//loading条
        mc: null,//地图控件
        gs: null,//GeometryService
        en: null,//Esri-Navigation
        ie: null,//地图初始范围
        config: null,//获取的配置文件中配置的信息
        baselayers: [],//底图信息
        //layersList: [],//图层信息
        district: null,//用户所拥有的区县权限
        //ec: null,//用户所拥有的区县权限的区县编码
        currentLayerID: -1,//当前选中图层的ID
        currentYear: -1,//当前选中的年份数据
        //featureLayers: [],//当前地图中添加的要素图层
        layerinfos: [],//当前地图中添加的图层的ID和索引信息
        dynamicLayer: null,
        mapclick: null//地图点击事件
    };

(function (global) {
    if (ie < 8) {
        var mainLoading = document.getElementById('main-loading');
        var appLoading = document.getElementById('app-loading');
        var ieNotes = document.getElementById('ie-note');
        appLoading.style.display = 'none';
        ieNotes.style.display = 'block';
        mainLoading.style.backgroundColor = "#fff";
        return;
    }

    //需要预先加载的资源
    dojoConfig = {
        parseOnLoad: false,
        async: true,
        tlmSiblingOfDojo: false,
        has: {
            'extend-esri': 1
        },
        packages: [{
            name: "widget",
            location: getPath() + "widget"
        }]
    };

    var resources = [getPath() + 'esri/dgrid/css/dgrid.css',
                     getPath() + 'css/index.css',
                     getPath() + 'css/bootstrap.css',
                     getPath() + 'esri/esri/css/esri.css',
                     getPath() + 'esri/dojox/widget/Dialog/Dialog.css',
                     getPath() + 'esri/dojox/image/resources/Lightbox.css',
                     getPath() + 'esri/dijit/themes/claro/claro.css',
                     getPath() + 'esri/init.js'];

    //计算并获取url中的pathname
    function getPath() {
        var fullPath, path;
        fullPath = window.location.pathname;
        if (fullPath === '/' || fullPath.substr(fullPath.length - 1) === '/') {
            path = fullPath;
        } else if (/\.html$/.test(fullPath.split('/').pop())) {
            var sections = fullPath.split('/');
            sections.pop();
            path = sections.join('/') + '/';
        } else if (/\.aspx/.test(fullPath.split('/').pop())) {
            var sections = fullPath.split('/');
            sections.pop();
            path = sections.join('/') + '/';
        } else {
            return false;
        }
        return path;
    }

    //计算加载文件的扩展名
    function getExtension(url) {
        url = url || "";
        var items = url.split("?")[0].split(".");
        return items[items.length - 1].toLowerCase();
    }

    function loadResources(ress, onOneLoad, onLoad) {
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
            loadResource(ress[i], _onOneLoad);
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

    function loadResource(url, onLoad) {
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
                type: 'text/css',
                rel: 'stylesheet',
                href: url,
                appendTo: 'head',
                onload: function () {
                    elementLoaded(url);
                }
            });

            //有的浏览器不会触发onload事件，比如safari，但是会更新document.styleSheets
            var ti = setInterval(function () {
                var styles = document.styleSheets;
                for (var i = 0; i < styles.length; i++) {
                    if (styles[i].href && styles[i].href.substr(styles[i].href.indexOf(url), styles[i].href.length) === url) {
                        clearInterval(ti);
                        elementLoaded(url);
                    }
                }
            }, 500);

            return result;
        }

        function loadJs(url) {
            var result = createElement({
                element: 'script',
                type: 'text/javascript',
                src: url,
                appendTo: 'body',
                onload: function () {
                    elementLoaded(url);
                },
                onreadystatechange: function () {
                    if (this.readyState === 'loaded' || this.readyState === 'complete') {
                        elementLoaded(url);
                    }
                }
            });
        }

        function elementLoaded(url) {
            if (onLoad) {
                onLoad(url);
            }
        }
    }


    loadResources(resources, function (url, loaded) {
        if (typeof loadingCallback === 'function') {
            loadingCallback(url, loaded, resources.length);
        }
    }, function () {
        continueLoad();
    });


    function continueLoad() {
        if (typeof require === 'undefined') {
            setTimeout(continueLoad, 100);
            return;
        }
        //配置模块路径
        require(['widget/main'], function (main) {
            loadingCallback('jimu', resources.length + 1, resources.length);
            main.initApp(resources.length);
        });
    }

})(window);
