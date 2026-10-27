/**
 * Duy Mobile - Core Site Orchestrator
 * Coordinates modular scripts (cart, product-modal, chatbot) and sets global shortcut listeners.
 */
(() => {
    "use strict";

    // Global keyboard shortcuts (Esc to close, Ctrl+K or / to search)
    document.addEventListener("keydown", event => {
        if (event.key === "Escape") {
            if (typeof window.closeChat === "function") {
                window.closeChat();
            }
            if (typeof window.closeModal === "function") {
                window.closeModal();
            }
        }
        else if ((event.ctrlKey && event.key === "k") || (event.key === "/" && document.activeElement.tagName !== "INPUT" && document.activeElement.tagName !== "TEXTAREA")) {
            event.preventDefault();
            const searchInput = document.querySelector(".dm-search-input");
            if (searchInput) {
                searchInput.focus();
                searchInput.select();
            }
        }
    });

    console.info("[DuyMobile] Modular client scripts initialized successfully.");
})();