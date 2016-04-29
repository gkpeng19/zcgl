/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dojox/encoding/digests/MD5'], function (declare, MD5) {
    var instance = null, clazz;
    clazz = declare(null, {
        username: "",
        password: "",
        token: "",
        status: "",
        constructor: function () {

        }
    });
    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz();
        }
        return instance;
    };
    return clazz;
});
