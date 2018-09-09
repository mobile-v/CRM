Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Dir/DirNomens/viewcontrollerDirNomensImg', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDirNomensImg',


    //Клик на изображение
    onFileFieldChange: function (pFileField, pPatchNew, pPatchOld, pFunc) {
        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + pFileField.UO_id);

        //Сохранение
        widgetXForm.submit({
            method: "POST",
            url: HTTP_Image,
            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                Ext.getCmp("SysGenID" + pFileField.UO_id).setValue(action.result.data.SysGenID);
                Ext.getCmp("SysGenIDPatch" + pFileField.UO_id).setValue(action.result.data.SysGenIDPatch);

                Ext.getCmp("imageShow" + pFileField.UO_id).setSrc(action.result.data.SysGenIDPatch);
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });
    },



    // Кнопки === === === === === === === === === === ===
    //!!! ВНИМАНИЕ !!!
    //!!!Это кнопка не используется!!!
    //Используется обработчик в Вьюхе "viewDirNomensImg"
    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        /*
        var UO_idCall = Ext.getCmp(Ext.getCmp("SysGenIDPatch" + aButton.UO_id).UO_idCall).UO_id;

        Ext.getCmp("SysGenID" + UO_idCall).setValue(Ext.getCmp("SysGenID" + aButton.UO_id).getValue());
        Ext.getCmp("SysGenIDPatch" + UO_idCall).setValue(Ext.getCmp("SysGenIDPatch" + aButton.UO_id).getValue());
        Ext.getCmp("imageShow" + UO_idCall).setSrc(Ext.getCmp("SysGenIDPatch" + aButton.UO_id).getValue());

        Ext.getCmp("viewDirNomensImg" + aButton.UO_id).close();
        */

        //var UO_idCall = aButton.UO_idCall;
        //Ext.getCmp(UO_idCall).setSrc(Ext.getCmp("SysGenIDPatch" + aButton.UO_id).getValue());
        //var UO_id = Ext.getCmp(Ext.getCmp("SysGenIDPatch" + aButton.UO_id).UO_idCall).UO_id;

        
        var UO_idCall = Ext.getCmp(Ext.getCmp("SysGenIDPatch" + aButton.UO_id).UO_idCall).UO_id;
        var UO_Param_id = Ext.getCmp("viewDirNomensImg" + aButton.UO_id).UO_Param_id;
        Ext.getCmp("SysGen" + UO_Param_id + "ID" + UO_idCall).setValue(Ext.getCmp("SysGenID" + aButton.UO_id).getValue());
        Ext.getCmp("SysGen" + UO_Param_id + "IDPatch" + UO_idCall).setValue(Ext.getCmp("SysGenIDPatch" + aButton.UO_id).getValue());
        Ext.getCmp("image" + UO_Param_id + "Show" + UO_idCall).setSrc(Ext.getCmp("SysGenIDPatch" + aButton.UO_id).getValue());

        Ext.getCmp("viewDirNomensImg" + aButton.UO_id).close();

    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("viewDirNomensImg" + aButton.UO_id).close();
    },

});