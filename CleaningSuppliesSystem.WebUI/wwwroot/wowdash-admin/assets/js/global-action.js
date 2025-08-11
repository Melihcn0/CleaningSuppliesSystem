document.addEventListener("DOMContentLoaded", () => {

    const deleteFunctionName = window.appConfig.deleteFunctionName;
    const deleteUrlPrefix = window.appConfig.deleteUrlPrefix;

    if (!deleteFunctionName || !deleteUrlPrefix) {
        console.warn("Delete function name veya url prefix bulunamadı.");
        return;
    }

    // CSRF token
    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenElement ? tokenElement.value : "";

    if (!token) {
        console.error("CSRF token bulunamadı!");
        return;
    }

    // Dinamik fonksiyon tanımla
    window[deleteFunctionName] = function (id) {
        const theme = document.documentElement.getAttribute("data-theme") || "light";

        Swal.fire({
            title: "Silmek istediğinizden emin misiniz?",
            text: "Bu işlemi geri alamazsınız!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Evet, sil!",
            cancelButtonText: "İptal",
            background: theme === "dark" ? "#1e1e2f" : "#fff",
            color: theme === "dark" ? "#fff" : "#000",
            allowOutsideClick: false,
            allowEscapeKey: false
        }).then(result => {
            if (result.isConfirmed) {
                const url = deleteUrlPrefix.endsWith("/") ? `${deleteUrlPrefix}${id}` : `${deleteUrlPrefix}/${id}`;

                fetch(url, {
                    method: "POST",
                    headers: {
                        "RequestVerificationToken": token
                    }
                })
                    .then(async res => {
                        const msg = await res.text();
                        const isSuccess = res.ok;

                        Swal.fire({
                            title: isSuccess ? "Silindi!" : "Silinemedi!",
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
                            title: "İstek Hatası!",
                            text: "Sunucuya ulaşılamadı. Lütfen tekrar deneyin.",
                            icon: "error",
                            confirmButtonText: "Tamam",
                            confirmButtonColor: "#d33",
                            background: theme === "dark" ? "#1e1e2f" : "#fff",
                            color: theme === "dark" ? "#fff" : "#000"
                        });
                    });
            }
        });
    };

    // Delete butonlarını dinle
    document.querySelectorAll(".delete-btn").forEach(btn => {
        btn.addEventListener("click", () => {
            const id = btn.dataset.id;
            if (id && typeof window[deleteFunctionName] === "function") {
                window[deleteFunctionName](id);
            } else {
                console.error("Buton üzerinde data-id yok veya fonksiyon bulunamadı.");
            }
        });
    });

});
