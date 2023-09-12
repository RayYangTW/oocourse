const isProfileCompleted = localStorage.getItem("isProfileCompleted");

// Check user completed user profile or not
$(document).ready(function () {
  if (isProfileCompleted === "false") {
    Swal.fire({
      icon: "warning",
      title: "尚無會員資料",
      text: "請先填寫會員資料。",
      showConfirmButton: true,
    }).then((result) => {
      if (result.isConfirmed) {
        location.href = "profile.html";
      }
    });
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
    localStorage.removeItem("userName");
    Swal.fire({
      icon: "success",
      title: "登出成功",
      text: "您已成功登出。",
      showConfirmButton: true,
    }).then((result) => {
      if (result.isConfirmed) {
        location.href = "/";
      }
    });
  });
});
