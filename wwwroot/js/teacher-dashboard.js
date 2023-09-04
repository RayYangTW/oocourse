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

  cb(start, end);
});

const salaryContainer = document.querySelector(".salary-data-container");

function renderTeachingFee(amountData) {
  salaryContainer.innerHTML = `
  <p>實際授課收入：
  ${amountData.teachingFeeData} - 平台費 
  ${amountData.platformFeeOfTeachingFee} = NTD. 
  ${amountData.teachingFeeData - amountData.platformFeeOfTeachingFee}
  </p>
  <p>最大授課可得收入：
  ${amountData.estimatedAmountData} - 平台費 
  ${amountData.platformFeeOfEstimatedAmount} = NTD. 
  ${amountData.estimatedAmountData - amountData.platformFeeOfEstimatedAmount}
  </p>
  <p>達成率：${amountData.achievementRate}%</p>
  `;
}

const courseContainer = document.querySelector(".course-data-container");

function renderCourseAmount(courseData) {
  courseContainer.innerHTML = `
  <p>完成課程總數：${courseData.taughtCourseAmount}</p>
  <p>開設課程總數：${courseData.openCourseAmount}</p>
  <p>達成率：${courseData.achievementRate}%</p>
  `;
}

const otherDataContainer = document.querySelector(".other-data-container");

function renderOtherData(otherData) {
  otherDataContainer.innerHTML = `
  <p>上課總時數：${otherData.totalDuration}</p>
  `;
}
