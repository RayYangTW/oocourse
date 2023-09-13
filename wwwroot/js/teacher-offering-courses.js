import { host } from "./config.js";
const endpoint = "/api/teacher/offeringCourses";
const cancelEndpoint = "/api/teacher/cancelCourse";

const jwt = localStorage.getItem("JWT");

const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};

// ====================== Bookings ======================
// Get the bookings container element
const offeringCoursesContainer = document.querySelector(".table-container");

// Function to render applications on the page
function renderMyOfferingCourses(courses) {
  offeringCoursesContainer.innerHTML = `
    <table class="table table-hover">
      <thead>
        <tr>
          <th>課程名稱</th>
          <th>開始時間</th>
          <th>結束時間</th>
          <th>價格</th>
          <th>取消開課</th>
        </tr>
      </thead>
      <tbody>
        ${courses.map(generateCourseHTML).join("")}
      </tbody>
    </table>
  `;
}

function generateCourseHTML(course) {
  return `
    <tr>
      <td>${course.courseName}</td>
      <td>${course.startTime}</td>
      <td>${course.endTime}</td>
      <td>${course.price}</td>
      <td><button class="cancel-button btn btn-policy" course-id="${course.id}">取消</button></td>
    </tr>
  `;
}

function renderNoContent() {
  bookingsContainer.innerHTML = `
    <div class="no-content">
      <h3>尚無課程刊登中</h3>
      <a href="${host}/teacher/publish-course.html"><button class="btn btn-primary">去刊登課程！</button></a>
    </div>
  `;
}

// Function to render error on the page
function renderErrorPage(error) {
  bookingsContainer.innerHTML = `
    <div class="error">
      <h3 class="error-message">抱歉！您搜尋的頁面不存在。</h3>
      <a href="/" class="btn btn-success error-link">回到首頁</a>
    </div>
  `;
}

/*******************************
 * Render the page
 *******************************/
axios
  .get(host + endpoint, config)
  .then((response) => {
    if (response.status === 200) {
      const courses = response.data;
      renderMyOfferingCourses(courses);
      initRemoveButton();
    } else if (response.status === 204) {
      renderNoContent();
    } else {
      renderErrorPage();
    }
  })
  .catch((error) => {
    renderErrorPage(error);
    console.error("Error fetching data:", error);
  });

function initRemoveButton() {
  const cancelButtons = document.querySelectorAll(".cancel-button");

  cancelButtons.forEach((button) => {
    button.addEventListener("click", () => {
      Swal.fire({
        title: "確定取消？",
        showDenyButton: true,
        showCancelButton: false,
        confirmButtonText: "確定",
        denyButtonText: `返回`,
      }).then((result) => {
        if (result.isConfirmed) {
          const loadingImg = document.querySelector(".loading");
          const htmlBody = document.querySelector("html");
          htmlBody.style.backgroundColor = "black";
          htmlBody.style.opacity = "0.5";
          loadingImg.style.display = "flex";

          const courseId = button.getAttribute("course-id");
          axios
            .delete(`${host}${cancelEndpoint}/${courseId}`, config)
            .then((response) => {
              if (response.status === 200) {
                Swal.fire("取消成功", "", "success").then(() => {
                  location.reload();
                });
              } else {
                Swal.fire("取消失敗", "", "error").then(() => {
                  console.error("Failed to cancel course:", response);
                });
              }
            })
            .catch((error) => {
              Swal.fire("取消失敗", "", "error").then(() => {
                console.error("Error canceling course:", error);
              });
            })
            .finally(() => {
              htmlBody.style.backgroundColor = "";
              htmlBody.style.opacity = "1";
              loadingImg.style.display = "none";
            });
        } else if (result.isDenied) {
          Swal.fire("課程未取消", "", "info").then(() => {
            location.reload();
          });
        }
      });
    });
  });
}
