namespace DataTables {
    $.fn.dataTable.ext.errMode = 'none';
    $.fn.dataTable.ext.buttons.reload = {
        text: 'Reload',
        action: function (e, dt, node, config) {
            dt.ajax.reload();
        }
    };
}
$(function () {
    $('body').on('DOMNodeInserted', function (e) {
        if ($(e.target).is('.dt-button-collection')) {
            $("ul.dt-button-collection li:has(a:empty)").remove();//Remove button column options from colvis
        }
    });
});