/**
 * Duy Mobile - Gemini AI Chatbot Module
 * Trợ lý tư vấn mua sắm và tra cứu đơn hàng AI
 */
(() => {
    "use strict";

    const chat = () => document.getElementById("chat");
    const chatMessages = () => document.getElementById("cm");
    const chatInput = () => document.getElementById("ci");
    const chatButton = () => document.getElementById("cs");
    const chatFab = () => document.getElementById("fab");

    function openChat() {
        const c = chat();
        if (!c) return;
        c.classList.add("open");
        const inp = chatInput();
        if (inp) inp.focus();
    }

    function closeChat() {
        const c = chat();
        if (c) c.classList.remove("open");
    }

    function toggleChat() {
        const c = chat();
        if (!c) return;
        c.classList.toggle("open");
        if (c.classList.contains("open")) {
            const inp = chatInput();
            if (inp) inp.focus();
        }
    }

    function addMessage(text, isUser = false, badgeHtml = "") {
        const container = chatMessages();
        if (!container) return null;

        const message = document.createElement("div");
        message.className = isUser ? "dm-msg dm-msg-me" : "dm-msg dm-msg-bot";
        message.innerHTML = (badgeHtml ? badgeHtml : "") + text.replace(/\n/g, "<br/>");

        container.appendChild(message);
        container.scrollTop = container.scrollHeight;
        return message;
    }

    async function sendMessage(customText = null) {
        const input = chatInput();
        const container = chatMessages();
        const text = (customText !== null ? customText : (input ? input.value : "")).trim();
        if (!text) return;

        addMessage(text, true);
        if (input) input.value = "";

        // Typing indicator
        const typingEl = document.createElement("div");
        typingEl.className = "dm-msg dm-msg-bot typing-indicator";
        typingEl.innerHTML = `<span class="dm-dot-pulse">● ● ●</span> <span style="font-size: 11.5px; color: var(--dm-text-muted);" class="ms-2">Duy Mobile AI đang tư vấn...</span>`;
        if (container) {
            container.appendChild(typingEl);
            container.scrollTop = container.scrollHeight;
        }

        try {
            const response = await fetch("/api/chat", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ message: text })
            });

            typingEl.remove();

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            const data = await response.json();

            let badgeHtml = "";
            if (data.source === "gemini") {
                badgeHtml = `<div class="mb-1"><span class="badge bg-success-subtle text-success border border-success-subtle" style="font-size:10px;"><i class="bi bi-stars me-1"></i>Google Gemini AI</span></div>`;
            } else if (data.source === "local_smart_catalog") {
                badgeHtml = `<div class="mb-1"><span class="badge bg-primary-subtle text-primary border border-primary-subtle" style="font-size:10px;" title="${data.note || ''}"><i class="bi bi-robot me-1"></i>Duy Mobile AI (Kho hàng)</span></div>`;
            } else if (data.source === "mvc_fallback") {
                badgeHtml = `<div class="mb-1"><span class="badge bg-warning-subtle text-warning border border-warning-subtle" style="font-size:10px;"><i class="bi bi-info-circle me-1"></i>Duy Mobile AI (Dự phòng)</span></div>`;
            }

            const bubble = addMessage(data.reply ?? "Duy Mobile sẵn sàng hỗ trợ bạn!", false, badgeHtml);

            if (bubble && data.items && Array.isArray(data.items) && data.items.length > 0) {
                const recContainer = document.createElement("div");
                recContainer.style.marginTop = "10px";
                recContainer.style.display = "flex";
                recContainer.style.flexDirection = "column";
                recContainer.style.gap = "6px";

                data.items.forEach(product => {
                    const cardBtn = document.createElement("button");
                    cardBtn.type = "button";
                    cardBtn.className = "btn btn-sm btn-light border text-start d-flex justify-content-between align-items-center p-2 rounded-3";
                    cardBtn.innerHTML = `<span class="fw-bold small text-dark"><i class="bi bi-phone text-primary me-1"></i> ${product.name}</span> <span class="badge bg-danger">${product.price}</span>`;

                    cardBtn.onclick = () => {
                        if (typeof window.openProduct === "function") {
                            window.openProduct(product.id);
                        }
                    };

                    recContainer.appendChild(cardBtn);
                });

                bubble.appendChild(recContainer);
            }

            if (container) container.scrollTop = container.scrollHeight;
        } catch (error) {
            typingEl.remove();
            console.error("[Chatbot] Lỗi:", error);
            addMessage("Hệ thống trợ lý AI tạm thời bận. Bạn có thể tra cứu nhanh qua Hotline 1800.6868 nhé!");
        }
    }

    function sendChatPrompt(prompt) {
        openChat();
        sendMessage(prompt);
    }

    function askAiWithPrompt(prompt) {
        sendChatPrompt(prompt);
    }

    // Attach listeners
    document.addEventListener("DOMContentLoaded", () => {
        const fab = chatFab();
        if (fab) fab.addEventListener("click", toggleChat);

        const btn = chatButton();
        if (btn) btn.addEventListener("click", () => sendMessage());

        const input = chatInput();
        if (input) {
            input.addEventListener("keydown", event => {
                if (event.key === "Enter") {
                    event.preventDefault();
                    sendMessage();
                }
            });
        }
    });

    // Expose to window for global access
    window.openChat = openChat;
    window.closeChat = closeChat;
    window.toggleChat = toggleChat;
    window.sendMessage = sendMessage;
    window.sendChatPrompt = sendChatPrompt;
    window.askAiWithPrompt = askAiWithPrompt;
})();
