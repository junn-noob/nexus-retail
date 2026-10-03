(() => {
  const $ = s => document.querySelector(s);
  const ov = $("#ov"), modal = $("#modal"), chat = $("#chat"), cm = $("#cm"), ci = $("#ci");

  // ---- Modal sản phẩm (nạp partial từ server, không chuyển trang) ----
  const closeModal = () => { ov.classList.remove("open"); document.body.style.overflow = ""; };
  async function openProduct(id) {
    const r = await fetch(`/Products/Quick/${id}`);
    if (!r.ok) return;
    modal.innerHTML = await r.text();
    ov.classList.add("open");
    document.body.style.overflow = "hidden";
  }
  function showPane(name) {
    modal.querySelectorAll("[data-pane]").forEach(e => e.hidden = e.dataset.pane !== name);
    modal.scrollTop = 0;
    if (name === "compare") loadCompare();
  }
  async function loadCompare() {
    const s = $("#cmpSel"); if (!s) return;
    const r = await fetch(`/Products/Compare?id=${s.dataset.id}&otherId=${s.value}`);
    $("#cmpBody").innerHTML = r.ok ? await r.text() : "";
  }

  document.addEventListener("click", e => {
    const card = e.target.closest(".card[data-id]");
    if (card) return openProduct(card.dataset.id);
    const v = e.target.closest("[data-view]");
    if (v) return showPane(v.dataset.view);
    const o = e.target.closest("[data-open]");
    if (o) return openProduct(o.dataset.open);
    if (e.target.closest("[data-close]") || e.target === ov) closeModal();
    if (e.target.closest("#heroAI")) { chat.classList.add("open"); ci.focus(); }
  });
  document.addEventListener("change", e => { if (e.target.id === "cmpSel") loadCompare(); });
  document.addEventListener("keydown", e => { if (e.key === "Escape") { closeModal(); chat.classList.remove("open"); } });

  // ---- Chatbot (gọi POST /api/chat) ----
  function add(text, user) {
    const d = document.createElement("div");
    d.className = "m" + (user ? " u" : "");
    d.textContent = text;
    cm.appendChild(d);
    cm.scrollTop = cm.scrollHeight;
    return d;
  }
  async function send() {
    const t = ci.value.trim(); if (!t) return;
    add(t, true); ci.value = "";
    try {
      const r = await fetch("/api/chat", { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ message: t }) });
      const data = await r.json();
      const bubble = add(data.reply);
      (data.items || []).forEach(p => {
        const b = document.createElement("button");
        b.dataset.open = p.id; b.textContent = `${p.name} — ${p.price}`;
        bubble.appendChild(b);
      });
    } catch { add("Không kết nối được máy chủ. Bạn thử lại sau nhé."); }
  }
  $("#fab").onclick = () => { chat.classList.toggle("open"); ci.focus(); };
  $("#cs").onclick = send;
  ci.addEventListener("keydown", e => { if (e.key === "Enter") send(); });
})();
