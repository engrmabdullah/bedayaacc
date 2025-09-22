// Home Page JavaScript Functions

window.initializeHomePage = () => {
    // Initialize AOS (Animate On Scroll)
    if (typeof AOS !== 'undefined') {
        AOS.init({
            duration: 800,
            easing: 'ease-out',
            once: true,
            offset: 100
        });
    }

    // Initialize number counters
    initializeCounters();

    // Initialize testimonial slider
    initializeTestimonialSlider();

    // Initialize scroll animations
    initializeScrollAnimations();

    // Initialize particle effects
    initializeParticleEffects();
};

// Number Counter Animation
function initializeCounters() {
    const counters = document.querySelectorAll('.hero-stat-number[data-count]');

    const animateCounter = (counter) => {
        const target = parseInt(counter.getAttribute('data-count'));
        const duration = 2000; // 2 seconds
        const step = target / (duration / 16); // 60fps
        let current = 0;

        const timer = setInterval(() => {
            current += step;
            if (current >= target) {
                current = target;
                clearInterval(timer);
            }
            counter.textContent = Math.floor(current).toLocaleString();
        }, 16);
    };

    // Intersection Observer for counters
    const counterObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                animateCounter(entry.target);
                counterObserver.unobserve(entry.target);
            }
        });
    });

    counters.forEach(counter => {
        counterObserver.observe(counter);
    });
}

// Testimonial Slider
let currentTestimonialIndex = 0;
let testimonialInterval;

function initializeTestimonialSlider() {
    const testimonials = document.querySelectorAll('.testimonial-card');
    const dots = document.querySelectorAll('.testimonial-dot');

    if (testimonials.length === 0) return;

    // Auto-rotate testimonials
    testimonialInterval = setInterval(() => {
        showTestimonial((currentTestimonialIndex + 1) % testimonials.length);
    }, 5000);

    // Pause on hover
    const slider = document.querySelector('.testimonials-slider');
    if (slider) {
        slider.addEventListener('mouseenter', () => {
            clearInterval(testimonialInterval);
        });

        slider.addEventListener('mouseleave', () => {
            testimonialInterval = setInterval(() => {
                showTestimonial((currentTestimonialIndex + 1) % testimonials.length);
            }, 5000);
        });
    }
}

window.showTestimonial = (index) => {
    const testimonials = document.querySelectorAll('.testimonial-card');
    const dots = document.querySelectorAll('.testimonial-dot');

    // Remove active class from all
    testimonials.forEach(testimonial => testimonial.classList.remove('active'));
    dots.forEach(dot => dot.classList.remove('active'));

    // Add active class to current
    if (testimonials[index]) {
        testimonials[index].classList.add('active');
    }
    if (dots[index]) {
        dots[index].classList.add('active');
    }

    currentTestimonialIndex = index;
};

// Scroll Animations
function initializeScrollAnimations() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('aos-animate');
            }
        });
    }, observerOptions);

    // Observe all elements with data-aos attribute
    document.querySelectorAll('[data-aos]').forEach(element => {
        observer.observe(element);
    });

    // Course cards animation
    document.querySelectorAll('.course-wrapper').forEach((card, index) => {
        setTimeout(() => {
            card.classList.add('aos-animate');
        }, index * 100);
    });
}

// Particle Effects for Hero Section
function initializeParticleEffects() {
    const heroSection = document.querySelector('.hero-section');
    if (!heroSection) return;

    // Create floating particles
    for (let i = 0; i < 20; i++) {
        createFloatingParticle(heroSection);
    }
}

function createFloatingParticle(container) {
    const particle = document.createElement('div');
    particle.className = 'floating-particle';
    particle.style.cssText = `
        position: absolute;
        width: 4px;
        height: 4px;
        background: rgba(255, 255, 255, 0.6);
        border-radius: 50%;
        pointer-events: none;
        z-index: 1;
    `;

    // Random position
    particle.style.left = Math.random() * 100 + '%';
    particle.style.top = Math.random() * 100 + '%';

    // Random animation
    const duration = 3 + Math.random() * 4; // 3-7 seconds
    const distance = 50 + Math.random() * 100; // 50-150px

    particle.style.animation = `floatParticle ${duration}s ease-in-out infinite`;

    container.appendChild(particle);

    // Remove and recreate after animation
    setTimeout(() => {
        if (particle.parentNode) {
            particle.parentNode.removeChild(particle);
            createFloatingParticle(container);
        }
    }, duration * 1000);
}

// Video Modal
window.playIntroVideo = () => {
    // Create modal backdrop
    const modal = document.createElement('div');
    modal.className = 'video-modal';
    modal.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(0, 0, 0, 0.9);
        z-index: 10000;
        display: flex;
        align-items: center;
        justify-content: center;
        opacity: 0;
        transition: opacity 0.3s ease;
    `;

    // Create video container
    const videoContainer = document.createElement('div');
    videoContainer.style.cssText = `
        position: relative;
        max-width: 90%;
        max-height: 90%;
        background: #000;
        border-radius: 12px;
        overflow: hidden;
    `;

    // Create close button
    const closeBtn = document.createElement('button');
    closeBtn.innerHTML = '<i class="fas fa-times"></i>';
    closeBtn.style.cssText = `
        position: absolute;
        top: 20px;
        right: 20px;
        background: rgba(255, 255, 255, 0.9);
        border: none;
        width: 40px;
        height: 40px;
        border-radius: 50%;
        cursor: pointer;
        z-index: 1;
        display: flex;
        align-items: center;
        justify-content: center;
        transition: all 0.3s ease;
    `;

    closeBtn.addEventListener('click', () => {
        modal.style.opacity = '0';
        setTimeout(() => {
            document.body.removeChild(modal);
        }, 300);
    });

    // Create video element (replace with your actual video)
    const video = document.createElement('iframe');
    video.src = 'https://www.youtube.com/embed/dQw4w9WgXcQ'; // Replace with your video URL
    video.style.cssText = `
        width: 800px;
        height: 450px;
        max-width: 100%;
        max-height: 100%;
        border: none;
    `;

    videoContainer.appendChild(video);
    videoContainer.appendChild(closeBtn);
    modal.appendChild(videoContainer);
    document.body.appendChild(modal);

    // Show modal
    setTimeout(() => {
        modal.style.opacity = '1';
    }, 10);

    // Close on backdrop click
    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            closeBtn.click();
        }
    });

    // Close on escape key
    const handleEscape = (e) => {
        if (e.key === 'Escape') {
            closeBtn.click();
            document.removeEventListener('keydown', handleEscape);
        }
    };
    document.addEventListener('keydown', handleEscape);
};

// Smooth scrolling for internal links
document.addEventListener('DOMContentLoaded', () => {
    const links = document.querySelectorAll('a[href^="#"]');
    links.forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const target = document.querySelector(link.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
});

// Hero background animation
function initializeHeroBackground() {
    const heroParticles = document.querySelector('.hero-particles');
    if (!heroParticles) return;

    let mouseX = 0;
    let mouseY = 0;

    document.addEventListener('mousemove', (e) => {
        mouseX = e.clientX / window.innerWidth;
        mouseY = e.clientY / window.innerHeight;

        heroParticles.style.transform = `translate(${mouseX * 20}px, ${mouseY * 20}px)`;
    });
}

// Initialize hero background on load
document.addEventListener('DOMContentLoaded', initializeHeroBackground);

// Lazy loading for images
function initializeLazyLoading() {
    const images = document.querySelectorAll('img[data-src]');

    const imageObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.getAttribute('data-src');
                img.removeAttribute('data-src');
                imageObserver.unobserve(img);
            }
        });
    });

    images.forEach(img => {
        imageObserver.observe(img);
    });
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
    initializeLazyLoading();
});

// CSS Animations (add to head)
const style = document.createElement('style');
style.textContent = `
    @keyframes floatParticle {
        0%, 100% { 
            transform: translateY(0px) translateX(0px) rotate(0deg);
            opacity: 0;
        }
        10%, 90% {
            opacity: 1;
        }
        50% { 
            transform: translateY(-100px) translateX(20px) rotate(180deg);
            opacity: 0.8;
        }
    }
    
    .floating-particle {
        animation: floatParticle 4s ease-in-out infinite !important;
    }
    
    .video-modal {
        backdrop-filter: blur(5px);
    }
    
    .hero-main-visual {
        animation: heroFloat 3s ease-in-out infinite;
    }
    
    @keyframes heroFloat {
        0%, 100% { transform: translate(-50%, -50%) translateY(0px); }
        50% { transform: translate(-50%, -50%) translateY(-10px); }
    }
`;
document.head.appendChild(style);

// Performance optimization: Debounce scroll events
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

// Optimize scroll performance
window.addEventListener('scroll', debounce(() => {
    // Add any scroll-based animations here
}, 10));

// Clean up intervals when page is hidden
document.addEventListener('visibilitychange', () => {
    if (document.hidden) {
        clearInterval(testimonialInterval);
    } else {
        initializeTestimonialSlider();
    }
});