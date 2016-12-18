(function ($) {
    $.fn.backTop = function () {
        var backBtn = this;
        var position = 1000;
        var speed = 900;
        $(document).scroll(function () {
            var pos = $(window).scrollTop();
            if (pos >= position) {
                backBtn.fadeIn(speed);
            }
            else {
                backBtn.fadeOut(speed);
            }
        });
        backBtn.click(function () {
            $("html, body").animate({ scrollTop: 0 }, 900);
        });
    };
}(jQuery));
class DateTimeDisplayer {
    constructor(elementId) {
        this._elementId = elementId;
    }
    start() {
        this.display(this._elementId);
        setInterval(() => this.display(this._elementId), 1000);
    }
    display(elementId) {
        $("#" + elementId).html(new Date().toLocaleString(navigator.languages[0]));
    }
}
function OpenDialog(el) {
    var width = parseInt($(el).attr('data-iframeWidth'));
    var height = parseInt($(el).attr('data-iframeHeight'));
    $("#dialogOpen").attr('href', $(el).attr('href'));
    var gridId = "grid";
    if ($(el).attr('data-related-gridid') !== undefined) {
        gridId = $(el).attr('data-related-gridid');
    }
    $("#dialogOpen").attr('data-related-gridid', gridId);
    window.reload = false;
    $("#dialogOpen").fancybox({
        maxWidth: 1400,
        maxHeight: 1000,
        fitToView: false,
        autoSize: false,
        width: width,
        height: height,
        closeClick: false,
        openEffect: 'fade',
        closeEffect: 'fade',
        openSpeed: 400,
        helpers: {
            overlay: { closeClick: false }
        },
        afterClose: function () {
            if (window.reload) {
                var gridId = this.element.attr('data-related-gridid');
                if (gridId !== undefined) {
                    var grid = window[gridId];
                    if (grid != null && grid !== undefined) {
                        grid.reload();
                    }
                }
            }
        }
    }).click();
}
function InitDialog() {
    $(".fancyBoxImage").fancybox();
    $("#dialogOpen").remove();
    $("body").append("<a id='dialogOpen' data-fancybox-type='iframe' style='display:none;' />");
    $(".dialog-button").unbind('click');
    $(".dialog-button").bind('click', function (e) {
        var popupTrigger = $(this);
        OpenDialog(popupTrigger);
        e.preventDefault();
        return false;
    });
}
$(document).ready(function () {
    InitDialog();
});
var JobStatus;
(function (JobStatus) {
    JobStatus[JobStatus["Queued"] = 0] = "Queued";
    JobStatus[JobStatus["Running"] = 1] = "Running";
    JobStatus[JobStatus["Finished"] = 2] = "Finished";
    JobStatus[JobStatus["Failed"] = 3] = "Failed";
})(JobStatus || (JobStatus = {}));
$(document).ready(function () {
    $('input').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
        increaseArea: '20%' // optional
    });
    $('select.multipleSelect').multipleSelect();
    $.each($('.input-validation-error'), function (key, item) {
        $(item).parents('.form-group').addClass('has-error');
    });
    $("input.input-validation-error").blur(function () {
        if ($(this).val().length > 0) {
            $(this).parents('.form-group').removeClass('has-error');
            $(this).parents('.form-group').find('span.field-validation-error').html('');
        }
    });
    $('.field-validation-error').addClass('text-danger');
    if ($('.validation-summary-errors li:hidden').length == 0) {
        $('.validation-summary-errors').addClass('alert alert-danger');
        $('.validation-summary-errors').attr('role', 'alert');
    }
});
$(document).ajaxError(function (evt, xhr, opts) {
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_DANGER,
        title: SM["UI_AjaxErrorTitle"],
        message: JSON.parse(xhr.responseText),
        buttons: [
            {
                label: SM["UI_Ok"],
                action: function (dialog) {
                    dialog.close();
                }
            }
        ]
    });
});
$.ajaxSetup({
    headers: { 'ClientTimeZone': new Date().getTimezoneOffset() }
});
//class ResetPageButton {   
//    constructor() {
//        //var dict = new Dictionary<string, string>();
//    }
//} 
var CellRenderers;
(function (CellRenderers) {
    CellRenderers.DateTimeRenderer = function (data, t, row, meta) {
        if (data != null) {
            return new Date(data).toLocaleString(navigator.languages[0]);
        }
        else {
            return "";
        }
    };
    CellRenderers.DeleteButtonRenderer = function (data, t, row, meta) {
        var html = "<a class='btn btn-danger btn-rowButton btn-rowDelete' href='#' id='" + data + "'>";
        html += "<i class='fa fa-trash-o fw'></i>" + SM["UI_Delete"] + "</a>";
        return html;
    };
    CellRenderers.EditButtonRenderer = function (data, t, row, meta) {
        var html = "<a class='btn btn-primary btn-rowButton dialog-button' href='Edit?id='" + data + "'>";
        html += "<i class='fa fa-pencil-square-o fw'></i>" + SM["UI_Edit"] + "</a>";
        return html;
    };
    CellRenderers.JobStatusRenderer = function (data, t, row, meta) {
        var html = "";
        if (data == JobStatus.Queued) {
            html = "<span style='display: inline-block;' class='label label-warning' data-placement='bottom'  data-toggle='tooltip' title='" + SM["UI_Queued"] + "'><i class='fa fa-clock-o fa-3x'></i></label>";
        }
        else if (data == JobStatus.Failed) {
            html = "<span style='display: inline-block;' class='label label-danger' data-placement='bottom'  data-toggle='tooltip'  title='" + SM["UI_Failed"] + "'><i class='fa fa-times fa-3x'></i></label>";
        }
        else if (data == JobStatus.Finished) {
            html = "<span style='display: inline-block;' class='label label-success' data-placement='bottom'  data-toggle='tooltip' title='" + SM["UI_Successful"] + "'><i class='fa fa-check-circle fa-3x'></i></label>";
        }
        else if (data == JobStatus.Running) {
            html = "<span style='display: inline-block;' class='label label-info' data-placement='bottom'  data-toggle='tooltip' title='" + SM["UI_Executing"] + "'><i class='fa fa-circle-o-notch fa-3x fa-spin'></i></label>";
        }
        return html;
    };
})(CellRenderers || (CellRenderers = {}));
var DataTables;
(function (DataTables) {
    $.fn.dataTable.ext.errMode = 'throw';
    $.fn.dataTable.ext.buttons.reload = {
        text: 'Reload',
        action: function (e, dt, node, config) {
            dt.ajax.reload();
        }
    };
})(DataTables || (DataTables = {}));
$(function () {
    $('body').on('DOMNodeInserted', function (e) {
        if ($(e.target).is('.dt-button-collection')) {
            $("ul.dt-button-collection li:has(a:empty)").remove(); //Remove button column options from colvis
        }
    });
});
class Grid {
    constructor(idColumnName) {
        this._id = "asGrid";
        this._width = "100%";
        this._iDisplayLength = -1;
        this._searching = false;
        this._serverSideProcessing = false;
        this._processing = true;
        this._info = true;
        this._ordering = true;
        this._paging = true;
        this._stateSave = false;
        this._dom = "lfrBtip";
        this._columns = [];
        this._gridButtons = [];
        this._hasDeleteButton = false;
        this._cellSelectable = true;
        this._responsive = false;
        this._idColumnName = idColumnName;
    }
    /* Getters/Setters */
    set Responsive(value) {
        this._responsive = value;
    }
    set HasDeleteButton(value) {
        this._hasDeleteButton = value;
    }
    set id(value) {
        this._id = value;
    }
    set width(value) {
        this._width = value;
    }
    set iDisplayLength(value) {
        this._iDisplayLength = value;
    }
    set Searching(value) {
        this._searching = value;
    }
    set ServerSideProcessing(value) {
        this._serverSideProcessing = value;
    }
    set Processing(value) {
        this._processing = value;
    }
    set Info(value) {
        this._info = value;
    }
    set Ordering(value) {
        this._ordering = value;
    }
    set Paging(value) {
        this._paging = value;
    }
    set Columns(value) {
        this._columns = value;
    }
    set StateSave(value) {
        this._stateSave = value;
    }
    set Order(value) {
        this._order = value;
    }
    set UrlGenerator(value) {
        this._urlGenerator = value;
    }
    get Columns() {
        return this._columns;
    }
    /* Functions */
    render() {
        this._gridButtons = [];
        this._gridButtons.push(GridButtons.ColumnVisibility());
        this._gridButtons.push(GridButtons.Reload());
        if (this._hasDeleteButton) {
            let col = new GridColumn("", this._idColumnName, CellRenderers.DeleteButtonRenderer);
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
        var dtCols = [];
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
            drawCallback: function (settings) {
                $('[data-toggle="tooltip"]').tooltip();
                $(".btn-rowDelete").on('click', function () {
                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_PRIMARY,
                        data: { 'id': $(this).attr('id'), 'dtInstance': settings.oInstance },
                        title: SM["UI_DataTables_DeleteConfirmTitle"],
                        message: SM["UI_DataTables_DeleteConfirmMessage"],
                        buttons: [
                            {
                                label: SM["UI_Yes"],
                                action: function (dialog) {
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
                                label: SM["UI_No"],
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }]
                    });
                });
                InitDialog();
            }
        });
    }
    reload() {
        this._table.api().ajax.url(this._urlGenerator()).load();
    }
}
class GridButtons {
    static ColumnVisibility() {
        return {
            extend: 'colvis',
            titleAttr: SM["UI_DataTables_colvis"],
            text: "<i class='fa fa-list-alt'></i>",
            className: "btn-default",
            columns: ':not(.button-cell)'
        };
    }
    static Reload() {
        return {
            extend: 'reload',
            titleAttr: SM["UI_DataTables_reload"],
            text: "<i class='fa fa-refresh'></i>",
            className: "btn-info"
        };
    }
}
class GridColumn {
    constructor(title, data, renderer) {
        this._orderable = false;
        this._className = undefined;
        this._width = undefined;
        this._title = title;
        this._data = data;
        this._renderer = renderer;
    }
    get Title() {
        return this._title;
    }
    get Data() {
        return this._data;
    }
    set Orderable(value) {
        this._orderable = value;
    }
    set Renderer(value) {
        this._renderer = value;
    }
    set ClassName(value) {
        this._className = value;
    }
    set Width(value) {
        this._width = value;
    }
    get Width() {
        return this._width;
    }
    ToDtColSetting() {
        return {
            data: this._data,
            orderable: this._orderable,
            render: this._renderer,
            className: this._className
        };
    }
}
class RowButtons {
    static UpdateButton(gridId, columnName) {
        function renderUpdateButton(data, t, row, meta) {
            var html = "<a  data-related-gridid='" + gridId + "' data-iframeWidth='800' data-iframeHeight='350' class='btn btn-primary btn-rowButton dialog-button' href='Edit?id=" + data + "'>";
            html += "<i class='fa fa-pencil-square-o fw'></i>" + SM["UI_Update"] + "</a>";
            return html;
        }
        var col = new GridColumn("", columnName, renderUpdateButton);
        col.Orderable = false;
        col.Width = "50px";
        col.ClassName = "button-cell";
        return col;
    }
}
var TableStyles;
(function (TableStyles) {
    TableStyles[TableStyles["None"] = 1] = "None";
    TableStyles[TableStyles["CellBorder"] = 2] = "CellBorder";
    TableStyles[TableStyles["OrderColumn"] = 4] = "OrderColumn";
    TableStyles[TableStyles["Stripe"] = 8] = "Stripe";
    TableStyles[TableStyles["Compact"] = 16] = "Compact";
    TableStyles[TableStyles["Hover"] = 32] = "Hover";
    TableStyles[TableStyles["RowBorder"] = 64] = "RowBorder";
})(TableStyles || (TableStyles = {}));
