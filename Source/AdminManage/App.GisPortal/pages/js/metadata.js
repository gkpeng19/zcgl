require(['dojo/request/xhr',
         'dgrid/OnDemandGrid',
         'dgrid/Grid',
         'dgrid/Keyboard',
         'dojo/dom',
         'dojo/on',
         'dojo/DeferredList',
         'dojo/dom-construct',
         'dojo/data/ItemFileWriteStore',
         'dijit/tree/ForestStoreModel',
         'dijit/Tree',
         'dgrid/test/data/createHierarchicalStore',
         'dgrid/test/data/hierarchicalCountryData',
         'dgrid/Tree',
         'dgrid/Selection',
         'dgrid/editor',
         'dijit/form/NumberSpinner',
         'dojo/_base/declare',
         'widget/utils/MetaUtils',
         'dojo/domReady!'],
function (xhr, DemandGrid, Grid, Keyboard, dom, on, DeferredList,
domConstruct, ItemFileWriteStore, ForestStoreModel, Tree,
createHierarchicalStore, hierarchicalCountryData, GridTree, Selection,
Editor, NumberSpinner, declare, MetaUtils) {
    //返回到登录界面
    function showLogin() {
        window.location.href = "../Login.aspx?ReturnUrl=" + window.location.href;
    }
    var deferred = xhr.get("../webservice/WebServiceMap.asmx/pGetMenuDetailLayer_Metadata", {
        handleAs: "json",
        timeout: 5000,
    });

    deferred.then(function (suc) {
        constructTree(suc);
    }, function(err){
        console.log("调用获取当前专题数据的WebService出现错误，请检查！" + err);
        showLogin();
    });

    function getTooltipResult(item) {
        if (!item.root && item["cnName"]) {
            return item["cnName"][0];
        }
    }

    function getLabelResult(item) {
        if (!item.root) {
            if (item && item["children"]) {
                return item["cnName"][0] + "（" + item["children"].length + "个）";
            } else {
                return item["cnName"][0];
            }
        }
    }

    function getLabelCls(item) {
        if (!item.root) {
            if (item && item["children"]) {
                return "coll-label";
            } else {
                return "layer-label";
            }
        }
    }

    function getIconCls(item, opened) {
        if (!item.root) {
            if (item && item["children"]) {
                return opened ? "coll-clsed" : "coll-cls";
            } else {
                return "polygon-clsed";
            }
        }
    }

    function constructTree(suc) {
        var data_tree = {
            identifier: 'id',
            label: 'cnName',
            items: suc
        };
        var store_tree = new ItemFileWriteStore({
            data: data_tree
        });
        var model_tree = new ForestStoreModel({
            store: store_tree,
            query: {
                type: 'root'
            }
        });
        var tree = new Tree({
            model: model_tree,
            persist: true,
            showRoot: false,
            openOnClick: true,
            autoExpand: true,
            getIconClass: getIconCls,
            getLabelClass: getLabelCls,
            getLabel: getLabelResult,
            getTooltip: getTooltipResult
        });
        tree.placeAt(dom.byId("treeTitle"));
        tree.startup();
        on(tree, "click", showMetadata);
        MetaUtils.getInstance().startup();
    }

    function showMetadata(item, node, evt) {
        var defer1 = xhr.get("../webservice/WebServiceMap.asmx/GetDescribe_Metadata", {
            handleAs: "json",
            timeout: 5000, 
            query: { 'layerid': item.id[0] }
        });
        var defer2 = xhr.get("../webservice/WebServiceMap.asmx/GetColsBylayer_Metadata", {
            handleAs: "json",
            timeout: 5000,
            query: { 'layerid': item.id[0] }
        });
        var deferred = new DeferredList([defer1, defer2]);
        deferred.then(function (suc) {
            MetaUtils.getInstance().toggle(suc);
        });
    }

});