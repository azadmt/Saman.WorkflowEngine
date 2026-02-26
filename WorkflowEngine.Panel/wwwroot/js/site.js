window.getFormValues = (formId) => {
    const form = document.getElementById(formId);
    if (!form) return {};

    const formData = new FormData(form);
    const result = {};

    for (const [key, value] of formData.entries()) {
        result[key] = value;
    }

    return result;
};