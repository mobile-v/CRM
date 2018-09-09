//Сентральная панель
Ext.define("PartionnyAccount.view.Sklad/Container/viewContainerCentralTab", {
    extend: "Ext.tab.Panel",
    alias: "widget.viewContainerCentralTab",
    layout: {
        type: "border",
        padding: 3
    },
    //title: "...",
    region: "center",
    bodyStyle: "background-color: transparent !important",
    //autoScroll: true,
    //floating: false,
    bodyPadding: varBodyPadding,
});