document.addEventListener("DOMContentLoaded", () => {
    const tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenEl ? tokenEl.value : "";
    const theme = document.documentElement.getAttribute("data-theme") || "light";

    function massUndoSoftDelete(endpoint, confirmTitle) {
        const selectedIds = Array.from(document.querySelectorAll('.row-checkbox:checked'))
            .map(cb => parseInt(cb.dataset.id));

        if (!selectedIds.length || !token || !endpoint) return;

        Swal.fire({
            title: confirmTitle,
            text: "Öğeler çöp kutusuna geri alınacak.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Evet, Geri al",
            cancelButtonText: "Hayır, İptal",
            background: theme === "dark" ? "#1e1e2f" : "#fff",
            color: theme === "dark" ? "#fff" : "#000",
            reverseButtons: true,
            allowOutsideClick: false,
            allowEscapeKey: false,
            allowEnterKey: false,
            didOpen: () => {
                const confirmBtn = Swal.getConfirmButton();
                const cancelBtn = Swal.getCancelButton();

                const addHoverEffect = (btn, color) => {
                    if (!btn) return;
                    btn.style.transition = "all 0.3s ease";
                    btn.addEventListener("mouseenter", () => {
                        btn.style.boxShadow = `0 4px 20px 0 ${color}80`;
                        btn.style.transform = "translateY(-2px)";
                    });
                    btn.addEventListener("mouseleave", () => {
                        btn.style.boxShadow = "none";
                        btn.style.transform = "translateY(0)";
                    });
                };

                addHoverEffect(confirmBtn, "#3085d6");
                addHoverEffect(cancelBtn, "#ff4d4f");
            }
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
                        showConfirmButton: false,
                        background: theme === "dark" ? "#1e1e2f" : "#fff",
                        color: theme === "dark" ? "#fff" : "#000",
                        allowOutsideClick: false,
                        allowEscapeKey: false,
                        allowEnterKey: false,
                        timer: isSuccess ? 1250 : 2500,
                        timerProgressBar: true,
                        didOpen: () => {
                            const pb = document.querySelector('.swal2-timer-progress-bar');
                            if (pb) {
                                pb.style.backgroundColor = isSuccess ? "#28a745" : "#dc3545";
                            }
                        }
                    }).then(() => {
                        if (isSuccess) window.location.reload();
                    });
                })
                .catch(() => {
                    Swal.fire({
                        title: "Sunucuya ulaşılamadı!",
                        text: "Lütfen tekrar deneyin.",
                        icon: "error",
                        showConfirmButton: false,
                        background: theme === "dark" ? "#1e1e2f" : "#fff",
                        color: theme === "dark" ? "#fff" : "#000",
                        allowOutsideClick: false,
                        allowEscapeKey: false,
                        allowEnterKey: false,
                        timer: 3000,
                        timerProgressBar: true,
                        didOpen: () => {
                            const pb = document.querySelector('.swal2-timer-progress-bar');
                            if (pb) {
                                pb.style.backgroundColor = "#dc3545";
                            }
                        }
                    });
                });
        });
    }

    const undoBtn = document.getElementById("undosoftdeleteSelectedBtn");
    if (!undoBtn) return;

    undoBtn.addEventListener("click", () => {
        const config = window.undoSoftDeleteConfig;
        if (!config || !config.url || !config.confirmTitle) return;

        massUndoSoftDelete(config.url, config.confirmTitle);
    });
});