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
      alert("請登入會員。");
      location.href = `${host}/user/signin.html`;
    }
  })
  .catch((err) => {
    alert("請登入會員。");
    location.href = `${host}/user/signin.html`;
    console.log(err);
  });
