$(document).ready(async function () {
    // Llamada inicial al cargar la página
    configuracionGlobal.sUrlServidorUtil = $("#apiBaseUrl").val();
    await CargaLogs();

    async function CargaLogs() {
        try {
            ShowLoading();
            const urlAPI = `${configuracionGlobal.sUrlServidorUtil}/api/Administracion/LeerLogRemoto`;

            // Petición GET
            const result = await $.ajax({
                method: 'GET',              // <-- Cambiado a GET
                url: urlAPI,
                dataType: 'json'
            });

            // Mostrar en la vista
            MostrarLogs(result);
            HideLoading();
            return result;
        } catch (error) {
            ShowAlert('Servicio de logs no disponible.', "Error");
            HideLoading();
            console.error(error);
        }
    }

    function MostrarLogs(lineas) {
        // Asumimos que tienes un <div id="logContainer"></div> en tu cshtml
        const container = $('#logContainer');
        container.empty();

        lineas.forEach(linea => {
            // Escapar caracteres HTML para evitar problemas
            container.append(`<div>${linea.replace(/</g, "&lt;").replace(/>/g, "&gt;")}</div>`);
        });

        // Opcional: hacer scroll al final para ver las últimas líneas
        container.scrollTop(container[0].scrollHeight);
    }
});
