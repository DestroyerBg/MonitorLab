function toggleGuideSection(contentId, arrowId) {
    const content = document.getElementById(contentId);
    const arrow = document.getElementById(arrowId);

    const isHidden = content.style.display === "none" || content.style.display === "";

    if (isHidden) {
        content.style.display = "block";

        content.animate(
            [{ opacity: 0 }, { opacity: 1 }],
            { duration: 300, easing: "ease-in-out" }
        );

        arrow.classList.remove("bi-chevron-down");
        arrow.classList.add("bi-chevron-up");
    } else {
        const animation = content.animate(
            [{ opacity: 1 }, { opacity: 0 }],
            { duration: 200, easing: "ease-in-out" }
        );

        animation.onfinish = () => {
            content.style.display = "none";
        };

        arrow.classList.remove("bi-chevron-up");
        arrow.classList.add("bi-chevron-down");
    }
}