// Very small helpers:
//  - toggleMenu(id): simple show/hide and arrow flip (▼ -> ▶)
//  - toggleMember(button): expand/collapse member details

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

// Toggle a member entry when its header/button is clicked.
// The button is passed via "this" in markup.
function toggleMember(btn) {
    const member = btn.closest('.member');
    if (!member) return;
    const isOpen = member.classList.toggle('open');

    // simple visual indicator: replace the leading diamond with filled/empty (or rotate)
    if (isOpen) {
        btn.textContent = btn.textContent.replace('◈', '◆');
    } else {
        btn.textContent = btn.textContent.replace('◆', '◈');
    }
}

// Optional: make Enter/Space trigger the same as click on keyboard-focusable toggles
document.addEventListener('keydown', (e) => {
    const target = e.target;
    if ((e.key === 'Enter' || e.key === ' ') && target.classList.contains('member-toggle')) {
        e.preventDefault();
        toggleMember(target);
    }
});