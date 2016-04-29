"getComputedStyle" in window || function () {
    function c(a, b, g, e) {
        var h = b[g];
        b = parseFloat(h);
        h = h.split(/\d/)[0];
        e = null !== e ? e : /%|em/.test(h) && a.parentElement ? c(a.parentElement, a.parentElement.currentStyle, "fontSize", null) : 16;
        a = "fontSize" == g ? e : /width/i.test(g) ? a.clientWidth : a.clientHeight;
        return "em" == h ? b * e : "in" == h ? 96 * b : "pt" == h ? 96 * b / 72 : "%" == h ? b / 100 * a : b;
    }
    function a(a, c) {
        var b = "border" == c ? "Width" : "", e = c + "Top" + b, h = c + "Right" + b, l = c + "Bottom" + b, b = c + "Left" + b;
        a[c] = (a[e] == a[h] == a[l] == a[b] ? [a[e]] : a[e] == a[l] && a[b] == a[h] ? [a[e], a[h]] : a[b] == a[h] ? [a[e], a[h], a[l]] : [a[e], a[h], a[l], a[b]]).join(" ");
    }
    function b(b) {
        var d, g = b.currentStyle, e = c(b, g, "fontSize", null);
        for (d in g) {
            /width|height|margin.|padding.|border.+W/.test(d) && "auto" !== this[d] ? this[d] = c(b, g, d, e) + "px" : "styleFloat" === d ? this["float"] = g[d] : this[d] = g[d];
        }
        a(this, "margin");
        a(this, "padding");
        a(this, "border");
        this.fontSize = e + "px";
        return this;
    }
    b.prototype = {};
    window.getComputedStyle = function (a) {
        return new b(a);
    };
}();