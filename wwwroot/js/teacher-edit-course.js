import { host } from "./config.js";
const endpoint = "/api/teacher/getTeacherFormData/";
const updateApi = "/api/teacher/updateCourse";

const jwt = localStorage.getItem("JWT");

const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};

const courseFormContainer = document.querySelector(".course-form-container");

function renderForm(course) {
  courseFormContainer.innerHTML = `
    <h1 class="text-center">編輯課程、增加時段</h1>
    <form id="teacher-application-form" enctype="multipart/form-data">
        <div class="form-group">
          <label for="course-name" class="form-label">課程名稱<span class="required-star"> *</span></label>
          <input
            type="text"
            class="form-control item"
            id="course-name"
            value="${course.courseName}"
            placeholder="課程名稱"
            required
          />
        </div>
        <div class="form-group">
          <label for="course-category" class="form-label">課程分類<span class="required-star"> *</span></label>
          <input
            type="text"
            class="form-control item"
            id="course-category"
            value="${course.courseCategory}"
            placeholder="課程分類"
            required
          />
        </div>
        <div class="form-group">
          <label for="course-way" class="form-label">授課方式(實體/線上)<span class="required-star"> *</span></label>
          <select class="form-control item" id="course-way" name="course-way" required>
            <option value="實體" ${
              course.courseWay === "實體" ? "selected" : ""
            }>實體</option>
            <option value="線上" ${
              course.courseWay === "線上" ? "selected" : ""
            }>線上</option>
          </select>
        </div>
        <div class="form-group">
          <label for="course-language" class="form-label">授課語言<span class="required-star"> *</span></label>
          <input
            type="text"
            class="form-control item"
            id="course-language"
            value="${course.courseLanguage}"
            placeholder="授課語言"
            required
          />
        </div>
        <div class="form-group">
          <label for="course-location" class="form-label">授課地點<span class="required-star"> *</span></label>
          <input
            type="text"
            class="form-control item"
            id="course-location"
            value="${course.courseLocation}"
            placeholder="授課地點"
            required
          />
        </div>
        <div class="form-group">
          <label for="course-intro" class="form-label">課程詳細介紹<span class="required-star"> *</span></label>
          <textarea
            type="text"
            class="form-control item"
            id="course-intro"
            placeholder="課程詳細介紹"
            required
          >${course.courseIntro}</textarea>
        </div>
        <div class="form-group">
          <label for="course-reminder" class="form-label">課程注意事項<span class="required-star"> *</span></label>
          <textarea
            type="text"
            class="form-control item"
            id="course-reminder"
            placeholder="課程注意事項"
            required
          >${course.courseReminder}</textarea>
        </div>
        <div class="form-group">
          <label for="course-image" class="form-label">課程宣傳照</label>
          <input
            type="file"
            class="form-control item"
            id="course-image"
            accept=",.png,.jpg,.jpeg"
          />
        </div>
        <div class="course-time-container">
          <label for="course-time" class="form-label">課程時段</label>
          <div class="row form-row">
            <div class="col-4">
              <label for="startTime">開始時間</label>
              <input
                type="datetime-local"
                class="form-control"
                id="startTime"
                name="startTime"
              />
            </div>
            <div class="col-4">
              <label for="endTime">結束時間</label>
              <input
                type="datetime-local"
                class="form-control"
                id="endTime"
                name="endTime"
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
            <div
              class="col-2 d-flex text-center align-items-center justify-content-center"
            >
              <button type="button" class="btn btn-primary" id="add-input">
                新增
              </button>
            </div>
          </div>
        </div>
        <div class="form-group text-center btn-container">
          <button type="submit" class="btn btn-block publish">送出</button>
        </div>
      </form>
  `;
}

function generateTimeBlock() {
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
}

function submitEditForm() {
  $("#teacher-application-form").submit((e) => {
    e.preventDefault();

    // Validate if endTime >= startTime
    const startTimeInputs = document.querySelectorAll(
      "input[name='startTime']"
    );
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

    console.log(...formData);

    const loadingImg = document.querySelector(".loading");
    const htmlBody = document.querySelector("html");
    htmlBody.style.backgroundColor = "black";
    htmlBody.style.opacity = "0.5";
    loadingImg.style.display = "flex";

    axios
      .put(host + updateApi, formData, config)
      .then((response) => {
        if (response.status === 200) {
          Swal.fire({
            icon: "success",
            title: "更新成功",
            text: "您已成功編輯課程。",
            showConfirmButton: true,
          }).then((result) => {
            if (result.isConfirmed) {
              location.reload();
            }
          });
        } else {
          return new Error("刊登失敗！");
        }
      })
      .catch((err) => {
        Swal.fire({
          icon: "error",
          title: "更新失敗",
          text: "請確認資料是否填寫正確。",
          showConfirmButton: true,
        }).then((result) => {
          if (result.isConfirmed) {
            location.href = document.referrer;
          }
        });
        console.log(err);
      })
      .finally(() => {
        htmlBody.style.backgroundColor = "";
        htmlBody.style.opacity = "1";
        loadingImg.style.display = "none";
      });
  });
}

/***************************
 * Render form
 ***************************/
axios
  .get(host + endpoint, config)
  .then((response) => {
    return response.data;
  })
  .then((course) => {
    renderForm(course);
    generateTimeBlock();
    submitEditForm();
  })
  .catch((err) => console.log(err));
