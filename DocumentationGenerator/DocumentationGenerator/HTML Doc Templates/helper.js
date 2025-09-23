function toggleMenu(id){
const menu = document.getElementById(id);
const button = menu.previousElementSibling; // assumes button is just before menu


if(menu.classList.contains("open")){
menu.classList.remove("open");
button.innerHTML = button.innerHTML.replace("▲", "▼").replace("▶", "▼");
} else {
menu.classList.add("open");
button.innerHTML = button.innerHTML.replace("▼", "▶");
}
}