import { host } from "./config.js";
const endpoint = "/api/user/bookings";

const jwt = localStorage.getItem("JWT");

const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};

// ====================== Bookings ======================
// Get the bookings container element
const bookingsContainer = document.querySelector(".table-container");

// Function to render applications on the page
function renderMyBookings(bookings) {
  bookingsContainer.innerHTML = `
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
        ${bookings.map(generateBookingsHTML).join("")}
      </tbody>
    </table>
  `;
}

function generateBookingsHTML(booking) {
  return `
    <tr>
      <td>${booking.courseName}</td>
      <td>${booking.startTime}</td>
      <td>${booking.endTime}</td>
      <td><a href="${host}/course.html?id=${booking.roomId}" class="course-link">課程</a></td>
    </tr>
  `;
}

function renderNoContent() {
  bookingsContainer.innerHTML = `
    <div class="no-content">
      <h3>尚無預約課程</h3>
      <a href="${host}/course/search.html"><button class="btn btn-primary">找課程！</button></a>
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
      const booking = response.data;
      renderMyBookings(booking);
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
