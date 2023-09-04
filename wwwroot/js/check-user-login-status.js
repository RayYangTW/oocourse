import { host } from "./config.js";

// Check user login or not
$(document).ready(function () {
  const signinLink = $("#signin-link");

  const jwtToken = localStorage.getItem("JWT");

  if (jwtToken) {
    signinLink.text("Profile");
    signinLink.attr("href", `${host}/user/portal.html`);
  } else {
    signinLink.text("登入");
    signinLink.attr("href", `${host}/user/signin.html`);
  }

  const userRole = localStorage.getItem("role");
  const navBar = $(".navbar-nav");

  if (userRole === "admin") {
    // 调用 generateAdminLinkHtml() 函数生成 HTML 代码
    const adminLinkHtml = generateAdminLinkHtml();

    // 创建一个 jQuery 对象来表示生成的管理员链接元素
    const adminLinkElement = $(adminLinkHtml);

    // 查找导航栏 ul 列表的第一个子元素 li
    const firstNavItem = navBar.children("li.nav-item:first");

    // 将生成的管理员链接元素插入到第一个 li 元素之前
    adminLinkElement.insertBefore(firstNavItem);
  }
});

function generateAdminLinkHtml() {
  return `
  <li class="nav-item">
    <a
      class="nav-link mx-2 admin-portal-link"
      href="./admin/portal.html"
      ><i class="fas fa-heart pe-2"></i>Admin</a
    >
  </li>
  `;
}
