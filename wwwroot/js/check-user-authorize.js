import { host } from "./config.js";
const endpoint = "/api/user/checkAuthorize";
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
        icon: "warning",
        title: "請登入會員",
        text: "您尚未登入會員。",
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
      icon: "warning",
      title: "請登入會員",
      text: "您尚未登入會員。",
      showConfirmButton: true,
    }).then((result) => {
      if (result.isConfirmed) {
        location.href = `${host}/user/signin.html`;
      }
    });
    console.log(err);
  });
