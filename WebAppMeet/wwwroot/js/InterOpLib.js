export function showAlert(title, message) {
    swal(title, message);
}

export function getElementText(id) {
    return document.getElementById(id).value;
}