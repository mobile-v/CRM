Ext.define("PartionnyAccount.view.Sklad/Container/ContainerLeft/viewPanelJournal", {
    extend: "Ext.tree.Panel",
    alias: "widget.viewPanelJournal",
    title: "Документы",
    width: 200,
    split: true,
    collapsible: true, //кнопка ">>" - спрятать
    bodyStyle: "background-color: transparent !important",

    rootVisible: false,
    store: "Sklad/Container/ContainerLeft/storePanelJournal"
});