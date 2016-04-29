var dojoConfig, jimuConfig;
var ie = ( function() {
		var undef, v = 3, div = document.createElement('div'), all = div.getElementsByTagName('i');
		div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->';
		while (all[0]) {
			div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->';
		}
		return v > 4 ? v : undef;
	}());

(function(argument) {
	if (ie < 8) {
		var mainLoading = document.getElementById('main-loading');
		var appLoading = document.getElementById('app-loading');
		var ieNotes = document.getElementById('ie-note');
		appLoading.style.display = 'none';
		ieNotes.style.display = 'block';
		mainLoading.style.backgroundColor = "#fff";
		return;
	}

	var resources = [];

	if (!window.apiUrl) {
		console.error('请配置arcgis api for javascript的路径！');
	} else if (!window.path) {
		console.error('发生错误，请查看window.path是否有值！');
	} else {
		if (window.location.protocol === 'https:') {
			var reg = /^http:\/\//i;
			if (reg.test(window.apiUrl)) {
				window.apiUrl = window.apiUrl.replace(reg, 'https://');
			}
			if (reg.test(window.path)) {
				window.path = window.path.replace(reg, 'https://');
			}
		}
		dojoConfig = {
			parseOnLoad : false,
			async : true,
			tlmSiblingOfDojo : false,
			has : {
				'extend-esri' : 1
			}
		};
		resources = resources.concat([window.apiUrl + 'dojo/resources/dojo.css', 
		window.apiUrl + 'dijit/themes/claro/claro.css', 
		window.apiUrl + 'esri/css/esri.css', 
		window.apiUrl + 'dojox/layout/resources/ResizeHandle.css'],
		window.path + 'jimu/css/window.css');
		dojoConfig.baseUrl = window.apiUrl + 'dojo';
		dojoConfig.packages = [{
			name : "widgets",
			location : window.path + "widget"
		}, {
			name : "jimu",
			location : window.path + "jimu"
		}, {
			name : "themes",
			location : window.path + "themes"
		}];

		resources.push(window.apiUrl + 'init.js');

		jimuConfig = {
			loadingId : 'main-loading',
			mainPageId : 'main-page',
			layoutId : 'jimu-layout-manager',
			mapId : 'map'
		};

		loadResources(resources, null, function(url, loaded) {
			if ( typeof loadingCallback === 'function') {
				loadingCallback(url, loaded, resources.length);
			}
		}, function() {
			continueLoad();
			function continueLoad() {
				if ( typeof require === 'undefined') {
					if (window.console) {
						console.log('等待arcgis api for javascript类库加载完成！');
					}
					setTimeout(continueLoad, 100);
					return;
				}
				window.appPath = window.path;
				/* arcgis api for javascript类库加载完成 */
				require(['jimu/main'], function(jimuMain) {
					loadingCallback('jimu', resources.length + 1, resources.length);
					jimuMain.initApp();
				});
			}
		});
	}
})();
