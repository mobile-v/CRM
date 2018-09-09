//Верхний Тулбар
Ext.define("PartionnyAccount.view.Sklad/Container/viewContainerHeader", {
    extend: "Ext.Toolbar",
    style: "background-color: #157fcc;",
    margin: '0 0 0 0', //margin: '0 0 3 0',
    alias: "widget.viewContainerHeader",
    height: 40,
    region: "north",
    frame: true,
    iconCls: 'windowIcon',
    defaultButtonUI: DefaultButtonUI,

    
    conf: {},

    initComponent: function () {
        //this.id = this.conf.id;
        //this.defaultButtonUI = this.conf.defaultButtonUI;

        this.callParent(arguments);
    },
    
    items: [

        "->",
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