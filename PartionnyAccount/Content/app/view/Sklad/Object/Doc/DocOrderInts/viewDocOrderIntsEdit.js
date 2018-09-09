Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocOrderInts/viewDocOrderIntsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocOrderIntsEdit",

    layout: "border",
    region: "center",
    title: lanOrder + "(1)",
    width: 700, height: 400,
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


        //body
        this.items = [

            //formPanelEdit


            {
                xtype: 'viewDocOrderIntsPattern',
                id: 'viewDocOrderIntsPattern' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

                storeDirPriceTypesGrid: this.storeDirPriceTypesGrid,

                storeDirServiceNomensGrid: this.storeDirServiceNomensGrid,
                storeDirNomensGrid1: this.storeDirNomensGrid1,
                storeDirNomensGrid2: this.storeDirNomensGrid2,
                storeDirNomensGrid3: this.storeDirNomensGrid3,
                //storeDirNomensGrid4: this.storeDirNomensGrid4,
                //storeDirNomensGrid5: this.storeDirNomensGrid5,
                //storeDirNomensGrid6: this.storeDirNomensGrid6,

                storeDirNomenCategoriesGrid: this.storeDirNomenCategoriesGrid,
                storeDirContractorsGrid: this.storeDirContractorsGrid,

                storeDirCharColoursGrid: this.storeDirCharColoursGrid,
                storeDirCharMaterialsGrid: this.storeDirCharMaterialsGrid,
                storeDirCharNamesGrid: this.storeDirCharNamesGrid,
                storeDirCharSeasonsGrid: this.storeDirCharSeasonsGrid,
                storeDirCharSexesGrid: this.storeDirCharSexesGrid,
                storeDirCharSizesGrid: this.storeDirCharSizesGrid,
                storeDirCharStylesGrid: this.storeDirCharStylesGrid,
                storeDirCharTexturesGrid: this.storeDirCharTexturesGrid,

                storeDirCurrenciesGrid: this.storeDirCurrenciesGrid,
            },

        ],


        this.callParent(arguments);
    }

});