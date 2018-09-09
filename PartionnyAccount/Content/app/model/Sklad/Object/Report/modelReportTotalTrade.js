Ext.define('PartionnyAccount.model.Sklad/Object/Report/modelReportTotalTrade', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "ListObjectNameRu" },       //Тип документа
        { name: "RemPartyID" },             //Партия

        { name: "DirNomenID" },             //Код товара
        { name: "DirNomenName" },           //Товар Наименование

        { name: "PriceCurrency" },          //Цена закупки
        { name: "PriceRetailVAT" },         //Цена продажи: Розница
        { name: "PriceRetailCurrency" },         //Цена продажи: Розница
        { name: "PriceWholesaleVAT" }, { name: "PriceWholesaleCurrency" },     //Цена продажи: Опт
        { name: "PriceIMVAT" },             //Цена продажи: И-М
        { name: "SumQuantity" },            //Сумма Пришло
        { name: "Purch_PriceCurrency" },    //Цена закупки
        //{ name: "Purch_Sums" },             //Сумма закупки
        { name: "Sale_PriceCurrency" },     //Цена продажи
        { name: "Sale_Quantity" },          //К-во
        { name: "Sums" },                   //Сумма
        { name: "SumProfit" },              //Прибыль
        { name: "Sale_Discount" },          //Скидка

        { name: "Quantity" },               //К-во
        { name: "Remnant" },                //Остаток
        { name: "SumRemnant" },             //Сумма Остатка
        { name: "DirNomenMinimumBalance" }, //Минимальный остаток

        { name: "DirEmployeeName" },        //Продавец
        { name: "DocDate", type: "date" },  //Дата
        { name: "DirWarehouseName" },       //Точка

        //Характеристики
        { name: "DirCharColourID" },        //Характеристики
        { name: "DirCharMaterialID" },      //Характеристики
        { name: "DirCharNameID" },          //Характеристики
        { name: "DirCharSeasonID" },        //Характеристики
        { name: "DirCharSexID" },           //Характеристики
        { name: "DirCharSizeID" },          //Характеристики
        { name: "DirCharStyleID" },         //Характеристики
        { name: "DirCharTextureID" },       //Характеристики

        { name: "DirChar" },                //Характеристики


        { name: "DirReturnTypeID" }, { name: "DirReturnTypeName" },   //Тип
        { name: "DirDescriptionID" }, { name: "DirDescriptionName" }, //Причина


        //Дополнительно
        { name: "DirContractorIDOrg" },
        { name: "DirContractorID" },
        { name: "DirWarehouseIDDebit" },
        { name: "DirWarehouseIDPurch" },
        { name: "DirCurrencyID" },
        { name: "DirCurrencyRate" },
        { name: "DirCurrencyMultiplicity" },

    ]
});