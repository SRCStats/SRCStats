"use strict";

import { HubConnectionBuilder } from "@microsoft/signalr";
import '../css/user.scss';

import $ from 'jquery';
window.jQuery = $;
window.$ = $;

var connection = new HubConnectionBuilder().withUrl("/userHub").build();
connection.serverTimeoutInMilliseconds = 120000;

connection.on("Error", function (e) {
    // todo: implement
    $("#userName").prop("disabled", false);
    $("#submit").prop("disabled", false);
});

connection.on("AwaitingThread", function () {
    // todo: implement an alert the first time to explain that we're waiting for a thread
    $("#thread-spinner").attr("hidden", false);
});

connection.on("ReceiveProgress", function (val, max, id) {
    if (max === 0)
        max = 1
    if (max > 10000)
        max = 10000
    $("#thread-spinner").attr("hidden", true);
    var prog = $("#prog-bar" + id);
    prog.css('width', ((val / max) * 100) / 7 + "%");
    prog.attr("aria-valuenow", val);
    prog.attr("aria-valuemax", max);
    switch (id) {
        case 1:
            $("#prog-info").text("Fetching user info (" + val + " of " + max + ")");
            break;
        case 2:
            $("#prog-info").text("Fetching user's runs (" + val + " of " + max + ")");
            break;
        case 3:
            $("#prog-info").text("Fetching user's examined runs (" + val + " of " + max + ")");
            break;
        case 4:
            $("#prog-info").text("Fetching user's moderated games (" + val + " of " + max + ")");
            break;
        case 5:
            $("#prog-info").text("Fetching user's ran games (" + val + " of " + max + ")");
            break;
        case 6:
            $("#prog-info").text("Fetching user's rankings (" + val + " of " + max + ")");
            break;
        case 7:
            $("#prog-info").text("Fetching user's pending runs (" + val + " of " + max + ")");
            break;
        default:
            $("#prog-info").text("");
            break;
    }
});

connection.on("Init", function () {
    $("#prog-info").text("Initializing...");
})

connection.on("Finalize", function () {
    $("#prog-info").text("Finalizing...");
})

connection.on("Complete", function (user) {
    window.location.assign("/user/" + user);
})

// todo: make this use a form
document.getElementById("submit").addEventListener("click", function (e) {
    var user = document.getElementById("userName").value;
    connection.start().then(function () {
        $("#userName").prop("disabled", true);
        $("#submit").prop("disabled", true);
        connection.invoke("FetchUser", user, true).catch(function (err) {
            return console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});