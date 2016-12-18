class RowButtons {
    public static UpdateButton(gridId, columnName): GridColumn {
        function renderUpdateButton(data, t, row, meta) {
            var html = "<a  data-related-gridid='" + gridId + "' data-iframeWidth='800' data-iframeHeight='350' class='btn btn-primary btn-rowButton dialog-button' href='Edit?id=" + data + "'>";
            html += "<i class='fa fa-pencil-square-o fw'></i>" + StringResources["Update"] + "</a>";

            return html;
        }
        var col = new GridColumn("", columnName, renderUpdateButton);
        col.Orderable = false;
        col.Width = "50px";
        col.ClassName = "button-cell";

        return col;
    }
}