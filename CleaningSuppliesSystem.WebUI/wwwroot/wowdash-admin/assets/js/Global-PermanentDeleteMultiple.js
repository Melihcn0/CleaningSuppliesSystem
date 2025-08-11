document.addEventListener("DOMContentLoaded", () => {
    const tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenEl ? tokenEl.value : "";
    const theme = document.documentElement.getAttribute("data-theme") || "light";

    function massPermanentDelete(endpoint, confirmTitle) {
        const selectedIds = Array.from(document.querySelectorAll('.row-checkbox:checked'))
            .map(cb => parseInt(cb.dataset.id))
            .filter(id => !isNaN(id));

        if (!selectedIds.length) {
            Swal.fire({
                title: "Seçim Yapılmadı!",
                text: "Lütfen en az bir öğe seçin.",
                icon: "warning",
                confirmButtonText: "Tamam",
                background: theme === "dark" ? "#1e1e2f" : "#fff",
                color: theme === "dark" ? "#fff" : "#000"
            });
            return;
        }

        if (!token || !endpoint) {
            console.warn("Token veya endpoint eksik!");
            return;
        }

        Swal.fire({
            title: confirmTitle,
            text: "Bu işlemi geri alamazsınız!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Evet",
            cancelButtonText: "İptal",
            background: theme === "dark" ? "#1e1e2f" : "#fff",
            color: theme === "dark" ? "#fff" : "#000",
            allowOutsideClick: false,
            allowEscapeKey: false
        }).then(result => {
            if (!result.isConfirmed) return;

            fetch(endpoint, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": token
                },
                body: JSON.stringify(selectedIds)
            })
                .then(async res => {
                    const msg = await res.text();
                    const isSuccess = res.ok;

                    Swal.fire({
                        title: isSuccess ? "Başarılı!" : "Hata!",
                        text: msg,
                        icon: isSuccess ? "success" : "error",
                        confirmButtonText: "Tamam",
                        confirmButtonColor: isSuccess ? "#3085d6" : "#d33",
                        background: theme === "dark" ? "#1e1e2f" : "#fff",
                        color: theme === "dark" ? "#fff" : "#000"
                    }).then(() => {
                        if (isSuccess) window.location.reload();
                    });
                })
                .catch(() => {
                    Swal.fire({
                        title: "Sunucuya ulaşılamadı!",
                        text: "Lütfen tekrar deneyin.",
                        icon: "error",
                        confirmButtonText: "Tamam",
                        confirmButtonColor: "#d33",
                        background: theme === "dark" ? "#1e1e2f" : "#fff",
                        color: theme === "dark" ? "#fff" : "#000"
                    });
                });
        });
    }

    const permDeleteBtn = document.getElementById("permanentdeleteSelectedBtn");
    if (!permDeleteBtn) {
        console.warn("Kalıcı silme butonu bulunamadı.");
        return;
    }

    permDeleteBtn.addEventListener("click", () => {
        const config = window.permanentDeleteMultipleConfig;
        if (!config || !config.url || !config.confirmTitle) {
            console.warn("permanentDeleteMultipleConfig eksik.");
            return;
        }

        massPermanentDelete(config.url, config.confirmTitle);
    });
});
