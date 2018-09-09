//Левое меню
Ext.define("PartionnyAccount.view.Sklad/Container/viewContainerLeft", {
    extend: "Ext.form.Panel",
    alias: "widget.viewContainerLeft",
    title: "Навигатор",
    width: 275,
    region: "west",
    split: true,
    collapsible: true, //кнопка ">>" - спрятать
    //closable: true,  //кнопка "Х" - закрыть
    autoScroll: true,
    loadMask: true,
    layout: 'accordion',
    items: [
        { xtype: "viewPanelMain", id: "viewPanelMain" },
        { xtype: "viewPanelDir", id: "viewPanelDir" },
        { xtype: "viewPanelDoc", id: "viewPanelDoc" },
        { xtype: "viewPanelOrder", id: "viewPanelOrder" },
        { xtype: "viewPanelCashOffice", id: "viewPanelCashOffice" },
        { xtype: "viewPanelReport", id: "viewPanelReport" }
    ]
});