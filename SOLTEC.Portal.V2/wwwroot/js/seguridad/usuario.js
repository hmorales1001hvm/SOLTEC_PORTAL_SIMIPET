$(document).ready(function () {

    $("#btnLogin").click(function () {

        var usuario = $("#usuario").val().trim();
        var password = $("#password").val().trim();

        if (!usuario || !password) {
            M.toast({ html: 'Por favor ingrese usuario y contraseña', classes: 'red darken-1' });
            return;
        }

        var datos = {
            Usuario: usuario,
            Password: password
        };

        $.ajax({
            url: "https://localhost:7033/api/Usuarios/Login", // Cambia por tu URL real
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(datos),
            success: function (data) {
                if (data.success) {
                    // ✅ Login exitoso → redirigir a Inicio
                    window.location.href = "/Home/Inicio";
                } else {
                    // ❌ Credenciales incorrectas
                    Swal.fire({
                        toast: true,
                        position: 'top-end',
                        icon: 'warning',
                        title: '¡Usuario o contraseña incorrectos!',
                        background: '#fff3cd',
                        color: '#856404',
                        timer: 3000,
                        timerProgressBar: true,
                        showConfirmButton: false
                    });
                }
            },
            error: function (xhr, status, error) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'error',
                    title: '¡Ocurrió un error al procesar su solicitud!',
                    background: '#f8d7da',
                    color: '#721c24',
                    timer: 3000,
                    timerProgressBar: true,
                    showConfirmButton: false
                });
            }
        });

    });

});
