Ext.define('PartionnyAccount.viewmodel.Sklad/Container/viewmodelContainerCentral', {
    extend: 'Ext.app.ViewModel',

    alias: 'viewmodel.viewmodelContainerCentral',

    /*
    //Пишет текст на панель справа
    formulas: {
        selectionText: function (get) {
            var selection = get('treelist.selection'),
                path;
            if (selection) {
                path = selection.getPath('text');
                path = path.replace(/^\/Root/, '');
                return 'Selected: ' + path;
            } else {
                return 'No node selected';
            }
        }
    },
    */

    stores: {
        navItems: {
            type: 'tree',

            root: {
                expanded: true,
                children: [

                    {
                        id: "Retail",
                        text: 'Чек',
                        iconCls: 'x-fa fa-barcode',
                        children: [
                            {
                                id: "DocRetails",
                                text: 'Список',
                                iconCls: 'x-fa fa-list-alt',
                                leaf: true
                            },
                            {
                                id: "viewDocRetailsEdit",
                                text: 'Новый',
                                iconCls: 'x-fa fa-newspaper-o',
                                leaf: true
                            }
                        ]
                    },

                    {
                        id: "RetailReturns",
                        text: 'Возврат',
                        iconCls: 'x-fa fa-external-link-square',
                        children: [
                            {
                                id: "DocRetailReturns",
                                text: 'Список',
                                iconCls: 'x-fa fa-list-alt',
                                leaf: true
                            },
                            {
                                id: "DocRetailReturnsEdit",
                                text: 'Новый',
                                iconCls: 'x-fa fa-newspaper-o',
                                leaf: true
                            }
                        ]
                    },

                    {
                        id: "Report",
                        text: 'Отчеты',
                        iconCls: 'x-fa fa-bar-chart',
                        children: [
                            {
                                id: "ReportCash",
                                text: 'Денег в кассе',
                                iconCls: 'x-fa fa-money',
                                leaf: true
                            }
                        ]
                    }

                ]
            }

        }
    }
});