document.addEventListener("DOMContentLoaded", () => {
    const theme = document.documentElement.getAttribute("data-theme") || "light";  // Tema bilgisini al

    const deleteSelectedBtn = document.getElementById('deleteSelectedBtn');
    if (!deleteSelectedBtn) return;

    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenElement ? tokenElement.value : "";

    deleteSelectedBtn.addEventListener('click', () => {
        const selectedCheckboxes = document.querySelectorAll('.row-checkbox:checked');
        if (selectedCheckboxes.length === 0) return;

        const ids = Array.from(selectedCheckboxes).map(cb => cb.dataset.id);

        Swal.fire({
            title: window.multiDeleteConfig.confirmTitle || "Seçilenleri silmek istediğinize emin misiniz?",
            text: "Bu işlemi geri alamazsınız!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Evet, sil!",
            cancelButtonText: "İptal",
            background: theme === "dark" ? "#1e1e2f" : "#fff",
            color: theme === "dark" ? "#fff" : "#000",
            allowOutsideClick: false,  // Ekran dışı tıklamayla kapanmasın
            allowEscapeKey: false,     // ESC tuşuyla kapanmasın
            allowEnterKey: false       // ENTER tuşuyla onaylama kapatma engellendi
        }).then(result => {
            if (result.isConfirmed) {
                fetch(window.multiDeleteConfig.deleteUrl, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "RequestVerificationToken": token
                    },
                    body: JSON.stringify(ids)
                })
                    .then(async res => {
                        const msg = await res.text();
                        const isSuccess = res.ok;

                        Swal.fire({
                            title: isSuccess ? "Silindi!" : "Silinemedi!",
                            text: msg,
                            icon: isSuccess ? "success" : "error",
                            confirmButtonText: "Tamam",
                            confirmButtonColor: "#d33",
                            customClass: {
                                popup: getSwalClass()
                            }
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
                            customClass: {
                                popup: getSwalClass()
                            }
                        });
                    });
            }
        });
    });
});
