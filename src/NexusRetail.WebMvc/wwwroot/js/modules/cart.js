/**
 * Duy Mobile - Cart Module
 * Quản lý giỏ hàng: thêm vào giỏ, cập nhật số lượng, hiển thị thông báo toast
 */
(() => {
    "use strict";

    function showCartToast(msg, isSuccess = true) {
        if (typeof Swal !== "undefined") {
            const Toast = Swal.mixin({
                toast: true,
                position: "bottom-end",
                showConfirmButton: false,
                timer: 2600,
                timerProgressBar: true,
                background: "#0f172a",
                color: "#ffffff",
                didOpen: (toast) => {
                    toast.onmouseenter = Swal.stopTimer;
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });
            Toast.fire({
                icon: isSuccess ? "success" : "error",
                title: msg
            });
            return;
        }

        let toastEl = document.getElementById("cartToast");
        if (!toastEl) {
            toastEl = document.createElement("div");
            toastEl.id = "cartToast";
            toastEl.style.cssText = "position:fixed; bottom:28px; right:28px; background:#0f172a; color:#fff; padding:14px 22px; border-radius:14px; font-weight:600; font-size:14px; box-shadow:0 12px 32px rgba(0,0,0,.25); z-index:9999; transition:all .3s ease; opacity:0; transform:translateY(20px); display:flex; align-items:center; gap:8px;";
            document.body.appendChild(toastEl);
        }

        const borderCol = isSuccess ? "#10b981" : "#ef4444";
        const iconClass = isSuccess ? "bi bi-check-circle-fill text-success" : "bi bi-exclamation-circle-fill text-danger";
        
        toastEl.style.borderLeft = `4px solid ${borderCol}`;
        toastEl.innerHTML = `<i class="${iconClass} fs-5"></i> <span>${msg}</span>`;
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
                showCartToast(data.message || "Đã thêm sản phẩm vào giỏ hàng", true);
            } else {
                showCartToast(data.message || "Không thể thêm sản phẩm vào giỏ", false);
            }
        })
        .catch(err => {
            console.error("[Cart] Lỗi thêm giỏ hàng:", err);
            showCartToast("Lỗi kết nối máy chủ!", false);
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

    // Expose to window for global access
    window.showCartToast = showCartToast;
    window.quickAddToCart = quickAddToCart;
    window.addToCartFromModal = addToCartFromModal;
    window.buyNowFromModal = buyNowFromModal;
    window.updateCartBadge = updateCartBadge;

    document.addEventListener("DOMContentLoaded", updateCartBadge);
})();
