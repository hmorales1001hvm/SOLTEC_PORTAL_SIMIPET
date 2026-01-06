$(document).ready(async function () {
    let id = 0;
    let idUsuario = $("#txtUsuario").val();
    configuracionGlobal.sUrlServidorUtil = $("#apiBaseUrl").val();

    const modales = document.querySelectorAll('.modal');
    M.Modal.init(modales, {
        opacity: 0.6,
        inDuration: 1200,
        outDuration: 800,
        dismissible: false, // NO se cierra al clic fuera
        startingTop: '4%',
        endingTop: '15%'
    });
    $('select').formSelect();

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

    // ============================
    // Cargar Tipos de Transmisión
    // ============================
    async function CargaTipos() {
        try {

            const urlAPI = `${configuracionGlobal.sUrlServidorUtil}/api/Administracion/TiposTransmisiones`;
            const result = await $.ajax({
                method: 'POST',
                url: urlAPI,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ IdSQLScript: 0 })
            });

            if (!result.exito) return ShowAlert(result.Message, "Warning");

            if (result.datosLista.length > 0) {
                LoadSelect("cboTipo", result.datosLista, "tipoTransmision", "idSQLScript", 0);
                LoadSelect("cboTipoDetalle", result.datosLista, "tipoTransmision", "idSQLScript", 0);
                M.updateTextFields();
            } else {
                ShowAlert('No se encontraron tipos de transmisión.', "Warning");
            }
            return result;
        } catch (error) {
            ShowAlert('Servicio de tipos no disponible.', "Error");
            console.error(error);
        } finally {

        }
    }


    let idSucursalSeleccionado = []; // array global para todas las páginas
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

            // Inicializar idSucursalSeleccionado con las pre-seleccionadas
            idSucursalSeleccionado = result.datosLista
                .filter(x => x.seleccionada === 1 || x.seleccionada === true)
                .map(x => x.clave);

            idSucursalSeleccionadoInicial = [...idSucursalSeleccionado];

            // Destruir tabla anterior si existe
            if ($.fn.DataTable.isDataTable('#tblSucusalesDetalle')) {
                $('#tblSucusalesDetalle').DataTable().destroy();
                $('#tblSucusalesDetalle tbody').empty();
            }

            // Inicializar DataTable
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
                pageLength:12,
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
                        text: '<i class="material-icons left">print</i> Imprimir',
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

            // ==============================
            // Checkbox "Seleccionar todo" que afecta TODAS las páginas
            // ==============================
            $('#chkSeleccionarTodo').off('change').on('change', function () {
                const isChecked = $(this).is(':checked');

                // Iterar sobre todas las filas de la tabla (todas las páginas)
                table.rows().every(function () {
                    const data = this.data();
                    const clave = data.clave;

                    // Actualizar array global
                    if (isChecked) {
                        if (!idSucursalSeleccionado.includes(clave)) idSucursalSeleccionado.push(clave);
                    } else {
                        idSucursalSeleccionado = idSucursalSeleccionado.filter(x => x !== clave);
                    }

                    // Actualizar checkbox de cada fila en el DOM
                    const $node = $(this.node());
                    $node.find('input.chk-clave').prop('checked', isChecked);
                });
            });

            // ==============================
            // Checkbox de cada fila
            // ==============================
            $('#tblSucusalesDetalle tbody').off('change', 'input.chk-clave').on('change', 'input.chk-clave', function () {
                const clave = $(this).val();
                if ($(this).is(':checked')) {
                    if (!idSucursalSeleccionado.includes(clave)) idSucursalSeleccionado.push(clave);
                } else {
                    idSucursalSeleccionado = idSucursalSeleccionado.filter(x => x !== clave);
                }

                // Actualizar checkbox del header según todas las filas
                const total = table.rows().count();
                const checked = idSucursalSeleccionado.length;
                $('#chkSeleccionarTodo').prop('checked', total === checked);
            });

            // Inicializar estado del checkbox "Seleccionar todo"
            const totalInicial = table.rows().count();
            const checkedInicial = idSucursalSeleccionado.length;
            $('#chkSeleccionarTodo').prop('checked', totalInicial === checkedInicial);

        } catch (error) {
            ShowAlert('Servicio de sucursales no disponible.', "Error");
            console.error(error);
        }
    }




    // Para obtener todas las sucursales seleccionadas (aunque estén en otras páginas)
    function ObtenerSucursalesSeleccionadas() {
        return idSucursalSeleccionado;
    }




    // ============================
    // Cargar lista de transmisiones
    // ============================
    async function CargaLista(idEmpresa, idSQLScript) {
        try {
            const urlAPI = `${configuracionGlobal.sUrlServidorUtil}/api/Administracion/Transmisiones`;
            const result = await $.ajax({
                method: 'POST',
                url: urlAPI,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ IdEmpresa: idEmpresa, IdSQLScript: idSQLScript, IdUsuario: idUsuario })
            });

            if (!result.datosLista || result.datosLista.length === 0) {
                ShowAlert('No se encontraron transmisiones para este usuario.', "Warning");
            }

            $('#tblTransmisiones').DataTable({
                headers: FormDataTable,
                data: result.datosLista,
                destroy: true,
                colReorder: true,
                responsive: true,
                columns: RetriveColumns(),
                columnDefs: [
                    { className: 'center-align', targets: [4, 5, 6, 7] }
                ],
                autoWidth: false,
                select: true,
                pageLength: FormDataTable.pageLength,
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
                    "<'row'<'col s12'p>>",

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
                        text: '<i class="material-icons left">print</i> Imprimir',
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
                    }
                },

                initComplete: function () {
                    // Inicializar selects con Materialize
                    const selects = document.querySelectorAll('.dataTables_length select');
                    M.FormSelect.init(selects);

                    // Alinear texto "Mostrar" con el combo
                    $('.dataTables_length').css({
                        display: 'flex',
                        alignItems: 'center',
                        gap: '0.5rem'
                    });
                }
            });

        } catch (error) {
            ShowAlert('Servicio de transmisiones no disponible.', "Error");
            console.error(error);
        }
    }


    // Agregar
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

    //Detalle
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

        $('#chkActivo').prop('checked', activoFila);
        $('#cboEmpresaDetalle').val(idEmpresaFila).prop('disabled', true).formSelect();

        M.updateTextFields();
        instance.open();
        HideLoading();
    });

    // Cambio de empresa en detalle (cargar sucursales)
    $('#cboEmpresaDetalle').change(async function () {
        const idEmpresaSeleccionada = $(this).val();
        if (!idEmpresaSeleccionada) return;

        idSucursalSeleccionadoInicial = [];
        ShowLoading();
        await CargaSucursales(idEmpresaSeleccionada, "");
        HideLoading();
    });

    //// Botón Buscar
    //$(document).on('click', '#btnBuscar', async function () {
    //    const idEmpresa = parseInt($('#cboEmpresa').val()) || 0;
    //    const idSQLScript = parseInt($('#cboTipo').val()) || 0;
    //    await CargaLista(idEmpresa, idSQLScript);
    //});

    // Inicializar carga de datos
    ShowLoading();
    await CargaEmpresas();
    await CargaSucursales(0, '');
    await CargaTipos();
    await CargaLista(0, 0);
    HideLoading();

    // ============================
    // Columnas para DataTable
    // ============================
    function RetriveColumns() {
        return [
            { data: 'empresa' },
            { data: 'llaveUnica' },
            { data: 'tipo' },
            { data: 'nombreUsuario' },
            { data: 'periodoTransmision' },
            { data: 'vigencia' },
            {
                data: 'activo',
                render: data => data
                    ? '<i class="material-icons green-text">check_circle</i>'
                    : '<i class="material-icons red-text">cancel</i>'
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
                        <a href="#!" class="modal-trigger detalle-btn" 
                           data-idempresa="${row.idEmpresa}" 
                           data-clave="${row.clave}" 
                           data-tipo="${row.idSQLScript}" 
                           data-dias="${row.dias}" 
                           data-vigencia="${row.vigencia}"
                           data-desde="${row.desde}"
                           data-hasta="${row.hasta}"
                           data-transmitird="${row.transmitirDesde}"
                           data-transmitirh="${row.transmitirHasta}"
                           data-activo="${row.activo}"
                           data-llaveunica="${row.llaveUnica}"
                           data-id="${row.id}">
                           <i class="material-icons blue-text">visibility</i>
                        </a>
                    `;
                }
            }
        ];
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
        // Crear un Set de iniciales y de actuales para búsquedas rápidas
        const inicialSet = new Set(idSucursalSeleccionadoInicial);
        const actualSet = new Set(idSucursalSeleccionadoActual);

        // Crear un Set con todas las claves posibles (union)
        const todasClaves = new Set([...inicialSet, ...actualSet]);

        // Generar el array final
        const resultado = Array.from(todasClaves).map(clave => {
            return {
                Clave: clave,
                Seleccionada: actualSet.has(clave) // true si está en la lista actual, false si fue desmarcada
            };
        });

        return resultado;
    }


    $(document).on('click', '#btnGuardar', async function () {
        try {

            const idEmpresa = $('#cboEmpresaDetalle').val();
            const idSQLScript = $('#cboTipoDetalle').val();
            const transmitirDesde = convertirDDMMYYYYaYYYYMMDD($('#transmitirDesde').val());
            const transmitirHasta = convertirDDMMYYYYaYYYYMMDD($('#transmitirHasta').val());
            const activo = $('#chkActivo').is(':checked');
            const dias = 0;

            const sucursalesSeleccionadas = ObtenerSucursalesSeleccionadas();
            if (idSucursalSeleccionadoInicial.length <= 0) {
                if (sucursalesSeleccionadas.length <= 0) {
                    Swal.fire({
                        toast: true,
                        position: 'top-end',
                        icon: 'warning',
                        title: '¡Debe seleccionar una o mas sucursales!',
                        background: '#fff3cd',
                        color: '#856404',
                        timer: 3000,
                        timerProgressBar: true,
                        showConfirmButton: false
                    });
                    return;
                }
            }
            const listaSucursales = mergeSucursales(idSucursalSeleccionadoInicial, idSucursalSeleccionado);

            if (!idEmpresa || idEmpresa === "0" || !idSQLScript || idSQLScript === "0" || !transmitirDesde || !transmitirHasta) {

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

            const transmitirDesde2 = parseDate($('#transmitirDesde').val());
            const transmitirHasta2 = parseDate($('#transmitirHasta').val());
            const hoy2 = new Date();
            hoy2.setHours(0, 0, 0, 0); // eliminar hora para comparar solo fecha

            // Validar que inicio <= fin
            if (transmitirDesde2 > transmitirHasta2) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'warning',
                    title: 'La fecha inicial de Transmisión no puede ser mayor a la fecha final.',
                    background: '#fff3cd',
                    color: '#856404',
                    timer: 3000,
                    timerProgressBar: true,
                    showConfirmButton: false
                });
                return;
            }

            const data = {
                Id: id,
                IdEmpresa: idEmpresa,
                Clave: '',
                IdSQLScript: idSQLScript,
                Dias: dias,
                Desde: '',
                Hasta: '',
                Activo: activo,
                TransmitirDesde: transmitirDesde,
                TransmitirHasta: transmitirHasta,
                IdUsuario: idUsuario,
                ListaSucursales: listaSucursales
            };

            ShowLoading();
            const response = await $.ajax({
                method: 'POST',
                url: `${configuracionGlobal.sUrlServidorUtil}/api/Administracion/GuardarTransmision`,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(data)
            });

            if (response.exito) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: '¡Transmisión guardada correctamente!',
                    background: '#d4edda',
                    color: '#155724',
                    timer: 3000,
                    timerProgressBar: true,
                    showConfirmButton: false
                });
                const instance = M.Modal.getInstance($('#modalDetalle'));
                instance.close();
                await CargaLista(0, 0);
                HideLoading();
            } else {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'error',
                    title: '¡No se pudo guardar la transmisión!\n' + response.mensaje,
                    background: '#f8d7da',
                    color: '#721c24',
                    timer: 3000,
                    timerProgressBar: true,
                    showConfirmButton: false
                });
                HideLoading();
            }

        } catch (error) {
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
