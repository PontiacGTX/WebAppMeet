export function showAlert(title, message) {
    swal(title, message);
}

export function getElementText(id) {
    return document.getElementById(id).value;
}

export function scrollToEnd(textarea) {
    textarea.scrollTop = textarea.scrollHeight;
}

export function exportToCanvas(base64,canvasId) {
    var myCanvas = document.getElementById(canvasId);
    console.log("canvas " + myCanvas);
    var ctx = myCanvas.getContext('2d');
    var img = new Image;
    img.onload = function () {
        ctx.drawImage(img, 0, 0); // Or at whatever offset you like
        myCanvas = document.getElementById(canvasId);
        console.log("canvas2 " + myCanvas);
    };

    img.src = base64;
    myCanvas = document.getElementById(canvasId);
    console.log("canvas1 " + myCanvas);
}

export function getLocalStorageToken() {
   let x= window.localStorage.getItem('accessToken.js');
    console.log(x);
    return x;
}

export function getLocalStorageTokenData() {
    var obj = window.localStorage.getItem('tokenData.js');
    return obj;

}