function switchTab(status) {
    const cards = document.querySelectorAll(".wf-card");
    const tabs = document.querySelectorAll(".wf-tab");

    tabs.forEach(t => t.classList.remove("active"));
    event.target.classList.add("active");

    cards.forEach(card => {
        const cardStatus = card.getAttribute("data-status");

        if (status === "all") {
            card.style.display = "block";
        } else {
            card.style.display = cardStatus === status ? "block" : "none";
        }
    });
}

function filterTasks() {
    const search = document.getElementById("taskSearch").value.toLowerCase();
    const cards = document.querySelectorAll(".wf-card");

    cards.forEach(card => {
        const title = card.getAttribute("data-title").toLowerCase();

        if (title.includes(search)) {
            card.style.display = "block";
        } else {
            card.style.display = "none";
        }
    });
}