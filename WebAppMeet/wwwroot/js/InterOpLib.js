export function showAlert(title, message) {
    swal(title, message);
}

export function getElementText(id) {
    return document.getElementById(id).value;
}

export function scrollToEnd(textarea) {
    textarea.scrollTop = textarea.scrollHeight;
}