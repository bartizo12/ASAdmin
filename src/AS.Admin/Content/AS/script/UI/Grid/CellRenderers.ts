module CellRenderers {
    export var DateTimeRenderer: DataTables.FunctionColumnRender = function (data: any, t: string, row: any, meta: DataTables.CellMetaSettings): string {
        if (data != null) {
            return new Date(data).toLocaleString(navigator.languages[0]);
        }
        else {
            return "";
        }
    }
    export var DeleteButtonRenderer: DataTables.FunctionColumnRender = function (data: any, t: string, row: any, meta: DataTables.CellMetaSettings): string {
        var html: string = "<a class='btn btn-danger btn-rowButton btn-rowDelete' href='#' id='" + data + "'>";
        html += "<i class='fa fa-trash-o fw'></i>" + StringResources["Delete"] + "</a>";

        return html;
    }
    export var EditButtonRenderer: DataTables.FunctionColumnRender = function (data: any, t: string, row: any, meta: DataTables.CellMetaSettings): string {
        var html: string = "<a class='btn btn-primary btn-rowButton dialog-button' href='Edit?id='" + data + "'>";
        html += "<i class='fa fa-pencil-square-o fw'></i>" + StringResources["Edit"] + "</a>";

        return html;
    }
    export var JobStatusRenderer: DataTables.FunctionColumnRender = function (data: any, t: string, row: any, meta: DataTables.CellMetaSettings): string {
        var html: string = "";

        if (data == JobStatus.Queued) {
            html = "<span style='display: inline-block;' class='label label-warning' data-placement='bottom'  data-toggle='tooltip' title='" + StringResources["Queued"] + "'><i class='fa fa-clock-o fa-3x'></i></label>";
        }
        else if (data == JobStatus.Failed) {
            html = "<span style='display: inline-block;' class='label label-danger' data-placement='bottom'  data-toggle='tooltip'  title='" + StringResources["Failed"] + "'><i class='fa fa-times fa-3x'></i></label>";
        }
        else if (data == JobStatus.Finished) {
            html = "<span style='display: inline-block;' class='label label-success' data-placement='bottom'  data-toggle='tooltip' title='" + StringResources["Successful"] + "'><i class='fa fa-check-circle fa-3x'></i></label>";
        }
        else if (data == JobStatus.Running) {
            html = "<span style='display: inline-block;' class='label label-info' data-placement='bottom'  data-toggle='tooltip' title='" + StringResources["Executing"] + "'><i class='fa fa-circle-o-notch fa-3x fa-spin'></i></label>";
        }

        return html;
    }
}