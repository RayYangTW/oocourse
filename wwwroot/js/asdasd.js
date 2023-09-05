const formContainer = document.querySelector(".form-container");
function renderTeacherApplication(defaultData) {
  formContainer.innerHTML = `
  <h1 class="text-center">教師資格申請</h1>
      <form id="teacher-application-form">
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="name"
            placeholder="姓名"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="email"
            placeholder="電子信箱"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="country"
            placeholder="國籍"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="location"
            placeholder="所在地"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="language"
            placeholder="溝通語言"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="category"
            placeholder="課程類別"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control item"
            id="experience"
            placeholder="教學經驗"
            required
          />
        </div>
        <div class="form-group">
          <textarea
            type="text"
            class="form-control item"
            id="description"
            placeholder="個人特色"
            required
          ></textarea>
        </div>
        <div class="form-group">
          <label for="certification-file" class="form-label">相關證照</label>
          <input
            type="file"
            class="form-control item"
            id="certification-file"
            accept=".pdf,.png,.jpg,.jpeg"
            multiple
          />
        </div>
        <div class="form-group text-center">
          <button type="submit" class="btn btn-block apply">送出申請</button>
        </div>
      </form>
  `;
}
