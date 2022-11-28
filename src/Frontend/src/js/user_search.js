"use strict";

import '../css/user.scss';

// todo: make this use a form
document.getElementById("submit").addEventListener("click", function (e) {
    var user = document.getElementById("userName").value;
    window.location.assign("/users/" + user);
    event.preventDefault();
});