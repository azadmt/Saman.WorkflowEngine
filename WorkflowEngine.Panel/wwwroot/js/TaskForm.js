window.submitTask = async function (taskId) {

    console.log("submitTask called with id:", taskId);

    const form = document.getElementById("taskForm");
    if (!form) {
        alert("فرم پیدا نشد");
        return;
    }

    const elements = form.querySelectorAll("input, select, textarea");
    const formDataObj = {};

    elements.forEach(el => {
        if (!el.name) return;

        // File input
        if (el.type === "file") {
            if (el.files && el.files.length > 0) {
                formDataObj[el.name] = el.files[0].name;
            } else {
                formDataObj[el.name] = "";
            }
            return;
        }

        // Multi select
        if (el.multiple) {
            const values = Array.from(el.selectedOptions).map(o => o.value);
            formDataObj[el.name] = values;
            return;
        }

        // Normal inputs
        formDataObj[el.name] = el.value ?? "";
    });

    const payload = {
        formData: formDataObj
    };

    try {
        const response = await fetch(`http://localhost:5020/api/tasks/${taskId}/complete`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });
        console.log(response);
        if (response.ok) {
            window.location.href = "/tasks";
        } else {
            const text = await response.text();
            console.error("API Error:", text);
            alert("خطا در ثبت وظیفه");
        }
    } catch (error) {
        console.error("Network Error:", error);
        alert("خطا در ارتباط با سرور");
    }
};