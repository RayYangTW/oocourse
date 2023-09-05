import { host } from "./config.js";

// Check user login or not
$(document).ready(function () {
  const signinLink = $("#signin-link");

  const jwtToken = localStorage.getItem("JWT");

  if (jwtToken) {
    signinLink.text("我的會員");
    signinLink.attr("href", `${host}/user/portal.html`);
  } else {
    signinLink.text("登入");
    signinLink.attr("href", `${host}/user/signin.html`);
  }

  const userRole = localStorage.getItem("role");
  const navBar = $(".navbar-nav");

  if (userRole === "admin") {
    const adminLinkHtml = generateAdminLinkHtml();

    const adminLinkElement = $(adminLinkHtml);

    const firstNavItem = navBar.children("li.nav-item:first");

    adminLinkElement.insertBefore(firstNavItem);
  }
});

function generateAdminLinkHtml() {
  return `
  <li class="nav-item">
    <a
      class="nav-link mx-2 admin-portal-link"
      href="${host}/admin/portal.html"
      ><i class="fas fa-heart pe-2"></i>Admin</a
    >
  </li>
  `;
}
