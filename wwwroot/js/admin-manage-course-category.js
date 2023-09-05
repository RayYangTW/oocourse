import { host } from "./config.js";
const endpoint = "/api/admin/courseCategory";

const jwt = localStorage.getItem("JWT");
const config = {
  headers: {
    Authorization: `Bearer ${jwt}`,
  },
};

const addCategoryForm = $("#add-category-form");
const tableContainer = document.querySelector(".table-container");

axios
  .get(host + endpoint, config)
  .then((response) => {
    console.log(response);
    return response.data;
  })
  .then((categoryData) => {
    renderCategories(categoryData);
  })
  .catch((err) => console.log(err));

function renderCategories(categoryData) {
  tableContainer.innerHTML = `
    <table class="table table-hover">
      <thead>
        <tr>
          <th>Id</th>
          <th>類別</th>
        </tr>
      </thead>
      <tbody>
        ${categoryData.map(getCategoryHTML).join("")}
      </tbody>
    </table>
  `;
}

function getCategoryHTML(category) {
  return `
    <tr>
      <td>${category.id}</td>
      <td>${category.category}</td>
    </tr>
  `;
}

addCategoryForm.submit((e) => {
  e.preventDefault();

  const category = $("#category").val();

  const data = {
    category: category,
  };

  axios
    .post(host + endpoint, data, config)
    .then((response) => {
      alert("新增成功！");
      location.reload();
    })
    .catch((err) => {
      console.error(err);
      if (err.response.status === 409) {
        return alert("類別重複！");
      }
      return alert("新增失敗！");
    });
});
