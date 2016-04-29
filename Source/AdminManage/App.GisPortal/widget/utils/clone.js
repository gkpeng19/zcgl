define(['./defaultValue'], function (defaultValue) {
    "use strict";
    /**
     * 当deep=true时对对象进行深拷贝。
     * 当deep=false时对对象进行浅拷贝
     */
    var clone = function (object, deep) {
        if (object === null || typeof object !== 'object') {
            return object;
        }
        deep = defaultValue(deep, false);
        var result = new object.constructor();
        for (var propertyName in object) {
            if (object.hasOwnProperty(propertyName)) {
                var value = object[propertyName];
                if (deep) {
                    value = clone(value, deep);
                }
                result[propertyName] = value;
            }
        }
        return result;
    };
    return clone;
});
