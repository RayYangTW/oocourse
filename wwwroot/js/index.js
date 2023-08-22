// Check user login or not
$(document).ready(function () {
  const loginLink = $("#login-link");

  const jwtToken = localStorage.getItem("JWT");

  if (jwtToken) {
    loginLink.text("Profile");
    loginLink.attr("href", "user/portal.html");
  } else {
    loginLink.text("登入");
    loginLink.attr("href", "user/signin.html");
  }
});
