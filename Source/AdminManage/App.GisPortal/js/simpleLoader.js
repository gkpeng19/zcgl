(function(global) {
	/*
	 加载JS和CSS文件资源
	 * */
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

	/*
	 计算加载文件的扩展名
	 * */
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
			return ( typeof root.appendChild(e) === 'object');
		}

		function loadCss(url) {
			var result = createElement({
				element : 'link',
				rel : 'stylesheet',
				type : 'text/css',
				href : url,
				onload : function() {
					elementLoaded(url);
				},
				appendTo : 'head'
			});
			var ti = setInterval(function() {
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
				element : 'script',
				type : 'text/javascript',
				onload : function() {
					elementLoaded(url);
				},
				onreadystatechange : function() {
					elementReadyStateChanged(url, this);
				},
				src : url,
				appendTo : 'body'
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

	function is(type, obj) {
		var clas = Object.prototype.toString.call(obj).slice(8, -1);
		return obj !== undefined && obj !== null && clas === type;
	}

	function isArray(item) {
		return is("Array", item);
	}
	global.loadResources = loadResources;
	global.loadResource = loadResource;
})(window);
