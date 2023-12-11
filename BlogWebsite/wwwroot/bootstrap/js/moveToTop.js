//Get the button
let mybutton = document.getElementById("moveToTopBtn");

// When the user scrolls down 20px from the top of the document, show the button
window.onscroll = function () {
    scrollFunction();
};

function scrollFunction() {
    if (
        document.body.scrollTop > 200 ||
        document.documentElement.scrollTop > 200
    ) {
        mybutton.style.display = "block";
    } else {
        mybutton.style.display = "none";
    }
}
// When the user clicks on the button, scroll to the top of the document
mybutton.addEventListener("click", moveToTop);

function moveToTop() {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
}

/*Lê Đức Tài, Bùi Ngọc Na*/








































/*Lê Đức Tài, Bùi Ngọc Na*/

