/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dojo/_base/lang', 'dojo/topic', 'dojo/parser', 'dijit/_WidgetBase', 'dijit/_TemplatedMixin', 'dojo/on', 'dojo/dom', 'dojo/dom-construct',
'dojo/dom-style', 'esri/dijit/Measurement', 'esri/units', 'dojo/text!./template/MeasureDialog.html'],
function (declare, lang, topic, parser, _WidgetBase, _TemplatedMixin, on, dom, domConstruct, domStyle, Measurement, Units, template) {
    var instance = null, clazz;
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        _open: false,
        pNode: null,
        map: null,
        mixin: false,
        measurement: null,
        postCreate: function () {
            this.titleNode.innerHTML = "地图测量";
            topic.subscribe("topic/PrintTaskDialog", lang.hitch(this, this.onCancel));
        },
        startup: function () {
            this.inherited(arguments);
            if (!this.mixin) {
                domConstruct.place(this.domNode, this.pNode);
                this.measurement = new Measurement({
                    map: this.map,
                    defaultAreaUnit: Units.SQUARE_MILES,
                    defaultLengthUnit: Units.KILOMETERS
                }, this.containerNode);
                this.measurement.startup();
                this.mixin = true;
            } else {
                this.onOpen();
            }
            topic.publish("topic/MeasureDialogShow");
            this.pauseEvent();
        },

        pauseEvent: function () {
            if (JZ.mapclick) {
                JZ.mapclick.pause();
            }
        },

        onOpen: function () {
            if (domStyle.get(this.domNode, "display") !== "none") {
                return;
            } else {
                domStyle.set(this.domNode, "display", "inline");
            }
        },
        onCancel: function () {
            domStyle.set(this.domNode, "display", "none");
            this.measurement.clearResult();
            this.measurement.setTool("area", false);
            this.measurement.setTool("distance", false);
            this.measurement.setTool("location", false);

            if (JZ.mapclick) {
                JZ.mapclick.resume();
            }
        }
    });

    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz({
                map: JZ.mc,
                pNode: "mapDiv"
            });
        }
        return instance;
    };
    return clazz;

});
