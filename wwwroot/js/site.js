/* ==========================================
   Site JS — Sidebar, Toast, Modal
   ========================================== */
'use strict';

document.addEventListener('DOMContentLoaded', function () {

  // ── Sidebar ────────────────────────────────
  const sidebar  = document.getElementById('sidebar');
  const toggle   = document.getElementById('sidebarToggle');
  const closeButton = document.getElementById('sidebarClose');
  const backdrop = document.getElementById('sidebarBackdrop');
  let lastFocusedElement = null;

  function isMobile() { return window.innerWidth < 992; }

  function syncSidebarState() {
    if (!sidebar || !toggle) return;
    const isOpen = sidebar.classList.contains('sidebar--open');
    const isCollapsed = sidebar.classList.contains('sidebar--collapsed');
    const expanded = isMobile() ? isOpen : !isCollapsed;
    toggle.setAttribute('aria-expanded', String(expanded));
    toggle.setAttribute('aria-label', isMobile()
      ? (isOpen ? 'إغلاق القائمة الرئيسية' : 'فتح القائمة الرئيسية')
      : (isCollapsed ? 'توسيع القائمة الرئيسية' : 'تصغير القائمة الرئيسية'));
    sidebar.setAttribute('aria-hidden', isMobile() && !isOpen ? 'true' : 'false');
  }

  function collapseSidebarDesktop() {
    sidebar.classList.toggle('sidebar--collapsed');
    const isCollapsed = sidebar.classList.contains('sidebar--collapsed');
    try { localStorage.setItem('sb', isCollapsed ? '1' : '0'); } catch(e){}
    syncSidebarState();
  }

  function openMobileSidebar() {
    lastFocusedElement = document.activeElement;
    sidebar.classList.add('sidebar--open');
    backdrop && backdrop.classList.add('sidebar-backdrop--visible');
    document.body.style.overflow = 'hidden';
    syncSidebarState();
    window.requestAnimationFrame(function () {
      const firstTarget = closeButton || sidebar.querySelector('.sb-link');
      firstTarget && firstTarget.focus();
    });
  }

  function closeMobileSidebar(restoreFocus) {
    sidebar.classList.remove('sidebar--open');
    backdrop && backdrop.classList.remove('sidebar-backdrop--visible');
    document.body.style.overflow = '';
    syncSidebarState();
    if (restoreFocus !== false && lastFocusedElement && typeof lastFocusedElement.focus === 'function') {
      lastFocusedElement.focus();
    }
  }

  if (sidebar && toggle) {
    // Restore desktop state
    try {
      if (!isMobile() && localStorage.getItem('sb') === '1') {
        sidebar.classList.add('sidebar--collapsed');
      }
    } catch(e){}
    syncSidebarState();

    toggle.addEventListener('click', function (e) {
      e.stopPropagation();
      if (isMobile()) {
        sidebar.classList.contains('sidebar--open') ? closeMobileSidebar(true) : openMobileSidebar();
      } else {
        collapseSidebarDesktop();
      }
    });

    backdrop && backdrop.addEventListener('click', function () { closeMobileSidebar(true); });
    closeButton && closeButton.addEventListener('click', function () { closeMobileSidebar(true); });

    sidebar.querySelectorAll('.sb-link').forEach(function (link) {
      link.addEventListener('click', function () {
        if (isMobile()) closeMobileSidebar(false);
      });
    });

    window.addEventListener('resize', debounce(function () {
      if (!isMobile()) {
        closeMobileSidebar(false);
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
      syncSidebarState();
    }, 200));

    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape' && isMobile() && sidebar.classList.contains('sidebar--open')) {
        closeMobileSidebar(true);
      }

      if (e.key === 'Tab' && isMobile() && sidebar.classList.contains('sidebar--open')) {
        const focusable = Array.from(sidebar.querySelectorAll('a[href], button:not([disabled])'));
        if (!focusable.length) return;
        const first = focusable[0];
        const last = focusable[focusable.length - 1];
        if (e.shiftKey && document.activeElement === first) {
          e.preventDefault();
          last.focus();
        } else if (!e.shiftKey && document.activeElement === last) {
          e.preventDefault();
          first.focus();
        }
      }
    });
  }

  // ── Toast ───────────────────────────────────
  // Auto-dismiss tempdata toast
  document.querySelectorAll('.tempdata-toast').forEach(function (tempToast) {
    setTimeout(function () {
      tempToast.style.transition = 'opacity 0.3s';
      tempToast.style.opacity = '0';
      setTimeout(function () { tempToast.remove(); }, 300);
    }, 4500);
  });
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
  el.setAttribute('role', type === 'error' ? 'alert' : 'status');

  const iconWrap = document.createElement('div');
  iconWrap.className = 'toast__icon';
  iconWrap.setAttribute('aria-hidden', 'true');
  const iconElement = document.createElement('i');
  iconElement.className = 'fas ' + icon;
  iconWrap.appendChild(iconElement);

  const content = document.createElement('div');
  content.className = 'toast__content';
  const text = document.createElement('p');
  text.className = 'toast__message';
  text.textContent = message;
  content.appendChild(text);

  const close = document.createElement('button');
  close.type = 'button';
  close.className = 'toast__close';
  close.setAttribute('aria-label', 'إغلاق التنبيه');
  const closeIcon = document.createElement('i');
  closeIcon.className = 'fas fa-times';
  closeIcon.setAttribute('aria-hidden', 'true');
  close.appendChild(closeIcon);

  el.append(iconWrap, content, close);

  close.addEventListener('click', function () { removeToast(el); });
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
