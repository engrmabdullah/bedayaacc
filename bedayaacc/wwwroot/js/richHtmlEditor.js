// wwwroot/js/richHtmlEditor.js
window.RichHtmlEditor = window.RichHtmlEditor || {
    _ensure: function () { /* marker for interop readiness */ },

    setHtml: function (el, html) {
        if (!el) return;
        el.innerHTML = html || "";
        // إزالة <br> الوهمية عند الفراغ لتحسين placeholder:empty
        if (!html || !html.trim()) el.innerHTML = "";
    },

    getHtml: function (el) {
        if (!el) return "";
        return el.innerHTML || "";
    },

    exec: function (cmd, value) {
        try {
            document.execCommand(cmd, false, value || null);
        } catch (e) {
            console.warn("execCommand failed:", cmd, e);
        }
    },

    prompt: function (msg) {
        return window.prompt(msg || "Enter value");
    },

    setDir: function (el, dir) {
        if (!el) return;
        el.setAttribute("dir", dir === "rtl" ? "rtl" : (dir === "ltr" ? "ltr" : "auto"));
    },

    insertHtml: function (html) {
        try {
            document.execCommand("insertHTML", false, html || "");
        } catch (e) {
            console.warn("insertHTML failed", e);
        }
    }
};
