const host = "http://localhost:5202";
const endpoint = "/api/teacher/myCourses";

const jwt = localStorage.getItem("JWT");

const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};

// ====================== Bookings ======================
// Get the bookings container element
const coursesContainer = document.querySelector(".table-container");

// Function to render applications on the page
function renderMyCourses(courses) {
  coursesContainer.innerHTML = `
    <table class="table table-hover">
      <thead>
        <tr>
          <th>課程名稱</th>
          <th>開始時間</th>
          <th>結束時間</th>
          <th>進入課程</th>
        </tr>
      </thead>
      <tbody>
        ${courses.map(generateCoursesHTML).join("")}
      </tbody>
    </table>
  `;
}

function generateCoursesHTML(course) {
  return `
    <tr>
      <td>${course.courseName}</td>
      <td>${course.startTime}</td>
      <td>${course.endTime}</td>
      <td><a href="${host}/course.html?id=${course.roomId}" class="course-link">課程</a></td>
    </tr>
  `;
}

function renderNoContent() {
  return `
    <div class="no-content">
      <h3>尚無預約課程</h3>
    </div>
  `;
}

// Function to render error on the page
function renderErrorPage(error) {
  coursesContainer.innerHTML = `
    <div class="error">
      <h3 class="error-message">抱歉！您搜尋的頁面不存在。</h3>
      <a href="/" class="btn btn-success error-link">回到首頁</a>
    </div>
  `;
}

/*******************************
 * Render the page
 *******************************/
// $(document).ready(function () {
//   // To get the id value
//   const urlParams = new URLSearchParams(window.location.search);
//   const idValue = urlParams.get("id");

//   // If value !== null, shows the courses
//   if (idValue !== null) {

//   } else {
//     coursesContainer.innerHTML = renderNoContent();
//   }
// });

axios
  .get(host + endpoint, config)
  .then((response) => {
    console.log(response);
    const course = response.data;
    renderMyCourses(course);
  })
  .catch((error) => {
    renderErrorPage(error);
    console.error("Error fetching data:", error);
  });
