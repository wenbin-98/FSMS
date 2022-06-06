// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


// Write your JavaScript code.
function printPage() {
    window.print();
}

var toDTO = function (sFormselector) {
    var dto = {};
    var arr = $(sFormselector).serializeArray();
    for (var i = 0; i < arr.length; i++) {
        dto[arr[i].name] = arr[i].value;
    }
    return dto;
}

var formatObjectKey = function (data){
    let newData = {};
    Object.entries(data).forEach(([key, value]) => {
        let editedKey = key.split(".").pop();
        newData[editedKey] = value;
        //console.log(key, value); // key ,value
    });

    return newData;
}

var fillDTOtoForm = function (dto) {
    for (key in dto) {
        if (dto.hasOwnProperty(key))
            $('input[name="' + key + '"]').val(dto[key]);
    }
}

String.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }

    return s;
}

Object.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var arg = arguments[i + 1];
        for (prop in arg) {
            var reg = new RegExp("\\[" + prop + "\\]", "gm");
            s = s.replace(reg, arg[prop]);
        }
    }
    return s;
}