// Check user login or not
$(document).ready(function () {
  const signinLink = $("#signin-link");

  const jwtToken = localStorage.getItem("JWT");

  if (jwtToken) {
    signinLink.text("Profile");
    signinLink.attr("href", "user/portal.html");
  } else {
    signinLink.text("登入");
    signinLink.attr("href", "user/signin.html");
  }
});
