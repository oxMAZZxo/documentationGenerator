// helper.js
// Simple toggle for sidebar menus.
function toggleMenu(id) {
    const menu = document.getElementById(id);
    if (!menu) return;

    const btn = menu.previousElementSibling;
    const arrow = btn && btn.querySelector('.arrow');

    if (menu.style.display === 'flex' || menu.classList.contains('open')) {
        menu.style.display = 'none';
        menu.classList.remove('open');
        if (arrow) arrow.textContent = '▼';
    } else {
        menu.style.display = 'flex';
        menu.classList.add('open');
        if (arrow) arrow.textContent = '▶';
    }
}

// Optional: allow Enter / Space to toggle when the button is focused
document.addEventListener('keydown', function (e) {
    const el = document.activeElement;
    if (!el) return;
    if ((e.key === 'Enter' || e.key === ' ') && el.matches('button[onclick^="toggleMenu"]')) {
        e.preventDefault();
        // extract id string from onclick attribute, e.g. toggleMenu('classes')
        const onclick = el.getAttribute('onclick') || '';
        const m = onclick.match(/toggleMenu\(['"]([^'"]+)['"]\)/);
        if (m) toggleMenu(m[1]);
    }
});