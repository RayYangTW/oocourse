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
    console.log(platformData);
    renderBasicData(platformData);
    userDataChart(platformData);
    courseDataChart(platformData);
  })
  .catch((err) => console.log(err));

const platformDataContainer = document.querySelector(
  ".platform-data-container"
);
function renderBasicData(platformData) {
  platformDataContainer.innerHTML = `
  
  <div class="user-data data-card">
    <div class="user-data-chart data-chart">
      <p class="title">使用者計數</p>
      <canvas id="user-doughnut-chart" style="width: 300px;"></canvas>
      <p class="label">${platformData.userData + platformData.teacherData}</p>
    </div>
  </div>

  <div class="course-data data-card">
    <div class="course-data-chart data-chart">
      <p class="title">課程數據</p>
      <canvas id="course-doughnut-chart" style="width: 300px;"></canvas>
      <p class="label">${platformData.courseAmountData}</p>
    </div>
  </div>

  <div class="course-data data-card">
    <div class="course-data-chart data-chart">
      <p class="title">已被預約課程總數</p>
      <p class="amount">${platformData.courseIsBookedData}</p>
    </div>
  </div>
  `;
}

function userDataChart(platformData) {
  // 數據
  var data = {
    labels: ["用戶數", "教師數"],
    datasets: [
      {
        data: [platformData.userData, platformData.teacherData],
        backgroundColor: ["#FF6384", "#36A2EB"],
      },
    ],
  };

  // 配置選項
  var options = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      datalabels: {
        color: "black",
        labels: {
          title: {
            font: {
              size: "20",
              weight: "bold",
            },
          },
        },
      },
    },
  };

  // 獲取Canvas元素
  var ctx = document.getElementById("user-doughnut-chart").getContext("2d");

  // 創建甜甜圈圖
  var myDoughnutChart = new Chart(ctx, {
    type: "doughnut",
    data: data,
    options: options,
  });
}

function courseDataChart(platformData) {
  // 數據
  var data = {
    labels: ["線上課總數", "實體課總數"],
    datasets: [
      {
        data: [platformData.onlineCourseData, platformData.offlineCourseData],
        backgroundColor: ["#FF6384", "#36A2EB"],
      },
    ],
  };

  // 配置選項
  var options = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      datalabels: {
        color: "black",
        labels: {
          title: {
            font: {
              size: "20",
              weight: "bold",
            },
          },
        },
      },
    },
  };

  // 獲取Canvas元素
  var ctx = document.getElementById("course-doughnut-chart").getContext("2d");

  // 創建甜甜圈圖
  var myDoughnutChart = new Chart(ctx, {
    type: "doughnut",
    data: data,
    options: options,
  });
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
  <div class="transaction-data data-card">
    <div class="transaction-data-chart data-chart">
      <p class="title">交易總額</p>
      <p class="amount">${transactionData.turnoverData}</p>
      <p class="unit">NTD</p>
    </div>
  </div>

  <div class="transaction-data data-card">
    <div class="transaction-data-chart data-chart">
      <p class="title">交易數</p>
      <p class="amount">${transactionData.transactionData}</p>
      <p class="unit">次</p>
    </div>
  </div>

  <div class="transaction-data data-card">
    <div class="transaction-data-chart data-chart">
      <p class="title">實際營收</p>
      <p class="amount">${transactionData.revenueData}</p>
      <p class="unit">NTD</p>
    </div>
  </div>
  `;
}

const courseDataContainer = document.querySelector(".course-data-container");
function renderCourseData(courseData) {
  courseDataContainer.innerHTML = `
  <div class="course-data data-card">
    <div class="course-data-chart data-chart">
      <p class="title">開設課程堂數</p>
      <p class="amount">${courseData.courseOfferingData}</p>
      <p class="unit">堂</p>
    </div>
  </div>

  <div class="course-data data-card">
    <div class="course-data-chart data-chart">
      <p class="title">完成課程總堂數</p>
      <p class="amount">${courseData.courseFinishedData}</p>
      <p class="unit">堂</p>
    </div>
  </div>

  <div class="course-data data-card">
    <div class="course-data-chart data-chart">
      <p class="title">達成率</p>
      <p class="amount">${(courseData.achievementRate * 100).toFixed()}</p>
      <p class="unit">％</p>
    </div>
  </div>
  `;
}
