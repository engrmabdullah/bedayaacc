
window.setHtmlLanguageAndDir = function (lang) {
    try {
        var html = document.documentElement;
        html.setAttribute('lang', lang);
        var isRtl = (lang === 'ar' || lang === 'ur' || lang === 'fa');
        html.setAttribute('dir', isRtl ? 'rtl' : 'ltr');

        // body class للمساعدة في CSS
        document.body.classList.toggle('rtl', isRtl);
        document.body.classList.toggle('ltr', !isRtl);
    } catch (e) { console.error(e); }
}

