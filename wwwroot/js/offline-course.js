/*****************************************
 * Fetch data from API and render the page
 *****************************************/
import { host } from "./config.js";
const endpoint = "/api/course/";
const endpointToCheckAccess = "/api/course/access";

const jwt = localStorage.getItem("JWT");

const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};

const urlParams = new URLSearchParams(window.location.search);
const roomId = urlParams.get("id");

axios
  .get(`${host}${endpointToCheckAccess}?roomId=${roomId}`, config)
  .then((response) => {
    if (response.status === 200) {
      initializeOfflineCourse();
      axios
        .get(host + endpoint + roomId, config)
        .then((response) => {
          return response.data;
        })
        .then((course) => {
          renderCourseDetail(course);
        })
        .catch((err) => console.log(err));
    }
  })
  .catch((err) => {
    location.href = `${host}/error.html`;
    console.log(err);
  });

function renderCourseDetail(course) {
  const courseContainer = document.querySelector(".left-section");
  courseContainer.innerHTML = `
  <div class="background">
    <div class="course-image-section">
      <img src="../assets/images/homepage-image.jpg" alt="" />
    </div>
    <div class="course-information-section">
      <div class="course-detail">
        <ul>
          <li>課程名稱：${course.courseName}</li>
          <li>開始時間：${course.startTime}</li>
          <li>結束時間：${course.endTime}</li>
          <li style="color:red;">上課方式：${course.courseWay}</li>
          <li>上課地點：${course.courseLocation}</li>
          <li style="color:red;">注意事項：${course.courseReminder}</li>
          <li>課程介紹：${course.courseIntro}</li>
        </ul>
      </div>
    </div>
  </div>
  `;
}

function initializeOfflineCourse() {
  /******************************
文字聊天功能
*******************************/
  // 禁止發送文字訊息，直到建立連接
  $(".send-btn").hide();

  // 連接服務
  const chatConn = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

  // 建立連接
  chatConn
    .start()
    .then(function () {
      // 連接成功顯示發送按鈕
      $(".send-btn").show();
    })
    .catch(function (err) {
      // 連接失敗返回錯誤訊息
      return console.error(err.toString());
    });

  // 發送消息
  $(".send-btn").click(sendMessage);
  $("#chat-input").keyup(function (event) {
    if (event.key === "Enter") {
      sendMessage();
    }
  });

  function sendMessage() {
    const user = localStorage.getItem("userName");
    const message = $("#chat-input").text();
    if (message !== null && message.length > 0) {
      chatConn.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
      });
      $("#chat-input").text("");
    }
  }

  // 接收消息
  chatConn.on("ReceiveMessage", function (user, message, time) {
    $("#chat-content").append(`<p>${user} : ${message}</p>`);
    $("#chat-content").animate({ scrollTop: 100000 });
  });
}
