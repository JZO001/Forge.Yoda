import { dxElement } from "devextreme/core/element"
import { dxEvent } from "devextreme/events"
import dxCheckBox from "devextreme/ui/check_box"
import dxDataGrid from "devextreme/ui/data_grid"
import dxDraggable from "devextreme/ui/draggable"
import dxDropDownButton from "devextreme/ui/drop_down_button"
import dxList from "devextreme/ui/list"
import dxMenu from "devextreme/ui/menu"
import dxPivotGrid, { dxPivotGridPivotGridCell } from "devextreme/ui/pivot_grid"
import { PivotGridDataSourceField } from "devextreme/ui/pivot_grid/data_source"
import dxSortable from "devextreme/ui/sortable"
import dxTreeList from "devextreme/ui/tree_list"

export type ChartSubtitle = {
    font?: string,
    offset?: string,
    text?: string
}

export type ChartSeriesData = {
    name?: any,
    valueField?: any
}

export type TooltipData = {
    seriesName?: string,
    valueText?: string
}

export type DxEventArgs<T> = {
    component?: T,
    element?: dxElement,
    model?: any
}

export type DxEventEventArgs<T> = DxEventArgs<T> & {
    jQueryEvent?: unknown,
    event?: dxEvent | unknown
}

export type ValueChangedEventArgs<T> = DxEventEventArgs<T> & {
    value?: any,
    previousValue?: any,
}

export type ClickEventArgs<T> = DxEventEventArgs<T> & {
    validationGroup?: any
}

export type DropDownButtonSelectionChangedEventArgs = DxEventArgs<dxDropDownButton> & {
    item?: any,
    previousItem?: any,
}

export type TreeListSelectionChangedEventArgs<T> = DxEventArgs<dxTreeList> & {
    currentSelectedRowKeys?: Array<any>,
    currentDeselectedRowKeys?: Array<any>,
    selectedRowKeys?: Array<any>,
    selectedRowsData?: Array<T>
}

export type ListSelectionChangedEventArgs = DxEventArgs<dxList> & {
    addedItems?: Array<any>,
    removedItems?: Array<any>
}

export type GridSelectionChangedEventArgs = DxEventArgs<dxDataGrid> & {
    currentSelectedRowKeys?: Array<any>,
    currentDeselectedRowKeys?: Array<any>,
    selectedRowKeys?: Array<any>,
    selectedRowsData?: Array<any>
}

export type PivotGridCellPreparedEventArgs = DxEventArgs<dxPivotGrid> & {
    area?: string,
    cellElement?: dxElement,
    cell?: dxPivotGridPivotGridCell,
    rowIndex?: number,
    columnIndex?: number
}

export type PivotGridCellClickEventArgs = DxEventEventArgs<dxPivotGrid> & {
    area?: string,
    cellElement?: dxElement,
    cell?: dxPivotGridPivotGridCell,
    rowIndex?: number,
    columnIndex?: number,
    columnFields?: Array<PivotGridDataSourceField>,
    rowFields?: Array<PivotGridDataSourceField>,
    dataFields?: Array<PivotGridDataSourceField>,
    cancel?: boolean
}

export type MenuItemClickEventArgs = DxEventEventArgs<dxMenu> & {
    itemData?: any,
    itemElement?: dxElement,
    itemIndex?: number
}

export type OptionChangedEventArgs<T> = DxEventArgs<T> & {
    name?: string,
    fullName?: string,
    value?: any,
    previousValue?: any
}

export type SortableDragStartEventArgs = DxEventArgs<dxSortable> & {
    event?: dxEvent | unknown
    cancel?: boolean,
    itemData?: any,
    itemElement?: dxElement,
    fromIndex?: number,
    fromData?: any
}

export type SortableReorderEventArgs = DxEventArgs<dxSortable> & {
    event?: dxEvent | unknown
    itemData?: any,
    itemElement?: dxElement,
    fromIndex?: number,
    toIndex?: number,
    fromComponent?: dxSortable | dxDraggable,
    toComponent?: dxSortable | dxDraggable,
    fromData?: any,
    toData?: any,
    dropInsideItem?: boolean,
    promise?: Promise<void> | unknown
}

export type MenuClickItemEventArgs = DxEventArgs<dxMenu> & {
    event?: dxEvent | unknown
    itemData?: any,
    itemElement?: dxElement,
    itemIndex?: number
}

export type CheckBoxValueChangedEventArgs = DxEventArgs<dxCheckBox> & {
    value?: any,
    previousValue?: any,
    event?: dxEvent | unknown
}
