
// 1. Gestion du hover sur la liste de navigation et du responsive
document.addEventListener('DOMContentLoaded', function () {
    let list = document.querySelectorAll('.navigation li');
    if (list.length > 0) {
        function activeLink() {
            list.forEach((item) => {
                item.classList.remove('hovered');
                let img = item.querySelector('.icon img');
                if (img) {
                    img.classList.remove('iconHovered');
                }
            });
            this.classList.add('hovered');
            let img = this.querySelector('.icon img');
            if (img) {
                img.classList.add('iconHovered');
            }
        }
        list.forEach((item) => item.addEventListener('mouseover', activeLink));
    }