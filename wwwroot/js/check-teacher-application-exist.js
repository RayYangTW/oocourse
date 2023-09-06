import { host } from "./config.js";
const endpoint = "/api/teacher/application";
const jwt = localStorage.getItem("JWT");
const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};

axios
  .get(host + endpoint, config)
  .then((response) => {
    if (response.status === 200) {
      Swal.fire({
        icon: "warning",
        title: "重複填寫",
        text: "您已填寫申請單，請靜待審核。",
        showConfirmButton: true,
      }).then((result) => {
        if (result.isConfirmed) {
          location.href = document.referrer;
        }
      });
    }
  })
  .catch((err) => {
    Swal.fire({
      icon: "error",
      title: "權限錯誤",
      text: "權限驗證錯誤，請重新登入。",
      showConfirmButton: true,
    }).then((result) => {
      if (result.isConfirmed) {
        location.href = `${host}/user/signin.html`;
      }
    });
    console.log(err);
  });
