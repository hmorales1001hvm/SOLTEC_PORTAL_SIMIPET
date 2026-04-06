$(document).ready(async function () {
    let id = 0;
    let idUsuario = $("#txtUsuario").val();
    $("#btnProcesarTodo").hide();
    configuracionGlobal.sUrlServidorUtil = $("#apiBaseUrl").val();
    $('.timepicker').timepicker();

    $('.datatable').DataTable({
        responsive: true,
        scrollX: true
    });

    $('#cboTipo, #cboTipoDetalle').append(`
    <option value="1">Normal</option>
    <option value="2">Históricos (Venta en línea)</option>
    <option value="3">Históricos (ON DEMAND)</option>
`);

    $('#cboEstatus').append(`
    <option value="TODOS">TODOS</option>
    <option value="PENDIENTE">PENDIENTE</option>
    <option value="RECIBIDO">RECIBIDO</option>
    <option value="PROCESANDO...">PROCESANDO...</option>
    <option value="PROCESADO">PROCESADO</option>
`);

    const modales = document.querySelectorAll('.modal');
    M.Modal.init(modales, {
        opacity: 0.6,
        inDuration: 1200,
        outDuration: 800,
        dismissible: false, 
        startingTop: '4%',
        endingTop: '15%'
    });
    $('select').formSelect();

    var $select = $("#cboHora");

    for (var h = 0; h < 24; h++) {
        for (var m = 0; m < 60; m += 10) {

            var hora = (h < 10 ? "0" : "") + h;
            var minuto = (m < 10 ? "0" : "") + m;

            var time = hora + ":" + minuto;

            $select.append($('<option>', {
                value: time,
                text: time
            }));
        }
    }

    if (M && M.FormSelect) {
        M.FormSelect.init($select);
    }

    function quitarTodos() {
        $('#cboEmpresaDetalle, #cboSucursalDetalle, #cboTipoDetalle')
            .find('option[value="0"]').remove();
    }

    async function CargaEmpresas() {
        try {
            ShowLoading();
            const urlAPI = `${configuracionGlobal.sUrlServidorUtil}/api/Administracion/Empresas`;
            const result = await $.ajax({
                method: 'POST',
                url: urlAPI,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ IdEmpresa: 0 })
            });

            if (!result.exito) return ShowAlert(result.Mensaje, "Warning");

            if (result.datosLista.length > 0) {
                LoadSelect("cboEmpresa", result.datosLista, "empresa", "idEmpresa", 0);
                LoadSelect("cboEmpresaDetalle", result.datosLista, "empresa", "idEmpresa", 0);
                M.updateTextFields();
            } else {
                ShowAlert('No se encontraron empresas.', "Warning");
            }
            return result;
        } catch (error) {
            ShowAlert('Servicio de empresas no disponible.', "Error");
            console.error(error);
        } finally {

        }
    }

    let idSucursalSeleccionado = []; 
    let idSucursalSeleccionadoInicial = [];

    async function CargaSucursales(idEmpresa, llaveUnica) {
        try {
            const urlAPI = `${configuracionGlobal.sUrlServidorUtil}/api/Administracion/Sucursales`;
            const result = await $.ajax({
                method: 'POST',
                url: urlAPI,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ IdEmpresa: idEmpresa, IdUsuario: idUsuario, LlaveUnica: llaveUnica })
            });

            if (!result.datosLista || result.datosLista.length === 0) return;

            idSucursalSeleccionado = result.datosLista
                .filter(x => x.seleccionada === 1 || x.seleccionada === true)
                .map(x => x.clave);

            idSucursalSeleccionadoInicial = [...idSucursalSeleccionado];

            if ($.fn.DataTable.isDataTable('#tblSucusalesDetalle')) {
                $('#tblSucusalesDetalle').DataTable().destroy();
                $('#tblSucusalesDetalle tbody').empty();
            }

            const table = $('#tblSucusalesDetalle').DataTable({
                data: result.datosLista,
                destroy: true,
                colReorder: true,
                responsive: true,
                columns: [
                    {
                        data: 'clave',
                        render: function (data) {
                            const checked = idSucursalSeleccionado.includes(data) ? 'checked' : '';
                            return `<label><input type="checkbox" class="filled-in chk-clave" value="${data}" ${checked}><span></span></label>`;
                        },
                        className: 'center-align'
                    },
                    { data: 'clave' },
                    { data: 'sucursal' }
                ],
                autoWidth: false,
                pageLength: 12,
                lengthChange: false,
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'copy',
                        text: '<i class="material-icons left">content_copy</i> Copiar',
                        titleAttr: 'Copiar al portapapeles',
                        className: 'btn waves-effect waves-light teal lighten-1',
                        exportOptions: { columns: ':visible' }
                    },
                    {
                        extend: 'excel',
                        text: '<i class="material-icons left">grid_on</i> Excel',
                        titleAttr: 'Exportar a Excel',
                        className: 'btn waves-effect waves-light green darken-2',
                        exportOptions: { columns: ':visible' }
                    },
                    {
                        extend: 'csv',
                        text: '<i class="material-icons left">description</i> CSV',
                        titleAttr: 'Exportar a CSV',
                        className: 'btn waves-effect waves-light blue darken-1',
                        exportOptions: { columns: ':visible' }
                    },
                    {
                        extend: 'pdf',
                        text: '<i class="material-icons left">picture_as_pdf</i> PDF',
                        titleAttr: 'Exportar a PDF',
                        className: 'btn waves-effect waves-light red darken-2',
                        exportOptions: { columns: ':visible' }
                    },
                    {
                        extend: 'print',
                        text: '<i class="material-icons left">print</i>Imprimir',
                        titleAttr: 'Imprimir tabla',
                        className: 'btn waves-effect waves-light blue',
                        exportOptions: { columns: ':visible' }
                    }
                ],
                language: {
                    ...FormDataTable.language,
                    buttons: {
                        copyTitle: 'Copiado al portapapeles',
                        copySuccess: {
                            _: 'Se copiaron %d filas al portapapeles',
                            1: 'Se copió 1 fila al portapapeles'
                        },
                        copy: 'Copiar',
                        excel: 'Excel',
                        csv: 'CSV',
                        pdf: 'PDF',
                        print: 'Imprimir'
                    }
                }
            });

            $('#chkSeleccionarTodo').off('change').on('change', function () {
                const isChecked = $(this).is(':checked');

                table.rows().every(function () {
                    const data = this.data();
                    const clave = data.clave;

                    if (isChecked) {
                        if (!idSucursalSeleccionado.includes(clave)) idSucursalSeleccionado.push(clave);
                    } else {
                        idSucursalSeleccionado = idSucursalSeleccionado.filter(x => x !== clave);
                    }

                    const $node = $(this.node());
                    $node.find('input.chk-clave').prop('checked', isChecked);
                });
            });

            $('#tblSucusalesDetalle tbody').off('change', 'input.chk-clave').on('change', 'input.chk-clave', function () {
                const clave = $(this).val();
                if ($(this).is(':checked')) {
                    if (!idSucursalSeleccionado.includes(clave)) idSucursalSeleccionado.push(clave);
                } else {
                    idSucursalSeleccionado = idSucursalSeleccionado.filter(x => x !== clave);
                }

                const total = table.rows().count();
                const checked = idSucursalSeleccionado.length;
                $('#chkSeleccionarTodo').prop('checked', total === checked);
            });

            const totalInicial = table.rows().count();
            const checkedInicial = idSucursalSeleccionado.length;
            $('#chkSeleccionarTodo').prop('checked', totalInicial === checkedInicial);

        } catch (error) {
            ShowAlert('Servicio de sucursales no disponible.', "Error");
            console.error(error);
        }
    }
    function ObtenerSucursalesSeleccionadas() {
        return idSucursalSeleccionado;
    }

    async function CargaLista(idEmpresa, idSQLScript) {
        try {
            const idTipo = parseInt($('#cboTipo').val()) || 0;
            const estatus = $('#cboEstatus').val() || "TODOS";

            const urlAPI = `${configuracionGlobal.sUrlServidorUtil}/api/Administracion/TransmisionesHistoricos`;
            const result = await $.ajax({
                method: 'POST',
                url: urlAPI,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ IdEmpresa: idEmpresa, IdSQLScript: idSQLScript, IdUsuario: idUsuario, TipoCarga: idTipo, Estatus: estatus  })
            });

            $('#tblTransmisiones').DataTable({
                headers: FormDataTable,
                data: result.datosLista,
                destroy: true,
                colReorder: true,
                responsive: true,
                columns: RetriveColumns(),
                columnDefs: [
                    { className: 'center-align', targets: [8, 9,10] }
                ],
                autoWidth: false,
                select: true,
                pageLength: 100000,
                lengthChange: true,
                lengthMenu: [
                    [5, 10, 25, 50, -1],
                    [5, 10, 25, 50, "Todos"]
                ],
                scrollY: 500,
                scroller: true,
                dom: "<'row valign-wrapper'<'col s5 dt-buttons-container'B><'col s4'l'<'select-wrapper'>>" +
                    "<'col s3 right-align'f>>" +
                    "t" +
                    "<'row'<'col s6'i><'col s6'p>>",

                buttons: [
                    {
                        extend: 'copy',
                        text: '<i class="material-icons left">content_copy</i> Copiar',
                        titleAttr: 'Copiar al portapapeles',
                        className: 'btn waves-effect waves-light teal lighten-1',
                        exportOptions: { columns: ':visible' }
                    },
                    {
                        extend: 'excel',
                        text: '<i class="material-icons left">grid_on</i> Excel',
                        titleAttr: 'Exportar a Excel',
                        className: 'btn waves-effect waves-light green darken-2',
                        exportOptions: { columns: ':visible' }
                    },
                    {
                        extend: 'csv',
                        text: '<i class="material-icons left">description</i> CSV',
                        titleAttr: 'Exportar a CSV',
                        className: 'btn waves-effect waves-light blue darken-1',
                        exportOptions: { columns: ':visible' }
                    },
                    {
                        extend: 'pdf',
                        text: '<i class="material-icons left">picture_as_pdf</i> PDF',
                        titleAttr: 'Exportar a PDF',
                        className: 'btn waves-effect waves-light red darken-2',
                        exportOptions: { columns: ':visible' }
                    },
                    {
                        extend: 'print',
                        text: '<i class="material-icons left">print</i>Imprimir',
                        titleAttr: 'Imprimir tabla',
                        className: 'btn waves-effect waves-light blue',
                        exportOptions: { columns: ':visible' }
                    }
                ],

                language: {
                    ...FormDataTable.language,
                    lengthMenu: "Mostrar: _MENU_",
                    buttons: {
                        copyTitle: 'Copiado al portapapeles',
                        copySuccess: { _: 'Se copiaron %d filas al portapapeles', 1: 'Se copió 1 fila al portapapeles' },
                        copy: 'Copiar', excel: 'Excel', csv: 'CSV', pdf: 'PDF', print: 'Imprimir'
                    },
                    info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
                },

                initComplete: function () {
                    const selects = document.querySelectorAll('.dataTables_length select');
                    M.FormSelect.init(selects);

                    $('.dataTables_length').css({
                        display: 'flex',
                        alignItems: 'center',
                        gap: '0.5rem'
                    });
                }
            });
            actualizarBotonProcesarTodo();
            setTimeout(() => {
                $('.tooltipped').tooltip();
            }, 100);

        } catch (error) {
            ShowAlert('Servicio de transmisiones no disponible.', "Error");
            console.error(error);
        }
    }

    let ejecucionEnCurso = false;
    async function actualizarTransmisionesPeriodicamente() {
        if (ejecucionEnCurso) return; 
        ejecucionEnCurso = true;

        try {
            const idEmpresa = parseInt($('#cboEmpresa').val()) || 0;


            //await CargaLista(idEmpresa, 0);
        } catch (error) {
            console.error('Error al actualizar transmisiones:', error);
        } finally {
            ejecucionEnCurso = false;
        }
    }
    
    actualizarTransmisionesPeriodicamente();
    setInterval(actualizarTransmisionesPeriodicamente, 30 * 1000);

    $('#btnAgregar').click(function () {
        id = 0;
        const $modal = $('#modalDetalle');

        $('#cboEmpresaDetalle').prop('disabled', false);
        $modal.find('.card-title').text('Agregar');
        $('#cboEmpresaDetalle, #cboSucursalDetalle, #cboTipoDetalle').val('');
        $('#diasTransmision, #vigenciaDesde, #vigenciaHasta').val('');
        quitarTodos();

        $('#transmitirDesde, #transmitirHasta').val('');
        $('#tblSucusalesDetalle tbody').empty();

        $('#cboEmpresaDetalle, #cboSucursalDetalle, #cboTipoDetalle').formSelect();
        M.updateTextFields();
        const instance = M.Modal.getInstance($modal[0]);
        instance.open();
    });

    function formatearFecha(fecha) {
        let [yyyy, mm, dd] = fecha.split('-');
        return `${dd}-${mm}-${yyyy}`;
    }

    $(document).on('click', '.detalle-btn', async function () {
        ShowLoading();
        const $modal = $('#modalDetalle');
        const instance = M.Modal.getInstance($modal[0]);

        id = $(this).data('id');
        const idEmpresaFila = $(this).data('idempresa');
        const claveSucursal = $(this).data('clave');
        const tipoFila = $(this).data('tipo');
        const diasFila = $(this).data('dias');
        const desde = $(this).data('desde');
        const hasta = $(this).data('hasta');
        const transmitirDesde = formatearFecha($(this).data('transmitird'));
        const transmitirHasta = formatearFecha($(this).data('transmitirh'));
        const llaveUnica = $(this).data('llaveunica');
        const activoFila = $(this).data('activo');

        $modal.find('.card-title').text('Detalle');
        quitarTodos();

        $('#cboEmpresaDetalle').val(idEmpresaFila).formSelect();
        $('#cboEmpresaDetalle').prop('disabled', true);

        await CargaSucursales(idEmpresaFila, llaveUnica);

        $('#cboTipoDetalle').val(tipoFila).formSelect();
        $('#diasTransmision').val(diasFila);
        $('#vigenciaDesde').val(desde);
        $('#vigenciaHasta').val(hasta);

        $('#transmitirDesde').val(transmitirDesde);
        $('#transmitirHasta').val(transmitirHasta);
        $('#cboEmpresaDetalle').val(idEmpresaFila).prop('disabled', true).formSelect();
        M.updateTextFields();
        instance.open();
        HideLoading();
    });

    $('#cboEmpresaDetalle').change(async function () {
        const idEmpresaSeleccionada = $(this).val();
        if (!idEmpresaSeleccionada) return;

        idSucursalSeleccionadoInicial = [];
        ShowLoading();
        await CargaSucursales(idEmpresaSeleccionada, "");
        HideLoading();
    });

    ShowLoading();
    await CargaEmpresas();
    await CargaSucursales(0, '');
    await CargaLista(0, 0);
    HideLoading();
    function RetriveColumns() {
        return [
            { data: 'id', visible: false },
            { data: 'fechaCreacion' },
            { data: 'fechaRecibido' },
            { data: 'fechaProcesado' },
            { data: 'sucursal' },
            { data: 'desde' },
            { data: 'hasta' },
            { data: 'nombreUsuario' },
            {
                data: 'estatus',
                render: function (data) {
                    const est = String(data).trim().toUpperCase();

                    let color = "black"; 

                    switch (est) {
                        case "PENDIENTE":
                            color = "orange-text";
                            break;

                        case "RECIBIDO":
                            color = "blue-text";
                            break;

                        case "PROCESANDO":
                        case "PROCESANDO...":
                            color = "amber-text text-darken-3";
                            break;

                        case "PROCESADO":
                            color = "green-text";
                            break;

                        default:
                            color = "grey-text";
                            break;
                    }

                    return `<span class="${color}" style="font-weight: bold;">${est}</span>`;
                }
            },

            {
                data: 'activo2',
                render: data => {
                    const value = String(data).trim().toUpperCase(); 
                    const isActive = value === "SI";

                    return isActive
                        ? '<i class="material-icons green-text">check_circle</i>'
                        : '<i class="material-icons red-text">radio_button_checked</i>';
                }
            },
            {
                data: null,
                orderable: false,
                render: function (data, type, row) {
                    const tipo = $('#cboTipo').val(); 
                    if (tipo === '2') {
                        return '';
                    }

                    if (row.estatus === "PENDIENTE") {
                        return `
                <a href="#!" class="tooltipped eliminarIcon"
                   data-id="${row.id}"
                   data-clave-sucursal="${row.clave}"
                   data-position="top"
                   data-tooltip="Eliminar registro">
                   <i class="material-icons red-text">delete</i>
                </a>
            `;
                    }

                    if (row.estatus === "RECIBIDO") {
                        return `
                <a href="#!" class="tooltipped subirIcon" 
                   data-id="${row.id}"
                   data-clave-sucursal="${row.clave}"
                   data-position="top" 
                   data-tooltip="Subir a Base de Datos">
                   <i class="material-icons blue-text">cloud_upload</i>
                </a>

                <a href="#!" class="tooltipped eliminarIcon"
                   data-id="${row.id}"
                   data-clave-sucursal="${row.clave}"
                   data-position="top"
                   data-tooltip="Eliminar registro">
                   <i class="material-icons red-text">delete</i>
                </a>`;
                    }
                    if (row.estatus === "PROCESADO") {
                        return `<a href="#!" class="tooltipped eliminarIcon" data-id="${row.id}"
                   data-clave-sucursal="${row.clave}"
                   data-position="top"
                   data-tooltip="Eliminar registro">
                   <i class="material-icons red-text">delete</i>
                </a>`;
                    }
                    return "";
                }
            }

        ];
    }

    function actualizarBotonProcesarTodo() {

        const tipo = $('#cboTipo').val(); 
        if (tipo === '2') {
            $("#btnProcesarTodo").hide();
            return;
        }

        const tabla = $("#tblTransmisiones").DataTable();
        const filas = tabla.rows().data().toArray();

        const hayRecibidos = filas.some(
            x => String(x.estatus).toUpperCase() === "RECIBIDO"
        );

        if (hayRecibidos) {
            $("#btnProcesarTodo").show();
        } else {
            $("#btnProcesarTodo").hide();
        }
    }

    $("#tblTransmisiones").on('draw.dt', function () {
        actualizarBotonProcesarTodo();
    });


    $(document).on("click", ".subirIcon", function () {
        const id = $(this).data("id");
        const sucursal = $(this).data("claveSucursal");

        Swal.fire({
            title: "¿Procesar histórico?",
            text: `Se procesará el archivo histórico de la sucursal ${sucursal}`,
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Sí, procesar",
            cancelButtonText: "Cancelar",
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33"
        }).then(async (result) => {

            if (!result.isConfirmed) return;

            try {
                Swal.fire({
                    title: "Procesando...",
                    text: "Este proceso puede tardar varios minutos...",
                    allowOutsideClick: false,
                    allowEscapeKey: false,
                    showConfirmButton: false,
                    didOpen: () => Swal.showLoading()
                });

                const url = `${configuracionGlobal.sUrlServidorUtil}/api/Administracion/ProcesarHistorico?sucursal=${sucursal}&id=${id}`;
                const response = await fetch(url);

                Swal.close();

                if (!response.ok) {
                    Swal.fire("Error", "No se pudo procesar el histórico.", "error");
                    return;
                }
                await response.blob();

                Swal.fire({
                    icon: "success",
                    title: "Histórico procesado",
                    text: "La información fue procesada correctamente.",
                    timer: 3000,
                    showConfirmButton: false
                });

            } catch (err) {
                Swal.close();
                console.error(err);
                Swal.fire("Error", "Ocurrió un problema al procesar el histórico.", "error");
            }
        });
    });


    $("#btnProcesarTodo").on("click", async function () {

        const tabla = $("#tblTransmisiones").DataTable();
        const filas = tabla.rows().data().toArray().filter(x => x.estatus === "RECIBIDO");

        if (filas.length === 0) {
            Swal.fire("Sin registros", "No hay sucursales con estatus RECIBIDO.", "info");
            return;
        }

        Swal.fire({
            title: "¿Procesar todos?",
            text: `Se procesarán ${filas.length} sucursal(es) con estatus RECIBIDO`,
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Sí, procesar todo",
            cancelButtonText: "Cancelar",
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33"
        }).then(async (result) => {

            if (!result.isConfirmed) return;

            try {

                Swal.fire({
                    title: "Procesando...",
                    html: "Iniciando...",
                    allowOutsideClick: false,
                    allowEscapeKey: false,
                    didOpen: () => Swal.showLoading()
                });

                let contador = 1;
                for (const row of filas) {

                    const id = row.id;
                    const sucursal = row.clave;
                    Swal.update({
                        html: `
                        <b>Procesando sucursal:</b> ${sucursal}<br>
                        <b>${contador}</b> de <b>${filas.length}</b> sucursales...
                    `,
                        showConfirmButton: false
                    });

                    const url = `${configuracionGlobal.sUrlServidorUtil}/api/Administracion/ProcesarHistorico?sucursal=${sucursal}&id=${id}`;
                    const response = await fetch(url);

                    if (!response.ok) {
                        console.error(`Error procesando sucursal: ${sucursal}`);
                        contador++;
                        continue;
                    }

                    await response.blob(); 

                    contador++;
                }

                Swal.close();
                Swal.fire({
                    icon: "success",
                    title: "Proceso completado",
                    text: "Todas las sucursales con estatus RECIBIDO han sido procesadas.",
                    timer: 3500,
                    showConfirmButton: false,
                    allowOutsideClick: false,
                    allowEscapeKey: false
                });

                await CargaLista(0, 0);

            } catch (err) {
                Swal.close();
                console.error(err);
                Swal.fire("Error", "Ocurrió un problema al procesar los registros.", "error");
            }

        });

    });
   
    $(document).on('click', '.eliminarIcon', function (e) {
        e.preventDefault();

        const id = $(this).data("id");
        const sucursal = $(this).data("claveSucursal");

        Swal.fire({
            title: '¿Está seguro de continuar?',
            text: "Esta acción eliminará la carga histórica seleccionada.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                CancelarTransmision(sucursal, id);
            }
        });
    });

    async function CancelarTransmision(sucursal, id) {
        try {
            ShowLoading();
            const response = await $.ajax({
                method: 'GET',
                url: `${configuracionGlobal.sUrlServidorUtil}/api/Administracion/CancelarTransmision?sucursal=${sucursal}&id=${id}`,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
            });

            if (response.exito) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Transmisión eliminada correctamente.',
                    showConfirmButton: false,
                    timer: 2000
                });

                const idEmpresa = parseInt($('#cboEmpresa').val()) || 0;
                await CargaLista(idEmpresa);

            } else {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'error',
                    title: 'No se pudo eliminar la transmisión.',
                    showConfirmButton: false,
                    timer: 3000
                });
            }
        } catch (error) {
            console.error(error);
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: 'Ocurrió un error al cancelar.',
                showConfirmButton: false,
                timer: 3000
            });
        } finally {
            HideLoading();
        }
    }
    function parseDate(str) {
        if (!str) return null;

        const parts = str.split('-');
        if (parts.length !== 3) return null;

        const day = parseInt(parts[0], 10);
        const month = parseInt(parts[1], 10) - 1;
        const year = parseInt(parts[2], 10);

        if (isNaN(day) || isNaN(month) || isNaN(year)) return null;

        return new Date(year, month, day);
    }
    function convertirDDMMYYYYaYYYYMMDD(fecha) {
        const partes = fecha.split('-');
        const dia = partes[0].padStart(2, '0');
        const mes = partes[1].padStart(2, '0');
        const anio = partes[2];
        return `${anio}-${mes}-${dia}`;
    }

    function mergeSucursales(idSucursalSeleccionadoInicial, idSucursalSeleccionadoActual) {
        const inicialSet = new Set(idSucursalSeleccionadoInicial);
        const actualSet = new Set(idSucursalSeleccionadoActual);
        const todasClaves = new Set([...inicialSet, ...actualSet]);
        const resultado = Array.from(todasClaves).map(clave => {
            return {
                Clave: clave,
                Seleccionada: actualSet.has(clave) 
            };
        });

        return resultado;
    }


    $(document).on('click', '#btnGuardar', async function () {
        try {

            const idEmpresa = $('#cboEmpresaDetalle').val();
            const idTipoDetalle = $('#cboTipoDetalle').val(); // 0 = Normal, 1 = Históricos
            const transmitirDesdeStr = $('#transmitirDesde').val();
            const transmitirHastaStr = $('#transmitirHasta').val();

            const transmitirDesde = convertirDDMMYYYYaYYYYMMDD(transmitirDesdeStr);
            const transmitirHasta = convertirDDMMYYYYaYYYYMMDD(transmitirHastaStr);

            const activo = true;
            const dias = 0;
            const hora = $('#cboHora').val() || "00:00";

            const sucursalesSeleccionadas = ObtenerSucursalesSeleccionadas();

            if (idSucursalSeleccionadoInicial.length <= 0 && sucursalesSeleccionadas.length <= 0) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'warning',
                    title: '¡Debe seleccionar una o más sucursales!',
                    background: '#fff3cd',
                    color: '#856404',
                    timer: 3000,
                    timerProgressBar: true,
                    showConfirmButton: false
                });
                return;
            }

            const listaSucursales = mergeSucursales(
                idSucursalSeleccionadoInicial,
                idSucursalSeleccionado
            );

            if (!idEmpresa || idEmpresa === "0" ||
                !transmitirDesde || !transmitirHasta ||
                idTipoDetalle === null || idTipoDetalle === "") {

                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'warning',
                    title: '¡Por favor complete todos los campos requeridos!',
                    background: '#fff3cd',
                    color: '#856404',
                    timer: 3000,
                    timerProgressBar: true,
                    showConfirmButton: false
                });
                return;
            }

            const transmitirDesde2 = parseDate(transmitirDesdeStr);
            const transmitirHasta2 = parseDate(transmitirHastaStr);

            if (!transmitirDesde2 || !transmitirHasta2) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'warning',
                    title: 'Fechas inválidas.',
                    timer: 3000,
                    showConfirmButton: false
                });
                return;
            }

            if (transmitirDesde2 > transmitirHasta2) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'warning',
                    title: 'La fecha inicial no puede ser mayor a la final.',
                    background: '#fff3cd',
                    color: '#856404',
                    timer: 3000,
                    timerProgressBar: true,
                    showConfirmButton: false
                });
                return;
            }

            const diffTime = transmitirHasta2.getTime() - transmitirDesde2.getTime();
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

            if (idTipoDetalle === "1" && diffDays > 5) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'warning',
                    title: 'Para tipo Normal el rango no puede ser mayor a 5 días.',
                    background: '#fff3cd',
                    color: '#856404',
                    timer: 4500,
                    timerProgressBar: true,
                    showConfirmButton: false
                });
                return;
            }

            if (idTipoDetalle === "2" && diffDays <= 5) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'warning',
                    title: 'Para tipo Históricos el rango debe ser mayor a 5 días.',
                    background: '#fff3cd',
                    color: '#856404',
                    timer: 4500,
                    timerProgressBar: true,
                    showConfirmButton: false
                });
                return;
            }

            if (!hora) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'warning',
                    title: '¡Debe seleccionar una hora!',
                    background: '#fff3cd',
                    color: '#856404',
                    timer: 3000,
                    timerProgressBar: true,
                    showConfirmButton: false
                });
                return;
            }

            const [hh, mm] = hora.split(':').map(Number);
            const tempFecha = new Date();
            tempFecha.setHours(hh, mm + 180, 0, 0);

            const horaHasta = `${String(tempFecha.getHours()).padStart(2, '0')}:${String(tempFecha.getMinutes()).padStart(2, '0')}`;
            const data = {
                Id: id,
                IdEmpresa: idEmpresa,
                Clave: '',
                Dias: dias,
                Desde: '',
                Hasta: '',
                Activo: activo,
                TransmitirDesde: `${transmitirDesde} ${hora}:00`,
                TransmitirHasta: `${transmitirHasta} ${horaHasta}:00`,
                IdUsuario: idUsuario,
                ListaSucursales: listaSucursales,
                TipoCarga: idTipoDetalle
            };
            ShowLoading();
            const response = await $.ajax({
                method: 'POST',
                url: `${configuracionGlobal.sUrlServidorUtil}/api/Administracion/GuardarTransmisionHistorico`,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(data)
            });

            if (response.exito) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: '¡Transmisión histórica guardada correctamente!',
                    background: '#d4edda',
                    color: '#155724',
                    timer: 3000,
                    timerProgressBar: true,
                    showConfirmButton: false
                });

                const instance = M.Modal.getInstance($('#modalDetalle'));
                instance.close();

                await CargaLista(0, 0);
            } else {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'error',
                    title: 'No se pudo guardar la transmisión histórica.\n' + response.mensaje,
                    background: '#f8d7da',
                    color: '#721c24',
                    timer: 3000,
                    timerProgressBar: true,
                    showConfirmButton: false
                });
            }

        } catch (error) {
            console.error(error);
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: '¡Ocurrió un error inesperado!',
                background: '#f8d7da',
                color: '#721c24',
                timer: 3000,
                timerProgressBar: true,
                showConfirmButton: false
            });
        } finally {
            HideLoading();
        }
    });

    $(document).on('click', '#btnBuscar', async function () {
        const idEmpresa = parseInt($('#cboEmpresa').val()) || 0;
        const idSQLScript = parseInt($('#cboTipo').val()) || 0;

        try {
            ShowLoading();
            await CargaLista(idEmpresa, idSQLScript);

        } catch (error) {
            console.error(error);
            ShowAlert('Ocurrió un error al cargar los datos.', 'Error');
        } finally {
            HideLoading();
        }
    });

});
