var resources = [apiUrl + 'dijit/themes/claro/claro.css', apiUrl + 'esri/css/esri.css'];

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

function elementLoaded(url) {
    console.log(url);
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
    /*
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
    */
}

for (var i = 0, length = resources.length; i < length; i++) {
    loadCss(resources[i]);
}