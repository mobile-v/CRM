//Сентральная панель
Ext.define("PartionnyAccount.view.Sklad/Container/viewContainerCentralPanel", {
    extend: "Ext.panel.Panel",
    alias: "widget.viewContainerCentralPanel",
    layout: {
        type: "border",
        padding: 0
    },
    //title: "...",
    region: "center",
    bodyStyle: "background-color: white", //bodyStyle: "background-color: transparent !important",
    //autoScroll: true,
    //floating: false
    bodyPadding: varBodyPadding,
});