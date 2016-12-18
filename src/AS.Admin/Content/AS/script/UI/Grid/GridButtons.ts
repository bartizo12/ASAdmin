class GridButtons {
    public static ColumnVisibility(): DataTables.DataTableButton {
        return {
            extend: 'colvis',
            titleAttr: StringResources["DataTables_colvis"],
            text: "<i class='fa fa-list-alt'></i>",
            className: "btn-default",
            columns: ':not(.button-cell)'
        };
    }
    public static Reload(): DataTables.DataTableButton {
        return {
            extend: 'reload',
            titleAttr: StringResources["DataTables_reload"],
            text: "<i class='fa fa-refresh'></i>",
            className: "btn-info"
        };
    }
}