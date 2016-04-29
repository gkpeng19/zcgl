define(function () {
    "use strict";
    /**
     * 如果对象定义过则返回true，如果对象未定义则返回false
     */
    var defined = function (value) {
        return value !== undefined;
    };

    return defined;
});
