/**
 * @author wangyafei
 */


var locateA = document.getElementById("checkLocation");
locateA.addEventListener("click", showMap);

function showMap(){
	if(HLJZ && HLJZ.LDMapSingle){
		HLJZ.LDMapSingle.show();
		document.getElementById("content").style.display = "block";
	}
}
