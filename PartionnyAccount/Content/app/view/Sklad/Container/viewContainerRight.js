//Правая панель
Ext.define("PartionnyAccount.view.Sklad/Container/viewContainerRight", {
    extend: "Ext.form.Panel",
    alias: "widget.viewContainerRight",
    title: lanMessageBar,
    width: 275,
    region: "east",
    split: true,
    collapsible: true, //кнопка ">>" - спрятать
    closable: true,  //кнопка "Х" - закрыть
    autoScroll: true,
    loadMask: true,
    modal: true,
    layout: 'accordion',
    items: [
        //Accordion №1: Отправить сообщение
        {
            //id: "view.container.right.email",
            title: lanAskAQuestion,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',

            items: [
                {
                    xtype: 'container', layout: { type: 'fit' }, flex: 1, margins: 5,
                    items: [
                        { xtype: 'label', text: lanTheme },
                        { xtype: 'textfield', name: 'MsgThema', id: 'MsgThema', width: 260, allowBlank: false },

                        { xtype: 'label', text: lanYourEMailAddress },
                        { xtype: 'textfield', name: 'MsgEMail', id: 'MsgEMail', width: 260, allowBlank: false, regex: /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/ },

                        { xtype: 'label', text: lanMessage },

                        //TextAreaTextEMail
                        { xtype: 'textarea', name: "MsgText", id: 'MsgText', width: 260, height: 125, allowBlank: false },
                    ]
                },

                {
                    xtype: 'button', text: lanSend, icon: '../Scripts/sklad/images/send16.png',
                    itemId: "btnSendEMail"
                },
                {
                    xtype: 'button', text: lanClear, icon: '../Scripts/sklad/images/clear16.png',
                    itemId: "btnClearEMail"
                }
            ]
        }
    ]
});