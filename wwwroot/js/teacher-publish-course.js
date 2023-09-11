import { host } from "./config.js";
const endpoint = "/api/teacher/publishCourse";

const jwt = localStorage.getItem("JWT");

const config = {
  headers: {
    Authorization: "Bearer " + jwt,
    "Content-Type": "multipart/form-data",
  },
};

$("#add-input").click(function () {
  const newTimeblock = $("<div>").addClass("row form-row");

  newTimeblock.html(`
      <div class="col-4">
        <label for="startTime">開始時間</label>
        <input 
          type="datetime-local" 
          class="form-control" 
          name="startTime"
          id="startTime"
        />
      </div>
      <div class="col-4">
        <label for="endTime">結束時間</label>
        <input 
          type="datetime-local" 
          class="form-control" 
          name="endTime" 
          id="endTime"
        />
      </div>
      <div class="col-2">
        <label for="price">費用</label>
        <input
          type="number"
          class="form-control"
          id="price"
          name="price"
        />
      </div>
      <div class="col-2 d-flex text-center align-items-center justify-content-center">
        <button type="button" class="btn btn-danger remove-input">移除</button>
      </div>
    `);

  $(".course-time-container").append(newTimeblock);
});

$(".course-time-container").on("click", ".remove-input", function () {
  $(this).closest(".form-row").remove();
});

$("#teacher-application-form").submit((e) => {
  e.preventDefault();

  // Validate if endTime >= startTime
  const startTimeInputs = document.querySelectorAll("input[name='startTime']");
  const endTimeInputs = document.querySelectorAll("input[name='endTime']");

  let isValid = true;
  for (let i = 0; i < startTimeInputs.length; i++) {
    const startTime = new Date(startTimeInputs[i].value).getTime();
    const endTime = new Date(endTimeInputs[i].value).getTime();

    if (isNaN(startTime) || isNaN(endTime) || endTime <= startTime) {
      Swal.fire({
        icon: "error",
        title: "資料錯誤",
        text: "結束時間必須在開始時間之後。",
        showConfirmButton: true,
      });
      isValid = false;
      break;
    }
  }

  if (!isValid) {
    return;
  }

  // Prepare to submit formData
  let formData = new FormData();
  formData.append("courseName", $("#course-name").val());
  formData.append("courseCategory", $("#course-category").val());
  formData.append("courseLocation", $("#course-location").val());
  formData.append("courseWay", $("#course-way").val());
  formData.append("courseLanguage", $("#course-language").val());
  formData.append("courseIntro", $("#course-intro").val());
  formData.append("courseReminder", $("#course-reminder").val());
  formData.append("courseImageFile", $("#course-image")[0].files[0]);

  let startTimeArr = [];
  if (startTime.length) {
    for (i = 0; i < startTime.length; i++) {
      startTimeArr.push(startTime[i].value);
    }
    for (var i = 0; i < startTime.length; i++) {
      formData.append("courses[" + i + "].startTime", startTimeArr[i]);
    }
  } else if (startTime === null) {
    formData.append("courses[0].startTime", null);
  } else {
    formData.append("courses[0].startTime", startTime.value);
  }

  let endTimeArr = [];
  if (endTime.length) {
    for (i = 0; i < endTime.length; i++) {
      endTimeArr.push(endTime[i].value);
    }
    for (var i = 0; i < endTime.length; i++) {
      formData.append("courses[" + i + "].endTime", endTimeArr[i]);
    }
  } else if (startTime === null) {
    formData.append("courses[0].endTime", null);
  } else {
    formData.append("courses[0].endTime", endTime.value);
  }

  let priceArr = [];
  if (price.length) {
    for (i = 0; i < price.length; i++) {
      priceArr.push(price[i].value);
    }
    for (var i = 0; i < price.length; i++) {
      formData.append("courses[" + i + "].price", priceArr[i]);
    }
  } else if (startTime === null) {
    formData.append("courses[0].price", null);
  } else {
    formData.append("courses[0].price", price.value);
  }

  const loadingImg = document.querySelector(".loading");
  const htmlBody = document.querySelector("html");
  htmlBody.style.backgroundColor = "black";
  htmlBody.style.opacity = "0.5";
  loadingImg.style.display = "flex";

  axios
    .post(host + endpoint, formData, config)
    .then((response) => {
      if (response.status === 200) {
        Swal.fire({
          icon: "success",
          title: "刊登成功",
          text: "您已成功刊登課程。",
          showConfirmButton: true,
        }).then((result) => {
          if (result.isConfirmed) {
            location.href = document.referrer;
          }
        });
      } else {
        return new Error("刊登失敗！");
      }
    })
    .catch((err) => {
      if (err.response.status === 403) {
        Swal.fire({
          icon: "error",
          title: "已有課程刊登中",
          text: "請勿重複刊登，若要更動請至編輯課程。",
          showConfirmButton: true,
        }).then((result) => {
          if (result.isConfirmed) {
            location.href = `${host}/teacher/edit-course.html`;
          }
        });
      } else if (err.response.status === 500) {
        console.log("Error code:500");
      }
      console.log(err);
    })
    .finally(() => {
      htmlBody.style.backgroundColor = "";
      htmlBody.style.opacity = "1";
      loadingImg.style.display = "none";
    });
});
