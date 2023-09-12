window.onload = function countup() {
  const counters = document.querySelectorAll(".counter");

  counters.forEach((counter) => {
    const startValue = 0;
    const endValue = counter.textContent;
    const duration = 2;

    const countUp = new CountUp(counter, startValue, endValue, 0, duration);

    if (!countUp.error) {
      countUp.start();
    } else {
      console.error(countUp.error);
    }
  });
};
