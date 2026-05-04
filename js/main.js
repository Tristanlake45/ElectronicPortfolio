const header = document.querySelector(".site-header");

let lastScrollY = window.scrollY;
let ticking = false;

function updateHeader() {
  const currentScrollY = window.scrollY;
  const scrollingDown = currentScrollY > lastScrollY;
  const pastHeader = currentScrollY > 90;

  if (pastHeader) {
    header.classList.add("nav-scrolled");
  } else {
    header.classList.remove("nav-scrolled");
  }

  if (scrollingDown && currentScrollY > 140) {
    header.classList.add("nav-hidden");
  } else {
    header.classList.remove("nav-hidden");
  }

  lastScrollY = currentScrollY;
  ticking = false;
}

window.addEventListener("scroll", () => {
  if (!ticking) {
    window.requestAnimationFrame(updateHeader);
    ticking = true;
  }
});