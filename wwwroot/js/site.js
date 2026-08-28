/* ==========================================
   Site JS — Sidebar, Toast, Modal
   ========================================== */
'use strict';

document.addEventListener('DOMContentLoaded', function () {

  // ── Sidebar ────────────────────────────────
  const sidebar  = document.getElementById('sidebar');
  const toggle   = document.getElementById('sidebarToggle');
  const mainContent = document.getElementById('mainContent');
  const backdrop = document.getElementById('sidebarBackdrop');

  function isMobile() { return window.innerWidth < 992; }

  function collapseSidebarDesktop() {
    sidebar.classList.toggle('sidebar--collapsed');
    const isCollapsed = sidebar.classList.contains('sidebar--collapsed');
    try { localStorage.setItem('sb', isCollapsed ? '1' : '0'); } catch(e){}
  }

  function openMobileSidebar() {
    sidebar.classList.add('sidebar--open');
    backdrop && backdrop.classList.add('sidebar-backdrop--visible');
    document.body.style.overflow = 'hidden';
  }

  function closeMobileSidebar() {
    sidebar.classList.remove('sidebar--open');
    backdrop && backdrop.classList.remove('sidebar-backdrop--visible');
    document.body.style.overflow = '';
  }

  if (sidebar && toggle) {
    // Restore desktop state
    try {
      if (!isMobile() && localStorage.getItem('sb') === '1') {
        sidebar.classList.add('sidebar--collapsed');
      }
    } catch(e){}

    toggle.addEventListener('click', function (e) {
      e.stopPropagation();
      if (isMobile()) {
        sidebar.classList.contains('sidebar--open') ? closeMobileSidebar() : openMobileSidebar();
      } else {
        collapseSidebarDesktop();
      }
    });

    backdrop && backdrop.addEventListener('click', closeMobileSidebar);

    sidebar.querySelectorAll('.sb-link').forEach(function (link) {
      link.addEventListener('click', function () {
        if (isMobile()) closeMobileSidebar();
      });
    });

    window.addEventListener('resize', debounce(function () {
      if (!isMobile()) {
        closeMobileSidebar();
        try {
          if (localStorage.getItem('sb') === '1') {
            sidebar.classList.add('sidebar--collapsed');
          } else {
            sidebar.classList.remove('sidebar--collapsed');
          }
        } catch(e){}
      } else {
        sidebar.classList.remove('sidebar--collapsed');
      }
    }, 200));

    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape' && isMobile() && sidebar.classList.contains('sidebar--open')) {
        closeMobileSidebar();
      }
    });
  }

  // ── Toast ───────────────────────────────────
  // Auto-dismiss tempdata toast
  const tempToast = document.getElementById('tempDataToast');
  if (tempToast) {
    setTimeout(function () {
      tempToast.style.transition = 'opacity 0.3s';
      tempToast.style.opacity = '0';
      setTimeout(function () { tempToast.remove(); }, 300);
    }, 4500);
  }
});

// ── Utilities ──────────────────────────────────
function debounce(fn, ms) {
  let t;
  return function () { clearTimeout(t); t = setTimeout(fn, ms); };
}

// ── Toast API ──────────────────────────────────
function showToast(message, type, duration) {
  type     = type     || 'info';
  duration = duration || 4000;

  const container = document.getElementById('toastContainer');
  if (!container) return;

  const icons = { success: 'fa-check-circle', error: 'fa-exclamation-circle', warning: 'fa-exclamation-triangle', info: 'fa-info-circle' };
  const icon  = icons[type] || icons.info;

  const el = document.createElement('div');
  el.className = 'toast toast--' + type;
  el.innerHTML =
    '<div class="toast__icon"><i class="fas ' + icon + '"></i></div>' +
    '<div class="toast__content"><p class="toast__message">' + message + '</p></div>' +
    '<button class="toast__close" aria-label="إغلاق"><i class="fas fa-times"></i></button>';

  el.querySelector('.toast__close').addEventListener('click', function () { removeToast(el); });
  container.appendChild(el);
  setTimeout(function () { removeToast(el); }, duration);
}

function removeToast(el) {
  el.style.transition = 'opacity 0.25s, transform 0.25s';
  el.style.opacity    = '0';
  el.style.transform  = 'translateX(20px)';
  setTimeout(function () { el && el.parentElement && el.parentElement.removeChild(el); }, 280);
}

function showSuccess(msg) { showToast(msg, 'success'); }
function showError(msg)   { showToast(msg, 'error'); }
function showWarning(msg) { showToast(msg, 'warning'); }
function showInfo(msg)    { showToast(msg, 'info'); }

// ── Modal API ──────────────────────────────────
function openModal(id) {
  const modal = document.getElementById(id);
  if (!modal) return;
  modal.classList.add('modal--open');
  document.body.style.overflow = 'hidden';

  const focusable = modal.querySelectorAll('button,[href],input,select,textarea,[tabindex]:not([tabindex="-1"])');
  if (focusable[0]) focusable[0].focus();

  function trap(e) {
    if (e.key === 'Escape') { closeModal(id); return; }
    if (e.key !== 'Tab' || !focusable.length) return;
    if (e.shiftKey) {
      if (document.activeElement === focusable[0]) { e.preventDefault(); focusable[focusable.length - 1].focus(); }
    } else {
      if (document.activeElement === focusable[focusable.length - 1]) { e.preventDefault(); focusable[0].focus(); }
    }
  }
  modal._trap = trap;
  modal.addEventListener('keydown', trap);
}

function closeModal(id) {
  const modal = document.getElementById(id);
  if (!modal) return;
  modal.classList.remove('modal--open');
  document.body.style.overflow = '';
  if (modal._trap) { modal.removeEventListener('keydown', modal._trap); delete modal._trap; }
}
