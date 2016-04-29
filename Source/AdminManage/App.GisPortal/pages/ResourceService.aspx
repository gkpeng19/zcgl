<%@ page language="C#" masterpagefile="~/PageHeader.Master" autoeventwireup="true" codebehind="ResourceService.aspx.cs" inherits="GIS.Portal.service.ResourceService" %>

<asp:Content ContentPlaceHolderID="head" runat="Server">
    <title>服务资源</title>
</asp:Content>
<asp:Content ContentPlaceHolderID="content" runat="Server">
    <!-- 内容栏 -->
    <div class="content clearfix">
        <div class="left-nav clearfix">
            <div class="left-dir">
                <ul>
                    <li><a href="#intro">1. 地图服务</a></li>
                    <li><a href="#apiuser">2. 影像服务</a></li>
                    <li><a href="#getwms">3. WMS 服务</a></li>
                    <li><a href="#getapi">4. 查询服务</a></li>
                </ul>
            </div>
        </div>
        <div class="right-nav clearfix">
            <a class="right-title" name="intro">地图服务</a>
            <table class="table table-bordered table-hover table-striped">
                <tr>
                    <th>坐标系</th>
                    <th>服务类型</th>
                    <th>覆盖范围</th>
                    <th>显示级别</th>
                    <th>服务调用地址</th>
                </tr>
                <tr>
                    <td>Beijing_Local</td>
                    <td>底图</td>
                    <td>北京市</td>
                    <td>0-9级</td>
                    <td>
                        <a title="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" href="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" style="color:red">
                            http://10.246.63.6:6080/arcgis/rest/services
                        </a>
                    </td>
                </tr>
                <tr>
                    <td>Beijing_Local</td>
                    <td>底图</td>
                    <td>北京市</td>
                    <td>0-9级</td>
                    <td>
                        <a title="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" href="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" style="color:red">
                            http://10.246.63.6:6080/arcgis/rest/services
                        </a>
                    </td>
                </tr>
                <tr>
                    <td>Beijing_Local</td>
                    <td>底图</td>
                    <td>北京市</td>
                    <td>0-9级</td>
                    <td>
                        <a title="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" href="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" style="color:red">
                            http://10.246.63.6:6080/arcgis/rest/services
                        </a>
                    </td>
                </tr>
                <tr>
                    <td>Beijing_Local</td>
                    <td>底图</td>
                    <td>北京市</td>
                    <td>0-9级</td>
                    <td>
                        <a title="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" href="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" style="color:red">
                            http://10.246.63.6:6080/arcgis/rest/services
                        </a>
                    </td>
                </tr>
            </table>
            <a class="right-title" name="apiuser">影像服务</a>
            <table class="table table-bordered table-hover table-striped">
                <tr>
                    <th>坐标系</th>
                    <th>服务类型</th>
                    <th>覆盖范围</th>
                    <th>显示级别</th>
                    <th>服务调用地址</th>
                </tr>
                <tr>
                    <td>Beijing_Local</td>
                    <td>底图</td>
                    <td>北京市</td>
                    <td>0-9级</td>
                    <td>
                        <a title="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" href="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" style="color:red">
                            http://10.246.63.6:6080/arcgis/rest/services
                        </a>
                    </td>
                </tr>
                <tr>
                    <td>Beijing_Local</td>
                    <td>底图</td>
                    <td>北京市</td>
                    <td>0-9级</td>
                    <td>
                        <a title="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" href="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" style="color:red">
                            http://10.246.63.6:6080/arcgis/rest/services
                        </a>
                    </td>
                </tr>
                <tr>
                    <td>Beijing_Local</td>
                    <td>底图</td>
                    <td>北京市</td>
                    <td>0-9级</td>
                    <td>
                        <a title="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" href="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" style="color:red">
                            http://10.246.63.6:6080/arcgis/rest/services
                        </a>
                    </td>
                </tr>
                <tr>
                    <td>Beijing_Local</td>
                    <td>底图</td>
                    <td>北京市</td>
                    <td>0-9级</td>
                    <td>
                        <a title="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" href="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" style="color:red">
                            http://10.246.63.6:6080/arcgis/rest/services
                        </a>
                    </td>
                </tr>
            </table>
            <a class="right-title" name="getwms">WMS服务</a>
            <table class="table table-bordered table-hover table-striped">
                <tr>
                    <th>坐标系</th>
                    <th>服务类型</th>
                    <th>覆盖范围</th>
                    <th>显示级别</th>
                    <th>服务调用地址</th>
                </tr>
                <tr>
                    <td>Beijing_Local</td>
                    <td>底图</td>
                    <td>北京市</td>
                    <td>0-9级</td>
                    <td>
                        <a title="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" href="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" style="color:red">
                            http://10.246.63.6:6080/arcgis/rest/services
                        </a>
                    </td>
                </tr>
                <tr>
                    <td>Beijing_Local</td>
                    <td>底图</td>
                    <td>北京市</td>
                    <td>0-9级</td>
                    <td>
                        <a title="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" href="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" style="color:red">
                            http://10.246.63.6:6080/arcgis/rest/services
                        </a>
                    </td>
                </tr>
                <tr>
                    <td>Beijing_Local</td>
                    <td>底图</td>
                    <td>北京市</td>
                    <td>0-9级</td>
                    <td>
                        <a title="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" href="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" style="color:red">
                            http://10.246.63.6:6080/arcgis/rest/services
                        </a>
                    </td>
                </tr>
                <tr>
                    <td>Beijing_Local</td>
                    <td>底图</td>
                    <td>北京市</td>
                    <td>0-9级</td>
                    <td>
                        <a title="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" href="http://10.246.63.6:6080/arcgis/rest/services/LXBJ/MapServer" style="color:red">
                            http://10.246.63.6:6080/arcgis/rest/services
                        </a>
                    </td>
                </tr>
            </table>
            <a class="right-title" name="getapi">地址查询服务</a>
            <p>
                地址服务
            </p>
        </div>
    </div>
    <%--<style type="text/css">
        table
        {
            margin:10px 20px 10px 20px;
            border-right:1px solid #E8EFF7;
            border-bottom:1px solid #E8EFF7;
        }
        .right-nav table tr td:first-child
        {
            border-right:none;
        }
        table span
        {
            font-size:14px;
            font-weight:bold;
        }
        .right-nav table tr td
        {
            height:35px;
            line-height:35px;
            border-top:1px solid #E8EFF7;
            border-left:1px solid #E8EFF7;
        }
    </style>--%>
</asp:Content>
