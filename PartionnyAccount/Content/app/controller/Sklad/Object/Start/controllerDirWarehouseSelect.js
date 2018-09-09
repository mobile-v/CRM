Ext.define("PartionnyAccount.controller.Sklad/Object/Start/controllerDirWarehouseSelect", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({

            'viewDirWarehouseSelect [itemId=DirWarehouseID]': { select: this.onDirWarehouseIDSelect },

            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirWarehouseSelect button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirWarehouseSelect button#btnWebServer': { "click": this.onBtnWebServerClick },
        });
    },


    onDirWarehouseIDSelect: function (combo, records, eOpts) {
        //Только если в настройках вклёчон ККМ: varKKMSActive_FromSet = true
        if (varKKMSActive_FromSet) {
            if (records.data.KKMSActive) {
                Ext.getCmp("KKMSNumDeviceStart").enable();
                Ext.getCmp("KKMLabel").setText("");

                //Раз-Блокируем Меню "ККМ"
                Ext.getCmp("RightKKM0").enable();
            }
            else {
                Ext.getCmp("KKMSNumDeviceStart").disable();
                Ext.getCmp("KKMLabel").setText("Не используется ККМ");

                //Блокируем Меню "ККМ"
                Ext.getCmp("RightKKM0").disable();
            }
        }

        //Авто-закрытие смены
        if (records.data.SmenaClose) {
            varSmenaClose = true;
            varSmenaCloseTime = records.data.SmenaCloseTime;
        }

    },


    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        
        //ККМ
        if (Ext.getCmp("DirWarehouseIDStart").valueCollection.items[0].data.KKMSActive) {
            varKKMSNumDevice = parseInt(Ext.getCmp("KKMSNumDeviceStart").getValue());

            //Блокируем Меню "ККМ"
            Ext.getCmp("RightKKM0").enable();
        }
        else {
            varKKMSActive = false;
            varKKMSNumDevice = 0

            //Блокируем Меню "ККМ"
            Ext.getCmp("RightKKM0").disable();
        }
        
        //Для смены склада в "viewDocXXX"
        var varDirWarehouseID_OLD = varDirWarehouseID;

        if (Ext.getCmp("DirWarehouseIDStart").getValue() == null) { Ext.Msg.alert(lanOrgName, "Пожалусте, выберите Точку!"); return; }

        varDirWarehouseIDEmpl = parseInt(Ext.getCmp("DirWarehouseIDStart").getValue());
        varDirWarehouseID = parseInt(Ext.getCmp("DirWarehouseIDStart").getValue());
        varDirWarehouseNameEmpl = Ext.getCmp("DirWarehouseIDStart").rawValue;
        
        if (Ext.getCmp("HeaderToolBarEmployees") != undefined) { //Если входим в модуль Розница
            funResizeBrowser();
            Ext.getCmp("HeaderToolBarEmployees").setTooltip("<font size=" + HeaderMenu_FontSize_1 + ">" + lanEmployee + ": " + lanDirEmployeeName + "</font>")
        }

        //Администратор Точки (Админ проекта - администратор любой точки)
        if (varDirEmployeeID != 1) {
            varIsAdmin = Ext.getCmp("DirWarehouseIDStart").valueCollection.items[0].data.IsAdmin;
        }
        else {
            //Админ проекта
            varIsAdmin = true;
        }

        Ext.getCmp("viewDirWarehouseSelect" + aButton.UO_id).close();

        //Меняем склад во всех открытых таб-ах.
        var tabPanelMain = Ext.getCmp("viewContainerCentral");
        for (var i = 0; i < tabPanelMain.items.items.length; i++) {

            /*

            //Все Доки, кроме СС
            if (
                tabPanelMain.items.items[i].UO_View != undefined &&
                tabPanelMain.items.items[i].UO_View.indexOf("viewDoc") > -1 &&
                tabPanelMain.items.items[i].storeGrid != undefined
               ) {
                var url = tabPanelMain.items.items[i].storeGrid.proxy.url;
                url = url.replace("DirWarehouseID=" + varDirWarehouseID_OLD, "DirWarehouseID=" + varDirWarehouseID);
                tabPanelMain.items.items[i].storeGrid.proxy.url = url;
                tabPanelMain.items.items[i].storeGrid.load({ waitMsg: lanLoading });
            }
            //СС
            else if (tabPanelMain.items.items[i].UO_View == "viewDocServiceWorkshops") {
                //0
                var url = tabPanelMain.items.items[i].storeGrid0.proxy.url;
                url = url.replace("DirWarehouseID=" + varDirWarehouseID_OLD, "DirWarehouseID=" + varDirWarehouseID);
                tabPanelMain.items.items[i].storeGrid0.proxy.url = url;
                //1
                var url = tabPanelMain.items.items[i].storeGrid1.proxy.url;
                url = url.replace("DirWarehouseID=" + varDirWarehouseID_OLD, "DirWarehouseID=" + varDirWarehouseID);
                tabPanelMain.items.items[i].storeGrid1.proxy.url = url;
                //2
                var url = tabPanelMain.items.items[i].storeGrid2.proxy.url;
                url = url.replace("DirWarehouseID=" + varDirWarehouseID_OLD, "DirWarehouseID=" + varDirWarehouseID);
                tabPanelMain.items.items[i].storeGrid2.proxy.url = url;
                //3
                var url = tabPanelMain.items.items[i].storeGrid3.proxy.url;
                url = url.replace("DirWarehouseID=" + varDirWarehouseID_OLD, "DirWarehouseID=" + varDirWarehouseID);
                tabPanelMain.items.items[i].storeGrid3.proxy.url = url;
                //4
                var url = tabPanelMain.items.items[i].storeGrid4.proxy.url;
                url = url.replace("DirWarehouseID=" + varDirWarehouseID_OLD, "DirWarehouseID=" + varDirWarehouseID);
                tabPanelMain.items.items[i].storeGrid4.proxy.url = url;
                //5
                var url = tabPanelMain.items.items[i].storeGrid5.proxy.url;
                url = url.replace("DirWarehouseID=" + varDirWarehouseID_OLD, "DirWarehouseID=" + varDirWarehouseID);
                tabPanelMain.items.items[i].storeGrid5.proxy.url = url;
                //6
                var url = tabPanelMain.items.items[i].storeGrid6.proxy.url;
                url = url.replace("DirWarehouseID=" + varDirWarehouseID_OLD, "DirWarehouseID=" + varDirWarehouseID);
                tabPanelMain.items.items[i].storeGrid6.proxy.url = url;
                //7
                var url = tabPanelMain.items.items[i].storeGrid7.proxy.url;
                url = url.replace("DirWarehouseID=" + varDirWarehouseID_OLD, "DirWarehouseID=" + varDirWarehouseID);
                tabPanelMain.items.items[i].storeGrid7.proxy.url = url;
                //8
                var url = tabPanelMain.items.items[i].storeGridX.proxy.url;
                url = url.replace("DirWarehouseID=" + varDirWarehouseID_OLD, "DirWarehouseID=" + varDirWarehouseID);
                tabPanelMain.items.items[i].storeGridX.proxy.url = url;
                //9
                var url = tabPanelMain.items.items[i].storeGrid9.proxy.url;
                url = url.replace("DirWarehouseID=" + varDirWarehouseID_OLD, "DirWarehouseID=" + varDirWarehouseID);
                tabPanelMain.items.items[i].storeGrid9.proxy.url = url;
            }

            */

            //Закрываем все вкладки
            //tabPanelMain.items.items[i].close();

        }

        //Закрываем все вкладки
        var i = tabPanelMain.items.items.length;
        while (i != 0) {
            tabPanelMain.items.items[i - 1].close();
            i = tabPanelMain.items.items.length;
        }

    },


    onBtnWebServerClick: function (aButton, aEvent, aOptions) {

        Ext.Ajax.setUseDefaultXhrHeader(false);
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: "http://127.0.0.1:81/?par=11111",
            method: 'GET',
            success: function (result) {

                var sData = Ext.decode(result.responseText);
                if (sData.success == true) {
                    alert("данные от кассы получены: " + sData.data.par);
                }
                else {
                    Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK })
                }

            },
            failure: function (form, action) {
                Ext.MessageBox.show({ title: lanFailure, msg: "Нет связи с Сервером ККМ!", icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
            }
        });

    },

});


var controllerDirWarehouseSelect_getLocation = function (href) {
    var l = document.createElement("a");
    l.href = href;
    return l;
};

