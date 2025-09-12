document.addEventListener("DOMContentLoaded", () => {
    const tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenEl ? tokenEl.value : "";
    const theme = document.documentElement.getAttribute("data-theme") || "light";

    function massAction(endpoint, confirmTitle, confirmTitle) {
        const selectedIds = Array.from(document.querySelectorAll('.row-checkbox:checked'))
            .map(cb => cb.dataset.id);

        if (!selectedIds.length || !token || !endpoint) return;

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
            allowEscapeKey: false,
            didOpen: () => {
                const confirmBtn = Swal.getConfirmButton();
                const cancelBtn = Swal.getCancelButton();
                [[confirmBtn, "#d33"], [cancelBtn, "#3085d6"]].forEach(([btn, color]) => {
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
                });
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
                        timer: isSuccess ? 1250 : 2500,
                        timerProgressBar: true,
                        background: theme === "dark" ? "#1e1e2f" : "#fff",
                        color: theme === "dark" ? "#fff" : "#000",
                        allowOutsideClick: false,
                        allowEscapeKey: false,
                        allowEnterKey: false,
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
                        background: theme === "dark" ? "#1e1e2f" : "#fff",
                        color: theme === "dark" ? "#fff" : "#000",
                        allowOutsideClick: false,
                        allowEscapeKey: false,
                        allowEnterKey: false,
                        didOpen: () => {
                            const pb = Swal.getPopup().querySelector('.swal2-timer-progress-bar');
                            if (pb) pb.style.backgroundColor = "#dc3545";
                        }
                    });
                });
        });
    }
});
