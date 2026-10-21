/**
 * Duy Mobile - Modern E-Commerce Client Scripts
 * Integration with Bootstrap 5 Modal, Live Product Compare, Gemini AI Chatbot, Cart Handlers
 */
(() => {
    "use strict";

    // Selector shortcuts
    const $ = (selector, parent = document) => parent.querySelector(selector);
    const $$ = (selector, parent = document) => parent.querySelectorAll(selector);

    // Core Elements
    const chat = $("#chat");
    const chatMessages = $("#cm");
    const chatInput = $("#ci");
    const chatButton = $("#cs");
    const chatFab = $("#fab");

    // =========================================================
    // PRODUCT QUICK VIEW MODAL (Bootstrap 5)
    // =========================================================

    function getModalInstance() {
        const el = document.getElementById("productModal");
        if (!el || typeof bootstrap === "undefined") return null;
        return bootstrap.Modal.getOrCreateInstance(el);
    }

    function closeModal() {
        const modal = getModalInstance();
        if (modal) modal.hide();
    }

    async function openProduct(id) {
        if (!id) return;
        const dialog = document.getElementById("productModalDialog");
        if (!dialog) return;

        // Show loading placeholder inside modal
        dialog.innerHTML = `
            <div class="modal-body text-center py-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Đang tải...</span>
                </div>
                <div class="mt-3 text-secondary fw-semibold">Đang tải thông số kỹ thuật...</div>
            </div>
        `;

        const modal = getModalInstance();
        if (modal) modal.show();

        try {
            const response = await fetch(`/Products/Quick/${encodeURIComponent(id)}`);
            if (!response.ok) {
                dialog.innerHTML = `
                    <div class="modal-header">
                        <h5 class="modal-title text-danger">Thông báo</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body text-center py-4">
                        <i class="bi bi-exclamation-circle text-danger fs-1 d-block mb-2"></i>
                        <h5>Không tìm thấy thông tin sản phẩm.</h5>
                        <p class="small text-secondary">Mã sản phẩm: ${id}</p>
                    </div>
                `;
                return;
            }

            dialog.innerHTML = await response.text();
        } catch (error) {
            console.error("Lỗi khi tải Quick View:", error);
            dialog.innerHTML = `
                <div class="modal-header">
                    <h5 class="modal-title text-danger">Lỗi kết nối</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body text-center py-4">
                    <p class="text-danger fw-semibold">Không thể kết nối đến máy chủ. Vui lòng thử lại sau.</p>
                </div>
            `;
        }
    }

    // =========================================================
    // LIVE PRODUCT COMPARISON
    // =========================================================

    async function loadCompareProducts() {
        const select = document.getElementById("cmpSel");
        const body = document.getElementById("cmpBody");
        if (!select || !body) return;

        const currentId = select.dataset.id;
        try {
            select.disabled = true;
            select.innerHTML = `<option value="">Đang tải danh sách so sánh...</option>`;

            const response = await fetch("/Products/CompareProducts");
            if (!response.ok) throw new Error(`HTTP ${response.status}`);

            const data = await response.json();
            select.innerHTML = `<option value="">-- Chọn sản phẩm thứ 2 để đối đầu --</option>`;

            if (data.items && Array.isArray(data.items)) {
                data.items.forEach(product => {
                    if (product.maSp === currentId) return;
                    const option = document.createElement("option");
                    option.value = product.maSp;
                    option.textContent = `${product.tenSp} - ${Number(product.giaBan).toLocaleString("vi-VN")} ₫`;
                    select.appendChild(option);
                });
            }
            select.disabled = false;
        } catch (error) {
            console.error("Lỗi tải danh sách so sánh:", error);
            select.disabled = false;
            select.innerHTML = `<option value="">Không thể nạp danh sách sản phẩm</option>`;
        }
    }

    async function loadCompare() {
        const select = document.getElementById("cmpSel");
        const body = document.getElementById("cmpBody");
        if (!select || !body) return;

        const id = select.dataset.id;
        const otherId = select.value;

        if (!id || !otherId) {
            body.innerHTML = `
                <div class="text-center py-4 text-secondary">
                    <i class="bi bi-arrow-left-right fs-2 d-block mb-2"></i>
                    Vui lòng chọn sản phẩm thứ 2 từ danh sách trên để xem bảng so sánh đối đầu.
                </div>
            `;
            return;
        }

        try {
            body.innerHTML = `
                <div class="text-center py-4">
                    <div class="spinner-border spinner-border-sm text-primary me-2"></div>
                    <span class="text-secondary fw-semibold">Đang tổng hợp thông số đối đầu...</span>
                </div>
            `;
            const params = new URLSearchParams({ id, otherId });
            const response = await fetch(`/Products/Compare?${params.toString()}`);
            if (!response.ok) {
                body.innerHTML = `<div class="p-3 text-danger text-center">Không thể so sánh sản phẩm (HTTP ${response.status})</div>`;
                return;
            }
            body.innerHTML = await response.text();
        } catch (error) {
            console.error("Lỗi so sánh sản phẩm:", error);
            body.innerHTML = `<div class="p-3 text-danger text-center">Không thể kết nối máy chủ.</div>`;
        }
    }

    // =========================================================
    // GEMINI AI CHATBOT
    // =========================================================

    function openChat() {
        const c = chat || $("#chat");
        if (!c) return;
        c.classList.add("open");
        const inp = chatInput || $("#ci");
        if (inp) inp.focus();
    }

    function closeChat() {
        const c = chat || $("#chat");
        if (c) c.classList.remove("open");
    }

    function toggleChat() {
        const c = chat || $("#chat");
        if (!c) return;
        c.classList.toggle("open");
        if (c.classList.contains("open")) {
            const inp = chatInput || $("#ci");
            if (inp) inp.focus();
        }
    }

    function addMessage(text, isUser = false, badgeHtml = "") {
        const container = chatMessages || $("#cm");
        if (!container) return null;

        const message = document.createElement("div");
        message.className = isUser ? "dm-msg dm-msg-me" : "dm-msg dm-msg-bot";
        message.innerHTML = (badgeHtml ? badgeHtml : "") + text.replace(/\n/g, "<br/>");

        container.appendChild(message);
        container.scrollTop = container.scrollHeight;
        return message;
    }

    async function sendMessage(customText = null) {
        const input = chatInput || $("#ci");
        const container = chatMessages || $("#cm");
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
                        openProduct(product.id);
                    };

                    recContainer.appendChild(cardBtn);
                });

                bubble.appendChild(recContainer);
            }

            if (container) container.scrollTop = container.scrollHeight;
        } catch (error) {
            typingEl.remove();
            console.error("Lỗi chatbot:", error);
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

    // =========================================================
    // CART HELPER FUNCTIONS
    // =========================================================

    function showCartToast(msg) {
        let toastEl = document.getElementById("cartToast");
        if (!toastEl) {
            toastEl = document.createElement("div");
            toastEl.id = "cartToast";
            toastEl.style.cssText = "position:fixed; bottom:28px; right:28px; background:#0f172a; color:#fff; padding:14px 22px; border-radius:14px; font-weight:600; font-size:14px; box-shadow:0 12px 32px rgba(0,0,0,.25); z-index:9999; transition:all .3s ease; opacity:0; transform:translateY(20px); border-left:4px solid #10b981; display:flex; align-items:center; gap:8px;";
            document.body.appendChild(toastEl);
        }
        toastEl.innerHTML = `<i class="bi bi-cart-check-fill text-success fs-5"></i> <span>${msg}</span>`;
        toastEl.style.opacity = "1";
        toastEl.style.transform = "translateY(0)";
        setTimeout(() => {
            toastEl.style.opacity = "0";
            toastEl.style.transform = "translateY(20px)";
        }, 2600);
    }

    function quickAddToCart(e, maSp) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        if (!maSp) return;

        const formData = new FormData();
        formData.append("maSp", maSp);
        formData.append("quantity", 1);

        fetch("/Cart/AddToCart", {
            method: "POST",
            body: formData
        })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                const badge = document.getElementById("cartCount");
                if (badge) badge.textContent = data.totalItems;
                showCartToast(data.message || "Đã thêm sản phẩm vào giỏ hàng");
            } else {
                alert(data.message || "Không thể thêm sản phẩm vào giỏ");
            }
        })
        .catch(err => {
            console.error("Lỗi thêm giỏ hàng:", err);
            alert("Lỗi kết nối máy chủ!");
        });
    }

    function addToCartFromModal(maSp) {
        quickAddToCart(null, maSp);
    }

    function buyNowFromModal(maSp) {
        const formData = new FormData();
        formData.append("maSp", maSp);
        formData.append("quantity", 1);
        fetch("/Cart/AddToCart", {
            method: "POST",
            body: formData
        })
        .then(() => {
            window.location.href = "/Cart/Index";
        })
        .catch(() => {
            window.location.href = "/Cart/Index";
        });
    }

    function updateCartBadge() {
        fetch("/Cart/GetCartCount")
            .then(r => r.json())
            .then(d => {
                const el = document.getElementById("cartCount");
                if (el) el.textContent = d.totalItems || 0;
            })
            .catch(() => {});
    }

    // =========================================================
    // GLOBAL LISTENERS
    // =========================================================

    if (chatFab) {
        chatFab.addEventListener("click", toggleChat);
    }

    if (chatButton) {
        chatButton.addEventListener("click", () => sendMessage());
    }

    if (chatInput) {
        chatInput.addEventListener("keydown", event => {
            if (event.key === "Enter") {
                event.preventDefault();
                sendMessage();
            }
        });
    }

    document.addEventListener("keydown", event => {
        if (event.key === "Escape") {
            closeChat();
        }
    });

    document.addEventListener("DOMContentLoaded", () => {
        updateCartBadge();
    });

    // =========================================================
    // EXPOSE TO WINDOW
    // =========================================================
    window.openProduct = openProduct;
    window.closeModal = closeModal;
    window.loadCompare = loadCompare;
    window.loadCompareProducts = loadCompareProducts;
    window.toggleChat = toggleChat;
    window.closeChat = closeChat;
    window.openChat = openChat;
    window.sendMessage = sendMessage;
    window.sendChatPrompt = sendChatPrompt;
    window.askAiWithPrompt = askAiWithPrompt;
    window.quickAddToCart = quickAddToCart;
    window.addToCartFromModal = addToCartFromModal;
    window.buyNowFromModal = buyNowFromModal;
    window.showCartToast = showCartToast;
    window.updateCartBadge = updateCartBadge;

})();