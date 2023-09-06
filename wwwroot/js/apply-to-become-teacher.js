import { host } from "./config.js";

$(document).ready(function () {
  $(".teacher-application-link").click(function (event) {
    event.preventDefault();
    const jwtToken = localStorage.getItem("JWT");

    if (jwtToken) {
      window.location.href = $(this).attr("href");
    } else {
      Swal.fire({
        icon: "warning",
        title: "請登入會員",
        text: "您尚未登入會員。",
        showConfirmButton: true,
      }).then((result) => {
        if (result.isConfirmed) {
          location.href = `${host}/user/signin.html`;
        }
      });
    }
  });
});
