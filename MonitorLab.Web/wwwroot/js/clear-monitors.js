function clearMonitorFilters() {
    document.getElementById("SearchTerm").value = "";
    document.getElementById("Brand").value = "";
    document.getElementById("Resolution").value = "";
    document.getElementById("PanelType").value = "";
    document.getElementById("MinRefreshRate").value = "";

    filterMonitors();
}