/*
    Открытия Окон (Табов, Панелей) в центральной панели
*/
function ObjectShow(widgetX) {
    
    //Центральная панель
    var viewContainerCentral = Ext.getCmp("viewContainerCentral");
    if (viewContainerCentral == undefined) { Ext.Msg.alert(lanFailure, "Central container not found! Press F5 key!"); return; }

    if (InterfaceSystem == 1) {
        ObjectShow_Windows(viewContainerCentral, widgetX);
        return;
    }
    else if (InterfaceSystem == 2) {
        if (widgetX.__proto__.UO_Extend == "Window") {
            ObjectShow_Windows(undefined, widgetX);
            return;
        }

        widgetX.closable = true;
        viewContainerCentral.add(widgetX);
        viewContainerCentral.setActiveTab(widgetX);
    }
    else if (InterfaceSystem == 3) {
        //viewContainerCentral.removeAll(true);

        widgetX.closable = true;
        //widgetX.layout = "card";
        widgetX.width = "100%"; widgetX.height = "100%";
        viewContainerCentral.add(widgetX);

        //Переключится на эту вкладку
        funInterfaceSystem3_next(true, widgetX);
    }

}



/*
    Если тип - Окно
*/

function ObjectShow_Windows(viewContainerCentral, widgetX) {
    if (viewContainerCentral != undefined) widgetX.renderTo = viewContainerCentral.body;
    widgetX.maximizable = true;
    widgetX.resizable = true;
    widgetX.border = false;
    //widgetX.layout = "border";
    widgetX.constrainHeader = true; // or just constrain = true if you prefer
    widgetX.constrain = true;
    //widgetX.width = 350; widgetX.height = 250;
    
    //Координаты размещения, если заданы
    if (!widgetX.UO_Center) {
        if (ObjectXY > 160) { ObjectXY = 1; }
        if (ObjectXY == 0) { ObjectXY = 1; }
        else ObjectXY += 15;

        widgetX.x = ObjectXY; widgetX.y = ObjectXY;
    }
    else {
        //В центре экрана
        widgetX.center();
    }

    widgetX.show();
    if (widgetX.UO_maximize) widgetX.maximize();
}