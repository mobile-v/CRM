Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Service/IM/viewcontrollerVitrinaInTrade', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerVitrinaInTrade',



    onSlider_QuantityChange: function (field, value) {
        
        value = parseInt(value, 10);

        if (value == 0) {
            Ext.getCmp("Slider_label1ID" + field.UO_id).setVisible(false); Ext.getCmp("Slider_DirNomen1ID" + field.UO_id).setVisible(false); Ext.getCmp("image1Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Slider_label2ID" + field.UO_id).setVisible(false); Ext.getCmp("Slider_DirNomen2ID" + field.UO_id).setVisible(false); Ext.getCmp("image2Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Slider_label3ID" + field.UO_id).setVisible(false); Ext.getCmp("Slider_DirNomen3ID" + field.UO_id).setVisible(false); Ext.getCmp("image3Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Slider_label4ID" + field.UO_id).setVisible(false); Ext.getCmp("Slider_DirNomen4ID" + field.UO_id).setVisible(false); Ext.getCmp("image4Show" + field.UO_id).setVisible(false);
        }
        if (value == 1) {
            Ext.getCmp("Slider_label1ID" + field.UO_id).setVisible(true); Ext.getCmp("Slider_DirNomen1ID" + field.UO_id).setVisible(true); Ext.getCmp("image1Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Slider_label2ID" + field.UO_id).setVisible(false); Ext.getCmp("Slider_DirNomen2ID" + field.UO_id).setVisible(false); Ext.getCmp("image2Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Slider_label3ID" + field.UO_id).setVisible(false); Ext.getCmp("Slider_DirNomen3ID" + field.UO_id).setVisible(false); Ext.getCmp("image3Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Slider_label4ID" + field.UO_id).setVisible(false); Ext.getCmp("Slider_DirNomen4ID" + field.UO_id).setVisible(false); Ext.getCmp("image4Show" + field.UO_id).setVisible(false);
        }
        else if (value == 2) {
            Ext.getCmp("Slider_label1ID" + field.UO_id).setVisible(true); Ext.getCmp("Slider_DirNomen1ID" + field.UO_id).setVisible(true); Ext.getCmp("image1Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Slider_label2ID" + field.UO_id).setVisible(true); Ext.getCmp("Slider_DirNomen2ID" + field.UO_id).setVisible(true); Ext.getCmp("image2Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Slider_label3ID" + field.UO_id).setVisible(false); Ext.getCmp("Slider_DirNomen3ID" + field.UO_id).setVisible(false); Ext.getCmp("image3Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Slider_label4ID" + field.UO_id).setVisible(false); Ext.getCmp("Slider_DirNomen4ID" + field.UO_id).setVisible(false); Ext.getCmp("image4Show" + field.UO_id).setVisible(false);
        }
        else if (value == 3) {
            Ext.getCmp("Slider_label1ID" + field.UO_id).setVisible(true); Ext.getCmp("Slider_DirNomen1ID" + field.UO_id).setVisible(true); Ext.getCmp("image1Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Slider_label2ID" + field.UO_id).setVisible(true); Ext.getCmp("Slider_DirNomen2ID" + field.UO_id).setVisible(true); Ext.getCmp("image2Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Slider_label3ID" + field.UO_id).setVisible(true); Ext.getCmp("Slider_DirNomen3ID" + field.UO_id).setVisible(true); Ext.getCmp("image3Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Slider_label4ID" + field.UO_id).setVisible(false); Ext.getCmp("Slider_DirNomen4ID" + field.UO_id).setVisible(false); Ext.getCmp("image4Show" + field.UO_id).setVisible(false);
        }
        else if (value == 4) {
            Ext.getCmp("Slider_label1ID" + field.UO_id).setVisible(true); Ext.getCmp("Slider_DirNomen1ID" + field.UO_id).setVisible(true); Ext.getCmp("image1Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Slider_label2ID" + field.UO_id).setVisible(true); Ext.getCmp("Slider_DirNomen2ID" + field.UO_id).setVisible(true); Ext.getCmp("image2Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Slider_label3ID" + field.UO_id).setVisible(true); Ext.getCmp("Slider_DirNomen3ID" + field.UO_id).setVisible(true); Ext.getCmp("image3Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Slider_label4ID" + field.UO_id).setVisible(true); Ext.getCmp("Slider_DirNomen4ID" + field.UO_id).setVisible(true); Ext.getCmp("image4Show" + field.UO_id).setVisible(true);
        }
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
            1,   // 1 - Новое, 2 - Редактировать
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



    onRecommended_QuantityChange: function (field, value) {
        value = parseInt(value, 10);
        
        if (value == 0) {
            //4
            Ext.getCmp("Recommended_label1ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen1ID" + field.UO_id).setVisible(false); //Ext.getCmp("image5Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label2ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen2ID" + field.UO_id).setVisible(false); //Ext.getCmp("image6Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label3ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen3ID" + field.UO_id).setVisible(false); //Ext.getCmp("image7Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label4ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen4ID" + field.UO_id).setVisible(false); //Ext.getCmp("image8Show" + field.UO_id).setVisible(false);
            //8
            Ext.getCmp("Recommended_label5ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen5ID" + field.UO_id).setVisible(false); //Ext.getCmp("image9Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label6ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen6ID" + field.UO_id).setVisible(false); //Ext.getCmp("image10Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label7ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen7ID" + field.UO_id).setVisible(false); //Ext.getCmp("image11Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label8ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen8ID" + field.UO_id).setVisible(false); //Ext.getCmp("image12Show" + field.UO_id).setVisible(false);
            //12
            Ext.getCmp("Recommended_label9ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen9ID" + field.UO_id).setVisible(false); //Ext.getCmp("image13Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label10ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen10ID" + field.UO_id).setVisible(false); //Ext.getCmp("image14Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label11ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen11ID" + field.UO_id).setVisible(false); //Ext.getCmp("image15Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label12ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen12ID" + field.UO_id).setVisible(false); //Ext.getCmp("image16Show" + field.UO_id).setVisible(false);
            //16
            Ext.getCmp("Recommended_label13ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen13ID" + field.UO_id).setVisible(false); //Ext.getCmp("image17Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label14ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen14ID" + field.UO_id).setVisible(false); //Ext.getCmp("image18Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label15ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen15ID" + field.UO_id).setVisible(false); //Ext.getCmp("image19Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label16ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen16ID" + field.UO_id).setVisible(false); //Ext.getCmp("image20Show" + field.UO_id).setVisible(false);

        }
        if (value == 4) {
            //4
            Ext.getCmp("Recommended_label1ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen1ID" + field.UO_id).setVisible(true); //Ext.getCmp("image5Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label2ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen2ID" + field.UO_id).setVisible(true); //Ext.getCmp("image6Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label3ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen3ID" + field.UO_id).setVisible(true); //Ext.getCmp("image7Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label4ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen4ID" + field.UO_id).setVisible(true); //Ext.getCmp("image8Show" + field.UO_id).setVisible(true);
            //8
            Ext.getCmp("Recommended_label5ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen5ID" + field.UO_id).setVisible(false); //Ext.getCmp("image9Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label6ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen6ID" + field.UO_id).setVisible(false); //Ext.getCmp("image10Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label7ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen7ID" + field.UO_id).setVisible(false); //Ext.getCmp("image11Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label8ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen8ID" + field.UO_id).setVisible(false); //Ext.getCmp("image12Show" + field.UO_id).setVisible(false);
            //12
            Ext.getCmp("Recommended_label9ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen9ID" + field.UO_id).setVisible(false); //Ext.getCmp("image13Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label10ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen10ID" + field.UO_id).setVisible(false); //Ext.getCmp("image14Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label11ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen11ID" + field.UO_id).setVisible(false); //Ext.getCmp("image15Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label12ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen12ID" + field.UO_id).setVisible(false); //Ext.getCmp("image16Show" + field.UO_id).setVisible(false);
            //16
            Ext.getCmp("Recommended_label13ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen13ID" + field.UO_id).setVisible(false); //Ext.getCmp("image17Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label14ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen14ID" + field.UO_id).setVisible(false); //Ext.getCmp("image18Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label15ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen15ID" + field.UO_id).setVisible(false); //Ext.getCmp("image19Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label16ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen16ID" + field.UO_id).setVisible(false); //Ext.getCmp("image20Show" + field.UO_id).setVisible(false);
        }
        else if (value == 8) {
            //4
            Ext.getCmp("Recommended_label1ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen1ID" + field.UO_id).setVisible(true); //Ext.getCmp("image5Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label2ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen2ID" + field.UO_id).setVisible(true); //Ext.getCmp("image6Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label3ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen3ID" + field.UO_id).setVisible(true); //Ext.getCmp("image7Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label4ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen4ID" + field.UO_id).setVisible(true); //Ext.getCmp("image8Show" + field.UO_id).setVisible(true);
            //8
            Ext.getCmp("Recommended_label5ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen5ID" + field.UO_id).setVisible(true); //Ext.getCmp("image9Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label6ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen6ID" + field.UO_id).setVisible(true); //Ext.getCmp("image10Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label7ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen7ID" + field.UO_id).setVisible(true); //Ext.getCmp("image11Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label8ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen8ID" + field.UO_id).setVisible(true); //Ext.getCmp("image12Show" + field.UO_id).setVisible(true);
            //12
            Ext.getCmp("Recommended_label9ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen9ID" + field.UO_id).setVisible(false); //Ext.getCmp("image13Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label10ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen10ID" + field.UO_id).setVisible(false); //Ext.getCmp("image14Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label11ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen11ID" + field.UO_id).setVisible(false); //Ext.getCmp("image15Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label12ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen12ID" + field.UO_id).setVisible(false); //Ext.getCmp("image16Show" + field.UO_id).setVisible(false);
            //16
            Ext.getCmp("Recommended_label13ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen13ID" + field.UO_id).setVisible(false); //Ext.getCmp("image17Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label14ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen14ID" + field.UO_id).setVisible(false); //Ext.getCmp("image18Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label15ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen15ID" + field.UO_id).setVisible(false); //Ext.getCmp("image19Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label16ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen16ID" + field.UO_id).setVisible(false); //Ext.getCmp("image20Show" + field.UO_id).setVisible(false);
        }
        else if (value == 12) {
            //4
            Ext.getCmp("Recommended_label1ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen1ID" + field.UO_id).setVisible(true); //Ext.getCmp("image5Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label2ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen2ID" + field.UO_id).setVisible(true); //Ext.getCmp("image6Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label3ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen3ID" + field.UO_id).setVisible(true); //Ext.getCmp("image7Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label4ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen4ID" + field.UO_id).setVisible(true); //Ext.getCmp("image8Show" + field.UO_id).setVisible(true);
            //8
            Ext.getCmp("Recommended_label5ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen5ID" + field.UO_id).setVisible(true); //Ext.getCmp("image9Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label6ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen6ID" + field.UO_id).setVisible(true); //Ext.getCmp("image10Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label7ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen7ID" + field.UO_id).setVisible(true); //Ext.getCmp("image11Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label8ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen8ID" + field.UO_id).setVisible(true); //Ext.getCmp("image12Show" + field.UO_id).setVisible(true);
            //12
            Ext.getCmp("Recommended_label9ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen9ID" + field.UO_id).setVisible(true); //Ext.getCmp("image13Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label10ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen10ID" + field.UO_id).setVisible(true); //Ext.getCmp("image14Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label11ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen11ID" + field.UO_id).setVisible(true); //Ext.getCmp("image15Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label12ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen12ID" + field.UO_id).setVisible(true); //Ext.getCmp("image16Show" + field.UO_id).setVisible(true);
            //16
            Ext.getCmp("Recommended_label13ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen13ID" + field.UO_id).setVisible(false); //Ext.getCmp("image17Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label14ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen14ID" + field.UO_id).setVisible(false); //Ext.getCmp("image18Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label15ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen15ID" + field.UO_id).setVisible(false); //Ext.getCmp("image19Show" + field.UO_id).setVisible(false);
            Ext.getCmp("Recommended_label16ID" + field.UO_id).setVisible(false); Ext.getCmp("Recommended_DirNomen16ID" + field.UO_id).setVisible(false); //Ext.getCmp("image20Show" + field.UO_id).setVisible(false);
        }
        else if (value == 16) {
            //4
            Ext.getCmp("Recommended_label1ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen1ID" + field.UO_id).setVisible(true); //Ext.getCmp("image5Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label2ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen2ID" + field.UO_id).setVisible(true); //Ext.getCmp("image6Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label3ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen3ID" + field.UO_id).setVisible(true); //Ext.getCmp("image7Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label4ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen4ID" + field.UO_id).setVisible(true); //Ext.getCmp("image8Show" + field.UO_id).setVisible(true);
            //8
            Ext.getCmp("Recommended_label5ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen5ID" + field.UO_id).setVisible(true); //Ext.getCmp("image9Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label6ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen6ID" + field.UO_id).setVisible(true); //Ext.getCmp("image10Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label7ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen7ID" + field.UO_id).setVisible(true); //Ext.getCmp("image11Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label8ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen8ID" + field.UO_id).setVisible(true); //Ext.getCmp("image12Show" + field.UO_id).setVisible(true);
            //12
            Ext.getCmp("Recommended_label9ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen9ID" + field.UO_id).setVisible(true); //Ext.getCmp("image13Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label10ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen10ID" + field.UO_id).setVisible(true); //Ext.getCmp("image14Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label11ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen11ID" + field.UO_id).setVisible(true); //Ext.getCmp("image15Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label12ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen12ID" + field.UO_id).setVisible(true); //Ext.getCmp("image16Show" + field.UO_id).setVisible(true);
            //16
            Ext.getCmp("Recommended_label13ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen13ID" + field.UO_id).setVisible(true); //Ext.getCmp("image17Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label14ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen14ID" + field.UO_id).setVisible(true); //Ext.getCmp("image18Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label15ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen15ID" + field.UO_id).setVisible(true); //Ext.getCmp("image19Show" + field.UO_id).setVisible(true);
            Ext.getCmp("Recommended_label16ID" + field.UO_id).setVisible(true); Ext.getCmp("Recommended_DirNomen16ID" + field.UO_id).setVisible(true); //Ext.getCmp("image20Show" + field.UO_id).setVisible(true);
        }
    },

    //4 === === === === === === === === === === === === === === === ===
    //Клик на изображение - "1"
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
    //Клик на изображение - "2"
    onImage6ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image6Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,   // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "6"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },
    //Клик на изображение - "3"
    onImage7ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image7Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "7"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },
    //Клик на изображение - "4"
    onImage8ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image8Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "8"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },

    //8 === === === === === === === === === === === === === === === ===
    //Клик на изображение - "5"
    onImage9ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image9Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "9"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },
    //Клик на изображение - "6"
    onImage10ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image10Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "10"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },
    //Клик на изображение - "7"
    onImage11ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image11Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "11"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },
    //Клик на изображение - "8"
    onImage12ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image12Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "12"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },

    //12 === === === === === === === === === === === === === === === ===
    //Клик на изображение - "9"
    onImage13ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image13Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "13"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },
    //Клик на изображение - "10"
    onImage14ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image14Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "14"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },
    //Клик на изображение - "11"
    onImage15ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image15Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "15"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },
    //Клик на изображение - "12"
    onImage16ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image16Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "16"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },


    //16 === === === === === === === === === === === === === === === ===
    //Клик на изображение - "13"
    onImage17ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image17Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "17"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },
    //Клик на изображение - "14"
    onImage18ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image18Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "18"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },
    //Клик на изображение - "15"
    onImage19ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image19Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "19"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },
    //Клик на изображение - "16"
    onImage20ShowClick: function (aButton, aEvent, param1) {
        var Params = [
            "image20Show" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            "20"
        ]
        ObjectEditConfig("viewDirNomensImg", Params);
    },


});

