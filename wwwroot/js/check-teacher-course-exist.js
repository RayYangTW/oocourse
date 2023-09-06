import { host } from "./config.js";
const endpoint = "/api/teacher/getTeacherFormData";
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
        icon: "error",
        title: "已有課程刊登中",
        text: "請勿重複刊登，若要更動請至編輯課程",
        showConfirmButton: true,
      }).then((result) => {
        if (result.isConfirmed) {
          location.href = `${host}/teacher/edit-course.html`;
        }
      });
    }
  })
  .catch((err) => {
    console.log(err);
  });
