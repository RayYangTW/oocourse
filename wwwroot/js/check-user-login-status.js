import { host } from "./config.js";

// Check user login or not
$(document).ready(function () {
  const signinLink = $("#signin-link");

  const userRole = localStorage.getItem("role");

  if (userRole === "user" || userRole === "teacher") {
    signinLink.text("我的會員");
    signinLink.attr("href", `${host}/user/portal.html`);
  } else if (userRole === "admin") {
    signinLink.hide();
  } else {
    signinLink.text("登入");
    signinLink.attr("href", `${host}/user/signin.html`);
  }
});
