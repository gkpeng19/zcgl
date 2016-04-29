define(['./defined'], function (defined) {
    "use strict";
    var DeveloperError = function (message) {
        /**
         * 错误名称
         */
        this.name = 'DeveloperError';
        /**
         * 错误描述信息
         */
        this.message = message;
        //IE没有stack属性
        var stack;
        try {
            throw new Error();
        } catch (e) {
            stack = e.stack;
        }
        this.stack = stack;
    };

    DeveloperError.prototype.toString = function () {
        var str = this.name + ': ' + this.message;
        if (defined(this.stack)) {
            str += '\n' + this.stack.toString();
        }
        return str;
    };
    DeveloperError.throwInstantiationError = function () {
        throw new DeveloperError('This function defines an interface and should not be called directly.');
    };
    return DeveloperError;
});
