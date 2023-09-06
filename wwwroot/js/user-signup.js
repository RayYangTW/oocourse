import { host } from "./config.js";
const endpoint = "/api/user/signup";

$("#signup-form").submit(function (event) {
  event.preventDefault();

  const email = $("#email").val();
  const password = $("#password").val();
  const checkPassword = $("#checkPassword").val();

  if (!email || !password || !checkPassword) {
    Swal.fire({
      icon: "error",
      title: "資訊不完整",
      text: "請填寫完整的電子郵件和密碼",
      showConfirmButton: true,
    });
    return;
  }

  if (password !== checkPassword) {
    Swal.fire({
      icon: "warning",
      title: "密碼與確認密碼不一致",
      text: "請重新輸入",
      showConfirmButton: true,
    });
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
      Swal.fire({
        icon: "success",
        title: "註冊成功",
        text: "您已成功註冊。",
        showConfirmButton: true,
      }).then((result) => {
        if (result.isConfirmed) {
          location.href = "/";
        }
      });
    })
    .catch((err) => {
      if (err.response.data === "Email existed!") {
        Swal.fire({
          icon: "warning",
          title: "Email已存在",
          text: "請重新輸入。",
          showConfirmButton: true,
        });
      }
      console.log("錯誤", err);
    });
});
