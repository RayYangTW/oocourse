const host = "http://localhost:5202";
const endpoint = "/api/teacher/application";

const applicationForm = $("#teacher-application-form");

applicationForm.submit((e) => {
  e.preventDefault();

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
      "Content-Type": "multipart/form-data",
    },
  };
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
      console.log(err);
      alert("資料上傳失敗！請注意是否都有填寫正確。");
    });
});
