Ext.define('PartionnyAccount.viewcontroller.Sklad/Container/controllerContainerCentral', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.controllerContainerCentral',

    onBtnMenu: function (button, pressed) {


        //Один клик *** *** *** *** *** *** *** *** *** ***
        var treelist = this.lookupReference('treelist');
        treelist.setConfig("expanderOnly", false);


        //Стиль *** *** *** *** *** *** *** *** *** ***
        var treelist = this.lookupReference('treelist'),
            ct = this.lookupReference('treelistContainer');

        treelist.setExpanderFirst(!pressed);
        treelist.setUi(pressed ? 'nav' : null);
        treelist.setHighlightPath(pressed);
        ct[pressed ? 'addCls' : 'removeCls']('treelist-with-nav');

        if (Ext.isIE8) {
            this.repaintList(treelist);
        }


        //Микро-стиль *** *** *** *** *** *** *** *** *** ***
        var treelist = this.lookupReference('treelist'),
            navBtn = this.lookupReference('navBtn'),
            ct = treelist.ownerCt;

        treelist.setMicro(pressed);

        if (pressed) {
            navBtn.setPressed(true);
            navBtn.disable();
            this.oldWidth = ct.width;
            ct.setWidth(44);
        }
        else {
            ct.setWidth(this.oldWidth);
            navBtn.enable();
        }

    },

    onTreelisttemClick: function (view, record, item) { //, index, e, eOpts
        
        switch (record.node.id)
        {
            case "DocRetails":

                var Params = [
                    "viewContainerCentral",
                    false, //UO_Center
                    false, //UO_Modal
                    //2     // 1 - Новое, 2 - Редактировать
                ]
                ObjectConfig("viewDocRetails", Params);

                break;
            case "viewDocRetailsEdit": alert(record.node.id); break;

            case "JourRetailReturns": alert(record.node.id); break;
            case "DocRetailReturns": alert(record.node.id); break;

            case "DocRetailReturns": alert(record.node.id); break;
        }

    },

    

    //*** OLD *** *** ***

    onToggleConfig: function (menuitem) {
        var treelist = this.lookupReference('treelist');

        treelist.setConfig(menuitem.config, menuitem.checked);
    },

    onToggleMicro: function (button, pressed) {
        var treelist = this.lookupReference('treelist'),
            navBtn = this.lookupReference('navBtn'),
            ct = treelist.ownerCt;

        treelist.setMicro(pressed);

        if (pressed) {
            //navBtn.setPressed(true);
            //navBtn.disable();
            this.oldWidth = ct.width;
            ct.setWidth(44);
        } else {
            ct.setWidth(this.oldWidth);
            //navBtn.enable();
        }

        // IE8 has an odd bug with handling font icons in pseudo elements;
        // it will render the icon once and not update it when something
        // like text color is changed via style addition or removal.
        // We have to force icon repaint by adding a style with forced empty
        // pseudo element content, (x-sync-repaint) and removing it back to work
        // around this issue.
        // See this: https://github.com/FortAwesome/Font-Awesome/issues/954
        // and this: https://github.com/twbs/bootstrap/issues/13863
        if (Ext.isIE8) {
            this.repaintList(treelist, pressed);
        }
    },

    onToggleNav: function (button, pressed) {
        var treelist = this.lookupReference('treelist'),
            ct = this.lookupReference('treelistContainer');

        treelist.setExpanderFirst(!pressed);
        treelist.setUi(pressed ? 'nav' : null);
        treelist.setHighlightPath(pressed);

        treelist[pressed ? 'addCls' : 'removeCls']('x-treelist-navigation'); //main-toolbar shadow
        ct[pressed ? 'addCls' : 'removeCls']('x-treelist-navigation'); //main-toolbar shadow
        //('x-treelist x-unselectable x-treelist-navigation x-treelist-expander-only x-box-item'); 
        //('treelist-with-nav');



        //ct.body.setStyle('background-color', '#32404e');
        //treelist.setStyle('background-color', '#32404e');

        //ct.removeClass('myClass');
        //ct.body.setStyle('background-color', '#32404e')
        //ct.body.setStyle('background-color', '#32404e')


        if (Ext.isIE8) {
            this.repaintList(treelist);
        }
    },

    //*** OLD *** *** ***

    

    repaintList: function (treelist, microMode) {
        treelist.getStore().getRoot().cascadeBy(function (node) {
            var item, toolElement;

            item = treelist.getItem(node);

            if (item && item.isTreeListItem) {
                if (microMode) {
                    toolElement = item.getToolElement();

                    if (toolElement && toolElement.isVisible(true)) {
                        toolElement.syncRepaint();
                    }
                }
                else {
                    if (item.element.isVisible(true)) {
                        item.iconElement.syncRepaint();
                        item.expanderElement.syncRepaint();
                    }
                }
            }
        });
    }
});