/**
 * Duy Mobile - Modern E-Commerce Client Scripts
 * Provides Quick View Modal, Live Product Compare, Gemini AI Chatbot, Cart Handlers
 */
(() => {
    "use strict";

    // Selector shortcuts
    const $ = (selector, parent = document) => parent.querySelector(selector);
    const $$ = (selector, parent = document) => parent.querySelectorAll(selector);

    // Core Elements
    const overlay = $("#ov");
    const modal = $("#modal");
    const chat = $("#chat");
    const chatMessages = $("#cm");
    const chatInput = $("#ci");
    const chatButton = $("#cs");
    const chatFab = $("#fab");

    // =========================================================
    // PRODUCT QUICK VIEW MODAL
    // =========================================================

    function closeModal() {
        if (!overlay) return;
        overlay.classList.remove("open");
        document.body.style.overflow = "";
    }

    async function openProduct(id) {
        if (!id || !overlay || !modal) return;

        try {
            // Show loading placeholder
            modal.innerHTML = `
                <div style="padding: 48px; text-align: center;">
                    <div style="font-size: 32px; animation: spin 1s infinite linear; display: inline-block;">⚙️</div>
                    <p style="margin-top: 12px; font-weight: 600; color: var(--mute);">Đang tải thông tin sản phẩm...</p>
                </div>
            `;
            overlay.classList.add("open");
            document.body.style.overflow = "hidden";

            const response = await fetch(`/Products/Quick/${encodeURIComponent(id)}`);
            if (!response.ok) {
                modal.innerHTML = `
                    <button class="x" onclick="closeModal()">✕</button>
                    <div style="padding: 40px; text-align: center;">
                        <p style="color: #ef4444; font-weight: 600;">Không tìm thấy thông tin sản phẩm.</p>
                        <button class="btn" style="margin-top: 16px;" onclick="closeModal()">Đóng lại</button>
                    </div>
                `;
                return;
            }

            modal.innerHTML = await response.text();
        } catch (error) {
            console.error("Lỗi khi tải Quick View:", error);
            if (modal) {
                modal.innerHTML = `
                    <button class="x" onclick="closeModal()">✕</button>
                    <div style="padding: 40px; text-align: center;">
                        <p style="color: #ef4444; font-weight: 600;">Lỗi kết nối máy chủ khi tải sản phẩm.</p>
                        <button class="btn" style="margin-top: 16px;" onclick="closeModal()">Đóng lại</button>
                    </div>
                `;
            }
        }
    }

    function showPane(name) {
        if (!modal) return;
        $$("[data-pane]", modal).forEach(pane => {
            pane.hidden = pane.dataset.pane !== name;
        });
        modal.scrollTop = 0;

        if (name === "compare") {
            loadCompareProducts();
        }
    }

    // =========================================================
    // LIVE PRODUCT COMPARISON
    // =========================================================

    async function loadCompareProducts() {
        if (!modal) return;
        const select = $("#cmpSel", modal);
        const body = $("#cmpBody", modal);
        if (!select || !body) return;

        const currentId = select.dataset.id;
        try {
            select.disabled = true;
            select.innerHTML = `<option value="">Đang tải danh sách...</option>`;

            const response = await fetch("/Products/CompareProducts");
            if (!response.ok) throw new Error(`HTTP ${response.status}`);

            const data = await response.json();
            select.innerHTML = `<option value="">-- Chọn sản phẩm để so sánh --</option>`;

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
            body.innerHTML = "<p style='color: var(--mute); font-size: 14px;'>Vui lòng chọn một sản phẩm ở trên để bắt đầu so sánh chi tiết.</p>";
        } catch (error) {
            console.error("Lỗi tải danh sách so sánh:", error);
            select.disabled = false;
            select.innerHTML = `<option value="">Không thể tải sản phẩm</option>`;
            body.innerHTML = "<p style='color: #ef4444;'>Không thể kết nối máy chủ để so sánh.</p>";
        }
    }

    async function loadCompare() {
        if (!modal) return;
        const select = $("#cmpSel", modal);
        const body = $("#cmpBody", modal);
        if (!select || !body) return;

        const id = select.dataset.id;
        const otherId = select.value;

        if (!id || !otherId) {
            body.innerHTML = "<p style='color: var(--mute); font-size: 14px;'>Vui lòng chọn một sản phẩm để bắt đầu so sánh.</p>";
            return;
        }

        try {
            body.innerHTML = "<p style='color: var(--mute); font-size: 14px;'>Đang nạp dữ liệu so sánh cấu hình...</p>";
            const params = new URLSearchParams({ id, otherId });
            const response = await fetch(`/Products/Compare?${params.toString()}`);
            if (!response.ok) {
                body.innerHTML = `<p style='color: #ef4444;'>Không thể so sánh sản phẩm (HTTP ${response.status})</p>`;
                return;
            }
            body.innerHTML = await response.text();
        } catch (error) {
            console.error("Lỗi so sánh sản phẩm:", error);
            body.innerHTML = "<p style='color: #ef4444;'>Không thể kết nối máy chủ.</p>";
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

    function addMessage(text, isUser = false) {
        const container = chatMessages || $("#cm");
        if (!container) return null;

        const message = document.createElement("div");
        message.className = isUser ? "msg me" : "msg bot";
        message.innerHTML = text.replace(/\n/g, "<br/>");

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
        typingEl.className = "msg bot typing-indicator";
        typingEl.innerHTML = `<span class="dot-pulse">● ● ●</span> <span style="font-size: 11.5px; color: var(--mute);">Duy Mobile AI đang xử lý...</span>`;
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
            const bubble = addMessage(data.reply ?? "Duy Mobile luôn sẵn sàng tư vấn cho bạn!");

            if (bubble && data.items && Array.isArray(data.items) && data.items.length > 0) {
                const recContainer = document.createElement("div");
                recContainer.style.marginTop = "10px";
                recContainer.style.display = "flex";
                recContainer.style.flexDirection = "column";
                recContainer.style.gap = "6px";

                data.items.forEach(product => {
                    const cardBtn = document.createElement("button");
                    cardBtn.type = "button";
                    cardBtn.style.padding = "8px 12px";
                    cardBtn.style.background = "var(--soft)";
                    cardBtn.style.border = "1px solid rgba(91, 75, 219, 0.2)";
                    cardBtn.style.borderRadius = "8px";
                    cardBtn.style.color = "var(--ink)";
                    cardBtn.style.fontWeight = "600";
                    cardBtn.style.fontSize = "12px";
                    cardBtn.style.textAlign = "left";
                    cardBtn.style.cursor = "pointer";
                    cardBtn.style.display = "flex";
                    cardBtn.style.justifyContent = "space-between";
                    cardBtn.style.alignItems = "center";
                    cardBtn.innerHTML = `<span>📱 ${product.name}</span> <span style="color:#e11d48; font-weight:800;">${product.price}</span>`;

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
            addMessage("Hệ thống trợ lý AI tạm thời bận hoặc không thể kết nối. Bạn có thể tra cứu nhanh qua Hotline 1800.6868 nhé!");
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
        let toast = document.getElementById("cartToast");
        if (!toast) {
            toast = document.createElement("div");
            toast.id = "cartToast";
            toast.style.cssText = "position:fixed; bottom:28px; right:28px; background:#0f172a; color:#fff; padding:14px 22px; border-radius:14px; font-weight:600; font-size:14px; box-shadow:0 12px 32px rgba(0,0,0,.25); z-index:9999; transition:all .3s ease; opacity:0; transform:translateY(20px); border-left:4px solid #10b981; display:flex; align-items:center; gap:8px;";
            document.body.appendChild(toast);
        }
        toast.textContent = msg;
        toast.style.opacity = "1";
        toast.style.transform = "translateY(0)";
        setTimeout(() => {
            toast.style.opacity = "0";
            toast.style.transform = "translateY(20px)";
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
                showCartToast("✅ " + data.message);
            } else {
                alert(data.message || "Không thể thêm sản phẩm vào giỏ hàng");
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
    // GLOBAL EVENT DELEGATION
    // =========================================================

    document.addEventListener("click", event => {
        // Modal close button
        if (event.target.closest("[data-close]")) {
            closeModal();
            return;
        }

        // Click outside modal
        if (overlay && event.target === overlay) {
            closeModal();
            return;
        }

        // Modal tab view switch
        const viewBtn = event.target.closest("[data-view]");
        if (viewBtn) {
            showPane(viewBtn.dataset.view);
            return;
        }

        // Open product explicitly
        const openBtn = event.target.closest("[data-open]");
        if (openBtn) {
            openProduct(openBtn.dataset.open);
            return;
        }

        // Add to cart buttons within cards shouldn't trigger card modal
        if (event.target.closest(".add-cart-btn")) {
            return;
        }

        // Product Card Click -> Open Quick View
        const card = event.target.closest(".card[data-id]");
        if (card) {
            openProduct(card.dataset.id);
            return;
        }

        // Open Chat button in Hero
        if (event.target.closest("#heroAI")) {
            openChat();
            return;
        }
    });

    document.addEventListener("change", event => {
        if (event.target.id === "cmpSel") {
            loadCompare();
        }
    });

    document.addEventListener("keydown", event => {
        if (event.key === "Escape") {
            closeModal();
            closeChat();
        }
    });

    // Chatbot Input Events
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

    // Initialize badge count on load
    document.addEventListener("DOMContentLoaded", () => {
        updateCartBadge();
    });

    // =========================================================
    // EXPOSE TO WINDOW OBJECT (Strictly for inline onclick attributes)
    // =========================================================
    window.openProduct = openProduct;
    window.closeModal = closeModal;
    window.showPane = showPane;
    window.loadCompare = loadCompare;
    window.loadCompareProducts = loadCompareProducts;
    window.toggleChat = toggleChat;
    window.closeChat = closeChat;
    window.closeChatModal = closeChat;
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