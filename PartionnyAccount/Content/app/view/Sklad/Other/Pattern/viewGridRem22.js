//ComboBox для выбора справочника, в которых небольшое к-во записей и отсутствуют группы //Используется в Справочник.Контрагент
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewGridRem22", {
    extend: "Ext.grid.Panel",
    alias: "widget.viewGridRem22",

    region: "center",
    loadMask: true,
    autoScroll: true,
    touchScroll: true,
    itemId: "grid",
    UI_DateFormatStr: false,

    conf: {},

    initComponent: function () {
        this.id = this.conf.id;
        this.UO_id = this.conf.UO_id;
        this.UO_idMain = this.conf.UO_idMain;
        this.UO_idCall = this.conf.UO_idCall;
        this.UO_View = this.conf.UO_View;

        this.columns = [
            //Партия
            { text: "Партия", dataIndex: "Rem2PartyID", width: 85, hidden: true, tdCls: 'x-change-cell', hidden: true }, //, hidden: true
            { text: "Документ", dataIndex: "DocSecondHandPurchID", width: 85, style: "height: 25px;", tdCls: 'x-change-cell' },
            //Товар
            { text: "Товар №", dataIndex: "DirServiceNomenID", width: 85, style: "height: 25px;", tdCls: 'x-change-cell', hidden: true },
            { text: "Товар Найм.", dataIndex: "DirServiceNomenName", flex: 1, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Дата док", dataIndex: "DocDate", width: 75, style: "height: 25px;", tdCls: 'x-change-cell' },
            //{ text: "Дата приём", dataIndex: "DocDatePurches", width: 75, style: "height: 25px;", tdCls: 'x-change-cell' },

            //{ text: "Поставщик", dataIndex: "DirContractorName", flex: 1, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Склад", dataIndex: "DirWarehouseName", flex: 1, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            //{ text: "Документ", dataIndex: "ListDocNameRu", flex: 1, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },

            //{ text: "Налог", dataIndex: "DirVatValue", width: 85, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            //{ text: "Поступило", dataIndex: "Quantity", width: 85, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            //{ text: "Остаток", dataIndex: "Remnant", width: 85, style: "height: 25px;", tdCls: 'x-change-cell' },

            //{ text: "Цена в вал.", dataIndex: "PriceVAT", width: 85, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Курс", dataIndex: "DirCurrencyRate", width: 85, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Кратность", dataIndex: "DirCurrencyMultiplicity", width: 85, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Себестои.", dataIndex: "PriceCurrency", width: 85, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },

            { text: "Цена Роз.(в вал.)", dataIndex: "PriceRetailVAT", width: 85, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Цена Роз.", dataIndex: "PriceRetailCurrency", width: 85, style: "height: 25px;", tdCls: 'x-change-cell' }, //, hidden: true

            //Характеристики
            /*
            { text: "Характеристики", dataIndex: "DirChar", flex: 1, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Цвет", dataIndex: "DirCharColourName", width: 100, style: "height: 25px;", tdCls: 'x-change-cell' },
            //{ text: "Производитель", dataIndex: "DirCharMaterialName", width: 100, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Имя", dataIndex: "DirCharNameName", width: 100, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Сезон", dataIndex: "DirCharSeasonName", width: 100, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Пол", dataIndex: "DirCharSexName", width: 100, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Размер", dataIndex: "DirCharSizeName", width: 100, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            //{ text: "Поставщик", dataIndex: "DirCharStyleName", width: 100, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Текстура", dataIndex: "DirCharTextureName", width: 100, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            */
            //{ text: "Серийный", dataIndex: "SerialNumber", width: 100, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            //{ text: "Штрих-Код", dataIndex: "Barcode", width: 100, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },

            //{ text: "Сотрудник", dataIndex: "DirEmployeeName", width: 75, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },

            //{ text: "№ причины", dataIndex: "DirDescriptionID", width: 100, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Тип возвр.", dataIndex: "DirReturnTypeName", width: 85, style: "height: 25px;", tdCls: 'x-change-cell' },
            { text: "Причина", dataIndex: "DirDescriptionName", width: 85, style: "height: 25px;", tdCls: 'x-change-cell' },

            { text: "Причина", dataIndex: "DirDescriptionName", width: 85, style: "height: 25px;", tdCls: 'x-change-cell' },
        ],


        this.callParent(arguments);
    },



    //Формат даты
    viewConfig: {
        getRowClass: function (record, index) {
            
            // 1. === Исправляем формат даты: "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"  ===  ===  ===  ===  === 
            for (var i = 0; i < record.store.model.fields.length; i++) {
                //Если поле типа "Дата"
                if (record.store.model.fields[i].type == "date") {
                    //Если есть дата, может быть пустое значение
                    if (record.data[record.store.model.fields[i].name] != null) {
                        
                        if (record.data[record.store.model.fields[i].name].length != 10) {
                            //Ext.Date.format
                            record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                        }
                    }
                }
            }


            //2.  === Стили ===  ===  ===  ===  === 
            if (parseInt(record.get('DirReturnTypeID')) == 2) { return 'price-held_no_partly_paid'; }
            else if (parseInt(record.get('DirReturnTypeID')) == 3) { return 'price-held_no_unpaid'; }


            // 3.  === Стили ===  ===  ===  ===  === 
            var DocDate1 = new Date(record.get('DocDate'));
            var DocDate2 = new Date();
            DocDate2.setDate(DocDate2.getDate() - 45);
            
            //3.1. Если аппарат просрочен: DocDate <= new Date()
            if (DocDate1 < DocDate2) {
                return 'status-DateDone-1';
            }

            
        }, //getRowClass

        stripeRows: true,

    },

       
    /*
    listeners: {
        viewready: function (grid) {
            var view = grid.view;

            // record the current cellIndex
            grid.mon(view, {
                uievent: function (type, view, cell, recordIndex, cellIndex, e) {
                    grid.cellIndex = cellIndex;
                    grid.recordIndex = recordIndex;
                }
            });

            grid.tip = Ext.create('Ext.tip.ToolTip', {
                target: view.el,
                delegate: '.x-grid-cell',
                trackMouse: true,
                renderTo: Ext.getBody(),
                listeners: {
                    beforeshow: function updateTipBody(tip) {
                        if (view != undefined)
                            if (tip != undefined)
                                if (view.getRecord(tip.triggerElement) != undefined) {

                                    var Img = Ext.create('Ext.Img', {
                                        src: view.getRecord(tip.triggerElement).get('SysGenIDPatch'),
                                        listeners: {
                                            render: function () {
                                                this.mon(this.getEl(), 'load', function (e) {
                                                    tip.update(
                                                        "<img src='" + view.getRecord(tip.triggerElement).get('SysGenIDPatch') + "' />"
                                                    );
                                                });
                                            }
                                        },
                                        renderTo: Ext.getBody()
                                    });

                                }

                    },

                }
            });

        }
    }
    */

});