/**
 * Advanced Interactions for Courses Page
 * Handles smooth animations, scroll effects, and enhanced UX
 */

class CoursesPageEnhancer {
    constructor() {
        this.init();
        this.setupEventListeners();
        this.setupIntersectionObservers();
        this.initializeAnimations();
    }

    init() {
        // Add CSS classes for enhanced functionality
        document.body.classList.add('courses-enhanced');

        // Initialize AOS library if available
        if (typeof AOS !== 'undefined') {
            AOS.init({
                duration: 800,
                easing: 'ease-out-cubic',
                offset: 100,
                once: true,
                disable: window.matchMedia('(prefers-reduced-motion: reduce)').matches
            });
        }
    }

    setupEventListeners() {
        // Enhanced search with debouncing
        const searchInput = document.querySelector('.search-input input');
        if (searchInput) {
            this.debounce(searchInput, 'input', this.handleSearchInput.bind(this), 300);
        }

        // Smooth form interactions
        const formElements = document.querySelectorAll('.form-input, .form-select');
        formElements.forEach(element => {
            element.addEventListener('focus', this.handleFocusIn.bind(this));
            element.addEventListener('blur', this.handleFocusOut.bind(this));
        });

        // Enhanced button interactions
        const buttons = document.querySelectorAll('.btn');
        buttons.forEach(button => {
            this.addRippleEffect(button);
            button.addEventListener('mouseenter', this.handleButtonHover.bind(this));
        });

        // Course card interactions
        const courseCards = document.querySelectorAll('.course-card');
        courseCards.forEach(card => {
            this.enhanceCourseCard(card);
        });

        // Smooth scrolling for pagination
        const paginationButtons = document.querySelectorAll('.btn-pagination');
        paginationButtons.forEach(button => {
            button.addEventListener('click', this.handlePaginationClick.bind(this));
        });

        // Keyboard navigation
        document.addEventListener('keydown', this.handleKeyboardNavigation.bind(this));

        // Responsive behavior
        window.addEventListener('resize', this.debounce(null, null, this.handleResize.bind(this), 250));
    }

    setupIntersectionObservers() {
        // Lazy loading for course images
        const imageObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    if (img.dataset.src) {
                        img.src = img.dataset.src;
                        img.classList.add('loaded');
                        imageObserver.unobserve(img);
                    }
                }
            });
        }, { rootMargin: '50px' });

        const courseImages = document.querySelectorAll('.course-image[data-src]');
        courseImages.forEach(img => imageObserver.observe(img));

        // Animate elements on scroll
        const animationObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-in');
                    // Stagger animation for grid items
                    const gridItems = entry.target.querySelectorAll('.course-card');
                    gridItems.forEach((item, index) => {
                        setTimeout(() => {
                            item.classList.add('animate-in');
                        }, index * 100);
                    });
                }
            });
        }, { threshold: 0.1 });

        const animatedSections = document.querySelectorAll('.courses-grid, .filters-card, .results-header');
        animatedSections.forEach(section => animationObserver.observe(section));
    }

    initializeAnimations() {
        // Add initial animation states
        const elementsToAnimate = document.querySelectorAll('.course-card, .filter-group');
        elementsToAnimate.forEach((element, index) => {
            element.style.animationDelay = `${index * 50}ms`;
        });

        // Typing animation for page title
        this.typeWriterEffect('.page-title');

        // Counter animation for results
        this.animateCounters();
    }

    // Utility Functions
    debounce(element, event, func, wait) {
        let timeout;
        const debouncedFunction = function () {
            const context = this;
            const args = arguments;
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(context, args), wait);
        };

        if (element && event) {
            element.addEventListener(event, debouncedFunction);
        }
        return debouncedFunction;
    }

    addRippleEffect(button) {
        button.classList.add('ripple');

        button.addEventListener('click', function (e) {
            const rect = this.getBoundingClientRect();
            const ripple = document.createElement('span');
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.classList.add('ripple-effect');

            this.appendChild(ripple);

            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    }

    typeWriterEffect(selector) {
        const element = document.querySelector(selector);
        if (!element) return;

        const text = element.textContent;
        element.textContent = '';
        element.style.borderRight = '3px solid var(--primary-color)';

        let index = 0;
        const timer = setInterval(() => {
            if (index < text.length) {
                element.textContent += text.charAt(index);
                index++;
            } else {
                clearInterval(timer);
                // Blinking cursor effect
                setInterval(() => {
                    element.style.borderRightColor =
                        element.style.borderRightColor === 'transparent'
                            ? 'var(--primary-color)'
                            : 'transparent';
                }, 750);
            }
        }, 100);
    }

    animateCounters() {
        const counters = document.querySelectorAll('.count-number');
        counters.forEach(counter => {
            const target = parseInt(counter.textContent);
            let current = 0;
            const increment = target / 50;

            const timer = setInterval(() => {
                current += increment;
                if (current >= target) {
                    current = target;
                    clearInterval(timer);
                }
                counter.textContent = Math.floor(current);
            }, 30);
        });
    }

    enhanceCourseCard(card) {
        // 3D tilt effect
        card.addEventListener('mousemove', (e) => {
            if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;

            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            const centerX = rect.width / 2;
            const centerY = rect.height / 2;

            const rotateX = (y - centerY) / 10;
            const rotateY = (centerX - x) / 10;

            card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) translateZ(20px)`;
        });

        card.addEventListener('mouseleave', () => {
            card.style.transform = 'perspective(1000px) rotateX(0deg) rotateY(0deg) translateZ(0px)';
        });

        // Enhanced image loading
        const img = card.querySelector('.course-image');
        if (img) {
            img.addEventListener('load', () => {
                img.classList.add('loaded');
            });
        }

        // Accessibility improvements
        card.setAttribute('tabindex', '0');
        card.addEventListener('keypress', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                const link = card.querySelector('.course-button');
                if (link) link.click();
            }
        });
    }

    // Event Handlers
    handleSearchInput(e) {
        const searchTerm = e.target.value;
        const searchIcon = e.target.parentElement.querySelector('i');

        if (searchTerm.length > 0) {
            searchIcon.style.transform = 'translateY(-50%) rotate(90deg) scale(1.1)';
            e.target.classList.add('has-content');
        } else {
            searchIcon.style.transform = 'translateY(-50%) rotate(0deg) scale(1)';
            e.target.classList.remove('has-content');
        }

        // Add subtle glow effect
        e.target.style.boxShadow = searchTerm.length > 0
            ? '0 0 20px rgba(99, 102, 241, 0.1)'
            : '';
    }

    handleFocusIn(e) {
        const parent = e.target.closest('.filter-group');
        if (parent) {
            parent.classList.add('focused');
            parent.style.transform = 'translateY(-2px)';
        }
    }

    handleFocusOut(e) {
        const parent = e.target.closest('.filter-group');
        if (parent) {
            parent.classList.remove('focused');
            parent.style.transform = 'translateY(0px)';
        }
    }

    handleButtonHover(e) {
        if (e.target.classList.contains('btn-primary')) {
            const rect = e.target.getBoundingClientRect();
            const mouseX = e.clientX - rect.left;
            const mouseY = e.clientY - rect.top;

            e.target.style.background = `radial-gradient(circle at ${mouseX}px ${mouseY}px, rgba(255,255,255,0.1), transparent 50%)`;
        }
    }

    handlePaginationClick(e) {
        e.preventDefault();

        // Smooth scroll to top of results
        const resultsSection = document.querySelector('.courses-grid');
        if (resultsSection) {
            resultsSection.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }

        // Add loading state
        const button = e.target;
        button.classList.add('loading');

        setTimeout(() => {
            button.classList.remove('loading');
        }, 1000);
    }

    handleKeyboardNavigation(e) {
        // Enhanced keyboard navigation
        const focusableElements = document.querySelectorAll(
            '.course-card, .btn, .form-input, .form-select, a[href]'
        );

        const currentIndex = Array.from(focusableElements).indexOf(document.activeElement);

        if (e.key === 'ArrowDown' || (e.key === 'Tab' && !e.shiftKey)) {
            e.preventDefault();
            const nextIndex = (currentIndex + 1) % focusableElements.length;
            focusableElements[nextIndex].focus();
        } else if (e.key === 'ArrowUp' || (e.key === 'Tab' && e.shiftKey)) {
            e.preventDefault();
            const prevIndex = currentIndex === 0 ? focusableElements.length - 1 : currentIndex - 1;
            focusableElements[prevIndex].focus();
        }
    }

    handleResize() {
        // Recalculate animations on resize
        if (typeof AOS !== 'undefined') {
            AOS.refresh();
        }

        // Adjust grid layout if needed
        const coursesGrid = document.querySelector('.courses-grid');
        if (coursesGrid) {
            const cards = coursesGrid.querySelectorAll('.course-card');
            cards.forEach((card, index) => {
                card.style.animationDelay = `${index * 50}ms`;
            });
        }
    }

    // Public methods for Blazor integration
    refreshAnimations() {
        if (typeof AOS !== 'undefined') {
            AOS.refresh();
        }
        this.animateCounters();
    }

    updateSearchSuggestions(suggestions) {
        // Implementation for search suggestions
        console.log('Search suggestions:', suggestions);
    }

    showLoadingState() {
        const coursesGrid = document.querySelector('.courses-grid');
        if (coursesGrid) {
            coursesGrid.classList.add('loading');
        }
    }

    hideLoadingState() {
        const coursesGrid = document.querySelector('.courses-grid');
        if (coursesGrid) {
            coursesGrid.classList.remove('loading');
        }
    }
}

// Initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.coursesEnhancer = new CoursesPageEnhancer();
    });
} else {
    window.coursesEnhancer = new CoursesPageEnhancer();
}

// CSS for ripple effects and additional animations
const additionalStyles = `
<style>
.ripple {
    position: relative;
    overflow: hidden;
}

.ripple-effect {
    position: absolute;
    border-radius: 50%;
    background: rgba(255, 255, 255, 0.3);
    transform: scale(0);
    animation: rippleAnimation 0.6s linear;
    pointer-events: none;
}

@keyframes rippleAnimation {
    to {
        transform: scale(4);
        opacity: 0;
    }
}

.course-image.loaded {
    animation: imageReveal 0.6s ease-out;
}

@keyframes imageReveal {
    0% {
        opacity: 0;
        transform: scale(1.1);
    }
    100% {
        opacity: 1;
        transform: scale(1);
    }
}

.animate-in {
    animation: slideInUp 0.6s ease-out;
}

@keyframes slideInUp {
    0% {
        transform: translateY(30px);
        opacity: 0;
    }
    100% {
        transform: translateY(0);
        opacity: 1;
    }
}

.filter-group.focused {
    transform: translateY(-2px);
    transition: transform 0.2s ease-out;
}

.btn.loading {
    pointer-events: none;
    opacity: 0.7;
}

.btn.loading::after {
    content: '';
    display: inline-block;
    width: 12px;
    height: 12px;
    border: 2px solid transparent;
    border-top: 2px solid currentColor;
    border-radius: 50%;
    animation: spin 1s linear infinite;
    margin-left: 8px;
}

.form-input.has-content {
    border-color: var(--accent-color);
    background: rgba(16, 185, 129, 0.02);
}

@media (prefers-reduced-motion: reduce) {
    .course-card {
        transform: none !important;
    }
    
    .animate-in {
        animation: none;
    }
    
    .ripple-effect {
        display: none;
    }
}
</style>
`;

// Inject additional styles
document.head.insertAdjacentHTML('beforeend', additionalStyles);