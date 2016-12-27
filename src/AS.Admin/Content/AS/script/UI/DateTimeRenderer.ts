class DateTimeRenderer {
    private _selector: string;

    constructor()
    constructor(selector: string)
    constructor(selector?: string)
    constructor(selector: string, value: any)
    constructor(selector?: string, value?: any) {
        this._selector = selector;

        if (value !== undefined && value != null) {
            if (moment(value).diff(new Date(0)) != 0) {
                value.setMinutes(value.getMinutes() + -value.getTimezoneOffset());
                $(this._selector).html(new Date(value.toISOString()).toLocaleString(Helper.GetLanguage()));
            }
            else {
                $(this._selector).html('-');
            }
        }
        else {
            this.start();
        }
    }
    start() {
        this.display(this._selector);
        setInterval(() => this.display(this._selector), 1000);
    }
    display(selector) {
        $(selector).html(new Date().toLocaleString(Helper.GetLanguage()));
    }
}