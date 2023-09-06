import { host } from "./config.js";
const endpoint = "/api/teacher/checkAuthorize";
const jwt = localStorage.getItem("JWT");
const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};

axios
  .get(host + endpoint, config)
  .then((response) => {
    if (response.status !== 200) {
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
