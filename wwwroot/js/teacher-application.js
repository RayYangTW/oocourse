import { host } from "./config.js";
const endpoint = "/api/teacher/application";

const applicationForm = $("#teacher-application-form");

applicationForm.submit((e) => {
  e.preventDefault();
  const jwt = localStorage.getItem("JWT");
  console.log(jwt);

  let formData = new FormData();
  formData.append("name", $("#name").val());
  formData.append("email", $("#email").val());
  formData.append("country", $("#country").val());
  formData.append("location", $("#location").val());
  formData.append("language", $("#language").val());
  formData.append("category", $("#category").val());
  formData.append("experience", $("#experience").val());
  formData.append("description", $("#description").val());

  let files = $("#certification-file")[0].files;
  for (let i = 0; i < files.length; i++) {
    formData.append("certificationFiles", files[i]);
  }

  console.log(...formData);

  const config = {
    headers: {
      Authorization: `Bearer ${jwt}`,
      "Content-Type": "multipart/form-data",
    },
  };

  const loadingImg = document.querySelector(".loading");
  const htmlBody = document.querySelector("html");
  htmlBody.style.backgroundColor = "black";
  htmlBody.style.opacity = "0.5";
  loadingImg.style.display = "flex";

  axios
    .post(host + endpoint, formData, config)
    .then((response) => {
      console.log("succeed");
      console.log(response.data);
      alert("申請成功！");
      location.href = "/";
    })
    .catch((err) => {
      console.log("failed");
      console.error(err);
      alert("資料上傳失敗！請注意是否都有填寫正確。");
    })
    .finally(() => {
      htmlBody.style.backgroundColor = "";
      htmlBody.style.opacity = "1";
      loadingImg.style.display = "none";
    });
});
