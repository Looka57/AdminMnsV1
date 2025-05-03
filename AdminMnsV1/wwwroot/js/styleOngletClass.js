
function openTab(x) {
    let contents = document.querySelectorAll(".tabContent")
    for (var i = 0; i < contents.length; i++) {
        contents[i].style.display = "none";

    }
    contents[x].style.display = "block";

}
