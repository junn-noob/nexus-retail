(() => {
  const side = document.getElementById("side"), scrim = document.getElementById("scrim"), toast = document.getElementById("toast");
  const open = on => { side.classList.toggle("open", on); scrim.classList.toggle("on", on); };
  document.getElementById("burger").onclick = () => open(true);
  scrim.onclick = () => open(false);
  if (toast.textContent.trim()) { toast.classList.add("on"); setTimeout(() => toast.classList.remove("on"), 2500); }
})();
