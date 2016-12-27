$(document).ready(function () {
    $('input').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
        increaseArea: '20%' // optional
    });
    Helper.InitializeDocument();
});
$(document).ajaxError(function (evt: any, xhr: any, opts: any): any {
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_DANGER,
        title: StringResources["AjaxErrorTitle"],
        message: JSON.parse(xhr.responseText),
        buttons: [
            {
                label: StringResources["Ok"],
                action: function (dialog: IBootstrapDialogContext) {
                    dialog.close();
                }
            }
        ]
    });
});
$(document).ajaxComplete(function () {
    Helper.InitializeDocument();
});
$.ajaxSetup({
    headers: { 'ClientTimeZone': new Date().getTimezoneOffset() }
});