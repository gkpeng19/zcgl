/**
 * @author Administrator
 */
define(['dojo/_base/declare',
    'dojo/dom-class',
    'dojo/dom-construct',
    'dijit/_WidgetBase',
    'dijit/_TemplatedMixin',
	'dgrid/OnDemandGrid',
    'dgrid/Selection',
    'dgrid/Keyboard',
    'dgrid/Tree',
    'dgrid/test/data/createHierarchicalStore',
    'dojo/text!./MetaUtils.html'],
function (declare, domClass, domConstruct, _WidgetBase, _TemplatedMixin,
    OnDemandGrid, Selection, Keyboard, GridTree, createHierarchicalStore,
    template) {
    var instance = null, clazz;
    var data = {
        identifier: 'id',
        items: [
        { id: 1, name: "基本信息" },
        { id: 2, name: "图层编码", propvalue: "", des: "", parent: 1 },
        { id: 3, name: "图层名称", propvalue: "", des: "", parent: 1 },
        { id: 4, name: "更新频率", parent: 1, propvalue: "", des: "" },
        { id: 5, name: "资源标识符", parent: 1, propvalue: "", des: "" },
        { id: 6, name: "资源字符集", parent: 1, propvalue: "", des: "" },
        { id: 7, name: "资源类型", parent: 1, propvalue: "", des: "" },
        { id: 8, name: "资源语种", parent: 1, propvalue: "", des: "" },
        { id: 9, name: "系列名称", parent: 1, propvalue: "", des: "" },
        { id: 10, name: "系列标识", parent: 1, propvalue: "", des: "" },
        { id: 11, name: "数据说明", parent: 1, propvalue: "", des: "" },
        { id: 12, name: "数据志说明", parent: 1, propvalue: "", des: "" },
        { id: 13, name: "分类标准", parent: 1, propvalue: "", des: "" },
        { id: 14, name: "数据类目名称", parent: 1, propvalue: "", des: "" },
        { id: 15, name: "数据类目编码", parent: 1, propvalue: "", des: "" },
        { id: 16, name: "数据要素类型", parent: 1, propvalue: "", des: "" },
        { id: 17, name: "图层要素记录数", parent: 1, propvalue: "", des: "" },
        { id: 18, name: "关键字", parent: 1, propvalue: "", des: "" },
        { id: 19, name: "属性结构", parent: 1, propvalue: "", des: "" },
        { id: 20, name: "数据词典", parent: 1, propvalue: "", des: "" },
        { id: 21, name: "数据文件名称", parent: 1, propvalue: "", des: "" },
        { id: 22, name: "数据表示类型", parent: 1, propvalue: "", des: "" },
        { id: 23, name: "数据文件格式", parent: 1, propvalue: "", des: "" },
        { id: 24, name: "数据格式版本", parent: 1, propvalue: "", des: "" },
        { id: 25, name: "数据存储介质", parent: 1, propvalue: "", des: "" },
        { id: 26, name: "数据文件大小", parent: 1, propvalue: "", des: "" },
        { id: 27, name: "数据名称", parent: 1, propvalue: "", des: "" },
        { id: 28, name: "生产时间", parent: 1, propvalue: "", des: "" },
        { id: 29, name: "上次更新时间", parent: 1, propvalue: "", des: "" },
        { id: 30, name: "标识信息" },
        { id: 31, name: "元数据文件名", parent: 30, propvalue: "", des: "" },
        { id: 32, name: "数据集英文名称", parent: 30, propvalue: "", des: "" },
        { id: 33, name: "版本", parent: 30, propvalue: "", des: "" },
        { id: 34, name: "元数据参考信息" },
        { id: 35, name: "基础资料", parent: 34, propvalue: "", des: "" },
        { id: 36, name: "参考资料", parent: 34, propvalue: "", des: "" },
        { id: 37, name: "基础资料起始时间", parent: 34, propvalue: "", des: "" },
        { id: 38, name: "基础资料截至时间", parent: 34, propvalue: "", des: "" },
        { id: 39, name: "生产原因及目的", parent: 34, propvalue: "", des: "" },
        { id: 40, name: "元数据的模型性质", parent: 34, propvalue: "", des: "" },
        { id: 41, name: "时间信息" },
        { id: 42, name: "现实性起始时间", parent: 41, propvalue: "", des: "" },
        { id: 43, name: "现实性结束时间", parent: 41, propvalue: "", des: "" },
        { id: 44, name: "空间信息" },
        { id: 45, name: "比例尺\分辨率", parent: 44, propvalue: "", des: "" },
        { id: 46, name: "坐标系类型", parent: 44, propvalue: "", des: "" },
        { id: 47, name: "坐标单位", parent: 44, propvalue: "", des: "" },
        { id: 48, name: "西南图廓角点横向坐标", parent: 44, propvalue: "", des: "" },
        { id: 49, name: "西南图廓角点纵向坐标", parent: 44, propvalue: "", des: "" },
        { id: 50, name: "东南图廓角点横向坐标", parent: 44, propvalue: "", des: "" },
        { id: 51, name: "东南图廓角点纵向坐标", parent: 44, propvalue: "", des: "" },
        { id: 52, name: "位置精度（米）", parent: 44, propvalue: "", des: "" },
        { id: 53, name: "数据生产单位" },
        { id: 54, name: "数据生产单位", parent: 53, propvalue: "", des: "" },
        { id: 55, name: "数据生产负责人", parent: 53, propvalue: "", des: "" },
        { id: 56, name: "数据生产负责人联系方式", parent: 53, propvalue: "", des: "" },
        { id: 57, name: "数据生产组织单位", parent: 53, propvalue: "", des: "" },
        { id: 58, name: "数据权威来源部门", parent: 53, propvalue: "", des: "" },
        { id: 59, name: "数据验收监理单位", parent: 53, propvalue: "", des: "" },
        { id: 60, name: "数据验收时间", parent: 53, propvalue: "", des: "" },
        { id: 61, name: "验收报告名称", parent: 53, propvalue: "", des: "" },
        { id: 62, name: "数据质量信息" },
        { id: 63, name: "数据用途限制", parent: 62, propvalue: "", des: "" },
        { id: 64, name: "数据安全限制分级", parent: 62, propvalue: "", des: "" },
        { id: 65, name: "共享范围", parent: 62, propvalue: "", des: "" },
        { id: 66, name: "数据质量总体评价", parent: 62, propvalue: "", des: "" },
        { id: 67, name: "数据志说明", parent: 62, propvalue: "", des: "" },
        { id: 68, name: "发行信息" },
        { id: 69, name: "数据分发介质", parent: 68, propvalue: "", des: "" },
        { id: 70, name: "数据分发格式", parent: 68, propvalue: "", des: "" },
        { id: 71, name: "提供数据方式", parent: 68, propvalue: "", des: "" },
        { id: 72, name: "用户订购方式", parent: 68, propvalue: "", des: "" },
        { id: 73, name: "在线资源链接地址", parent: 68, propvalue: "", des: "" },
        { id: 74, name: "负责单位联系信息" },
        { id: 75, name: "数据所有权单位名称", parent: 74, propvalue: "", des: "" },
        { id: 76, name: "数据所有权单位地址", parent: 74, propvalue: "", des: "" },
        { id: 77, name: "数据所有权单位网址", parent: 74, propvalue: "", des: "" },
        { id: 78, name: "数据所有权单位联系人", parent: 74, propvalue: "", des: "" },
        { id: 79, name: "数据所有权单位联系人职务", parent: 74, propvalue: "", des: "" },
        { id: 80, name: "数据所有权单位联系人电话", parent: 74, propvalue: "", des: "" },
        { id: 81, name: "数据所有权单位联系人电子邮件", parent: 74, propvalue: "", des: "" },
        { id: 82, name: "数据所有权单位联系人传真", parent: 74, propvalue: "", des: "" },
        { id: 83, name: "数据所有权单位邮编", parent: 74, propvalue: "", des: "" },
        { id: 84, name: "在线资源链接地址", parent: 74, propvalue: "", des: "" },
        { id: 85, name: "元数据联系信息" },
        { id: 86, name: "元数据联系单位", parent: 85, propvalue: "", des: "" },
        { id: 87, name: "元数据负责人", parent: 85, propvalue: "", des: "" },
        { id: 88, name: "元数据联系方地址", parent: 85, propvalue: "", des: "" },
        { id: 89, name: "元数据联系方邮政编码", parent: 85, propvalue: "", des: "" },
        { id: 90, name: "元数据联系方电话号码", parent: 85, propvalue: "", des: "" },
        { id: 91, name: "元数据联系方电子邮件", parent: 85, propvalue: "", des: "" },
        { id: 92, name: "元数据参考信息" },
        { id: 93, name: "元数据语言", parent: 92, propvalue: "", des: "" },
        { id: 94, name: "元数据安全限制分级", parent: 92, propvalue: "", des: "" },
        { id: 95, name: "元数据创建日期", parent: 92, propvalue: "", des: "" }
        ]
    };

    var fieldColumns = [
        { label: '字段名称', field: 'COLCODE', className: "propertyCls" },
        { label: '字段值', field: 'COLNAME' },
        { label: '字段类型', field: 'COLTYPE' },
        { label: '字段长度', field: 'ColLen' },
        { label: '小数位数', field: 'ColXSLen' },
        { label: '顺序号', field: 'ODERNUM' },
        { label: '描述', field: 'DES' }
    ];

    var StandardGrid = declare([OnDemandGrid, Selection, Keyboard, GridTree]);
    var CustomGrid = declare([OnDemandGrid, Selection, Keyboard]);

    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        pNode: "",
        layerMetaGrid: null,
        fieldMetaGrid: null,
        startup: function () {
            this.inherited(arguments);
            domConstruct.place(this.domNode, this.pNode, "last");
            this.constructLayerGrid();
            this.constructFieldGrid();
        },

        toggle: function (suc) {
            if (suc.length === 2) {
                this.constructFieldGrid(suc[1][1]);
            } else {
                alert("发生错误，请重新运行程序！");
            }
            var store = createHierarchicalStore({ data: data }, true);
            this.layerMetaGrid.set("collection", store);
        },

        constructFieldGrid: function (suc) {
            suc = suc ? suc : [];
            var fieldStore = (function () { return { identifier: "ID", items: suc }; })();
            if (!this.fieldMetaGrid) {
                this.fieldMetaGrid = new CustomGrid({
                    columns: fieldColumns,
                    loadingMessage: '正在加载数据中，请稍后...',
                    noDataMessage: '没有发现数据'
                }, this.jzTabDetailTable);
            }
            var store = createHierarchicalStore({ data: fieldStore }, true);
            this.fieldMetaGrid.set("collection", store);
            //this.fieldMetaGrid.renderArray(fieldStore.items);
        },

        constructLayerGrid: function () {
            this.layerMetaGrid = new StandardGrid({
                collection: createHierarchicalStore({ data: data }, true),
                columns: [
                    {
                        renderExpando: true,
                        label: "属性名称",
                        field: "name",
                        sortable: true
                    },
                    { label: "属性值", field: "propvalue", sortable: true },
                    { label: "描述信息", field: "des", sortable: true }
                ],
                selectionMode: "none"
            }, this.jzTabBasicTable);
        },

        onBasicShow: function () {
            domClass.add(this.jzBasic, "active");
            domClass.remove(this.jzDetail, "active");
            domClass.add(this.jzTabBasic, "jz-tab-show");
            domClass.remove(this.jzTabDetail, "jz-tab-show");
        },
        onDetailShow: function () {
            domClass.remove(this.jzBasic, "active");
            domClass.add(this.jzDetail, "active");
            domClass.remove(this.jzTabBasic, "jz-tab-show");
            domClass.add(this.jzTabDetail, "jz-tab-show");
        }
    });

    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz({
                pNode: "metadataTable"
            });
        }
        return instance;
    };
    return clazz;
});
