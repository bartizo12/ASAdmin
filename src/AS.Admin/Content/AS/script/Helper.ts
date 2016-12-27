module Helper {
    export function GetLanguage(): string {
        if (navigator.userLanguage !== undefined) {
            return navigator.userLanguage;
        }
        return navigator.languages[0];
    }
    export function InitializeDocument() {
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
                
                item[item.value !== undefined ? "value" : "innerHTML"] = new Date(date.toISOString()).toLocaleString(Helper.GetLanguage());
            }
            catch (e) {
                console.log(e);
            }
        });
    }
}