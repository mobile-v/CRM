/*
    1. Обработка ошибок пришедших с Срервера
    2. Вывод соответствующего сообщения
*/
function funPanelSubmitFailure(form, action) {
    
    //На всякий случай.
    if (action == undefined) { Ext.MessageBox.show({ title: lanFailure, msg: "Error!", icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return; }

    //Права.
    //if (action.result != undefined && action.result.data != undefined && action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
    if (action.result != undefined && action.result.data != undefined && action.result.data[0] == "{") {
        var sData = Ext.decode(action.result.data);
        if (sData.msgType == "1") {
            Ext.MessageBox.show({ title: lanFailure, msg: sData.msg, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
            return;
        }
    }

    var msg = "";
    if (action.response != undefined && action.response.responseText != undefined) {
        //msg = Ext.decode(action.response.responseText).ExceptionMessage;
        msg = Ext.decode(action.response.responseText);
        if (msg.ExceptionMessage != undefined) msg = msg.ExceptionMessage
        else msg = msg.data

    }

    switch (action.failureType) {
        case Ext.form.action.Action.CLIENT_INVALID:
            Ext.MessageBox.show({ title: lanFailure, msg: lanMsg006 + "<hr />" + msg, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
            break;
        case Ext.form.action.Action.CONNECT_FAILURE:
            Ext.MessageBox.show({ title: lanFailure, msg: lanMsg007 + "<hr />" + msg, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
            break;
        case Ext.form.action.Action.SERVER_INVALID:
            if (action.result.msg != undefined) { Ext.Msg.minWidth = 380; Ext.Msg.alert(lanFailure, action.result.msg + "<hr />" + msg); }
            else if (action.result.data != undefined) { Ext.Msg.minWidth = 380; if (action.result.data = msg) msg = ""; Ext.Msg.alert(lanFailure, action.result.data + "<hr />" + msg); }
            else { Ext.MessageBox.show({ title: lanFailure, msg: txtMsg009 + "<hr />" + msg, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); }

            break;
        default:
            if (action.result != undefined && action.result.msg != undefined) { Ext.MessageBox.show({ title: lanFailure, msg: action.result.msg, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); }
            else { Ext.MessageBox.show({ title: lanFailure, msg: msg, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); }
            break;
    }
}



/*
    !!! Не используется !!!
    Исправление бага расположения Окна в центральной панели
*/
function funWinPosition(win) {
    //Что бы окошко не отскакивало от верхней и левой панели
    win.on('move', function (win, x, y) { if (y < 0 || x < 0) { win.setPosition(x < 0 ? 0 : x, y < 0 ? 0 : y); } });
    win.on('maximize', function () { win.setPosition(0, 0); });
    win.on('restore', function () { win.setPosition(0, 0); });
    //Ext.getCmp("viewcontainercentral").on('move', function () { win.toggleMaximize() });
    //Координаты расположение окна - увеличиваем или обнуляем
    funObjectXY();
}
//Координаты размещение окон в Центральной панели
function funObjectXY() {
    if (ObjectXY > 160) { ObjectXY = 1; }
    else ObjectXY += 20;
}



/*
    true - focus, false - not focus
    focused - когда удаляем что-то из таблицы, то не надо окно вперёд выводить, а то перекрывает сообщение "Удалено!"
*/
function funSearchWin(id, focused) {
    var _panel = Ext.getCmp(id);
    if (_panel != null) {
        if (focused) {
            if (InterfaceSystem == 1 || InterfaceSystem == 3) {
                //Win or Panel
                _panel.focus(false, true);
            }
            else {
                //Tab
                var viewContainerCentral = Ext.getCmp("viewContainerCentral");
                viewContainerCentral.setActiveTab(_panel);
            }
        }
        return true;
    }
    else { return false; }
}



/*
    InterfaceSystem == 3
    Если панель типа: layout: 'card' (разновидность вкладок с переключателем внизу Центральной панели)
*/

//Активная: Предыдущая панель
function funInterfaceSystem3_prev() {
    var layout = Ext.getCmp("viewContainerCentral").getLayout();
    if (layout.getPrev()) {
        layout.prev();
        return true;
    }
    else {
        return false;
    }
}
//Активная: Следующая панель
function funInterfaceSystem3_next(del, widgetX) {
    //1. Удалить все предыдущие, до открытой вкладки
    funInterfaceSystem3_next_del(del, widgetX)

    //2. Показать новый
    var layout = Ext.getCmp("viewContainerCentral").getLayout();
    if (layout.getNext()) {
        layout.next();
        return true;
    }
    else {
        return false;
    }
}
function funInterfaceSystem3_next_del(del, widgetX) {
    if (del) {
        var arrayID = [];
        var countID = -1;

        var items = Ext.getCmp("viewContainerCentral").items;
        var bDel = false;
        for (var i = 0; i < items.length; i++) {
            //Уже, после того как нашли активную вкладку, то удалить все выше Активной
            if (bDel) {
                //Формируем массив для удаляения, потому, что при удалении смещется массив виджетов на панели Централ (i = i-1) и не удаляет все элементы
                if (items.items[i].id != widgetX.id) {
                    countID++;
                    arrayID[countID] = items.items[i].id;
                }
            }
            else {
                //Если нашли Активную вкладку, то после неё все удалить (дальше по циклу)
                if (Ext.getCmp(widgetX.UO_idCall) != undefined && Ext.getCmp(widgetX.UO_idCall).UO_idMain == items.items[i].id) bDel = true;
            }
        }

        //Удаляем
        for (var i = 0; i <= countID; i++) {
            Ext.getCmp("viewContainerCentral").remove(arrayID[i], false);
        }
    }
}

//При закрытии Панелей срабатывает функция
function funInterfaceSystem3_closePanel(aPanel) {
    if (InterfaceSystem == 3) {
        //alert("Close");

        //Получаем номер в Центральной панели
        //a. Если не "0", то делаем активной i-1
        //b. Если 0, то делаем активной "0"
        var items = Ext.getCmp("viewContainerCentral").items;
        for (var i = 0; i < items.length; i++) {
            //aPanel.id ( == viewDirContractors1)
            if (items.items[i].id == aPanel.id) {
                if (i > 0) {
                    //делаем активной i-1
                    funInterfaceSystem3_prev();
                }
                else {
                    //делаем активной "0"
                    funInterfaceSystem3_next(false);

                }
            }
        }
    }
}



/*
    parseBool
*/
function funParseBool(strValue) {
    return strValue == "true" ? true : false
}



/*
    Справочники
    Поиск в Гриде
    Где:
     - iStartAddFunc: запуск дополнительной функции:
        1: Номенклатура, показать/спрятать поле остаток
*/
function funGridDir(obj, txt, url, iStartAddFunc) {
    //Получаем storeGrid и делаем load()
    var storeGrid = Ext.getCmp("grid_" + obj.UO_id).getStore();
    storeGrid.proxy.url = url + "?DirEmployeeID=" + Ext.getCmp("DirEmployeeID" + Ext.getCmp(obj.UO_idCall).UO_id).getValue() + "&parSearch=" + txt; //HTTP_DirBanksGrid
    storeGrid.load();

    if (iStartAddFunc == 1) {
        if (txt.length > 0) funDirNomens_NomenRemains_Visible(obj.UO_id, true);
        else funDirNomens_NomenRemains_Visible(obj.UO_id, false);
    }
}
//Дополнение к "funGridDir(...)"
//Номенклатура (Товар): Показать/Спрятать поле "Остаток"
function funDirNomens_NomenRemains_Visible(obj, bVisible) {
    Ext.getCmp("grid_" + obj.UO_id).columns.forEach(function (col) { if ((col.dataIndex == "NomenRemains")) { col.setVisible(bVisible); } });
}



/*
    Документы
    Изменили дату + Поиск в Гриде
    Где:
     - iFilterType: кликнули слева на ТриВью
        1: 
     - params: например для Сервисного центра, что бы для каждой формы отображать записи по статучам
*/
function funGridDoc(UO_id, url, iFilterType, params) {
    //Документы
    if (Ext.getCmp("grid_" + UO_id) != undefined) {
        var _DateS = Ext.Date.format(Ext.getCmp("DateS" + UO_id).getValue(), 'Y-m-d');
        var _DatePo = Ext.Date.format(Ext.getCmp("DatePo" + UO_id).getValue(), 'Y-m-d');

        if (iFilterType == undefined) iFilterType = 0;

        //Получаем storeGrid и делаем load()
        var storeGrid = Ext.getCmp("grid_" + UO_id).getStore();
        storeGrid.proxy.url =
            url +
            "?parSearch=" + encodeURIComponent(Ext.getCmp("TriggerSearchGrid" + UO_id).value) +
            "&DateS=" + _DateS +
            "&DatePo=" + _DatePo +
            "&DirWarehouseID=" + varDirWarehouseID +
            "&FilterType=" + iFilterType +
            "&" + params;
        storeGrid.load();
    }

    //Сервисный цент: Архив

    else {
        var _DateS = Ext.Date.format(Ext.getCmp("DateS" + UO_id).getValue(), 'Y-m-d');
        var _DatePo = Ext.Date.format(Ext.getCmp("DatePo" + UO_id).getValue(), 'Y-m-d');
        //var _DirWarehouseID = Ext.Date.format(Ext.getCmp("DirWarehouseIDPanelGrid9_" + UO_id).getValue(), 'Y-m-d');

        var TriggerSearchGrid = Ext.getCmp("TriggerSearchGrid" + UO_id).value;
        TriggerSearchGrid = TriggerSearchGrid.replace("+", "%");

        //Получаем storeGrid и делаем load()
        var storeGrid = Ext.getCmp("PanelGrid9_" + UO_id).getStore();
        storeGrid.proxy.url =
            url +
            "&parSearch=" + encodeURIComponent(Ext.getCmp("TriggerSearchGrid" + UO_id).value) +
            "&DateS=" + _DateS +
            "&DatePo=" + _DatePo +
            //"&DirWarehouseID=" + _DirWarehouseID +
            "&" + params;
        storeGrid.load();
    }
}

//Для БУ.Разбор - там есть id-шник "grid_" из-за чего работает не корректно (как для обычных документов)!
function funGridDoc_2(UO_id, url, iFilterType, params) {
    
    var _DateS = Ext.Date.format(Ext.getCmp("DateS" + UO_id).getValue(), 'Y-m-d');
    var _DatePo = Ext.Date.format(Ext.getCmp("DatePo" + UO_id).getValue(), 'Y-m-d');

    var TriggerSearchGrid = Ext.getCmp("TriggerSearchGrid" + UO_id).value;
    TriggerSearchGrid = TriggerSearchGrid.replace("+", "%");

    //Получаем storeGrid и делаем load()
    var storeGrid = Ext.getCmp("PanelGrid9_" + UO_id).getStore();
    storeGrid.proxy.url =
        url +
        "&parSearch=" + encodeURIComponent(Ext.getCmp("TriggerSearchGrid" + UO_id).value) +
        "&DateS=" + _DateS +
        "&DatePo=" + _DatePo +
        "&" + params;
    storeGrid.load();
}



/*
    Помечена ли запись на удаление в Гриде
*/
function funGridRecordDel(record) {
    if (record.data.Del == true) {
        Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
        return true;
    }
    else {
        return false;
    }
}



/*
    Запрос на сервер за Налогом Контрагента
*/
function funRequestContractorVat(DirContractorID, id, fun_RecalculationSums) {

    var loadingMask = new Ext.LoadMask({
        msg: 'Please wait...',
        target: Ext.getCmp("viewContainerCentral") //Ext.getCmp("tree_" + aButton.UO_id)
    });
    loadingMask.show();

    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: HTTP_DirContractors + DirContractorID + "/",
        method: 'GET',
        success: function (result) {
            loadingMask.hide();

            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                Ext.Msg.alert(lanOrgName, txtMsg008 + result.responseText);
                return;
            }
            else {
                //Меняем Налог
                Ext.getCmp("DirVatName" + id).setValue(sData.data.DirVatName);
                Ext.getCmp("DirVatValue" + id).setValue(sData.data.DirVatValue);
                Ext.getCmp("DirVatID" + id).setValue(sData.data.DirVatID);

                fun_RecalculationSums(id, true);
            }
        },
        failure: function (result) {
            loadingMask.hide();
            Ext.Msg.alert(lanOrgName, txtMsg008 + result.responseText);
        }
    });

}



/*
    Для Скидки в Документах
*/
function funSpreadDiscount(id, ArrPrice, XXX_RecalculationSums) {

    //Хранилище
    var storeGrid = Ext.getCmp("grid_" + id).store;
    //Грид
    var PanelGrid = Ext.getCmp("grid_" + id);

    //Алгоритм:
    //В каждой позиции меняем цену (уменьшаем/увеличиваем) на процент

    //Переменные:
    //1. Процент
    var dDiscount = parseFloat(Ext.getCmp("Discount" + id).getValue());
    //1. Цены
    var PriceCurrency = ArrPrice[0],
        PriceVAT = ArrPrice[1],
        SUMPriceCurrency = ArrPrice[2],
        DirCurrencyRate = ArrPrice[3],
        DirCurrencyMultiplicity = ArrPrice[4],
        Quantity = ArrPrice[5];


    //Пробегаем по всем спецификациям
    for (var i = 0; i <= storeGrid.data.items.length - 1; i++) {
        //Цена в вал.
        storeGrid.data.items[i].data[PriceCurrency] =
            (
            parseFloat(storeGrid.data.items[i].data[PriceCurrency]) -
            parseFloat(storeGrid.data.items[i].data[PriceCurrency]) * (parseFloat(dDiscount) / 100)
            ).toFixed(varFractionalPartInSum);

        //Цена в тек.вал.        
        storeGrid.data.items[i].data[PriceVAT] =
            (
            (parseFloat(storeGrid.data.items[i].data[PriceCurrency]) /
            parseFloat(storeGrid.data.items[i].data[DirCurrencyRate])) *
            parseFloat(storeGrid.data.items[i].data[DirCurrencyMultiplicity])
            ).toFixed(varFractionalPartInSum);

        //Сумма
        storeGrid.data.items[i].data[SUMPriceCurrency] =
            (
            parseFloat(storeGrid.data.items[i].data[PriceCurrency]) *
            parseFloat(storeGrid.data.items[i].data[Quantity])
            ).toFixed(varFractionalPartInSum);
    }


    //Анулируем Скидку
    Ext.getCmp("Discount" + id).setValue(0);

    //Обновляем Грид. Т.к. после изменения сторе в гриде не видно.
    PanelGrid.reconfigure(storeGrid);
    PanelGrid.getView().refresh();

    //Пересчет сумм
    XXX_RecalculationSums(id, false);
}



/*
    Для Скидки в Документах
*/
function funSpreadSummOther(id, ArrPrice, XXX_RecalculationSums) { //record, 

    //Хранилище
    var storeGrid = Ext.getCmp("grid_" + id).store;
    //Грид
    var PanelGrid = Ext.getCmp("grid_" + id);

    var PriceCurrency = ArrPrice[0],
        PriceVAT = ArrPrice[1],
        SUMPriceCurrency = ArrPrice[2],
        DirCurrencyRate = ArrPrice[3],
        DirCurrencyMultiplicity = ArrPrice[4],
        Quantity = ArrPrice[5];


    //Надо учесть, что в сумме содержится скидка!

    //Алгоритм:
    //1. Подсчитываем сумму (без скидки)
    //2. Вычисляем разницу X1 "Старой суммы" (реальной суммы) и "Новой суммы" (введёной вручную) - 
    //   На это число надо уменьшить/увеличить цены товара, учтя количество.
    //3. Т.к. цены и к-ва для каждой позиции поэтому для каждой позиции надо определить процент от общей суммы:
    //   а) X2 = 100 / (Старая сумма)  - это "Сумма на 1%"
    //   б) Множим сумму каждой позиции на X2 и получаем процент каждой позиции от суммы
    //4. Прибаляем (отнимает) от каждой позиции X1 * X2


    //1. Подсчитываем сумму (без скидки)
    var SumReal = 0; //DontDiscount
    for (var i = 0; i <= storeGrid.data.items.length - 1; i++) {
        SumReal +=
            parseFloat
            (
                parseFloat(storeGrid.data.items[i].data[PriceCurrency]) * parseFloat(storeGrid.data.items[i].data[Quantity])
            );
    }


    //2. Вычисляем разницу X1 "Старой суммы" (реальной суммы) и "Новой суммы" (введёной вручную) - 
    //   На это число надо уменьшить/увеличить цены товара, учтя количество.
    var DifferenceSums = parseFloat(parseFloat(Ext.getCmp("SummOther" + id).getValue()) - parseFloat(SumReal));


    //3. Т.к. цены и к-ва для каждой позиции разные, то поэтому для каждой позиции надо определить процент от общей суммы:
    //   а) X2 = 100 / (Старая сумма)  - это "Сумма на 1%"
    //   б) Множим сумму каждой позиции на X2 и получаем процент каждой позиции от суммы
    var PercentOne = 1 / SumReal;
    for (var i = 0; i <= storeGrid.data.items.length - 1; i++) {
        var PercentPositions =
            parseFloat(
                parseFloat(parseFloat(storeGrid.data.items[i].data[PriceCurrency]) *
                parseFloat(storeGrid.data.items[i].data[Quantity]) *
                parseFloat(PercentOne))
            );


        //4. Прибаляем (отнимает) от каждой позиции X1 * X2

        //Цена в вал.
        storeGrid.data.items[i].data[PriceCurrency] =
            parseFloat
            (
                parseFloat(storeGrid.data.items[i].data[PriceCurrency]) +
                parseFloat((parseFloat(DifferenceSums) * parseFloat(PercentPositions)) / parseFloat(storeGrid.data.items[i].data[Quantity]))
            ).toFixed(varFractionalPartInSum);

        //Цена в тек.вал.        
        storeGrid.data.items[i].data[PriceVAT] =
            parseFloat
            (
                (parseFloat(storeGrid.data.items[i].data[PriceCurrency]) /
                parseFloat(storeGrid.data.items[i].data[DirCurrencyRate])) *
                parseFloat(storeGrid.data.items[i].data[DirCurrencyMultiplicity])
            ).toFixed(varFractionalPartInSum);

        //Сумма
        storeGrid.data.items[i].data[SUMPriceCurrency] =
            parseFloat
            (
                parseFloat(storeGrid.data.items[i].data[PriceCurrency]) *
                parseFloat(storeGrid.data.items[i].data[Quantity])
            ).toFixed(varFractionalPartInSum);

    }


    //Анулируем Скидку
    Ext.getCmp("SummOther" + id).setValue(0);

    //Обновляем Грид. Т.к. после изменения сторе в гриде не видно.
    PanelGrid.reconfigure(storeGrid);
    PanelGrid.getView().refresh();

    //Пересчет сумм
    XXX_RecalculationSums(id, false);

}



/*
    parseBool
*/
function fun_Select_Contractor_Warehouse(id) {
    
    if (Ext.getCmp("DirContractorID" + id).getValue() == "" || isNaN(parseInt(Ext.getCmp("DirContractorID" + id).getValue()))) { return false; }
    if (Ext.getCmp("DirWarehouseID" + id).getValue() == "" || isNaN(parseInt(Ext.getCmp("DirWarehouseID" + id).getValue()))) { return false; }

    return true;
}



/*
    Создание копии объекта.
    Используется в контроллере "PartionnyAccount.controller.Sklad/Other/Form/controllerDirNomensQuantity", при "Подборе товара", когда вставляем товар, то вставляем сам объект, а не копию. И при смене к-ва меняется и в Record (панели номенклатуры) и в Store (спецификации), получается хрень полная!
    //deep
*/
function fun_ObjectCopy(obj) {
    if (Object.prototype.toString.call(obj) === '[object Array]') {
        var out = [], i = 0, len = obj.length;
        for (; i < len; i++) {
            out[i] = arguments.callee(obj[i]);
        }
        return out;
    }
    if (typeof obj === 'object') {
        var out = {}, i;
        for (i in obj) {
            out[i] = arguments.callee(obj[i]);
        }
        return out;
    }
    return obj;
}



/*
    Сохранение спецификации документа только для "Добавить позицию"
     - ListObjectTypeID - тип объекта: Документ, ПФ, ...

    Вызывает функцию "fun_SaveTab2"
*/
function fun_SaveTabDoc1(aButton, fn_RecalculationSums) {

    //Может быть так, что пользователь ввёл текст в КомбоБокс, которого нет в списке, тогда надо очистить Комбо!
    fun_ComboBox_Search_Correct("DirCharColourID" + aButton.UO_id);
    fun_ComboBox_Search_Correct("DirCharMaterialID" + aButton.UO_id);
    fun_ComboBox_Search_Correct("DirCharNameID" + aButton.UO_id);
    fun_ComboBox_Search_Correct("DirCharSeasonID" + aButton.UO_id);
    fun_ComboBox_Search_Correct("DirCharSexID" + aButton.UO_id);
    fun_ComboBox_Search_Correct("DirCharSizeID" + aButton.UO_id);
    fun_ComboBox_Search_Correct("DirCharStyleID" + aButton.UO_id);
    fun_ComboBox_Search_Correct("DirCharTextureID" + aButton.UO_id);

    //Проблемка с Характеристиками: с КомбоБоксаов можем получить только ID-шник выбранной записи, а нужно ещё и наименование.
    if (Ext.getCmp("DirCharColourName" + aButton.UO_id)) {
        Ext.getCmp("DirCharColourName" + aButton.UO_id).setValue(Ext.getCmp("DirCharColourID" + aButton.UO_id).getRawValue());
        Ext.getCmp("DirCharMaterialName" + aButton.UO_id).setValue(Ext.getCmp("DirCharMaterialID" + aButton.UO_id).getRawValue());
        Ext.getCmp("DirCharNameName" + aButton.UO_id).setValue(Ext.getCmp("DirCharNameID" + aButton.UO_id).getRawValue());
        Ext.getCmp("DirCharSeasonName" + aButton.UO_id).setValue(Ext.getCmp("DirCharSeasonID" + aButton.UO_id).getRawValue());
        Ext.getCmp("DirCharSexName" + aButton.UO_id).setValue(Ext.getCmp("DirCharSexID" + aButton.UO_id).getRawValue());
        Ext.getCmp("DirCharSizeName" + aButton.UO_id).setValue(Ext.getCmp("DirCharSizeID" + aButton.UO_id).getRawValue());
        Ext.getCmp("DirCharStyleName" + aButton.UO_id).setValue(Ext.getCmp("DirCharStyleID" + aButton.UO_id).getRawValue());
        Ext.getCmp("DirCharTextureName" + aButton.UO_id).setValue(Ext.getCmp("DirCharTextureID" + aButton.UO_id).getRawValue());
    }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);
    //Проверка
    if (!widgetXForm.UO_GridSave) { Ext.Msg.alert("", "UO_GridSave == undefined"); return; }


    //Сохраняем в Грид
    //Полее подробно: stackoverflow.com/questions/13155827/how-to-insert-values-in-store-in-extjs

    //Форма
    var form = widgetXForm.getForm();
    //Валидация
    if (!form.isValid()) {
        Ext.Msg.alert(lanOrgName, "Пожалуйста, заполните все поля формы!");
        return;
    }

    //Получаем данные полей с формы
    var rec = form.getFieldValues();

    //Заполяем скрытое поле "DirChar" - это список всех Характеристик через запятую
    rec.DirChar =
        Ext.getCmp("DirCharColourID" + aButton.UO_id).rawValue + " " +
        Ext.getCmp("DirCharMaterialID" + aButton.UO_id).rawValue + " " +
        Ext.getCmp("DirCharNameID" + aButton.UO_id).rawValue + " " +
        Ext.getCmp("DirCharSeasonID" + aButton.UO_id).rawValue + " " +
        Ext.getCmp("DirCharSexID" + aButton.UO_id).rawValue + " " +
        Ext.getCmp("DirCharSizeID" + aButton.UO_id).rawValue + " " +
        Ext.getCmp("DirCharStyleID" + aButton.UO_id).rawValue + " " +
        Ext.getCmp("DirCharTextureID" + aButton.UO_id).rawValue;

    //Получаем Store Грида
    var store = Ext.getCmp(aButton.UO_idCall).getStore();

    //Сохранение в Грид
    //if (widgetXForm.UO_GridRecord && widgetXForm.UO_GridRecord.data.Sub == undefined) {
    if (widgetXForm.UO_GridIndex != undefined) {
        //UPDATE
        store.remove(widgetXForm.UO_GridRecord);
        store.insert(widgetXForm.UO_GridIndex, rec);

        //Закрываем форму
        Ext.getCmp(aButton.UO_idMain).close();
        //Пересчитываем сумму
        fn_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false);
    }
    else {
        //INSERT
        //store.add(rec);

        //Поиск задвоенности === === === === === === === === === === ===  ===  ===  ===  === 
        //И сохранение
        //fun_SaveTabDoc2(rec, store, fn_RecalculationSums, Ext.getCmp(aButton.UO_idCall).UO_id, aButton.UO_idMain);

        //Insert
        store.insert(store.data.items.length, rec);
        //Пересчитываем сумму
        fn_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false);
        //Закрываем форму
        Ext.getCmp(aButton.UO_idMain).close();
    }
}
function fun_SaveTabDocNoChar1(aButton, fn_RecalculationSums) {
    
    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);
    //Проверка
    if (!widgetXForm.UO_GridSave) { Ext.Msg.alert("", "UO_GridSave == undefined"); return; }


    //Сохраняем в Грид
    //Полее подробно: stackoverflow.com/questions/13155827/how-to-insert-values-in-store-in-extjs

    //Форма
    var form = widgetXForm.getForm();
    //Валидация
    if (!form.isValid()) {
        Ext.Msg.alert(lanOrgName, "Пожалуйста, заполните все поля формы!");
        return;
    }

    //Получаем данные полей с формы
    var rec = form.getFieldValues();


    //Получаем Store Грида
    var store = Ext.getCmp(aButton.UO_idCall).getStore();

    //Сохранение в Грид
    //if (widgetXForm.UO_GridRecord && widgetXForm.UO_GridIndex != undefined) {
    if (widgetXForm.UO_GridIndex != undefined) {
        //UPDATE
        store.remove(widgetXForm.UO_GridRecord);
        store.insert(widgetXForm.UO_GridIndex, rec);

        //Закрываем форму
        Ext.getCmp(aButton.UO_idMain).close();
        //Пересчитываем сумму
        if (fn_RecalculationSums) fn_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false);
    }
    else {
        //INSERT
        //store.add(rec);

        //Поиск задвоенности === === === === === === === === === === ===  ===  ===  ===  === 
        //И сохранение
        //fun_SaveTabDoc2(rec, store, fn_RecalculationSums, Ext.getCmp(aButton.UO_idCall).UO_id, aButton.UO_idMain);

        //Insert
        store.insert(store.data.items.length, rec);
        //Пересчитываем сумму
        if (fn_RecalculationSums) fn_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false);
        //Закрываем форму
        Ext.getCmp(aButton.UO_idMain).close();
    }
}
function fun_SaveTabPf1(aButton) {

    //Проблемка с Характеристиками: с КомбоБоксаов можем получить только ID-шник выбранной записи, а нужно ещё и наименование.
    Ext.getCmp("ListObjectFieldNameRu" + aButton.UO_id).setValue(Ext.getCmp("ListObjectFieldNameID" + aButton.UO_id).getRawValue());

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);
    //Проверка
    if (!widgetXForm.UO_GridSave) { Ext.Msg.alert("", "UO_GridSave == undefined"); return; }


    //Сохраняем в Грид
    //Полее подробно: stackoverflow.com/questions/13155827/how-to-insert-values-in-store-in-extjs

    //Форма
    var form = widgetXForm.getForm();
    //Валидация
    if (!form.isValid()) {
        Ext.Msg.alert(lanOrgName, "Пожалуйста, заполните все поля формы!");
        return;
    }

    //Получаем данные полей с формы
    var rec = form.getFieldValues();

    //Получаем Store Грида
    var store = Ext.getCmp(aButton.UO_idCall).getStore();

    //Сохранение в Грид
    //if (widgetXForm.UO_GridRecord && widgetXForm.UO_GridRecord.data.Sub == undefined) {
    if (widgetXForm.UO_GridIndex != undefined) {
        //UPDATE
        store.remove(widgetXForm.UO_GridRecord);
        store.insert(widgetXForm.UO_GridIndex, rec);

        //Закрываем форму
        Ext.getCmp(aButton.UO_idMain).close();
    }
    else {
        //INSERT
        //store.add(rec);

        //Поиск задвоенности === === === === === === === === === === ===  ===  ===  ===  === 
        //И сохранение
        //fun_SaveTabPF2(rec, store, Ext.getCmp(aButton.UO_idCall).UO_id, aButton.UO_idMain);

        //Insert
        store.insert(store.data.items.length, rec);
        //Пересчитываем сумму
        //fn_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false);
        //Закрываем форму
        Ext.getCmp(aButton.UO_idMain).close();
    }
}
/*
    Сохранение спецификации документа для "Подбор товара"

    Проверка на задвоенность
    Сохраняем "rec" в "store" 
*/
function fun_SaveTabDoc2(rec, store, fn_RecalculationSums, id_Widget, id_EditForm) {
    
    var j = -1;           //Позиция найденого товара
    var fQuantity = 0;    //Количество
    var QuantityDoc = 0;  //Для "Акта Инвентаризации", что бы востановить удалённое поле "К-во по Док."

    //Поиск
    for (var i = 0; i <= store.data.items.length - 1; i++) {
        if (store.data.items[i].data.DirNomenID == rec.DirNomenID) {
            if (Ext.getCmp("grid_" + id_Widget).UO_idMain.indexOf("DocInventoryEdit") == -1) {
                fQuantity = store.data.items[i].data.Quantity;
            }
            else {
                fQuantity = store.data.items[i].data.QuantityReal;
                QuantityDoc = store.data.items[i].data.QuantityDoc;
            }

            j = i; break;
        }
    }

    //Если в таблице уже присутствует такой товар, то спрашиваем: увеличить существующи или добавить новой позицией
    if (j != -1) {
        Ext.Msg.show({
            title: lanOrgName,
            msg: 'Такой товар уже существует!<br />Увеличить количество?',
            buttons: Ext.Msg.YESNO,
            fn: function (btn) {
                if (btn == "yes") {
                    //меняем к-во
                    rec.Quantity = parseFloat(rec.Quantity) + parseFloat(fQuantity);
                    //Update
                    store.removeAt(j);
                    store.insert(j, rec);
                    //Пересчитываем сумму
                    fn_RecalculationSums(id_Widget, false);
                }
                else {
                    //Update
                    store.insert(store.data.items.length, rec);
                    //Пересчитываем сумму
                    fn_RecalculationSums(id_Widget, false);
                }
            },
            icon: Ext.window.MessageBox.QUESTION
        });
    }
    else {
        //Insert
        store.insert(store.data.items.length, rec);
        //Пересчитываем сумму
        fn_RecalculationSums(id_Widget, false);
    }

    //Закрываем форму
    Ext.getCmp(id_EditForm).close();
}
function fun_SaveTabPF2(rec, store, id_Widget, id_EditForm) {

    var j = -1;           //Позиция найденого товара

    //Поиск
    for (var i = 0; i <= store.data.items.length - 1; i++) {
        if (store.data.items[i].data.ListObjectFieldNameID == rec.ListObjectFieldNameID && store.data.items[i].data.ListObjectPFTabName == rec.ListObjectPFTabName) {
            j = i; break;
        }
    }

    //Если в таблице уже присутствует такой товар, то спрашиваем: увеличить существующи или добавить новой позицией
    if (j != -1) {
        Ext.Msg.show({
            title: lanOrgName,
            msg: 'Такая запись уже есть!<br />Перезаписать её (Yes) или Создать новую (No)?',
            buttons: Ext.Msg.YESNO,
            fn: function (btn) {
                if (btn == "yes") {
                    //Update
                    store.removeAt(j);
                    store.insert(j, rec);
                }
                else {
                    //Update
                    store.insert(store.data.items.length, rec);
                }
            },
            icon: Ext.window.MessageBox.QUESTION
        });
    }
    else {
        //Insert
        store.insert(store.data.items.length, rec);
    }

    //Закрываем форму
    Ext.getCmp(id_EditForm).close();
}



/*
    Контекстное Меню (правая кнопка мыши) для Грида
*/
var contextMenuTree = new Ext.menu.Menu({
    //renderTo: Ext.getBody(),
    itemId: 'contextMenuForTreePanel',
    items: [

        {
            icon: '../Scripts/sklad/images/folder_add.png',
            text: "Новая категория",
            tooltip: "Новая категория",
            itemId: "FolderNew",
            iconAlign: 'left', textAlign: 'right'
        },
        {
            icon: '../Scripts/sklad/images/folder_page.png',
            text: "Новая подкатегория",
            tooltip: "Новая подкатегория",
            itemId: "FolderNewSub",
            iconAlign: 'left', textAlign: 'right'
        },
        {
            icon: '../Scripts/sklad/images/folder_edit.png',
            text: "Редактировать",
            tooltip: "Редактировать",
            itemId: "FolderEdit",
            iconAlign: 'left', textAlign: 'right'
        },
        {
            icon: '../Scripts/sklad/images/folder_link.png',
            text: "Создать копию",
            tooltip: "Создать копию",
            itemId: "FolderCopy",
            iconAlign: 'left', textAlign: 'right'
        },
        {
            icon: '../Scripts/sklad/images/folder_delete.png',
            text: "Удалить",
            tooltip: "Удалить",
            itemId: "FolderDel",
            iconAlign: 'left', textAlign: 'right'
        },
        {
            icon: '../Scripts/sklad/images/folder_go.png',
            text: "Сделать корневой",
            tooltip: "Сделать корневой",
            itemId: "FolderSubNull",
            iconAlign: 'left', textAlign: 'right'
        },
        "-",
        {
            icon: '../Scripts/sklad/images/folder_go.png',
            text: "Разрешить Под-Группы?",
            tooltip: "Разрешить Под-Группы?",
            itemId: "AddSub",
            iconAlign: 'left', textAlign: 'right'
        },

    ],
    listeners: {
        click: function (dv, record, item, index, e) {

            switch (record.itemId) {
                case 'FolderNew':
                    dv.folderNew(dv.UO_id);
                    break;

                case 'FolderNewSub':
                    dv.folderNewSub(dv.UO_id);
                    break;

                case 'FolderEdit':
                    dv.folderEdit(dv.UO_id);
                    break;

                case 'FolderCopy':
                    dv.folderCopy(dv.UO_id);
                    break;

                case 'FolderDel':
                    dv.folderDel(dv.UO_id);
                    break;

                case 'FolderSubNull':
                    dv.folderSubNull(dv.UO_id);
                    break;

                case 'AddSub':
                    dv.addSub(dv.UO_id);
                    break;
            }

        }
    }
});



/*
    Этот метод не задействован, т.к. нет приходной цены в текущей валюте!!! (Используется в Документах, напр. в "Приходной накладной")
    Редактирование "Цены с НДС" (Учётная цена в текущей валюте)
*/



//Поменяли "Приходную цену" (она же "Цены с НДС" или "Учётная цена")
function fn_controllerDirNomensEdit_PriceVAT_Change(Object_ID, PrimaryFieldID) {
    if (varPriceChange_ReadOnly) return;
    varPriceChange_ReadOnly = true;
    
    if (Ext.getCmp("PriceVAT" + Object_ID) == undefined) { return; } //В Документе "Платежное поручение" отсутствует такое поле.
    if (parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) == 0) {
        //if (PrimaryFieldID != "-1") { Ext.Msg.alert(lanOrgName, txtMsg081); }
        varPriceChange_ReadOnly = false;
        return;
    }

    //1.Цена прихода без НДС
    if (Ext.getCmp("DirNomenHistoryPurchPrice" + Object_ID) != undefined) {
        var DirNomenHistoryPurchPrice = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) / (1 + parseFloat(Ext.getCmp("DirVatValue" + Object_ID).getValue()) / 100);
        Ext.getCmp("DirNomenHistoryPurchPrice" + Object_ID).setValue(DirNomenHistoryPurchPrice.toFixed(varFractionalPartInPrice));
    }

    //1.Цена прихода в текущей валюте
    if (Ext.getCmp("PriceCurrency" + Object_ID) != undefined) {
        var PriceRetailCurrency = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue())
        Ext.getCmp("PriceCurrency" + Object_ID).setValue(PriceRetailCurrency.toFixed(varFractionalPartInPrice));
    }

    //2.1.Розничная Цена с НДС
    if (Ext.getCmp("PriceRetailVAT" + Object_ID) != undefined) {
        var PriceRetailVAT = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) + parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * (parseFloat(Ext.getCmp("MarkupRetail" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceRetailVAT" + Object_ID).setValue(PriceRetailVAT.toFixed(varFractionalPartInPrice));
    }
    //2.2.Розничная цена в рублях
    if (Ext.getCmp("PriceRetailCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceRetailCurrency = parseFloat(Ext.getCmp("PriceRetailVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue())
        Ext.getCmp("PriceRetailCurrency" + Object_ID).setValue(PriceRetailCurrency.toFixed(varFractionalPartInPrice));
    }

    //3.1.Оптовая Цена с НДС
    if (Ext.getCmp("PriceWholesaleVAT" + Object_ID) != undefined) {
        var PriceWholesaleVAT = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) + parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * (parseFloat(Ext.getCmp("MarkupWholesale" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceWholesaleVAT" + Object_ID).setValue(PriceWholesaleVAT.toFixed(varFractionalPartInPrice));
    }
    //3.2.Оптовая цена в рублях
    if (Ext.getCmp("PriceWholesaleCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceWholesaleCurrency = parseFloat(Ext.getCmp("PriceWholesaleVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue())
        Ext.getCmp("PriceWholesaleCurrency" + Object_ID).setValue(PriceWholesaleCurrency.toFixed(varFractionalPartInPrice));
    }

    //4.1.IM Цена с НДС
    if (Ext.getCmp("PriceIMVAT" + Object_ID) != undefined) {
        var PriceIMVAT = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) + parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * (parseFloat(Ext.getCmp("MarkupIM" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceIMVAT" + Object_ID).setValue(PriceIMVAT.toFixed(varFractionalPartInPrice));
    }
    //4.2.Оптовая цена в рублях
    if (Ext.getCmp("PriceIMCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceIMCurrency = parseFloat(Ext.getCmp("PriceIMVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue())
        Ext.getCmp("PriceIMCurrency" + Object_ID).setValue(PriceIMCurrency.toFixed(varFractionalPartInPrice));
    }


    varPriceChange_ReadOnly = false;
}
//Этот метод не задействован, т.к. нет приходной цены в текущей валюте!!! (Используется в Документах, напр. в "Приходной накладной")
//Редактирование "Цены с НДС" (Учётная цена в текущей валюте)
function fn_controllerDirNomensEdit_PriceCurrency_Change(Object_ID, PrimaryFieldID) { //(Object_ID)
    if (varPriceChange_ReadOnly) return;
    varPriceChange_ReadOnly = true;
    
    //1.Цена прихода без НДС
    if (Ext.getCmp("DirNomenHistoryPurchPrice" + Object_ID) != undefined) {
        var DirNomenHistoryPurchPrice = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) / (1 + parseFloat(Ext.getCmp("DirVatValue" + Object_ID).getValue()) / 100);
        Ext.getCmp("DirNomenHistoryPurchPrice" + Object_ID).setValue(DirNomenHistoryPurchPrice.toFixed(varFractionalPartInPrice));
    }
    //1.Цена прихода с НДС в валюте
    if (Ext.getCmp("PriceCurrency" + Object_ID) != undefined && Ext.getCmp("PriceVAT" + Object_ID) != undefined) {
        var PriceVAT = parseFloat(Ext.getCmp("PriceCurrency" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue())
        Ext.getCmp("PriceVAT" + Object_ID).setValue(PriceVAT.toFixed(varFractionalPartInPrice));
    }

    //2.1.Розничная Цена с НДС
    if (Ext.getCmp("PriceRetailVAT" + Object_ID) != undefined) {
        var PriceRetailVAT = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) + parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * (parseFloat(Ext.getCmp("MarkupRetail" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceRetailVAT" + Object_ID).setValue(PriceRetailVAT.toFixed(varFractionalPartInPrice));
    }
    //2.2.Розничная цена в рублях
    if (Ext.getCmp("PriceRetailCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceRetailCurrency = parseFloat(Ext.getCmp("PriceRetailVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue())
        Ext.getCmp("PriceRetailCurrency" + Object_ID).setValue(PriceRetailCurrency.toFixed(varFractionalPartInPrice));
    }

    //3.1.Оптовая Цена с НДС
    if (Ext.getCmp("PriceWholesaleVAT" + Object_ID) != undefined) {
        var PriceWholesaleVAT = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) + parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * (parseFloat(Ext.getCmp("MarkupWholesale" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceWholesaleVAT" + Object_ID).setValue(PriceWholesaleVAT.toFixed(varFractionalPartInPrice));
    }
    //3.2.Оптовая цена в рублях
    if (Ext.getCmp("PriceWholesaleCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceWholesaleCurrency = parseFloat(Ext.getCmp("PriceWholesaleVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue())
        Ext.getCmp("PriceWholesaleCurrency" + Object_ID).setValue(PriceWholesaleCurrency.toFixed(varFractionalPartInPrice));
    }

    //4.1.IM Цена с НДС
    if (Ext.getCmp("PriceIMVAT" + Object_ID) != undefined) {
        var PriceIMVAT = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) + parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * (parseFloat(Ext.getCmp("MarkupIM" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceIMVAT" + Object_ID).setValue(PriceIMVAT.toFixed(varFractionalPartInPrice));
    }
    //4.2.Оптовая цена в рублях
    if (Ext.getCmp("PriceIMCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceIMCurrency = parseFloat(Ext.getCmp("PriceIMVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue())
        Ext.getCmp("PriceIMCurrency" + Object_ID).setValue(PriceIMCurrency.toFixed(varFractionalPartInPrice));
    }


    varPriceChange_ReadOnly = false;
}

//Поменяли "Цену без НДС"
function fn_controllerDirNomensEdit_DirNomenHistoryPurchPrice_Change(Object_ID) {
    if (varPriceChange_ReadOnly) return;
    varPriceChange_ReadOnly = true;


    //1.Цена прихода с НДС
    if (Ext.getCmp("PriceVAT" + Object_ID) != undefined) {
        var PriceVAT = parseFloat(Ext.getCmp("DirNomenHistoryPurchPrice" + Object_ID).getValue()) * (1 + parseFloat(Ext.getCmp("DirVatValue" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceVAT" + Object_ID).setValue(PriceVAT.toFixed(varFractionalPartInPrice));
    }

    //2.1.Розничная Цена с НДС
    if (Ext.getCmp("PriceRetailVAT" + Object_ID) != undefined) {
        var PriceRetailVAT = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) + parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * (parseFloat(Ext.getCmp("MarkupRetail" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceRetailVAT" + Object_ID).setValue(PriceRetailVAT.toFixed(varFractionalPartInPrice));
    }
    //2.2.Розничная цена в рублях
    if (Ext.getCmp("PriceRetailCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceRetailCurrency = parseFloat(Ext.getCmp("PriceRetailVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue())
        Ext.getCmp("PriceRetailCurrency" + Object_ID).setValue(PriceRetailCurrency.toFixed(varFractionalPartInPrice));
    }

    //3.1.Оптовая Цена с НДС
    if (Ext.getCmp("PriceWholesaleVAT" + Object_ID) != undefined) {
        var PriceWholesaleVAT = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) + parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * (parseFloat(Ext.getCmp("MarkupWholesale" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceWholesaleVAT" + Object_ID).setValue(PriceWholesaleVAT.toFixed(varFractionalPartInPrice));
    }
    //3.2.Оптовая цена в рублях
    if (Ext.getCmp("PriceWholesaleCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceWholesaleCurrency = parseFloat(Ext.getCmp("PriceWholesaleVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue())
        Ext.getCmp("PriceWholesaleCurrency" + Object_ID).setValue(PriceWholesaleCurrency.toFixed(varFractionalPartInPrice));
    }

    //4.1.Оптовая Цена с НДС
    if (Ext.getCmp("PriceIMVAT" + Object_ID) != undefined) {
        var PriceIMVAT = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) + parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * (parseFloat(Ext.getCmp("MarkupIM" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceIMVAT" + Object_ID).setValue(PriceIMVAT.toFixed(varFractionalPartInPrice));
    }
    //4.2.Оптовая цена в рублях
    if (Ext.getCmp("PriceIMCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceIMCurrency = parseFloat(Ext.getCmp("PriceIMVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue())
        Ext.getCmp("PriceIMCurrency" + Object_ID).setValue(PriceIMCurrency.toFixed(varFractionalPartInPrice));
    }

    varPriceChange_ReadOnly = false;
}

//Поменяли "Расход Розница Наценка"
function fn_controllerDirNomensEdit_MarkupRetail_Change(Object_ID) {
    if (varPriceChange_ReadOnly) return;
    varPriceChange_ReadOnly = true;

    //1.Расход.Розница.[Цена с НДС]
    var PriceRetailVAT = 0; //вынесли, для того, что бы не "съело" дробные числа, если в валюте (/31)
    if (Ext.getCmp("PriceRetailVAT" + Object_ID) != undefined) {
        PriceRetailVAT = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) + parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * (parseFloat(Ext.getCmp("MarkupRetail" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceRetailVAT" + Object_ID).setValue(PriceRetailVAT.toFixed(varFractionalPartInPrice));
    }

    //2.Расход.Розница.[Цена с НДС в рублях]
    if (Ext.getCmp("PriceRetailCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceRetailCurrency = parseFloat(PriceRetailVAT) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue());
        Ext.getCmp("PriceRetailCurrency" + Object_ID).setValue(PriceRetailCurrency.toFixed(varFractionalPartInPrice));
    }

    varPriceChange_ReadOnly = false;
}
//Поменяли "Расход Розница Наценка"
function fn_controllerDirNomensEdit_PriceRetailVAT_Change(Object_ID) {
    if (varPriceChange_ReadOnly) return;
    varPriceChange_ReadOnly = true;

    //1.Расход.Розница.[Наценка]
    if (Ext.getCmp("MarkupRetail" + Object_ID) != undefined && parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) != 0) {
        var MarkupRetail = 100 * (parseFloat(Ext.getCmp("PriceRetailVAT" + Object_ID).getValue()) - parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue())) / parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue());
        Ext.getCmp("MarkupRetail" + Object_ID).setValue(MarkupRetail.toFixed(varFractionalPartInOther));
    }

    //2.Расход.Розница.[Цена в рублях]
    if (Ext.getCmp("PriceRetailCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceRetailCurrency = parseFloat(Ext.getCmp("PriceRetailVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue());
        Ext.getCmp("PriceRetailCurrency" + Object_ID).setValue(PriceRetailCurrency.toFixed(varFractionalPartInPrice));
    }

    varPriceChange_ReadOnly = false;
}
//Поменяли "Расход Розница Наценка"
function fn_controllerDirNomensEdit_PriceRetailCurrency_Change(Object_ID) {

    if (varPriceChange_ReadOnly) return;
    varPriceChange_ReadOnly = true;
    
    //1.Расход.Розница.[Цена с НДС]
    if (Ext.getCmp("PriceRetailVAT" + Object_ID) != undefined) {
        var PriceRetailVAT = parseFloat(Ext.getCmp("PriceRetailCurrency" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue());
        Ext.getCmp("PriceRetailVAT" + Object_ID).setValue(PriceRetailVAT.toFixed(varFractionalPartInPrice));
    }

    //2.Расход.Розница.[Наценка]
    if (Ext.getCmp("MarkupRetail" + Object_ID) != undefined && parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) != 0) {
        var MarkupRetail = (PriceRetailVAT / parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) - 1) * 100;
        Ext.getCmp("MarkupRetail" + Object_ID).setValue(MarkupRetail.toFixed(varFractionalPartInOther));
    }

    varPriceChange_ReadOnly = false;
}

//Поменяли "Расход Опт Наценка"
function fn_controllerDirNomensEdit_MarkupWholesale_Change(Object_ID) {
    if (varPriceChange_ReadOnly) return;
    varPriceChange_ReadOnly = true;

    //1.Расход.Розница.[Цена с НДС]
    var PriceWholesaleVAT
    if (Ext.getCmp("PriceWholesaleVAT" + Object_ID) != undefined) {
        PriceWholesaleVAT = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) + parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * (parseFloat(Ext.getCmp("MarkupWholesale" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceWholesaleVAT" + Object_ID).setValue(PriceWholesaleVAT.toFixed(varFractionalPartInPrice));
    }

    //2.Расход.Розница.[Цена с НДС в рублях]
    if (Ext.getCmp("PriceWholesaleCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceWholesaleCurrency = parseFloat(PriceWholesaleVAT) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue());
        Ext.getCmp("PriceWholesaleCurrency" + Object_ID).setValue(PriceWholesaleCurrency.toFixed(varFractionalPartInPrice));
    }

    varPriceChange_ReadOnly = false;
}
//Поменяли "Расход Опт Наценка"
function fn_controllerDirNomensEdit_PriceWholesaleVAT_Change(Object_ID) {
    if (varPriceChange_ReadOnly) return;
    varPriceChange_ReadOnly = true;

    //1.Расход.Розница.[Наценка]
    if (Ext.getCmp("MarkupWholesale" + Object_ID) != undefined && parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) != 0) {
        var MarkupWholesale = 100 * (parseFloat(Ext.getCmp("PriceWholesaleVAT" + Object_ID).getValue()) - parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue())) / parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue());
        Ext.getCmp("MarkupWholesale" + Object_ID).setValue(MarkupWholesale.toFixed(varFractionalPartInOther));
    }

    //2.Расход.Розница.[Цена в рублях]
    if (Ext.getCmp("PriceWholesaleCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceWholesaleCurrency = parseFloat(Ext.getCmp("PriceWholesaleVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue());
        Ext.getCmp("PriceWholesaleCurrency" + Object_ID).setValue(PriceWholesaleCurrency.toFixed(varFractionalPartInPrice));
    }

    varPriceChange_ReadOnly = false;
}
//Поменяли "Расход Опт Наценка"
function fn_controllerDirNomensEdit_PriceWholesaleCurrency_Change(Object_ID) {
    if (varPriceChange_ReadOnly) return;
    varPriceChange_ReadOnly = true;

    //1.Расход.Розница.[Цена с НДС]
    if (Ext.getCmp("PriceWholesaleVAT" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) != 0) {
        var PriceWholesaleVAT = parseFloat(Ext.getCmp("PriceWholesaleCurrency" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue());
        Ext.getCmp("PriceWholesaleVAT" + Object_ID).setValue(PriceWholesaleVAT.toFixed(varFractionalPartInPrice));
    }

    //2.Расход.Розница.[Наценка]
    if (Ext.getCmp("MarkupWholesale" + Object_ID) != undefined && parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) != 0) {
        var MarkupWholesale = (PriceWholesaleVAT / parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) - 1) * 100;
        Ext.getCmp("MarkupWholesale" + Object_ID).setValue(MarkupWholesale.toFixed(varFractionalPartInOther));
    }

    varPriceChange_ReadOnly = false;
}

//Поменяли "Расход IM Наценка"
function fn_controllerDirNomensEdit_MarkupIM_Change(Object_ID) {
    if (varPriceChange_ReadOnly) return;
    varPriceChange_ReadOnly = true;

    //1.Расход.Розница.[Цена с НДС]
    var PriceIMVAT
    if (Ext.getCmp("PriceIMVAT" + Object_ID) != undefined) {
        PriceIMVAT = parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) + parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) * (parseFloat(Ext.getCmp("MarkupIM" + Object_ID).getValue()) / 100);
        Ext.getCmp("PriceIMVAT" + Object_ID).setValue(PriceIMVAT.toFixed(varFractionalPartInPrice));
    }

    //2.Расход.Розница.[Цена с НДС в рублях]
    if (Ext.getCmp("PriceIMCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceIMCurrency = parseFloat(PriceIMVAT) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue());
        Ext.getCmp("PriceIMCurrency" + Object_ID).setValue(PriceIMCurrency.toFixed(varFractionalPartInPrice));
    }

    varPriceChange_ReadOnly = false;
}
//Поменяли "Расход IM Наценка"
function fn_controllerDirNomensEdit_PriceIMVAT_Change(Object_ID) {
    if (varPriceChange_ReadOnly) return;
    varPriceChange_ReadOnly = true;

    //1.Расход.Розница.[Наценка]
    if (Ext.getCmp("MarkupIM" + Object_ID) != undefined && parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) != 0) {
        var MarkupIM = 100 * (parseFloat(Ext.getCmp("PriceIMVAT" + Object_ID).getValue()) - parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue())) / parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue());
        Ext.getCmp("MarkupIM" + Object_ID).setValue(MarkupIM.toFixed(varFractionalPartInOther));
    }

    //2.Расход.Розница.[Цена в рублях]
    if (Ext.getCmp("PriceIMCurrency" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue()) != 0) {
        var PriceIMCurrency = parseFloat(Ext.getCmp("PriceIMVAT" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue());
        Ext.getCmp("PriceIMCurrency" + Object_ID).setValue(PriceIMCurrency.toFixed(varFractionalPartInPrice));
    }

    varPriceChange_ReadOnly = false;
}
//Поменяли "Расход IM Наценка"
function fn_controllerDirNomensEdit_PriceIMCurrency_Change(Object_ID) {
    if (varPriceChange_ReadOnly) return;
    varPriceChange_ReadOnly = true;

    //1.Расход.Розница.[Цена с НДС]
    if (Ext.getCmp("PriceIMVAT" + Object_ID) != undefined && parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) != 0) {
        var PriceIMVAT = parseFloat(Ext.getCmp("PriceIMCurrency" + Object_ID).getValue()) / parseFloat(Ext.getCmp("DirCurrencyRate" + Object_ID).getValue()) * parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + Object_ID).getValue());
        Ext.getCmp("PriceIMVAT" + Object_ID).setValue(PriceIMVAT.toFixed(varFractionalPartInPrice));
    }

    //2.Расход.Розница.[Наценка]
    if (Ext.getCmp("MarkupIM" + Object_ID) != undefined && parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) != 0) {
        var MarkupIM = (PriceIMVAT / parseFloat(Ext.getCmp("PriceVAT" + Object_ID).getValue()) - 1) * 100;
        Ext.getCmp("MarkupIM" + Object_ID).setValue(MarkupIM.toFixed(varFractionalPartInOther));
    }

    varPriceChange_ReadOnly = false;
}




/*
    Вызов формы редактирования "viewDirNomensEdit" (PartionnyAccount.view.Object/Dir/DirNomens/viewDirNomensEdit)
*/
function fn_controllerDirNomens_onTree_Edit(pObjectName, UO_idCall, New_Edit, Sub) {

    ObjectID++;
    _ObjectID = ObjectID;

    //Это данные из вызвашей таблицы, например Контрагент, которого редактируем (получить его ID-шник)
    //Создаём копию данных "Дата", т.к. если Панель, но вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
    var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

    //Store Combo "storeDirNomenCategoriesGrid"
    var storeDirNomenCategoriesGrid = Ext.create("store.storeDirNomenCategoriesGrid"); storeDirNomenCategoriesGrid.setData([], false);
    storeDirNomenCategoriesGrid.load({ waitMsg: lanLoading });

    // === Формируем и показываем окно ===
    var widgetX = Ext.create("widget." + pObjectName, {
        id: pObjectName + _ObjectID,
        UO_id: ObjectID,
        UO_idMain: "win_" + pObjectName + _ObjectID,
        UO_idCall: UO_idCall, //"tree_" + aButton.UO_id,
        modal: true,

        storeDirNomenCategoriesGrid: storeDirNomenCategoriesGrid
    });


    //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
    //Если панель, но удаляется текущий виджет
    ObjectShow_Win(widgetX, pObjectName, _ObjectID, true);


    //Лоадер
    var loadingMask = new Ext.LoadMask({
        msg: 'Please wait...',
        target: widgetX //Ext.getCmp("tree_" + aButton.UO_id)
    });
    loadingMask.show();

    storeDirNomenCategoriesGrid.on('load', function () {
        loadingMask.hide();

        //Если редактировать, то: Загрузка данных в Форму "widgetXPanel"
        if (New_Edit >= 2) {
            var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);

            //Если форма уже загружена выходим!
            if (widgetXForm.UO_Loaded) return;

            widgetXForm.load({
                method: "GET",
                timeout: varTimeOutDefault,
                waitMsg: lanLoading,
                url: HTTP_DirNomensTree + IdcallModelData.id + "/", //"tree_" + aButton.UO_id
                success: function (result) {
                    widgetXForm.UO_Loaded = true;
                    //Если копия
                    if (New_Edit == 3) {

                    }

                    if (Ext.getCmp("DirNomenPhoto" + widgetX.UO_id).getValue() != "") { Ext.getCmp("DirNomenPhotoShow" + widgetX.UO_id).setSrc("/Users/File/Images/DirNomenPhoto/" + Ext.getCmp("DirNomenPhoto" + widgetX.UO_id).getValue()); }
                },
                failure: function (form, action) {
                    //widgetX.close();
                    Ext.getCmp("win_" + widgetX.id).close();
                    funPanelSubmitFailure(form, action);

                    //Фокус на открывшийся Виджет
                    widgetX.focus();
                }
            });
        }
        else {
            Ext.getCmp("Sub" + widgetX.UO_id).setValue(Sub);
        }

    });
}



/*
    Проставить наценки из Настроек
    (Если наценки отрицательные, то ставим их из Настроек)
*/
function funMarkupSet(id, sMsg) {

    if (Ext.getCmp("MarkupRetail" + id) == undefined) { return; }
    if (sMsg == undefined) { sMsg = ""; }

    if (
        parseFloat(Ext.getCmp("MarkupRetail" + id).getValue()) <= 0 || isNaN(parseFloat(Ext.getCmp("MarkupRetail" + id).getValue())) ||
        parseFloat(Ext.getCmp("MarkupWholesale" + id).getValue()) <= 0 || isNaN(parseFloat(Ext.getCmp("MarkupWholesale" + id).getValue())) ||
        parseFloat(Ext.getCmp("MarkupIM" + id).getValue()) <= 0 || isNaN(parseFloat(Ext.getCmp("MarkupIM" + id).getValue()))
    ) {

        Ext.MessageBox.show({
            title: lanOrgName,
            msg: sMsg + "Наценки не правильные исправить (поменять их на наценки из настроек)?",
            icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {
                    if (parseFloat(Ext.getCmp("MarkupRetail" + id).getValue()) <= 0 || isNaN(parseFloat(Ext.getCmp("MarkupRetail" + id).getValue())))
                        Ext.getCmp("MarkupRetail" + id).setValue(varMarkupRetail);

                    if (parseFloat(Ext.getCmp("MarkupWholesale" + id).getValue()) <= 0 || isNaN(parseFloat(Ext.getCmp("MarkupWholesale" + id).getValue())))
                        Ext.getCmp("MarkupWholesale" + id).setValue(varMarkupWholesale);

                    if (parseFloat(Ext.getCmp("MarkupIM" + id).getValue()) <= 0 || isNaN(parseFloat(Ext.getCmp("MarkupIM" + id).getValue())))
                        Ext.getCmp("MarkupIM" + id).setValue(varMarkupIM);
                }
            }
        });

    }
}



/*
    Переоткрытие ветки дерева, которое редактировали
    Используется в редактировании Документа (слева Дерево с таваром)
*/

//Делает выбранный Нод активный: переводит на него каретку (курсор).
function fun_ReopenTree_NodeActive(_id, treePanel, storeNomenTree, ID, ID0, ID1, ID2, ID3, ID4) {

    var node = storeNomenTree.getNodeById(ID);
    if (node != null) { treePanel.getSelectionModel().select(node, true); fun_DirNomenPatchFull_RemParties1(_id, node); }
    else {
        var node = storeNomenTree.getNodeById(ID0);
        if (node != null) { treePanel.getSelectionModel().select(node, true); fun_DirNomenPatchFull_RemParties1(_id, node); }
        else {
            var node = storeNomenTree.getNodeById(ID1);
            if (node != null) { treePanel.getSelectionModel().select(node, true); fun_DirNomenPatchFull_RemParties1(_id, node); }
            else {
                var node = storeNomenTree.getNodeById(ID2);
                if (node != null) { treePanel.getSelectionModel().select(node, true); fun_DirNomenPatchFull_RemParties1(_id, node); }
                else {
                    var node = storeNomenTree.getNodeById(ID3);
                    if (node != null) { treePanel.getSelectionModel().select(node, true); fun_DirNomenPatchFull_RemParties1(_id, node); }
                    else {
                        var node = storeNomenTree.getNodeById(ID4);
                        if (node != null) { treePanel.getSelectionModel().select(node, true); fun_DirNomenPatchFull_RemParties1(_id, node); }
                        else {
                            //...
                        }
                    }
                }
            }
        }
    }

}

//Открыто на одной форме
// !!! БЛЯДЬ !!! НЕ трогать этот метод !!! Он вызывается во всех документах и некоторых справочниках !!!
function fun_ReopenTree_1(_id, UO_idMain, UO_idCall, result_data) {
    if (Ext.getCmp(UO_idMain) != undefined) { Ext.getCmp(UO_idMain).close(); }

    var ID = result_data.ID
    var ID0 = result_data.ID0
    var ID1 = result_data.ID1
    var ID2 = result_data.ID2
    var ID3 = result_data.ID3
    var ID4 = result_data.ID4

    var treePanel = Ext.getCmp(UO_idCall);

    var storeNomenTree = treePanel.getStore();
    storeNomenTree.load();

    storeNomenTree.on('load', function () {

        //Делает выбранный Нод активный: переводит на него каретку (курсор).
        fun_ReopenTree_NodeActive(_id, treePanel, storeNomenTree, ID, ID0, ID1, ID2, ID3, ID4);

        //4-0
        if (ID4 != 0) {
            var node = storeNomenTree.getNodeById(ID4); ID4 = 0;
            if (node != null) treePanel.expandPath(node.getPath());
        }
        else
            //3-0
            if (ID3 != 0) {
                var node = storeNomenTree.getNodeById(ID3); ID3 = 0;
                if (node != null) treePanel.expandPath(node.getPath());
            }
            else
                //2-0
                if (ID2 != 0) {
                    var node = storeNomenTree.getNodeById(ID2); ID2 = 0;
                    if (node != null) treePanel.expandPath(node.getPath());
                }
                else
                    //1-0
                    if (ID1 != 0) {
                        var node = storeNomenTree.getNodeById(ID1); ID1 = 0;
                        if (node != null) treePanel.expandPath(node.getPath());
                    }
                    else
                        //0
                        if (ID0 != 0) {
                            var node = storeNomenTree.getNodeById(ID0); ID0 = 0;
                            if (node != null) treePanel.expandPath(node.getPath());
                        }

    });
}

//Открыто в отдельном окне
function fun_ReopenTree_2(UO_idMain, UO_idCall, result_data) {
    if (Ext.getCmp(UO_idMain) != undefined) { Ext.getCmp(UO_idMain).close(); }

    var ID = result_data.ID
    var ID0 = result_data.ID0
    var ID1 = result_data.ID1
    var ID2 = result_data.ID2
    var ID3 = result_data.ID3
    var ID4 = result_data.ID4

    var treePanel = Ext.getCmp(UO_idCall);

    var storeNomenTree = treePanel.getStore();
    storeNomenTree.load();

    storeNomenTree.on('load', function () {

        //Делает выбранный Нод активный: переводит на него каретку (курсор).
        fun_ReopenTree_NodeActive(treePanel, storeNomenTree, ID, ID0, ID1, ID2, ID3, ID4);

        //4-0
        if (ID4 != 0) {
            var node = storeNomenTree.getNodeById(ID4); ID4 = 0;
            if (node != null) treePanel.expandPath(node.getPath());
        }
        else
            //3-0
            if (ID3 != 0) {
                var node = storeNomenTree.getNodeById(ID3); ID3 = 0;
                if (node != null) treePanel.expandPath(node.getPath());
            }
            else
                //2-0
                if (ID2 != 0) {
                    var node = storeNomenTree.getNodeById(ID2); ID2 = 0;
                    if (node != null) treePanel.expandPath(node.getPath());
                }
                else
                    //1-0
                    if (ID1 != 0) {
                        var node = storeNomenTree.getNodeById(ID1); ID1 = 0;
                        if (node != null) treePanel.expandPath(node.getPath());
                    }
                    else
                        //0
                        if (ID0 != 0) {
                            var node = storeNomenTree.getNodeById(ID0); ID0 = 0;
                            if (node != null) treePanel.expandPath(node.getPath());
                        }

    });
}



/*
    Вернуть выбранный Нод
*/
function funReturnNode(id) {
    var panel = Ext.getCmp("tree_" + id),
        sm = panel.getSelectionModel(),
        node;

    if (sm.hasSelection()) {
        node = sm.getSelection()[0];

        return node;
    }
}

/*
    Вернуть выбранный Нод
*/
function fun_DirPriceTypeID_ChangePrice(UO_GridRecord, DirPriceTypeID) {
    switch (DirPriceTypeID) {

        case 1: {
            UO_GridRecord.data.PriceCurrency = UO_GridRecord.data.PriceRetailCurrency;
            UO_GridRecord.data.PriceVAT = UO_GridRecord.data.PriceRetailVAT;
        };
            break;

        case 2: {
            UO_GridRecord.data.PriceCurrency = UO_GridRecord.data.PriceWholesaleCurrency;
            UO_GridRecord.data.PriceVAT = UO_GridRecord.data.PriceWholesaleVAT;
        };
            break;

        case 3: {
            UO_GridRecord.data.PriceCurrency = UO_GridRecord.data.PriceIMCurrency;
            UO_GridRecord.data.PriceVAT = UO_GridRecord.data.PriceIMVAT;
        };
            break;

    }

    return UO_GridRecord;

}


/*
    Подсчет сум в документах
    1. Если есть поля "Discount", то это: Списание, Инвентаризация
    2. Если нет поля "Discount", то это : Приход, Расход, ...
*/
function fun_DocX_RecalculationSums(id, ShowMsg, funN) {
    if (Ext.getCmp("Discount" + id)) {
        fun_DocExt_RecalculationSums(id, ShowMsg)
    }
    else if (funN == "fun_DocInt_RecalculationSums2") {
        fun_DocInt_RecalculationSums2(id, ShowMsg)
    }
    else {
        fun_DocInt_RecalculationSums(id, ShowMsg)
    }
}
//Внешние документы: Приход, Расход
function fun_DocExt_RecalculationSums(id, ShowMsg) {
    //Стор для "Табличной части"
    var storeGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeGrid; //storeDocPurchTabsGrid; //можно так: Ext.getCmp("viewDocPurchesEdit" + id).storeDocPurchTabsGrid;
    //Скидка, если undefined, isNaN или "", то ставим её == 0
    if (isNaN(parseFloat(Ext.getCmp("Discount" + id).getValue()))) { Ext.getCmp("Discount" + id).setValue(0); }
    //Ставка Налога
    var DirVatValue = 0;
    var Match = true; //совпадение ставок НДС для всех Номенклатур.
    var DirVatValue = parseFloat(Ext.getCmp("DirVatValue" + id).getValue());

    var SumOfVATCurrency = 0; //var SumVATCurrency = 0;
    for (var i = 0; i < storeGrid.data.items.length; i++) {
        //Номенклатура + проверка на совпадение ставок НДС для всех Номенклатур.
        //Сумма спецификации
        var pSumOfVATCurrency = parseFloat(storeGrid.data.items[i].data.Quantity * storeGrid.data.items[i].data.PriceCurrency);
        //Меняем сумму спецификации (если добавили новую позицию)
        storeGrid.data.items[i].data.SUMPurchPriceVATCurrency = pSumOfVATCurrency.toFixed(varFractionalPartInSum);

        SumOfVATCurrency += parseFloat(pSumOfVATCurrency);
        //SumVATCurrency += parseFloat((parseFloat(pSumOfVATCurrency) - parseFloat(pSumOfVATCurrency) / (1 + (DirVatValue) / 100)));
    }

    //Рефреш грида, если изменилась сумма спецификации (если добавили новую позицию)
    Ext.getCmp("grid_" + id).getView().refresh();
    //Сумма с НДС
    SumOfVATCurrency = parseFloat(parseFloat(SumOfVATCurrency) - parseFloat(parseFloat(SumOfVATCurrency) * parseFloat(Ext.getCmp("Discount" + id).getValue()) / 100));
    Ext.getCmp('SumOfVATCurrency' + id).setValue(SumOfVATCurrency.toFixed(varFractionalPartInSum)); //Подсчёт суммы SUM(Учётная цена * К-во)
    //Сумма НДС
    //parseFloat(parseFloat(SumVATCurrency) - parseFloat(parseFloat(SumVATCurrency) * parseFloat(Ext.getCmp("Discount" + id).getValue()) / 100));
    SumVATCurrency = parseFloat((parseFloat(SumOfVATCurrency) - parseFloat(SumOfVATCurrency) / (1 + (DirVatValue) / 100)));
    Ext.getCmp('SumVATCurrency' + id).setValue(SumVATCurrency.toFixed(varFractionalPartInSum));     //Подсчёт суммы SUM(НДС)
    //Надо доплатить
    var HavePay = parseFloat(parseFloat(Ext.getCmp("SumOfVATCurrency" + id).getValue()) - parseFloat(Ext.getCmp("Payment" + id).getValue()));
    Ext.getCmp("HavePay" + id).setValue(HavePay.toFixed(varFractionalPartInSum));

    if (ShowMsg) {
        if (DirVatValue_Collection != "") {
            if (Match) { Ext.Msg.alert(lanOrgName, txtVatChanges + "<b>" + DirVatValue_Collection + " %</b>" + txtVatRecalc); } //Сообщение о пересчёте НДС
            else { Ext.Msg.alert(lanOrgName, txtVatNotMatch + "<BR>" + txtVatChanges + "<b>" + DirVatValue_Collection + " %</b>" + txtVatRecalc); } //Сообщение о пересчёте НДС
        }
    }
};
//Внутренние документы: Списание
function fun_DocInt_RecalculationSums(id, ShowMsg) {
    
    //Стор для "Табличной части"
    var storeGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeGrid; //storeDocInventoryTabsGrid; //можно так: Ext.getCmp("viewDocInventoriesEdit" + id).storeDocInventoryTabsGrid;
    var Match = true; //совпадение ставок НДС для всех Номенклатур.

    var SumOfVATCurrency = 0;
    for (var i = 0; i < storeGrid.data.items.length; i++) {
        //Номенклатура + проверка на совпадение ставок НДС для всех Номенклатур.
        //Сумма спецификации
        var pSumOfVATCurrency = parseFloat(storeGrid.data.items[i].data.Quantity * storeGrid.data.items[i].data.PriceCurrency);
        //Меняем сумму спецификации (если добавили новую позицию)
        storeGrid.data.items[i].data.SUMPurchPriceVATCurrency = pSumOfVATCurrency.toFixed(varFractionalPartInSum);
        SumOfVATCurrency += parseFloat(pSumOfVATCurrency);
    }

    //Рефреш грида, если изменилась сумма спецификации (если добавили новую позицию)
    Ext.getCmp("grid_" + id).getView().refresh();
    //Сумма с НДС
    Ext.getCmp('SumOfVATCurrency' + id).setValue(SumOfVATCurrency.toFixed(varFractionalPartInSum)); //Подсчёт суммы SUM(Учётная цена * К-во)
};
//Внутренние документы: Инвентаризация
function fun_DocInt_RecalculationSums2(id, ShowMsg) {
    
    //Стор для "Табличной части"
    var storeGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeGrid; //storeDocInventoryTabsGrid; //можно так: Ext.getCmp("viewDocInventoriesEdit" + id).storeDocInventoryTabsGrid;
    var Match = true; //совпадение ставок НДС для всех Номенклатур.

    var SumOfVATCurrency = 0;
    for (var i = 0; i < storeGrid.data.items.length; i++) {
        //Номенклатура + проверка на совпадение ставок НДС для всех Номенклатур.
        //Сумма спецификации
        var pSumOfVATCurrency = parseFloat(storeGrid.data.items[i].data.Quantity_WriteOff * storeGrid.data.items[i].data.PriceCurrency);
        //Меняем сумму спецификации (если добавили новую позицию)
        storeGrid.data.items[i].data.SUMPurchPriceVATCurrency = pSumOfVATCurrency.toFixed(varFractionalPartInSum);
        SumOfVATCurrency += parseFloat(pSumOfVATCurrency);
    }

    //Рефреш грида, если изменилась сумма спецификации (если добавили новую позицию)
    Ext.getCmp("grid_" + id).getView().refresh();
    //Сумма с НДС
    Ext.getCmp('SumOfVATCurrency' + id).setValue(SumOfVATCurrency.toFixed(varFractionalPartInSum)); //Подсчёт суммы SUM(Учётная цена * К-во)
};



/*
    Очистить КомбоБокс, если открыт ввод текста (поиск в комбо)
    Может быть так, что пользователь ввёл текст в КомбоБокс, которого нет в списке, тогда надо очистить Комбо!
*/
function fun_ComboBox_Search_Correct(fieldId) {
    //Может быть так, что пользователь ввёл текст в КомбоБокс, которого нет в списке, тогда надо очистить Комбо!
    var combobox = Ext.getCmp(fieldId);
    if (!combobox) return;
    var v = combobox.getValue();
    var record = combobox.findRecord(combobox.valueField || combobox.displayField, v);
    var index = combobox.store.indexOf(record);
    if (index < 0) { combobox.setValue(null); }
}



/*
    Поиск по Артикулу: DirNomens
*/
function fun_onTriggerSearchTreeClick_Search(aButton, bReset) {
    if (Ext.getCmp("TriggerSearchTree" + aButton.UO_id).getValue() == "") return;
    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).disable(); //Кнопку поиска делаем не активной

    var SearchType = Ext.getCmp("SearchType" + aButton.UO_id).getValue();
    var TriggerSearchTree = Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value;
    if (SearchType < 1000) {

        if ((SearchType == 1 || SearchType == 2) && !isNumber(TriggerSearchTree)) { Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable(); alert("Для поиска по коду товара надо указывать число!"); return; }

        var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: Ext.getCmp("tree_" + aButton.UO_id) });
        loadingMask.show();

        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            //                                        id,                                        iPriznak
            url: HTTP_DirNomens + Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value + "/" + SearchType + "/",
            method: 'GET',
            success: function (result) {
                loadingMask.hide();
                var sData = Ext.decode(result.responseText);
                if (sData.success == true) {
                    var sData = Ext.decode(result.responseText);

                    if (sData.data == -1) {
                        //Ext.Msg.alert(lanOrgName, "Ничего не найдено!");

                        var treePanel = Ext.getCmp("tree_" + aButton.UO_id);
                        var storeNomenTree = treePanel.getStore();

                        var ID = Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value;
                        var node = storeNomenTree.getNodeById(ID);
                        if (node != null) { treePanel.getSelectionModel().select(node, true); fun_DirNomenPatchFull_RemParties1(aButton.UO_id, node); }
                        else { Ext.Msg.alert(lanOrgName, "Ничего не найдено!");}

                        return;
                    }

                    fun_ReopenTree_1(aButton.UO_id, undefined, "tree_" + aButton.UO_id, sData.data);

                    if (bReset) {
                        //Чистим форму - это Обязательно! Но оставляем строку поиска
                        var TriggerSearchTree = Ext.getCmp("TriggerSearchTree" + aButton.UO_id).getValue();
                        //Очистить форму - Глючит!!!
                        //if (Ext.getCmp("form_" + aButton.UO_id) != undefined) { Ext.getCmp("form_" + aButton.UO_id).reset(); }
                        //Поиск позвращаем полюбому
                        Ext.getCmp("TriggerSearchTree" + aButton.UO_id).setValue(TriggerSearchTree);
                    }


                    //Заполняем Категорию товара  и выводим партии товара
                    //fun_DirNomenPatchFull_RemParties(aButton.UO_id);


                } else {
                    Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK })
                }
            },
            failure: function (form, action) {
                loadingMask.hide();
                //Права.
                /*if (action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
                Ext.Msg.alert(lanOrgName, txtMsg008 + action.result.data);*/
                funPanelSubmitFailure(form, action);
            }
        });

    }
    else {

        //Запустили из справочника "Товар" (там нет партий)
        if (Ext.getCmp("gridParty_" + aButton.UO_id) == undefined) return;

        //Если панель скрыта, то показывать "партии товара"
        if (Ext.getCmp("gridParty_" + aButton.UO_id).collapsed) {
            Ext.getCmp("gridParty_" + ObjectID).expand(Ext.Component.DIRECTION_NORTH, true);
        }


        //Получаем storeRemPartiesGrid и делаем load()
        var DirWarehouseID;
        if (Ext.getCmp("DirWarehouseID" + aButton.UO_id) == undefined) DirWarehouseID = Ext.getCmp("DirWarehouseIDFrom" + aButton.UO_id).getValue();
        else DirWarehouseID = Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue();

        var storeRemPartiesGrid = Ext.getCmp("gridParty_" + aButton.UO_id).getStore();
        storeRemPartiesGrid.proxy.url = HTTP_RemParties + "?parSearch=" + Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value + "&" + "DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue() + "&DirWarehouseID=" + DirWarehouseID + "&DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + aButton.UO_id).getValue(), "Y-m-d")
        storeRemPartiesGrid.load();

        storeRemPartiesGrid.on('load', function () {

        });

    }

    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable(); //Кнопку поиска делаем активной
}

/*
    Поиск по Артикулу: DirServiceNomens
*/
function fun_onTriggerSearchTreeClick_Search_Servise(aButton, bReset) {
    if (Ext.getCmp("TriggerSearchTree" + aButton.UO_id).getValue() == "") return;
    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).disable(); //Кнопку поиска делаем не активной

    var SearchType = Ext.getCmp("SearchType" + aButton.UO_id).getValue();
    var TriggerSearchTree = Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value;
    if (SearchType < 1000) {

        if ((SearchType == 1 || SearchType == 2) && !isNumber(TriggerSearchTree)) { Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable(); alert("Для поиска по коду товара надо указывать число!"); return; }

        var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: Ext.getCmp("tree_" + aButton.UO_id) });
        loadingMask.show();

        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            //                                        id,                                        iPriznak
            url: HTTP_DirServiceNomens + Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value + "/" + SearchType + "/",
            method: 'GET',
            success: function (result) {
                loadingMask.hide();
                var sData = Ext.decode(result.responseText);
                if (sData.success == true) {
                    var sData = Ext.decode(result.responseText);

                    if (sData.data == -1) {
                        Ext.Msg.alert(lanOrgName, "Ничего не найдено!");
                        return;
                    }

                    fun_ReopenTree_1(aButton.UO_id, undefined, "tree_" + aButton.UO_id, sData.data);

                    if (bReset) {
                        //Чистим форму - это Обязательно! Но оставляем строку поиска
                        var TriggerSearchTree = Ext.getCmp("TriggerSearchTree" + aButton.UO_id).getValue();
                        //Очистить форму
                        if (Ext.getCmp("form_" + aButton.UO_id) != undefined) { Ext.getCmp("form_" + aButton.UO_id).reset(); }
                        //Поиск позвращаем полюбому
                        Ext.getCmp("TriggerSearchTree" + aButton.UO_id).setValue(TriggerSearchTree);
                    }


                    //Заполняем Категорию товара  и выводим партии товара
                    //fun_DirServiceNomenPatchFull_RemParties(aButton.UO_id);


                } else {
                    Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK })
                }
            },
            failure: function (form, action) {
                loadingMask.hide();
                //Права.
                /*if (action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
                Ext.Msg.alert(lanOrgName, txtMsg008 + action.result.data);*/
                funPanelSubmitFailure(form, action);
            }
        });

    }
    else {

        //Запустили из справочника "Товар" (там нет партий)
        if (Ext.getCmp("gridParty_" + aButton.UO_id) == undefined) return;

        //Если панель скрыта, то показывать "партии товара"
        if (Ext.getCmp("gridParty_" + aButton.UO_id).collapsed) {
            Ext.getCmp("gridParty_" + ObjectID).expand(Ext.Component.DIRECTION_NORTH, true);
        }


        //Получаем storeRemPartiesGrid и делаем load()
        var DirWarehouseID;
        if (Ext.getCmp("DirWarehouseID" + aButton.UO_id) == undefined) DirWarehouseID = Ext.getCmp("DirWarehouseIDFrom" + aButton.UO_id).getValue();
        else DirWarehouseID = Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue();

        var storeRemPartiesGrid = Ext.getCmp("gridParty_" + aButton.UO_id).getStore();
        storeRemPartiesGrid.proxy.url = HTTP_RemParties + "?parSearch=" + Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value + "&" + "DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue() + "&DirWarehouseID=" + DirWarehouseID + "&DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + aButton.UO_id).getValue(), "Y-m-d")
        storeRemPartiesGrid.load();

        storeRemPartiesGrid.on('load', function () {

        });

    }

    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable(); //Кнопку поиска делаем активной
}

// *** RemParties ***
//1. Функциии для получения значения категории товара "DirNomenPatchFull" и списка партий товара "RemParties"
//1.1. Функция вызываемая из фнкции "fun_onTriggerSearchTreeClick_Search", при поиске товара по коду.
function fun_DirNomenPatchFull_RemParties1(id, node) {
    //Может быть и такое. Причина: 
    //id - открівшейся формі, а виджеты к которым обращаемся на форме UO_idCall
    if (Ext.getCmp("TriggerSearchTree" + id) == undefined) return;

    if (node.data.id == parseInt(Ext.getCmp("TriggerSearchTree" + id).getValue())) {

        //Полученная ветки
        var rec;
        var sm = Ext.getCmp("tree_" + id).getSelectionModel().getSelection();
        if (sm != undefined) rec = sm[0];
        else { return; }

        Ext.getCmp("tree_" + id).getView().focusRow(node);

        fun_DirNomenPatchFull_RemParties2(id, rec);
    }
}
//1.2. Функция вызываемая из контроллеров, при клике на Дерево товара
function fun_DirNomenPatchFull_RemParties2(id, rec) {
    //var id = view.grid.UO_id;
    //Полный путь от Группы к выбранному объкту
    Ext.getCmp("DirNomenPatchFull" + id).setValue(rec.get('DirNomenPatchFull'));

    //Нет списка партий у документа: например Акт выполненных работ
    if (Ext.getCmp("gridParty_" + id) == undefined) return;

    var storeGrid = Ext.getCmp("gridParty_" + id).getStore();
    //Если панель скрыта, то не показывать "партии товара"
    if (Ext.getCmp("gridParty_" + id).collapsed) { storeGrid.setData([], false); return; }
    //Выбрана ли Организация (если новая накладная, то может быть и не выбрана Организация)
    if (Ext.getCmp("DirContractorIDOrg" + id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Организацию (так как партии привязаны к Организации)!"); return; }

    //Склад
    //1. Если это документ "Перемещение"
    var DirWarehouseID = Ext.getCmp("DirWarehouseID" + id);
    if (DirWarehouseID == undefined) DirWarehouseID = Ext.getCmp("DirWarehouseIDFrom" + id);
    //2. Выбран ли Склад (если новая накладная, то может быть и не выбран Склад)
    if (DirWarehouseID == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад (так как партии привязаны к Складу)!"); return; }




    //Получаем storeGrid и делаем load() *** *** *** *** *** *** *** ***

    //Если Розница, то DocDateS и DocDatePo
    var DocDateX;
    if (Ext.getCmp("DocDate" + id) == undefined) {
        DocDateX = "&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + id).getValue(), "Y-m-d") + "&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + id).getValue(), "Y-m-d");
    }
    else {
        DocDateX = "&DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + id).getValue(), "Y-m-d")
    }

    Ext.getCmp("tree_" + id).setDisabled(true);

    storeGrid.proxy.url =
        HTTP_RemParties +
        "?DirNomenID=" + rec.get('id') +
        "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + id).getValue() +
        "&DirWarehouseID=" + DirWarehouseID.getValue() +
        DocDateX;

    storeGrid.load();
    Ext.getCmp("tree_" + id).setDisabled(false);
}
//1.2. Функция вызываемая из контроллеров, при клике на Дерево товара (только для Розницы)
function fun_DirNomenPatchFull_RemParties3(id, rec) {
    //var id = view.grid.UO_id;
    //Полный путь от Группы к выбранному объкту
    Ext.getCmp("DirNomenPatchFull" + id).setValue(rec.get('DirNomenPatchFull'));

    //Нет списка партий у документа: например Акт выполненных работ
    if (Ext.getCmp("gridParty_" + id) == undefined) return;

    var storeGrid = Ext.getCmp("gridParty_" + id).getStore();
    //Если панель скрыта, то не показывать "партии товара"
    if (Ext.getCmp("gridParty_" + id).collapsed) { storeGrid.setData([], false); return; }

    //Выбрана ли Организация (если новая накладная, то может быть и не выбрана Организация)
    var DirContractorIDOrg = Ext.getCmp("DirContractorIDOrg" + id);
    if (DirContractorIDOrg == undefined || DirContractorIDOrg == null || DirContractorIDOrg.getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Организацию (так как партии привязаны к Организации)!"); return; }

    //Склад
    //1. Если это документ "Перемещение"
    var DirWarehouseID = Ext.getCmp("DirWarehouseID" + id);
    if (DirWarehouseID == undefined) DirWarehouseID = Ext.getCmp("DirWarehouseIDFrom" + id);
    //2. Выбран ли Склад (если новая накладная, то может быть и не выбран Склад)
    if (DirWarehouseID == undefined || DirWarehouseID == null || DirWarehouseID.getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад (так как партии привязаны к Складу)!"); return; }

    //Получаем storeGrid и делаем load()
    Ext.getCmp("tree_" + id).setDisabled(true);

    storeGrid.proxy.url =
        HTTP_RemParties +
        "?DirNomenID=" + rec.get('id') +
        "&DirContractorIDOrg=" + DirContractorIDOrg.getValue() +
        "&DirWarehouseID=" + DirWarehouseID.getValue() +
        "&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + id).getValue(), "Y-m-d") +
        "&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + id).getValue(), "Y-m-d");

    storeGrid.load();
    Ext.getCmp("tree_" + id).setDisabled(false);
}

// *** Rem2Parties ***
//1. Функциии для получения значения категории товара "DirNomenPatchFull" и списка партий товара "Rem2Parties"
//1.1. Функция вызываемая из фнкции "fun_onTriggerSearchTreeClick_Search", при поиске товара по коду.
function fun_DirNomenPatchFull_Rem2Parties1(id, node) {
    //Может быть и такое. Причина: 
    //id - открівшейся формі, а виджеты к которым обращаемся на форме UO_idCall
    if (Ext.getCmp("TriggerSearchTree" + id) == undefined) return;

    if (node.data.id == parseInt(Ext.getCmp("TriggerSearchTree" + id).getValue())) {

        //Полученная ветки
        var rec;
        var sm = Ext.getCmp("tree_" + id).getSelectionModel().getSelection();
        if (sm != undefined) rec = sm[0];
        else { return; }

        Ext.getCmp("tree_" + id).getView().focusRow(node);

        fun_DirNomenPatchFull_Rem2Parties2(id, rec);
    }
}
//1.2. Функция вызываемая из контроллеров, при клике на Дерево товара
function fun_DirNomenPatchFull_Rem2Parties2(id, rec) {
    //var id = view.grid.UO_id;
    //Полный путь от Группы к выбранному объкту
    Ext.getCmp("DirNomenPatchFull" + id).setValue(rec.get('DirNomenPatchFull'));

    //Нет списка партий у документа: например Акт выполненных работ
    if (Ext.getCmp("gridParty_" + id) == undefined) return;

    var storeGrid = Ext.getCmp("gridParty_" + id).getStore();
    //Если панель скрыта, то не показывать "партии товара"
    if (Ext.getCmp("gridParty_" + id).collapsed) { storeGrid.setData([], false); return; }
    //Выбрана ли Организация (если новая накладная, то может быть и не выбрана Организация)
    if (Ext.getCmp("DirContractorIDOrg" + id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Организацию (так как партии привязаны к Организации)!"); return; }

    //Склад
    //1. Если это документ "Перемещение"
    var DirWarehouseID = Ext.getCmp("DirWarehouseID" + id);
    if (DirWarehouseID == undefined) DirWarehouseID = Ext.getCmp("DirWarehouseIDFrom" + id);
    //2. Выбран ли Склад (если новая накладная, то может быть и не выбран Склад)
    if (DirWarehouseID == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад (так как партии привязаны к Складу)!"); return; }




    //Получаем storeGrid и делаем load() *** *** *** *** *** *** *** ***

    //Если Розница, то DocDateS и DocDatePo
    var DocDateX;
    if (Ext.getCmp("DocDate" + id) == undefined) {
        DocDateX = "&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + id).getValue(), "Y-m-d") + "&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + id).getValue(), "Y-m-d");
    }
    else {
        DocDateX = "&DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + id).getValue(), "Y-m-d")
    }

    Ext.getCmp("tree_" + id).setDisabled(true);

    storeGrid.proxy.url =
        HTTP_Rem2Parties +
        "?DirServiceNomenID=" + rec.get('id') +
        "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + id).getValue() +
        "&DirWarehouseID=" + DirWarehouseID.getValue() +
        DocDateX;
    
    storeGrid.load();
    Ext.getCmp("tree_" + id).setDisabled(false);
}
//1.2. Функция вызываемая из контроллеров, при клике на Дерево товара (только для Розницы)
function fun_DirNomenPatchFull_Rem2Parties3(id, rec) {
    //var id = view.grid.UO_id;
    //Полный путь от Группы к выбранному объкту
    Ext.getCmp("DirNomenPatchFull" + id).setValue(rec.get('DirNomenPatchFull'));

    //Нет списка партий у документа: например Акт выполненных работ
    if (Ext.getCmp("gridParty_" + id) == undefined) return;

    var storeGrid = Ext.getCmp("gridParty_" + id).getStore();
    //Если панель скрыта, то не показывать "партии товара"
    if (Ext.getCmp("gridParty_" + id).collapsed) { storeGrid.setData([], false); return; }

    //Выбрана ли Организация (если новая накладная, то может быть и не выбрана Организация)
    var DirContractorIDOrg = Ext.getCmp("DirContractorIDOrg" + id);
    if (DirContractorIDOrg == undefined || DirContractorIDOrg == null || DirContractorIDOrg.getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Организацию (так как партии привязаны к Организации)!"); return; }

    //Склад
    //1. Если это документ "Перемещение"
    var DirWarehouseID = Ext.getCmp("DirWarehouseID" + id);
    if (DirWarehouseID == undefined) DirWarehouseID = Ext.getCmp("DirWarehouseIDFrom" + id);
    //2. Выбран ли Склад (если новая накладная, то может быть и не выбран Склад)
    if (DirWarehouseID == undefined || DirWarehouseID == null || DirWarehouseID.getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад (так как партии привязаны к Складу)!"); return; }

    //Получаем storeGrid и делаем load()
    Ext.getCmp("tree_" + id).setDisabled(true);

    storeGrid.proxy.url =
        HTTP_Rem2Parties +
        "?DirNomenID=" + rec.get('id') +
        "&DirContractorIDOrg=" + DirContractorIDOrg.getValue() +
        "&DirWarehouseID=" + DirWarehouseID.getValue() +
        "&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + id).getValue(), "Y-m-d") +
        "&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + id).getValue(), "Y-m-d");

    storeGrid.load();
    Ext.getCmp("tree_" + id).setDisabled(false);
}




/*
    Число ли это
*/
function isNumber(n) {
    return !isNaN(parseInt(n)) && isFinite(n);
}



/*
    Resize Browser: При уменьшении ширины окна браузера уменьшать и верщнее меню
*/
function funResizeBrowser() {
    
    var clientWidth = document.documentElement.clientWidth;
    if (clientWidth >= 1080) {
        //Увеличиваем текст
        //Ext.getCmp("RightSysSettings0").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + lanSettings + "</font>");
        //Ext.getCmp("RightDir0").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + lanDirectories + "</font>");
        //Ext.getCmp("RightDoc0").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + lanTrade + "</font>");
        Ext.getCmp("RightVitrina0").setText("");
        Ext.getCmp("RightDocService0").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + lanService + "</font>");
        Ext.getCmp("RightDocSecondHands0").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + "Б/У" + "</font>");
        Ext.getCmp("RightDocOrderInt0").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + lanOrders + "</font>");
        Ext.getCmp("RightDocBankCash0").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + "Финансы" + "</font>");
        Ext.getCmp("RightSalaries0").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + "ЗП" + "</font>");
        Ext.getCmp("RightLogistics0").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + "Логистика" + "</font>");
        Ext.getCmp("RightAnalitics0").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + "Аналитика" + "</font>");
        Ext.getCmp("RightKKM0").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + "ККМ" + "</font>");
        //Ext.getCmp("RightReport0").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + lanReports + "</font>");
        Ext.getCmp("HeaderToolBarEmployees").setText("<font size=" + HeaderMenu_FontSize_1 + ">" + varDirEmployeeLogin + " (" + varDirWarehouseNameEmpl + ")" + "</font>");
    }
    else if (clientWidth < 1080 && clientWidth >= 875) {
        //Уменьшаем текст
        //Ext.getCmp("RightSysSettings0").setText(lanSettings);
        //Ext.getCmp("RightDir0").setText(lanDirectories);
        //Ext.getCmp("RightDoc0").setText(lanTrade);
        Ext.getCmp("RightVitrina0").setText("");
        Ext.getCmp("RightDocService0").setText(lanService);
        Ext.getCmp("RightDocSecondHands0").setText("Б/У");
        Ext.getCmp("RightDocOrderInt0").setText(lanOrders);
        Ext.getCmp("RightDocBankCash0").setText("Финансы");
        Ext.getCmp("RightSalaries0").setText("ЗП");
        Ext.getCmp("RightLogistics0").setText("Логистика");
        Ext.getCmp("RightAnalitics0").setText("Аналитика");
        Ext.getCmp("RightKKM0").setText("ККМ");
        //Ext.getCmp("RightReport0").setText(lanReports);
        Ext.getCmp("HeaderToolBarEmployees").setText(varDirEmployeeLogin + " (" + varDirWarehouseNameEmpl + ")");
    }
    else if (clientWidth < 875 && clientWidth >= 540) {
        //Оставляем только иконки
        //Ext.getCmp("RightSysSettings0").setText("");
        //Ext.getCmp("RightDir0").setText("");
        //Ext.getCmp("RightDoc0").setText("");
        Ext.getCmp("RightVitrina0").setText("");
        Ext.getCmp("RightDocService0").setText("");
        Ext.getCmp("RightDocSecondHands0").setText("");
        Ext.getCmp("RightDocOrderInt0").setText("");
        Ext.getCmp("RightDocBankCash0").setText("");
        Ext.getCmp("RightSalaries0").setText("");
        Ext.getCmp("RightLogistics0").setText("");
        Ext.getCmp("RightAnalitics0").setText("");
        Ext.getCmp("RightKKM0").setText("");
        //Ext.getCmp("RightReport0").setText("");
        Ext.getCmp("HeaderToolBarEmployees").setText(varDirEmployeeLogin + " (" + varDirWarehouseNameEmpl + ")");
    }
    else if (clientWidth < 520) {
        //Оставляем только иконки
        //Ext.getCmp("RightSysSettings0").setText("");
        //Ext.getCmp("RightDir0").setText("");
        //Ext.getCmp("RightDoc0").setText("");
        Ext.getCmp("RightVitrina0").setText("");
        Ext.getCmp("RightDocService0").setText("");
        Ext.getCmp("RightDocSecondHands0").setText("");
        Ext.getCmp("RightDocOrderInt0").setText("");
        Ext.getCmp("RightDocBankCash0").setText("");
        Ext.getCmp("RightSalaries0").setText("");
        Ext.getCmp("RightLogistics0").setText("");
        Ext.getCmp("RightAnalitics0").setText("");
        Ext.getCmp("RightKKM0").setText("");
        //Ext.getCmp("RightReport0").setText("");
        Ext.getCmp("HeaderToolBarEmployees").setText(varDirWarehouseNameEmpl);
    }
}



/*
    Отключить Веб-камеру
*/
function fun_VideoOff(video, localMediaStream) {
    video.pause();
    video.src = "";
    localMediaStream.getTracks()[0].stop();
}



/*
    Drop and Down в Дереве: перемещение Нодов в дереве
*/
function fun_Nods_Drop_Down(HTTP_XXX, node, data, overModel, dropPosition, dropPosition1, dropPosition2, dropPosition3) {

    //Если это не узел, то выйти и сообщить об этом!
    if (overModel.data.leaf) { Ext.Msg.alert(lanOrgName, "В данную ветвь перемещать запрещено!"); return; }

    //Раскроем ветку с ID=1, перед перемещением
    var treePanel = Ext.getCmp("tree_" + data.view.panel.UO_id);
    var storeDirServiceNomensTree = treePanel.getStore();
    var node = storeDirServiceNomensTree.getNodeById(overModel.data.id);
    if (node != null) {
        storeDirServiceNomensTree.UO_OnStop = false;

        //Раскрытие нужного нода
        //node.collapse(true);
        treePanel.expandPath(node.getPath());
        node.expand();

        if (node.firstChild == null) {

            //Проверка: у нода нет подчинённых (раскрыт и нет подчинённых)
            if (node.isFirst == true) {

                //Событие на раскрытие - раскрылось
                storeDirServiceNomensTree.on('load', function () {

                    if (storeDirServiceNomensTree.UO_OnStop) { return; }
                    else { storeDirServiceNomensTree.UO_OnStop = true; }

                    //Запрос на сервер - !!! ДВАЖДЫ ПОВТОРЯЕТСЯ !!! №1
                    Ext.Ajax.request({
                        timeout: varTimeOutDefault,
                        url: HTTP_XXX + "?id=" + data.records[0].data.id + "&sub=" + overModel.data.id,
                        method: 'PUT',
                        success: function (result) {
                            var sData = Ext.decode(result.responseText);
                            if (sData.success == true) {
                                //...
                            } else {
                                Ext.getCmp("tree_" + data.view.panel.UO_id).view.store.load();
                                Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                            }
                        },
                        failure: function (form, action) {
                            //Права.
                            /*if (action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
                            Ext.Msg.alert(lanOrgName, txtMsg008 + action.result.data);*/
                            Ext.getCmp("tree_" + data.view.panel.UO_id).view.store.load();
                            funPanelSubmitFailure(form, action);
                        }
                    });

                });

            }
            else {
                //Запрос на сервер - !!! ДВАЖДЫ ПОВТОРЯЕТСЯ !!! №1
                Ext.Ajax.request({
                    timeout: varTimeOutDefault,
                    url: HTTP_XXX + "?id=" + data.records[0].data.id + "&sub=" + overModel.data.id,
                    method: 'PUT',
                    success: function (result) {
                        var sData = Ext.decode(result.responseText);
                        if (sData.success == true) {
                            //...
                        } else {
                            Ext.getCmp("tree_" + data.view.panel.UO_id).view.store.load();
                            Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                        }
                    },
                    failure: function (form, action) {
                        //Права.
                        /*if (action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
                        Ext.Msg.alert(lanOrgName, txtMsg008 + action.result.data);*/
                        Ext.getCmp("tree_" + data.view.panel.UO_id).view.store.load();
                        funPanelSubmitFailure(form, action);
                    }
                });
            }



        }
        else {
            //Запрос на сервер - !!! ДВАЖДЫ ПОВТОРЯЕТСЯ !!! №2
            Ext.Ajax.request({
                timeout: varTimeOutDefault,
                url: HTTP_XXX + "?id=" + data.records[0].data.id + "&sub=" + overModel.data.id,
                method: 'PUT',
                success: function (result) {
                    var sData = Ext.decode(result.responseText);
                    if (sData.success == true) {
                        
                    } else {
                        Ext.getCmp("tree_" + data.view.panel.UO_id).view.store.load();
                        Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                    }
                },
                failure: function (form, action) {
                    //Права.
                    /*if (action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
                    Ext.Msg.alert(lanOrgName, txtMsg008 + action.result.data);*/
                    Ext.getCmp("tree_" + data.view.panel.UO_id).view.store.load();
                    funPanelSubmitFailure(form, action);
                }
            });
        }
    }

};



/*
    запрос на сервер за ценами из Истории
*/
//1. Для всего остального
function fun_viewDocPurchTabsEdit_RequestPrice(form, UO_GridRecord, ObjectID, UO_idCall) {

    Ext.Ajax.request({
        timeout: varTimeOutDefault, waitMsg: lanUpload, method: 'GET',
        url: HTTP_DirNomenHistories + UO_GridRecord.data.id + "/?Action=1",
        //url: HTTP_RemParties + UO_GridRecord.data.id + "/?Action=1", 
        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                //Если нет данных на Сервере, то проставляем по умолчанию:
                Ext.getCmp("PriceCurrency" + ObjectID).setValue(0);
                Ext.getCmp("PriceVAT" + ObjectID).setValue(0);
                Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
                Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
                Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);
                //Если наценки отрицательные, то ставим их из Настроек
                funMarkupSet(ObjectID, sData.data + "<br />");
                return;
            }
            else {
                if (form != undefined) {
                    //Создаём модель
                    var UO_GridRecord = Ext.create("PartionnyAccount.model.Sklad/Object/Rem/RemParties/modelRemPartiesGrid")
                    //Пишем в модель данные
                    UO_GridRecord.data = sData.data.Result;
                    form.loadRecord(UO_GridRecord);
                    funMarkupSet(ObjectID); //Если наценки отрицательные, то ставим их из Настроек
                }
                else {
                    //Цена в зависимости от выбранного "Типа Цены"
                    /*if (UO_idCall != undefined) {
                        if (Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue() == 1) {
                            Ext.getCmp("PriceCurrency" + ObjectID).setValue(sData.data.Result.PriceRetailCurrency);
                            //Ext.getCmp("PriceVAT" + ObjectID).setValue(sData.data.Result.PriceRetailVAT);
                        }
                        else if (Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue() == 2) {
                            Ext.getCmp("PriceCurrency" + ObjectID).setValue(sData.data.Result.PriceWholesaleCurrency);
                            //Ext.getCmp("PriceVAT" + ObjectID).setValue(sData.data.Result.PriceWholesaleVAT);
                        }
                        else if (Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue() == 3) {
                            Ext.getCmp("PriceCurrency" + ObjectID).setValue(sData.data.Result.PriceIMCurrency);
                            //Ext.getCmp("PriceVAT" + ObjectID).setValue(sData.data.Result.PriceIMVAT);
                        }
                    }*/
                    if (ObjectID != undefined) {
                        if (Ext.getCmp("DirPriceTypeID" + ObjectID).getValue() == 1) {
                            Ext.getCmp("PriceCurrency" + ObjectID).setValue(sData.data.Result.PriceRetailCurrency);
                            //Ext.getCmp("PriceVAT" + ObjectID).setValue(sData.data.Result.PriceRetailVAT);
                        }
                        else if (Ext.getCmp("DirPriceTypeID" + ObjectID).getValue() == 2) {
                            Ext.getCmp("PriceCurrency" + ObjectID).setValue(sData.data.Result.PriceWholesaleCurrency);
                            //Ext.getCmp("PriceVAT" + ObjectID).setValue(sData.data.Result.PriceWholesaleVAT);
                        }
                        else if (Ext.getCmp("DirPriceTypeID" + ObjectID).getValue() == 3) {
                            Ext.getCmp("PriceCurrency" + ObjectID).setValue(sData.data.Result.PriceIMCurrency);
                            //Ext.getCmp("PriceVAT" + ObjectID).setValue(sData.data.Result.PriceIMVAT);
                        }
                    }
                    Ext.getCmp("DirCurrencyID" + ObjectID).setValue(sData.data.Result.DirCurrencyID);
                    Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(sData.data.Result.DirCurrencyRate);
                    Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(sData.data.Result.DirCurrencyMultiplicity);
                }

                return;
            }
        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });

}
//2. Для Заказов
function fun_controllerDocOrderIntsEdit_RequestPrice(DirNomenID, ObjectID) {
    
    Ext.Ajax.request({
        timeout: varTimeOutDefault, waitMsg: lanUpload, method: 'GET',
        url: HTTP_DirNomenHistories + DirNomenID + "/?Action=1",
        success: function (result) {
            
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                //Если нет данных на Сервере, то проставляем по умолчанию:
                Ext.getCmp("PriceCurrency" + ObjectID).setValue(0);
                Ext.getCmp("PriceVAT" + ObjectID).setValue(0);
                Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
                Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
                Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);
                //Если наценки отрицательные, то ставим их из Настроек
                funMarkupSet(ObjectID, sData.data + "<br />");
                return;
            }
            else {
                if (ObjectID != undefined) {
                    if (Ext.getCmp("DirPriceTypeID" + ObjectID).getValue() == 1) {
                        Ext.getCmp("PriceCurrency" + ObjectID).setValue(sData.data.Result.PriceRetailCurrency);
                        if (Ext.getCmp("PriceVAT" + ObjectID) != undefined) Ext.getCmp("PriceVAT" + ObjectID).setValue(sData.data.Result.PriceRetailVAT);
                    }
                    else if (Ext.getCmp("DirPriceTypeID" + ObjectID).getValue() == 2) {
                        Ext.getCmp("PriceCurrency" + ObjectID).setValue(sData.data.Result.PriceWholesaleCurrency);
                        if (Ext.getCmp("PriceVAT" + ObjectID) != undefined) Ext.getCmp("PriceVAT" + ObjectID).setValue(sData.data.Result.PriceWholesaleVAT);
                    }
                    else if (Ext.getCmp("DirPriceTypeID" + ObjectID).getValue() == 3) {
                        Ext.getCmp("PriceCurrency" + ObjectID).setValue(sData.data.Result.PriceIMCurrency);
                        if (Ext.getCmp("PriceVAT" + ObjectID) != undefined) Ext.getCmp("PriceVAT" + ObjectID).setValue(sData.data.Result.PriceIMVAT);
                    }
                }
                Ext.getCmp("DirCurrencyID" + ObjectID).setValue(sData.data.Result.DirCurrencyID);
                Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(sData.data.Result.DirCurrencyRate);
                Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(sData.data.Result.DirCurrencyMultiplicity);

                return;
            }
        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });

}




function preselectItem(grid, name, value) {
    
    var store = grid.store;

    store.load({ waitMsg: lanLoading });
    store.on('load', function () {

        var index = grid.store.find(name, value);
        grid.getSelectionModel().select(grid.store.getAt(index));

    });

}




/*
    Добавить N-дней к дате
*/
function fun_addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return Ext.Date.format(result, "Y-m-d");
}

/*
    Добавить N-месяцев к дате
*/
function fun_addMonth(date, month) {
    var result = new Date(new Date(date).setMonth(date.getMonth() + month));
    return Ext.Date.format(result, "Y-m-d");
}




/*
    Переключаемся на уже открытую вкладку
*/
//ObjectEditConfig
function fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy) {
    if (!varTabIdenty) return;

    var tabPanelMain = Ext.getCmp("viewContainerCentral");
    var i = tabPanelMain.items.items.length;
    for (var i = 0; i < tabPanelMain.items.items.length; i++) {
        if (tabPanelMain.items.items[i].UO_Identy == UO_Identy) {

            //Делаем активной найденную вкладку
            tabPanelMain.setActiveTab(tabPanelMain.items.items[i]);

            //Разблокировка вызвавшего окна
            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

            return true;
        }
    }
}
//ObjectConfig
function fun_TabIdenty_ObjectConfig(UO_Identy) {
    if (!varTabIdenty) return;

    var tabPanelMain = Ext.getCmp("viewContainerCentral");
    var i = tabPanelMain.items.items.length;
    for (var i = 0; i < tabPanelMain.items.items.length; i++) {
        if (tabPanelMain.items.items[i].UO_Identy == UO_Identy) {

            //Делаем активной найденную вкладку
            tabPanelMain.setActiveTab(tabPanelMain.items.items[i]);

            //Разблокировка вызвавшего окна
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, false);

            return true;
        }
    }
}



function fun_isInteger(num) {
    return (num ^ 0) === num;
}



function fun_onTriggerSearchGridClick(grid, name, value) {
    
    var store = grid.getStore();
    if (value) {
        store.filter(name, value, false, false);
        //store.filter(name, new RegExp("^" + value + "$"));
    }
    else {
        store.clearFilter();
    }

}

