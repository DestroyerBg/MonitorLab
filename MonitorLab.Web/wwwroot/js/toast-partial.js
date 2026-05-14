document.addEventListener("DOMContentLoaded", function () {
    const toastElement = document.getElementById("monitorlabToast");

if (toastElement) {
        const toast = new bootstrap.Toast(toastElement, {
    delay: 3500
        });

toast.show();
    }
});
