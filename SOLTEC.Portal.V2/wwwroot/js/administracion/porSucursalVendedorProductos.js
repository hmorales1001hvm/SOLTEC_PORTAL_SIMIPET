$(document).ready(async function () {
    let idUsuario = $("#txtUsuario").val();
    configuracionGlobal.sUrlServidorUtil = $("#apiBaseUrl").val();

    const hoy = new Date();
    const fechaInicial = new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate() - 10);

    const fechaStr = `${('0' + fechaInicial.getDate()).slice(-2)}-${('0' + (fechaInicial.getMonth() + 1)).slice(-2)}-${fechaInicial.getFullYear()}`;
    $('#reporteDesde').val(fechaStr);

    const modales = document.querySelectorAll('.modal');
    M.Modal.init(modales);

    let empresaSucursal = JSON.parse($("#txtEmpresaSucursal").val());

    const $select = $("#cboSucursal");
    $select.empty(); // Limpia el contenido anterior

    //$select.append('<option value="0" selected>Tod@s</option>');
    empresaSucursal.forEach(item => {
        const texto = `${item.nombreSucursal} (${item.claveSimi})`;
        $select.append(`<option value="${item.claveSimi}">${texto}</option>`);
    });
    $('select').formSelect();

    // === Columnas ===
    function RetriveColumns() {
        return [
            {
                data: 'sucursal',
                title: 'Sucursal',
                //visible: false,
                //render: function (data, type, row, meta) {
                //    return type === 'display' ? '' : data;
                //}
            },
            { data: 'vendedor', title: 'Vendedor' },
            { data: 'producto', title: 'Producto' },
            { data: 'piezas', title: 'Piezas' },
            { data: 'descuento', title: 'Descuento' },
            { data: 'iva', title: 'IVA' },
            { data: 'ventaNeta', title: 'Venta Neta' }
        ];
    }

    // === Cargar datos desde API ===
    async function CargaLista() {
        const desdeStr = $('#reporteDesde').val();
        const hastaStr = $('#reporteHasta').val();
        let sel = $('#cboSucursal').val(); // array o null
        let opciones = $("#cboSucursal option").map((i, e) => e.value).get();
        let claveSimi = [];

        if (!sel || sel.length === 0 || sel.includes("0")) {
            claveSimi = opciones.filter(v => v !== "0");
        } else {
            claveSimi = sel;
        }

        const [diaD, mesD, anioD] = desdeStr.split('-');
        const [diaH, mesH, anioH] = hastaStr.split('-');

        const filtros = {
            Desde: `${anioD}-${mesD}-${diaD}`,
            Hasta: `${anioH}-${mesH}-${diaH}`,
            ClavesSIMI: claveSimi === "0" ? null : claveSimi
        };

        await $.ajax({
            url: `${configuracionGlobal.sUrlServidorUtil}/api/Reportes/GetPorSucursalVendedorProducto`,
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
                    buttons: [
                        {
                            extend: 'copy',
                            text: '<i class="material-icons left">content_copy</i> Copiar',
                            titleAttr: 'Copiar al portapapeles',
                            className: 'btn waves-effect waves-light teal lighten-1',
                            exportOptions: {
                                columns: [0, 1, 2, 3, 4, 5, 6],
                                modifier: { page: 'all' }
                            }
                        },
                        {
                            extend: 'excel',
                            text: '<i class="material-icons left">grid_on</i> Excel',
                            titleAttr: 'Exportar a Excel',
                            className: 'btn waves-effect waves-light green darken-2',
                            exportOptions: {
                                columns: [0, 1, 2, 3, 4, 5, 6],
                                modifier: { page: 'all' }
                            }
                        },
                        {
                            extend: 'csv',
                            text: '<i class="material-icons left">description</i> CSV',
                            titleAttr: 'Exportar a CSV',
                            className: 'btn waves-effect waves-light blue darken-1',
                            exportOptions: {
                                columns: [0, 1, 2, 3, 4, 5, 6],
                                modifier: { page: 'all' }
                            }
                        },
                        {
                            extend: 'pdf',
                            text: '<i class="material-icons left">picture_as_pdf</i> PDF',
                            titleAttr: 'Exportar a PDF',
                            className: 'btn waves-effect waves-light red darken-2',
                            exportOptions: {
                                columns: [0, 1, 2, 3, 4, 5, 6],
                                modifier: { page: 'all' }
                            }
                        },
                        {
                            extend: 'print',
                            text: '<i class="material-icons left">print</i> Imprimir',
                            titleAttr: 'Imprimir tabla',
                            className: 'btn waves-effect waves-light blue',
                            exportOptions: {
                                columns: [0, 1, 2, 3, 4, 5, 6],
                                modifier: { page: 'all' }
                            }
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
                    columnDefs: [
                        {
                            targets: [3, 4, 5, 6],
                            className: 'right-align',
                            render: function (data, type, row, meta) {
                                if (type === 'display' || type === 'filter') {
                                    const num = parseFloat(data);
                                    if (isNaN(num)) return data;
                                    if ([2].includes(meta.col)) return num.toLocaleString('es-MX', { minimumFractionDigits: 0, maximumFractionDigits: 0 });
                                    return num.toLocaleString('es-MX', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                                }
                                return data;
                            }
                        }
                    ],
                    autoWidth: false,
                    select: true,
                    rowGroup: {
                        dataSrc: 'sucursal',
                        startRender: function (rows, group) {
                            const suma = campo => rows.data().pluck(campo).reduce((a, b) => a + parseFloat(b || 0), 0);
                            return $('<tr/>')
                                .addClass('grupo-sucursal')
                                .append(`<td colspan="2" style="font-weight:bold; background-color:#f5f5f5;">${group}</td>`)
                                .append(`<td style="background-color:#f5f5f5;"></td>`)
                                .append(`<td style="text-align:right; font-weight:bold; background-color:#f5f5f5;">${suma('piezas').toLocaleString('es-MX', { minimumFractionDigits: 0 })}</td>`)
                                .append(`<td style="text-align:right; font-weight:bold; background-color:#f5f5f5;">${suma('descuento').toLocaleString('es-MX', { minimumFractionDigits: 2 })}</td>`)
                                .append(`<td style="text-align:right; font-weight:bold; background-color:#f5f5f5;">${suma('iva').toLocaleString('es-MX', { minimumFractionDigits: 2 })}</td>`)
                                .append(`<td style="text-align:right; font-weight:bold; background-color:#f5f5f5;">${suma('ventaNeta').toLocaleString('es-MX', { minimumFractionDigits: 2 })}</td>`)
                                .append(`<td style="background-color:#f5f5f5;"></td>`);
                        }
                    },
                    footerCallback: function (row, data, start, end, display) {
                        const api = this.api();
                        const numVal = i => typeof i === 'string' ? parseFloat(i.replace(/[\$,]/g, '')) || 0 : typeof i === 'number' ? i : 0;

                        for (let col = 3; col <= 6; col++) {
                            let total = api.column(col, { page: 'current' }).data().reduce((a, b) => a + numVal(b), 0);
                            const sinDecimales = [2].includes(col);
                            const alignClass = sinDecimales ? 'center-align' : 'right-align';
                            $(api.column(col).footer())
                                .html(total.toLocaleString('es-MX', { minimumFractionDigits: sinDecimales ? 0 : 2, maximumFractionDigits: sinDecimales ? 0 : 2 }))
                                .removeClass('right-align center-align')
                                .addClass(alignClass)
                                .css('font-weight', 'bold');
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


    // === Botón Buscar ===
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

    // === Carga inicial ===
    ShowLoading();
    await CargaLista();
    HideLoading();
});
