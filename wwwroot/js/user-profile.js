import { host } from "./config.js";
const endpoint = "/api/user/profile";

const jwt = localStorage.getItem("JWT");

// Render profile form
const profileContainer = document.querySelector(".form-container");

function renderProfile(profile) {
  profileContainer.innerHTML = `
    <h1 class="text-center">我的會員資料</h1>
      <form id="profile-form">
        <div class="form-group">
          <div class="avatar-container">
            <img src="${profile.avatar}" alt="${profile.nickname}">
          </div>
        </div>
        <div class="form-group">
          <label for="name" class="form-label">名字<span class="required-star"> *</span></label>
          <input
            type="text"
            class="form-control item"
            id="name"
            value="${profile.name}"
            maxlength="30"
            required
          />
        </div>
        <div class="form-group">
          <label for="nickname" class="form-label">暱稱</label>
          <input
            type="text"
            class="form-control item"
            id="nickname"
            value="${profile.nickname}"
            maxlength="30"
          />
        </div>
        <div class="form-group">
          <label for="gender" class="form-label">性別<span class="required-star"> *</span></label>
          <select class="form-control item" id="gender" name="gender" required>
            <option value="" disabled selected>請選擇性別</option>
            <option value="男" ${
              profile.gender === "男" ? "selected" : ""
            }>男</option>
            <option value="女" ${
              profile.gender === "女" ? "selected" : ""
            }>女</option>
            <option value="其他" ${
              profile.gender === "其他" ? "selected" : ""
            }>其他</option>
          </select>
        </div>
        <div class="form-group">
          <label for="interest" class="form-label">感興趣的<span class="required-star"> *</span></label>
          <input
            type="text"
            class="form-control item"
            id="interest"
            value="${profile.interest}"
            maxlength="100"
            required
          />
        </div>
        <div class="form-group">
          <label for="avatar-file" class="form-label">個人照</label>
          <input
            type="file"
            class="form-control item"
            id="avatar-file"
            accept=".png,.jpg,.jpeg"
          />
        </div>
        <div class="form-group text-center">
          <button type="submit" class="btn btn-block modify">修改</button>
        </div>
      </form>
  `;
  updateProfile(profile);
}
$(document).ready(() => {
  const config = {
    headers: {
      Authorization: "Bearer " + jwt,
    },
  };
  axios
    .get(host + endpoint, config)
    .then((profile) => {
      renderProfile(profile.data);
    })
    .catch((err) => {
      console.log(err);
    });
});

//. Render profile form

// Update Profile
function updateProfile(profile) {
  const profileForm = document.querySelector("#profile-form");

  profileForm.addEventListener("submit", (e) => {
    e.preventDefault();

    let formData = new FormData();
    formData.append("name", $("#name").val());
    formData.append("nickname", $("#nickname").val());
    formData.append("gender", $("#gender").val());
    formData.append("interest", $("#interest").val());
    formData.append("avatarFile", $("#avatar-file")[0].files[0]);

    const config = {
      headers: {
        Authorization: "Bearer " + jwt,
      },
    };

    const loadingImg = document.querySelector(".loading");
    const htmlBody = document.querySelector("html");
    htmlBody.style.backgroundColor = "black";
    htmlBody.style.opacity = "0.5";
    loadingImg.style.display = "flex";

    axios
      .put(host + endpoint, formData, config)
      .then((response) => {
        localStorage.setItem("isProfileCompleted", true);
        localStorage.setItem("userName", $("#name").val());
        Swal.fire({
          icon: "success",
          title: "修改成功",
          text: "您已成功修改資料。",
          showConfirmButton: true,
        }).then((result) => {
          if (result.isConfirmed) {
            location.href = "portal.html";
          }
        });
      })
      .catch((err) => {
        console.log(err);
        Swal.fire({
          icon: "error",
          title: "修改失敗",
          text: "請注意是否都有填寫正確。",
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
