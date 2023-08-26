const host = "http://localhost:5202";
const endpoint = "/api/course/";

const jwt = localStorage.getItem("JWT");

const config = {
  headers: {
    Authorization: "Bearer " + jwt,
  },
};

const urlParams = new URLSearchParams(window.location.search);
const roomId = urlParams.get("id");

const courseContainer = document.querySelector(".course-container");

axios
  .get(host + endpoint + roomId, config)
  .then((response) => {
    console.log(response);
    return response.data;
  })
  .then((course) => {
    renderCourseDetail(course);
  })
  .catch((err) => console.log(err));

function renderCourseDetail(course) {
  courseContainer.innerHTML = `
  <div class="background">
        <div class="row">
          <div class="course-image-section col-4">
            <img src="../assets/images/homepage-image.jpg" alt="" />
          </div>
          <div class="course-information-section col-8">
            <div class="course-detail">
              <ul>
                <li>課程名稱：${course.courseName}</li>
                <li>開始時間：${course.startTime}</li>
                <li>結束時間：${course.endTime}</li>
                <li>上課方式：${course.courseWay}</li>
                <li>上課地點：${course.courseLocation}</li>
                <li>注意事項：${course.courseReminder}</li>
              </ul>
            </div>
            <div class="room-link-container">
              <a href="./course/online.html?id=${course.roomId}"
                ><button class="btn room-link">進入課程</button></a
              >
            </div>
          </div>
        </div>
      </div>
  `;
}
