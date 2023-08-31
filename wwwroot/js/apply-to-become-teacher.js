import { host } from "./config.js";

$(document).ready(function () {
  $(".teacher-application-link").click(function (event) {
    event.preventDefault();
    const jwtToken = localStorage.getItem("JWT");

    if (jwtToken) {
      window.location.href = $(this).attr("href");
    } else {
      alert("請先登入！");
      window.location.href = `${host}/user/signin.html`;
    }
  });
});
