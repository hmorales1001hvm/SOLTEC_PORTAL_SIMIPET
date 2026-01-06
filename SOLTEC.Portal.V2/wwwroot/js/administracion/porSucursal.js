$(document).ready(async function () {
    let idUsuario = $("#txtUsuario").val();
    configuracionGlobal.sUrlServidorUtil = $("#apiBaseUrl").val();
    const hoy = new Date();
    const fechaInicial = new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate() - 365);

    // Formatear como dd-mm-yyyy
    const fechaStr = `${('0' + fechaInicial.getDate()).slice(-2)}-${('0' + (fechaInicial.getMonth() + 1)).slice(-2)}-${fechaInicial.getFullYear()}`;
    $('#reporteDesde').val(fechaStr);

    // Inicializar modales de Materialize
    M.Modal.init(document.querySelectorAll('.modal'), {
        opacity: 0.6,
        inDuration: 1200,
        outDuration: 800,
        dismissible: false,
        startingTop: '4%',
        endingTop: '15%'
    });

    // Cargar sucursales en select
    let empresaSucursal = JSON.parse($("#txtEmpresaSucursal").val());
    const $select = $("#cboSucursal");
    $select.empty();
    //$select.append('<option value="0" selected>Tod@s</option>');
    empresaSucursal.forEach(item => {
        const texto = `${item.nombreSucursal} (${item.claveSimi})`;
        $select.append(`<option value="${item.claveSimi}">${texto}</option>`);
    });
    $('select').not('.dataTables_length select').formSelect();

    // Columnas DataTable
    function RetriveColumns() {
        return [
            { data: 'sucursal', title: 'Sucursal' },
            { data: 'tcksNetos', title: 'Tickets Netos' },
            { data: 'totalDescuentos', title: 'Total Descuentos' },
            { data: 'ventaBaseComision', title: 'Venta Base Comisión' },
            { data: 'ventaConPremio', title: 'Venta con Premio' },
            { data: 'ventaNeta', title: 'Venta Neta' },
            { data: 'promedioXNota', title: 'Promedio por Nota' },
        ];
    }

    // Cargar lista desde API
    async function CargaLista() {
        const desdeStr = $('#reporteDesde').val();
        const hastaStr = $('#reporteHasta').val();
        let sel = $('#cboSucursal').val();
        let opciones = $("#cboSucursal option").map((i, e) => e.value).get();
        let claveSimi = [];

        if (!sel || sel.length === 0 || sel.includes("0")) {
            claveSimi = opciones.filter(v => v !== "0");
        } else {
            claveSimi = sel;
        }

        if (!desdeStr || !hastaStr) {
            Swal.fire({
                icon: 'warning',
                title: 'Faltan datos',
                text: 'Debes seleccionar un rango de fechas antes de continuar.'
            });
            return;
        }

        const [diaD, mesD, anioD] = desdeStr.split('-');
        const [diaH, mesH, anioH] = hastaStr.split('-');

        const filtros = {
            Desde: `${anioD}-${mesD}-${diaD}`,
            Hasta: `${anioH}-${mesH}-${diaH}`,
            ClavesSIMI: claveSimi.length === 0 ? null : claveSimi
        };

        await $.ajax({
            url: `${configuracionGlobal.sUrlServidorUtil}/api/Reportes/GetPorSucursal`,
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(filtros),
            success: function (result) {
                if (!result || !result.datosLista) {
                    ShowAlert('No se recibieron datos válidos', "Warning");
                    HideLoading();
                    return;
                }

                if ($.fn.DataTable.isDataTable('#tblAcumulado')) {
                    $('#tblAcumulado').DataTable().destroy();
                }

                result.datosLista = result.datosLista.map(x => {
                    let match = empresaSucursal.find(es =>
                        String(es.claveSimi).trim() === String(x.claveSimi).trim()
                    );
                    return { ...x, sucursal: match ? match.nombreSucursal : "" };
                });

                // Crear DataTable
                const tabla = $('#tblAcumulado').DataTable({
                    dom: "<'row valign-wrapper'<'col s5 dt-buttons-container'B><'col s4'l'<'select-wrapper'>>" +
                        "<'col s3 right-align'f>>" +
                        "t" +
                        "<'row'<'col s12'p>>",
                    data: result.datosLista,
                    columns: RetriveColumns(),
                    destroy: true,
                    responsive: true,
                    colReorder: true,
                    lengthChange: true,
                    pageLength: 10,
                    lengthMenu: [
                        [5, 10, 25, 50, -1],
                        [5, 10, 25, 50, "Todos"]
                    ],
                    scrollY: 500,
                    scroller: true,
                    columnDefs: [
                        {
                            targets: [1, 2, 3],
                            className: 'right-align',
                            render: function (data, type) {
                                if (type === 'display' || type === 'filter') {
                                    const num = parseFloat(data);
                                    return isNaN(num) ? data : num.toLocaleString('es-MX', { minimumFractionDigits: 0, maximumFractionDigits: 0 });
                                }
                                return data;
                            }
                        },
                        {
                            targets: [4, 5, 6],
                            className: 'right-align',
                            render: function (data, type) {
                                if (type === 'display' || type === 'filter') {
                                    const num = parseFloat(data);
                                    if (isNaN(num)) return data;
                                    const formatted = num.toLocaleString('es-MX', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                                    return num < 0 ? `<span style="color:red;">${formatted}</span>` : formatted;
                                }
                                return data;
                            }
                        }
                    ],
                    footerCallback: function () {
                        const api = this.api();
                        const numVal = i => typeof i === 'string' ? parseFloat(i.replace(/[\$,]/g, '')) || 0
                            : typeof i === 'number' ? i : 0;
                        for (let col = 1; col <= 6; col++) {
                            let total = api.column(col, { page: 'current' }).data().reduce((a, b) => a + numVal(b), 0);
                            const sinDecimales = [1, 2, 3].includes(col);
                            const alignClass = sinDecimales ? 'center-align' : 'right-align';
                            $(api.column(col).footer())
                                .html(total.toLocaleString('es-MX', {
                                    minimumFractionDigits: sinDecimales ? 0 : 2,
                                    maximumFractionDigits: sinDecimales ? 0 : 2
                                }))
                                .removeClass('right-align center-align')
                                .addClass(alignClass)
                                .css('font-weight', 'bold');
                        }
                    },
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
                        // Inicializar selects de DataTables con Materialize
                        const selects = document.querySelectorAll('.dataTables_length select');
                        M.FormSelect.init(selects);
                    }
                });
            },
            error: function (xhr, status, error) {
                HideLoading();
                ShowAlert('Error cargando datos: ' + error, "Error");
            }
        });
    }

    // Evento del ícono Detalle
    $(document).on('click', '.detalle-btn', async function () {
        const fecha = $(this).data('fecha');
        const $modal = $('#modalDetalle');
        const instance = M.Modal.getInstance($modal[0]);
        $modal.find('.card-title').text(`Detalle ${fecha}`);
        $('#lblFechaDetalle').text(fecha);
        instance.open();
    });

    // Botón Buscar
    $(document).on('click', '#btnBuscar', async function () {
        try {
            ShowLoading();
            await CargaLista();
        } catch (error) {
            ShowAlert('Ocurrió un error al cargar los datos.', 'Error');
        } finally {
            HideLoading();
        }
    });

    // Carga inicial
    ShowLoading();
    await CargaLista();
    HideLoading();
});
