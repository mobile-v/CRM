/*
    Открытия Окон (Табов, Панелей) в ДРУГИХ, НЕ центральной панели!
*/
function ObjectShow_NotCentral(widgetX, ContainerWidget) {

    //Контейнер в котором будет распологатся Виджет (напри: "viewContainerCentral" или в "PanelNomen")
    if (ContainerWidget == undefined) { Ext.Msg.alert(lanFailure, "ContainerWidget container not found! Press F5 key!"); return; }

    //widgetX.closable = true;
    widgetX.width = "100%"; widgetX.height = "100%";
    ContainerWidget.add(widgetX);

}