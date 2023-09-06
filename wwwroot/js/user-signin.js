import { host } from "./config.js";
const endpoint = "/api/user/signin";

$("#signin-form").submit(function (event) {
  event.preventDefault();

  const email = $("#email").val();
  const password = $("#password").val();

  if (!email || !password) {
    alert("請填寫完整的電子郵件和密碼");
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
      console.log(response);
      if (response.status === 200) {
        return response.data;
      } else {
        throw new Error("登入失敗");
      }
    })
    .then((responseData) => {
      const JWT = responseData.access_token;
      localStorage.setItem("JWT", JWT);
      const userRole = responseData.user.role;
      localStorage.setItem("role", userRole);
      const isProfileCompleted = responseData.user.isProfileCompleted;
      localStorage.setItem("isProfileCompleted", isProfileCompleted);
      const userName = responseData.user.userName;
      localStorage.setItem("userName", userName);
      Swal.fire({
        icon: "success",
        title: "登入成功",
        text: "您已成功登入。",
        showConfirmButton: true,
      }).then((result) => {
        if (result.isConfirmed) {
          location.href = "/";
        }
      });
    })
    .catch((error) => {
      Swal.fire({
        icon: "error",
        title: "登入失敗",
        text: "請確認登入資料是否正確。",
        showConfirmButton: true,
      });
      console.log("錯誤", error);
      console.log("錯誤", error.response.data);
    });
});
