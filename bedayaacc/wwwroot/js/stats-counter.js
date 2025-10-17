/**
 * Enhanced Stats Counter with Animation
 * Features:
 * - Smooth number counting animation
 * - Progress bar animation
 * - Intersection Observer for triggering on scroll
 * - Support for different suffixes (+, %, /7, etc.)
 * - Customizable duration and easing
 */

(function () {
    'use strict';

    // Configuration
    const CONFIG = {
        duration: 2000,           // Animation duration in milliseconds
        threshold: 0.3,           // Intersection threshold (0-1)
        rootMargin: '0px 0px -100px 0px',  // Start animation 100px before entering viewport
        progressDelay: 150,       // Delay between each progress bar animation
        numberDelay: 300,         // Delay before starting number animation
        easingFunction: easeOutCubic  // Easing function for smooth animation
    };

    /**
     * Easing function for smooth animation
     */
    function easeOutCubic(t) {
        return 1 - Math.pow(1 - t, 3);
    }

    /**
     * Animate a single number element
     */
    function animateNumber(element, target, suffix, duration) {
        const startTime = performance.now();

        function update(currentTime) {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            const easedProgress = CONFIG.easingFunction(progress);
            const currentValue = Math.floor(easedProgress * target);

            element.textContent = currentValue + suffix;

            if (progress < 1) {
                requestAnimationFrame(update);
            } else {
                element.textContent = target + suffix;
            }
        }

        requestAnimationFrame(update);
    }

    /**
     * Initialize stats counter
     */
    function initializeStatsCounter() {
        const statsSection = document.getElementById('statsSection');

        if (!statsSection) {
            console.warn('Stats section not found');
            return;
        }

        // Create intersection observer
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    animateStats(entry.target);
                    observer.unobserve(entry.target);
                }
            });
        }, {
            threshold: CONFIG.threshold,
            rootMargin: CONFIG.rootMargin
        });

        observer.observe(statsSection);
    }

    /**
     * Animate all stats in the section
     */
    function animateStats(section) {
        const statNumbers = section.querySelectorAll('.stat-number-enhanced');
        const statItems = section.querySelectorAll('.stat-item-enhanced');

        // Animate numbers
        statNumbers.forEach((element, index) => {
            const target = parseInt(element.getAttribute('data-count'));
            const suffix = element.getAttribute('data-suffix') || '';

            // Add delay for staggered animation
            setTimeout(() => {
                animateNumber(element, target, suffix, CONFIG.duration);
            }, CONFIG.numberDelay + (index * 100));
        });

        // Animate progress bars
        statItems.forEach((item, index) => {
            setTimeout(() => {
                item.classList.add('animated');
            }, index * CONFIG.progressDelay);
        });
    }

    /**
     * Alternative: Counter with more features
     */
    class StatsCounter {
        constructor(element, options = {}) {
            this.element = element;
            this.target = parseInt(element.getAttribute('data-count'));
            this.suffix = element.getAttribute('data-suffix') || '';
            this.duration = options.duration || 2000;
            this.decimals = options.decimals || 0;
            this.separator = options.separator || ',';

            this.startValue = 0;
            this.startTime = null;
        }

        format(num) {
            if (this.decimals > 0) {
                num = num.toFixed(this.decimals);
            } else {
                num = Math.floor(num);
            }

            // Add thousands separator
            if (this.separator && num >= 1000) {
                return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, this.separator);
            }

            return num;
        }

        animate() {
            if (this.startTime === null) {
                this.startTime = performance.now();
            }

            const currentTime = performance.now();
            const elapsed = currentTime - this.startTime;
            const progress = Math.min(elapsed / this.duration, 1);
            const easedProgress = CONFIG.easingFunction(progress);
            const currentValue = this.startValue + (this.target - this.startValue) * easedProgress;

            this.element.textContent = this.format(currentValue) + this.suffix;

            if (progress < 1) {
                requestAnimationFrame(() => this.animate());
            } else {
                this.element.textContent = this.format(this.target) + this.suffix;
            }
        }

        start() {
            this.startTime = null;
            this.animate();
        }
    }

    /**
     * Usage with advanced features
     */
    function initializeAdvancedCounter() {
        const statsSection = document.getElementById('statsSection');
        if (!statsSection) return;

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const statNumbers = entry.target.querySelectorAll('.stat-number-enhanced');

                    statNumbers.forEach((element, index) => {
                        setTimeout(() => {
                            const counter = new StatsCounter(element, {
                                duration: 2000,
                                decimals: 0,
                                separator: ','  // Add comma for thousands (optional)
                            });
                            counter.start();
                        }, 300 + (index * 100));
                    });

                    // Animate progress bars
                    const statItems = entry.target.querySelectorAll('.stat-item-enhanced');
                    statItems.forEach((item, index) => {
                        setTimeout(() => {
                            item.classList.add('animated');
                        }, index * 150);
                    });

                    observer.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0.3,
            rootMargin: '0px 0px -100px 0px'
        });

        observer.observe(statsSection);
    }

    // Export to global scope
    window.initializeStatsCounter = initializeStatsCounter;
    window.initializeAdvancedCounter = initializeAdvancedCounter;
    window.StatsCounter = StatsCounter;

    // Auto-initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeStatsCounter);
    } else {
        initializeStatsCounter();
    }

})();