/**
 * Duy Mobile - Core Site Orchestrator
 * Coordinates global shortcuts and unified AJAX product filtering/pagination.
 */
(() => {
    "use strict";

    // Global keyboard shortcuts (Esc to close modals/chat, Ctrl+K or / to focus search)
    document.addEventListener("keydown", event => {
        if (event.key === "Escape") {
            if (typeof window.closeChat === "function") window.closeChat();
            if (typeof window.closeModal === "function") window.closeModal();
        } else if (
            (event.ctrlKey && event.key === "k") ||
            (event.key === "/" && !["INPUT", "TEXTAREA", "SELECT"].includes(document.activeElement?.tagName))
        ) {
            event.preventDefault();
            const searchInput = document.querySelector(".dm-search-input");
            if (searchInput) {
                searchInput.focus();
                searchInput.select();
            }
        }
    });

    // Unified AJAX catalog filtering & pagination (Fetch API + Razor PartialView)
    const params = new URLSearchParams(window.location.search);
    const catalogState = {
        brand: params.get("brand") || "",
        price: params.get("price") || "",
        q: params.get("q") || "",
        page: Math.max(1, parseInt(params.get("page") || "1", 10) || 1)
    };

    let activeAbort = null;

    const syncActiveBrandPill = () => {
        document.querySelectorAll(".dm-brand-pill").forEach(el => {
            const isMatch = (el.dataset.id || "") === catalogState.brand;
            el.classList.toggle("active", isMatch);
        });
    };

    const syncActivePriceLinks = () => {
        document.querySelectorAll(".dm-filter-opt[data-price]").forEach(el => {
            const p = el.dataset.price || "";
            const isMatch = p === catalogState.price;
            el.classList.toggle("active", isMatch);
            el.querySelector(".dm-price-check")?.classList.toggle("d-none", !isMatch);

            const qs = new URLSearchParams();
            qs.set("page", "1");
            if (p) qs.set("price", p);
            if (catalogState.q) qs.set("q", catalogState.q);
            if (catalogState.brand) qs.set("brand", catalogState.brand);
            el.setAttribute("href", `/?${qs.toString()}#productCatalog`);
        });
    };

    const syncFilterChrome = () => {
        document.querySelectorAll(".dm-search-form input[name='q']").forEach(inp => {
            inp.value = catalogState.q;
        });

        const hasFilters = Boolean(catalogState.brand || catalogState.price || catalogState.q);
        document.getElementById("btnResetFilters")?.classList.toggle("d-none", !hasFilters);

        const qBox = document.getElementById("activeQueryBox");
        const qTxt = document.getElementById("activeQueryText");
        if (qBox && qTxt) {
            qBox.classList.toggle("d-none", !catalogState.q);
            qTxt.textContent = catalogState.q ? `"${catalogState.q}"` : "";
        }

        const bBox = document.getElementById("activeBrandBox");
        const bTxt = document.getElementById("activeBrandText");
        if (bBox && bTxt) {
            bBox.classList.toggle("d-none", !catalogState.brand);
            const activePill = document.querySelector(`.dm-brand-pill[data-id="${CSS.escape(catalogState.brand)}"]`);
            bTxt.textContent = catalogState.brand
                ? (activePill ? activePill.textContent.trim() : catalogState.brand)
                : "";
        }
    };

    const filterCatalog = async (overrides = {}) => {
        Object.assign(catalogState, overrides);

        const resultsContainer = document.getElementById("productCatalogResults");
        if (!resultsContainer) {
            const query = new URLSearchParams();
            if (catalogState.brand) query.set("brand", catalogState.brand);
            if (catalogState.price) query.set("price", catalogState.price);
            if (catalogState.q) query.set("q", catalogState.q);
            const qs = query.toString();
            window.location.href = (qs ? `/?${qs}` : "/") + "#productCatalog";
            return;
        }

        syncActiveBrandPill();
        syncActivePriceLinks();
        syncFilterChrome();

        if (activeAbort) activeAbort.abort();
        activeAbort = new AbortController();

        const grid = document.getElementById("productGrid");
        if (grid) {
            grid.innerHTML = `
                <div class="col-12 py-5 text-center">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Đang tải...</span>
                    </div>
                    <div class="mt-3 text-secondary fw-semibold">Đang nạp danh sách sản phẩm...</div>
                </div>`;
        }

        const qs = new URLSearchParams();
        qs.set("page", String(catalogState.page));
        if (catalogState.brand) qs.set("brand", catalogState.brand);
        if (catalogState.price) qs.set("price", catalogState.price);
        if (catalogState.q) qs.set("q", catalogState.q);

        try {
            const response = await fetch(`/Home/FilterProducts?${qs.toString()}`, {
                signal: activeAbort.signal,
                headers: { "X-Requested-With": "XMLHttpRequest" }
            });
            if (!response.ok) throw new Error(`HTTP ${response.status}`);

            resultsContainer.innerHTML = await response.text();

            const urlParams = new URLSearchParams();
            if (catalogState.brand) urlParams.set("brand", catalogState.brand);
            if (catalogState.price) urlParams.set("price", catalogState.price);
            if (catalogState.q) urlParams.set("q", catalogState.q);
            if (catalogState.page > 1) urlParams.set("page", String(catalogState.page));
            const newUrl = urlParams.toString() ? `/?${urlParams.toString()}#productCatalog` : "/#productCatalog";
            window.history.replaceState(null, "", newUrl);

            document.getElementById("productCatalog")?.scrollIntoView({ behavior: "smooth", block: "start" });
        } catch (err) {
            if (err.name === "AbortError") return;
            console.error("[DuyMobile] Lỗi tải sản phẩm:", err);
            if (grid) {
                grid.innerHTML = `
                    <div class="col-12 py-5 text-center text-danger">
                        <i class="bi bi-exclamation-triangle fs-1 d-block mb-2"></i>
                        <h5>Không thể kết nối đến máy chủ.</h5>
                        <p class="small text-secondary">Vui lòng thử lại sau giây lát.</p>
                    </div>`;
            }
        }
    };

    window.locTheoNhanHieu = (maNhanHieu, _element, page = 1) => {
        filterCatalog({ brand: maNhanHieu || "", page: page || 1 });
    };

    document.addEventListener("DOMContentLoaded", () => {
        syncActiveBrandPill();
        syncActivePriceLinks();
        syncFilterChrome();

        // Tự động ẩn toast thông báo trạng thái sau 4.5 giây để tránh che nút thao tác
        document.querySelectorAll(".toast.show").forEach(toastEl => {
            setTimeout(() => {
                if (typeof bootstrap !== "undefined" && bootstrap.Toast) {
                    bootstrap.Toast.getOrCreateInstance(toastEl).hide();
                } else {
                    toastEl.classList.remove("show");
                }
            }, 4500);
        });

        document.querySelectorAll(".dm-search-form").forEach(form => {
            form.addEventListener("submit", event => {
                if (!document.getElementById("productCatalogResults")) return;
                event.preventDefault();
                const val = (form.querySelector("input[name='q']")?.value || "").trim();
                filterCatalog({ q: val, page: 1 });
            });
        });

        document.addEventListener("click", event => {
            const brandLink = event.target.closest(".dm-brand-pill[data-id]");
            if (brandLink) {
                event.preventDefault();
                filterCatalog({ brand: brandLink.dataset.id || "", page: 1 });
                return;
            }

            if (!document.getElementById("productCatalogResults")) return;

            const priceLink = event.target.closest(".dm-filter-opt[data-price]");
            if (priceLink) {
                event.preventDefault();
                filterCatalog({ price: priceLink.dataset.price || "", page: 1 });
                return;
            }

            const pageLink = event.target.closest("#paginationArea a[data-page]");
            if (pageLink) {
                event.preventDefault();
                const targetPage = parseInt(pageLink.dataset.page || "1", 10) || 1;
                filterCatalog({ page: targetPage });
                return;
            }

            const resetLink = event.target.closest("[data-reset-filter]");
            if (resetLink) {
                event.preventDefault();
                filterCatalog({ brand: "", price: "", q: "", page: 1 });
            }
        });
    });
})();
