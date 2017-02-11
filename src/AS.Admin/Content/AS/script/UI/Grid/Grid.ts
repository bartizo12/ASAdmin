class Grid {
    /* Members/Fields */
    private _idColumnName: string;
    private _table: any;
    private _id: string = "asGrid";
    private _width: string = "100%";
    private _iDisplayLength: number = -1;
    private _searching: boolean = false;
    private _serverSideProcessing: boolean = false;
    private _processing: boolean = true;
    private _info: boolean = true;
    private _ordering: boolean = true;
    private _paging: boolean = true;
    private _stateSave: boolean = false;
    private _dom: string = "lfrBtip";
    private _order: (string | number)[];
    private _columns: GridColumn[] = [];
    private _gridButtons: DataTables.DataTableButton[] = [];
    private _urlGenerator: () => string;
    private _drawCallBack: () => void;
    private _hasDeleteButton: boolean = false;
    private _cellSelectable: boolean = true;
    private _responsive: boolean = false;

    /* Getters/Setters */
    set Responsive(value: boolean) {
        this._responsive = value;
    }
    set HasDeleteButton(value: boolean) {
        this._hasDeleteButton = value;
    }
    set id(value: string) {
        this._id = value;
    }
    set width(value: string) {
        this._width = value;
    }
    set iDisplayLength(value: number) {
        this._iDisplayLength = value;
    }
    set Searching(value: boolean) {
        this._searching = value;
    }
    set ServerSideProcessing(value: boolean) {
        this._serverSideProcessing = value;
    }
    set Processing(value: boolean) {
        this._processing = value;
    }
    set Info(value: boolean) {
        this._info = value;
    }
    set Ordering(value: boolean) {
        this._ordering = value;
    }
    set Paging(value: boolean) {
        this._paging = value;
    }
    set Columns(value: GridColumn[]) {
        this._columns = value;
    }
    set StateSave(value: boolean) {
        this._stateSave = value;
    }
    set Order(value: (string | number)[]) {
        this._order = value;
    }
    set UrlGenerator(value: () => string) {
        this._urlGenerator = value;
    }
    set DrawCallBack(value: () => void) {
        this._drawCallBack = value;
    }
    get Columns(): GridColumn[] {
        return this._columns;
    }
    set GridButtons(value: DataTables.DataTableButton[]) {
        this._gridButtons = value;
    }
    get GridButtons(): DataTables.DataTableButton[] {
        return this._gridButtons;
    }
    /* Constructor */
    constructor()
    constructor(idColumnName: string)
    constructor(idColumnName?: string) {
        this._idColumnName = idColumnName;

        this._gridButtons = [];
        this._gridButtons.push(GridButtons.ColumnVisibility());
        this._gridButtons.push(GridButtons.Reload());
    }
    /* Functions */
    render() {
        if (this._hasDeleteButton) {
            let col: GridColumn = new GridColumn("", this._idColumnName, CellRenderers.DeleteButtonRenderer);
            col.Orderable = false;
            col.Width = "80px";
            col.ClassName = "button-cell";
            this._columns.push(col);
        }

        var cssClasses = "table table-striped table-bordered dataTable";

        var tableHtml = "<table id='" + this._id + "' class='" + cssClasses + "' data-idColumnName='" + this._idColumnName + "' style='width:100%' >";
        tableHtml += "<thead>";
        tableHtml += "<tr>";
        for (var i = 0; i < this._columns.length; i++) {
            tableHtml += "<th";

            if (this._columns[i].Width !== undefined) {
                tableHtml += " style='width:" + this._columns[i].Width + "'";
            }
            tableHtml += ">" + this._columns[i].Title + "</th>";
        }
        tableHtml += "</tr>";
        tableHtml += "</thead>";
        tableHtml += "</table>";
        $("#" + this._id + "-wrapper").empty();
        $("#" + this._id + "-wrapper").append(tableHtml);

        var dtCols: DataTables.ColumnSettings[] = [];

        if (this._columns != null && this._columns !== undefined) {
            for (let i = 0; i < this._columns.length; i++) {
                dtCols.push(this._columns[i].ToDtColSetting());
            }
        }
        this._table = $("#" + this._id).dataTable({
            searching: this._searching,
            stateSave: this._stateSave,
            serverSide: this._serverSideProcessing,
            processing: this._processing,
            buttons: this._gridButtons,
            order: this._order,
            info: this._info,
            dom: this._dom,
            responsive: this._responsive,
            ordering: this._ordering,
            paging: this._paging,
            ajax: this._urlGenerator(),
            columns: dtCols,
            language: {
                emptyTable: StringResources["DataTable_EmptyTable"],
                info: StringResources["DataTable_Info"],
                infoEmpty: StringResources["DataTable_InfoEmpty"],
                infoFiltered: StringResources["DataTable_InfoFiltered"],
                lengthMenu: StringResources["DataTable_LengthMenu"],
                loadingRecords: StringResources["DataTable_LoadingRecords"],
                processing: StringResources["DataTable_Processing"],
                zeroRecords: StringResources["DataTable_ZeroRecords"],
                paginate: {
                    next: StringResources["DataTable_Next"],
                    previous: StringResources["DataTable_Previous"],
                    first: StringResources["DataTable_First"],
                    last: StringResources["DataTable_Last"]
                },
                aria: {
                    sortAscending: StringResources["DataTable_SortAscending"],
                    sortDescending: StringResources["DataTable_SortDescending"]
                }
            },
            createdRow: function (row, data, index) {
                $("td.html-formatted-cell", row).each(function () {
                    var str = $(this).html();
                    $(this).text(str);
                });
                $("td.truncate-cell", row).each(function (index, elem) {
                    var content = $(elem).html().replace(/&/g, '&amp;')
                        .replace(/"/g, '&quot;')
                        .replace(/'/g, '&#39;')
                        .replace(/</g, '&lt;')
                        .replace(/>/g, '&gt;');
                    var newContent = '<div data-toggle="tooltip" title="' + content + '" class="truncate-cell-container"><div class="truncate-cell-content">' + content + '</div>';
                    newContent += '<div class="truncate-cell-spacer">' + content + '</div>';
                    newContent += '<span>&nbsp;</span></div>';

                    $(elem).html(newContent);
                });
            },
            drawCallback: function (settings) {
                $(".btn-rowDelete").on('click', function () {
                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_PRIMARY,
                        data: { 'id': $(this).attr('id'), 'dtInstance': settings.oInstance },
                        title: StringResources["ConfirmTitle"],
                        message: StringResources["DataTables_DeleteConfirmMessage"],
                        buttons: [
                            {
                                label: StringResources["Yes"],
                                action: function (dialog: IBootstrapDialogContext) {
                                    dialog.close();
                                    var ix = window.location.pathname.lastIndexOf('/');

                                    $.ajax({
                                        context: dialog.getData('dtInstance'),
                                        url: window.location.pathname.substring(0, ix + 1) + "Delete?id=" + dialog.getData('id'),
                                        type: 'DELETE',
                                        success: function (msg) {
                                            this.api().ajax.reload();
                                        }
                                    });
                                }
                            },
                            {
                                label: StringResources["No"],
                                action: function (dialog: IBootstrapDialogContext) {
                                    dialog.close();
                                }
                            }]
                    });
                });
                InitDialog();
                $('[data-toggle="tooltip"]').tooltip();
            }
        });
    }
    reload() {
        $(".dataTables_length select").trigger('change');
        this._table.api().ajax.url(this._urlGenerator()).load();
    }
}