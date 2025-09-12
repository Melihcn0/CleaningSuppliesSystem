document.addEventListener("DOMContentLoaded", () => {
    const permanentDeleteUrl = window.permanentDeleteConfig?.url;
    const confirmTitle = window.permanentDeleteConfig?.confirmTitle;
    const theme = document.documentElement.getAttribute("data-theme") || "light";
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    if (!permanentDeleteUrl || !token) return;

    const bgColor = theme === "dark" ? "#1e1e2f" : "#fff";
    const textColor = theme === "dark" ? "#fff" : "#000";

    const buttons = document.querySelectorAll(".permanent-delete-btn");
    if (!buttons.length) return;

    const handleDelete = (id) => {
        if (Number.isNaN(id)) return;

        Swal.fire({
            title: confirmTitle,
            text: "Bu işlem geri alınamaz!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Evet, Sil",
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

            fetch(`${permanentDeleteUrl}?id=${encodeURIComponent(id)}`, {
                method: "POST",
                headers: { "RequestVerificationToken": token }
            })
                .then(async res => {
                    const msg = await res.text();
                    const isSuccess = res.ok;

                    Swal.fire({
                        title: isSuccess ? "Başarılı!" : "Hata!",
                        text: msg,
                        icon: isSuccess ? "success" : "error",
                        showConfirmButton: false,
                        timer: isSuccess ? 1250 : 2500,
                        timerProgressBar: true,
                        background: bgColor,
                        color: textColor,
                        didOpen: () => {
                            const pb = Swal.getPopup().querySelector('.swal2-timer-progress-bar');
                            if (pb) pb.style.backgroundColor = isSuccess ? "#28a745" : "#dc3545";
                        },
                        willClose: () => {
                            if (isSuccess) window.location.reload();
                        }
                    });
                })
                .catch(() => {
                    Swal.fire({
                        title: "Sunucuya ulaşılamadı!",
                        text: "Lütfen tekrar deneyin.",
                        icon: "error",
                        showConfirmButton: false,
                        timer: 3000,
                        timerProgressBar: true,
                        background: bgColor,
                        color: textColor,
                        didOpen: () => {
                            const pb = Swal.getPopup().querySelector('.swal2-timer-progress-bar');
                            if (pb) pb.style.backgroundColor = "#dc3545";
                        }
                    });
                });
        });
    };

    buttons.forEach(btn => {
        btn.addEventListener("click", () => {
            const id = parseInt(btn.dataset.id);
            handleDelete(id);
        });
    });
});
