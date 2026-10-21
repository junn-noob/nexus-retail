(() => {
    "use strict";

    // =========================================================
    // Helpers
    // =========================================================
    const $ = (selector, parent = document) =>
        parent.querySelector(selector);

    const $$ = (selector, parent = document) =>
        parent.querySelectorAll(selector);


    // =========================================================
    // Elements
    // =========================================================

    // Product modal
    const overlay = $("#ov");
    const modal = $("#modal");

    // Chatbot
    const chat = $("#chat");
    const chatMessages = $("#cm");
    const chatInput = $("#ci");
    const chatButton = $("#cs");
    const chatFab = $("#fab");


    // =========================================================
    // PRODUCT MODAL
    // =========================================================

    function closeModal() {
        if (!overlay)
            return;

        overlay.classList.remove("open");
        document.body.style.overflow = "";
    }


    async function openProduct(id) {

        if (!id || !overlay || !modal)
            return;

        try {
            const response = await fetch(
                `/Products/Quick/${encodeURIComponent(id)}`
            );

            if (!response.ok) {
                console.error(
                    `Không thể tải sản phẩm ${id}. HTTP ${response.status}`
                );

                return;
            }

            modal.innerHTML = await response.text();

            overlay.classList.add("open");
            document.body.style.overflow = "hidden";
        }
        catch (error) {
            console.error("Lỗi khi tải Quick View:", error);
        }
    }


    function showPane(name) {

    if (!modal)
        return;

    $$("[data-pane]", modal).forEach(pane => {
        pane.hidden = pane.dataset.pane !== name;
    });

    modal.scrollTop = 0;

    // Khi mở tab compare thì lấy danh sách sản phẩm
    if (name === "compare") {
        loadCompareProducts();
    }
}


// =========================================================
// COMPARE - LOAD PRODUCT LIST
// =========================================================

async function loadCompareProducts() {

    if (!modal)
        return;

    const select = $("#cmpSel", modal);
    const body = $("#cmpBody", modal);

    if (!select || !body) {
        console.error("Không tìm thấy cmpSel hoặc cmpBody");
        return;
    }

    const currentId = select.dataset.id;

    console.log("Sản phẩm hiện tại:", currentId);

    try {

        select.disabled = true;

        select.innerHTML =
            `<option value="">Đang tải...</option>`;

        const response = await fetch(
            "/Products/CompareProducts"
        );

        console.log(
            "CompareProducts status:",
            response.status
        );

        if (!response.ok) {
            throw new Error(
                `HTTP ${response.status}`
            );
        }

        const data = await response.json();

        console.log(
            "Danh sách sản phẩm:",
            data
        );

        select.innerHTML =
            `<option value="">-- Chọn sản phẩm --</option>`;

        if (!data.items) {
            console.error(
                "API không trả về items:",
                data
            );

            return;
        }

        data.items.forEach(product => {

            // Không hiển thị chính sản phẩm đang xem
            if (product.maSp === currentId)
                return;

            const option =
                document.createElement("option");

            option.value =
                product.maSp;

            option.textContent =
                `${product.tenSp} - ${Number(product.giaBan)
                    .toLocaleString("vi-VN")} ₫`;

            select.appendChild(option);
        });

        select.disabled = false;

        body.innerHTML =
            "<p>Chọn một sản phẩm để bắt đầu so sánh.</p>";
    }
    catch (error) {

        console.error(
            "Lỗi tải danh sách so sánh:",
            error
        );

        select.disabled = false;

        select.innerHTML =
            `<option value="">Không thể tải sản phẩm</option>`;

        body.innerHTML =
            "<p>Không thể tải danh sách sản phẩm.</p>";
    }
}


// =========================================================
// COMPARE - COMPARE TWO PRODUCTS
// =========================================================

async function loadCompare() {

    if (!modal)
        return;

    const select = $("#cmpSel", modal);
    const body = $("#cmpBody", modal);

    if (!select || !body)
        return;

    const id = select.dataset.id;
    const otherId = select.value;


    if (!id || !otherId) {

        body.innerHTML =
            "<p>Chọn một sản phẩm để bắt đầu so sánh.</p>";

        return;
    }

    try {

        body.innerHTML =
            "<p>Đang tải dữ liệu so sánh...</p>";

        const params = new URLSearchParams({
            id: id,
            otherId: otherId
        });

        const response = await fetch(
            `/Products/Compare?${params.toString()}`
        );


        if (!response.ok) {

            body.innerHTML =
                `<p>Không thể so sánh sản phẩm. HTTP ${response.status}</p>`;

            return;
        }

        body.innerHTML =
            await response.text();
    }
    catch (error) {

        console.error(
            "Lỗi compare:",
            error
        );

        body.innerHTML =
            "<p>Không thể kết nối tới máy chủ.</p>";
    }
}

// =========================================================
// FORMAT MONEY
// =========================================================

function formatMoney(value) {

    if (value == null)
        return "0 ₫";

    return Number(value).toLocaleString(
        "vi-VN"
    ) + " ₫";
}


    // =========================================================
    // CHATBOT
    // =========================================================

    function addMessage(text, isUser = false) {
        if (!chatMessages) return null;

        const message = document.createElement("div");
        message.className = isUser ? "msg me" : "msg bot";
        message.innerHTML = text.replace(/\n/g, "<br/>");

        chatMessages.appendChild(message);
        chatMessages.scrollTop = chatMessages.scrollHeight;
        return message;
    }

    async function sendMessage(customText = null) {
        if (!chatInput) return;

        const text = (customText || chatInput.value).trim();
        if (!text) return;

        addMessage(text, true);
        chatInput.value = "";

        // Typing indicator
        const typingEl = document.createElement("div");
        typingEl.className = "msg bot typing-indicator";
        typingEl.innerHTML = `<span class="dot-pulse">● ● ●</span> <span style="font-size: 11.5px; color: var(--mute);">Duy Mobile AI đang tư vấn...</span>`;
        chatMessages.appendChild(typingEl);
        chatMessages.scrollTop = chatMessages.scrollHeight;

        try {
            const response = await fetch("/api/chat", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    message: text
                })
            });

            typingEl.remove();

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            const data = await response.json();
            const bubble = addMessage(data.reply ?? "Duy Mobile sẵn sàng hỗ trợ bạn!");

            if (!bubble) return;

            if (data.items && data.items.length > 0) {
                const recContainer = document.createElement("div");
                recContainer.style.marginTop = "8px";
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
                    cardBtn.style.color = "var(--ac)";
                    cardBtn.style.fontWeight = "700";
                    cardBtn.style.fontSize = "12px";
                    cardBtn.style.textAlign = "left";
                    cardBtn.style.cursor = "pointer";
                    cardBtn.style.display = "flex";
                    cardBtn.style.justifyContent = "space-between";
                    cardBtn.style.alignItems = "center";
                    cardBtn.innerHTML = `<span>📱 ${product.name}</span> <span style="color:#dc2626; font-weight:800;">${product.price}</span>`;
                    
                    cardBtn.onclick = () => {
                        openProduct(product.id);
                    };

                    recContainer.appendChild(cardBtn);
                });

                bubble.appendChild(recContainer);
            }
            chatMessages.scrollTop = chatMessages.scrollHeight;
        }
        catch (error) {
            typingEl.remove();
            console.error("Lỗi chatbot:", error);
            addMessage("Không thể kết nối đến máy chủ tư vấn. Bạn có thể thử lại hoặc gọi Hotline 1800.6868 nhé!");
        }
    }


    function toggleChat() {

        if (!chat)
            return;

        chat.classList.toggle("open");

        if (
            chat.classList.contains("open") &&
            chatInput
        ) {
            chatInput.focus();
        }
    }


    function closeChat() {

        if (chat)
            chat.classList.remove("open");
    }


    // =========================================================
    // CLICK EVENTS
    // =========================================================

    document.addEventListener("click", event => {

        // ---------------------------------------------
        // Close modal
        // ---------------------------------------------

        const closeButton =
            event.target.closest("[data-close]");

        if (closeButton) {
            closeModal();
            return;
        }


        if (overlay && event.target === overlay) {
            closeModal();
            return;
        }


        // ---------------------------------------------
        // Modal navigation
        // ---------------------------------------------

        const viewButton =
            event.target.closest("[data-view]");

        if (viewButton) {
            showPane(viewButton.dataset.view);
            return;
        }


        // ---------------------------------------------
        // Open product explicitly
        // ---------------------------------------------

        const openButton =
            event.target.closest("[data-open]");

        if (openButton) {

            const id =
                openButton.dataset.open;

            openProduct(id);

            return;
        }


        // ---------------------------------------------
        // Product card
        // ---------------------------------------------

        const card =
            event.target.closest(".card[data-id]");

        if (card) {

            const id =
                card.dataset.id;

            openProduct(id);

            return;
        }


        // ---------------------------------------------
        // AI button
        // ---------------------------------------------

        const aiButton =
            event.target.closest("#heroAI");

        if (aiButton) {

            if (chat)
                chat.classList.add("open");

            if (chatInput)
                chatInput.focus();
        }
    });


    // =========================================================
    // CHANGE EVENTS
    // =========================================================

    document.addEventListener("change", event => {

        if (event.target.id === "cmpSel") {
            loadCompare();
        }
    });


    // =========================================================
    // KEYBOARD
    // =========================================================

    document.addEventListener("keydown", event => {

        if (event.key !== "Escape")
            return;

        closeModal();
        closeChat();
    });


    // =========================================================
    // CHAT EVENTS
    // =========================================================

    if (chatFab) {
        chatFab.addEventListener(
            "click",
            toggleChat
        );
    }


    if (chatButton) {
        chatButton.addEventListener(
            "click",
            sendMessage
        );
    }


    if (chatInput) {

        chatInput.addEventListener(
            "keydown",
            event => {

                if (event.key === "Enter") {
                    event.preventDefault();

                    sendMessage();
                }
            }
        );
})();

// ==========================================
// CART HELPER FUNCTIONS
// ==========================================
function quickAddToCart(e, maSp) {
    if (e) {
        e.stopPropagation();
        e.preventDefault();
    }
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
            alert(data.message || "Lỗi khi thêm giỏ hàng");
        }
    })
    .catch(() => alert("Không thể kết nối đến máy chủ!"));
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

function showCartToast(msg) {
    let toast = document.getElementById("cartToast");
    if (!toast) {
        toast = document.createElement("div");
        toast.id = "cartToast";
        toast.style.cssText = "position:fixed; bottom:24px; right:24px; background:#1e1e2f; color:#fff; padding:14px 22px; border-radius:12px; font-weight:600; font-size:14px; box-shadow:0 8px 24px rgba(0,0,0,.25); z-index:9999; transition:all .3s ease; opacity:0; transform:translateY(20px); border-left:4px solid #16a34a;";
        document.body.appendChild(toast);
    }
    toast.textContent = msg;
    toast.style.opacity = "1";
    toast.style.transform = "translateY(0)";
    setTimeout(() => {
        toast.style.opacity = "0";
        toast.style.transform = "translateY(20px)";
    }, 2500);
}