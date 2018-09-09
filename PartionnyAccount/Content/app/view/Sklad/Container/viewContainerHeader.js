//Верхний Тулбар
Ext.define("PartionnyAccount.view.Sklad/Container/viewContainerHeader", {
    extend: "Ext.Toolbar",
    //style: "background-color: #157fcc;",
    margin: '0 0 0 0', //margin: '0 0 3 0',
    alias: "widget.viewContainerHeader",
    height: 40,
    region: "north",
    frame: true,
    iconCls: 'windowIcon',
    //defaultButtonUI: DefaultButtonUI,

    bodyStyle: 'background:white;', //bodyStyle: 'opacity:0.5;',
    bodyPadding: varBodyPadding,
    

    monitorResize: true,
    listeners: {
        resize: {
            fn: function (el) {
                funResizeBrowser();
            }
        }
    },


    conf: {},

    initComponent: function () {
        //this.id = this.conf.id;
        //this.defaultButtonUI = this.conf.defaultButtonUI;

        this.callParent(arguments);
    },
    
    items: [

        //Настройки
        " ",
        {
            xtype: 'button',
            //text: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanSettings + "</font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanSettings + "</font>",
            icon: '../Scripts/sklad/images/settings.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "RightSysSettings0", hidden: true,
            menu: [
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanMyCompany + "</font>",
                    icon: '../Scripts/sklad/images/company16.png',
                    itemId: "btnMyCompany", id: "RightMyCompany", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanBonus + " (для Продажи)" + "</font>",
                    icon: '../Scripts/sklad/images/payment.png',
                    itemId: "btnBonuses", id: "RightDirBonuses", hidden: true,
                },
                /*{
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanBonus + " (для Работы)" + "</font>",
                    icon: '../Scripts/sklad/images/payment.png',
                    itemId: "btnBonus2es", id: "RightDirBonus2es", hidden: true,
                },*/
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanEmployees + "</font>",
                    icon: '../Scripts/sklad/images/Dir/employees16.png',
                    itemId: "btnEmployees", id: "RightDirEmployees", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanSettings + "</font>",
                    icon: '../Scripts/sklad/images/settings.png',
                    itemId: "btnSettings", id: "RightSysSettings", hidden: true,
                },
                {
                    text: "<font size=2>" + lanExchangeData + "</font>", icon: '../Scripts/sklad/images/dataexchange.png',
                    id: "RightDataExchange", hidden: true,
                    menu: [
                        {
                            text: "<font size=2>Импорт</font>", icon: '../Scripts/sklad/images/dataexchange_imports.png',
                            menu: [
                                {
                                    text: "<font size=2>Приходная накладная (Excel)</font>", icon: '../Scripts/sklad/images/dataexchange_imports.png',
                                    itemId: "viewImportsDocPurchesExcel"
                                },
                            ]
                        },
                    ]
                },
                {
                    text: "<font size=2>" + lanExchangeData + "</font>", icon: '../Scripts/sklad/images/dataexchange.png',
                    id: "RightDataExchange", hidden: true,
                    menu: [
                        {
                            text: "<font size=2>Excel</font>", icon: '../Scripts/sklad/images/excel.png',
                            menu: [
                                {
                                    text: "<font size=2>Импорт: Приходная накладная (Excel)</font>", icon: '../Scripts/sklad/images/dataexchange_imports.png',
                                    itemId: "viewImportsDocPurchesExcel"
                                },
                            ]
                        },
                        {
                            text: "<font size=2>API</font>", icon: '../Scripts/sklad/images/dataexchange_imports.png',
                            menu: [
                                {
                                    text: "<font size=2>API 1.0</font>", icon: '../Scripts/sklad/images/dataexchange_imports.png',
                                    itemId: "viewAPI10"
                                },
                            ]
                        },
                        {
                            text: "<font size=2>Интернет-Магазины</font>", icon: '../Scripts/sklad/images/dataexchange_imports.png',
                            menu: [
                                {
                                    text: "<font size=2>Витрина 'InTrade'</font>", icon: '../Scripts/sklad/images/dataexchange_imports.png',
                                    itemId: "viewVitrinaInTrade"
                                },
                            ]
                        },
                    ]
                },

                //Учетные данные клиента (Не активное)
                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanYourData + "</font>",
                    icon: '../Scripts/sklad/images/nomenclature.png',
                    itemId: "btnYourData", id: "RightYourData", hidden: true,
                    disabled: true
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanDiscPay + "</font>",
                    icon: '../Scripts/sklad/images/payment.png',
                    itemId: "btnDiscPay", id: "RightDiscPay", hidden: true,
                    disabled: true
                },

                //Справка
                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanDispJurnalDetail + "</font>",
                    icon: '../Scripts/sklad/images/disp.png',
                    itemId: "btnJourDisps", id: "RightSysJourDisps", hidden: true,
                    disabled: true
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Справка" + "</font>",
                    icon: '../Scripts/sklad/images/help16.png',
                    itemId: "btnHelp",
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Информация" + "</font>",
                    icon: '../Scripts/sklad/images/info16.png',
                    itemId: "btnInfo",
                },
               
            ]
        },

        //Справочники
        " ",
        {
            xtype: 'button',
            //text: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanDirectories + "</font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanDirectories + "</font>",
            icon: '../Scripts/sklad/images/directory.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "RightDir0", hidden: true,
            menu: [
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanGoods + "</font>", icon: '../Scripts/sklad/images/Dir/goods16.png',
                    itemId: "btnNomens", id: "RightDirNomens", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanCategoriesGoods + "</font>", icon: '../Scripts/sklad/images/text_list_bullets.png',
                    itemId: "btnNomenCategories", id: "RightDirNomenCategories", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanContractors + "</font>", icon: '../Scripts/sklad/images/Dir/contractors16.png',
                    itemId: "btnContractors", id: "RightDirContractors", hidden: true,
                },
                {
                    //text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanWarehouses + " (" + lanPointSales + ")" + "</font>", icon: '../Scripts/sklad/images/Dir/warehouses16.png',
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Точки продаж" + "</font>", icon: '../Scripts/sklad/images/Dir/warehouses16.png',
                    itemId: "btnWarehouses", id: "RightDirWarehouses", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanBanks + "</font>", icon: '../Scripts/sklad/images/bank.png',
                    itemId: "btnBanks", id: "RightDirBanks", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanCashOffices + "</font>", icon: '../Scripts/sklad/images/Dir/cashoffices16.png',
                    itemId: "btnCashOffices", id: "RightDirCashOffices", hidden: true,
                },

                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanCurrencys + "</font>", icon: '../Scripts/sklad/images/Dir/currency16.png',
                    itemId: "btnCurrencies", id: "RightDirCurrencies", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanVats + "</font>", icon: '../Scripts/sklad/images/Dir/vat16.png',
                    itemId: "btnVats", id: "RightDirVats", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanDiscounts + " (для Контраента)" + "</font>", icon: '../Scripts/sklad/images/Dir/discount16.png',
                    itemId: "btnDiscounts", id: "RightDirDiscounts", hidden: true,
                },

                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Статьи выплат" + "</font>", icon: '../Scripts/sklad/images/Dir/discount16.png',
                    itemId: "btnDirDomesticExpenses", id: "RightDirDomesticExpenses", hidden: true,
                },

                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Комментарии к скидкам" + "</font>", icon: '../Scripts/sklad/images/Dir/discount16.png',
                    itemId: "btnDirDescriptionDiscounts", id: "RightDirDescriptionDiscounts", //hidden: true,
                },


                {
                    text: "<font size=2>" + lanCharacteristics + "</font>", icon: '../Scripts/sklad/images/chart.png',
                    //Цвет, Текстура, Размер, Имя товара
                    //Тип, Производитель, Вес, Пол (Муж/Жен)
                    menu: [
                        {
                            text: "<font size=2>" + lanCharacteristicColor + "</font>", icon: '../Scripts/sklad/images/color.png',
                            itemId: "btnCharColours", id: "RightDirCharColours", hidden: true,
                        },
                        {
                            text: "<font size=2>" + "Характеристика: Производитель" + "</font>", icon: '../Scripts/sklad/images/Dir/material16.png',
                            itemId: "btnCharMaterials", id: "RightDirCharMaterials", hidden: true,
                        },
                        {
                            text: "<font size=2>" + lanCharacteristicProductName + "</font>", icon: '../Scripts/sklad/images/names.png',
                            itemId: "btnCharNames", id: "RightDirCharNames", hidden: true,
                        },
                        {
                            text: "<font size=2>" + "Характеристика: Сезон" + "</font>", icon: '../Scripts/sklad/images/Dir/season16.png',
                            itemId: "btnCharSeasons", id: "RightDirCharSeasons", hidden: true,
                        },
                        {
                            text: "<font size=2>" + "Характеристика: Пол" + "</font>", icon: '../Scripts/sklad/images/Dir/sex16.png',
                            itemId: "btnCharSexes", id: "RightDirCharSexes", hidden: true,
                        },
                        {
                            text: "<font size=2>" + lanCharacteristicSize + "</font>", icon: '../Scripts/sklad/images/size.png',
                            itemId: "btnCharSizes", id: "RightDirCharSizes", hidden: true,
                        },
                        {
                            text: "<font size=2>" + "Характеристика: Поставщик" + "</font>", icon: '../Scripts/sklad/images/Dir/style16.png',
                            itemId: "btnCharStyles", id: "RightDirCharStyles", hidden: true,
                        },
                        {
                            text: "<font size=2>" + lanCharacteristicTexture + "</font>", icon: '../Scripts/sklad/images/texture.png',
                            itemId: "btnCharTextures", id: "RightDirCharTextures", hidden: true,
                        },

                    ]
                },

                /*
                "-",
                {
                    text: "<font size=2><b>Сервисный центр</b></font>"
                },
                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Устройства" + "</font>", icon: '../Scripts/sklad/images/Dir/goods16.png',
                    itemId: "btnServiceNomens",
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Категории устройств" + "</font>", icon: '../Scripts/sklad/images/text_list_bullets.png',
                    itemId: "btnServiceNomenCategories",
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Клиенты" + "</font>", icon: '../Scripts/sklad/images/Dir/contractors16.png',
                    itemId: "btnServiceContractors",
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Выполненные работы" + "</font>", icon: '../Scripts/sklad/images/repairs16.png',
                    itemId: "btnServiceJobNomens",
                },
                */
                /*{
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Категории выполненной работы" + "</font>", icon: '../Scripts/sklad/images/text_list_bullets.png',
                    itemId: "btnServiceJobNomenCategories",
                },*/
            ]
        },

        //Торговля
        " ",
        {
            xtype: 'button',
            //text: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanTrade + "</font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanTrade + "</font>",
            icon: '../Scripts/sklad/images/journal.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "RightDoc0", hidden: true,
            menu: [
                //1.
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Поступления" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnPurches", id: "RightDocPurches", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Возврат поставщику" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnReturnVendors", id: "RightDocReturnVendors", hidden: true,
                },
                //2.
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Перемещение" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnMovements", id: "RightDocMovements", hidden: true,
                },
                //3.
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Продажа" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnSales", id: "RightDocSales", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Возврат от покупателя" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnReturnsCustomers", id: "RightDocReturnsCustomers", hidden: true,
                },
                //4.
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Акт выполненных работ" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnActOnWorks", id: "RightDocActOnWorks", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Счет" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnAccounts", id: "RightDocAccounts", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Списание" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnActWriteOffs", id: "RightDocActWriteOffs", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Инвентаризация" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnInventories", id: "RightDocInventories", hidden: true,
                },
                //Переоценка товара
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Переоценка" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnDocReassessments", id: "RightDocReassessments", hidden: true,
                },
                //5. Розница
                /*{
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Розница" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnDocRetailsEdit", id: "RightDocRetails", hidden: true,
                },*/
                /*
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Розничный Возврат" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnDocRetailReturnsEdit", id: "RightDocRetailReturns", hidden: true,
                },
                */
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Переоценка" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnDocNomenRevaluations", id: "RightDocNomenRevaluations", hidden: true,
                },

                //Отчеты
                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Отчет по Торговле" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnReportTotalTrade", id: "RightReportTotalTrade", //hidden: true,
                },
                /*{
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Заказ товара на перемещение" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnReportMovementNomen", id: "RightReportMovementNomen", //hidden: true,
                },*/
            ]
        },

        //Витрина
        " ",
        {
            xtype: 'button',
            text: "<font size=" + HeaderMenu_FontSize_1 + "></font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">Витрина</font>",
            icon: '../Scripts/sklad/images/shop_16.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "RightVitrina0", hidden: true,
            menu: [
                //5. Розница
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Розница" + "</font>", icon: '../Scripts/sklad/images/shop_16.png',
                    itemId: "btnDocRetailsEdit", id: "RightDocRetails", hidden: true,
                },
            ]
        },

        //Заказы
        " ",
        {
            xtype: 'button',
            text: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanOrders + "</font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanOrders + "</font>",
            icon: '../Scripts/sklad/images/Dir/bankaccount16.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "RightDocOrderInt0", hidden: true,
            menu: [
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Предзаказ" + "</font>", icon: '../Scripts/sklad/images/Dir/bankaccount16.png',
                    itemId: "btnOrderIntsNew", id: "RightDocOrderIntsNew", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Список заказов" + "</font>", icon: '../Scripts/sklad/images/Dir/bankaccount16.png',
                    itemId: "btnOrderInts", id: "RightDocOrderInts", hidden: true,
                },

                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Отчеты" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnOrderIntsReport", id: "RightDocOrderIntsReport", hidden: true,
                },

                "-",
                {
                    text: "<font size=2><b>" + lanDirectories + "</b></font>"
                },
                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Статусы заказов" + "</font>", icon: '../Scripts/sklad/images/directory.png',
                    itemId: "btnDirOrdersStates", id: "RightDirOrdersStates", //hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Шаблоны SMS" + "</font>", icon: '../Scripts/sklad/images/sms16.png',
                    itemId: "btnDocOrderIntsSmsTemplates", id: "RightDocOrderIntsSmsTemplates", //hidden: true,
                },
            ]
        },

        //Сервис
        " ",
        {
            xtype: 'button',
            text: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanService + "</font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanService + "</font>",
            icon: '../Scripts/sklad/images/repairs16.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "RightDocService0", hidden: true,
            menu: [

                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Приёмка" + "</font>", icon: '../Scripts/sklad/images/repairs16.png',
                    itemId: "btnServicePurches", id: "RightDocServicePurches", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Мастерская" + "</font>", icon: '../Scripts/sklad/images/repairs16.png',
                    itemId: "btnServiceWorkshops", id: "RightDocServiceWorkshops", hidden: true,
                },

                "-",
                {
                    text: "<font size=2><b>" + lanDocuments + "</b></font>"
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">Перемещение</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnDocServiceMovements", id: "RightDocServiceMovements", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">Инвентаризация</font>", icon: '../Scripts/sklad/images/table_gen.png',
                    itemId: "btnServiceInventoriesEdit", id: "RightDocServiceInventories", //hidden: true,
                },

                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Отчет" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnServicePurchesReport", id: "RightDocServicePurchesReport", hidden: true,
                },

                "-",
                {
                    text: "<font size=2><b>" + lanDirectories + "</b></font>"
                },
                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Устройства" + "</font>", icon: '../Scripts/sklad/images/Dir/goods16.png',
                    itemId: "btnServiceNomens", id: "RightDirServiceNomens", hidden: true,
                },
                /*{
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Категории устройств" + "</font>", icon: '../Scripts/sklad/images/text_list_bullets.png',
                    itemId: "btnServiceNomenCategories", id: "RightDirServiceNomenCategories", hidden: true,
                },*/
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Клиенты" + "</font>", icon: '../Scripts/sklad/images/Dir/contractors16.png',
                    itemId: "btnServiceContractors", id: "RightDirServiceContractors", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Выполненные работы" + "</font>", icon: '../Scripts/sklad/images/repairs16.png',
                    itemId: "btnServiceJobNomens", id: "RightDirServiceJobNomens", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Результаты диагностики" + "</font>", icon: '../Scripts/sklad/images/mobile_phone_exclamation_16.png',
                    itemId: "btnServiceDiagnosticRresults", id: "RightDirServiceDiagnosticRresults", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Типовые неисправности" + "</font>", icon: '../Scripts/sklad/images/activation.png',
                    itemId: "btnServiceNomenTypicalFaults", id: "RightDirServiceNomenTypicalFaults", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Шаблоны SMS" + "</font>", icon: '../Scripts/sklad/images/sms16.png',
                    itemId: "btnSmsTemplates", id: "RightDirSmsTemplates", hidden: true,
                },
            ]
        },

        // Б/У
        " ",
        {
            xtype: 'button',
            text: "<font size=" + HeaderMenu_FontSize_1 + ">" + "Б/У" + "</font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + "Б/У" + "</font>",
            icon: '../Scripts/sklad/images/secondhand16.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "RightDocSecondHands0", hidden: true,
            menu: [

                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Покупка" + "</font>", icon: '../Scripts/sklad/images/secondhand16.png',
                    itemId: "btnSecondHandPurches", id: "RightDocSecondHandPurches", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "ППП" + "</font>", icon: '../Scripts/sklad/images/hand116.png',
                    itemId: "btnSecondHandWorkshops", id: "RightDocSecondHandWorkshops", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Продажа" + "</font>", icon: '../Scripts/sklad/images/secondhand216.png',
                    itemId: "btnSecondHandRetailsEdit", id: "RightDocSecondHandRetails", hidden: true,
                },

                "-",
                {
                    text: "<font size=2><b>" + lanDocuments + "</b></font>"
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">Перемещение</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnSecondHandMovements", id: "RightDocSecondHandMovements", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">Инвентаризация</font>", icon: '../Scripts/sklad/images/table_gen.png',
                    itemId: "btnSecondHandInventoriesEdit", id: "RightDocSecondHandInventories", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">Разборка аппарата</font>", icon: '../Scripts/sklad/images/table_gen.png',
                    itemId: "btnSecondHandRazborsEdit", id: "RightDocSecondHandRazbors", hidden: true,
                },

                "-",
                {
                    text: "<font size=2><b>" + lanDirectories + "</b></font>"
                },
                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Выполненные работы" + "</font>", icon: '../Scripts/sklad/images/repairs16.png',
                    itemId: "btnServiceJobNomens1", id: "RightDirServiceJobNomens1", hidden: true,
                },

            ]
        },

        //Финансы: Касса + Банк
        {
            xtype: 'button',
            text: "<font size=" + HeaderMenu_FontSize_1 + ">" + "Финансы" + "</font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + "Банк и Касса" + "</font>",
            icon: '../Scripts/sklad/images/Dir/cashoffices16.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "RightDocBankCash0", hidden: true,
            menu: [
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Выплаты другие" + "</font>", icon: '../Scripts/sklad/images/Dir/discount16.png',
                    itemId: "btnDocDomesticExpenses", id: "RightDocDomesticExpenses", hidden: true,
                },

                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Банк" + "</font>", icon: '../Scripts/sklad/images/bank.png',
                    itemId: "btnBanksEdit", id: "RightDocBankSums", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Касса" + "</font>", icon: '../Scripts/sklad/images/Dir/cashoffices16.png',
                    itemId: "btnCashOfficesEdit", id: "RightDocCashOfficeSums", hidden: true,
                },

                "-",
                {
                    text: "<font size=2><b>" + lanDocuments + "</b></font>"
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">Перемещение</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnCashOfficeSumMovements", id: "RightDocCashOfficeSumMovements", hidden: true,
                },

                "-",
                {
                    text: "<font size=2><b>" + lanReports + "</b></font>"
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Отчет" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnReportBanksCashOffices", id: "RightReportBanksCashOffices", hidden: true, // - тут не правильный "RightBanksCashOfficesReport", надо бы его переделать на "RightReportBanksCashOffices"
                },
                
            ]
        },

        //Зарплата
        {
            xtype: 'button',
            text: "<font size=" + HeaderMenu_FontSize_1 + ">" + "ЗП" + "</font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + "Зарплата" + "</font>",
            icon: '../Scripts/sklad/images/Dir/discount16.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "RightSalaries0", hidden: true,
            menu: [
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Выплаты ЗП" + "</font>", icon: '../Scripts/sklad/images/Dir/discount16.png',
                    itemId: "btnDocDomesticExpenseSalaries", id: "RightDocDomesticExpenseSalaries", hidden: true,
                },

                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Зарплата (по сотрудникам)" + "</font>", icon: '../Scripts/sklad/images/Dir/wallet16.png',
                    itemId: "btnReportSalaries", id: "RightReportSalaries", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Зарплата (по точкам)" + "</font>", icon: '../Scripts/sklad/images/Dir/wallet16.png',
                    itemId: "btnReportSalariesWarehouses", id: "RightReportSalariesWarehouses", hidden: true,
                },
            ]
        },

        //Логистика
        {
            xtype: 'button',
            text: "<font size=" + HeaderMenu_FontSize_1 + ">" + "Логистика" + "</font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + "Логистика" + "</font>",
            icon: '../Scripts/sklad/images/orders.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "RightLogistics0", hidden: true,
            menu: [
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Торговля" + "</font>", icon: '../Scripts/sklad/images/table_add.png',
                    itemId: "btnMovementsLogisticsNew", id: "RightDocMovementsLogisticsNew", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Б/У" + "</font>", icon: '../Scripts/sklad/images/table_add.png',
                    itemId: "btnSecondHandMovementsLogisticsNew", id: "RightDocSecondHandMovementsLogisticsNew", hidden: true,
                },

                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Логистика" + "</font>", icon: '../Scripts/sklad/images/journal.png',
                    itemId: "btnLogistics", id: "RightDocMovementsLogistics", hidden: true,
                },

                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Отчет" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnDocMovementsLogisticsReport", id: "RightDocMovementsLogisticsReport", //hidden: true,
                },

                "-",
                {
                    text: "<font size=2><b>" + lanDirectories + "</b></font>"
                },
                "-",
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Шаблоны SMS" + "</font>", icon: '../Scripts/sklad/images/sms16.png',
                    itemId: "btnDocMovementsLogisticsSmsTemplates", id: "RightDocMovementsLogisticsSmsTemplates", hidden: true,
                },
            ]
        },

        //Аналитика
        {
            xtype: 'button',
            text: "<font size=" + HeaderMenu_FontSize_1 + ">" + "Аналитика" + "</font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + "Аналитика" + "</font>",
            icon: '../Scripts/sklad/images/abacus.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "RightAnalitics0", hidden: true,
            menu: [
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "по филиалу" + "</font>", icon: '../Scripts/sklad/images/Dir/warehouses16.png',
                    itemId: "btnAnalitics1", id: "Right1Analitics", //hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "по сотруднику" + "</font>", icon: '../Scripts/sklad/images/users.png',
                    itemId: "btnAnalitics2", id: "Right2Analitics", //hidden: true,
                },
            ]
        },


        //ККМ
        {
            xtype: 'button',
            text: "<font size=" + HeaderMenu_FontSize_1 + ">" + "ККМ" + "</font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + "ККМ" + "</font>",
            icon: '../Scripts/sklad/images/modem.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "RightKKM0", hidden: true,
            menu: [
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "X-отчет" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnKKMXReport", id: "RightKKMXReport", hidden: true,
                },

                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Открытие смены" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnKKMOpen", id: "RightKKMOpen", hidden: true,
                },

                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Внесение денег в кассу" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnKKMAdding", id: "RightKKMAdding", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Инкассация денег из кассы" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnKKMEncashment", id: "RightKKMEncashment", hidden: true,
                },

                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Закрытие смены" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnKKMClose", id: "RightKKMClose", hidden: true,
                },

                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Печать состояния расчетов и связи с ОФД" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnKKMPrintOFD", id: "RightKKMPrintOFD", hidden: true,
                },

                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Получить данные последнего чека из ФН" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnKKMCheckLastFN", id: "RightKKMCheckLastFN", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Получить текущее состояние ККТ" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnKKMState", id: "RightKKMState", hidden: true,
                },
                {
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + "Получение списка ККМ" + "</font>", icon: '../Scripts/sklad/images/report.png',
                    itemId: "btnKKMList", id: "RightKKMList", hidden: true,
                },

            ]
        },








        "->",
        {
            //xtype: 'button',
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">Сообщения</font>",
            icon: '../Scripts/sklad/images/notification.png',
            height: 35,
            iconAlign: 'left', textAlign: 'right',
            id: "HeaderToolBarNotification", 
            UO_CointItems: 0,
            text: "0",
            menu: [
                /*{
                    text: "<font size=" + HeaderMenu_FontSize_2 + ">" + lanMyCompany + "</font>",
                    icon: '../Scripts/sklad/images/company16.png',
                },*/
            ]
        },

        {
            //text: "",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanEmployee + ": " + lanDirEmployeeName + "</font>",
            id: "HeaderToolBarEmployees",
            icon: '../Scripts/sklad/Images/Dir/employees16.png',
            height: 25,
            iconAlign: 'left', textAlign: 'right',
            handler: function () {
                
                //Показываем форму с выбором склада (в котором будет работать сотрудник)
                var Params = [
                    Ext.getCmp("Viewport"), //UO_idCall
                    true, //UO_Center
                    true, //UO_Modal
                    1     // 1 - Новое, 2 - Редактировать
                ]
                ObjectEditConfig("viewDirWarehouseSelect", Params);

            }
        },
        "  ",
        {
            //text: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanDevelop + "</font>",
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanDevelopDetail + "</font>",
            icon: '../Scripts/sklad/images/develop2.png',
            height: 35, //width: 25,
            iconAlign: 'left', textAlign: 'left',
            id: "RightDevelop", hidden: true,
            handler:
                function () {
                    Ext.MessageBox.show({
                        title: lanOrgName,
                        msg: txtMsg035, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
                        fn: function (buttons) {
                            if (buttons == "yes") { window.open('Develop/', '_newtab'); }
                        }
                    });
                }
        },
        {
            //text: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanAbout + "</font>",
            icon: '../Scripts/sklad/images/about.png',
            height: 35, //width: 25, //width: 110,
            iconAlign: 'left', textAlign: 'right',
            handler:
                function () {
                    Ext.Msg.alert(lanOrgName, lanOrgName + "<BR> " + verSystem + "<BR> " + varSystemDate + "<BR> " + varSystemDevelop);
                }

        },
        {
            //text: "<font size=" + HeaderMenu_FontSize_1 + "></font>", //lanExit
            tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanExit + "</font>",
            icon: '../Scripts/sklad/images/exit.png',
            height: 35, //width: 25,
            iconAlign: 'left', textAlign: 'right',
            handler:
                function () {
                    Ext.MessageBox.show({
                        title: lanOrgName,
                        msg: lanExit2, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
                        fn: function (buttons) { if (buttons == "yes") { Ext.util.Cookies.clear('CookieIPOL'); window.location.href = '/account/login/'; } }
                    });
                }
        }

    ]

});