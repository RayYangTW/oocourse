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
    return response.data;
  })
  .then((bookingData) => {
    renderBookingDetail(bookingData);
  })
  .catch((err) => {
    console.log(err);
    if (err.response.status === 403) {
      Swal.fire({
        icon: "error",
        title: "操作錯誤",
        text: "請勿預約自己的課程！",
        showConfirmButton: true,
      }).then((result) => {
        if (result.isConfirmed) {
          location.href = document.referrer;
        }
      });
    }
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

    axios
      .post(host + linePayEndPoint, formData, config)
      .then((response) => {
        if (response.data.redirectUrl) {
          location.href = response.data.redirectUrl;
        } else {
          Swal.fire({
            icon: "error",
            title: "跳轉失敗",
            text: "無跳轉網址",
            showConfirmButton: true,
          }).then((result) => {
            if (result.isConfirmed) {
              location.href = document.referrer;
            }
          });
        }
      })
      .catch((err) => {
        console.log(err);
        Swal.fire({
          icon: "error",
          title: "晚了一步",
          text: "課程已被預約",
          showConfirmButton: true,
        }).then((result) => {
          if (result.isConfirmed) {
            location.href = document.referrer;
          }
        });
      });
  });
}
