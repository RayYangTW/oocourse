// Search
document
  .getElementById("searchForm")
  .addEventListener("submit", function (event) {
    event.preventDefault(); // 阻止表單的預設提交行為
    const searchKeyword = document
      .getElementById("main-search-input")
      .value.trim();
    location.href = `/course/search.html?keyword=${searchKeyword}`;
  });
