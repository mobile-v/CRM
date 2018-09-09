/*
    Variables
*/

//Название сервиса
lanOrgName = " СРМ 'Торговля и Сервисный центр'";

//Системные
let varCopyrightSystem = "Copyright © 2018";
let verSystem = "версия 3.2.3.1001";
let varSystemDate = "от 2018.09.05";
let varSystemDevelop = "Сайт: <a href='https://intradecloud.com/' target=blank>Учет в Торговле и Сервисном центре</a>";


// *** *** *** Глобальные переменные *** *** *** *** *** 

/*
    HTTP
*/
//Help
let HTTP_Help = "https://intradecloud.com/pomoshch/";
let HTTP_Pay = "https://intradecloud.com/payment";
//MSSQL
let HTTP_DirCountriesGridMSSQL = "api/login/Dir/DirCountriesMSSQL/";
let HTTP_DirLanguagesGridMSSQL = "api/login/Dir/DirLanguagesMSSQL/";
let HTTP_DirCustomersMSSQL = "api/login/Dir/DirCustomersMSSQL/";
//Sys
let HTTP_SysSettingsSet = "api/sklad/Sys/SysSettings/"; //!!! Не для Грида !!!
let HTTP_SysSettings = "api/sklad/Sys/SysSettings/";
let HTTP_SysJourDispsGrid = "api/sklad/Sys/SysJourDisps/";
//Dir
//?type=Grid && ?type=Tree
let HTTP_DirNomens = "api/sklad/Dir/DirNomens/"; let HTTP_DirNomenHistories = "api/sklad/Dir/DirNomenHistories/"; //let HTTP_Image = "api/sklad/Dir/DirNomensImg/";
let HTTP_DirNomenCategories = "api/sklad/Dir/DirNomenCategories/";
let HTTP_DirServiceNomens = "api/sklad/Dir/DirServiceNomens/"; let HTTP_DirServiceNomenHistories = "api/sklad/Dir/DirServiceNomenHistories/";
let HTTP_DirServiceNomenCategories = "api/sklad/Dir/DirServiceNomenCategories/";
let HTTP_DirWarehouses = "api/sklad/Dir/DirWarehouses/"; 
let HTTP_DirCurrencies = "api/sklad/Dir/DirCurrencies/"; let HTTP_DirCurrencyHistories = "api/sklad/Dir/DirCurrencyHistories/"; 
let HTTP_DirEmployees = "api/sklad/Dir/DirEmployees/"; let HTTP_DirEmployeeHistories = "api/sklad/Dir/DirEmployeeHistories/"; let HTTP_DirEmployeeWarehouses = "api/sklad/Dir/DirEmployeeWarehouses/";
let HTTP_DirBonuses = "api/sklad/Dir/DirBonuses/"; let HTTP_DirBonusTabs = "api/sklad/Dir/DirBonusTabs/";
let HTTP_DirBonus2es = "api/sklad/Dir/DirBonus2es/"; let HTTP_DirBonus2Tabs = "api/sklad/Dir/DirBonus2Tabs/";
let HTTP_DirBanks = "api/sklad/Dir/DirBanks/";
let HTTP_DirCashOffices = "api/sklad/Dir/DirCashOffices/"; 
let HTTP_DirContractors = "api/sklad/Dir/DirContractors/"; 
let HTTP_DirContractor1Types = "api/sklad/Dir/DirContractor1Types/"; 
let HTTP_DirContractor2Types = "api/sklad/Dir/DirContractor2Types/"; 
let HTTP_DirVats = "api/sklad/Dir/DirVats/"; 
let HTTP_DirDiscounts = "api/sklad/Dir/DirDiscounts/"; let HTTP_DirDiscountTabs = "api/sklad/Dir/DirDiscountTabs/";
let HTTP_DirNomenTypes = "api/sklad/Dir/DirNomenTypes/";
let HTTP_DirCharColours = "api/sklad/Dir/DirCharColours/";
let HTTP_DirCharMaterials = "api/sklad/Dir/DirCharMaterials/";
let HTTP_DirCharNames = "api/sklad/Dir/DirCharNames/";
let HTTP_DirCharSeasons = "api/sklad/Dir/DirCharSeasons/";
let HTTP_DirCharSexes = "api/sklad/Dir/DirCharSexes/";
let HTTP_DirCharSizes = "api/sklad/Dir/DirCharSizes/";
let HTTP_DirCharStyles = "api/sklad/Dir/DirCharStyles/";
let HTTP_DirCharTextures = "api/sklad/Dir/DirCharTextures/";
let HTTP_DirPaymentTypes = "api/sklad/Dir/DirPaymentTypes/";
let HTTP_DirCashOfficeSumTypes = "api/sklad/Dir/DirCashOfficeSumTypes/";
let HTTP_DirBankSumTypes = "api/sklad/Dir/DirBankSumTypes/";
let HTTP_DirPriceTypes = "api/sklad/Dir/DirPriceTypes/";
let HTTP_DirDescriptions = "api/sklad/Dir/DirDescriptions/";
let HTTP_DirReturnTypes = "api/sklad/Dir/DirReturnTypes/";
let HTTP_DirMovementDescriptions = "api/sklad/Dir/DirMovementDescriptions/";
let HTTP_DirMovementStatuses = "api/sklad/Dir/DirMovementStatuses/";
let HTTP_DirDomesticExpenses = "api/sklad/Dir/DirDomesticExpenses/";
let HTTP_DirOrdersStates = "api/sklad/Dir/DirOrdersStates/";
//Dir - Service
let HTTP_DirServiceContractors = "api/sklad/Dir/DirServiceContractors/";
let HTTP_DirServiceStatuses = "api/sklad/Dir/DirServiceStatuses/";
let HTTP_DirServiceJobNomens = "api/sklad/Dir/DirServiceJobNomens/"; let HTTP_DirServiceJobNomenHistories = "api/sklad/Dir/DirServiceJobNomenHistories/";
let HTTP_DirServiceComplects = "api/sklad/Dir/DirServiceComplects/";
let HTTP_DirServiceProblems = "api/sklad/Dir/DirServiceProblems/";
let HTTP_DirSmsTemplates = "api/sklad/Dir/DirSmsTemplates/";
let HTTP_DirServiceDiagnosticRresults = "api/sklad/Dir/DirServiceDiagnosticRresults/";
let HTTP_DirServiceNomenTypicalFaults = "api/sklad/Dir/DirServiceNomenTypicalFaults/";
let HTTP_DirServiceNomenPrices = "api/sklad/Dir/DirServiceNomenPrices/";
//Dir - SecondHand
let HTTP_DirSecondHandStatuses = "api/sklad/Dir/DirSecondHandStatuses/";
//Doc
let HTTP_DocCashOfficeSums = "api/sklad/Doc/DocCashOfficeSums/"; let HTTP_DocBankSums = "api/sklad/Doc/DocBankSums/"; let HTTP_DocCashOfficeSumMovements = "api/sklad/Doc/DocCashOfficeSumMovements/";
let HTTP_DocPurches = "api/sklad/Doc/DocPurches/"; let HTTP_DocPurchTabs = "api/sklad/Doc/DocPurchTabs/";
let HTTP_DocSales = "api/sklad/Doc/DocSales/"; let HTTP_DocSaleTabs = "api/sklad/Doc/DocSaleTabs/";
let HTTP_DocMovements = "api/sklad/Doc/DocMovements/"; let HTTP_DocMovementTabs = "api/sklad/Doc/DocMovementTabs/";
let HTTP_DocReturnVendors = "api/sklad/Doc/DocReturnVendors/"; let HTTP_DocReturnVendorTabs = "api/sklad/Doc/DocReturnVendorTabs/";
let HTTP_DocActWriteOffs = "api/sklad/Doc/DocActWriteOffs/"; let HTTP_DocActWriteOffTabs = "api/sklad/Doc/DocActWriteOffTabs/";
let HTTP_DocReturnsCustomers = "api/sklad/Doc/DocReturnsCustomers/"; let HTTP_DocReturnsCustomerTabs = "api/sklad/Doc/DocReturnsCustomerTabs/";
let HTTP_DocActOnWorks = "api/sklad/Doc/DocActOnWorks/"; let HTTP_DocActOnWorkTabs = "api/sklad/Doc/DocActOnWorkTabs/";
let HTTP_DocAccounts = "api/sklad/Doc/DocAccounts/"; let HTTP_DocAccountTabs = "api/sklad/Doc/DocAccountTabs/";
let HTTP_DocInventories = "api/sklad/Doc/DocInventories/"; let HTTP_DocInventoryTabs = "api/sklad/Doc/DocInventoryTabs/";
let HTTP_DocRetails = "api/sklad/Doc/DocRetails/"; let HTTP_DocRetailTabs = "api/sklad/Doc/DocRetailTabs/";
let HTTP_DocRetailReturns = "api/sklad/Doc/DocRetailReturns/"; let HTTP_DocRetailReturnTabs = "api/sklad/Doc/DocRetailReturnTabs/";
let HTTP_DocRetailActWriteOffs = "api/sklad/Doc/DocRetailActWriteOffs/";
let HTTP_DocOrderInts = "api/sklad/Doc/DocOrderInts/";
let HTTP_DocNomenRevaluations = "api/sklad/Doc/DocNomenRevaluations/"; let HTTP_DocNomenRevaluationTabs = "api/sklad/Doc/DocNomenRevaluationTabs/";
//Doc - Service
let HTTP_DocServicePurches = "api/sklad/Doc/DocServicePurches/";
let HTTP_DocServicePurch1Tabs = "api/sklad/Doc/DocServicePurch1Tabs/";
let HTTP_DocServicePurch2Tabs = "api/sklad/Doc/DocServicePurch2Tabs/";
let HTTP_DocServiceMovs = "api/sklad/Doc/DocServiceMovs/";
let HTTP_DocServiceMovTabs = "api/sklad/Doc/DocServiceMovTabs/";
let HTTP_DocServiceInvs = "api/sklad/Doc/DocServiceInvs/";
let HTTP_DocServiceInvTabs = "api/sklad/Doc/DocServiceInvTabs/";
//Doc - SecondHand
let HTTP_DocSecondHandPurches = "api/sklad/Doc/DocSecondHandPurches/";
let HTTP_DocSecondHandPurch1Tabs = "api/sklad/Doc/DocSecondHandPurch1Tabs/";
let HTTP_DocSecondHandPurch2Tabs = "api/sklad/Doc/DocSecondHandPurch2Tabs/";
//let HTTP_DocSecondHandRetails = "api/sklad/Doc/DocSecondHandRetails/"; let HTTP_DocSecondHandRetailTabs = "api/sklad/Doc/DocSecondHandRetailTabs/";
//let HTTP_DocSecondHandRetailReturns = "api/sklad/Doc/DocSecondHandRetailReturns/";
//let HTTP_DocSecondHandRetailActWriteOffs = "api/sklad/Doc/DocSecondHandRetailActWriteOffs/";
//let HTTP_DocSecondHandMovements = "api/sklad/Doc/DocSecondHandMovements/";
//let HTTP_DocSecondHandMovementTabs = "api/sklad/Doc/DocSecondHandMovementTabs/";
let HTTP_DocSecondHandMovs = "api/sklad/Doc/DocSecondHandMovs/";
let HTTP_DocSecondHandMovTabs = "api/sklad/Doc/DocSecondHandMovTabs/";
//let HTTP_DocSecondHandInventories = "api/sklad/Doc/DocSecondHandInventories/";
//let HTTP_DocSecondHandInventoryTabs = "api/sklad/Doc/DocSecondHandInventoryTabs/";
let HTTP_DocSecondHandInvs = "api/sklad/Doc/DocSecondHandInvs/";
let HTTP_DocSecondHandInvTabs = "api/sklad/Doc/DocSecondHandInvTabs/";
let HTTP_DocSecondHandRazbors = "api/sklad/Doc/DocSecondHandRazbors/";
//let HTTP_DocSecondHandRazborTabs = "api/sklad/Doc/DocSecondHandRazborTabs/";
let HTTP_DocSecondHandRazbor2Tabs = "api/sklad/Doc/DocSecondHandRazbor2Tabs/";
let HTTP_DocSecondHandSales = "api/sklad/Doc/DocSecondHandSales/";
let HTTP_DocSecondHandReturns = "api/sklad/Doc/DocSecondHandReturns/";
//Doc - Salary
let HTTP_DocSalaries = "api/sklad/Doc/DocSalaries/"; let HTTP_DocSalaryTabs = "api/sklad/Doc/DocSalaryTabs/";
//Doc - Salary
let HTTP_DocDomesticExpenses = "api/sklad/Doc/DocDomesticExpenses/"; let HTTP_DocDomesticExpenseTabs = "api/sklad/Doc/DocDomesticExpenseTabs/";
//Doc - Logistics
let HTTP_Logistics = "api/sklad/Logistic/Logistics/"; let HTTP_LogisticTabs = "api/sklad/Logistic/LogisticTabs/";
//Pay
let HTTP_Pays = "api/sklad/Pay/Pay/";
//Report
let HTTP_ReportTotalTrade = "api/sklad/Report/ReportTotalTrade/";
let HTTP_ReportBanksCashOffices = "api/sklad/Report/ReportBanksCashOffices/";
let HTTP_DocServicePurchesReport = "api/sklad/Report/DocServicePurchesReport/";
let HTTP_ReportLogistics = "api/sklad/Report/ReportLogistics/";
let HTTP_ReportSalaries = "api/sklad/Report/ReportSalaries/"; let HTTP_ReportSalariesWarehouses = "api/sklad/Report/ReportSalariesWarehouses/";
//Rem
let HTTP_RemParties = "api/sklad/Rem/RemParties/"; //let HTTP_Rem2Parties = "api/sklad/Rem/Rem2Parties/";
let HTTP_RemPartyMinuses = "api/sklad/Rem/RemPartyMinuses/"; //let HTTP_Rem2PartyMinuses = "api/sklad/Rem/Rem2PartyMinuses/";
//List
let HTTP_ListObjects = "api/sklad/List/ListObjects/";
let HTTP_ListObjectFieldNames = "api/sklad/List/ListObjectFieldNames/";
let HTTP_ListLanguages = "api/sklad/List/ListLanguages/";
let HTTP_ListObjectPFs = "api/sklad/List/ListObjectPFs/";
let HTTP_ListObjectPFTabs = "api/sklad/List/ListObjectPFTabs/";
//Service
let HTTP_ImportsDocPurchesExcel = "api/WebApi/ExchangeData/ImportsDocPurchesExcel/";
let HTTP_Api10 = "API/API10/"; //"api/Sklad/Service/API/API10/"; - можно и так указать, но тогда надо в WebApiConfig так же указать!
let HTTP_DirWebShopUO = "API/DirWebShopUOs/"; 
//Image
let HTTP_Image = "api/sklad/Dir/Image/"; //let HTTP_DirMshnsImg = "api/sklad/Dir/DirMshnsImg/";
//Log
let HTTP_LogServices = "api/sklad/Log/LogServices/";
let HTTP_LogMovements = "api/sklad/Log/LogMovements/"; let HTTP_LogLogistics = "api/sklad/Log/LogLogistics/";
let HTTP_LogOrderInts = "api/sklad/Log/LogOrderInts/";
let HTTP_LogSecondHands = "api/sklad/Log/LogSecondHands/";
let HTTP_LogSecondHandRazbors = "api/sklad/Log/LogSecondHandRazbors/";
//Sms
let HTTP_Sms = "api/sklad/SMS/Sms/";
//Timer
let HTTP_Timer = "api/Timer/";


/* OLD */
let HTTP_DirBankAccountsGrid = "api/sklad/Dir/DirBankAccounts/";
let HTTP_DirContractsGrid = "api/sklad/Dir/DirContracts/";
let HTTP_DirBankAccountTypesGrid = "api/sklad/Dir/DirBankAccountTypes/";
let HTTP_DirCountriesGrid = "api/sklad/Dir/DirCountries/";
let HTTP_DirDepartmentsGrid = "api/sklad/Dir/DirDepartments/";
let HTTP_DirPostsGrid = "api/sklad/Dir/DirPosts/"; let HTTP_DirPostGroups = "api/sklad/Dir/DirPostGroups/";
let HTTP_DirPriceTypesGrid = "api/sklad/Dir/DirPriceTypes/";
let HTTP_DirNomensGrid = "api/sklad/Dir/DirNomens/"; let HTTP_DirNomenGroups = "api/sklad/Dir/DirNomenGroups/"; let HTTP_DirNomenHistoriesGrid = "api/sklad/Dir/DirNomenHistories/";
let HTTP_DirBarCodesGrid = "api/sklad/Dir/DirBarCodes/";
let HTTP_DirUnitMeasuresGrid = "api/sklad/Dir/DirUnitMeasures/";
let HTTP_DirPointSalesGrid = "api/sklad/Dir/DirPointSales/";
let HTTP_DirBudgetClassesGrid = "api/sklad/Dir/DirBudgetClasses/";
let HTTP_DirBudgetLevelsGrid = "api/sklad/Dir/DirBudgetLevels/";
let HTTP_DirOKATOesGrid = "api/sklad/Dir/DirOKATOes/";
let HTTP_DirTaxCodesGrid = "api/sklad/Dir/DirTaxCodes/";
let HTTP_DirOrdersStatesGrid = "api/sklad/Dir/DirOrdersStates/"; let HTTP_DirOrdersStateGroups = "api/sklad/Dir/DirOrdersStateGroups/";
let HTTP_DirPostViewsGrid = "api/sklad/Dir/DirPostViews/";
let HTTP_DirPostTypesGrid = "api/sklad/Dir/DirPostTypes/";
let HTTP_DirPostImportancesGrid = "api/sklad/Dir/DirPostImportances/";
let HTTP_DirPostUrgenciesGrid = "api/sklad/Dir/DirPostUrgencies/";


//Контекстное Меню (правая кнопка мыши) для Грида
/*let contextMenu = Ext.create('Ext.menu.Menu', {
    width: 200,
    items: [
        {
            text: 'Preview',
            handler: function (par1, par2) {
                let record = grid ? grid.getSelection()[0] : null;
                if (!record) {
                    return;
                }
                alert(record.get('name'));
            }
        }
    ]
});*/


//Лоадер
var varLoadingMaskUnion// = new Ext.LoadMask({ msg: 'Please wait...', target: Ext.getCmp("viewContainerCentral") }); 

//Шрифты (Header menu)
let HeaderMenu_FontSize_1 = 3; //Верхнее меню, 1-й уровень (ContainerHeader.js)
let HeaderMenu_FontSize_2 = 2; //Верхнее меню, 2-й уровень (ContainerHeader.js)

//При изменении цен, что бы не изменялись все одновременно.
let varPriceChange_ReadOnly = false;
//ID-шник для Виджетов, которые выводящиеся на экран более 1-го раза.
let ObjectID = 0; // GLOBAL_ID
//ID-шник для Store спецификации (каждой позиции). Хотя может использоватся везде где только можно.
//Т.к. в 6-й версии появился "store.data.items[1].data.id" (в 4-й не было). 
//1. При вставке записи в спецификацию этот id-шник может повторятся, т.к. Мы можем вставить 2-ы одну и ту же запись.
//2. При проставлении количества "Quantity" глюки: меняем количество в переменно "rec", а оно меняется в store, т.к. id-шники обоих совпадают, если запись из подбора товара уже вставлена в спецификацию.
let ObjectTOtherID = 0; // GLOBAL_ID

//Расположение окон объектов
let ObjectXY = 0;

//Размеры окошка: Windows: height, width
let varArrSizeList = [250, 450];
let varArrSizeDir = [350, 600];
let varArrSizeJurn = [305, 650];
let varArrSizePay = [250, 600];



//Переменные

//Типичные неисправности
let Fault1Name = "Замена дисплейного модуля (экран+сенсор в сборе)";
let Fault2Name = "Замена сенсорного стекла (тачскрина)";
let Fault3Name = "Замена разъема зарядки";
let Fault4Name = "Замена разъема sim-карты";
let Fault5Name = "Обновление ПО (прошивка)";
let Fault6Name = "Замена динамика (слуховой)";
let Fault7Name = "Замена микрофона";
let Fault8Name = "Замена динамика (звонок)";
let Fault9Name = "Восстановление после попадания жидкости";
let Fault10Name = "Восстановление цепи питания";
let Fault11Name = "Ремонт материнской платы";
let Fault12Name = "Резерв-5";
let Fault13Name = "Резерв-6";
let Fault14Name = "Резерв-7";


let SearchType_values = [
    [1, "Артикул"] //В товаре (код)
    /*
    ,
    [2, 'В товаре (старый код)'],
    [3, 'В товаре (артикул)'],
    [4, 'В товаре (наименование)'],
    [5, 'В товаре (полное наименование)'],
    [1000, 'В партиях (код, серии, штрих-код)'],
    */
];

let DirDomesticExpense_values = [
    [1, "Другие"],
    [2, "Зарплата"],
    [3, "Инкасация"]
];

let DirOrderIntType_values = [
    [1, "Предзаказы (Лёгкий)"],
    [2, "Впрок (Аналитика)"],
    [3, "Мастерская"],
    [4, "Б/У"],
];

//Гарантия в Мастерскрй СС
let DocYear_values = [
    [2017, "2017"],
    [2018, "2018"],
    [2019, "2019"],
    [2020, "2020"],
    [2021, "2021"],
    [2022, "2022"],
    [2023, "2023"],
    [2024, "2024"],
    [2025, "2025"],
    [2026, "2026"],
    [2027, "2027"],
    [2028, "2028"],
];

//Гарантия в Мастерскрй СС
let ServiceTypeRepair_values = [
    [1, "1 месяц"],
    [2, "2 месяц"],
    [3, "3 месяц"],
    [4, "4 месяц"],
    [5, "5 месяц"],
    [6, "6 месяц"],
    [7, "7 месяц"],
    [8, "8 месяц"],
    [9, "9 месяц"],
    [10, "10 месяц"],
    [11, "11 месяц"],
    [12, "12 месяц"],
];

//Шаблоны СМС
//СЦ
let DirSmsTemplateType_values1 = [
    [1, "Сервисный центр: Другое"],
    [2, "Сервисный центр: Отремонтированный"],
    [3, "Сервисный центр: Отказной"],
    [4, "Сервисный центр: Выдан"],
];
//Логистика
let DirSmsTemplateType_values2 = [
    [1, "Логистика"],
];
//Заказы
let DirSmsTemplateType_values3 = [
    [1, "Заказы"],
];

let DirDomesticExpenses_Sign_values = [
    [-1, "Изъятие из Кассы/Банка"],
    [1, "Поступление в Кассу/Банк"],
];
let SalaryPercentTradeType_values = [
    [1, "Процент с продажи"],
    [2, "Процент с прибыли"],
    [3, "Фиксированная сумма с каждой продажи"],
];
let SalaryPercentService1TabsType_values = [
    [1, "Процент с суммы всех работ"],
    [2, "Фиксированная сумма за все работы в рамках одного ремонта"],
    [3, "Фиксированная сумма с каждого ремнта"],
];
let SalaryPercentService2TabsType_values = [
    [1, "Процент с продажи"],
    [2, "Процент с прибыли"],
    [3, "Фиксированная сумма за одну использованную к ремонту запчасти"],
];



//Гарантия в Мастерскрй СС
/*let IsAdminNameRu_values = [
    [1, "Администратор"],
    [0, "Не Администратор"],
];*/
let urlBaseImg = '../Scripts/sklad/images/';
let IsAdminNameRu_store = Ext.create('Ext.data.Store', {
    fields: ['IsAdmin', 'IsAdminNameRu', "img"],
    data: [
        //{ IsAdminNameRu: 'Администратор', IsAdminNameRuName: urlBaseImg + 'add.png' },
        //{ IsAdminNameRu: 'Не Администратор', IsAdminNameRuName: urlBaseImg + 'cancel.png' },
        {
            //IsAdmin: true,
            IsAdminNameRu: 'Администратор',
            img: urlBaseImg + 'add.png'
        },
        {
            //IsAdmin: false,
            IsAdminNameRu: 'Не Администратор',
            img: urlBaseImg + 'cancel.png'
        },
    ]
});
let WarehouseAllNameRu_store = Ext.create('Ext.data.Store', {
    fields: ['WarehouseAll', 'WarehouseAllNameRu', "img"],
    data: [
        //{ WarehouseAllNameRu: 'Администратор', WarehouseAllNameRuName: urlBaseImg + 'add.png' },
        //{ WarehouseAllNameRu: 'Не Администратор', WarehouseAllNameRuName: urlBaseImg + 'cancel.png' },
        {
            //WarehouseAll: true,
            WarehouseAllNameRu: 'Виден',
            img: urlBaseImg + 'add.png'
        },
        {
            //WarehouseAll: false,
            WarehouseAllNameRu: 'Не Виден',
            img: urlBaseImg + 'cancel.png'
        },
    ]
});

let ExistName_store = Ext.create('Ext.data.Store', {
    fields: ['Exist', 'ExistName', "img"],
    data: [
        {
            Exist: true,
            ExistName: 'Присутствует',
            img: urlBaseImg + 'add.png'
        },
        {
            Exist: false,
            ExistName: 'Отсутствует', //'Списывается с ЗП',
            img: urlBaseImg + 'cancel.png'
        },
        /*
        {
            Exist: false,
            ExistName: 'Отсутствует',
            img: urlBaseImg + 'disabled.png'
        },
        {
            Exist: false,
            ExistName: 'На разбор',
            img: urlBaseImg + 'activation.png'
        },
        */
    ]
});

//Таймаут по умолчанию:
let varTimeOutDefault = 60000; // 60 seconds, 1 minute

let varBlock = false;


/*
    Request to Server
*/

//К-во попыток запроса в Реквестах к серверу в случае не удачи
let varCountErrorRequest = 2;

//Получаем значение переменно:
let varCountErrorSettingsRequest = 0;
let varJurDateS = "2016-08-01";     //Дата начала периода.
let varJurDatePo = "2020-12-31";    //Дата начала периода.
let varFractionalPartInSum = 2;     //К-во знаков после запятой.
let varFractionalPartInPrice = 2;   //К-во знаков после запятой.
let varFractionalPartInOther = 2;   //К-во знаков после запятой.
let varDirVatValue = 0;             //Наименование "Ставки НДС"
let varChangePriceNomen = false;    //Пересчитывать цены Номенклатуры, при проведении Приходной накладной.
let varMethodAccounting = 1;        //Метод учёта
let varDeletedRecordsShow = true;   //Показывать удалённые записи.
let varDirContractorIDOrg = 1;      //Организация по умолчанию
let varDirContractorNameOrg = "";      //Организация по умолчанию
let varDirCurrencyID = 1;           //Код "Валюты"
let varDirCurrencyName = "";        //"Валюта"
let varDirCurrencyNameShort = "";        //"Валюта"
let varDirCurrencyRate = 1;         //"Курс"
let varDirCurrencyMultiplicity = 1; //"Кратность"
let varDirWarehouseID = 1;          //По Умолчанию в настройках
let varMarkupRetail = 30;
let varMarkupWholesale = 20;
let varMarkupIM = 20;
let varMarkupSales1 = 5;
let varMarkupSales2 = 10;
let varMarkupSales3 = 15;
let varMarkupSales4 = 20;
let varCashBookAdd = false;
let varReserve = false;
let varBarIntNomen = 9901;
let varBarIntContractor = 9902;
let varBarIntDoc = 9903;
let varSelectOneClick = true;
let varDateFormat = 1; let DateFormatStr = "Y-m-d";
//К-во выводимых строк в Гриде
let varPageSizeDir = 11;
let varPageSizeJurn = 9;
//Минимальный остаток
let varDirNomenMinimumBalance = 1;
//DirEmployees
let varDirWarehouseIDEmpl = 1;    //Сотрудник может совершать операции только по этому складу
let varDirWarehouseNameEmpl = "";
let varDirContractorIDOrgEmpl = 0;//Сотрудник может совершать операции только по этой организации
let varDirEmployeeID = 0
let varLoginMS = "..."
let varDirEmployeeLogin = "..."
let lanDirEmployeeName = "..."
let varReadinessDay = 5;          //Сервисный центр -> Приёмка: Число дней до даты готовности
let ServiceTypeRepair = 1;        //Сервисный центр -> Гарантия в Мастерской
let varPhoneNumberBegin = 7;      //Сервисный центр -> Приёмка: Телефон.начало
let varIsAdmin = true;            //Администратор торговой точки
let varRightDocServiceWorkshopsTab2ReturnCheck = true;            //СЦ: разрещить возврат на склад
let varRightDocServiceWorkshopsTab1AddCheck = true;            //СЦ: разрешить ручное добавление работ
//Sms
let SmsAutoShow = true;           //Сервисный центр -> Приёмка: Число дней до даты готовности
let SmsAutoShow9 = true;          //Сервисный центр -> Приёмка: Число дней до даты готовности
let SmsAutoShowServiceFromArchiv = false;           //Сервисный центр -> Приёмка: Число дней до даты готовности
let varSmsServiceMov = true;           //Сервисный центр -> Приёмка: Число дней до даты готовности
//Скидки в документах
let varRightDocDescriptionCheck = false;
//Идентичные вкладки: повторять или нет
let varTabIdenty = true;
//Скидки
let varDiscountPercentMarket = 20;
let varDiscountPercentService = 20;
let varDiscountPercentSecondHand = 20;
//ККМ
let varKKMSActive_FromSet = false; let varKKMSActive = false;
let varKKMSUrlServer = "http://localhost:5893/";
let varKKMSUser = "User";
let varKKMSPassword = 30;
let varKKMSNumDevice = -1;
let varKKMSCashierVATIN = "";
let varKKMSTaxVariant = 0;
let varKKMSTax = 0;
let varRightDocServicePurchesDiscountCheck = 0;
let varRightDocServicePurchesExtraditionCheck = false;
let varPayType = 0; let varPayTypeName = "Касса + Банк";
let varDiscountMarketType = 1;  //Скидка: от суммы или от цены (если продали более 1 аппарата)
//Авто-закрытие смены на ККМ
let varSmenaClose = false;
let varSmenaCloseTime = "";
let varSmenaClose2 = false;
let varRightReportSalariesEmplCheck = false;


/* GRID */
//Тип цен (Из Настроек)
let varDirPriceTypeID = 1;
let varStoreDirPriceTypesGrid; let varStoreDirPaymentTypesGrid; let varStoreDirServiceStatusesGrid;
let varStoreDirCharColoursGrid; let varStoreDirCharMaterialsGrid; let varStoreDirCharNamesGrid; let varStoreDirCharSeasonsGrid; let varStoreDirCharSexesGrid; let varStoreDirCharSizesGrid; let varStoreDirCharStylesGrid; let varStoreDirCharTexturesGrid; let varStoreDirCurrenciesGrid;
let varStoreDirServiceContractorsGrid;
let varStoreDirServiceDiagnosticRresultsGrid;
let varStoreDirServiceNomenTypicalFaultsGrid;
let varStoreDirReturnTypesGrid;
//Срок гарантии прошёл
let varWarrantyPeriodPassed = true;


// === Переменные для оплат клиента ===
let varDirPayServiceName = ""; //Тарифный план (Tariff1)
let varCountUser = ""; //Сотрудников (Tariff2)
//let varCountTT = ""; //Торговых точек (Tariff3)
let varCountNomen = ""; //Товаров (XXX)
let varPayDateEnd = ""; //Окончание (Tariff4)
//let varCountIM = ""; //Интернет магазинов (Tariff5)
let varPaymentExpired = false; //Оплата просрочена (вычисляется по переменной "varPayDateEnd")
let nDaysLeft = 999; //Через сколько дней заканчивается подписка (вычисляется по переменной "varPayDateEnd")

//Уже загружали настройки "Variables_SettingsRequest()"?
//Если "да", то не менять склад!
let Variables_SettingsRequest_Load = false;

let varRightDocServicePurchesDateDoneCheck = true;

try {

    //Settings === === === === === === === === === === === === ===
    function Variables_SettingsRequest() {

        
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            //url: HTTP_SettingsSelect + "?pID=Settings&FromVariables=1",
            url: HTTP_SysSettingsSet + "2",
            method: 'GET',

            success: function (result) {
                
                var sData = Ext.decode(result.responseText);
                if (sData.success == false) {
                    if (varCountErrorSettingsRequest < varCountErrorRequest + 5) {
                        varCountErrorSettingsRequest++;
                        Variables_SettingsRequest();
                        return;
                    }
                    else {
                        Ext.MessageBox.show({
                            title: lanOrgName,
                            msg: txtMsg017 + "<BR>" + sData.data,
                            icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
                            fn: function (buttons) {
                                if (buttons == "yes") { location.reload(); }
                            }
                        });
                        return;
                    }
                }
                else {
                    //Для "Employeess" and "Payment"
                    sData1 = sData.data; //Employeess + Currency
                    //Для "Settings"
                    sDataSysSettings = sData.data.sysSettings;
                    sDataDirEmployees = sData.data.dirEmployees;
                    //Для "SysDirConstants" - сложный запрос, поэтому отрабатывает отдельным запросом на Сервер
                    //sDataSysDirConstants = sData.data.sysDirConstants;


                    if (varType == "")
                    {
                        //Grids *** *** ***
                        //Тип цен и Характеристики
                        //Лоадер
                        //var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: Ext.getCmp("viewContainerCentral") }); loadingMask.show();
                        varLoadingMaskUnion = new Ext.LoadMask({ msg: 'Please wait...', target: Ext.getCmp("viewContainerCentral") }); 
                        varLoadingMaskUnion.show();

                        varStoreDirPriceTypesGrid = Ext.create("store.storeDirPriceTypesGrid"); varStoreDirPriceTypesGrid.setData([], false); varStoreDirPriceTypesGrid.proxy.url = HTTP_DirPriceTypes + "?type=Grid"; varStoreDirPriceTypesGrid.load({ waitMsg: lanLoading })
                        varStoreDirPriceTypesGrid.on('load', function () {
                            varStoreDirPaymentTypesGrid = Ext.create("store.storeDirPaymentTypesGrid"); varStoreDirPaymentTypesGrid.setData([], false); varStoreDirPaymentTypesGrid.proxy.url = HTTP_DirPaymentTypes + "?type=Grid"; varStoreDirPaymentTypesGrid.load({ waitMsg: lanLoading })
                            varStoreDirPaymentTypesGrid.on('load', function () {
                                varStoreDirServiceStatusesGrid = Ext.create("store.storeDirServiceStatusesGrid"); varStoreDirServiceStatusesGrid.setData([], false); varStoreDirServiceStatusesGrid.proxy.url = HTTP_DirServiceStatuses + "?type=Grid"; varStoreDirServiceStatusesGrid.load({ waitMsg: lanLoading })
                                varStoreDirServiceStatusesGrid.on('load', function () {
                                    varStoreDirCharColoursGrid = Ext.create("store.storeDirCharColoursGrid"); varStoreDirCharColoursGrid.setData([], false); varStoreDirCharColoursGrid.proxy.url = HTTP_DirCharColours + "?type=Grid"; varStoreDirCharColoursGrid.load({ waitMsg: lanLoading });
                                    varStoreDirCharColoursGrid.on('load', function () {
                                        varStoreDirCharMaterialsGrid = Ext.create("store.storeDirCharMaterialsGrid"); varStoreDirCharMaterialsGrid.setData([], false); varStoreDirCharMaterialsGrid.proxy.url = HTTP_DirCharMaterials + "?type=Grid"; varStoreDirCharMaterialsGrid.load({ waitMsg: lanLoading });
                                        varStoreDirCharMaterialsGrid.on('load', function () {
                                            varStoreDirCharNamesGrid = Ext.create("store.storeDirCharNamesGrid"); varStoreDirCharNamesGrid.setData([], false); varStoreDirCharNamesGrid.proxy.url = HTTP_DirCharNames + "?type=Grid"; varStoreDirCharNamesGrid.load({ waitMsg: lanLoading });
                                            varStoreDirCharNamesGrid.on('load', function () {
                                                varStoreDirCharSeasonsGrid = Ext.create("store.storeDirCharSeasonsGrid"); varStoreDirCharSeasonsGrid.setData([], false); varStoreDirCharSeasonsGrid.proxy.url = HTTP_DirCharSeasons + "?type=Grid"; varStoreDirCharSeasonsGrid.load({ waitMsg: lanLoading });
                                                varStoreDirCharSeasonsGrid.on('load', function () {
                                                    varStoreDirCharSexesGrid = Ext.create("store.storeDirCharSexesGrid"); varStoreDirCharSexesGrid.setData([], false); varStoreDirCharSexesGrid.proxy.url = HTTP_DirCharSexes + "?type=Grid"; varStoreDirCharSexesGrid.load({ waitMsg: lanLoading });
                                                    varStoreDirCharSexesGrid.on('load', function () {
                                                        varStoreDirCharSizesGrid = Ext.create("store.storeDirCharSizesGrid"); varStoreDirCharSizesGrid.setData([], false); varStoreDirCharSizesGrid.proxy.url = HTTP_DirCharSizes + "?type=Grid"; varStoreDirCharSizesGrid.load({ waitMsg: lanLoading });
                                                        varStoreDirCharSizesGrid.on('load', function () {
                                                            varStoreDirCharStylesGrid = Ext.create("store.storeDirCharStylesGrid"); varStoreDirCharStylesGrid.setData([], false); varStoreDirCharStylesGrid.proxy.url = HTTP_DirCharStyles + "?type=Grid"; varStoreDirCharStylesGrid.load({ waitMsg: lanLoading });
                                                            varStoreDirCharStylesGrid.on('load', function () {
                                                                varStoreDirCharTexturesGrid = Ext.create("store.storeDirCharTexturesGrid"); varStoreDirCharTexturesGrid.setData([], false); varStoreDirCharTexturesGrid.proxy.url = HTTP_DirCharTextures + "?type=Grid"; varStoreDirCharTexturesGrid.load({ waitMsg: lanLoading });
                                                                varStoreDirCharTexturesGrid.on('load', function () {
                                                                    varStoreDirServiceContractorsGrid = Ext.create("store.storeDirServiceContractorsGrid"); varStoreDirServiceContractorsGrid.setData([], false); varStoreDirServiceContractorsGrid.proxy.url = HTTP_DirServiceContractors + "/1/1/?type=Grid"; varStoreDirServiceContractorsGrid.load({ waitMsg: lanLoading });
                                                                    varStoreDirServiceContractorsGrid.on('load', function () {
                                                                        varStoreDirServiceDiagnosticRresultsGrid = Ext.create("store.storeDirServiceDiagnosticRresultsGrid"); varStoreDirServiceDiagnosticRresultsGrid.setData([], false); varStoreDirServiceDiagnosticRresultsGrid.proxy.url = HTTP_DirServiceDiagnosticRresults + "?type=Grid"; varStoreDirServiceDiagnosticRresultsGrid.load({ waitMsg: lanLoading });
                                                                        varStoreDirServiceDiagnosticRresultsGrid.on('load', function () {
                                                                            varStoreDirServiceNomenTypicalFaultsGrid = Ext.create("store.storeDirServiceNomenTypicalFaultsGrid"); varStoreDirServiceNomenTypicalFaultsGrid.setData([], false); varStoreDirServiceNomenTypicalFaultsGrid.proxy.url = HTTP_DirServiceNomenTypicalFaults + "?type=Grid"; varStoreDirServiceNomenTypicalFaultsGrid.load({ waitMsg: lanLoading });
                                                                            varStoreDirServiceNomenTypicalFaultsGrid.on('load', function () {
                                                                                varStoreDirReturnTypesGrid = Ext.create("store.storeDirReturnTypesGrid"); varStoreDirReturnTypesGrid.setData([], false); varStoreDirReturnTypesGrid.proxy.url = HTTP_DirReturnTypes + "?type=Grid"; varStoreDirReturnTypesGrid.load({ waitMsg: lanLoading });
                                                                                varStoreDirReturnTypesGrid.on('load', function () {
                                                                                    varLoadingMaskUnion.hide();
                                                                                });
                                                                            });
                                                                        });
                                                                    });
                                                                });
                                                            });
                                                        });
                                                    });
                                                });
                                            });
                                        });
                                    });
                                });
                            });
                        });
                    }


                    // === === === sDataSysSettings === === === 
                    if (sDataSysSettings != undefined) {
                        varJurDateS = Ext.Date.format(new Date(sDataSysSettings.JurDateS), 'Y-m-d');
                        varJurDatePo = Ext.Date.format(new Date(sDataSysSettings.JurDatePo), 'Y-m-d');

                        varFractionalPartInSum = parseInt(sDataSysSettings.FractionalPartInSum);
                        varFractionalPartInPrice = parseInt(sDataSysSettings.FractionalPartInPrice);
                        varFractionalPartInOther = parseInt(sDataSysSettings.FractionalPartInOther);
                        varDirVatValue = parseFloat(sDataSysSettings.DirVatValue);
                        varChangePriceNomen = sDataSysSettings.ChangePriceNomen;
                        varMethodAccounting = sDataSysSettings.MethodAccounting;
                        varDeletedRecordsShow = sDataSysSettings.DeletedRecordsShow;

                        varDirContractorIDOrg = sDataSysSettings.dirContractorOrg.DirContractorID;
                        varDirContractorNameOrg = sDataSysSettings.dirContractorOrg.DirContractorName;

                        varDirCurrencyID = parseInt(sDataSysSettings.DirCurrencyID);
                        varDirCurrencyName = sDataSysSettings.dirCurrency.DirCurrencyName;
                        varDirCurrencyNameShort = sDataSysSettings.dirCurrency.DirCurrencyNameShort;
                        varDirCurrencyRate = parseFloat(sData1.DirCurrencyRate);
                        varDirCurrencyMultiplicity = parseFloat(sData1.DirCurrencyMultiplicity);

                        if (!Variables_SettingsRequest_Load) {
                            varDirWarehouseID = parseInt(sDataSysSettings.DirWarehouseID);
                        }

                        varMarkupRetail = parseFloat(sDataSysSettings.MarkupRetail);
                        varMarkupWholesale = parseFloat(sDataSysSettings.MarkupWholesale);
                        varMarkupIM = parseFloat(sDataSysSettings.MarkupIM);
                        varMarkupSales1 = parseFloat(sDataSysSettings.MarkupSales1);
                        varMarkupSales2 = parseFloat(sDataSysSettings.MarkupSales2);
                        varMarkupSales3 = parseFloat(sDataSysSettings.MarkupSales3);
                        varMarkupSales4 = parseFloat(sDataSysSettings.MarkupSales4);

                        varCashBookAdd = sDataSysSettings.CashBookAdd;
                        varReserve = sDataSysSettings.CashBookAdd;
                        varBarIntNomen = parseInt(sDataSysSettings.BarIntNomen);
                        varBarIntContractor = parseInt(sDataSysSettings.BarIntContractor);
                        varBarIntDoc = parseInt(sDataSysSettings.BarIntDoc);
                        varSelectOneClick = sDataSysSettings.SelectOneClick;
                        varPageSizeDir = parseInt(sDataSysSettings.PageSizeDir);
                        varPageSizeJurn = parseInt(sDataSysSettings.PageSizeJurn);
                        varDirPriceTypeID = parseInt(sDataSysSettings.DirPriceTypeID);
                        varDirNomenMinimumBalance = parseInt(sDataSysSettings.DirNomenMinimumBalance);
                        varReadinessDay = parseInt(sDataSysSettings.ReadinessDay);
                        ServiceTypeRepair = parseInt(sDataSysSettings.ServiceTypeRepair);
                        varPhoneNumberBegin = sDataSysSettings.PhoneNumberBegin;
                        SmsAutoShow = sDataSysSettings.SmsAutoShow;
                        SmsAutoShowServiceFromArchiv = sDataSysSettings.SmsAutoShowServiceFromArchiv;
                        varSmsServiceMov = sDataSysSettings.SmsServiceMov;
                        //Срок гарантии прошёл
                        varWarrantyPeriodPassed = sDataSysSettings.WarrantyPeriodPassed;
                        //Переключаемся на уже открытую вкладку
                        varTabIdenty = sDataSysSettings.TabIdenty;
                        //Скидки
                        varDiscountPercentMarket = sDataSysSettings.DiscountPercentMarket;
                        varDiscountPercentService = sDataSysSettings.DiscountPercentService;
                        varDiscountPercentSecondHand = sDataSysSettings.DiscountPercentSecondHand;

                        //ККМ
                        varKKMSActive_FromSet = sDataSysSettings.KKMSActive; varKKMSActive = sDataSysSettings.KKMSActive;
                        varKKMSUrlServer = sDataSysSettings.KKMSUrlServer;
                        varKKMSUser = sDataSysSettings.KKMSUser;
                        varKKMSPassword = sDataSysSettings.KKMSPassword;
                        if (varKKMSNumDevice == -1) varKKMSNumDevice = sDataSysSettings.KKMSNumDevice;
                        varKKMSCashierVATIN = sDataSysSettings.KKMSCashierVATIN;
                        varKKMSTaxVariant = sDataSysSettings.KKMSTaxVariant;
                        varKKMSTax = sDataSysSettings.KKMSTax;
                        //Если открыта форма "Старт"
                       
                        if (!varKKMSActive) {
                            //Блокируем Меню "ККМ"
                            Ext.getCmp("RightKKM0").disable();

                            //на форме Старт блокируме (если открыта)
                            if (Ext.getCmp("KKMSNumDeviceStart")) {
                                Ext.getCmp("KKMSNumDeviceStart").disable();
                            }
                        }
                        else {
                            //На форме Старт присваеваем Номер ККМ по умолчанию
                            if (Ext.getCmp("KKMSNumDeviceStart")) {
                                Ext.getCmp("KKMSNumDeviceStart").setValue(varKKMSNumDevice);
                            }
                        }

                        varPayType = sDataSysSettings.PayType;
                        if (varPayType == 1) {
                            varPayTypeName = "Касса";
                        }
                        else if (varPayType == 2) {
                            varPayTypeName = "Банк";
                        }

                        varDiscountMarketType = sDataSysSettings.varDiscountMarketType;



                        //Не работает в новой версии!
                        /*
                        DateFormat = parseInt(sDataSysSettings.DateFormat);
                        if (DateFormat == 1) DateFormatStr = "Y-m-d";
                        else if (DateFormat == 2) DateFormatStr = "d-m-Y";
                        */
                    }



                    // === === === sDataDirEmployees === === === 
                    if (sDataDirEmployees != undefined) {
                        //ID-шник вошедшего Сотрудника
                        varDirEmployeeID = sDataDirEmployees.DirEmployeeID;

                        if (!Variables_SettingsRequest_Load) {
                            if (sDataDirEmployees.DirWarehouseID != null) { varDirWarehouseIDEmpl = parseInt(sDataDirEmployees.DirWarehouseID); varDirWarehouseNameEmpl = sData1.DirWarehouseName; }
                            else varDirWarehouseIDEmpl = 0;
                        }

                        if (sDataDirEmployees.DirContractorIDOrg != null) varDirContractorIDOrgEmpl = parseInt(sDataDirEmployees.DirContractorIDOrg);
                        else varDirContractorIDOrgEmpl = 0;

                        /*if (varType == "Retail") {
                            //Ext.getCmp("viewContainerCentralX").setTitle(Ext.getCmp("viewContainerCentralX").getTitle() + " (" + varDirWarehouseNameEmpl + ")");
                            Ext.getCmp("varDirWarehouseNameEmpl").setText("Точка: " + varDirWarehouseNameEmpl);
                        }*/

                        //Для кнопки в верхнем меню - используется в app.js
                        if (Ext.getCmp("HeaderToolBarEmployees") != undefined) { //Если входим в модуль Розница
                            lanDirEmployeeName = sData.data.DirEmployeesName;
                        }


                        //Права *** *** ***

                        if (Ext.getCmp("RightSysSettings0") == undefined) return;

                        //Настройки
                        Ext.getCmp("RightSysSettings0").setVisible(sDataDirEmployees.RightSysSettings0); //if (sDataDirEmployees.RightSysSettings0 == true) { Ext.getCmp("RightSysSettings0").setVisible(true); }
                        Ext.getCmp("RightMyCompany").setVisible(sDataDirEmployees.RightMyCompanyCheck); //if (sDataDirEmployees.RightMyCompanyCheck == true) { Ext.getCmp("RightMyCompany").setVisible(true); }
                        Ext.getCmp("RightDirEmployees").setVisible(sDataDirEmployees.RightDirEmployeesCheck); //if (sDataDirEmployees.RightDirEmployeesCheck == true) { Ext.getCmp("RightDirEmployees").setVisible(true); }
                        Ext.getCmp("RightSysSettings").setVisible(sDataDirEmployees.RightSysSettingsCheck); //if (sDataDirEmployees.RightSysSettingsCheck == true) { Ext.getCmp("RightSysSettings").setVisible(true); }
                        Ext.getCmp("RightSysJourDisps").setVisible(sDataDirEmployees.RightSysJourDispsCheck); //if (sDataDirEmployees.RightSysJourDispsCheck == true) { Ext.getCmp("RightSysJourDisps").setVisible(true); }
                        Ext.getCmp("RightDataExchange").setVisible(sDataDirEmployees.RightDataExchangeCheck); //if (sDataDirEmployees.RightDataExchangeCheck == true) { Ext.getCmp("RightDataExchange").setVisible(true); }
                        Ext.getCmp("RightYourData").setVisible(sDataDirEmployees.RightYourDataCheck); //if (sDataDirEmployees.RightYourDataCheck == true) { Ext.getCmp("RightYourData").setVisible(true); }
                        Ext.getCmp("RightDiscPay").setVisible(sDataDirEmployees.RightDiscPayCheck); //if (sDataDirEmployees.RightDiscPayCheck == true) { Ext.getCmp("RightDiscPay").setVisible(true); }

                        //Справочники
                        Ext.getCmp("RightDir0").setVisible(sDataDirEmployees.RightDir0); //if (sDataDirEmployees.RightDir0 == true) { Ext.getCmp("RightDir0").setVisible(true); }
                        Ext.getCmp("RightDirNomens").setVisible(sDataDirEmployees.RightDirNomensCheck); //if (sDataDirEmployees.RightDirNomensCheck == true) { Ext.getCmp("RightDirNomens").setVisible(true); }
                        Ext.getCmp("RightDirNomenCategories").setVisible(sDataDirEmployees.RightDirNomenCategoriesCheck); //if (sDataDirEmployees.RightDirNomenCategoriesCheck == true) { Ext.getCmp("RightDirNomenCategories").setVisible(true); }
                        Ext.getCmp("RightDirContractors").setVisible(sDataDirEmployees.RightDirContractorsCheck); //if (sDataDirEmployees.RightDirContractorsCheck == true) { Ext.getCmp("RightDirContractors").setVisible(true); }
                        Ext.getCmp("RightDirWarehouses").setVisible(sDataDirEmployees.RightDirWarehousesCheck); //if (sDataDirEmployees.RightDirWarehousesCheck == true) { Ext.getCmp("RightDirWarehouses").setVisible(true); }
                        Ext.getCmp("RightDirBanks").setVisible(sDataDirEmployees.RightDirBanksCheck); //if (sDataDirEmployees.RightDirBanksCheck == true) { Ext.getCmp("RightDirBanks").setVisible(true); }
                        Ext.getCmp("RightDirCashOffices").setVisible(sDataDirEmployees.RightDirCashOfficesCheck); //if (sDataDirEmployees.RightDirCashOfficesCheck == true) { Ext.getCmp("RightDirCashOffices").setVisible(true); }
                        Ext.getCmp("RightDirCurrencies").setVisible(sDataDirEmployees.RightDirCurrenciesCheck); //if (sDataDirEmployees.RightDirCurrenciesCheck == true) { Ext.getCmp("RightDirCurrencies").setVisible(true); }
                        Ext.getCmp("RightDirVats").setVisible(sDataDirEmployees.RightDirVatsCheck); //if (sDataDirEmployees.RightDirVatsCheck == true) { Ext.getCmp("RightDirVats").setVisible(true); }
                        Ext.getCmp("RightDirDiscounts").setVisible(sDataDirEmployees.RightDirDiscountsCheck); //if (sDataDirEmployees.RightDirDiscountsCheck == true) { Ext.getCmp("RightDirDiscounts").setVisible(true); }
                        Ext.getCmp("RightDirBonuses").setVisible(sDataDirEmployees.RightDirBonusesCheck); //if (sDataDirEmployees.RightDirBonusesCheck == true) { Ext.getCmp("RightDirBonuses").setVisible(true); } //Ext.getCmp("RightDirBonus2es").setVisible(true);
                        Ext.getCmp("RightDirCharColours").setVisible(sDataDirEmployees.RightDirCharColoursCheck); //if (sDataDirEmployees.RightDirCharColoursCheck == true) { Ext.getCmp("RightDirCharColours").setVisible(true); }
                        Ext.getCmp("RightDirCharMaterials").setVisible(sDataDirEmployees.RightDirCharMaterialsCheck); //if (sDataDirEmployees.RightDirCharMaterialsCheck == true) { Ext.getCmp("RightDirCharMaterials").setVisible(true); }
                        Ext.getCmp("RightDirCharNames").setVisible(sDataDirEmployees.RightDirCharNamesCheck); //if (sDataDirEmployees.RightDirCharNamesCheck == true) { Ext.getCmp("RightDirCharNames").setVisible(true); }
                        Ext.getCmp("RightDirCharSeasons").setVisible(sDataDirEmployees.RightDirCharSeasonsCheck); //if (sDataDirEmployees.RightDirCharSeasonsCheck == true) { Ext.getCmp("RightDirCharSeasons").setVisible(true); }
                        Ext.getCmp("RightDirCharSexes").setVisible(sDataDirEmployees.RightDirCharSexesCheck); //if (sDataDirEmployees.RightDirCharSexesCheck == true) { Ext.getCmp("RightDirCharSexes").setVisible(true); }
                        Ext.getCmp("RightDirCharSizes").setVisible(sDataDirEmployees.RightDirCharSizesCheck); //if (sDataDirEmployees.RightDirCharSizesCheck == true) { Ext.getCmp("RightDirCharSizes").setVisible(true); }
                        Ext.getCmp("RightDirCharStyles").setVisible(sDataDirEmployees.RightDirCharStylesCheck); //if (sDataDirEmployees.RightDirCharStylesCheck == true) { Ext.getCmp("RightDirCharStyles").setVisible(true); }
                        Ext.getCmp("RightDirCharTextures").setVisible(sDataDirEmployees.RightDirCharTexturesCheck); //if (sDataDirEmployees.RightDirCharTexturesCheck == true) { Ext.getCmp("RightDirCharTextures").setVisible(true); }

                        //Торговля
                        Ext.getCmp("RightDoc0").setVisible(sDataDirEmployees.RightDoc0); //if (sDataDirEmployees.RightDoc0 == true) { Ext.getCmp("RightDoc0").setVisible(true); }
                        Ext.getCmp("RightDocPurches").setVisible(sDataDirEmployees.RightDocPurchesCheck); //if (sDataDirEmployees.RightDocPurchesCheck == true) { Ext.getCmp("RightDocPurches").setVisible(true); }
                        Ext.getCmp("RightDocReturnVendors").setVisible(sDataDirEmployees.RightDocReturnVendorsCheck); //if (sDataDirEmployees.RightDocReturnVendorsCheck == true) { Ext.getCmp("RightDocReturnVendors").setVisible(true); }
                        if (sDataDirEmployees.RightDocMovementsCheck == true) {
                            Ext.getCmp("RightDocMovements").setVisible(true);
                            Ext.getCmp("RightDocMovementsLogisticsNew").setVisible(true);
                        }
                        if (sDataDirEmployees.RightDocSalesCheck == true) { Ext.getCmp("RightDocSales").setVisible(true); }
                        if (sDataDirEmployees.RightDocReturnsCustomersCheck == true) { Ext.getCmp("RightDocReturnsCustomers").setVisible(true); }
                        if (sDataDirEmployees.RightDocActOnWorksCheck == true) { Ext.getCmp("RightDocActOnWorks").setVisible(true); }
                        if (sDataDirEmployees.RightDocAccountsCheck == true) { Ext.getCmp("RightDocAccounts").setVisible(true); }
                        if (sDataDirEmployees.RightDocActWriteOffsCheck == true) { Ext.getCmp("RightDocActWriteOffs").setVisible(true); }
                        if (sDataDirEmployees.RightDocInventoriesCheck == true) { Ext.getCmp("RightDocInventories").setVisible(true); }
                        if (sDataDirEmployees.RightDocNomenRevaluationsCheck == true) { Ext.getCmp("RightDocNomenRevaluations").setVisible(true); }
                        //Скидки в документах
                        if (sDataDirEmployees.RightDocDescriptionCheck == true) { varRightDocDescriptionCheck = sDataDirEmployees.RightDocDescriptionCheck; }

                        //Торговля.Витрина
                        if (sDataDirEmployees.RightVitrina0 == true) { Ext.getCmp("RightVitrina0").setVisible(true); }
                        if (sDataDirEmployees.RightDocRetailsCheck == true) { Ext.getCmp("RightDocRetails").setVisible(true); }

                        //Сервис
                        if (sDataDirEmployees.RightDocService0 == true) { Ext.getCmp("RightDocService0").setVisible(true); }
                        if (sDataDirEmployees.RightDocServicePurchesCheck == true) { Ext.getCmp("RightDocServicePurches").setVisible(true); }
                        if (sDataDirEmployees.RightDocServiceWorkshopsCheck == true) { Ext.getCmp("RightDocServiceWorkshops").setVisible(true); }
                        varRightDocServiceWorkshopsTab2ReturnCheck = sDataDirEmployees.RightDocServiceWorkshopsTab2ReturnCheck;
                        varRightDocServiceWorkshopsTab1AddCheck = sDataDirEmployees.RightDocServiceWorkshopsTab1AddCheck;
                        //if (sDataDirEmployees.RightDocServiceOutputsCheck == true) { Ext.getCmp("RightDocServiceOutputs").setVisible(true); }
                        //if (sDataDirEmployees.RightDocServiceArchivesCheck == true) { Ext.getCmp("RightDocServiceArchives").setVisible(true); }
                        varRightDocServicePurchesExtraditionCheck = sDataDirEmployees.RightDocServicePurchesExtraditionCheck;
                        if (sDataDirEmployees.RightDocServicePurchesReportCheck == true) { Ext.getCmp("RightDocServicePurchesReport").setVisible(true); }
                        varRightDocServicePurchesDateDoneCheck = sDataDirEmployees.RightDocServicePurchesDateDoneCheck;
                        if (sDataDirEmployees.RightDirServiceNomensCheck == true) { Ext.getCmp("RightDirServiceNomens").setVisible(true); }
                        //if (sDataDirEmployees.RightDirServiceNomenCategoriesCheck == true) { Ext.getCmp("RightDirServiceNomenCategories").setVisible(true); }
                        if (sDataDirEmployees.RightDirServiceContractorsCheck == true) { Ext.getCmp("RightDirServiceContractors").setVisible(true); }
                        if (sDataDirEmployees.RightDirServiceJobNomensCheck == true) { Ext.getCmp("RightDirServiceJobNomens").setVisible(true); Ext.getCmp("RightDirServiceJobNomens1").setVisible(true); }
                        if (sDataDirEmployees.RightDirSmsTemplatesCheck == true) {
                            Ext.getCmp("RightDirSmsTemplates").setVisible(true);
                            Ext.getCmp("RightDocMovementsLogisticsSmsTemplates").setVisible(true);
                        }
                        if (sDataDirEmployees.RightDirServiceDiagnosticRresultsCheck == true) { Ext.getCmp("RightDirServiceDiagnosticRresults").setVisible(true); }
                        if (sDataDirEmployees.RightDirServiceNomenTypicalFaultsCheck == true) { Ext.getCmp("RightDirServiceNomenTypicalFaults").setVisible(true); }
                        if (sDataDirEmployees.RightDocServiceMovementsCheck == true) { Ext.getCmp("RightDocServiceMovements").setVisible(true); }
                        varRightDocServicePurchesDiscountCheck = sDataDirEmployees.RightDocServicePurchesDiscountCheck;

                        //Аналитика
                        if (sDataDirEmployees.RightAnalitics0 == true) { Ext.getCmp("RightAnalitics0").setVisible(true); }

                        //Б/У
                        if (sDataDirEmployees.RightDocSecondHands0 == true) { Ext.getCmp("RightDocSecondHands0").setVisible(true); }
                        if (sDataDirEmployees.RightDocSecondHandPurchesCheck == true) { Ext.getCmp("RightDocSecondHandPurches").setVisible(true); }
                        if (sDataDirEmployees.RightDocSecondHandWorkshopsCheck == true) { Ext.getCmp("RightDocSecondHandWorkshops").setVisible(true); }
                        if (sDataDirEmployees.RightDocSecondHandRetailsCheck == true) { Ext.getCmp("RightDocSecondHandRetails").setVisible(true); }
                        if (sDataDirEmployees.RightDocSecondHandMovementsCheck == true) {
                            Ext.getCmp("RightDocSecondHandMovements").setVisible(true);
                            Ext.getCmp("RightDocSecondHandMovementsLogisticsNew").setVisible(true);
                        }
                        if (sDataDirEmployees.RightDocSecondHandInventoriesCheck == true) {
                            Ext.getCmp("RightDocSecondHandInventories").setVisible(true);
                        }
                        if (sDataDirEmployees.RightDocSecondHandRazborsCheck == true) {
                            Ext.getCmp("RightDocSecondHandRazbors").setVisible(true);
                        }

                        //Заказы
                        if (sDataDirEmployees.RightDocOrderInt0 == true) { Ext.getCmp("RightDocOrderInt0").setVisible(true); }
                        if (sDataDirEmployees.RightDocOrderIntsNewCheck == true) { Ext.getCmp("RightDocOrderIntsNew").setVisible(true); }
                        if (sDataDirEmployees.RightDocOrderIntsCheck == true) { Ext.getCmp("RightDocOrderInts").setVisible(true); }
                        if (sDataDirEmployees.RightDocOrderIntsReportCheck == true) { Ext.getCmp("RightDocOrderIntsReport").setVisible(true); }
                        //if (sDataDirEmployees.RightDocOrderIntsArchiveCheck == true) { Ext.getCmp("RightDocOrderIntsArchive").setVisible(true); }

                        //Деньги: Касса + Банк
                        if (sDataDirEmployees.RightDocBankCash0 == true) { Ext.getCmp("RightDocBankCash0").setVisible(true); }
                        if (sDataDirEmployees.RightDocBankSumsCheck == true) { Ext.getCmp("RightDocBankSums").setVisible(true); }
                        if (sDataDirEmployees.RightDocCashOfficeSumMovementsCheck == true) { Ext.getCmp("RightDocCashOfficeSumMovements").setVisible(true); }
                        if (sDataDirEmployees.RightDocCashOfficeSumsCheck == true) { Ext.getCmp("RightDocCashOfficeSums").setVisible(true); }
                        if (sDataDirEmployees.RightReportBanksCashOfficesCheck == true) { Ext.getCmp("RightReportBanksCashOffices").setVisible(true); }
                        if (sDataDirEmployees.RightDirDomesticExpensesCheck == true) { Ext.getCmp("RightDirDomesticExpenses").setVisible(true); }

                        //Зарплата
                        if (sDataDirEmployees.RightSalaries0 == true) { Ext.getCmp("RightSalaries0").setVisible(true); }
                        if (sDataDirEmployees.RightDocDomesticExpenseSalaries == true) { Ext.getCmp("RightDocDomesticExpenseSalaries").setVisible(true); }
                        if (sDataDirEmployees.RightDocDomesticExpenses == true) { Ext.getCmp("RightDocDomesticExpenses").setVisible(true); }
                        if (sDataDirEmployees.RightReportSalariesCheck == true) { Ext.getCmp("RightReportSalaries").setVisible(true); }
                        if (sDataDirEmployees.RightReportSalariesWarehousesCheck == true) { Ext.getCmp("RightReportSalariesWarehouses").setVisible(true); }
                        varRightReportSalariesEmplCheck = sDataDirEmployees.RightReportSalariesEmplCheck;

                        //Логистика
                        if (sDataDirEmployees.RightLogistics0 == true) { Ext.getCmp("RightLogistics0").setVisible(true); }
                        if (sDataDirEmployees.RightDocMovementsLogisticsCheck == true) { Ext.getCmp("RightDocMovementsLogistics").setVisible(true); }

                        //Отчеты
                        //if (sDataDirEmployees.RightReport0 == true) { Ext.getCmp("RightReport0").setVisible(true); }
                        //if (sDataDirEmployees.RightReportPriceListCheck == true) { Ext.getCmp("RightReportPriceList").setVisible(true); }
                        //if (sDataDirEmployees.RightReportRemnantsCheck == true) { Ext.getCmp("RightReportRemnants").setVisible(true); }
                        //if (sDataDirEmployees.RightReportProfitCheck == true) { Ext.getCmp("RightReportProfit").setVisible(true); }

                        //ККМ
                        if (sDataDirEmployees.RightKKM0 == true) { Ext.getCmp("RightKKM0").setVisible(true); }
                        if (sDataDirEmployees.RightKKMXReportCheck == true) { Ext.getCmp("RightKKMXReport").setVisible(true); }
                        if (sDataDirEmployees.RightKKMOpenCheck == true) { Ext.getCmp("RightKKMOpen").setVisible(true); }
                        if (sDataDirEmployees.RightKKMEncashmentCheck == true) { Ext.getCmp("RightKKMEncashment").setVisible(true); }
                        if (sDataDirEmployees.RightKKMAddingCheck == true) { Ext.getCmp("RightKKMAdding").setVisible(true); }
                        if (sDataDirEmployees.RightKKMCloseCheck == true) { Ext.getCmp("RightKKMClose").setVisible(true); }
                        if (sDataDirEmployees.RightKKMPrintOFDCheck == true) { Ext.getCmp("RightKKMPrintOFD").setVisible(true); }
                        if (sDataDirEmployees.RightKKMCheckLastFNCheck == true) { Ext.getCmp("RightKKMCheckLastFN").setVisible(true); }
                        if (sDataDirEmployees.RightKKMStateCheck == true) { Ext.getCmp("RightKKMState").setVisible(true); }
                        if (sDataDirEmployees.RightKKMListCheck == true) { Ext.getCmp("RightKKMList").setVisible(true); }


                        if (sDataDirEmployees.RightDevelopCheck == true) { Ext.getCmp("RightDevelop").setVisible(true); }

                    }



                    // === === === Остальное === === === 
                    if (sData != undefined) {
                        //Для кнопки в верхнем меню
                        /*
                        var _btnEmployees = Ext.getCmp("viewContainerHeader").getComponent('btnEmployees');
                        if (_btnEmployees != undefined) { //Если входим в модуль Розница
                            _btnEmployees.setText("<font size=" + HeaderMenu_FontSize_1 + ">" + sData1.DirEmployeeLogin + "</font>")
                            _btnEmployees.setTooltip("<font size=" + HeaderMenu_FontSize_1 + ">" + lanEmployee + ": " + sData1.DirEmployeeName + "</font>")
                            lanDirEmployeeName = sData1.DirEmployeeName;
                        }*/

                        //Под каким именем вошли
                        varLoginMS = sData1.LoginMS;
                        varDirEmployeeLogin = sData1.dirEmployees.DirEmployeeLogin; //sData1.DirEmployeeLogin;
                        lanDirEmployeeName = sData1.DirEmployeeName;

                        //Оплаты
                        varDirPayServiceName = sData1.DirPayServiceName;
                        varCountUser = sData1.CountUser;
                        //varCountTT = sData1.CountTT;
                        varCountNomen = sData1.CountNomen;
                        varPayDateEnd = Ext.Date.format(new Date(sData1.PayDateEnd), 'Y-m-d');
                        //varCountIM = sData1.CountIM;


                        //Оплата: если осталось менее 4-х дней выводим "viewMain"
                        /*
                        var dToday = new Date(); // текущая дата
                        var dPayDateEnd = new Date(varPayDateEnd); // дата конца оплаты
                        nDaysLeft = dPayDateEnd > dToday ? Math.ceil((dPayDateEnd - dToday) / (1000 * 60 * 60 * 24)) : null; // а тут мы вычисляем, сколько же осталось дней — находим разницу в миллисекундах и переводим её в дни
                        if (nDaysLeft <= 5 || varPayDateEnd == "2150-01-01") {
                            varPaymentExpired = true; //Оплата просрочена

                            var Params = ["viewPanelMain"];
                            ObjectConfig("viewMain", Params);
                        }
                        */

                        /*
                        if (varType == "") {
                            var Params = ["viewPanelMain"];
                            ObjectConfig("viewMain", Params);
                        }
                        */
                    }


                    //Всё! Загрузили настройки!
                    Variables_SettingsRequest_Load = true;


                    //Запуск таймера
                    Variables_RunnerRequest();
                }

            },
            failure: function (result) {
                if (varCountErrorSettingsRequest < varCountErrorRequest + 5) {
                    varCountErrorSettingsRequest++;
                    Variables_SettingsRequest();
                }
                else {
                    Ext.MessageBox.show({
                        title: lanOrgName,
                        msg: txtMsg017,
                        icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
                        fn: function (buttons) {
                            if (buttons == "yes") { location.reload(); }
                        }
                    });
                }
            }

        });

    }


    function Variables_RunnerRequest() {
        //Таймер === === === === === === === === === === === === ===
        var runner = new Ext.util.TaskRunner(), clock, updateClock, task, MaxJurnChatID = 0, MaxJurnChatPublicID = 0;
        clock = Ext.getBody().appendChild({ id: 'clock' });
        // Start a simple clock task that updates a div once per second
        updateClock = function () {
            LoadTab();
        };
        //Запуск таймера
        //1s - 1000, 240s - 240000
        task = runner.start({
            run: updateClock,
            interval: 125000 //10000
        });


        function LoadTab() {


            Ext.Ajax.request({
                timeout: varTimeOutDefault,
                waitMsg: lanUpload,
                url: HTTP_Timer + "2",
                method: 'GET',
                success: function (result) {
                    var sData = Ext.decode(result.responseText);
                    if (sData.sucess == false) {
                        //console.log(sData.data);
                    }
                    else {

                        //0. Переменная
                        var mNot = Ext.getCmp('HeaderToolBarNotification');
                        //Удаляем все записи из меню
                        while (mNot.menu.items.items.length > 0) {
                            //console.log("---Start Remove---");
                            mNot.menu.remove(mNot.menu.items.items[mNot.menu.items.items.length - 1]);
                            //console.log("---End Remove---");
                        }

                        //К-во
                        var localCount = 0;


                        //1. Инвентаризация
                        if (sData.DocSecondHandInv.length > 0) {
                            //Новые записи в меню
                            for (var i = 0; i < sData.DocSecondHandInv.length; i++) {
                                mNot.menu.add(
                                    {
                                        text:
                                            "<b>Новое сообщение</b><br />" +
                                            "БУ.Инвентаризация №<b>" + sData.DocSecondHandInv[i].DocSecondHandInvID + "</b> на точке <b>" + sData.DocSecondHandInv[i].DirWarehouseName + "</b> подписана товароведом!<br />" +
                                            "Теперь Вы можете её подписать!",

                                        icon: '../Scripts/sklad/images/doc_held.png',
                                        handler: function (par1, par2, par3) {
                                            alert("Подпишите документ БУ.Инвентаризация, после чего он будет проведён товароведом!");
                                        }
                                    },
                                );
                            }
                            localCount += mNot.menu.items.items.length
                        }


                        //n. К-во записей
                        var localColor = ["red", "blue", "green"];
                        var loclaRand = Math.floor(Math.random() * (2 - 0)) + 0;
                        if (localCount > 0) {
                            mNot.setText(
                                "<b style='color: " + localColor[loclaRand] + "'>" +
                                (localCount).toString() +
                                "</b>"
                            );
                        }
                        else {
                            mNot.setText("0");
                        }


                    }
                },
                failure: function (result) {
                    console.log(result);
                }
            });
            



            //=== === === === === === === === === === === === === === === === === === === === === === === === === === ===
            //Если в Точке выбрана опция ато-закрытия смены на ККМ и смена ещё не закрыта
            //varKKMSActive  - ККм Активна
            //varSmenaClose  - В точке стоит опция авто-закрытик смены
            //varSmenaClose2 - Смена ещё не закрыта
            if (varKKMSActive && varSmenaClose && !varSmenaClose2) {

                /*
                var d1 = new Date();
                d1.setMinutes(d1.getMinutes() + 1);
                alert(d1);
                */
                
                //Дата + Время
                var date = new Date();
                //Hours
                var Hours = date.getHours(); if (Hours.toString().length == 1) Hours = "0" + Hours;
                //Minutes
                var Minutes1 = date.getMinutes(); if (Minutes1.toString().length == 1) Minutes1 = "0" + Minutes1;

                date.setMinutes(date.getMinutes() + 1);
                var Minutes2 = date.getMinutes(); if (Minutes2.toString().length == 1) Minutes2 = "0" + Minutes2;

                date.setMinutes(date.getMinutes() + 1);
                var Minutes3 = date.getMinutes(); if (Minutes3.toString().length == 1) Minutes3 = "0" + Minutes3;

                date.setMinutes(date.getMinutes() + 1);
                var Minutes4 = date.getMinutes(); if (Minutes4.toString().length == 1) Minutes4 = "0" + Minutes4;

                date.setMinutes(date.getMinutes() + 1);
                var Minutes5 = date.getMinutes(); if (Minutes5.toString().length == 1) Minutes5 = "0" + Minutes5;

                date.setMinutes(date.getMinutes() + 1);
                var Minutes6 = date.getMinutes(); if (Minutes6.toString().length == 1) Minutes6 = "0" + Minutes6;
                //Seconds
                //var Seconds = date.getSeconds(); if (Seconds.toString().length == 1) Seconds = "0" + Seconds;

                //Время
                var time1 = Hours.toString() + ':' + Minutes1.toString(); // + ':' + Seconds;
                var time2 = Hours.toString() + ':' + Minutes2.toString(); // + ':' + Seconds;
                var time3 = Hours.toString() + ':' + Minutes3.toString(); // + ':' + Seconds;
                var time4 = Hours.toString() + ':' + Minutes4.toString(); // + ':' + Seconds;
                var time5 = Hours.toString() + ':' + Minutes5.toString(); // + ':' + Seconds;
                var time6 = Hours.toString() + ':' + Minutes6.toString(); // + ':' + Seconds;

                
                if (varSmenaCloseTime == time1 || varSmenaCloseTime == time2 || varSmenaCloseTime == time3 || varSmenaCloseTime == time4 || varSmenaCloseTime == time5 || varSmenaCloseTime == time6) {
                    //alert("Сейчас " + time1 + "! Закрыть смену на ККМ???");

                    Ext.MessageBox.show({
                        title: 'Закрытие смены на ККМ',
                        msg: "Сейчас " + time1 + "! Закрыть смену на ККМ???<br /> Yes - закрыть, No - спросить позже, Cancel - больше не спрашивать",
                        buttons: Ext.Msg.YESNOCANCEL,
                        icon: Ext.Msg.QUESTION,
                        fn: function (btn) {
                            if (btn == "yes") {
                                CloseShift(false);
                                varSmenaClose2 = true;
                            }
                            else if (btn == "no") {
                                //NO              
                            }
                            else {
                                varSmenaClose2 = true;
                            }
                        }
                    });

                }

            }
            //=== === === === === === === === === === === === === === === === === === === === === === === === === === ===


        }
    }


} catch (ex) {
    var exMsg = ex;
    if (exMsg.message != undefined) exMsg = ex.message;

    Ext.Msg.alert(lanOrgName, "Ошибка в запросе на сервер!<br />Подробности:" + exMsg);
}
