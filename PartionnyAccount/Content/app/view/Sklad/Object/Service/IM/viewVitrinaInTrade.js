Ext.define("PartionnyAccount.view.Sklad/Object/Service/IM/viewVitrinaInTrade", {
    extend: "Ext.Window", //extend: "Ext.panel.Panel",
    alias: "widget.viewVitrinaInTrade",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 625, height: 425,
    region: "center",
    monitorValid: true,
    //autoScroll: false,
    defaultType: 'textfield',
    title: "Витрина InTrade",

    frame: true,
    border: false,
    resizable: true, maximizable: true,
    //modal: true,
    buttonAlign: 'left',

    timeout: varTimeOutDefault,
    waitMsg: lanLoading,

    UO_maximize: true, //Максимизировать во весь экран
    UO_Center: true,    //true - в центре экрана, false - окна каскадом

    autoHeight: true,

    //Контроллер
    controller: 'viewcontrollerVitrinaInTrade',

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {



        //=== 1 === === ===
        var DirNomenGroup_Top_values = [
            [1, "Категории товара"],
            [2, "Страницы: Главная, Товар, О нас, ..."]
        ];
        var PanelGeneral = Ext.create('Ext.panel.Panel', {
            id: "panelGeneral_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

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

                { xtype: 'textfield', fieldLabel: lanName, name: "DirWebShopUOID", id: "DirWebShopUOID" + this.UO_id, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: lanName, name: "DirWebShopUOName", id: "DirWebShopUOName" + this.UO_id, flex: 1, allowBlank: false },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: "Domain Name", name: "DomainName", id: "DomainName" + this.UO_id, flex: 1, regex: /^[a-zA-Z0-9]+$/, allowBlank: false },
                        { xtype: "label", text: ".intradecloud.com" },
                    ]
                },

                {
                    xtype: 'viewComboBox',
                    fieldLabel: "Интерфейс верхнего меню", emptyText: "...", allowBlank: false, flex: 1, //disabled: true,

                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                    store: new Ext.data.SimpleStore({
                        fields: ['DirNomenGroup_Top', 'DirNomenGroup_TopName'],
                        data: DirNomenGroup_Top_values
                    }),

                    valueField: 'DirNomenGroup_Top',
                    hiddenName: 'DirNomenGroup_Top',
                    displayField: 'DirNomenGroup_TopName',
                    name: 'DirNomenGroup_Top', itemId: "DirNomenGroup_Top", id: "DirNomenGroup_Top" + this.UO_id,
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //disabled: true
                    //hidden: true
                },


                { xtype: 'container', height: 20, layout: { align: 'stretch', type: 'hbox' }, },
                { xtype: "label", text: lanDefault + " (для документа 'Заказы покупателя')" },
                { xtype: 'container', height: 10, layout: { align: 'stretch', type: 'hbox' }, },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: "checkbox", boxLabel: txtMinusResidues, flex: 1, name: "Nomen_Remains", itemId: "Nomen_Remains", id: "Nomen_Remains" + this.UO_id, inputValue: true, UO_Numb: 1, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                        { xtype: "checkbox", boxLabel: lanReserve, flex: 1, name: "Orders_Reserve", itemId: "Orders_Reserve", width: 5, id: "Orders_Reserve" + this.UO_id, inputValue: true, UO_Numb: 1, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //DirContractorOrg
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Организация", flex: 1, allowBlank: false, //, emptyText: "..."
                            
                            store: this.storeDirContractorsOrgGrid, // store getting items from server
                            valueField: 'DirContractorID',
                            hiddenName: 'DirContractorID',
                            displayField: 'DirContractorName',
                            name: 'Orders_DirContractorIDOrg', itemId: "Orders_DirContractorIDOrg", id: "Orders_DirContractorIDOrg" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //disabled: true
                            //editable: false, typeAhead: false, minChars: 200,
                        },

                        //DirContractor
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanContractor, flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirContractorsGrid, // store getting items from server
                            valueField: 'DirContractorID',
                            hiddenName: 'DirContractorID',
                            displayField: 'DirContractorName',
                            name: 'Orders_DirContractorID', itemId: "Orders_DirContractorID", id: "Orders_DirContractorID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //Orders_Doc_DirOrdersStateID
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanState + " (" + lanDocument + ")", flex: 1, allowBlank: false, //, emptyText: "..."

                            store: this.storeDirOrdersStatesGrid_Orders_Doc, // store getting items from server
                            valueField: 'DirOrdersStateID',
                            hiddenName: 'DirOrdersStateID',
                            displayField: 'DirOrdersStateName',
                            name: 'Orders_Doc_DirOrdersStateID', itemId: "Orders_Doc_DirOrdersStateID", id: "Orders_Doc_DirOrdersStateID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //disabled: true
                            //editable: false, typeAhead: false, minChars: 200,
                        },

                        //Orders_Nomen_DirOrdersStateID
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanState + " (" + lanNomenclature + ")", flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirOrdersStatesGrid_Orders_Nomen, // store getting items from server
                            valueField: 'DirOrdersStateID',
                            hiddenName: 'DirOrdersStateID',
                            displayField: 'DirOrdersStateName',
                            name: 'Orders_Nomen_DirOrdersStateID', itemId: "Orders_Nomen_DirOrdersStateID", id: "Orders_Nomen_DirOrdersStateID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //DirPriceTypes
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Тип цены", flex: 1, allowBlank: false,

                            store: this.storeDirPriceTypesGrid, // store getting items from server
                            valueField: 'DirPriceTypeID',
                            hiddenName: 'DirPriceTypeID',
                            displayField: 'DirPriceTypeName',
                            name: 'Nomen_DirPriceTypeID', itemId: "Nomen_DirPriceTypeID", id: "Nomen_DirPriceTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            //editable: true, typeAhead: true, minChars: 2
                            //readOnly: true
                        },

                        //DirWarehouseID
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanWarehouse, flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirWarehousesGrid, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'Orders_DirWarehouseID', itemId: "Orders_DirWarehouseID", id: "Orders_DirWarehouseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                    ]
                },

                { xtype: 'container', height: 5 },

                //DirCurrencyID
                {
                    xtype: 'viewComboBox',
                    fieldLabel: lanCurrency, flex: 1, allowBlank: false,

                    store: this.storeDirCurrenciesGrid, // store getting items from server
                    valueField: 'DirCurrencyID',
                    hiddenName: 'DirCurrencyID',
                    displayField: 'DirCurrencyName',
                    name: 'DirCurrencyID', itemId: "DirCurrencyID", id: "DirCurrencyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    //editable: true, typeAhead: true, minChars: 2
                },

            ],

        });



        //=== 2 === === ===
        var Image1Show = Ext.create('Ext.Img', {
            id: 'image1Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage1ShowClick' } },
            hidden: true,

            /*style: {
                'width': '50px', 'height': '150px', 'margin': 'auto' //'display': 'block', 'margin': 'auto', //'width': '50%', 'height': '50%'
            },*/

        });
        var Image2Show = Ext.create('Ext.Img', {
            id: 'image2Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage2ShowClick' } },
            hidden: true,
        });
        var Image3Show = Ext.create('Ext.Img', {
            id: 'image3Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage3ShowClick' } },
            hidden: true,
        });
        var Image4Show = Ext.create('Ext.Img', {
            id: 'image4Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage4ShowClick' } },
            hidden: true,
        });

        var PanelSlider = Ext.create('Ext.panel.Panel', {
            id: "panelSlider_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: "Слайдер",
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

                { xtype: "label", text: "Слайдер: укажите 2-4 (желательно акционных) товара в верху на главной странице, а также к каждому товару укажите изображение формата '1140x380'" },
                {
                    xtype: 'numberfield',
                    value: 0, maxValue: 4, minValue: 0,
                    allowBlank: false, flex: 1, fieldLabel: "К-во товара в Слайдере",
                    name: 'Slider_Quantity', itemId: 'Slider_Quantity', id: 'Slider_Quantity' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

                    listeners: {
                        change: 'onSlider_QuantityChange'
                    },
                },
                


                //Товар №1 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №1", id: "Slider_label1ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №1", emptyText: "...", 
                    name: 'Slider_DirNomen1ID', itemId: "Slider_DirNomen1ID", id: "Slider_DirNomen1ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },

                { xtype: 'textfield', fieldLabel: "Gen1ID", name: "SysGen1ID", id: "SysGen1ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen1IDPatch", id: "SysGen1IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },

                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' },  autoScroll: true,
                    items: [
                        Image1Show,
                    ]
                },
                


                //Товар №2 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №2", id: "Slider_label2ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №1", emptyText: "...", 
                    name: 'Slider_DirNomen2ID', itemId: "Slider_DirNomen2ID", id: "Slider_DirNomen2ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },

                { xtype: 'textfield', fieldLabel: "Gen2ID", name: "SysGen2ID", id: "SysGen2ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen2IDPatch", id: "SysGen2IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image2Show,
                    ]
                },



                //Товар №3 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №3", id: "Slider_label3ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №1", emptyText: "...", 
                    name: 'Slider_DirNomen3ID', itemId: "Slider_DirNomen3ID", id: "Slider_DirNomen3ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },

                { xtype: 'textfield', fieldLabel: "Gen3ID", name: "SysGen3ID", id: "SysGen3ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen3IDPatch", id: "SysGen3IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image3Show,
                    ]
                },



                //Товар №4 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №4", id: "Slider_label4ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №1", emptyText: "...", 
                    name: 'Slider_DirNomen4ID', itemId: "Slider_DirNomen4ID", id: "Slider_DirNomen4ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },

                { xtype: 'textfield', fieldLabel: "Gen4ID", name: "SysGen4ID", id: "SysGen4ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen4IDPatch", id: "SysGen4IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image4Show,
                    ]
                },
                

            ],

        });



        //=== 3 === === ===
        /*
        //4 Товара
        var Image5Show = Ext.create('Ext.Img', {
            id: 'image5Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage5ShowClick' } },
            hidden: true,
        });
        var Image6Show = Ext.create('Ext.Img', {
            id: 'image6Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage6ShowClick' } },
            hidden: true,
        });
        var Image7Show = Ext.create('Ext.Img', {
            id: 'image7Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage7ShowClick' } },
            hidden: true,
        });
        var Image8Show = Ext.create('Ext.Img', {
            id: 'image8Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage8ShowClick' } },
            hidden: true,
        });
        //8 Товара
        var Image9Show = Ext.create('Ext.Img', {
            id: 'image9Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage9ShowClick' } },
            hidden: true,
        });
        var Image10Show = Ext.create('Ext.Img', {
            id: 'image10Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage10ShowClick' } },
            hidden: true,
        });
        var Image11Show = Ext.create('Ext.Img', {
            id: 'image11Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage11ShowClick' } },
            hidden: true,
        });
        var Image12Show = Ext.create('Ext.Img', {
            id: 'image12Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage12ShowClick' } },
            hidden: true,
        });
        //12 Товара
        var Image13Show = Ext.create('Ext.Img', {
            id: 'image13Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage13ShowClick' } },
            hidden: true,
        });
        var Image14Show= Ext.create('Ext.Img', {
            id: 'image14Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage14ShowClick' } },
            hidden: true,
        });
        var Image15Show = Ext.create('Ext.Img', {
            id: 'image15Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage15ShowClick' } },
            hidden: true,
        });
        var Image16Show = Ext.create('Ext.Img', {
            id: 'image16Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage16ShowClick' } },
            hidden: true,
        });
        //16 Товара
        var Image17Show = Ext.create('Ext.Img', {
            id: 'image17Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage17ShowClick' } },
            hidden: true,
        });
        var Image18Show = Ext.create('Ext.Img', {
            id: 'image18Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage18ShowClick' } },
            hidden: true,
        });
        var Image19Show = Ext.create('Ext.Img', {
            id: 'image19Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage19ShowClick' } },
            hidden: true,
        });
        var Image20Show = Ext.create('Ext.Img', {
            id: 'image20Show' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg", //width: "20%", height: "20%",
            listeners: { el: { click: 'onImage20ShowClick' } },
            hidden: true,
        });
        */
        var PanelRecommended = Ext.create('Ext.panel.Panel', {
            id: "panelRecommended_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: "Рекомендованные",
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

                { xtype: "label", text: "Рекомендованный товар на главной странице, после Слайдера. Укажите товар, желательно с изображениями." },
                {
                    xtype: 'numberfield',
                    value: 0, maxValue: 16, minValue: 0, step: 4,
                    allowBlank: false, flex: 1, fieldLabel: "К-во товара в Рекомендованных",
                    name: 'Recommended_Quantity', itemId: 'Recommended_Quantity', id: 'Recommended_Quantity' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

                    listeners: {
                        change: 'onRecommended_QuantityChange'
                    },
                },




                //4 Товара === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === ===

                //Товар №1 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №1", id: "Recommended_label1ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №1", emptyText: "...", 
                    name: 'Recommended_DirNomen1ID', itemId: "Recommended_DirNomen1ID", id: "Recommended_DirNomen1ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen5ID", name: "SysGen5ID", id: "SysGen5ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen5IDPatch", id: "SysGen5IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image5Show,
                    ]
                },*/

                //Товар №2 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №2", id: "Recommended_label2ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №1", emptyText: "...", 
                    name: 'Recommended_DirNomen2ID', itemId: "Recommended_DirNomen2ID", id: "Recommended_DirNomen2ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen6ID", name: "SysGen6ID", id: "SysGen6ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen6IDPatch", id: "SysGen6IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image6Show
                    ]
                },*/

                //Товар №3 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №3", id: "Recommended_label3ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №1", emptyText: "...", 
                    name: 'Recommended_DirNomen3ID', itemId: "Recommended_DirNomen3ID", id: "Recommended_DirNomen3ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen7ID", name: "SysGen7ID", id: "SysGen7ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen7IDPatch", id: "SysGen7IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image7Show
                    ]
                },*/

                //Товар №4 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №4", id: "Recommended_label4ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №1", emptyText: "...", 
                    name: 'Recommended_DirNomen4ID', itemId: "Recommended_DirNomen4ID", id: "Recommended_DirNomen4ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen8ID", name: "SysGen8ID", id: "SysGen8ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen8IDPatch", id: "SysGen8IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image8Show
                    ]
                },*/




                //8 Товаров === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === ===

                //Товар №5 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №5", id: "Recommended_label5ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №5", emptyText: "...", 
                    name: 'Recommended_DirNomen5ID', itemId: "Recommended_DirNomen5ID", id: "Recommended_DirNomen5ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen9ID", name: "SysGen9ID", id: "SysGen9ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen9IDPatch", id: "SysGen9IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image9Show
                    ]
                },*/

                //Товар №6 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №6", id: "Recommended_label6ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №1", emptyText: "...", 
                    name: 'Recommended_DirNomen6ID', itemId: "Recommended_DirNomen6ID", id: "Recommended_DirNomen6ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen10ID", name: "SysGen10ID", id: "SysGen10ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen10IDPatch", id: "SysGen10IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image10Show
                    ]
                },*/

                //Товар №7 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №7", id: "Recommended_label7ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №7", emptyText: "...", 
                    name: 'Recommended_DirNomen7ID', itemId: "Recommended_DirNomen7ID", id: "Recommended_DirNomen7ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen11ID", name: "SysGen11ID", id: "SysGen11ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen11IDPatch", id: "SysGen11IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image11Show
                    ]
                },*/

                //Товар №8 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №8", id: "Recommended_label8ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №8", emptyText: "...", 
                    name: 'Recommended_DirNomen8ID', itemId: "Recommended_DirNomen8ID", id: "Recommended_DirNomen8ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen12ID", name: "SysGen12ID", id: "SysGen12ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen12IDPatch", id: "SysGen12IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image12Show
                    ]
                },*/




                //12 Товаров === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === ===

                //Товар №9 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №9", id: "Recommended_label9ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №9", emptyText: "...", 
                    name: 'Recommended_DirNomen9ID', itemId: "Recommended_DirNomen9ID", id: "Recommended_DirNomen9ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen13ID", name: "SysGen13ID", id: "SysGen13ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen13IDPatch", id: "SysGen13IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image13Show
                    ]
                },*/

                //Товар №10 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №10", id: "Recommended_label10ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №10", emptyText: "...", 
                    name: 'Recommended_DirNomen10ID', itemId: "Recommended_DirNomen10ID", id: "Recommended_DirNomen10ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen10ID", name: "SysGen14ID", id: "SysGen14ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen14IDPatch", id: "SysGen14IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image14Show
                    ]
                },*/

                //Товар №11 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №11", id: "Recommended_label11ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №11", emptyText: "...", 
                    name: 'Recommended_DirNomen11ID', itemId: "Recommended_DirNomen11ID", id: "Recommended_DirNomen11ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen15ID", name: "SysGen15ID", id: "SysGen15ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen15IDPatch", id: "SysGen15IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image15Show
                    ]
                },*/

                //Товар №12 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №12", id: "Recommended_label12ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №12", emptyText: "...", 
                    name: 'Recommended_DirNomen12ID', itemId: "Recommended_DirNomen12ID", id: "Recommended_DirNomen12ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen16ID", name: "SysGen16ID", id: "SysGen16ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen16IDPatch", id: "SysGen16IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image16Show
                    ]
                },*/




                //16 Товаров === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === === ===

                //Товар №13 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №13", id: "Recommended_label13ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №13", emptyText: "...", 
                    name: 'Recommended_DirNomen13ID', itemId: "Recommended_DirNomen13ID", id: "Recommended_DirNomen13ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen17ID", name: "SysGen17ID", id: "SysGen17ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen17IDPatch", id: "SysGen17IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image17Show
                    ]
                },*/

                //Товар №14 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №14", id: "Recommended_label14ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №14", emptyText: "...", 
                    name: 'Recommended_DirNomen14ID', itemId: "Recommended_DirNomen14ID", id: "Recommended_DirNomen14ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen18ID", name: "SysGen18ID", id: "SysGen18ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen18IDPatch", id: "SysGen18IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image18Show
                    ]
                },*/

                //Товар №15 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №15", id: "Recommended_label15ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №15", emptyText: "...", 
                    name: 'Recommended_DirNomen15ID', itemId: "Recommended_DirNomen15ID", id: "Recommended_DirNomen15ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen19ID", name: "SysGen19ID", id: "SysGen19ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen19IDPatch", id: "SysGen19IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image19Show
                    ]
                },*/

                //Товар №16 === === === === === === === === === === === === === === === === ===
                { xtype: "label", text: "Товар №16", id: "Recommended_label16ID" + this.UO_id, hidden: true },
                {
                    xtype: 'viewComboBoxTree',

                    store: this.storeNomenTree, // store getting items from server
                    selectChildren: false,
                    canSelectFolders: false,

                    flex: 1, //allowBlank: false, //fieldLabel: "Товар №16", emptyText: "...", 
                    name: 'Recommended_DirNomen16ID', itemId: "Recommended_DirNomen16ID", id: "Recommended_DirNomen16ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, //typeAhead: false, minChars: 200,
                    hidden: true,
                },
                /*{ xtype: 'textfield', fieldLabel: "Gen20ID", name: "SysGen20ID", id: "SysGen20ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Image path", name: "SysGen20IDPatch", id: "SysGen20IDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, flex: 1, allowBlank: true, hidden: true, },
                {
                    xtype: 'container',
                    layout: { type: 'hbox', align: 'center', pack: 'center' }, autoScroll: true,
                    items: [
                        Image20Show
                    ]
                },*/




            ],

        });



        //=== 4 HTML === === ===
        var PanelPayment = new Ext.FormPanel({
            title: "Оплата",
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;', frame: true, monitorValid: true, layout: 'fit', autoScroll: true,
            defaultType: 'textfield', defaults: { anchor: '100%' },
            items: [
                {
                    xtype: "htmleditor",
                    name: "Payment", region: 'center', hideLabel: true, labelSeparator: '',
                },
            ]
        });

        //=== 5 HTML === === ===
        var PanelAboutUs = new Ext.FormPanel({
            title: "О нас",
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;', frame: true, monitorValid: true, layout: 'fit', autoScroll: true,
            defaultType: 'textfield', defaults: { anchor: '100%' },
            items: [
                {
                    xtype: "htmleditor",
                    name: "AboutUs", region: 'center', hideLabel: true, labelSeparator: '',
                },
            ]
        });

        //=== 6 HTML === === ===
        var PanelDeliveryInformation = new Ext.FormPanel({
            title: "Доставка",
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;', frame: true, monitorValid: true, layout: 'fit', autoScroll: true,
            defaultType: 'textfield', defaults: { anchor: '100%' },
            items: [
                {
                    xtype: "htmleditor",
                    name: "DeliveryInformation", region: 'center', hideLabel: true, labelSeparator: '',
                },
            ]
        });

        //=== 7 HTML === === ===
        var PanelPrivacyPolicy = new Ext.FormPanel({
            title: "Конфиденциальность",
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;', frame: true, monitorValid: true, layout: 'fit', autoScroll: true,
            defaultType: 'textfield', defaults: { anchor: '100%' },
            items: [
                {
                    xtype: "htmleditor",
                    name: "PrivacyPolicy", region: 'center', hideLabel: true, labelSeparator: '',
                },
            ]
        });

        //=== 8 HTML === === ===
        var PanelTermsConditions = new Ext.FormPanel({
            title: "Сроки & Условия",
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;', frame: true, monitorValid: true, layout: 'fit', autoScroll: true,
            defaultType: 'textfield', defaults: { anchor: '100%' },
            items: [
                {
                    xtype: "htmleditor",
                    name: "TermsConditions", region: 'center', hideLabel: true, labelSeparator: '',
                },
            ]
        });

        //=== 9 HTML === === ===
        var PanelContactUs = new Ext.FormPanel({
            title: "Свяжитесь с нами",
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;', frame: true, monitorValid: true, layout: 'fit', autoScroll: true,
            defaultType: 'textfield', defaults: { anchor: '100%' },
            items: [
                {
                    xtype: "htmleditor",
                    name: "ContactUs", region: 'center', hideLabel: true, labelSeparator: '',
                },
            ]
        });

        //=== 10 HTML === === ===
        var PanelReturns = new Ext.FormPanel({
            title: "Возвраты",
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;', frame: true, monitorValid: true, layout: 'fit', autoScroll: true,
            defaultType: 'textfield', defaults: { anchor: '100%' },
            items: [
                {
                    xtype: "htmleditor",
                    name: "Returns", region: 'center', hideLabel: true, labelSeparator: '',
                },
            ]
        });

        //=== 11 HTML === === ===
        var PanelSiteMap = new Ext.FormPanel({
            title: "Карта сайта",
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;', frame: true, monitorValid: true, layout: 'fit', autoScroll: true,
            defaultType: 'textfield', defaults: { anchor: '100%' },
            items: [
                {
                    xtype: "htmleditor",
                    name: "SiteMap", region: 'center', hideLabel: true, labelSeparator: '',
                },
            ]
        });

        //=== 12 HTML === === ===
        var PanelAffiliate = new Ext.FormPanel({
            title: "Филиалы",
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;', frame: true, monitorValid: true, layout: 'fit', autoScroll: true,
            defaultType: 'textfield', defaults: { anchor: '100%' },
            items: [
                {
                    xtype: "htmleditor",
                    name: "Affiliate", region: 'center', hideLabel: true, labelSeparator: '',
                },
            ]
        });

        //=== 13 HTML === === ===
        var PanelSpecials = new Ext.FormPanel({
            title: "Специальные предложения",
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;', frame: true, monitorValid: true, layout: 'fit', autoScroll: true,
            defaultType: 'textfield', defaults: { anchor: '100%' },
            items: [
                {
                    xtype: "htmleditor",
                    name: "Specials", region: 'center', hideLabel: true, labelSeparator: '',
                },
            ]
        });






        var tabPanel = Ext.create('Ext.tab.Panel', {
            id: "tab_1_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            region: "center",
            bodyStyle: 'background:transparent;',
            width: "100%", height: "100%",
            autoHeight: true,
            split: true,

            items: [
                PanelGeneral,
                PanelSlider, PanelRecommended,
                PanelPayment, PanelAboutUs, PanelDeliveryInformation, PanelPrivacyPolicy, PanelTermsConditions, PanelContactUs, PanelReturns, PanelSiteMap, PanelAffiliate, PanelSpecials, 
            ]

        });



        //Form-Panel
        var formPanel = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            UO_Loaded: this.UO_Loaded,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)


            bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
            region: "center", //!!! Важно для Ресайз-а !!!
            monitorValid: true,
            defaultType: 'textfield',

            //layout: 'border',
            //defaults: { anchor: '100%' },
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start',
                split: true,
            },
            split: true,

            width: "100%", height: "100%",
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,

            items: [
                tabPanel
            ],


            //buttonAlign: 'left',
            buttons: [
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
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                    text: lanHelp, icon: '../Scripts/sklad/images/help16.png'
                },
            ]

        });




        //body
        this.items = [

            formPanel

        ],


        this.callParent(arguments);
    }

});