import { host } from "./config.js";
const feeDataEndpoint = "/api/teacher/teachingFeeData";
const courseDataEndpoint = "/api/teacher/CourseData";
const teachTimeEndpoint = "/api/teacher/teachingTimeData";

const jwt = localStorage.getItem("JWT");
const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};

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
        `${host}${feeDataEndpoint}?start=${startDate}&end=${endDate}`,
        config
      )
      .then((response) => {
        return response.data;
      })
      .then((amountData) => {
        renderTeachingFee(amountData);
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
        renderCourseAmount(courseData);
      })
      .catch((err) => console.log(err));

    axios
      .get(
        `${host}${teachTimeEndpoint}?start=${startDate}&end=${endDate}`,
        config
      )
      .then((response) => {
        return response.data;
      })
      .then((otherData) => {
        renderOtherData(otherData);
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

const salaryContainer = document.querySelector(".salary-data-container");

function renderTeachingFee(amountData) {
  salaryContainer.innerHTML = `
  <div class="salary-data data-card">
    <div class="salary-data-chart data-chart">
      <p class="title">實際授課收入</p>
        <p class="calculation">${amountData.originTeachingFee}
         - 平台費 ${amountData.platformFeeOfTeachingFee}
         = </p>
         <p class="amount">${amountData.teachingFeeData}</span>
         <p class="unit">NTD</p>
      </p>
    </div>
  </div>

  <div class="salary-data data-card">
    <div class="salary-data-chart data-chart">
      <p class="title">潛在收入</p>
        <p class="calculation">${amountData.originEstimatedAmount}
         - 平台費 ${amountData.platformFeeOfEstimatedAmount}
         = </p>
         <p class="amount">${amountData.estimatedAmountData}</p>
         <p class="unit">NTD</p>
    </div>
  </div>

  <div class="salary-data data-card">
    <div class="salary-data-chart data-chart">
      <p class="title">達成率</p>
        <p class="amount">${(amountData.achievementRate * 100).toFixed()}</p>
        <p class="unit">%</p>
    </div>
  </div>
  `;
}

const courseContainer = document.querySelector(".course-data-container");

function renderCourseAmount(courseData) {
  courseContainer.innerHTML = `
  <div class="course-data data-card">
    <div class="course-data-chart data-chart">
      <p class="title">完成課程總數</p>
      <p class="amount">${courseData.taughtCourseAmount}</p>
      <p class="unit">堂</p>
    </div>
  </div>

  <div class="course-data data-card">
    <div class="course-data-chart data-chart">
      <p class="title">開設課程總數</p>
      <p class="amount">${courseData.openCourseAmount}</p>
      <p class="unit">堂</p>
    </div>
  </div>

  <div class="course-data data-card">
    <div class="course-data-chart data-chart">
      <p class="title">達成率</p>
      <p class="amount">${(courseData.achievementRate * 100).toFixed()}</p>
      <p class="unit">%</p>
    </div>
  </div>
  `;
}

const otherDataContainer = document.querySelector(".other-data-container");

function renderOtherData(otherData) {
  otherDataContainer.innerHTML = `
  <div class="other-data data-card">
    <div class="other-data-chart data-chart">
      <p class="title">上課總時數</p>
        <span class="amount counter">${otherData.totalDuration}</span>
    </div>
  </div>
  `;
}
