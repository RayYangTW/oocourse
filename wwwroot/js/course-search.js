const host = "http://localhost:5202";
const endpoint = "/api/course/search";

// ====================== Course ======================
// Get the course container element
const courseContainer = document.querySelector(".courses-container");

// Function to generate the HTML for a single course
function getCourseHTML(course) {
  return `
  <a href="./course-detail.html?id=${course.id}" class="course">
    <img class="course-image" src="${course.courseImage}" alt="${course.courseName}">
    <div class= course-info>
      <div class="course-name">${course.courseName}</div>
      <div class="course-way">${course.courseWay}</div>
      <div class="course-location">${course.courseLocation}</div>
    </div>
  </a>
  `;
}

// Function to render course on the page
function renderCourses(courses) {
  courseContainer.innerHTML = courses.map(getCourseHTML).join("");
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

// ====================== Search by relocation ======================
$(document).ready(() => {
  const urlParams = new URLSearchParams(window.location.search);
  const searchKeyword = urlParams.get("keyword");

  if (searchKeyword !== null && searchKeyword.length > 0) {
    axios
      .get(host + endpoint + `?keyword=${searchKeyword}`)
      .then((response) => {
        if (response.data.length <= 0) {
          return Promise.reject(new Error("nothing found!"));
        } else {
          return response.data;
        }
      })
      .then((courses) => {
        renderCourses(courses);
      })
      .catch((err) => {
        renderErrorPage();
        console.log(err);
      });
  } else {
    axios
      .get(host + endpoint + `/all`)
      .then((response) => {
        return response.data;
      })
      .then((courses) => {
        renderCourses(courses);
      })
      .catch((err) => console.log(err));
  }
});

// ====================== Search by submit form ======================
document
  .getElementById("searchForm")
  .addEventListener("submit", function (event) {
    event.preventDefault(); // 阻止表單的預設提交行為
    const searchKeyword = document
      .getElementById("main-search-input")
      .value.trim();
    location.href = `/course/search.html?keyword=${searchKeyword}`;
  });
