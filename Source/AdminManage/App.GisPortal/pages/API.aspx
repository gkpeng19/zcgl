<%@ page language="C#" masterpagefile="~/PageHeader.Master" autoeventwireup="true" codebehind="API.aspx.cs" inherits="GIS.Portal.api.API" %>

<asp:Content ContentPlaceHolderID="head" runat="Server">
    <title>地图API</title>
</asp:Content>
<asp:Content ContentPlaceHolderID="content" runat="Server">
    <!-- 内容栏 -->
    <div class="content clearfix">
        <div class="left-nav clearfix">
            <div class="left-dir">
                <ul>
                    <li><a href="#intro">1. API 介绍</a></li>
                    <li><a href="#apiuser">2. 面向用户</a></li>
                    <li><a href="#getapi">3. 获取 API</a></li>
                    <li><a href="#version">4. 版本说明</a></li>
                    <li><a href="#locate">5. 定位示例</a></li>
                    <li><a href="#edit">6. 编辑示例</a></li>
                    <li><a href="#marker">7. 标注示例</a></li>
                </ul>
            </div>
        </div>
        <div class="right-nav clearfix">
            <a class="right-title" name="intro">API 介绍</a>
            <p>
                本套API是由JavaScript语言编写的应用程序接口，它能够帮助您在网站中制作各种类型、行业的地图应用，还可以使地图功能够以模块化集成在不同类型的系统应用中。
           
            </p>

            <a class="right-title" name="apiuser">面向用户</a>
            <p>
                本套API面向的读者是有一定JavaScript编程经验的读者，此外，读者还应该对地图产品有一定的了解。初级程序员通过1-2天的学习，即可掌握API的使用。
		
            </p>

            <a class="right-title" name="getapi">获取 API</a>
            <p>
                地图API是由JavaScript语言编写的，您在使用之前需要通过标签将API引用到页面中:<br />
            </p>
            <table class="table table-none-border">
                <tr>
                    <td>
                        <code>&lt;</code>
                        <code class="keyword">script</code>
                        <code class="keyword">src</code>
                        <code class="plain">=</code>
                        <code class="colored">"<a href="http://10.246.60.16/esri/init.js">http://10.246.60.16/esri/init.js</a>"
							</code>
                        <code class="keyword">type</code>
                        <code class="plain">=</code>
                        <code class="colored">"text/javascript"</code>
                        <code class="plain">&gt;&lt;/</code>
                        <code class="keyword">script</code>
                        <code class="plain">&gt;</code>
                    </td>
                </tr>
                <tr>
                    <td>
                        <code>&lt;</code>
                        <code class="keyword">script</code>
                        <code class="keyword">src</code>
                        <code class="plain">=</code>
                        <code class="colored">"<a href="http://10.246.60.16/System/javascript/index1.js">http://10.246.60.16/System/javascript/index1.js</a>"
							</code>
                        <code class="keyword">type</code>
                        <code class="plain">=</code>
                        <code class="colored">"text/javascript"</code>
                        <code class="plain">&gt;&lt;/</code>
                        <code class="keyword">script</code>
                        <code class="plain">&gt;</code>
                    </td>
                </tr>
                <tr>
                    <td>
                        <code>&lt;</code>
                        <code class="keyword">link</code>
                        <code class="keyword">rel</code>
                        <code class="plain">=</code>
                        <code class="colored">"stylesheet"</code>
                        <code class="keyword">href</code>
                        <code class="plain">=</code>
                        <code class="colored">"<a href="http://10.246.60.16/System/css/index.css">http://10.246.60.16/System/css/index.css</a>"
							</code>
                        <code class="plain">/&gt;</code>
                    </td>
                </tr>
            </table>

            <a class="right-title" name="version">版本说明</a>
            <p>
                此API为地图展示版本，主要完成地图的展示和操作功能，如地图定位、放大和缩小等！
		
            </p>
            <p>
                兼容性:
			
                <span>浏览器：IE 8.0+、Firefox 3.6+、Opera 9.0+、Safari 3.0+、Chrome</span>
                <span>操作系统：Windows、Mac、Linux</span>
            </p>

            <a class="right-title" name="locate">定位示例</a>
            <p>
                本套API面向的读者是有一定JavaScript编程经验的读者，此外，读者还应该对地图产品有一定的了解。初级程序员通过1-2天的学习，即可掌握API的使用。
			
                <a id="checkLocation" style="color: #FF0000; cursor: pointer">点击查看</a>
            </p>

            <a class="right-title" name="edit">编辑示例</a>
            <p>
                本套API面向的读者是有一定JavaScript编程经验的读者，此外，读者还应该对地图产品有一定的了解。初级程序员通过1-2天的学习，即可掌握API的使用。
			   
                <a id="editMap" style="color: #FF0000; cursor: pointer">点击查看</a>
            </p>

            <a class="right-title" name="marker">标注示例</a>
            <p>
                本套API面向的读者是有一定JavaScript编程经验的读者，此外，读者还应该对地图产品有一定的了解。初级程序员通过1-2天的学习，即可掌握API的使用。
           
            </p>
        </div>
    </div>
    <div id="locateWindowContent" style="top: -1000px; z-index: 9; width: 800px; height: auto!important; min-height: 500px; max-height: 700px; border: 1px solid #eee; padding: 0 10px 0px 10px; position: fixed; left: 50%; margin-left: -400px; margin-top: -250px; background-color: #FFF; border-radius: 5px; -moz-border-radius: 5px; -webkit-border-radius: 5px; -ms-border-radius: 5px;">
        <div style="margin: 10px 0px 10px 0px; padding: 10px; padding-top: 0px; border-bottom: 1px solid #EEE">
            <span>地图定位示例</span>
            <button id="btnCloseLocateWin" class="close">x</button>
        </div>
    </div>
    <script type="text/javascript">
        var dojoConfig = {
            async: 1,
            isDebug: true,
            parseOnLoad: true,
            tlmSiblingOfDojo: false,
            paths: {
                widget: location.pathname.replace(/\/[^/]+$/, "") + "../../widget"
            }
        };
    </script>
    <script type="text/javascript" src="../esri/init.js"></script>
    <script type="text/javascript" src="../control/jzmap.js"></script>
    <script type="text/javascript">
        (function () {
            function addEvent(ele, type, fn) {
                if (window.addEventListener) {
                    ele.addEventListener(type, fn, false);
                } else if (window.attachEvent) {
                    ele.attachEvent("on" + type, fn);
                }
            }

            function removeEvent(ele, type, fn) {
                if (window.removeEventListener) {
                    ele.removeEventListener(type, fn, false);
                } else if (window.detachEvent) {
                    ele.detachEvent("on" + type, fn);
                }
            }

            var aLink = document.getElementById("checkLocation");
            var editLink = document.getElementById("editMap");
            var btnClose = document.getElementById("btnCloseLocateWin");

            addEvent(aLink, "click", showMap);
            addEvent(btnClose, "click", closeWindow);
            addEvent(editLink, "click", editMap);

            function editMap() {
                if (JZ.MarkGisMap) {
                    document.getElementById("locateWindowContent").style.top = "50%";

                    //JZ.domConstruct.place("<div class='modal-backdrop in'></div>", JZ.win.body(), "last");

                    JZ.MarkGisMap(null, { TYPE: 1, SEARCHFIELD: 'OBJCODE,OBJYEAR', SEARCHVALUE: '10001004,2014' });
                }
            }

            function showMap() {
                if (JZ.MarkGisMap) {
                    document.getElementById("locateWindowContent").style.top = "50%";
                    //document.write("<div class='modal-backdrop in'></div>");
                    //JZ.LocateGisMap("locateWindowContent", null, {TYPE:2,  SEARCHFIELD: 'OBJCODE,OBJYEAR', SEARCHVALUE: '10001004,2010' });
                    JZ.MarkGisMap("locateWindowContent", null, { TYPE: 2 });
                }
            }

            function closeWindow(e) {
                if (e && e.preventDefault) {
                    e.preventDefault();
                } else {
                    window.event.returnValue = false;
                }

                document.getElementById("locateWindowContent").style.top = "-1000px";
                //JZ.domConstruct.destroy(JZ.query(".modal-backdrop")[0]);
            }
        })();
    </script>

</asp:Content>

