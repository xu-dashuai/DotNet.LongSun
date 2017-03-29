var pharos = pharos || {};
(function (para) {



    //#region 公共方法

    //正则方法
    para.regex = {
        //用户名应以字母开头，只能含有字母数字下划线，长度大于2位小于10位
        userName: function (userName) {
            return /^[a-zA-Z]{1}[0-9a-zA-Z_]{1,9}$/.test(userName);
        },
        number: function (source) {
            if (/^\d+(\.\d+)?$/.test(source))
                return source;
            return 0;
        }
    }
    //json方法
    para.json = {
        //编辑JSON对象 json：对象 property：属性 value：属性值 
        //如果没有value 则删除属性
        //如果有属性而又要加入 则加入失败 但不会返回错误
        edit: function (json, property, value,isjoin) {
            //如果value被忽略
            if (typeof value == 'undefined') {
                delete json[property];
            }
            //如果不含有属性
            if (!(property in json)) {
                json[property] = value;
                return;
            }
            else {
                if (typeof isjoin == 'undefined') {
                    delete json[property];
                    json[property] = value;
                    return;
                }
                else {
                    json[property] = (json[property] + ',' + value);
                    return;
                }
            }
        },
        //将json转化成string格式
        tostring: function (O) {
            var S = [];
            var J = "";
            if (Object.prototype.toString.apply(O) === '[object Array]') {
                for (var i = 0; i < O.length; i++)
                    S.push(pharos.json.tostring(O[i]));
                J = '[' + S.join(',') + ']';
            }
            else if (Object.prototype.toString.apply(O) === '[object Date]') {
                J = "new Date(" + O.getTime() + ")";
            }
            else if (Object.prototype.toString.apply(O) === '[object RegExp]' || Object.prototype.toString.apply(O) === '[object Function]') {
                J = O.toString();
            }
            else if (Object.prototype.toString.apply(O) === '[object Object]') {
                for (var i in O) {
                    O[i] = typeof (O[i]) == 'string' ? '"' + O[i] + '"' : (typeof (O[i]) === 'object' ? pharos.json.tostring(O[i]) : O[i]);
                    S.push('"' + i.toString() + '":' + (O[i]==null?'null':O[i].toString()));
                }
                J = '{' + S.join(',') + '}';
            }
            return J;
        },
        //将json转化成表单数据传回后台
        //type : post get
        submit: function (json, type, url) {
            $form = $('<form></form>').attr('method', type).attr('action', url);
            for (var i in json) {
                $form.append($("<input name='" + i.toString() + "'/>").val((json[i].toString())));
            }
            $("body").append($form);
            $form.hide();
            $form.submit();
        },
        //如果数据里面为json,返回json.targetProperty==targetValue的json.returnProperty,否则返回nullReturnValue
        getArrayValue: function (array, targetProperty,targetValue,returnProperty,nullReturnValue) {
            for (var i = 0; i < array.length; i++) {
                if (targetProperty in array[i] && array[i][targetProperty] == targetValue) {
                    if (returnProperty in array[i])
                        return array[i][returnProperty];
                }
            }
            return nullReturnValue;
        },
        //将表单转换成json
        formtojson: function (e) {
            var array = e.serializeArray();
            var json = {};
            $.each(array, function (index, value) {
                pharos.json.edit(json, value.name, value.value, true);
            });
            return json;
        }
    }
    //window方法
    para.window = {
        //验证当前页面是否为最上层 如果不是则设置为最上层
        toTop: function () {
            if (window != window.top.window) {
                window.top.location = window.location;
            }
        },
        //取顶层window，如果没有则返回当前window
        topWindow: function () {
            return window.top.window;
        },
        //取出url中的参数
        getPamaras: function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        }
    },
    //easyui拓展 使用前必须引入 jquery.easyui.min.js
    para.easyui =  {
        //窗口 参数与easyui一致 id,easyui参数
        window : {
            //顶层窗口
            topOpen: function (id, params) {
                $top = window.top.$;
                $win = $top("#" + id);
                if ($win.length > 0) {
                    $win.remove();
                }
                $win = $top("<div id='" + id + "'></div>");
                $top("body").append($win);
                $top("body").data(id+'_jquery',$);
                $win.window(params);
            },
            topClose: function (id) {
                $top = window.top.$;
                $win = $top("#" + id);
                if ($win.length > 0) {
                    $win.window("close");
                }
                else {
                    console.log("找不到窗口");
                }
            },
            curJquery: function (id) {
                return window.top.$("body").data(id + '_jquery');
            }
        },
        //对话框窗口 参数与easyui一致 id,easyui参数
        dialog: {
            //顶层窗口
            topOpen: function (id, params) {
                $top = window.top.$;
                $win = $top("#" + id);
                if ($win.length > 0) {
                    $win.remove();
                }
                $win = $top("<div id='" + id + "'></div>");
                $top("body").append($win);
                $top("body").data(id + '_jquery', $);
                $win.dialog(params);
            },
            topClose: function (id) {
                $top = window.top.$;
                $win = $top("#" + id);
                if ($win.length > 0) {
                    $win.dialog("close");
                }
                else {
                    console.log("找不到窗口");
                }
            },
            curJquery: function (id) {
                return window.top.$("body").data(id + '_jquery');
            }
        },
        //提示
        alert: function (type,msg) {
            $.messager.alert(type, msg);
        },
        //确认 
        //type 类型
        //msg 确认信息
        //callback 确认后事件
        confirm: function (type,msg,callback) {
            $.messager.confirm(type, msg, function (r) { if (r) { callback();} });
        }
    },
    //上传方法
    para.upload = {
        //ajaxfileupload拓展 使用前必须引入 ajaxfileupload.js
        ajaxfileupload: {
            //id input的ID (name与id同名)
            //dourl 处理地址 (处理完后只回传json(""))
            //msgurl 数据回传地址 (由于浏览器不同的兼容)
            //callback 从msgurl收到数据后的处理事件 参数为data
            //errorback 回传出错是处理事件
            upload: function (id, dourl, msgurl, callback, errorback) {
                try {
                    $.ajaxFileUpload({
                        url: dourl,
                        type: 'post',
                        secureuri: false,
                        fileElementId: id,
                        success: function () {
                            $.ajax({
                                url: msgurl,
                                type: 'post',
                                dataType: 'json',
                                aysnc: false,
                                success: function (data) {
                                    callback(data);
                                }
                            });
                        },
                        error: function (e) {
                            if (typeof errorback == 'undefined') {
                            }
                            else {
                                errorback(e);
                            }
                        }
                    });
                }
                catch (e) {
                    console.log("请先引用ajaxfileupload.js");
                }
            }
        }
    }


    //#endregion


})(pharos);

jQuery.cookie = function (name, value, options) {
    if (typeof value != 'undefined') { // name and value given, set cookie
        options = options || {};
        if (value === null) {
            value = '';
            options.expires = -1;
        }
        var expires = '';
        if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
            var date;
            if (typeof options.expires == 'number') {
                date = new Date();
                date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
            } else {
                date = options.expires;
            }
            expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
        }
        var path = options.path ? '; path=' + options.path : '';
        var domain = options.domain ? '; domain=' + options.domain : '';
        var secure = options.secure ? '; secure' : '';
        document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
    } else { // only name given, get cookie
        var cookieValue = null;
        if (document.cookie && document.cookie != '') {
            var cookies = document.cookie.split(';');
            for (var i = 0; i < cookies.length; i++) {
                var cookie = jQuery.trim(cookies[i]);
                // Does this cookie string begin with the name we want?
                if (cookie.substring(0, name.length + 1) == (name + '=')) {
                    cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                    break;
                }
            }
        }
        return cookieValue;
    }
};

/*==========javascript扩展==========*/
// 对Date的扩展，将 Date 转化为指定格式的String   
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符，   
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)   
// 例子：   
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423   
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18  
// 经过Json序列化的时间为：2014-01-11T20:05:49，直接getDate()将得到加上时区偏移量的值，故用UTC时间
Date.prototype.format = function (fmt) { //author: meizz   
    var o = {
        "M+": this.getUTCMonth() + 1,                 //月份   
        "d+": this.getUTCDate(),                    //日   
        "h+": this.getUTCHours(),               //小时(北京时间为UTC+8)   
        "m+": this.getUTCMinutes(),                 //分   
        "s+": this.getUTCSeconds(),                 //秒   
        "q+": Math.floor((this.getUTCMonth() + 3) / 3), //季度   
        "S": this.getUTCMilliseconds()             //毫秒   
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getUTCFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}