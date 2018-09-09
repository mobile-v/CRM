Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewDirNomensEdit", {
    extend: "Ext.panel.Panel",
    //extend: InterfaceSystemObjName,
    alias: "widget.viewDirNomensEdit",

    layout: "border",
    region: "center",
    //title: lanGoods,
    //width: 750, height: 350,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    //Контроллер
    controller: 'viewcontrollerDirNomens',

    conf: {},

    initComponent: function () {

        var SettingsKKMSTax_values = [
            [0, "0 (НДС 0%)"],
            [10, "10 (НДС 10%)"],
            [18, "18 (НДС 18%)"],
            [-1, "-1 (НДС не облагается)"],
            [118, "118 (НДС 18/118)"],
            [110, "110 (НДС 10/110)"],
        ];
        var GeneralPanel = Ext.create('Ext.panel.Panel', {
            id: "generalPanel_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanPrimary,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',

            defaults: { anchor: '100%' },
            //width: "100%", height: 115 + varBodyPadding,
            autoScroll: true,
            //split: true,

            items: [

                { xtype: 'container', height: 5 },

                //ID
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: "Код (старый)", name: "DirNomenID_OLD", id: "DirNomenID_OLD" + this.UO_id, allowBlank: true, readOnly: true, margin: "0 0 0 10", hidden: true },
                        { xtype: 'textfield', fieldLabel: "Sub", name: "Sub", id: "Sub" + this.UO_id, allowBlank: true, hidden: true }
                    ]
                },

                //Article + NomenType
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: "Артикул", name: "DirNomenID", id: "DirNomenID" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: true, hidden: true, readOnly: true },
                        { xtype: 'textfield', fieldLabel: "Артикул", name: "DirNomenID_INSERT", id: "DirNomenID_INSERT" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: true, readOnly: true },
                        //{ xtype: 'textfield', fieldLabel: lanArticle, name: "DirNomenArticle", id: "DirNomenArticle" + this.UO_id, flex: 1, allowBlank: true, readOnly: true },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanNomenType, allowBlank: false, flex: 1, //, emptyText: "Тип"
                            margin: "0 0 0 10",
                            store: this.storeDirNomenTypesGrid, // store getting items from server
                            valueField: 'DirNomenTypeID',
                            hiddenName: 'DirNomenTypeID',
                            displayField: 'DirNomenTypeName',
                            name: 'DirNomenTypeID', itemId: "DirNomenTypeID", id: "DirNomenTypeID" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },

                { xtype: 'container', height: 5 },

                { xtype: 'textfield', fieldLabel: lanName, name: "DirNomenName", id: "DirNomenName" + this.UO_id, flex: 1, allowBlank: false },
                { xtype: 'textfield', fieldLabel: lanNameFull, name: "DirNomenNameFull", id: "DirNomenNameFull" + this.UO_id, flex: 1, allowBlank: true },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },


                {
                    xtype:'label',
                    text: "Если товар, то выбор категории обязателен!",
                    cls: 'x-form-item myBold',
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Категория", allowBlank: true, flex: 1, //, emptyText: "Тип"
                    
                            store: this.storeDirNomenCategoriesGrid, // store getting items from server
                            valueField: 'DirNomenCategoryID',
                            hiddenName: 'DirNomenCategoryID',
                            displayField: 'DirNomenCategoryName',
                            name: 'DirNomenCategoryID', itemId: "DirNomenCategoryID", id: "DirNomenCategoryID" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

                            editable: true,
                            typeAhead: true,
                            minChars: 2,
                        },
                        { xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirNomenCategoryEdit", id: "btnDirNomenCategoryEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirNomenCategoryReload", id: "btnDirNomenCategoryReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', tooltip: "Clear", itemId: "btnDirNomenCategoryClear", text: "X", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }
                    ]
                },



                { xtype: "checkbox", boxLabel: "Использовать в И-М?", name: "ImportToIM", itemId: "ImportToIM", width: 5, id: "ImportToIM" + this.UO_id, inputValue: true, UO_Numb: 1, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },



                {
                    xtype: 'viewComboBox',
                    fieldLabel: "НДС в процентах для ККМ (пока что не используется)", emptyText: "...", allowBlank: false, flex: 1, //disabled: true,

                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                    store: new Ext.data.SimpleStore({
                        fields: ['KKMSTax', 'KKMSTaxName'],
                        data: SettingsKKMSTax_values
                    }),

                    valueField: 'KKMSTax',
                    hiddenName: 'KKMSTax',
                    displayField: 'KKMSTaxName',
                    name: 'KKMSTax', itemId: "KKMSTax", id: "KKMSTax" + this.UO_id,
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //disabled: true
                    //hidden: true
                },


            ],

        });


        var DescriptionFullPanel = Ext.create('Ext.panel.Panel', {
            id: "descriptionFullPanel_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: "Витрина: описание",
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',

            defaults: { anchor: '100%' },
            //width: "100%", height: 115 + varBodyPadding,
            autoScroll: true,
            //split: true,

            items: [


                { xtype: 'textfield', fieldLabel: "URL (a-z, 0-9, -)", name: "DirNomenNameURL", id: "DirNomenNameURL" + this.UO_id, flex: 1, allowBlank: true, regex: /^[a-zA-Z--_-0-9]+$/ },


                { xtype: 'label', text: lanNameShort },

                {
                    xtype: "textarea",
                    width: "100%", //height: "100%",
                    flex: 1,
                    name: "Description",
                    id: "Description" + this.UO_id,
                    hideLabel: false, labelSeparator: '111', //anchor: "100%" //, autoHeight: true
                },

                { xtype: 'label', text: lanNameFull },

                {
                    xtype: "htmleditor",
                    width: "100%", //height: "100%",
                    flex: 1,
                    name: "DescriptionFull",
                    id: "DescriptionFull" + this.UO_id,
                    hideLabel: false, labelSeparator: '111', //anchor: "100%" //, autoHeight: true
                },

            ],

        });


        //Фото товара
        var ImageShow = Ext.create('Ext.Img', {
            id: 'imageShow' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            style: {
                'display': 'block', 'margin': 'auto', //'width': '50%', 'height': '50%'
            },
            listeners: { el: { click: 'onImageShowClick' } },
        });
        var Image1Show = Ext.create('Ext.Img', {
            id: 'image1Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            style: {
                'display': 'block', 'margin': 'auto', //'width': '50%', 'height': '50%'
            },
            listeners: { el: { click: 'onImage1ShowClick' } },
        });
        var Image2Show = Ext.create('Ext.Img', {
            id: 'image2Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            style: {
                'display': 'block', 'margin': 'auto', //'width': '50%', 'height': '50%'
            },
            listeners: { el: { click: 'onImage2ShowClick' } },
        });
        var Image3Show = Ext.create('Ext.Img', {
            id: 'image3Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            style: {
                'display': 'block', 'margin': 'auto', //'width': '50%', 'height': '50%'
            },
            listeners: { el: { click: 'onImage3ShowClick' } },
        });
        var Image4Show = Ext.create('Ext.Img', {
            id: 'image4Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            style: {
                'display': 'block', 'margin': 'auto', //'width': '50%', 'height': '50%'
            },
            listeners: { el: { click: 'onImage4ShowClick' } },
        });
        var Image5Show = Ext.create('Ext.Img', {
            id: 'image5Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            style: {
                'display': 'block', 'margin': 'auto', //'width': '50%', 'height': '50%'
            },
            listeners: { el: { click: 'onImage5ShowClick' } },
        });

        var ImgPanel = Ext.create('Ext.panel.Panel', {
            id: "imgPanel_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: "Витрина: фото",
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',

            //defaults: { anchor: '100%' },
            //width: "100%", height: 115 + varBodyPadding,
            autoScroll: true,
            //split: true,

            items: [

                { xtype: 'label', text: "Фото №0" },
                { xtype: 'textfield', fieldLabel: "GenID", name: "SysGenID", id: "SysGenID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGenIDPatch", id: "SysGenIDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                ImageShow,

                { xtype: 'label', text: "Фото №1" },
                { xtype: 'textfield', fieldLabel: "Gen1ID", name: "SysGen1ID", id: "SysGen1ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen1IDPatch", id: "SysGen1IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                Image1Show,

                { xtype: 'label', text: "Фото №2" },
                { xtype: 'textfield', fieldLabel: "Gen2ID", name: "SysGen2ID", id: "SysGen2ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen2IDPatch", id: "SysGen2IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                Image2Show,

                { xtype: 'label', text: "Фото №3" },
                { xtype: 'textfield', fieldLabel: "Gen3ID", name: "SysGen3ID", id: "SysGen3ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen3IDPatch", id: "SysGen3IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                Image3Show,

                { xtype: 'label', text: "Фото №4" },
                { xtype: 'textfield', fieldLabel: "Gen4ID", name: "SysGen4ID", id: "SysGen4ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen4IDPatch", id: "SysGen4IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                Image4Show,

                { xtype: 'label', text: "Фото №5" },
                { xtype: 'textfield', fieldLabel: "Gen5ID", name: "SysGen5ID", id: "SysGen5ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen5IDPatch", id: "SysGen5IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                Image5Show,

            ],

        });


        var tabPanel = Ext.create('Ext.tab.Panel', {
            id: "tab_1_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            region: "center",
            bodyStyle: 'background:transparent;',
            //width: "100%", height: "100%",
            autoHeight: true,
            split: true,

            items: [
                GeneralPanel, DescriptionFullPanel, ImgPanel //DescriptionPanel, 
            ]

        });


        //body
        this.items = [
            
            tabPanel,

        ],


        this.buttons = [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                    text: lanSave, icon: '../Scripts/sklad/images/save.png'
                },
                " ",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                    text: lanCancel, icon: '../Scripts/sklad/images/cancel.png'
                },

                "-",

                {
                    id: "btnHistory" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHistory",
                    text: lanHistory, icon: '../Scripts/sklad/images/history.png',
                    disabled: true
                },

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                    text: lanHelp, icon: '../Scripts/sklad/images/help16.png'
                },
        ],



        this.callParent(arguments);
    }

});