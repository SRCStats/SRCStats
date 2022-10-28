"use strict";

import { HubConnectionBuilder } from "@microsoft/signalr";

import '../css/webhooks.scss';

let nodes = new Array();
let users = new Array();
let counter = 0;

$.fn.selectpicker.Constructor.BootstrapVersion = '4';

var connection = new HubConnectionBuilder().withUrl("/webhookHub").build();
connection.serverTimeoutInMilliseconds = 120000;

$("#enable-verification").click(function () {
    if ($(this).is(':checked')) {
        $("#verification-ig").removeAttr("hidden");
    }
    else {
        $("#verification-ig").attr("hidden", true);
    }
})

$("#enable-records").click(function () {
    if ($(this).is(':checked')) {
        $("#records-ig").removeAttr("hidden");
    }
    else {
        $("#records-ig").attr("hidden", true);
    }
})

document.getElementById("rgames-input").addEventListener('keydown', (e) => {
    if (e.keyCode == 13) {
        connection.start().then(() => {
            $("#rgames-input").attr("disabled", true);
            connection.invoke("GetGame", $("#rgames-input").val()).catch(function (err) {
                return console.error(err.toString());
            });
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }
});

document.getElementById("rusers-input").addEventListener('keydown', (e) => {
    if (e.keyCode == 13) {
        connection.start().then(() => {
            $("#rusers-input").attr("disabled", true);
            $("#rusers-list a").each(function (i) {
                if (this.textContent.toLowerCase() === $("#rusers-input").val().toLowerCase()) {
                    $("#rusers-input").removeAttr("disabled");
                    $("#rusers-input").val("");
                    return;
                }
            });
            connection.invoke("GetUser", $("#rusers-input").val()).catch(function (err) {
                return console.error(err.toString());
            })
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }
});

connection.on("ConfirmGame", function (o) {
    const list = $("#category-picker");
    const name = document.createElement('a');
    const opt = document.createElement('optgroup');
    const miscOpt = document.createElement('optgroup');
    name.href = "#";
    name.classList.add("list-group-item", "list-group-item-action");
    name.textContent = o.names.international;
    name.addEventListener('click', () => {
        nodes = nodes.filter(node => !String(node.label).startsWith(name.textContent))
        name.parentNode.removeChild(name);
        resetList(list);
    })
    $("#rgames-list").append(name);
    opt.label = o.names.international + " - Main";
    miscOpt.label = o.names.international + " - Miscellaneous";
    for (var i = 0; i < o.categories.data.length; i++) {
        if (o.categories.data[i].type == "per-level")
            continue;
        const choice = new Option(o.categories.data[i].name, o.categories.data[i].id)
        choice.classList.add(o.abbreviation);
        if (o.categories.data[i].miscellaneous)
            miscOpt.append(choice);
        else
            opt.append(choice);
    }
    if (opt.childElementCount != 0)
        nodes.push(opt);
    if (miscOpt.childElementCount != 0)
        nodes.push(miscOpt);
    resetList(list);
    $("#rgames-input").removeAttr("disabled");
    $("#rgames-input").val("");
    connection.stop();
})

connection.on("NotAGame", function () {
    $("#rgames-input").removeAttr("disabled");
    $("#rgames-input").val("");
    connection.stop();
})

connection.on("ConfirmUser", function (o) {
    const name = document.createElement('a');
    name.href = "#";
    name.classList.add("list-group-item", "list-group-item-action");
    name.setAttribute("index", counter);
    name.textContent = o.international;
    name.addEventListener('click', () => {
        name.remove();
        console.log(name.getAttribute("index"));
        users.splice(name.getAttribute("index"), 1);
        resetList($("#category-picker"));
    })
    $("#rusers-list").append(name);
    $("#rusers-input").removeAttr("disabled");
    $("#rusers-input").val("");
    users.push(o.id);
    counter++;
    connection.stop();
})

connection.on("NotAUser", function () {
    $("#rusers-input").removeAttr("disabled");
    $("#rusers-input").val("");
    connection.stop();
})

document.getElementById("submit-webhook").addEventListener('click', (e) => {
    $("#rcategories-val").val($("#category-picker").val().join())
    $("#rusers-val").val(users.join());
    $("#revents-val").val($("#events-picker").selectpicker('val'));
    if ($("#webhook-val").val() == "" || ($("#rcategories-val").val() == "" && $("#rusers-val").val() == ""))
        return;
    $("#webhook-form").submit();
})

function resetList(list) {
    const users = new Array();
    list.selectpicker("destroy");
    list.empty();
    list.append(nodes);
    list.selectpicker();
}

$('#selectAllCats').on('click', function (e) {
    const list = $("#category-picker");
    list.selectpicker("selectAll");
    resetList(list);
});