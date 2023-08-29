const isProfileCompleted = localStorage.getItem("isProfileCompleted");

// Check user completed user profile or not
$(document).ready(function () {
  if (isProfileCompleted === "false") {
    alert("請先填寫會員資料！");
    location.href = "profile.html";
  }
});

// Check user is teacher or not
$(document).ready(function () {
  const userRole = localStorage.getItem("role");
  if (userRole === "teacher") {
    const newLink = $("<a>", {
      href: "/teacher/portal.html",
      class: "course-management",
    }).append(
      $("<button>", {
        type: "button",
        class: "btn btn-policy",
        text: "教師課程管理",
      })
    );

    newLink.insertAfter(".links-container .my-member-profile");
  }
});

$(document).ready(() => {
  const signoutBtn = $(".signout");

  signoutBtn.click(() => {
    localStorage.removeItem("JWT");
    localStorage.removeItem("role");
    localStorage.removeItem("isProfileCompleted");
    alert("登出成功！");
    location.href = "/";
  });
});
