define(['dojo/_base/declare', 'dijit/_WidgetBase', 'dojo/dom-style', 'dojo/dom-construct'],
    function (declare, _WidgetBase, domStyle, domConstruct) {
    return declare(_WidgetBase, {
        'baseClass': 'loading-indicator',
        declaredClass: 'widget.dijit.LoadingIndicator',
        hidden: false,

        postCreate: function () {
            this.inherited(arguments);
            this.hidden = this.hidden === true;
            if (this.hidden) {
                domStyle.set(this.domNode, {
                    display: 'none'
                });
            }
            domStyle.set(this.domNode, {
                width: '100%',
                height: '100%'
            });
            var str = '<div class="widget-loading"><img src="../images/widget-loading.gif"><p>正在加载中，请稍后...</p></div>';
            domConstruct.place(str, this.domNode);
        },

        show: function () {
            if (!this.domNode) {
                return;
            }
            domStyle.set(this.domNode, 'display', 'block');
        },

        hide: function () {
            if (!this.domNode) {
                return;
            }
            domStyle.set(this.domNode, 'display', 'none');
        }
    });
});