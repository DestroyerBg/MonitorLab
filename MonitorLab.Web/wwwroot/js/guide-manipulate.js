document.addEventListener("DOMContentLoaded", () => {
    const hash = window.location.hash;

    if (!hash) {
        return;
    }

    const sectionId = hash.replace("#", "");

    const sections = {
        specs: ["specsContent", "specsArrow"],
        panels: ["panelsContent", "panelsArrow"],
        ports: ["portsContent", "portsArrow"],
        usage: ["usageContent", "usageArrow"],
        comparison: ["comparisonContent", "comparisonArrow"]
    };

    if (!sections[sectionId]) {
        return;
    }

    const [contentId, arrowId] = sections[sectionId];

    const content = document.getElementById(contentId);
    const arrow = document.getElementById(arrowId);

    if (!content || !arrow) {
        return;
    }

    content.style.display = "block";
    arrow.classList.remove("bi-chevron-down");
    arrow.classList.add("bi-chevron-up");

    setTimeout(() => {
        document.getElementById(sectionId)?.scrollIntoView({
            behavior: "smooth",
            block: "start"
        });
    }, 100);
});