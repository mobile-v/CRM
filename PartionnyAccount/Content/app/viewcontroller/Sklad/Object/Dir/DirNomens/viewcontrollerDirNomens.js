Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Dir/DirNomens/viewcontrollerDirNomens', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDirNomens',


    //Клик на изображение - ""
    onImageShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "imageShow" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            ""
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },

    //Клик на изображение - "1"
    onImage1ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image1Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "1"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },

    //Клик на изображение - "2"
    onImage2ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image2Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "2"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },

    //Клик на изображение - "3"
    onImage3ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image3Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "3"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },

    //Клик на изображение - "4"
    onImage4ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image4Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "4"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },

    //Клик на изображение - "5"
    onImage5ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image5Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "5"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },


});