import { host } from "./config.js";
const endpoint = "/api/teacher/application";
const defaultDataEndpoint = "/api/user/applicationDefaultData";

const jwt = localStorage.getItem("JWT");
const config = {
  headers: {
    Authorization: `Bearer ${jwt}`,
  },
};

const formContainer = document.querySelector(".form-container");

axios
  .get(host + defaultDataEndpoint, config)
  .then((response) => {
    console.log(response);
    return response.data;
  })
  .then((defaultData) => {
    renderTeacherApplication(defaultData);
    submitApplication();
    initializeCountrySelect();
  })
  .catch((err) => console.log(err));

function renderTeacherApplication(defaultData) {
  formContainer.innerHTML = `
  <h1 class="text-center">教師資格申請</h1>
      <form id="teacher-application-form">
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="name"
            placeholder="姓名"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="email"
            placeholder="電子信箱"
            value="${defaultData.userEmail}"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="country"
            placeholder="國籍"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="location"
            placeholder="所在地"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="language"
            placeholder="溝通語言"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="category"
            placeholder="課程類別"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="experience"
            placeholder="教學經驗"
            required
          />
        </div>
        <div class="form-group">
          <textarea
            type="text"
            class="form-control item"
            id="description"
            placeholder="個人特色"
            required
          ></textarea>
        </div>
        <div class="form-group">
          <label for="certification-file" class="form-label">相關證照</label>
          <input
            type="file"
            class="form-control item"
            id="certification-file"
            accept=".pdf,.png,.jpg,.jpeg"
            multiple
          />
        </div>
        <div class="form-group text-center">
          <button type="submit" class="btn btn-block apply">送出申請</button>
        </div>
      </form>
  `;
}

function submitApplication() {
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
        Swal.fire({
          icon: "success",
          title: "申請成功",
          text: "您已成功申請，請靜待審核。",
          showConfirmButton: true,
        }).then((result) => {
          if (result.isConfirmed) {
            location.href = document.referrer;
          }
        });
      })
      .catch((err) => {
        console.log("failed");
        console.error(err);
        Swal.fire({
          icon: "error",
          title: "申請失敗",
          text: "請確認資料是否填寫正確。",
          showConfirmButton: true,
        });
      })
      .finally(() => {
        htmlBody.style.backgroundColor = "";
        htmlBody.style.opacity = "1";
        loadingImg.style.display = "none";
      });
  });
}

function initializeCountrySelect() {
  $("#country").countrySelect({
    defaultCountry: "tw",
    preferredCountries: ["tw", "us", "ca"],
    responsiveDropdown: true,
  });
}
