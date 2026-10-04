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

    console.log("So sánh:", id, otherId);

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

        console.log(
            "Compare status:",
            response.status
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

        if (!chatMessages)
            return null;

        const message = document.createElement("div");

        message.className = isUser ? "m u" : "m";
        message.textContent = text;

        chatMessages.appendChild(message);

        chatMessages.scrollTop =
            chatMessages.scrollHeight;

        return message;
    }


    async function sendMessage() {

        if (!chatInput)
            return;

        const text = chatInput.value.trim();

        if (!text)
            return;

        addMessage(text, true);

        chatInput.value = "";

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


            if (!response.ok) {
                throw new Error(
                    `HTTP ${response.status}`
                );
            }


            const data = await response.json();

            const bubble = addMessage(
                data.reply ?? "Không có phản hồi."
            );


            if (!bubble)
                return;


            (data.items ?? []).forEach(product => {

                const button =
                    document.createElement("button");

                button.type = "button";

                button.dataset.open =
                    product.id;

                button.textContent =
                    `${product.name} — ${product.price}`;

                bubble.appendChild(button);
            });
        }
        catch (error) {

            console.error(
                "Lỗi chatbot:",
                error
            );

            addMessage(
                "Không kết nối được máy chủ. Bạn thử lại sau nhé."
            );
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
    }

})();