// Copyright (c) Microsoft. All rights reserved. Licensed under the Apache License, Version 2.0.
// See LICENSE.txt in the project root for complete license information.
//Taken from : https://github.com/bolinfest/typescript/blob/master/src/compiler/hashTable.ts
var BlockIntrinsics = (function () {
    function BlockIntrinsics() {
        this.prototype = undefined;
        this.toString = undefined;
        this.toLocaleString = undefined;
        this.valueOf = undefined;
        this.hasOwnProperty = undefined;
        this.propertyIsEnumerable = undefined;
        this.isPrototypeOf = undefined;
        // initialize the 'constructor' field
        this["constructor"] = undefined;
    }
    return BlockIntrinsics;
}());
var StringHashTable = (function () {
    function StringHashTable() {
        this.itemCount = 0;
        this.table = (new BlockIntrinsics());
    }
    StringHashTable.prototype.getAllKeys = function () {
        var result = [];
        for (var k in this.table) {
            if (this.table[k] != undefined) {
                result[result.length] = k;
            }
        }
        return result;
    };
    StringHashTable.prototype.add = function (key, data) {
        if (this.table[key] != undefined) {
            return false;
        }
        this.table[key] = data;
        this.itemCount++;
        return true;
    };
    StringHashTable.prototype.addOrUpdate = function (key, data) {
        if (this.table[key] != undefined) {
            this.table[key] = data;
            return false;
        }
        this.table[key] = data;
        this.itemCount++;
        return true;
    };
    StringHashTable.prototype.map = function (fn, context) {
        for (var k in this.table) {
            var data = this.table[k];
            if (data != undefined) {
                fn(k, this.table[k], context);
            }
        }
    };
    StringHashTable.prototype.every = function (fn, context) {
        for (var k in this.table) {
            var data = this.table[k];
            if (data != undefined) {
                if (!fn(k, this.table[k], context)) {
                    return false;
                }
            }
        }
        return true;
    };
    StringHashTable.prototype.some = function (fn, context) {
        for (var k in this.table) {
            var data = this.table[k];
            if (data != undefined) {
                if (fn(k, this.table[k], context)) {
                    return true;
                }
            }
        }
        return false;
    };
    StringHashTable.prototype.count = function () { return this.itemCount; };
    StringHashTable.prototype.lookup = function (key) {
        var data = this.table[key];
        if (data != undefined) {
            return data;
        }
        else {
            return (null);
        }
    };
    return StringHashTable;
}());
// The resident table is expected to reference the same table object, whereas the
// transientTable may reference different objects over time
// REVIEW:  WARNING:  For performance reasons, neither the primary nor secondary table may be null
var DualStringHashTable = (function () {
    function DualStringHashTable(primaryTable, secondaryTable) {
        this.primaryTable = primaryTable;
        this.secondaryTable = secondaryTable;
        this.insertPrimary = true;
    }
    DualStringHashTable.prototype.getAllKeys = function () {
        return this.primaryTable.getAllKeys().concat(this.secondaryTable.getAllKeys());
    };
    DualStringHashTable.prototype.add = function (key, data) {
        if (this.insertPrimary) {
            return this.primaryTable.add(key, data);
        }
        else {
            return this.secondaryTable.add(key, data);
        }
    };
    DualStringHashTable.prototype.addOrUpdate = function (key, data) {
        if (this.insertPrimary) {
            return this.primaryTable.addOrUpdate(key, data);
        }
        else {
            return this.secondaryTable.addOrUpdate(key, data);
        }
    };
    DualStringHashTable.prototype.map = function (fn, context) {
        this.primaryTable.map(fn, context);
        this.secondaryTable.map(fn, context);
    };
    DualStringHashTable.prototype.every = function (fn, context) {
        return this.primaryTable.every(fn, context) && this.secondaryTable.every(fn, context);
    };
    DualStringHashTable.prototype.some = function (fn, context) {
        return this.primaryTable.some(fn, context) || this.secondaryTable.some(fn, context);
    };
    DualStringHashTable.prototype.count = function () {
        return this.primaryTable.count() + this.secondaryTable.count();
    };
    DualStringHashTable.prototype.lookup = function (key) {
        var data = this.primaryTable.lookup(key);
        if (data != undefined) {
            return data;
        }
        else {
            return this.secondaryTable.lookup(key);
        }
    };
    return DualStringHashTable;
}());
function numberHashFn(key) {
    var c2 = 0x27d4eb2d; // a prime or an odd constant
    key = (key ^ 61) ^ (key >>> 16);
    key = key + (key << 3);
    key = key ^ (key >>> 4);
    key = key * c2;
    key = key ^ (key >>> 15);
    return key;
}
function combineHashes(key1, key2) {
    return key2 ^ ((key1 >> 5) + key1);
}
var HashEntry = (function () {
    function HashEntry(key, data) {
        this.key = key;
        this.data = data;
    }
    return HashEntry;
}());
var HashTable = (function () {
    function HashTable(size, hashFn, equalsFn) {
        this.size = size;
        this.hashFn = hashFn;
        this.equalsFn = equalsFn;
        this.itemCount = 0;
        this.table = new Array();
        for (var i = 0; i < this.size; i++) {
            this.table[i] = null;
        }
    }
    HashTable.prototype.add = function (key, data) {
        var current;
        var entry = new HashEntry(key, data);
        var val = this.hashFn(key);
        val = val % this.size;
        for (current = this.table[val]; current != null; current = current.next) {
            if (this.equalsFn(key, current.key)) {
                return false;
            }
        }
        entry.next = this.table[val];
        this.table[val] = entry;
        this.itemCount++;
        return true;
    };
    HashTable.prototype.remove = function (key) {
        var current;
        var val = this.hashFn(key);
        val = val % this.size;
        var result = null;
        var prevEntry = null;
        for (current = this.table[val]; current != null; current = current.next) {
            if (this.equalsFn(key, current.key)) {
                result = current.data;
                this.itemCount--;
                if (prevEntry) {
                    prevEntry.next = current.next;
                }
                else {
                    this.table[val] = current.next;
                }
                break;
            }
            prevEntry = current;
        }
        return result;
    };
    HashTable.prototype.count = function () { return this.itemCount; };
    HashTable.prototype.lookup = function (key) {
        var current;
        var val = this.hashFn(key);
        val = val % this.size;
        for (current = this.table[val]; current != null; current = current.next) {
            if (this.equalsFn(key, current.key)) {
                return (current.data);
            }
        }
        return (null);
    };
    return HashTable;
}());
// Simple Hash table with list of keys and values matching each other at the given index
var SimpleHashTable = (function () {
    function SimpleHashTable() {
        this.keys = [];
        this.values = [];
    }
    SimpleHashTable.prototype.lookup = function (key, findValue) {
        var searchArray = this.keys;
        if (findValue) {
            searchArray = this.values;
        }
        for (var i = 0; i < searchArray.length; i++) {
            if (searchArray[i] == key) {
                return {
                    key: this.keys[i],
                    data: this.values[i],
                };
            }
        }
        return null;
    };
    SimpleHashTable.prototype.add = function (key, data) {
        var lookupData = this.lookup(key);
        if (lookupData) {
            return false;
        }
        this.keys[this.keys.length] = key;
        this.values[this.values.length] = data;
        return true;
    };
    return SimpleHashTable;
}());
function OpenDialog(el) {
    var width = parseInt($(el).attr('data-iframeWidth'));
    var height = parseInt($(el).attr('data-iframeHeight'));
    $("#dialogOpen").attr('href', $(el).attr('href'));
    var gridId = "grid";
    if (width > screen.width) {
        width = screen.width - 50;
    }
    if (width > screen.height) {
        height = screen.height - 50;
    }
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
var DomManager = (function () {
    function DomManager() {
    }
    DomManager.prototype.initializeDocument = function () {
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
        $("div.form-group > div.form-group-input input.text-box").addClass("form-control");
        $("div.form-group > div.form-group-input textarea.text-box").addClass("form-control");
        $.each($('input[type="datetime"] , .datetime-field'), function (key, item) {
            try {
                var dateStr = item[item.value !== undefined ? "value" : "innerHTML"];
                var date = new Date(dateStr);
                date.setMinutes(date.getMinutes() + -date.getTimezoneOffset());
                item[item.value !== undefined ? "value" : "innerHTML"] = new Date(date.toISOString()).toLocaleString(navigator.languages[0]);
            }
            catch (e) {
                console.log(e);
            }
        });
    };
    return DomManager;
}());
var FormInputType;
(function (FormInputType) {
    FormInputType[FormInputType["Text"] = 0] = "Text";
    FormInputType[FormInputType["Email"] = 1] = "Email";
    FormInputType[FormInputType["Url"] = 2] = "Url";
    FormInputType[FormInputType["MultiLine"] = 3] = "MultiLine";
    FormInputType[FormInputType["Checkbox"] = 4] = "Checkbox";
    FormInputType[FormInputType["Password"] = 5] = "Password";
    FormInputType[FormInputType["DigitOnly"] = 6] = "DigitOnly";
    FormInputType[FormInputType["Html"] = 7] = "Html";
})(FormInputType || (FormInputType = {}));
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
    new DomManager().initializeDocument();
});
$(document).ajaxError(function (evt, xhr, opts) {
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_DANGER,
        title: StringResources["AjaxErrorTitle"],
        message: JSON.parse(xhr.responseText),
        buttons: [
            {
                label: StringResources["Ok"],
                action: function (dialog) {
                    dialog.close();
                }
            }
        ]
    });
});
$(document).ajaxComplete(function () {
    new DomManager().initializeDocument();
});
$.ajaxSetup({
    headers: { 'ClientTimeZone': new Date().getTimezoneOffset() }
});
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
var DateTimeRenderer = (function () {
    function DateTimeRenderer(selector, value) {
        this._selector = selector;
        if (value !== undefined && value != null) {
            if (moment(value).diff(new Date(0)) != 0) {
                value.setMinutes(value.getMinutes() + -value.getTimezoneOffset());
                $(this._selector).html(new Date(value.toISOString()).toLocaleString(navigator.languages[0]));
            }
            else {
                $(this._selector).html('-');
            }
        }
        else {
            this.start();
        }
    }
    DateTimeRenderer.prototype.start = function () {
        var _this = this;
        this.display(this._selector);
        setInterval(function () { return _this.display(_this._selector); }, 1000);
    };
    DateTimeRenderer.prototype.display = function (selector) {
        $(selector).html(new Date().toLocaleString(navigator.languages[0]));
    };
    return DateTimeRenderer;
}());
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
        html += "<i class='fa fa-trash-o fw'></i>" + StringResources["Delete"] + "</a>";
        return html;
    };
    CellRenderers.EditButtonRenderer = function (data, t, row, meta) {
        var html = "<a class='btn btn-primary btn-rowButton dialog-button' href='Edit?id='" + data + "'>";
        html += "<i class='fa fa-pencil-square-o fw'></i>" + StringResources["Edit"] + "</a>";
        return html;
    };
    CellRenderers.JobStatusRenderer = function (data, t, row, meta) {
        var html = "";
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
    };
})(CellRenderers || (CellRenderers = {}));
var DataTables;
(function (DataTables) {
    $.fn.dataTable.ext.errMode = 'none';
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
var Grid = (function () {
    function Grid(idColumnName) {
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
    Object.defineProperty(Grid.prototype, "Responsive", {
        /* Getters/Setters */
        set: function (value) {
            this._responsive = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "HasDeleteButton", {
        set: function (value) {
            this._hasDeleteButton = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "id", {
        set: function (value) {
            this._id = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "width", {
        set: function (value) {
            this._width = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "iDisplayLength", {
        set: function (value) {
            this._iDisplayLength = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "Searching", {
        set: function (value) {
            this._searching = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "ServerSideProcessing", {
        set: function (value) {
            this._serverSideProcessing = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "Processing", {
        set: function (value) {
            this._processing = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "Info", {
        set: function (value) {
            this._info = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "Ordering", {
        set: function (value) {
            this._ordering = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "Paging", {
        set: function (value) {
            this._paging = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "Columns", {
        get: function () {
            return this._columns;
        },
        set: function (value) {
            this._columns = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "StateSave", {
        set: function (value) {
            this._stateSave = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "Order", {
        set: function (value) {
            this._order = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "UrlGenerator", {
        set: function (value) {
            this._urlGenerator = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Grid.prototype, "DrawCallBack", {
        set: function (value) {
            this._drawCallBack = value;
        },
        enumerable: true,
        configurable: true
    });
    /* Functions */
    Grid.prototype.render = function () {
        this._gridButtons = [];
        this._gridButtons.push(GridButtons.ColumnVisibility());
        this._gridButtons.push(GridButtons.Reload());
        if (this._hasDeleteButton) {
            var col = new GridColumn("", this._idColumnName, CellRenderers.DeleteButtonRenderer);
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
            for (var i_1 = 0; i_1 < this._columns.length; i_1++) {
                dtCols.push(this._columns[i_1].ToDtColSetting());
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
                                label: StringResources["No"],
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }]
                    });
                });
                InitDialog();
                $('[data-toggle="tooltip"]').tooltip();
            }
        });
    };
    Grid.prototype.reload = function () {
        $(".dataTables_length select").trigger('change');
        this._table.api().ajax.url(this._urlGenerator()).load();
    };
    return Grid;
}());
var GridButtons = (function () {
    function GridButtons() {
    }
    GridButtons.ColumnVisibility = function () {
        return {
            extend: 'colvis',
            titleAttr: StringResources["DataTables_colvis"],
            text: "<i class='fa fa-list-alt'></i>",
            className: "btn-default",
            columns: ':not(.button-cell)'
        };
    };
    GridButtons.Reload = function () {
        return {
            extend: 'reload',
            titleAttr: StringResources["DataTables_reload"],
            text: "<i class='fa fa-refresh'></i>",
            className: "btn-info"
        };
    };
    return GridButtons;
}());
var GridColumn = (function () {
    function GridColumn(title, data, renderer) {
        this._orderable = false;
        this._className = undefined;
        this._width = undefined;
        this._title = title;
        this._data = data;
        this._renderer = renderer;
    }
    Object.defineProperty(GridColumn.prototype, "Title", {
        get: function () {
            return this._title;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(GridColumn.prototype, "Data", {
        get: function () {
            return this._data;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(GridColumn.prototype, "Orderable", {
        set: function (value) {
            this._orderable = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(GridColumn.prototype, "Renderer", {
        set: function (value) {
            this._renderer = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(GridColumn.prototype, "ClassName", {
        set: function (value) {
            this._className = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(GridColumn.prototype, "Width", {
        get: function () {
            return this._width;
        },
        set: function (value) {
            this._width = value;
        },
        enumerable: true,
        configurable: true
    });
    GridColumn.prototype.ToDtColSetting = function () {
        return {
            data: this._data,
            orderable: this._orderable,
            render: this._renderer,
            className: this._className
        };
    };
    return GridColumn;
}());
var RowButtons = (function () {
    function RowButtons() {
    }
    RowButtons.UpdateButton = function (gridId, columnName) {
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
    };
    return RowButtons;
}());
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
var ResetPageButton = (function () {
    function ResetPageButton(id) {
        this._id = id;
        this._items = new StringHashTable();
        var button = this;
        $("#" + id).attr('type', 'button');
        $(document).ready(function () {
            var DefaultId = "FormElementId";
            var idCounter = 0;
            $("input,select").each(function (i, elem) {
                var id = $(this).attr('id');
                if (id === undefined) {
                    id = DefaultId + idCounter++;
                    $(this).attr('id', id);
                }
                button._items.addOrUpdate(id, $(this).val());
            });
            $(document).on('click', "#" + id, { button: button }, function (event) {
                button.reset();
            });
        });
    }
    ResetPageButton.prototype.reset = function () {
        for (var i = 0; i < this._items.count(); i++) {
            var id = this._items.getAllKeys()[i];
            var value = this._items.lookup(id);
            $("#" + id).val(value);
        }
        if (grid !== undefined) {
            (grid).reload();
        }
        if (typeof $.fn.selectpicker !== "undefined") {
            $('select.selectpicker').selectpicker('refresh');
        }
    };
    return ResetPageButton;
}());
//# sourceMappingURL=asframe.js.map