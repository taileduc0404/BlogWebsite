window.onscroll = function () {
    var moveToTopBtn = document.getElementById("moveToTopBtn");
    if (document.body.scrollTop > 200 || document.documentElement.scrollTop > 200) {
        moveToTopBtn.style.display = "block";
    } else {
        moveToTopBtn.style.display = "none";
    }
};

// Scroll to the top of the page
function moveToTop() {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
}   