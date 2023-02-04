document.getElementById("logoutLink").addEventListener("click", function() {
    localStorage.removeItem("accessToken");

    localStorage.removeItem("tokenData.js");
    console.log('cleared local strorage');
});