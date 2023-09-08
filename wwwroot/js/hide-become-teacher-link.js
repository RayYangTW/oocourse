import { host } from "./config.js";

// Check user login or not
$(document).ready(function () {
  const userRole = localStorage.getItem("role");
  const applicationLink = $(".teacher-application-link");

  if (userRole === "admin") {
    const adminLinkHtml = generateAdminLinkHtml();
    const adminLinkElement = $(adminLinkHtml);

    adminLinkElement.insertBefore(applicationLink);
    applicationLink.hide();
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
