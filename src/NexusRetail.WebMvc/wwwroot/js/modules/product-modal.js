/**
 * Duy Mobile - Product Modal & Comparison Module
 * Quản lý Modal Quick View và đối đầu so sánh cấu hình trực tiếp
 */
(() => {
    "use strict";

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
                    <div class="modal-header border-0 pb-0">
                        <h6 class="modal-title fw-bold text-danger">Thông báo</h6>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body text-center py-4">
                        <i class="bi bi-exclamation-circle text-danger fs-1 d-block mb-2"></i>
                        <h6 class="fw-bold">Không tìm thấy thông tin sản phẩm</h6>
                        <p class="small text-secondary mb-0">Mã sản phẩm: ${id}</p>
                    </div>
                `;
                return;
            }

            dialog.innerHTML = await response.text();
        } catch (error) {
            console.error("[ProductModal] Lỗi khi tải Quick View:", error);
            dialog.innerHTML = `
                <div class="modal-header border-0 pb-0">
                    <h6 class="modal-title fw-bold text-danger">Lỗi kết nối</h6>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body text-center py-4">
                    <p class="text-danger fw-semibold mb-0">Không thể kết nối đến máy chủ. Vui lòng thử lại sau.</p>
                </div>
            `;
        }
    }

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
            console.error("[ProductModal] Lỗi tải danh sách so sánh:", error);
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
                    <i class="bi bi-arrow-left-right fs-2 d-block mb-2 text-muted"></i>
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
            console.error("[ProductModal] Lỗi so sánh sản phẩm:", error);
            body.innerHTML = `<div class="p-3 text-danger text-center">Không thể kết nối máy chủ.</div>`;
        }
    }

    // Expose to window for global access
    window.openProduct = openProduct;
    window.closeModal = closeModal;
    window.loadCompareProducts = loadCompareProducts;
    window.loadCompare = loadCompare;
})();
