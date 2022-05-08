// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


// Write your JavaScript code.
var cookies = document.cookie;

function checkCookie() {
    var username = getCookie("username");
    var url_string = window.location.href;
    var url = new URL(url_string);
    var link = url.pathname.split("/");
    var action = link[2];
    if (username == "" && action != "Login") {
        location.replace("/User/Login");
    }
}

function setCookie() {
    var d = new Date();
    d.setTime(d.getTime() + (1 * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = "username" + "=" + "a" + ";" + expires + ";path=/";
    location.replace("/Home/Index");
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function clearCookies() {
    document.cookie = "username=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    location.href("/User/Login");
}

function printPage() {
    window.print();
}

$(function () {
    checkCookie();
})