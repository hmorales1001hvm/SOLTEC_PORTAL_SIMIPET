
window.saveData = false;

const configuracionGlobal = {
    sUrlServidorUtil: '@ViewBag.ApiBaseUrl'
};

const FormDataTable = {
    language: {
        emptyTable: "No se encontraron registros para visualizar",
        info: "Mostrando registros del _START_ al _END_ de _TOTAL_ registros",
        infoEmpty: "Mostrando registros del 0 al 0 de 0 registros",
        infoFiltered: "(filtrado de un total de _MAX_ registros)",
        infoPostFix: "",
        thousands: ",",
        loadingRecords: "Cargando registros...",
        search: "Buscar:",
        processing: "Procesando...",
        zeroRecords: "No se encontraron resultados con los criterios de búsqueda especificados",
        paginate: {
            first: "Primero",
            last: "Último",
            next: "Siguiente",
            previous: "Anterior"
        },
        aria: {
            sortAscending: ": activar para ordenar la columna de manera ascendente",
            sortDescending: ": activar para ordenar la columna de manera descendente"
        },
        select: {
            rows: ""
        }
    },
    lengthChange: true,
    pageLength: 20

};


function ConfirmarAccion({
    titulo = '¿Estás seguro?',
    texto = 'Esta acción no se puede deshacer.',
    icono = 'warning',
    textoConfirmar = 'Sí',
    textoCancelar = 'Cancelar',
    callback = null
}) {
    Swal.fire({
        title: titulo,
        text: texto,
        icon: icono,
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: textoConfirmar,
        cancelButtonText: textoCancelar
    }).then((result) => {
        if (result.isConfirmed && typeof callback === 'function') {
            callback();
        }
    });
}


function ShowAlert(message, alertType) {
    if (alertType == undefined) {
        alertType = "Info";
    }
    SelectClass(alertType, message);
}

function SelectClass(mType, message) {
    var mClass = "";

    switch (mType) {
        case "Info":
            Swal.fire({
                icon: "info",
                title: "Aviso",
                text: message,
                confirmButtonText: "Aceptar",
                confirmButtonColor: '#42A5F5'
            });
            break;
        case "Error":
            Swal.fire({
                icon: "error",
                title: "Error",
                text: message,
                confirmButtonText: "Aceptar",
                confirmButtonColor: '#42A5F5'
            });
            break;
        case "Warning":
            Swal.fire({
                icon: "warning",
                title: "Aviso",
                text: message,
                confirmButtonText: "Aceptar",
                confirmButtonColor: '#42A5F5'
            });
            break;
        case "Success":
            Swal.fire({
                icon: "success",
                title: "Aviso",
                text: message,
                confirmButtonText: "Aceptar",
                confirmButtonColor: '#42A5F5'
            });
            break;
        default:
            Swal.fire({
                icon: "info",
                title: "Aviso",
                text: message,
                confirmButtonText: "Aceptar",
                confirmButtonColor: '#42A5F5'
            });
            break;
    }
}

function LoadSelect(cboName, datos, textoCampo, valorCampo, valorSeleccionado = null) {
    const $select = $('#' + cboName);
    $select.empty();

    // Si no hay valor seleccionado o es 0, mostrar "Seleccione..."
    if (valorSeleccionado === null || valorSeleccionado === 0)
        $select.append('<option value="" disabled selected>Seleccione...</option>');
    else
        $select.append('<option value="" disabled>Seleccione...</option>');

    // Agregar opción "Todos" con valor 0
    $select.append(`<option value="0" ${valorSeleccionado === 0 ? 'selected' : ''}>Tod@s</option>`);

    datos.forEach(item => {
        const valor = item[valorCampo];
        const texto = item[textoCampo];
        const selected = (valor == valorSeleccionado) ? 'selected' : '';
        $select.append(`<option value="${valor}" ${selected}>${texto}</option>`);
    });

    // Refrescar el select de Materialize
    $select.formSelect();
}

//function ShowLoading() {
//    $('body').css('overflow', 'hidden');
//    $('#loading-overlay').show();
//}

//function HideLoading() {
//    $('#loading-overlay').fadeOut(200, function () {
//        $('body').css('overflow', 'auto');
//    });
//}


function ShowLoading() {
    $('#loading-overlay').fadeIn(200);
}

function HideLoading() {
    $('#loading-overlay').fadeOut(200);
}


//const idleTimeLimit = 10000; // 10 segundos
//const imageInterval = 5000;  // 5 segundos

//const images = [
//    '/images/wallpaper/1.jpeg',
//    '/images/wallpaper/2.jpeg',
//    '/images/wallpaper/3.jpeg',
//    '/images/wallpaper/4.jpeg',
//    '/images/wallpaper/5.jpeg',
//    '/images/wallpaper/6.jpeg',
//    '/images/wallpaper/7.jpeg',
//    '/images/wallpaper/8.jpeg',
//    '/images/wallpaper/9.jpeg'
//];

//let idleTimer = null;
//let imageTimer = null;
//let currentImage = 0;

//const overlay = document.getElementById('idle-wallpaper');
//const img = document.getElementById('idle-image');

//function startIdleTimer() {
//    clearTimeout(idleTimer);
//    idleTimer = setTimeout(showWallpaper, idleTimeLimit);
//}

//function showWallpaper() {
//    overlay.style.display = 'block';
//    changeImage();

//    imageTimer = setInterval(changeImage, imageInterval);
//}

//function hideWallpaper() {
//    overlay.style.display = 'none';
//    clearInterval(imageTimer);
//    startIdleTimer();
//}

//function changeImage() {
//    currentImage = Math.floor(Math.random() * images.length);
//    img.src = images[currentImage];
//}

//// Eventos de actividad
//['mousemove', 'keydown', 'mousedown', 'scroll', 'touchstart']
//    .forEach(event => {
//        document.addEventListener(event, () => {
//            if (overlay.style.display === 'block') {
//                hideWallpaper();
//            }
//            startIdleTimer();
//        });
//    });

//// Iniciar
//startIdleTimer();