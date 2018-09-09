//Маршрутизатор на View-грид
//Параметры:
//  pObjectName - Наименование обекта: Справочник.Товары, Документ.[Приходные накладные]
//  Params      - массив дополнительных параметров:
//   - UO_idCall        - ID-и Вьюхи, которая вызвала
//   - UO_Center        - Разместить в центре экрана
//   - UO_Modal         - Все остальные элементы не активные
//   - UO_Function_Tree - При "Клике" на Tree вызвать функцию
//   - UO_Function_Grid - При "Клике" на Grid вызвать функцию
//   - TreeShow         - Показывать Tree, если есть
//   - GridShow         - Показывать Grid
//   - TreeServerParam1 - Передача параметра серверу (для Трии)
//   - GridServerParam1 - Передача параметра серверу (для Грида)
//   - ContainerWidget  - Контейнер, в котором будут распологатся виджет "pObjectName"

//Подбор товара
//   - UO_idTab         - id-шник Спецификации (Табличной части) документа, что бы туда вставить выбранный Товар (с левой панели)
//   - UO_FunRecalcSum  - Функция пересчета сумм, для каждого докумена своя

//Массив
//   - ArrList          - Массив "Params[12]" в которм содержатся параметры индивидуально для каждой Вьюхи
//                        ArrList = [Data1, Data2, ...]

function ObjectConfig(pObjectName, Params) {

    //Если окно открыто, то устанавливаем на него фокус
    if (funSearchWin(pObjectName, true)) return;

    //Параметры
    var UO_idCall = Params[0];                                                  // ID-и Вьюхи, которая вызвала
    var UO_Center = Params[1]; if (UO_Center == undefined) UO_Center = false;   // Разместить в центре экрана
    var UO_Modal = Params[2]; if (UO_Modal == undefined) UO_Modal = false;      // Все остальные элементы не активные
    var UO_Function_Tree = Params[3];                                           // При "Клике" (на Tree or Grid) вызвать функцию
    var UO_Function_Grid = Params[4];                                           // При "Клике" (на Tree or Grid) вызвать функцию
    var TreeShow = Params[5]; if (TreeShow == undefined) TreeShow = true;       // Показывать Tree, если есть
    var GridShow = Params[6]; if (GridShow == undefined) GridShow = true;       // Показывать Grid
    var TreeServerParam1 = Params[7];                                           // Передача параметра серверу (для Трии)
    var GridServerParam1 = Params[8];                                           // Передача параметра серверу (для Грида)
    var ContainerWidget = Params[9];                                            // Контейнер, в котором будут распологатся виджет "pObjectName"
    //Подбор товара
    var UO_idTab = Params[10];                                                  // id-шник Спецификации (Табличной части) документа, что бы туда вставить выбранный Товар (с левой панели)
    var UO_FunRecalcSum = Params[11];                                           // Функция пересчета сумм, для каждого докумена своя
    //Массив
    var ArrList = Params[12];                                                   //  Массив "Params[12]" в которм содержатся параметры индивидуально для каждой Вьюхи


    //Для id
    ObjectID++;

    //Виджет "widgetX" который в конце функции будет добавлен в функцию "ObjectShow(widgetX, ContainerWidget)"
    var widgetX;



    try {


        //Блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
        //ObjectEditConfig_UO_idCall_true_false(true);



        switch (pObjectName) {



            // === Main ===

            case "viewMain": {
                // === Формируем и показываем окно ===
                //Panel
                widgetX = Ext.create("widget." + pObjectName, {
                    id: pObjectName,
                    UO_id: "",
                    UO_idMain: pObjectName,
                    UO_View: pObjectName,

                    UO_idCall: UO_idCall, //"viewPanelMain"

                    modal: UO_Modal
                });

                //Добавляем в свойство "Html" оплаты клиента
                var varDirPayServiceNameHtml = "Тарифный план: " + varDirPayServiceName + "<br />";
                if (varPaymentExpired) varDirPayServiceNameHtml = "<b style='color:red'>Тарифный план: " + varDirPayServiceName + "</b><br />";
                var varPayDateEndHtml = "Окончание: " + varPayDateEnd + "<br /><br />";
                if (varPaymentExpired) {
                    varPayDateEndHtml = "<b style='color:red'>Окончание: " + varPayDateEnd + " (через: " + nDaysLeft + " дня)<br /></b><br />";
                }

                widgetX.setHtml(
                    widgetX.html +
                    "<center>" +
                    varDirPayServiceNameHtml + //"Тарифный план: " + varDirPayServiceName + "<br />" +
                    "Сотрудников: " + varCountUser + "<br />" +
                    "Торговых точек: " + varCountTT + "<br />" +
                    "Интернет магазинов: " + varCountIM + "<br />" +
                    varPayDateEndHtml + //"Окончание: " + varPayDateEnd + "<br /><br />" +
                    "© «ВТорговомОблаке» 2017" +
                    "</center>"
                    )
                //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
                //ObjectShow(widgetX);
                break;
            }


            //Разработчик ПФ *** *** ***


            case "viewListObjectPFs": {

                var ListObjectID;
                var sm = Ext.getCmp("viewPanelJournal").getSelectionModel();
                if (sm.hasSelection()) {
                    ListObjectID = sm.getSelection()[0].get('id'); //sm.getSelection()[0].get('text');
                }

                //Store Grid
                var storeGrid = Ext.create("store.storeListObjectPFsGrid"); storeGrid.setData([], false); //storeGrid.load({ waitMsg: lanLoading });
                storeGrid.proxy.url = HTTP_ListObjectPFs + "?type=Tree&ListObjectID=" + ListObjectID;
                storeGrid.load({ waitMsg: lanLoading });

                //Panel
                widgetX = Ext.create("widget." + pObjectName, {
                    id: pObjectName + ObjectID,
                    UO_id: ObjectID,
                    UO_idMain: pObjectName + ObjectID,
                    UO_View: pObjectName,
                    UO_idCall: UO_idCall, //"viewPanelMain",
                    UO_Center: UO_Center,

                    UO_Function_Tree: UO_Function_Tree,
                    UO_Function_Grid: UO_Function_Grid,
                    GridServerParam1: GridServerParam1, //rec.data.id ()

                    modal: UO_Modal,
                    //storeGroup: storeGroup,
                    storeGrid: storeGrid,
                });

                break;
            }





            default:
                Ext.Msg.alert("ObjectConfig", "Object '" + pObjectName + "' not found!");
                return;
        }



        //Блокировать или Разблокировать вызвавший элемент "UO_idCall"
        function ObjectEditConfig_UO_idCall_true_false(Disable) {
            if (Disable) {
                //Блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
                if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined) { Ext.getCmp(UO_idCall).disable(); }
            }
            else {
                //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
                if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined) { Ext.getCmp(UO_idCall).enable(); }
            }
        }

        //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
        //ObjectEditConfig_UO_idCall_true_false(false);


        //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
        if (ContainerWidget == undefined) ObjectShow(widgetX);
        //Или в Панель, например в журналах "Подбор товара"
        else ObjectShow_NotCentral(widgetX, ContainerWidget);



    } catch (ex) {
        var exMsg = ex;
        if (exMsg.message != undefined) exMsg = ex.message;

        Ext.Msg.alert(lanOrgName, "Ошибка в скрипте! Вышлите, пожалуйста скриншот на: support@uchetoblako.ru<br />Подробности:" + exMsg);

        //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
        if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined) { Ext.getCmp(UO_idCall).enable(); }
    }

}