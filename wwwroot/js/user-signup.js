import { host } from "./config.js";
const endpoint = "/api/user/signup";

$("#signup-form").submit(function (event) {
  event.preventDefault();

  const email = $("#email").val();
  const password = $("#password").val();
  const checkPassword = $("#checkPassword").val();

  if (!email || !password || !checkPassword) {
    alert("請填寫完整的電子郵件和密碼");
    return;
  }

  if (password !== checkPassword) {
    alert("密碼不一致");
    return;
  }

  const data = {
    provider: "native",
    email: email,
    password: password,
  };

  const config = {
    headers: {
      "Content-Type": "application/json",
    },
  };

  axios
    .post(host + endpoint, data, config)
    .then((response) => {
      if (response.status === 200) {
        return response.data;
      } else {
        throw new Error("註冊失敗");
      }
    })
    .then(() => {
      alert("註冊成功");
      location.href = "/";
    })
    .catch((err) => {
      if (err.response.data === "Email existed!") {
        alert("Email已存在。");
      }
      console.log("錯誤", err);
    });
});
