document.addEventListener("DOMContentLoaded", () => {
    const permanentDeleteUrl = window.permanentDeleteConfig?.url;
    const confirmTitle = window.permanentDeleteConfig?.confirmTitle || "Bu öğeyi silmek istediğinize emin misiniz?";
    const theme = document.documentElement.getAttribute("data-theme") || "light";
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    console.log("CSRF token:", token);

    const bgColor = theme === "dark" ? "#1e1e2f" : "#fff";
    const textColor = theme === "dark" ? "#fff" : "#000";

    if (!permanentDeleteUrl) {
        console.warn("❗ permanentDeleteUrl tanımlı değil.");
        return;
    }

    if (!token) {
        console.warn("❗ CSRF token bulunamadı.");
        return;
    }

    const buttons = document.querySelectorAll(".permanent-delete-btn");

    if (!buttons.length) {
        console.warn("❗ Hiçbir permanent-delete-btn bulunamadı.");
        return;
    }

    buttons.forEach(btn => {
        btn.addEventListener("click", () => {
            const id = parseInt(btn.dataset.id);
            console.log("Silinecek ID:", id);
            if (Number.isNaN(id)) {
                console.warn("❗ Geçersiz ID:", btn.dataset.id);
                return;
            }

            Swal.fire({
                title: confirmTitle,
                text: "Bu işlemi geri alamazsınız!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#d33",
                cancelButtonColor: "#3085d6",
                confirmButtonText: "Evet, sil!",
                cancelButtonText: "İptal",
                background: theme === "dark" ? "#1e1e2f" : "#fff",
                color: theme === "dark" ? "#fff" : "#000",
                allowOutsideClick: false,
                allowEscapeKey: false
            }).then(result => {
                if (!result.isConfirmed) return;

                fetch(`${permanentDeleteUrl}?id=${encodeURIComponent(id)}`, {
                    method: "POST",
                    headers: {
                        "RequestVerificationToken": token
                    },
                    credentials: "same-origin"
                })
                    .then(res => res.text().then(msg => ({ ok: res.ok, msg })))
                    .then(({ ok, msg }) => {
                        Swal.fire({
                            title: ok ? "Başarılı!" : "Hata!",
                            text: msg || (ok ? "Silme işlemi başarılı." : "Beklenmeyen bir hata oluştu."),
                            icon: ok ? "success" : "error",
                            background: bgColor,
                            color: textColor
                        }).then(() => {
                            if (ok) window.location.reload();
                        });
                    })
                    .catch(() => {
                        Swal.fire({
                            title: "Sunucuya ulaşılamadı!",
                            text: "Lütfen tekrar deneyin.",
                            icon: "error",
                            background: bgColor,
                            color: textColor
                        });
                    });
            });
        });
    });
});
