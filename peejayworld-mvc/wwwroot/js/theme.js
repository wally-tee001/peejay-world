// Theme toggle initializer
(function() {
    function initThemeToggle() {
        const root = document.documentElement;
        const btn = document.getElementById('theme-toggle');
        const saved = localStorage.getItem('theme');
        const initial = saved || 'light';
        if (initial === 'dark') {
            root.setAttribute('data-theme', 'dark');
            if (btn) btn.textContent = '☀️';
        } else {
            root.removeAttribute('data-theme');
            if (btn) btn.textContent = '🌙';
        }

        function toggleTheme() {
            const current = root.getAttribute('data-theme') === 'dark' ? 'dark' : 'light';
            const next = current === 'dark' ? 'light' : 'dark';
            if (next === 'dark') {
                root.setAttribute('data-theme', 'dark');
                if (btn) btn.textContent = '☀️';
            } else {
                root.removeAttribute('data-theme');
                if (btn) btn.textContent = '🌙';
            }
            localStorage.setItem('theme', next);
        }

        window.toggleTheme = toggleTheme;

        if (btn) {
            btn.addEventListener('click', toggleTheme);
            btn.addEventListener('keydown', function(e) {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    toggleTheme();
                }
            });
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initThemeToggle);
    } else {
        initThemeToggle();
    }
})();
