/* 配置arcgis api for javascript路径 */
var apiUrl = "esri/";
/* 一些帮助方法 */
(function(global) {
	/*
	 计算并获取url中的pathname
	 * */
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
	/*
	 获取查询字符串
	 * */
	function getQueryObject() {
		var query = window.location.search;
		if (query.indexOf('?') > -1) {
			query = query.substr(1);
		}
		var pairs = query.split('&');
		var queryObject = {};
		for (var i = 0; i < pairs.length; i++) {
			var splits = decodeURIComponent(pairs[i]).split('=');
			queryObject[splits[0]] = splits[1];
		}
		return queryObject;
	}
	
	var queryObject = getQueryObject();
	path = getPath();
	global.queryObject = queryObject;
})(window);
