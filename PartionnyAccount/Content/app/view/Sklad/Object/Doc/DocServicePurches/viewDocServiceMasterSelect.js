Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocServicePurches/viewDocServiceMasterSelect", {
    extend: "Ext.Window", //extend: "Ext.panel.Panel",
    //extend: InterfaceSystemObjName,
    alias: "widget.viewDocServiceMasterSelect",

    layout: "border",
    region: "center",
    title: "Выбор мастера",
    width: 350, height: 150,
    autoScroll: false,
    closable: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {
        
        //1. General-Panel
        var formPanelEdit = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanGeneral,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,
            autoHeight: true,

            items: [

                { xtype: 'container', height: 5 },

                {
                    xtype: 'viewComboBox',
                    //fieldLabel: lanWarehouse,
                    flex: 1, allowBlank: false, style: "height: 25px;",
                    margin: "0 0 0 5",
                    store: this.storeDirEmployeesGrid, // store getting items from server
                    valueField: 'DirEmployeeID',
                    hiddenName: 'DirEmployeeID',
                    displayField: 'DirEmployeeName',
                    name: 'DirEmployeeID', itemId: "DirEmployeeID", id: "DirEmployeeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, typeAhead: false, minChars: 200,
                },
            ],


            //buttonAlign: 'left',
            buttons: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave", style: "width: 120px; height: 40px;",
                    text: "<font size=" + HeaderMenu_FontSize_1 + "><b>Выбрать</b></font>", icon: '../Scripts/sklad/images/save.png'
                }
            ]

        });



        //body
        this.items = [

            formPanelEdit

        ],


        this.callParent(arguments);
    }

});