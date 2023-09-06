import { host } from "./config.js";
const endpoint = "/api/admin/teacher/applications";

// ====================== Applications ======================
// Get the applications container element
const applicationsContainer = document.querySelector(".table-container");

// Function to generate the HTML for a single application
function getApplicationHTML(application) {
  return `
    <tr>
      <td>${application.id}</td>
      <td>${application.createdTime}</td>
      <td>${application.description}</td>
      <td><a href="./applications.html?id=${application.id}" class="application-link">查看</a></td>
    </tr>
  `;
}

// Function to render applications on the page
function renderApplications(applications) {
  applicationsContainer.innerHTML = `
    <h2 class="text-center">教師資格申請單</h2>
    <table class="table table-hover">
      <thead>
        <tr>
          <th>申請單號</th>
          <th>申請日期</th>
          <th>標題</th>
          <th>審核</th>
        </tr>
      </thead>
      <tbody>
        ${applications.map(getApplicationHTML).join("")}
      </tbody>
    </table>
  `;
}

// Function to render single application
function renderApplicationDetail(application) {
  applicationsContainer.innerHTML = `
    <div class="application-detail form-container">
      <h1 class="text-center">教師資格申請</h1>
      <form id="teacher-application-form">
        <div class="form-group">
          <label for="id" class="form-label">申請單號</label>
          <input
            type="text"
            class="form-control item"
            id="id"
            placeholder="ID"
            value="${application.id}"
            readonly
          />
        </div>
        <div class="form-group">
          <label for="name" class="form-label">姓名</label>
          <input
            type="text"
            class="form-control item"
            id="name"
            placeholder="姓名"
            value="${application.name}"
            readonly
          />
        </div>
        <div class="form-group">
          <label for="email" class="form-label">電子信箱</label>
          <input
            type="text"
            class="form-control item"
            id="email"
            placeholder="電子信箱"
            value="${application.email}"
            readonly
          />
        </div>
        <div class="form-group">
          <label for="country" class="form-label">國籍</label>
          <input
            type="text"
            class="form-control item"
            id="country"
            placeholder="國籍"
            value="${application.country}"
            readonly
          />
        </div>
        <div class="form-group">
          <label for="location" class="form-label">所在地</label>
          <input
            type="text"
            class="form-control item"
            id="location"
            placeholder="所在地"
            value="${application.location}"
            readonly
          />
        </div>
        <div class="form-group">
          <label for="language" class="form-label">溝通語言</label>
          <input
            type="text"
            class="form-control item"
            id="language"
            placeholder="溝通語言"
            value="${application.language}"
            readonly
          />
        </div>
        <div class="form-group">
          <label for="category" class="form-label">課程類別</label>
          <input
            type="text"
            class="form-control item"
            id="category"
            placeholder="課程類別"
            value="${application.category}"
            readonly
          />
        </div>
        <div class="form-group">
          <label for="experience" class="form-label">教學經驗</label>
          <input
            type="text"
            class="form-control item"
            id="experience"
            placeholder="教學經驗"
            value="${application.experience}"
            readonly
          />
        </div>
        <div class="form-group">
          <label for="description" class="form-label">個人特色</label>
          <textarea
            type="text"
            class="form-control item"
            id="description"
            placeholder="個人特色"
            readonly
          >${application.description}</textarea>
        </div>
        <div class="form-group">
          <label for="certification-files" class="form-label">相關證照</label>
          <div class="certification-files-container">
            <ol>
              ${application.certifications
                .map(
                  (file) => `
              <li class="certification-file">
                <a href="${file.certification}" target="_blank">${application.name}</a>
              </li>
              `
                )
                .join("")}
            </ol>
          </div>
        </div>
        <div class="form-group text-center button-container">
          <button type="button" class="btn btn-block approved">審核通過</button>
          <button type="button" class="btn btn-block denied">否決</button>
        </div>
      </form>
    </div>
  `;
}

// Function to render error on the page
function renderErrorPage(error) {
  applicationsContainer.innerHTML = `
    <div class="error">
      <h3 class="error-message">抱歉！您搜尋的頁面不存在。</h3>
      <a href="./index.html" class="btn btn-success error-link">回到首頁</a>
    </div>
  `;
}

/*******************************
 * Make buttons have function
 *******************************/
function activateButtons(application) {
  $(".approved").on("click", function () {
    axios
      .post(`${host}${endpoint}/approve/${application.id}`)
      .then((response) => {
        if (response.status === 200) {
          Swal.fire({
            icon: "success",
            title: "完成審核",
            text: "您已成功審核。",
            showConfirmButton: true,
          }).then((result) => {
            if (result.isConfirmed) {
              location.href = `${host}/admin/teacher/applications.html`;
            }
          });
        }
      })
      .catch((err) => {
        console.log(err);
      });
  });

  // 監聽否決按鈕的點擊事件
  $(".denied").on("click", function () {
    axios
      .post(`${host}${endpoint}/deny/${application.id}`)
      .then((response) => {
        if (response.status === 200) {
          Swal.fire({
            icon: "success",
            title: "否決成功",
            text: "您已成功否決該申請。",
            showConfirmButton: true,
          }).then((result) => {
            if (result.isConfirmed) {
              location.href = `${host}/admin/teacher/applications.html`;
            }
          });
        }
      })
      .catch((err) => {
        console.log(err);
      });
    location.href = "/admin/teacher/applications.html";
  });
}

/*******************************
 * Render the page
 *******************************/
$(document).ready(function () {
  // To get the id value
  const urlParams = new URLSearchParams(window.location.search);
  const idValue = urlParams.get("id");
  // If value !== null, shows the application
  if (idValue !== null && idValue.length > 0) {
    axios
      .get(`${host}${endpoint}/${idValue}`)
      .then((response) => {
        const application = response.data;
        renderApplicationDetail(application);
        activateButtons(application);
      })
      .catch((error) => {
        renderErrorPage(error);
        console.error("Error fetching data:", error);
      });
  } else {
    // If value === null, shows the list of applications
    axios
      .get(host + endpoint)
      .then((response) => {
        const applications = response.data;
        renderApplications(applications);
      })
      .catch((error) => {
        console.error("Error fetching data:", error);
      });
  }
});
