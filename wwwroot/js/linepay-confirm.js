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
    if (response.status === 200) {
      Swal.fire({
        icon: "success",
        title: "付款成功",
        text: "您已成功完成付款。",
        showConfirmButton: true,
      });
      renderBookingDetail(response.data);
    } else {
      Swal.fire({
        icon: "error",
        title: "付款失敗",
        text: "您未完成付款。",
        showConfirmButton: true,
      });
      renderBookingDetail(response.data);
    }
  })
  .catch((err) => {
    Swal.fire({
      icon: "error",
      title: "付款失敗",
      text: "課程已被預約",
      showConfirmButton: true,
    }).then((result) => {
      if (result.isConfirmed) {
        location.href = `${host}`;
      }
    });
  });

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
