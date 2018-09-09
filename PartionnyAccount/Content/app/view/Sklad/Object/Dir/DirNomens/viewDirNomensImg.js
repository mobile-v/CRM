Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirNomens/viewDirNomensImg", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDirNomensImg",

    layout: "border",
    region: "center",
    title: lanImage,
    width: 600, height: 450,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    //Контроллер
    controller: 'viewcontrollerDirNomensImg',

    conf: {},

    initComponent: function () {
        
        //Файл-изображение
        var ImageShow = Ext.create('Ext.Img', {
            id: 'imageShow' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            src: "../../Scripts/sklad/images/ru_default_no_foto.jpg",
            //width: 500, height: 240,
            style: {
                'display': 'block',
                'margin': 'auto'
            }
        });
        var formPanelEdit1 = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)

            title: "Файл",
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanGeneral,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            //width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'anchor',
            //defaults: { anchor: '100%' },
            autoScroll: true,
            autoHeight: true,

            items: [
                
                { xtype: 'textfield', fieldLabel: "SysGenID", name: "SysGenID", id: "SysGenID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: true, hidden: true }, //, readOnly: true
                { xtype: 'textfield', fieldLabel: "SysGenIDPatch", name: "SysGenIDPatch", id: "SysGenIDPatch" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: true, hidden: true }, //, readOnly: true

                //Кнопка выбора изображения
                {
                    xtype: 'filefield',
                    name: 'FileField',
                    id: 'FileField' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

                    regex: /^.*\.(jpg|jpeg|JPG|JPEG|png|PNG)$/,
                    //regex: /^.*\.(jpg|jpeg|JPG|JPEG)$/,
                    regexText: 'Выберите файл с изображением в формате jpg или png!',
                    listeners: {
                        change: function (fld, value) {
                            var newValue = value.replace(/C:\\fakepath\\/g, '');
                            fld.setRawValue(newValue);
                        }
                    },

                    //fieldLabel: 'Excel',
                    labelWidth: 125,
                    msgTarget: 'side',
                    allowBlank: true,
                    anchor: '100%',
                    buttonText: "Выбрать изображение логотипа",

                    listeners: { 'change': 'onFileFieldChange' },
                    /*listeners: {
                        'change': function (pParam1, pParam2, pParam3, pParam4) {
                            Ext.getCmp("imageShow").src = "";
                        },
                    }*/
                },

                ImageShow
            ],


            //buttonAlign: 'left',
            buttons: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                    text: lanSave, icon: '../Scripts/sklad/images/save.png',
                    listeners: { click: 'onBtnSaveClick' }
                },
                " ",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                    text: lanCancel, icon: '../Scripts/sklad/images/cancel.png',
                    listeners: { click: 'onBtnCancelClick' }
                },
            ]

        });


        var localMediaStream;
        var formPanelEdit2 = Ext.create('Ext.form.Panel', {
            id: "formPanelEdit2_",
            layout: 'hbox',
            title: "Веб-камера",
            items: [
                {
                    width: '100%', height: '100%',
                    title: "Web-камера",
                    id: 'preview',
                    html: '<video  id="video" width="350" height="250" autoplay></video>',
                    flex: 1,
                    tbar: [
                        {
                            text: "Веб-камера", icon: '../Scripts/sklad/images/video16.png',
                            handler: function () {
                                // Grab elements, create settings, etc.
                                var video = document.getElementById('video');
                                // Get access to the camera!
                                if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
                                    // Not adding "{ audio: true }" since we only want video now
                                    navigator.mediaDevices.getUserMedia({ video: true }).then(function (stream) {
                                        localMediaStream = stream;
                                        video.src = window.URL.createObjectURL(stream);
                                        video.play();
                                    });
                                }
                            }
                        },
                        {
                            text: "Скриншот", icon: '../Scripts/sklad/images/screenshooter16.png',
                            handler: function () {
                                var video = document.getElementById("video");
                                var canvas = document.getElementById("canvas");
                                context = canvas.getContext("2d");
                                context.drawImage(video, 0, 0, 350, 250);

                                fun_VideoOff(video, localMediaStream);
                            }
                        },
                    ]
                },
                {
                    width: '100%', height: '100%', //width: 450, height: 350,
                    title: "Скриншот",
                    id: "canvasX",
                    html: '<canvas id="canvas" width="350" height="250"></canvas>', //width="320" height="240"
                    flex: 1,
                },
            ],


            //buttonAlign: 'left',
            buttons: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                    text: lanSave, icon: '../Scripts/sklad/images/save.png',
                    handler: function (par1, par2) {

                        //var canvas = document.getElementById("canvas");
                        //window.open(canvas.toDataURL("image/png"));

                        var photoWebCamX = document.getElementById('canvas').toDataURL("image/png");
                        photoWebCamX = photoWebCamX.replace(/^data:image\/(png|jpg);base64,/, "");


                        //Форма на Виджете
                        var widgetXForm = Ext.getCmp("formPanelEdit2_");

                        //Сохранение
                        widgetXForm.submit({
                            //headers: { 'Content-Type': 'multipart/form-data; charset=UTF-8' },
                            params: { photoWebCam: photoWebCamX },

                            method: "PUT",
                            url: HTTP_Image + "/777/",
                            timeout: varTimeOutDefault,
                            waitMsg: lanUploading,

                            success: function (form, action) {
                                Ext.getCmp("SysGenID" + par1.UO_id).setValue(action.result.data.SysGenID);
                                Ext.getCmp("SysGenIDPatch" + par1.UO_id).setValue(action.result.data.SysGenIDPatch);
                                Ext.getCmp("imageShow" + par1.UO_id).setSrc(action.result.data.SysGenIDPatch);


                                /*
                                var UO_idCall = Ext.getCmp(Ext.getCmp("SysGenIDPatch" + par1.UO_id).UO_idCall).UO_id;

                                Ext.getCmp("SysGenID" + UO_idCall).setValue(Ext.getCmp("SysGenID" + par1.UO_id).getValue());
                                Ext.getCmp("SysGenIDPatch" + UO_idCall).setValue(Ext.getCmp("SysGenIDPatch" + par1.UO_id).getValue());
                                Ext.getCmp("imageShow" + UO_idCall).setSrc(Ext.getCmp("SysGenIDPatch" + par1.UO_id).getValue());
                                */

                                var UO_idCall = Ext.getCmp(Ext.getCmp("SysGenIDPatch" + par1.UO_id).UO_idCall).UO_id;
                                var UO_Param_id = Ext.getCmp("viewDirNomensImg" + par1.UO_id).UO_Param_id;

                                Ext.getCmp("SysGen" + UO_Param_id + "ID" + UO_idCall).setValue(Ext.getCmp("SysGenID" + par1.UO_id).getValue());
                                Ext.getCmp("SysGen" + UO_Param_id + "IDPatch" + UO_idCall).setValue(Ext.getCmp("SysGenIDPatch" + par1.UO_id).getValue());
                                Ext.getCmp("image" + UO_Param_id + "Show" + UO_idCall).setSrc(Ext.getCmp("SysGenIDPatch" + par1.UO_id).getValue());


                                Ext.getCmp("viewDirNomensImg" + par1.UO_id).close();
                            },
                            failure: function (form, action) {
                                funPanelSubmitFailure(form, action);
                            }
                        });

                    }
                }
            ]
        });





        var tabPanel = Ext.create('Ext.tab.Panel', {
            id: "tab_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            region: "center",
            bodyStyle: 'background:transparent;',
            //width: "100%", height: "100%",
            autoHeight: true,
            split: true,

            items: [
                formPanelEdit1, formPanelEdit2
            ]

        });


        //body
        this.items = [

            tabPanel

        ],


        this.callParent(arguments);
    }

});