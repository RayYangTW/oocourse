const host = "http://localhost:5202";
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
      // Go back to last page and refresh
      alert("登入成功");
      location.href = document.referrer;
    })
    .catch((error) => {
      console.log("錯誤", error);
    });
});