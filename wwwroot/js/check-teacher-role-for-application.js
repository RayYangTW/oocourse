checkRole();

function checkRole() {
  const role = localStorage.getItem("role");

  if (role === "teacher") {
    alert("已有教師身份，不必再次申請。");
    return (location.href = document.referrer);
  }
}
