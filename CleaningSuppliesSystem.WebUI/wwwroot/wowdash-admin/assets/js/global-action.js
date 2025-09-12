document.addEventListener("DOMContentLoaded", () => {

    const deleteFunctionName = window.appConfig.deleteFunctionName;
    const deleteUrlPrefix = window.appConfig.deleteUrlPrefix;

    if (!deleteFunctionName || !deleteUrlPrefix) return;

    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    if (!token) return;

    const theme = document.documentElement.getAttribute("data-theme") || "light";
    const bgColor = theme === "dark" ? "#1e1e2f" : "#fff";
    const textColor = theme === "dark" ? "#fff" : "#000";

    // Timer progress bar destekli silme fonksiyonu
    window[deleteFunctionName] = async function (id) {
        if (!id) return;

        const confirmResult = await Swal.fire({
            title: "Öğe çöp kutusuna taşınsın mı?",
            text: "Öge çöp kutusundan geri alınabilir.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Evet, Sil",
            cancelButtonText: "Hayır, İptal",
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            background: bgColor,
            color: textColor,
            reverseButtons: true,
            allowOutsideClick: false,
            allowEscapeKey: false,
            allowEnterKey: false,
            didOpen: () => {
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
                addHoverEffect(Swal.getConfirmButton(), "#3085d6");
                addHoverEffect(Swal.getCancelButton(), "#ff4d4f");
            }
        });

        if (!confirmResult.isConfirmed) return;

        try {
            const res = await fetch(`${deleteUrlPrefix}${deleteUrlPrefix.endsWith("/") ? "" : "/"}${id}`, {
                method: "POST",
                headers: { "RequestVerificationToken": token }
            });

            const msg = await res.text();
            const isSuccess = res.ok;

            await Swal.fire({
                title: isSuccess ? "Silindi!" : "Silinemedi!",
                text: msg,
                icon: isSuccess ? "success" : "error",
                confirmButtonText: "Tamam",
                confirmButtonColor: isSuccess ? "#3085d6" : "#d33",
                background: bgColor,
                color: textColor,
                timer: isSuccess ? 1250 : 2500,
                timerProgressBar: true,
                showConfirmButton: false,
                allowOutsideClick: false,
                allowEscapeKey: false,
                allowEnterKey: false,
                didOpen: () => {
                    const progress = Swal.getPopup().querySelector('.swal2-timer-progress-bar');
                    if (progress) progress.style.background = isSuccess ? '#28a745' : '#dc3545';
                }
            });

            if (isSuccess) window.location.reload();

        } catch {
            Swal.fire({
                title: "İstek Hatası!",
                text: "Sunucuya ulaşılamadı. Lütfen tekrar deneyin.",
                icon: "error",
                confirmButtonText: "Tamam",
                confirmButtonColor: "#d33",
                background: bgColor,
                color: textColor,
                timer: 3000,
                timerProgressBar: true,
                showConfirmButton: false,
                allowOutsideClick: false,
                allowEscapeKey: false,
                allowEnterKey: false,
                didOpen: () => {
                    const progress = Swal.getPopup().querySelector('.swal2-timer-progress-bar');
                    if (progress) progress.style.background = '#dc3545';
                }
            });
        }
    };

    // Butonları dinle
    document.querySelectorAll(".delete-btn").forEach(btn => {
        btn.addEventListener("click", () => {
            const id = btn.dataset.id;
            if (id && typeof window[deleteFunctionName] === "function") {
                window[deleteFunctionName](id);
            }
        });
    });

});
