define([
        './defined',
        './ErrorLog'
], function (
        defined,
        DeveloperError) {
    "use strict";

    /**
     * 二分查找算法
     */
    var binarySearch = function (array, itemToFind, comparator) {
        if (!defined(array)) {
            throw new DeveloperError('需要传递一个数组');
        }
        if (!defined(itemToFind)) {
            throw new DeveloperError('需要传递一个查找的对象');
        }
        if (!defined(comparator)) {
            throw new DeveloperError('比较函数是必须的！');
        }
        var low = 0;
        var high = array.length - 1;
        var i;
        var comparison;

        while (low <= high) {
            i = ~~((low + high) / 2);
            comparison = comparator(array[i], itemToFind);
            if (comparison < 0) {
                low = i + 1;
                continue;
            }
            if (comparison > 0) {
                high = i - 1;
                continue;
            }
            return i;
        }
        return ~(high + 1);
    };

    /**
     * 当进行二分查找时传递进来的比较函数
     * @example
     * function compareNumbers(a, b) {
     *     return a - b;
     * }
     */

    return binarySearch;
});