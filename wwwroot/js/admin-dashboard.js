import { host } from "./config.js";
const platformDataEndpoint = "/api/admin/platformData";
const transactionDataEndpoint = "/api/admin/transactionData";
const courseDataEndpoint = "/api/admin/courseData";

const jwt = localStorage.getItem("JWT");
const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};

axios
  .get(host + platformDataEndpoint, config)
  .then((response) => {
    return response.data;
  })
  .then((platformData) => {
    renderBasicData(platformData);
  })
  .catch((err) => console.log(err));

const platformDataContainer = document.querySelector(
  ".platform-data-container"
);
function renderBasicData(platformData) {
  platformDataContainer.innerHTML = `
  <p>使用者人數：${platformData.userData}</p>
  <p>教師數：${platformData.teacherData}</p>
  <p>開設課程總堂數：${platformData.courseAmountData}</p>
  <p>線上課總數：${platformData.onlineCourseData}</p>
  <p>實體課總數：${platformData.offlineCourseData}</p>
  <p>已被預約課程總數：${platformData.courseIsBookedData}</p>
  `;
}

$(function () {
  var start = moment().subtract(29, "days");
  var end = moment();

  function cb(start, end) {
    $("#reportrange span").html(
      start.format("YYYY-MM-DD") + " - " + end.format("YYYY-MM-DD")
    );
    const startDate = start.format("YYYY-MM-DD");
    const endDate = end.format("YYYY-MM-DD");

    axios
      .get(
        `${host}${transactionDataEndpoint}?start=${startDate}&end=${endDate}`,
        config
      )
      .then((response) => {
        return response.data;
      })
      .then((transactionData) => {
        renderTransaction(transactionData);
      })
      .catch((err) => console.log(err));

    axios
      .get(
        `${host}${courseDataEndpoint}?start=${startDate}&end=${endDate}`,
        config
      )
      .then((response) => {
        return response.data;
      })
      .then((courseData) => {
        renderCourseData(courseData);
      })
      .catch((err) => console.log(err));
  }

  $("#reportrange").daterangepicker(
    {
      startDate: start,
      endDate: end,
      ranges: {
        今天: [moment(), moment()],
        昨天: [moment().subtract(1, "days"), moment().subtract(1, "days")],
        過去7天: [moment().subtract(6, "days"), moment()],
        過去30天: [moment().subtract(29, "days"), moment()],
        這個月: [moment().startOf("month"), moment().endOf("month")],
        上個月: [
          moment().subtract(1, "month").startOf("month"),
          moment().subtract(1, "month").endOf("month"),
        ],
      },
      autoApply: true,
    },
    cb
  );
  $('[data-range-key="Custom Range"]').text("選擇區間");

  cb(start, end);
});

const transactionContainer = document.querySelector(
  ".transaction-data-container"
);
function renderTransaction(transactionData) {
  transactionContainer.innerHTML = `
  <p>交易總額：NTD. ${transactionData.turnoverData}</p>
  <p>交易數：${transactionData.transactionData}</p>
  <p>實際營收：NTD. ${transactionData.revenueData}</p>
  `;
}

const courseDataContainer = document.querySelector(".course-data-container");
function renderCourseData(courseData) {
  courseDataContainer.innerHTML = `
  <p>開設課程堂數：${courseData.courseOfferingData}</p>
  <p>完成課程總堂數：${courseData.courseFinishedData}</p>
  <p>達成率：${courseData.achievementRate}</p>
  `;
}
