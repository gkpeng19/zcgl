<%@ page language="C#" masterpagefile="~/PageHeader.Master" autoeventwireup="true" codebehind="Metadata.aspx.cs" inherits="GIS.Portal.metadata.Metadata" %>

<asp:Content ContentPlaceHolderID="head" runat="Server">
    <title>元数据管理</title>
</asp:Content>
<asp:Content ContentPlaceHolderID="content" runat="Server">
    <!-- 内容栏 -->
    <div class="content clearfix">
        <div class="left-nav clearfix">
            <div class="left-dir" style="padding:20px 5px;">
                <div id='treeTitle' class="tree-class">
                </div>
            </div>
        </div>
        <div id="metadataTable" class="right-nav clearfix">
        </div>
    </div>
    <script type="text/javascript" src="js/config.js"></script>
    <script type="text/javascript" src="../esri/init.js"></script>
    <script type="text/javascript" src="js/metadata.js"></script>
</asp:Content>
