async function filterMonitors() {
    const searchTerm = document.getElementById("SearchTerm")?.value ?? "";
    const brand = document.getElementById("Brand")?.value ?? "";
    const resolution = document.getElementById("Resolution")?.value ?? "";
    const panelType = document.getElementById("PanelType")?.value ?? "";
    const minRefreshRate = document.getElementById("MinRefreshRate")?.value ?? "";

    const params = new URLSearchParams({
        searchTerm,
        brand,
        resolution,
        panelType,
        minRefreshRate
    });

    const response = await fetch(`/Monitors/Filter?${params.toString()}`);

    if (!response.ok) {
        console.log('Filter request failed');
        return;
    }

    const html = await response.text();

    document.getElementById("monitorGrid").innerHTML = html;
}
