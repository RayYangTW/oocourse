import { host } from "./config.js";
const endpoint = "/api/booking/";
const linePayEndPoint = "/api/checkout/linepay";

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
          <button class="btn submit">LINEPay付款</button>
        </div>
      </div>
    </div>
  </div>
  `;
  submitBooking(booking);
}

function submitBooking(booking) {
  const submitBtn = document.querySelector(".submit");

  submitBtn.addEventListener("click", () => {
    const formData = new FormData();
    formData.append("productName", booking.courseName);
    formData.append("productQty", 1);
    formData.append("productPrice", booking.price);
    formData.append("courseId", courseId);

    console.log(...formData);
    axios
      .post(host + linePayEndPoint, formData, config)
      .then((response) => {
        console.log(response);
        // alert("預訂成功！");
        // location.href = "/";
        if (response.data.redirectUrl) {
          location.href = response.data.redirectUrl;
        } else {
          console.log("無跳轉網址");
        }
      })
      .catch((err) => {
        // if (err.response.status === 403) {
        //   alert("該課程已被預訂，請重新選購。");
        // }
        console.log(err);
      });
  });
}
