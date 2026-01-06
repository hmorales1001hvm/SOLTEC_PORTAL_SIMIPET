$(document).ready(async function () {
    let idUsuario = $("#txtUsuario").val();
    configuracionGlobal.sUrlServidorUtil = $("#apiBaseUrl").val();
    let empresaSucursal = JSON.parse($("#txtEmpresaSucursal").val());

    const $select = $("#cboSucursal");
    $select.empty(); // Limpia el contenido anterior

    //$select.append('<option value="0" selected>Tod@s</option>');
    empresaSucursal.forEach(item => {
        const texto = `${item.nombreSucursal} (${item.claveSimi})`;
        $select.append(`<option value="${item.claveSimi}">${texto}</option>`);
    });
    $('select').formSelect();


    function RetriveColumns() {
        return [
            { data: 'sucursal', title: 'Clave' },       // 0
            { data: 'nombre', title: 'Nombre Sucursal' },// 1
            { data: 'ventas', title: 'Ventas' },        // 2
            { data: 'existencias', title: 'Existencias' }// 4
        ];
    }

    async function CargaLista() {
        const desdeStr = $('#reporteDesde').val();
        let sel = $('#cboSucursal').val(); // array o null
        let opciones = $("#cboSucursal option").map((i, e) => e.value).get();
        let claveSimi = [];

        if (!desdeStr) {
            Swal.fire({
                icon: 'warning',
                title: 'Faltan datos',
                text: 'Debes seleccionar una fecha a consultar.'
            });
            return;
        }

        const [diaD, mesD, anioD] = desdeStr.split('-');

        if (!sel || sel.length === 0 || sel.includes("0")) {
            claveSimi = opciones.filter(v => v !== "0");
        } else {
            claveSimi = sel;
        }

        const filtros = {
            ClavesSIMI: claveSimi === "0" ? null : claveSimi,
            Desde: `${anioD}-${mesD}-${diaD}`
        };

        await $.ajax({
            url: `${configuracionGlobal.sUrlServidorUtil}/api/Reportes/GetInventarioValuacion`,
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(filtros),
            success: function (result) {
                if (!result) {
                    ShowAlert('No se recibieron datos válidos', "Warning");
                    HideLoading();
                    return;
                }

                if ($.fn.DataTable.isDataTable('#tblAcumulado')) {
                    $('#tblAcumulado').DataTable().destroy();
                }

                result.datosLista = result.datosLista.map(x => {
                    let match = empresaSucursal.find(es =>
                        String(es.claveSimi).trim() === String(x.sucursal).trim()
                    );
                    return { ...x, nombre: match ? match.nombreSucursal : "" };
                });

                $('#tblAcumulado').DataTable({
                    dom: "<'row valign-wrapper'<'col s5 dt-buttons-container'B><'col s4'l'<'select-wrapper'>>" +
                        "<'col s3 right-align'f>>" +
                        "t" +
                        "<'row'<'col s12'p>>",
                    data: result.datosLista,
                    columns: RetriveColumns(),
                    destroy: true,
                    colReorder: true,
                    responsive: true,
                    lengthChange: true,
                    pageLength: FormDataTable.pageLength,
                    lengthMenu: [
                        [5, 10, 25, 50, -1],
                        [5, 10, 25, 50, "Todos"]
                    ],
                    scrollY: 500,
                    scroller: true,
                    columnDefs: [
                        {
                            targets: [0, 1],
                            className: 'left-align'
                        },
                        {
                            targets: [2, 3],
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
                        const numVal = i => typeof i === 'string' ? parseFloat(i.replace(/[\$,]/g, '')) || 0 : i || 0;

                        [2, 3].forEach(col => {
                            let total = api.column(col, { page: 'current' }).data().reduce((a, b) => a + numVal(b), 0);
                            $(api.column(col).footer())
                                .html(total.toLocaleString('es-MX', { minimumFractionDigits: 2, maximumFractionDigits: 2 }))
                                .css('font-weight', 'bold')
                                .addClass('right-align');
                        });

                        $(api.column(1).footer())
                            .html('TOTALES:')
                            .css('font-weight', 'bold')
                            .removeClass('right-align')
                            .addClass('left-align');

                        $(api.column(0).footer()).html('');
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

                        // Alinear texto "Mostrar" con el combo
                        $('.dataTables_length').css({
                            display: 'flex',
                            alignItems: 'center',
                            gap: '0.5rem'
                        });
                    }
                });
            },
            error: function (xhr, status, error) {
                HideLoading();
                ShowAlert('Error cargando datos: ' + error, "Error");
            }
        });
    }


    $(document).on('click', '.detalle-btn', async function () {
        const fecha = $(this).data('fecha');
        const $modal = $('#modalDetalle');
        const instance = M.Modal.getInstance($modal[0]);
        $modal.find('.card-title').text(`Detalle ${fecha}`);
        $('#lblFechaDetalle').text(fecha);
        instance.open();
    });

    $(document).on('click', '#btnBuscar', async function () {
        try { ShowLoading(); await CargaLista(); }
        catch { ShowAlert('Ocurrió un error al cargar los datos.', 'Error'); }
        finally { HideLoading(); }
    });

    ShowLoading();
    await CargaLista();
    HideLoading();
});
