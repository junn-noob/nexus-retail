/**
 * Duy Mobile - Core Site Orchestrator
 * Coordinates modular scripts (cart, product-modal, chatbot) and sets global shortcut listeners.
 */
(() => {
    "use strict";

    // Global keyboard shortcuts (Esc to close open drawers or modals)
    document.addEventListener("keydown", event => {
        if (event.key === "Escape") {
            if (typeof window.closeChat === "function") {
                window.closeChat();
            }
            if (typeof window.closeModal === "function") {
                window.closeModal();
            }
        }
    });

    console.info("[DuyMobile] Modular client scripts initialized successfully.");
})();