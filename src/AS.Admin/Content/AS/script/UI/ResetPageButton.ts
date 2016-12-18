class ResetPageButton {
    private _items: StringHashTable;
    private _id: string;

    constructor(id: string) {
        this._id = id;
        this._items = new StringHashTable();
        var button = this;
        $("#" + id).attr('type', 'button');

        $(document).ready(function () {
            const DefaultId = "FormElementId";
            let idCounter = 0;

            $("input,select").each(function (i, elem) {
                let id = $(this).attr('id');

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
    reset() {
        for (let i = 0; i < this._items.count(); i++) {
            let id = this._items.getAllKeys()[i];
            let value = this._items.lookup(id);
            $("#" + id).val(value);
        }
        if (grid !== undefined) {
            ((grid) as Grid).reload();
        }
        if (typeof $.fn.selectpicker !== "undefined") {
            $('select.selectpicker').selectpicker('refresh');
        }
    }
}