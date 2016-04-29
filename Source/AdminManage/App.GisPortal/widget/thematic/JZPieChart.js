define("widget/thematic/JZPieChart",
    [
     "dojo/_base/declare",
     "dojo/_base/lang",
     "dojox/charting/Chart",
     "dojox/charting/plot2d/Pie",
     "dojox/charting/action2d/Highlight",
     "dojox/charting/action2d/MoveSlice",
     "dojox/charting/action2d/Tooltip",
     "dojox/charting/action2d/Shake",
     "dojox/charting/action2d/Magnify",
     "dojox/charting/themes/ThreeD",
     "dojox/charting/themes/PlotKit/blue",
     "widget/thematic/JZChartGraphics"
    ],
    /* 
     "dojox/charting/themes/Julie",
     "dojox/charting/themes/ThreeD",
     "dojox/charting/themes/WatersEdge",
     "dojox/charting/themes/Distinctive",
     "dojox/charting/themes/BlueDusk",
     "dojox/charting/themes/Adobebricks",
     "dojox/charting/themes/Dollar",
     "dojox/charting/themes/Distinctive" */
    function (declare, lang, Chart, Pie, Highlight, MoveSlice, Tooltip, Shake, Magnify, ThreeD, Blue,
        JZChartGraphics) {
        return declare(JZChartGraphics, {
            watchobject: null,

            constructor: function (graphic) {
                lang.mixin(this, {bindGraphic:graphic});
            },

            _draw: function (divContainer, r) {
                /*
                var _chart = new Chart(divContainer);
                var myTheme = Claro;
                myTheme.chart.fill = "transparent";
                myTheme.chart.stroke = "transparent";
                myTheme.plotarea.fill = "transparent";
                _chart.setTheme(myTheme);
                _chart.addPlot("default", {
                    type: Pie,
                    radius: r
                }).
                addSeries(this.getId(), this.getSeries());
                new Tooltip(_chart, "default");
                new MoveSlice(_chart, "default");
                _chart.render();
                */
                var myChart = new Chart(divContainer);
                Blue.chart.fill = "transparent";
                Blue.chart.stroke = "transparent";
                Blue.plotarea.fill = "transparent";
                Blue.plotarea.stroke = null;
                Blue.axis.stroke = null;
                myChart.setTheme(Blue);
                myChart.addPlot("default", {
                    type: Pie,
                    fontColor: '#FFFFFF',
                    radius: r
                });
                myChart.addSeries(this.getId(), this.getSeries());
                new MoveSlice(myChart, "default");
                new Highlight(myChart, "default");
                new Tooltip(myChart, "default");
                myChart.render();





                this.chart = myChart;
            }
    });
});