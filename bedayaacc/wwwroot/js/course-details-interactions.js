/**
 * Advanced Interactions for Course Details Page
 * Handles animations, scroll effects, and enhanced user experience
 */

class CourseDetailsEnhancer {
    constructor() {
        this.init();
        this.setupEventListeners();
        this.setupScrollEffects();
        this.setupAnimations();
        this.setupAccessibility();
    }

    init() {
        // Add enhanced class to body
        document.body.classList.add('course-details-enhanced');

        // Initialize intersection observer for animations
        this.initIntersectionObserver();

        // Setup scroll progress indicator
        this.createScrollProgress();

        // Initialize smooth scrolling
        this.initSmoothScrolling();
    }

    setupEventListeners() {
        // Enhanced accordion behavior
        this.setupAccordionEnhancements();

        // Register button interactions
        this.setupRegisterButton();

        // Image lazy loading and optimization
        this.setupImageOptimization();

        // Keyboard navigation enhancements
        this.setupKeyboardNavigation();

        // Copy to clipboard functionality
        this.setupClipboardFeatures();

        // Social sharing enhancements
        this.setupSocialSharing();

        // Print optimization
        this.setupPrintOptimization();
    }

    setupScrollEffects() {
        let ticking = false;

        window.addEventListener('scroll', () => {
            if (!ticking) {
                requestAnimationFrame(() => {
                    this.handleScroll();
                    ticking = false;
                });
                ticking = true;
            }
        });
    }

    setupAnimations() {
        // Counter animations
        this.animateCounters();

        // Progress bars
        this.animateProgressBars();

        // Typing effect for title
        this.setupTypingEffect();

        // Staggered animations for lists
        this.setupStaggeredAnimations();
    }

    setupAccessibility() {
        // ARIA labels and roles
        this.enhanceAria();

        // Focus management
        this.setupFocusManagement();

        // Screen reader announcements
        this.setupScreenReaderSupport();

        // High contrast mode detection
        this.detectHighContrast();
    }

    // ========================================
    // Core Functionality
    // ========================================

    initIntersectionObserver() {
        const options = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        this.observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('revealed');

                    // Trigger specific animations based on element type
                    this.triggerElementAnimation(entry.target);
                }
            });
        }, options);

        // Observe all animatable elements
        const animatableElements = document.querySelectorAll(
            '.course-card, .accordion-item, .lesson-item, .content-list li, .related-item'
        );

        animatableElements.forEach(el => {
            el.classList.add('reveal-on-scroll');
            this.observer.observe(el);
        });
    }

    triggerElementAnimation(element) {
        // Add specific animations based on element class
        if (element.classList.contains('course-card')) {
            this.animateCard(element);
        } else if (element.classList.contains('lesson-item')) {
            this.animateLessonItem(element);
        } else if (element.classList.contains('content-list')) {
            this.animateContentList(element);
        }
    }

    handleScroll() {
        const scrollY = window.scrollY;
        const windowHeight = window.innerHeight;
        const documentHeight = document.documentElement.scrollHeight;

        // Update scroll progress
        this.updateScrollProgress(scrollY, documentHeight, windowHeight);

        // Parallax effects
        this.updateParallaxElements(scrollY);

        // Sticky sidebar enhancements
        this.updateStickyElements(scrollY);

        // Header visibility based on scroll
        this.updateHeaderVisibility(scrollY);
    }

    // ========================================
    // Animation Functions
    // ========================================

    animateCard(card) {
        card.style.animationDelay = '0s';
        card.classList.add('animate-in');

        // Animate child elements with stagger
        const children = card.querySelectorAll('.card-body > *');
        children.forEach((child, index) => {
            setTimeout(() => {
                child.classList.add('fade-in-up');
            }, index * 100);
        });
    }

    animateLessonItem(item) {
        const icon = item.querySelector('.lesson-icon');
        const title = item.querySelector('.lesson-title');
        const duration = item.querySelector('.lesson-duration');

        if (icon) {
            setTimeout(() => icon.classList.add('bounce-in'), 100);
        }
        if (title) {
            setTimeout(() => title.classList.add('slide-in-left'), 200);
        }
        if (duration) {
            setTimeout(() => duration.classList.add('fade-in'), 300);
        }
    }

    animateContentList(list) {
        const items = list.querySelectorAll('li');
        items.forEach((item, index) => {
            setTimeout(() => {
                item.style.animationDelay = `${index * 0.1}s`;
                item.classList.add('slide-in-right');
            }, index * 50);
        });
    }

    animateCounters() {
        const counters = document.querySelectorAll('[data-counter]');
        counters.forEach(counter => {
            const target = parseInt(counter.dataset.counter);
            const duration = parseInt(counter.dataset.duration) || 2000;

            this.animateNumber(counter, 0, target, duration);
        });
    }

    animateNumber(element, start, end, duration) {
        const startTime = performance.now();
        const range = end - start;

        const step = (currentTime) => {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);

            // Easing function (easeOutCubic)
            const easeProgress = 1 - Math.pow(1 - progress, 3);

            const current = Math.floor(start + (range * easeProgress));
            element.textContent = current;

            if (progress < 1) {
                requestAnimationFrame(step);
            }
        };

        requestAnimationFrame(step);
    }

    setupTypingEffect() {
        const titleElement = document.querySelector('.course-title');
        if (!titleElement || titleElement.classList.contains('typing-complete')) return;

        const text = titleElement.textContent;
        titleElement.textContent = '';
        titleElement.style.borderRight = '3px solid var(--primary-color)';

        let index = 0;
        const typeWriter = () => {
            if (index < text.length) {
                titleElement.textContent += text.charAt(index);
                index++;
                setTimeout(typeWriter, 100);
            } else {
                titleElement.classList.add('typing-complete');
                // Blinking cursor effect
                setInterval(() => {
                    titleElement.style.borderRightColor =
                        titleElement.style.borderRightColor === 'transparent'
                            ? 'var(--primary-color)'
                            : 'transparent';
                }, 750);
            }
        };

        setTimeout(typeWriter, 500); // Start after page load
    }

    animateProgressBars() {
        const progressBars = document.querySelectorAll('.progress-fill');
        progressBars.forEach(bar => {
            const width = bar.dataset.width || '0%';
            bar.style.setProperty('--progress-width', width);
            bar.style.animation = 'progressFill 1.5s ease-out';
        });
    }

    setupStaggeredAnimations() {
        const staggerGroups = document.querySelectorAll('[data-stagger]');
        staggerGroups.forEach(group => {
            const items = group.children;
            const delay = parseInt(group.dataset.stagger) || 100;

            Array.from(items).forEach((item, index) => {
                item.style.animationDelay = `${index * delay}ms`;
                item.classList.add('stagger-item');
            });
        });
    }

    // ========================================
    // Enhanced Interactions
    // ========================================

    setupAccordionEnhancements() {
        const accordionButtons = document.querySelectorAll('.accordion-button');

        accordionButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                this.handleAccordionClick(e);
            });

            // Add ripple effect
            this.addRippleEffect(button);
        });
    }

    handleAccordionClick(e) {
        const button = e.currentTarget;
        const target = document.querySelector(button.dataset.bsTarget);

        if (target) {
            // Add smooth scroll to accordion when it expands
            setTimeout(() => {
                if (target.classList.contains('show')) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'nearest'
                    });
                }
            }, 350); // After Bootstrap's collapse animation
        }
    }

    setupRegisterButton() {
        const registerBtn = document.querySelector('.register-button');
        if (!registerBtn) return;

        // Add loading state capability
        registerBtn.addEventListener('click', (e) => {
            this.handleRegisterClick(e);
        });

        // Add pulse effect on hover
        registerBtn.addEventListener('mouseenter', () => {
            registerBtn.classList.add('pulse-effect');
        });

        registerBtn.addEventListener('mouseleave', () => {
            registerBtn.classList.remove('pulse-effect');
        });

        // Add ripple effect
        this.addRippleEffect(registerBtn);
    }

    handleRegisterClick(e) {
        const button = e.currentTarget;

        // Add loading state
        button.classList.add('loading');
        button.disabled = true;

        // Create loading spinner
        const originalText = button.innerHTML;
        button.innerHTML = `
            <span class="loading-spinner"></span>
            <span>جاري التسجيل...</span>
        `;

        // Simulate loading (remove in production)
        setTimeout(() => {
            button.classList.remove('loading');
            button.disabled = false;
            button.innerHTML = originalText;
        }, 2000);
    }

    addRippleEffect(element) {
        element.addEventListener('click', (e) => {
            const rect = element.getBoundingClientRect();
            const ripple = document.createElement('span');
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.classList.add('ripple-effect');

            element.appendChild(ripple);

            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    }

    // ========================================
    // Scroll Effects
    // ========================================

    createScrollProgress() {
        const progressBar = document.createElement('div');
        progressBar.className = 'scroll-progress';
        progressBar.innerHTML = '<div class="scroll-progress-fill"></div>';
        document.body.appendChild(progressBar);

        // Add CSS
        const style = document.createElement('style');
        style.textContent = `
            .scroll-progress {
                position: fixed;
                top: 0;
                left: 0;
                right: 0;
                height: 4px;
                background: rgba(99, 102, 241, 0.1);
                z-index: 9999;
                backdrop-filter: blur(10px);
            }
            .scroll-progress-fill {
                height: 100%;
                background: var(--primary-gradient);
                width: 0%;
                transition: width 0.1s ease-out;
                border-radius: 0 2px 2px 0;
            }
        `;
        document.head.appendChild(style);
    }

    updateScrollProgress(scrollY, documentHeight, windowHeight) {
        const progress = (scrollY / (documentHeight - windowHeight)) * 100;
        const progressFill = document.querySelector('.scroll-progress-fill');
        if (progressFill) {
            progressFill.style.width = `${Math.min(progress, 100)}%`;
        }
    }

    updateParallaxElements(scrollY) {
        const parallaxElements = document.querySelectorAll('.parallax-bg');
        parallaxElements.forEach(element => {
            const speed = element.dataset.speed || 0.5;
            const yPos = -(scrollY * speed);
            element.style.setProperty('--scroll-y', `${yPos}px`);
        });
    }

    updateStickyElements(scrollY) {
        const stickyCard = document.querySelector('.pricing-card');
        if (!stickyCard) return;

        const stickyOffset = stickyCard.offsetTop;
        const windowHeight = window.innerHeight;

        if (scrollY > stickyOffset - 100) {
            stickyCard.classList.add('sticky-active');
        } else {
            stickyCard.classList.remove('sticky-active');
        }
    }

    updateHeaderVisibility(scrollY) {
        const header = document.querySelector('.course-header');
        if (!header) return;

        if (scrollY > 100) {
            header.classList.add('scrolled');
        } else {
            header.classList.remove('scrolled');
        }
    }

    // ========================================
    // Additional Features
    // ========================================

    setupImageOptimization() {
        const images = document.querySelectorAll('img');
        images.forEach(img => {
            // Add loading placeholder
            img.addEventListener('load', () => {
                img.classList.add('loaded');
            });

            // Error handling
            img.addEventListener('error', () => {
                img.classList.add('error');
                // Could replace with placeholder image
            });
        });
    }

    setupKeyboardNavigation() {
        document.addEventListener('keydown', (e) => {
            // Enhanced tab navigation
            if (e.key === 'Tab') {
                document.body.classList.add('keyboard-navigation');
            }

            // Quick navigation shortcuts
            if (e.ctrlKey || e.metaKey) {
                switch (e.key) {
                    case 'r':
                        e.preventDefault();
                        this.focusRegisterButton();
                        break;
                    case 'c':
                        e.preventDefault();
                        this.focusCurriculum();
                        break;
                }
            }
        });

        // Remove keyboard navigation class on mouse use
        document.addEventListener('mousedown', () => {
            document.body.classList.remove('keyboard-navigation');
        });
    }

    focusRegisterButton() {
        const registerBtn = document.querySelector('.register-button');
        if (registerBtn) {
            registerBtn.focus();
            registerBtn.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    }

    focusCurriculum() {
        const curriculum = document.querySelector('#curriculum');
        if (curriculum) {
            curriculum.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
    }

    setupClipboardFeatures() {
        // Add copy buttons to code blocks if any
        const codeBlocks = document.querySelectorAll('pre code');
        codeBlocks.forEach(code => {
            const copyBtn = this.createCopyButton();
            code.parentNode.style.position = 'relative';
            code.parentNode.appendChild(copyBtn);
        });
    }

    createCopyButton() {
        const button = document.createElement('button');
        button.className = 'copy-btn';
        button.innerHTML = '<i class="fas fa-copy"></i>';
        button.title = 'Copy to clipboard';

        button.addEventListener('click', (e) => {
            const code = e.target.closest('pre').querySelector('code');
            navigator.clipboard.writeText(code.textContent).then(() => {
                button.innerHTML = '<i class="fas fa-check"></i>';
                setTimeout(() => {
                    button.innerHTML = '<i class="fas fa-copy"></i>';
                }, 2000);
            });
        });

        return button;
    }

    setupSocialSharing() {
        // Create floating share buttons
        const shareContainer = document.createElement('div');
        shareContainer.className = 'social-share-floating';
        shareContainer.innerHTML = `
            <button class="share-btn" data-platform="twitter" title="Share on Twitter">
                <i class="fab fa-twitter"></i>
            </button>
            <button class="share-btn" data-platform="facebook" title="Share on Facebook">
                <i class="fab fa-facebook"></i>
            </button>
            <button class="share-btn" data-platform="linkedin" title="Share on LinkedIn">
                <i class="fab fa-linkedin"></i>
            </button>
            <button class="share-btn" data-platform="whatsapp" title="Share on WhatsApp">
                <i class="fab fa-whatsapp"></i>
            </button>
        `;

        document.body.appendChild(shareContainer);

        // Add event listeners
        shareContainer.querySelectorAll('.share-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                this.handleSocialShare(e.target.dataset.platform);
            });
        });
    }

    handleSocialShare(platform) {
        const url = encodeURIComponent(window.location.href);
        const title = encodeURIComponent(document.title);

        const shareUrls = {
            twitter: `https://twitter.com/intent/tweet?url=${url}&text=${title}`,
            facebook: `https://www.facebook.com/sharer/sharer.php?u=${url}`,
            linkedin: `https://www.linkedin.com/sharing/share-offsite/?url=${url}`,
            whatsapp: `https://wa.me/?text=${title}%20${url}`
        };

        window.open(shareUrls[platform], '_blank', 'width=600,height=400');
    }

    setupPrintOptimization() {
        window.addEventListener('beforeprint', () => {
            document.body.classList.add('printing');
        });

        window.addEventListener('afterprint', () => {
            document.body.classList.remove('printing');
        });
    }

    // ========================================
    // Accessibility Enhancements
    // ========================================

    enhanceAria() {
        // Add ARIA labels to interactive elements
        const accordionButtons = document.querySelectorAll('.accordion-button');
        accordionButtons.forEach((button, index) => {
            button.setAttribute('aria-label', `توسيع القسم ${index + 1}`);

            // Update aria-expanded based on state
            const target = document.querySelector(button.dataset.bsTarget);
            if (target) {
                const updateAria = () => {
                    const isExpanded = target.classList.contains('show');
                    button.setAttribute('aria-expanded', isExpanded);
                };

                // Initial state
                updateAria();

                // Listen for changes
                target.addEventListener('shown.bs.collapse', updateAria);
                target.addEventListener('hidden.bs.collapse', updateAria);
            }
        });

        // Add ARIA labels to lesson items
        const lessonItems = document.querySelectorAll('.lesson-item');
        lessonItems.forEach((item, index) => {
            item.setAttribute('aria-label', `درس ${index + 1}`);
            item.setAttribute('tabindex', '0');
        });

        // Add ARIA labels to content lists
        const contentLists = document.querySelectorAll('.content-list');
        contentLists.forEach(list => {
            list.setAttribute('role', 'list');
            const items = list.querySelectorAll('li');
            items.forEach(item => {
                item.setAttribute('role', 'listitem');
            });
        });
    }

    setupFocusManagement() {
        // Create focus trap for modals
        this.createFocusTrap();

        // Enhanced focus indicators
        this.enhanceFocusIndicators();

        // Skip to main content link
        this.createSkipLink();
    }

    createFocusTrap() {
        const focusableElements = 'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])';

        document.addEventListener('keydown', (e) => {
            if (e.key === 'Tab') {
                const modal = document.querySelector('.modal.show');
                if (modal) {
                    const focusable = modal.querySelectorAll(focusableElements);
                    const firstFocusable = focusable[0];
                    const lastFocusable = focusable[focusable.length - 1];

                    if (e.shiftKey) {
                        if (document.activeElement === firstFocusable) {
                            lastFocusable.focus();
                            e.preventDefault();
                        }
                    } else {
                        if (document.activeElement === lastFocusable) {
                            firstFocusable.focus();
                            e.preventDefault();
                        }
                    }
                }
            }
        });
    }

    enhanceFocusIndicators() {
        const style = document.createElement('style');
        style.textContent = `
            .keyboard-navigation *:focus {
                outline: 3px solid var(--primary-color) !important;
                outline-offset: 2px !important;
                border-radius: 4px;
            }
            
            .focus-visible {
                outline: 2px solid var(--primary-color);
                outline-offset: 2px;
                border-radius: 4px;
            }
        `;
        document.head.appendChild(style);
    }

    createSkipLink() {
        const skipLink = document.createElement('a');
        skipLink.href = '#main-content';
        skipLink.textContent = 'تخطي إلى المحتوى الرئيسي';
        skipLink.className = 'skip-link';

        const skipStyle = document.createElement('style');
        skipStyle.textContent = `
            .skip-link {
                position: absolute;
                top: -40px;
                left: 6px;
                background: var(--primary-color);
                color: white;
                padding: 8px 16px;
                text-decoration: none;
                border-radius: 4px;
                z-index: 10000;
                font-weight: bold;
                transition: top 0.3s;
            }
            
            .skip-link:focus {
                top: 6px;
                outline: 2px solid white;
                outline-offset: 2px;
            }
        `;

        document.head.appendChild(skipStyle);
        document.body.insertBefore(skipLink, document.body.firstChild);

        // Add main content ID if not exists
        const mainContent = document.querySelector('.container') || document.querySelector('main');
        if (mainContent && !mainContent.id) {
            mainContent.id = 'main-content';
            mainContent.setAttribute('tabindex', '-1');
        }
    }

    setupScreenReaderSupport() {
        // Create live region for announcements
        const liveRegion = document.createElement('div');
        liveRegion.setAttribute('aria-live', 'polite');
        liveRegion.setAttribute('aria-atomic', 'true');
        liveRegion.className = 'sr-only';
        liveRegion.id = 'live-region';
        document.body.appendChild(liveRegion);

        // Announce page changes
        this.announcePageLoad();

        // Announce accordion state changes
        this.setupAccordionAnnouncements();
    }

    announcePageLoad() {
        const courseTitle = document.querySelector('.course-title')?.textContent;
        if (courseTitle) {
            setTimeout(() => {
                this.announce(`تم تحميل صفحة تفاصيل الكورس: ${courseTitle}`);
            }, 1000);
        }
    }

    setupAccordionAnnouncements() {
        const accordionButtons = document.querySelectorAll('.accordion-button');
        accordionButtons.forEach(button => {
            button.addEventListener('click', () => {
                setTimeout(() => {
                    const isExpanded = button.getAttribute('aria-expanded') === 'true';
                    const sectionName = button.textContent.trim();
                    const message = isExpanded
                        ? `تم فتح قسم ${sectionName}`
                        : `تم إغلاق قسم ${sectionName}`;
                    this.announce(message);
                }, 350);
            });
        });
    }

    announce(message) {
        const liveRegion = document.getElementById('live-region');
        if (liveRegion) {
            liveRegion.textContent = message;
            // Clear after announcement
            setTimeout(() => {
                liveRegion.textContent = '';
            }, 1000);
        }
    }

    detectHighContrast() {
        // Detect Windows High Contrast Mode
        const highContrastMediaQuery = window.matchMedia('(prefers-contrast: high)');

        const handleHighContrastChange = (e) => {
            if (e.matches) {
                document.body.classList.add('high-contrast');
                // Adjust animations and effects
                this.adjustForHighContrast();
            } else {
                document.body.classList.remove('high-contrast');
            }
        };

        highContrastMediaQuery.addListener(handleHighContrastChange);
        handleHighContrastChange(highContrastMediaQuery);
    }

    adjustForHighContrast() {
        // Disable subtle animations and effects
        const style = document.createElement('style');
        style.textContent = `
            .high-contrast .glow-effect,
            .high-contrast .floating-badge,
            .high-contrast .parallax-bg {
                animation: none !important;
                transform: none !important;
            }
            
            .high-contrast .course-card::before,
            .high-contrast .register-button::before {
                display: none !important;
            }
        `;
        document.head.appendChild(style);
    }

    // ========================================
    // Utility Functions
    // ========================================

    debounce(func, wait) {
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

    throttle(func, limit) {
        let inThrottle;
        return function () {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }

    // ========================================
    // Public API for Blazor Integration
    // ========================================

    refreshAnimations() {
        // Re-initialize animations after content changes
        this.setupAnimations();

        // Re-observe new elements
        const newElements = document.querySelectorAll('.course-card:not(.revealed), .accordion-item:not(.revealed)');
        newElements.forEach(el => {
            el.classList.add('reveal-on-scroll');
            this.observer.observe(el);
        });
    }

    updateProgress(completedLessons, totalLessons) {
        const progressBars = document.querySelectorAll('.progress-fill');
        const percentage = (completedLessons / totalLessons) * 100;

        progressBars.forEach(bar => {
            bar.style.setProperty('--progress-width', `${percentage}%`);
            bar.style.animation = 'progressFill 1s ease-out';
        });
    }

    showNotification(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;
        notification.innerHTML = `
            <div class="notification-content">
                <span class="notification-message">${message}</span>
                <button class="notification-close" aria-label="إغلاق الإشعار">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `;

        document.body.appendChild(notification);

        // Auto remove after 5 seconds
        setTimeout(() => {
            notification.classList.add('fade-out');
            setTimeout(() => notification.remove(), 300);
        }, 5000);

        // Manual close
        notification.querySelector('.notification-close').addEventListener('click', () => {
            notification.classList.add('fade-out');
            setTimeout(() => notification.remove(), 300);
        });

        // Announce to screen readers
        this.announce(message);
    }

    highlightElement(selector, duration = 3000) {
        const element = document.querySelector(selector);
        if (element) {
            element.classList.add('highlight-pulse');
            element.scrollIntoView({ behavior: 'smooth', block: 'center' });

            setTimeout(() => {
                element.classList.remove('highlight-pulse');
            }, duration);
        }
    }

    // ========================================
    // Cleanup and Destroy
    // ========================================

    destroy() {
        // Remove event listeners
        window.removeEventListener('scroll', this.handleScroll);

        // Disconnect observers
        if (this.observer) {
            this.observer.disconnect();
        }

        // Remove added elements
        const addedElements = document.querySelectorAll('.scroll-progress, .social-share-floating');
        addedElements.forEach(el => el.remove());

        // Remove added classes
        document.body.classList.remove('course-details-enhanced', 'keyboard-navigation');
    }
}

// ========================================
// Additional CSS for Enhanced Features
// ========================================

const enhancedStyles = `
<style>
/* Notification Styles */
.notification {
    position: fixed;
    top: 20px;
    right: 20px;
    background: var(--bg-white);
    border-radius: var(--radius-lg);
    box-shadow: var(--shadow-xl);
    border: 1px solid var(--border-light);
    z-index: 10000;
    min-width: 300px;
    opacity: 0;
    transform: translateX(100%);
    animation: slideInNotification 0.3s ease-out forwards;
}

@keyframes slideInNotification {
    to {
        opacity: 1;
        transform: translateX(0);
    }
}

.notification.fade-out {
    animation: slideOutNotification 0.3s ease-in forwards;
}

@keyframes slideOutNotification {
    to {
        opacity: 0;
        transform: translateX(100%);
    }
}

.notification-content {
    padding: 1rem 1.5rem;
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 1rem;
}

.notification-close {
    background: none;
    border: none;
    cursor: pointer;
    color: var(--text-muted);
    padding: 0.25rem;
    border-radius: var(--radius-sm);
    transition: var(--transition);
}

.notification-close:hover {
    background: var(--bg-gray);
    color: var(--text-primary);
}

/* Social Share Floating */
.social-share-floating {
    position: fixed;
    left: 20px;
    top: 50%;
    transform: translateY(-50%);
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    z-index: 1000;
}

.share-btn {
    width: 3rem;
    height: 3rem;
    border: none;
    border-radius: 50%;
    background: var(--primary-gradient);
    color: white;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    box-shadow: var(--shadow-md);
    transition: var(--transition);
}

.share-btn:hover {
    transform: scale(1.1);
    box-shadow: var(--shadow-lg);
}

/* Copy Button */
.copy-btn {
    position: absolute;
    top: 0.5rem;
    right: 0.5rem;
    background: var(--bg-gray);
    border: 1px solid var(--border-color);
    border-radius: var(--radius-sm);
    padding: 0.5rem;
    cursor: pointer;
    opacity: 0;
    transition: var(--transition);
}

pre:hover .copy-btn {
    opacity: 1;
}

/* Highlight Pulse */
.highlight-pulse {
    animation: highlightPulse 1s ease-in-out 3;
}

@keyframes highlightPulse {
    0%, 100% {
        background-color: transparent;
    }
    50% {
        background-color: rgba(99, 102, 241, 0.1);
        border-radius: var(--radius-md);
    }
}

/* Screen Reader Only */
.sr-only {
    position: absolute;
    width: 1px;
    height: 1px;
    padding: 0;
    margin: -1px;
    overflow: hidden;
    clip: rect(0, 0, 0, 0);
    white-space: nowrap;
    border: 0;
}

/* Loading State for Register Button */
.register-button.loading {
    pointer-events: none;
    opacity: 0.8;
}

.register-button .loading-spinner {
    width: 16px;
    height: 16px;
    margin-right: 0.5rem;
}

/* Sticky Card Active State */
.pricing-card.sticky-active {
    box-shadow: var(--shadow-2xl);
    transform: scale(1.02);
}

/* Mobile Responsive Adjustments */
@media (max-width: 768px) {
    .social-share-floating {
        position: fixed;
        bottom: 20px;
        left: 50%;
        transform: translateX(-50%);
        flex-direction: row;
        background: var(--bg-white);
        padding: 0.75rem;
        border-radius: 50px;
        box-shadow: var(--shadow-xl);
        border: 1px solid var(--border-light);
    }
    
    .notification {
        left: 10px;
        right: 10px;
        width: auto;
        min-width: auto;
    }
    
    .scroll-progress {
        height: 3px;
    }
}

/* Print Styles */
@media print {
    .notification,
    .social-share-floating,
    .scroll-progress,
    .copy-btn {
        display: none !important;
    }
    
    .course-card {
        break-inside: avoid;
        box-shadow: none !important;
        border: 1px solid #ccc !important;
    }
    
    .register-button {
        display: none;
    }
}
</style>
`;

// Initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.courseDetailsEnhancer = new CourseDetailsEnhancer();
    });
} else {
    window.courseDetailsEnhancer = new CourseDetailsEnhancer();
}

// Inject enhanced styles
document.head.insertAdjacentHTML('beforeend', enhancedStyles);