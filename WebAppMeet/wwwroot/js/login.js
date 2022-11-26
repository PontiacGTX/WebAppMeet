async function loginUser() {

    alert("fuck");

    let datos = {
        'Email': document.getElementById('Input_Email').value,
        'Password': document.getElementById('Input_Password').value
               'RememberMe': document.getElementById('Input_RememberMe').value
    }
    console.log(datos);
    let response = await fetch('/Identity/Account/Redirect?returnUrl=/identity/account/Logout', {
        method: "POST",
        body: JSON.stringify(datos),
        headers: { "Content-type": "application/json; charset=UTF-8" }
    });
    let json = await response.json();
    console.log(json);
    //        .then(response => response.json())
    //        .then(json => console.log(json));
    //.catch (err => console.log(err));
}