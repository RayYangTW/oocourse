checkRole();

function checkRole() {
  const role = localStorage.getItem("role");

  if (role === "teacher") {
    return Swal.fire({
      icon: "warning",
      title: "已是教師",
      text: "已有教師身份，不必再次申請。",
      showConfirmButton: true,
    }).then((result) => {
      if (result.isConfirmed) {
        location.href = document.referrer;
      }
    });
  }
}
