const host = "http://localhost:5202";
const endpoint = "/api/booking/";

const jwt = localStorage.getItem("JWT");

const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};
const bookingFormContainer = document.querySelector(".booking-container");

const urlParams = new URLSearchParams(window.location.search);
const courseId = urlParams.get("courseId");

axios
  .get(host + endpoint + courseId, config)
  .then((response) => {
    console.log(response);
    return response.data;
  })
  .then((bookingData) => {
    renderBookingDetail(bookingData);
  })
  .catch((err) => console.log(err));

function renderBookingDetail(booking) {
  bookingFormContainer.innerHTML = `
  <div class="background">
    <div class="row">
      <div class="course-image-section col-4">
        <img src="${booking.courseImage}" alt="" />
      </div>
      <div class="course-information-section col-8">
        <div class="booking-info">
          <h3>預約資訊：</h3>
          <ul>
            <li>開始時間：${booking.startTime}</li>
            <li>結束時間：${booking.endTime}</li>
            <li>價格：${booking.price}</li>
          </ul>
        </div>
        <div class="course-detail">
          <ul>
            <li>課程名稱：${booking.courseName}</li>
            <li>上課方式：${booking.courseWay}</li>
            <li>上課地點：${booking.courseLocation}</li>
            <li>注意事項：${booking.courseReminder}</li>
          </ul>
        </div>
        <div class="submit-button-container">
          <button class="btn submit">確認</button>
        </div>
      </div>
    </div>
  </div>
  `;
  submitBooking();
}

function submitBooking() {
  const submitBtn = document.querySelector(".submit");

  submitBtn.addEventListener("click", () => {
    console.log("我被按下了");
    console.log(config);
    axios
      .post(host + endpoint + courseId, "", config)
      .then(() => {
        alert("預定成功！");
        location.href = "/";
      })
      .catch((err) => console.log(err));
  });
}
