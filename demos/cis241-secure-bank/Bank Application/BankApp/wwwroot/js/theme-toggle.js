(function () {
    const body = document.body;
    const fab = document.getElementById("themeToggleFab");
    if (!fab) return;

    const iconSpan = fab.querySelector(".theme-icon");

    function applyTheme(theme) {
        if (theme === "light") {
            body.classList.remove("theme-dark");
            body.classList.add("theme-light");
            iconSpan.textContent = "☀️";
        } else {
            body.classList.remove("theme-light");
            body.classList.add("theme-dark");
            iconSpan.textContent = "🌙";
        }
    }

    const saved = localStorage.getItem("aurora-theme");
    applyTheme(saved === "light" ? "light" : "dark");

    fab.addEventListener("click", () => {
        const newTheme = body.classList.contains("theme-light")
            ? "dark"
            : "light";

        applyTheme(newTheme);
        localStorage.setItem("aurora-theme", newTheme);
    });
})();
