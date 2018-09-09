Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirNomens/viewDirNomensWinComboEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDirNomensWinComboEdit",

    layout: "border",
    region: "center",
    title: lanGoods,
    width: 635, height: 390,
    autoScroll: false,

    UO_maximize: false, //Максимизировать во весь экран
    UO_Center: true,    //true - в центре экрана, false - окна каскадом
    UO_Modal: true,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {
        
        var formEdit = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            //title: "Редактирование",
            region: "center",
            layout: 'border', // тип лэйоута - трехколонник с подвалом и шапкой
            bodyStyle: 'background:transparent;',
            //width: "100%", height: "100%",
            autoHeight: true,

            items: [
                {
                    xtype: 'viewDirNomensComboEdit',
                    id: 'viewDirNomensComboEdit' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

                    storeDirPriceTypesGrid: this.storeDirPriceTypesGrid,

                    storeDirNomensGrid1: this.storeDirNomensGrid1,
                    storeDirNomensGrid2: this.storeDirNomensGrid2,
                    //storeDirNomensGrid3: this.storeDirNomensGrid3,
                    //storeDirNomensGrid4: this.storeDirNomensGrid4,
                    //storeDirNomensGrid5: this.storeDirNomensGrid5,
                    //storeDirNomensGrid6: this.storeDirNomensGrid6,

                    storeDirNomenCategoriesGrid: this.storeDirNomenCategoriesGrid,

                    storeDirCharColoursGrid: this.storeDirCharColoursGrid,
                    storeDirCharMaterialsGrid: this.storeDirCharMaterialsGrid,
                    storeDirCharNamesGrid: this.storeDirCharNamesGrid,
                    storeDirCharSeasonsGrid: this.storeDirCharSeasonsGrid,
                    storeDirCharSexesGrid: this.storeDirCharSexesGrid,
                    storeDirCharSizesGrid: this.storeDirCharSizesGrid,
                    storeDirCharStylesGrid: this.storeDirCharStylesGrid,
                    storeDirCharTexturesGrid: this.storeDirCharTexturesGrid,

                    storeDirCurrenciesGrid: this.storeDirCurrenciesGrid,
                }
            ]
        });


        //body
        this.items = [

            formEdit

        ],


        this.callParent(arguments);
    }

});