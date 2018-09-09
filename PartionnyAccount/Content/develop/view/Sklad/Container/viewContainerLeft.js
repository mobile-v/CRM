//Левое меню
Ext.define("PartionnyAccount.view.Sklad/Container/viewContainerLeft", {
    extend: "Ext.form.Panel",
    alias: "widget.viewContainerLeft",
    title: "Навигатор",
    width: 250,
    region: "west",
    split: true,
    collapsible: true, //кнопка ">>" - спрятать
    //closable: true,  //кнопка "Х" - закрыть
    autoScroll: true,
    loadMask: true,
    layout: 'accordion',
    items: [
        { xtype: "viewPanelJournal", id: "viewPanelJournal", itemId: "viewPanelJournal" }
    ]
});