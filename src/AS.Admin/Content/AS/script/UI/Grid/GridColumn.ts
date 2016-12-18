class GridColumn {
    private _title: string;
    private _data: string;
    private _orderable: boolean = false;
    private _renderer: DataTables.FunctionColumnRender;
    private _className: string = undefined;
    private _width: string = undefined;

    constructor(title: string, data: string)
    constructor(title: string, data: string, renderer: DataTables.FunctionColumnRender)
    constructor(title: string, data: string, renderer?: DataTables.FunctionColumnRender)
    constructor(title: string, data: string, renderer?: DataTables.FunctionColumnRender) {
        this._title = title;
        this._data = data;
        this._renderer = renderer;
    }
    get Title(): string {
        return this._title;
    }
    get Data(): string {
        return this._data;
    }
    set Orderable(value: boolean) {
        this._orderable = value;
    }
    set Renderer(value: DataTables.FunctionColumnRender) {
        this._renderer = value;
    }
    set ClassName(value: string) {
        this._className = value;
    }
    set Width(value: string) {
        this._width = value;
    }
    get Width() {
        return this._width;
    }
    ToDtColSetting(): DataTables.ColumnSettings {
        return {
            data: this._data,
            orderable: this._orderable,
            render: this._renderer,
            className: this._className
        };
    }
}