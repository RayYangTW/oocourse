import { host } from "./config.js";
const endpoint = "/api/checkout/linepay/confirm";

const jwt = localStorage.getItem("JWT");

const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};

const urlParams = new URLSearchParams(window.location.search);
const transactionId = urlParams.get("transactionId");
const orderId = urlParams.get("orderId");

const bookingFormContainer = document.querySelector(".booking-container");

axios
  .get(
    host + endpoint + `?transactionId=${transactionId}&orderId=${orderId}`,
    config
  )
  .then((response) => {
    console.log(response);
    if (response.status === 200) {
      alert("付款完成！");
      renderBookingDetail(response.data);
    } else {
      alert("付款失敗！");
      renderBookingDetail(response.data);
    }
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
        <a href="/">
          <button class="btn back-home">回首頁</button>
        </a>
      </div>
    </div>
  </div>
  `;
}
