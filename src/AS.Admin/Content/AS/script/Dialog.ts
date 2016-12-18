function OpenDialog(el: JQuery) {
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
                var gridId: string = this.element.attr('data-related-gridid');

                if (gridId !== undefined) {
                    var grid = window[gridId] as Grid;

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