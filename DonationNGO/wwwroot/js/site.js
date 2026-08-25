// ===== GIVE-AID MODERN FRONTEND JS =====
// Premium animations and interactions

document.addEventListener('DOMContentLoaded', function () {
    // Initialize all animations and effects
    initPageLoading();
    initNavbarEffects();
    initScrollAnimations();
    initSmoothScrolling();
    initMobileMenu();
    initCounterAnimation();
    initParallaxEffects();
    initFormAnimations();
});

// ===== PAGE LOADING ANIMATION =====
//function initPageLoading() {
//    const loadingScreen = document.createElement('div');
//    loadingScreen.className = 'page-loading';
//    loadingScreen.innerHTML = '<div class="loading-spinner"></div>';
//    document.body.appendChild(loadingScreen);

//    window.addEventListener('load', function () {
//        setTimeout(() => {
//            loadingScreen.classList.add('fade-out');
//            setTimeout(() => {
//                loadingScreen.remove();
//                document.body.classList.add('page-loaded');
//            }, 500);
//        }, 1000);
//    });
//}

// ===== NAVBAR EFFECTS =====
function initNavbarEffects() {
    const navbar = document.querySelector('.navbar');
    const navLinks = document.querySelectorAll('.nav-link');

    // Navbar background on scroll
    window.addEventListener('scroll', function () {
        if (window.scrollY > 100) {
            navbar.style.background = 'rgba(255, 255, 255, 0.95)';
            navbar.style.backdropFilter = 'blur(20px)';
            navbar.style.boxShadow = '0 4px 30px rgba(0, 0, 0, 0.1)';
        } else {
            navbar.style.background = 'rgba(255, 255, 255, 0.08)';
            navbar.style.boxShadow = 'none';
        }
    });

    // Active link highlighting
    navLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            navLinks.forEach(l => l.classList.remove('active'));
            this.classList.add('active');
        });
    });

    // Hover effects with delay
    navLinks.forEach(link => {
        link.addEventListener('mouseenter', function () {
            this.style.transition = 'all 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94)';
        });
    });
}

// ===== SCROLL ANIMATIONS =====
function initScrollAnimations() {
    // Initialize AOS if available, otherwise use custom animations
    if (typeof AOS !== 'undefined') {
        AOS.init({
            duration: 800,
            easing: 'ease-in-out',
            once: true,
            mirror: false
        });
    } else {
        // Custom scroll animations
        const animatedElements = document.querySelectorAll('.fade-in, .fade-in-left, .fade-in-right');

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1 });

        animatedElements.forEach(el => {
            el.style.opacity = '0';
            el.style.transform = 'translateY(50px)';
            el.style.transition = 'all 0.8s cubic-bezier(0.25, 0.46, 0.45, 0.94)';
            observer.observe(el);
        });
    }

    // Parallax effect for hero section
    window.addEventListener('scroll', function () {
        const scrolled = window.pageYOffset;
        const hero = document.querySelector('.hero-section');
        if (hero) {
            hero.style.transform = `translateY(${scrolled * 0.5}px)`;
        }
    });
}

// ===== SMOOTH SCROLLING =====
function initSmoothScrolling() {
    const links = document.querySelectorAll('a[href^="#"]');

    links.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();

            const targetId = this.getAttribute('href');
            if (targetId === '#') return;

            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                const offsetTop = targetElement.offsetTop - 80;

                window.scrollTo({
                    top: offsetTop,
                    behavior: 'smooth'
                });
            }
        });
    });
}

// ===== MOBILE MENU TOGGLE =====
function initMobileMenu() {
    const navbarToggler = document.querySelector('.navbar-toggler');
    const navbarCollapse = document.querySelector('.navbar-collapse');

    if (navbarToggler && navbarCollapse) {
        navbarToggler.addEventListener('click', function () {
            navbarCollapse.classList.toggle('show');

            // Animate hamburger icon
            this.classList.toggle('active');
        });
    }

    // Close mobile menu when clicking on a link
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
    navLinks.forEach(link => {
        link.addEventListener('click', () => {
            if (navbarCollapse.classList.contains('show')) {
                navbarCollapse.classList.remove('show');
                navbarToggler.classList.remove('active');
            }
        });
    });
}

// ===== COUNTER ANIMATION =====
function initCounterAnimation() {
    const counters = document.querySelectorAll('.counter');

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                animateCounter(entry.target);
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.5 });

    counters.forEach(counter => observer.observe(counter));
}

function animateCounter(counter) {
    const target = +counter.getAttribute('data-target');
    const duration = 2000;
    const step = target / (duration / 16);
    let current = 0;

    const timer = setInterval(() => {
        current += step;
        if (current >= target) {
            counter.textContent = target.toLocaleString();
            clearInterval(timer);
        } else {
            counter.textContent = Math.floor(current).toLocaleString();
        }
    }, 16);
}

// ===== PARALLAX EFFECTS =====
function initParallaxEffects() {
    const parallaxElements = document.querySelectorAll('.parallax');

    window.addEventListener('scroll', function () {
        const scrolled = window.pageYOffset;

        parallaxElements.forEach(element => {
            const speed = element.getAttribute('data-speed') || 0.5;
            element.style.transform = `translateY(${scrolled * speed}px)`;
        });
    });
}

// ===== FORM ANIMATIONS =====
function initFormAnimations() {
    const formInputs = document.querySelectorAll('.form-control');

    formInputs.forEach(input => {
        // Focus effects
        input.addEventListener('focus', function () {
            this.parentElement.classList.add('focused');
        });

        input.addEventListener('blur', function () {
            if (!this.value) {
                this.parentElement.classList.remove('focused');
            }
        });

        // Label animation
        const label = this.querySelector('label');
        if (label && input.value) {
            input.parentElement.classList.add('focused');
        }
    });

    // Form submission animations
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Processing...';
                submitBtn.disabled = true;
            }
        });
    });
}

// ===== GLASS CARD HOVER EFFECTS =====
function initGlassCardEffects() {
    const glassCards = document.querySelectorAll('.glass-card, .card, .dashboard-card');

    glassCards.forEach(card => {
        card.addEventListener('mousemove', function (e) {
            const rect = this.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            const centerX = rect.width / 2;
            const centerY = rect.height / 2;

            const angleY = (x - centerX) / 25;
            const angleX = (centerY - y) / 25;

            this.style.transform = `perspective(1000px) rotateX(${angleX}deg) rotateY(${angleY}deg) translateY(-10px)`;
        });

        card.addEventListener('mouseleave', function () {
            this.style.transform = 'perspective(1000px) rotateX(0) rotateY(0) translateY(0)';
        });
    });
}

// ===== THEME TOGGLE (OPTIONAL) =====
function initThemeToggle() {
    const themeToggle = document.createElement('button');
    themeToggle.className = 'btn btn-outline-primary theme-toggle';
    themeToggle.innerHTML = '<i class="fas fa-moon"></i>';
    themeToggle.style.position = 'fixed';
    themeToggle.style.bottom = '20px';
    themeToggle.style.right = '20px';
    themeToggle.style.zIndex = '1000';

    document.body.appendChild(themeToggle);

    themeToggle.addEventListener('click', function () {
        document.body.classList.toggle('dark-theme');
        const icon = this.querySelector('i');
        if (document.body.classList.contains('dark-theme')) {
            icon.className = 'fas fa-sun';
        } else {
            icon.className = 'fas fa-moon';
        }
    });
}

// ===== UTILITY FUNCTIONS =====
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Initialize all effects when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    initPageLoading();
    initNavbarEffects();
    initScrollAnimations();
    initSmoothScrolling();
    initMobileMenu();
    initCounterAnimation();
    initParallaxEffects();
    initFormAnimations();
    initGlassCardEffects();
    initThemeToggle();
});

// Reinitialize on page changes (for SPA-like behavior)
window.addEventListener('pageshow', function () {
    initScrollAnimations();
    initCounterAnimation();
});