const host = "http://localhost:5202";
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
      <td><a href="${host}/lesson.html?id=${booking.roomId}" class="lesson-link">課程</a></td>
    </tr>
  `;
}

function renderNoContent() {
  return `
    <div class="no-content">
      <h3>尚無預約課程</h3>
      <button class="btn "><a href="../course/search.html">找課程！</a></button>
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
// $(document).ready(function () {
//   // To get the id value
//   const urlParams = new URLSearchParams(window.location.search);
//   const idValue = urlParams.get("id");

//   // If value !== null, shows the bookings
//   if (idValue !== null) {

//   } else {
//     bookingsContainer.innerHTML = renderNoContent();
//   }
// });

axios
  .get(host + endpoint, config)
  .then((response) => {
    const booking = response.data;
    renderMyBookings(booking);
  })
  .catch((error) => {
    renderErrorPage(error);
    console.error("Error fetching data:", error);
  });