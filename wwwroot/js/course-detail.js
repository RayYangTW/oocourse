import { host } from "./config.js";
const endpoint = "/api/course/search/";

// ====================== Course ======================
// Get the course container element
const courseContainer = document.querySelector(".course-container");

// Function to generate the HTML for a single course
function getCourseHTML(course) {
  return `
  <div class="course-info-container">
    <div class="image-section">
      <img src="${course.courseImage}" alt="${course.courseName}" />
    </div>
    <div class="course-detail-section">
      <p>課程名稱：${course.courseName}</p>
      <p>課程類別：${course.courseCategory}</p>
      <p>課程地點：${course.courseLocation}</p>
      <p>課程使用語言：${course.courseLanguage}</p>
      <p>授課方式：${course.courseWay}</p>
    </div>
    <div class="course-intro-section">
      <p>課程介紹：${course.courseIntro}</p>
      <p>課程注意事項：${course.courseReminder}</p>
    </div>
  </div>
  <div class="course-available-time-container">
    <div class="title-section">
      <h3>可預約時段</h3>
    </div>
    <table class="table table-hover">
      <thead>
        <tr>
          <th>開始時間</th>
          <th>結束時間</th>
          <th>價格</th>
          <th>預約</th>
        </tr>
      </thead>
      <tbody>            
        ${course.courses
          .map(
            (courseDetail) => `
            <tr>
            <td>${courseDetail.startTime}</td>
            <td>${courseDetail.endTime}</td>
            <td>${courseDetail.price}</td>
            <td><a href="booking.html?courseId=${courseDetail.id}" class="booking-link">預約</a></td>
            </tr>
        `
          )
          .join("")}
      </tbody>
    </table>
  </div>
  `;
}

// Function to render course on the page
function renderCourseDetail(course) {
  courseContainer.innerHTML = getCourseHTML(course);
}

// Function to render error on the page
function renderErrorPage(error) {
  courseContainer.innerHTML = `
      <div class="error">
      <h3 class="error-message">抱歉！您搜尋的頁面不存在。</h3>
      <a href="/" class="btn btn-success error-link">回到首頁</a>
      </div>
      `;
}

// Get course data from api
$(document).ready(() => {
  const urlParams = new URLSearchParams(window.location.search);
  const courseId = urlParams.get("id");
  axios
    .get(host + endpoint + courseId)
    .then((response) => {
      console.log(response);
      if (response.data.length <= 0) {
        return Promise.reject(new Error("nothing found!"));
      } else {
        return response.data;
      }
    })
    .then((course) => {
      renderCourseDetail(course);
    })
    .catch((err) => {
      renderErrorPage();
      console.log(err);
    });
});
