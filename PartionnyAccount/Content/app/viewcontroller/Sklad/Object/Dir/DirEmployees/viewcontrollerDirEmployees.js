Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Dir/DirEmployees/viewcontrollerDirEmployees', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDirEmployees',


    // *** Настройки *** *** ***
    onRightSysSettings0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightMyCompanyCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightMyCompanyCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightMyCompany1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirEmployeesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirEmployeesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirEmployees1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightSysSettingsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightSysSettingsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightSysSettings1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightSysJourDispsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightSysJourDispsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightSysJourDisps1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDataExchangeCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDataExchangeCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDataExchange1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightYourDataCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightYourDataCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightYourData1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDiscPayCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDiscPayCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDiscPay1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("RightMyCompanyCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightMyCompanyCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightMyCompany3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirEmployeesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirEmployeesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirEmployees3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightSysSettingsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightSysSettingsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightSysSettings3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightSysJourDispsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightSysJourDispsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightSysJourDisps3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDataExchangeCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDataExchangeCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDataExchange3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightYourDataCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightYourDataCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightYourData3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDiscPayCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDiscPayCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDiscPay3" + ctl.UO_id).setValue(true);
        }
    },

    onRightMyCompanyCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightMyCompany1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightMyCompany3" + ctl.UO_id).setValue(true); }
    },
    onRightDirEmployeesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirEmployees1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirEmployees3" + ctl.UO_id).setValue(true); }
    },
    onRightSysSettingsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightSysSettings1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightSysSettings3" + ctl.UO_id).setValue(true); }
    },
    onRightSysJourDispsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightSysJourDisps1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightSysJourDisps3" + ctl.UO_id).setValue(true); }
    },
    onRightDataExchangeCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDataExchange1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDataExchange3" + ctl.UO_id).setValue(true); }
    },
    onRightYourDataCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightYourData1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightYourData3" + ctl.UO_id).setValue(true); }
    },
    onRightDiscPayCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDiscPay1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDiscPay3" + ctl.UO_id).setValue(true); }
    },


    // *** Справочники *** *** ***
    onRightDir0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightDirNomensCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirNomensCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirNomens1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirNomenCategoriesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirNomenCategoriesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirNomenCategories1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirContractorsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirContractorsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirContractors1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirWarehousesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirWarehousesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirWarehouses1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirBanksCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirBanksCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirBanks1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCashOfficesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirCashOfficesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirCashOffices1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCurrenciesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirCurrenciesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirCurrencies1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirVatsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirVatsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirVats1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirDiscountsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirDiscountsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirDiscounts1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirBonusesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirBonusesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirBonuses1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirOrdersStatesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirOrdersStatesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirOrdersStates1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharColoursCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirCharColoursCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirCharColours1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharMaterialsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirCharMaterialsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirCharMaterials1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharNamesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirCharNamesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirCharNames1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharSeasonsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirCharSeasonsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirCharSeasons1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharSexesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirCharSexesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirCharSexes1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharSizesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirCharSizesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirCharSizes1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharStylesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirCharStylesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirCharStyles1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharTexturesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirCharTexturesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirCharTextures1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("RightDirNomensCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirNomensCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirNomens3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirNomenCategoriesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirNomenCategoriesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirNomenCategories3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirContractorsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirContractorsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirContractors3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirWarehousesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirWarehousesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirWarehouses3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirBanksCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirBanksCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirBanks3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCashOfficesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirCashOfficesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirCashOffices3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCurrenciesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirCurrenciesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirCurrencies3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirVatsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirVatsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirVats3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirDiscountsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirDiscountsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirDiscounts3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirBonusesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirBonusesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirBonuses3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirOrdersStatesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirOrdersStatesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirOrdersStates3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharColoursCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirCharColoursCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirCharColours3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharMaterialsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirCharMaterialsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirCharMaterials3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharNamesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirCharNamesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirCharNames3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharSeasonsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirCharSeasonsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirCharSeasons3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharSexesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirCharSexesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirCharSexes3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharSizesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirCharSizesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirCharSizes3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharStylesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirCharStylesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirCharStyles3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirCharTexturesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirCharTexturesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirCharTextures3" + ctl.UO_id).setValue(true);
        }
    },

    onRightDirNomensCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirNomens1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirNomens3" + ctl.UO_id).setValue(true); }
    },
    onRightDirNomenCategoriesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirNomenCategories1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirNomenCategories3" + ctl.UO_id).setValue(true); }
    },
    onRightDirContractorsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirContractors1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirContractors3" + ctl.UO_id).setValue(true); }
    },
    onRightDirWarehousesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirWarehouses1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirWarehouses3" + ctl.UO_id).setValue(true); }
    },
    onRightDirBanksCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirBanks1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirBanks3" + ctl.UO_id).setValue(true); }
    },
    onRightDirCashOfficesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirCashOffices1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirCashOffices3" + ctl.UO_id).setValue(true); }
    },
    onRightDirCurrenciesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirCurrencies1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirCurrencies3" + ctl.UO_id).setValue(true); }
    },
    onRightDirVatsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirVats1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirVats3" + ctl.UO_id).setValue(true); }
    },
    onRightDirDiscountsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirDiscounts1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirDiscounts3" + ctl.UO_id).setValue(true); }
    },
    onRightDirBonusesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirBonuses1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirBonuses3" + ctl.UO_id).setValue(true); }
    },
    onRightDirOrdersStatesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirOrdersStates1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirOrdersStates3" + ctl.UO_id).setValue(true); }
    },
    onRightDirCharColoursCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirCharColours1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirCharColours3" + ctl.UO_id).setValue(true); }
    },
    onRightDirCharMaterialsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirCharMaterials1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirCharMaterials3" + ctl.UO_id).setValue(true); }
    },
    onRightDirCharNamesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirCharNames1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirCharNames3" + ctl.UO_id).setValue(true); }
    },
    onRightDirCharSeasonsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirCharSeasons1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirCharSeasons3" + ctl.UO_id).setValue(true); }
    },
    onRightDirCharSexesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirCharSexes1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirCharSexes3" + ctl.UO_id).setValue(true); }
    },
    onRightDirCharSizesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirCharSizes1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirCharSizes3" + ctl.UO_id).setValue(true); }
    },
    onRightDirCharStylesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirCharStyles1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirCharStyles3" + ctl.UO_id).setValue(true); }
    },
    onRightDirCharTexturesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirCharTextures1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirCharTextures3" + ctl.UO_id).setValue(true); }
    },


    // *** Документы *** *** ***
    onRightDoc0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightDocPurchesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocPurchesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocPurches1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocReturnVendorsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocReturnVendorsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocReturnVendors1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocMovementsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocMovementsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocMovements1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSalesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocSalesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocSales1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocReturnsCustomersCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocReturnsCustomersCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocReturnsCustomers1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocActOnWorksCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocActOnWorksCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocActOnWorks1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocAccountsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocAccountsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocAccounts1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocActWriteOffsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocActWriteOffsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocActWriteOffs1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocInventoriesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocInventoriesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocInventories1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocNomenRevaluationsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocNomenRevaluationsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocNomenRevaluations1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportTotalTradeCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightReportTotalTradeCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightReportTotalTrade1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportTotalTradePriceCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightReportTotalTradePriceCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightReportTotalTradePrice1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocDescriptionCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocDescriptionCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocDescription1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("RightDocPurchesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocPurchesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocPurches3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocReturnVendorsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocReturnVendorsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocReturnVendors3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocMovementsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocMovementsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocMovements3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSalesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocSalesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocSales3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocReturnsCustomersCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocReturnsCustomersCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocReturnsCustomers3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocActOnWorksCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocActOnWorksCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocActOnWorks3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocAccountsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocAccountsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocAccounts3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocActWriteOffsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocActWriteOffsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocActWriteOffs3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocActWriteOffsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocActWriteOffsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocActWriteOffs3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocInventoriesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocInventoriesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocInventories3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocNomenRevaluationsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocNomenRevaluationsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocNomenRevaluations3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportTotalTradeCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightReportTotalTradeCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightReportTotalTrade3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportTotalTradePriceCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightReportTotalTradePriceCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightReportTotalTradePrice3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocDescriptionCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocDescriptionCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocDescription3" + ctl.UO_id).setValue(true);
        }
    },


    onRightDocPurchesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocPurches1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocPurches3" + ctl.UO_id).setValue(true); }
    },
    onRightDocReturnVendorsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocReturnVendors1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocReturnVendors3" + ctl.UO_id).setValue(true); }
    },
    onRightDocMovementsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocMovements1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocMovements3" + ctl.UO_id).setValue(true); }
    },
    onRightDocSalesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocSales1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocSales3" + ctl.UO_id).setValue(true); }
    },
    onRightDocReturnsCustomersCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocReturnsCustomers1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocReturnsCustomers3" + ctl.UO_id).setValue(true); }
    },
    onRightDocActOnWorksCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocActOnWorks1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocActOnWorks3" + ctl.UO_id).setValue(true); }
    },
    onRightDocAccountsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocAccounts1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocAccounts3" + ctl.UO_id).setValue(true); }
    },
    onRightDocActWriteOffsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocActWriteOffs1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocActWriteOffs3" + ctl.UO_id).setValue(true); }
    },
    onRightDocInventoriesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocInventories1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocInventories3" + ctl.UO_id).setValue(true); }
    },
    onRightDocNomenRevaluationsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocNomenRevaluations1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocNomenRevaluations3" + ctl.UO_id).setValue(true); }
    },
    onRightReportTotalTradeCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightReportTotalTrade1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightReportTotalTrade3" + ctl.UO_id).setValue(true); }
    },
    onRightReportTotalTradePriceCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightReportTotalTradePrice1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightReportTotalTradePrice3" + ctl.UO_id).setValue(true); }
    },
    onRightDocDescriptionCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocDescription1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocDescription3" + ctl.UO_id).setValue(true); }
    },


    // *** Документы.Витрина *** *** ***
    onRightVitrina0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightDocRetailsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocRetailsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocRetails1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocRetailReturnsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocRetailReturnsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocRetailReturns1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("RightDocRetailsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocRetailsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocRetails3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocRetailReturnsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocRetailReturnsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocRetailReturns3" + ctl.UO_id).setValue(true);
        }
    },

    onRightDocRetailsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocRetails1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocRetails3" + ctl.UO_id).setValue(true); }
    },
    onRightDocRetailReturnsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocRetailReturns1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocRetailReturns3" + ctl.UO_id).setValue(true); }
    },


    // *** Сервис *** *** ***
    onRightDocService0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightDocServicePurchesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServicePurchesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocServicePurches1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServicePurch1TabsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServicePurch1TabsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocServicePurch1Tabs1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServicePurch2TabsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServicePurch2TabsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocServicePurch2Tabs1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceWorkshopsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServiceWorkshopsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocServiceWorkshops1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceWorkshopsTab2ReturnCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServiceWorkshopsTab2ReturnCheck" + ctl.UO_id).setReadOnly(false); //Ext.getCmp("RightDocServiceWorkshopsTab2Return1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceWorkshopsTab1AddCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServiceWorkshopsTab1AddCheck" + ctl.UO_id).setReadOnly(false); //Ext.getCmp("RightDocServiceWorkshopsTab1Add1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceOutputsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServiceOutputsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocServiceOutputs1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceArchivesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServiceArchivesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocServiceArchives1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServicePurchesExtraditionCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServicePurchesExtraditionCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocServicePurchesExtradition1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServicePurchesDiscountCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServicePurchesDiscountCheck" + ctl.UO_id).setReadOnly(false); //Ext.getCmp("RightDocServicePurchesDiscount1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceMovementsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServiceMovementsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocServiceMovements1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceInventoriesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServiceInventoriesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocServiceInventories1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirServiceNomensCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirServiceNomensCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirServiceNomens1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirServiceNomenCategoriesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirServiceNomenCategoriesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirServiceNomenCategories1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirServiceContractorsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirServiceContractorsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirServiceContractors1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirServiceJobNomensCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirServiceJobNomensCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirServiceJobNomens1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirSmsTemplatesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirSmsTemplatesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirSmsTemplates1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirServiceDiagnosticRresultsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirServiceDiagnosticRresultsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirServiceDiagnosticRresults1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirServiceNomenTypicalFaultsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirServiceNomenTypicalFaultsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirServiceNomenTypicalFaults1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServicePurchesReportCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServicePurchesReportCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocServicePurchesReport1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServicePurchesDateDoneCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocServicePurchesDateDoneCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocServicePurchesReport1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("RightDocServicePurchesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServicePurchesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServicePurches3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServicePurch1TabsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServicePurch1TabsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServicePurch1Tabs3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServicePurch2TabsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServicePurch2TabsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServicePurch2Tabs3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceWorkshopsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServiceWorkshopsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServiceWorkshops3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceWorkshopsTab2ReturnCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServiceWorkshopsTab2ReturnCheck" + ctl.UO_id).setReadOnly(true); //Ext.getCmp("RightDocServiceWorkshopsTab2Return3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceWorkshopsTab1AddCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServiceWorkshopsTab1AddCheck" + ctl.UO_id).setReadOnly(true); //Ext.getCmp("RightDocServiceWorkshopsTab1Add3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceOutputsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServiceOutputsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServiceOutputs3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceArchivesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServiceArchivesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServiceArchives3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServicePurchesExtraditionCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServicePurchesExtraditionCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServicePurchesExtradition3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServicePurchesDiscountCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServicePurchesDiscountCheck" + ctl.UO_id).setReadOnly(true); //Ext.getCmp("RightDocServicePurchesDiscount3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceMovementsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServiceMovementsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServiceMovements3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServiceInventoriesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServiceInventoriesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServiceInventories3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirServiceNomensCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirServiceNomensCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirServiceNomens3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirServiceNomenCategoriesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirServiceNomenCategoriesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirServiceNomenCategories3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirServiceContractorsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirServiceContractorsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirServiceContractors3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirServiceJobNomensCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirServiceJobNomensCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirServiceJobNomens3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirSmsTemplatesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirSmsTemplatesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirSmsTemplates3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirServiceDiagnosticRresultsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirServiceDiagnosticRresultsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirServiceDiagnosticRresults3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirServiceNomenTypicalFaultsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirServiceNomenTypicalFaultsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirServiceNomenTypicalFaults3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServicePurchesReportCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServicePurchesReportCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServicePurchesReport3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocServicePurchesDateDoneCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocServicePurchesDateDoneCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServicePurchesReport3" + ctl.UO_id).setValue(true);
        }
    },

    onRightDocServicePurchesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocServicePurches1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocServicePurches3" + ctl.UO_id).setValue(true); }
    },
    onRightDocServicePurch1TabsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocServicePurch1Tabs1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocServicePurch1Tabs3" + ctl.UO_id).setValue(true); }
    },
    onRightDocServicePurch2TabsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocServicePurch2Tabs1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocServicePurch2Tabs3" + ctl.UO_id).setValue(true); }
    },
    onRightDocServiceWorkshopsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocServiceWorkshops1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocServiceWorkshops3" + ctl.UO_id).setValue(true); }
    },
    onRightDocServiceWorkshopsTab2ReturnCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

    },
    onRightDocServiceWorkshopsTab1AddCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

    },
    onRightDocServiceWorkshopsOnlyUsersCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //if (val) { Ext.getCmp("RightDocServiceWorkshopsOnlyUsers1" + ctl.UO_id).setValue(true); }
        //else { Ext.getCmp("RightDocServiceWorkshopsOnlyUsers3" + ctl.UO_id).setValue(true); }
    },
    onRightDocServiceOutputsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }
        
        if (val) {
            Ext.getCmp("RightDocServiceOutputs1" + ctl.UO_id).setValue(true);

            Ext.getCmp("RightDocServicePurchesExtraditionCheck" + ctl.UO_id).setReadOnly(false);
            Ext.getCmp("RightDocServicePurchesExtradition1" + ctl.UO_id).setReadOnly(false);
            Ext.getCmp("RightDocServicePurchesExtradition2" + ctl.UO_id).setReadOnly(false);
            Ext.getCmp("RightDocServicePurchesExtradition3" + ctl.UO_id).setReadOnly(false);
        }
        else {
            Ext.getCmp("RightDocServiceOutputs3" + ctl.UO_id).setValue(true);

            Ext.getCmp("RightDocServicePurchesExtraditionCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServicePurchesExtraditionCheck" + ctl.UO_id).setValue(false);
            Ext.getCmp("RightDocServicePurchesExtradition1" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServicePurchesExtradition1" + ctl.UO_id).setValue(false);
            Ext.getCmp("RightDocServicePurchesExtradition2" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServicePurchesExtradition2" + ctl.UO_id).setValue(false);
            Ext.getCmp("RightDocServicePurchesExtradition3" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocServicePurchesExtradition3" + ctl.UO_id).setValue(true);
        }
    },
    onRightDocServiceArchivesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocServiceArchives1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocServiceArchives3" + ctl.UO_id).setValue(true); }
    },
    onRightDocServicePurchesExtraditionCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocServicePurchesExtradition1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocServicePurchesExtradition3" + ctl.UO_id).setValue(true); }
    },
    /*onRightDocServicePurchesDiscountCheckChecked: function (ctl, val) {
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocServicePurchesDiscount1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocServicePurchesDiscount3" + ctl.UO_id).setValue(true); }
    },*/
    onRightDocServicePurchesReportCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocServicePurchesReport1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocServicePurchesReport3" + ctl.UO_id).setValue(true); }
    },
    onRightDocServicePurchesDateDoneCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }
    },
    onRightDocServiceMovementsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocServiceMovements1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocServiceMovements3" + ctl.UO_id).setValue(true); }
    },
    /*onRightDocServiceMovementsCheckChecked: function (ctl, val) {
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocServiceMovements1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocServiceMovements3" + ctl.UO_id).setValue(true); }
    },*/
    onRightDocServiceInventoriesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocServiceInventories1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocServiceInventories3" + ctl.UO_id).setValue(true); }
    },
    onRightDirServiceNomenCategoriesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirServiceNomenCategories1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirServiceNomenCategories3" + ctl.UO_id).setValue(true); }
    },
    onRightDirServiceContractorsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirServiceContractors1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirServiceContractors3" + ctl.UO_id).setValue(true); }
    },
    onRightDirServiceJobNomensCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirServiceJobNomens1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirServiceJobNomens3" + ctl.UO_id).setValue(true); }
    },
    onRightDirSmsTemplatesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirSmsTemplates1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirSmsTemplates3" + ctl.UO_id).setValue(true); }
    },
    onRightDirServiceDiagnosticRresultsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirServiceDiagnosticRresults1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirServiceDiagnosticRresults3" + ctl.UO_id).setValue(true); }
    },
    onRightDirServiceNomenTypicalFaultsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirServiceNomenTypicalFaults1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirServiceNomenTypicalFaults3" + ctl.UO_id).setValue(true); }
    },


    // *** Б/У *** *** ***
    onRightDocSecondHands0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightDocSecondHandPurchesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocSecondHandPurchesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocSecondHandPurches1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandPurch1TabsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocSecondHandPurch1TabsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocSecondHandPurch1Tabs1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandPurch2TabsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocSecondHandPurch2TabsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocSecondHandPurch2Tabs1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandWorkshopsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocSecondHandWorkshopsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocSecondHandWorkshops1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandRetailsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocSecondHandRetailsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocSecondHandRetails1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandRetailReturnsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocSecondHandRetailReturnsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocSecondHandRetailReturns1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandRetailActWriteOffsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocSecondHandRetailActWriteOffsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocSecondHandRetailActWriteOffs1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandMovementsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocSecondHandMovementsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocSecondHandMovements1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandsReportCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocSecondHandsReportCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocSecondHandsReport1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandInventoriesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocSecondHandInventoriesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocSecondHandInventories1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandRazborsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocSecondHandRazborsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocSecondHandRazbors1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("RightDocSecondHandPurchesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocSecondHandPurchesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocSecondHandPurches3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandPurch1TabsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocSecondHandPurch1TabsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocSecondHandPurch1Tabs3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandPurch2TabsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocSecondHandPurch2TabsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocSecondHandPurch2Tabs3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandWorkshopsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocSecondHandWorkshopsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocSecondHandWorkshops3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandRetailsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocSecondHandRetailsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocSecondHandRetails3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandRetailReturnsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocSecondHandRetailReturnsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocSecondHandRetailReturns3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandRetailActWriteOffsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocSecondHandRetailActWriteOffsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocSecondHandRetailActWriteOffs3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandMovementsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocSecondHandMovementsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocSecondHandMovements3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandsReportCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocSecondHandsReportCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocSecondHandsReport3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandInventoriesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocSecondHandInventoriesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocSecondHandInventories3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocSecondHandRazborsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocSecondHandRazborsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocSecondHandRazbors3" + ctl.UO_id).setValue(true);
        }
    },

    onRightDocSecondHandPurchesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocSecondHandPurches1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocSecondHandPurches3" + ctl.UO_id).setValue(true); }
    },
    onRightDocSecondHandPurch1TabsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocSecondHandPurch1Tabs1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocSecondHandPurch1Tabs3" + ctl.UO_id).setValue(true); }
    },
    onRightDocSecondHandPurch2TabsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocSecondHandPurch2Tabs1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocSecondHandPurch2Tabs3" + ctl.UO_id).setValue(true); }
    },
    onRightDocSecondHandWorkshopsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocSecondHandWorkshops1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocSecondHandWorkshops3" + ctl.UO_id).setValue(true); }
    },
    onRightDocSecondHandRetailsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocSecondHandRetails1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocSecondHandRetails3" + ctl.UO_id).setValue(true); }
    },
    onRightDocSecondHandRetailReturnsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocSecondHandRetailReturns1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocSecondHandRetailReturns3" + ctl.UO_id).setValue(true); }
    },
    onRightDocSecondHandRetailActWriteOffsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocSecondHandRetailActWriteOffs1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocSecondHandRetailActWriteOffs3" + ctl.UO_id).setValue(true); }
    },
    onRightDocSecondHandMovementsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocSecondHandMovements1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocSecondHandMovements3" + ctl.UO_id).setValue(true); }
    },
    onRightDocSecondHandsReportCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocSecondHandsReport1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocSecondHandsReport3" + ctl.UO_id).setValue(true); }
    },
    onRightDocSecondHandRazborsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocSecondHandRazbors1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocSecondHandRazbors3" + ctl.UO_id).setValue(true); }
    },
    onRightDocSecondHandInventoriesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocSecondHandInventories1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocSecondHandInventories3" + ctl.UO_id).setValue(true); }
    },


    // *** Заказы *** *** ***
    onRightDocOrderInt0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }
        
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightDocOrderIntsNewCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocOrderIntsNewCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocOrderIntsNew1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocOrderIntsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocOrderIntsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocOrderInts1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocOrderIntsReportCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocOrderIntsReportCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocOrderIntsReport1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocOrderIntsArchiveCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocOrderIntsArchiveCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocOrderIntsArchive1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirOrderIntContractorsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirOrderIntContractorsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirOrderIntContractors1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("RightDocOrderIntsNewCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocOrderIntsNewCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocOrderIntsNew3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocOrderIntsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocOrderIntsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocOrderInts3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocOrderIntsReportCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocOrderIntsReportCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocOrderIntsReport3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocOrderIntsArchiveCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocOrderIntsArchiveCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocOrderIntsArchive3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirOrderIntContractorsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirOrderIntContractorsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirOrderIntContractors3" + ctl.UO_id).setValue(true);
        }
    },

    onRightDocOrderIntsNewCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }
        
        if (val) { Ext.getCmp("RightDocOrderIntsNew1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocOrderIntsNew3" + ctl.UO_id).setValue(true); }
    },
    onRightDocOrderIntsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocOrderInts1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocOrderInts3" + ctl.UO_id).setValue(true); }
    },
    onRightDocOrderIntsReportCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocOrderIntsReport1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocOrderIntsReport3" + ctl.UO_id).setValue(true); }
    },
    onRightDocOrderIntsArchiveCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocOrderIntsArchive1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocOrderIntsArchive3" + ctl.UO_id).setValue(true); }
    },
    onRightDirOrderIntContractorsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirOrderIntContractors1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirOrderIntContractors3" + ctl.UO_id).setValue(true); }
    },



    // *** Логистика *** *** ***
    onRightLogistics0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }
        
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightDocMovementsLogisticsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocMovementsLogisticsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocMovementsLogistics1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("RightDocMovementsLogisticsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocMovementsLogisticsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocMovementsLogistics3" + ctl.UO_id).setValue(true);
        }
    },

    onRightDocMovementsLogisticsCheckChecked: function (ctl, val) {
        //if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }
        
        if (val) { Ext.getCmp("RightDocMovementsLogistics1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocMovementsLogistics3" + ctl.UO_id).setValue(true); }
    },



    // *** Аналитика *** *** ***
    onRightAnalitics0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("Right1AnaliticsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("Right1AnaliticsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("Right1Analitics1" + ctl.UO_id).setValue(true);
            Ext.getCmp("Right2AnaliticsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("Right2AnaliticsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("Right2Analitics1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("Right1AnaliticsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("Right1AnaliticsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("Right1Analitics3" + ctl.UO_id).setValue(true);
            Ext.getCmp("Right2AnaliticsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("Right2AnaliticsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("Right2Analitics3" + ctl.UO_id).setValue(true);
        }
    },

    onRight1AnaliticsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("Right1Analitics1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("Right1Analitics3" + ctl.UO_id).setValue(true); }
    },
    onRight2AnaliticsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("Right2Analitics1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("Right2Analitics3" + ctl.UO_id).setValue(true); }
    },



    // *** Деньги *** *** ***
    onRightDocBankCash0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightDocBankSumsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocBankSumsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocBankSums1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocCashOfficeSumMovementsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocCashOfficeSumMovementsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocCashOfficeSumMovements1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocCashOfficeSumsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocCashOfficeSumsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocCashOfficeSums1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportBanksCashOfficesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightReportBanksCashOfficesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightReportBanksCashOffices1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("RightDocBankSumsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocBankSumsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocBankSums3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocCashOfficeSumMovementsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocCashOfficeSumMovementsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocCashOfficeSumMovements3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocCashOfficeSumsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocCashOfficeSumsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocCashOfficeSums3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportBanksCashOfficesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightReportBanksCashOfficesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightReportBanksCashOffices3" + ctl.UO_id).setValue(true);
        }
    },

    onRightDocBankSumsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocBankSums1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocBankSums3" + ctl.UO_id).setValue(true); }
    },
    onRightDocCashOfficeSumMovementsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocCashOfficeSumMovements1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocCashOfficeSumMovements3" + ctl.UO_id).setValue(true); }
    },
    onRightDocCashOfficeSumsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocCashOfficeSums1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocCashOfficeSums3" + ctl.UO_id).setValue(true); }
    },
    onRightReportBanksCashOfficesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightReportBanksCashOffices1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightReportBanksCashOffices3" + ctl.UO_id).setValue(true); }
    },


    // *** Зарплата *** *** ***
    onRightSalaries0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightReportSalariesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightReportSalariesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightReportSalaries1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportSalariesWarehousesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightReportSalariesWarehousesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightReportSalariesWarehouses1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirDomesticExpensesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirDomesticExpensesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirDomesticExpenses1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocDomesticExpenseSalariesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocDomesticExpenseSalariesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocDomesticExpenseSalaries1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocDomesticExpensesCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocDomesticExpensesCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocDomesticExpenses1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportSalariesEmplCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightReportSalariesEmplCheck" + ctl.UO_id).setReadOnly(false); 
        }
        else {
            Ext.getCmp("RightReportSalariesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightReportSalariesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightReportSalaries3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportSalariesWarehousesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightReportSalariesWarehousesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightReportSalariesWarehouses3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirDomesticExpensesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDirDomesticExpensesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDirDomesticExpenses3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocDomesticExpenseSalariesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocDomesticExpenseSalariesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocDomesticExpenseSalaries3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocDomesticExpensesCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocDomesticExpensesCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocDomesticExpenses3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportSalariesEmplCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightReportSalariesEmplCheck" + ctl.UO_id).setReadOnly(true); 
        }
    },

    onRightReportSalariesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightReportSalaries1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightReportSalaries3" + ctl.UO_id).setValue(true); }
    },
    onRightReportSalariesWarehousesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightReportSalariesWarehouses1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightReportSalariesWarehouses3" + ctl.UO_id).setValue(true); }
    },
    onRightDirDomesticExpensesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirDomesticExpenses1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirDomesticExpenses3" + ctl.UO_id).setValue(true); }
    },
    onRightDocDomesticExpenseSalariesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocDomesticExpenseSalaries1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocDomesticExpenseSalaries3" + ctl.UO_id).setValue(true); }
    },
    onRightDocDomesticExpensesCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocDomesticExpenses1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocDomesticExpenses3" + ctl.UO_id).setValue(true); }
    },
    onRightReportSalariesEmplCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }
    },



    // *** Отчет *** *** ***
    onRightReport0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightReportPriceListCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightReportPriceListCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightReportPriceList1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportRemnantsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightReportRemnantsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightReportRemnants1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportProfitCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightReportProfitCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightReportProfit1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocPurchesPrintCodeCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDocPurchesPrintCodeCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDocPurchesPrintCode1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("RightReportPriceListCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightReportPriceListCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightReportPriceList3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportRemnantsCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightReportRemnantsCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightReportRemnants3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightReportProfitCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightReportProfitCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightReportProfit3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDocPurchesPrintCodeCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDocPurchesPrintCodeCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDocPurchesPrintCode3" + ctl.UO_id).setValue(true);
        }
    },

    onRightReportPriceListCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightReportPriceList1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightReportPriceList3" + ctl.UO_id).setValue(true); }
    },
    onRightReportRemnantsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightReportRemnants1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightReportRemnants3" + ctl.UO_id).setValue(true); }
    },
    onRightReportProfitCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightReportProfit1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightReportProfit3" + ctl.UO_id).setValue(true); }
    },
    onRightDocPurchesPrintCodeCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDocPurchesPrintCode1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDocPurchesPrintCode3" + ctl.UO_id).setValue(true); }
    },



    // *** ККМ *** *** ***
    onRightKKM0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightKKMXReportCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightKKMXReportCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightKKMXReport1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMOpenCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightKKMOpenCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightKKMOpen1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMEncashmentCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightKKMEncashmentCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightKKMEncashment1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMAddingCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightKKMAddingCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightKKMAdding1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMCloseCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightKKMCloseCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightKKMClose1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMPrintOFDCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightKKMPrintOFDCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightKKMPrintOFD1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMCheckLastFNCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightKKMCheckLastFNCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightKKMCheckLastFN1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMStateCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightKKMStateCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightKKMState1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMListCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightKKMListCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightKKMList1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("RightKKMXReportCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightKKMXReportCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightKKMXReport3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMOpenCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightKKMOpenCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightKKMOpen3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMEncashmentCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightKKMEncashmentCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightKKMEncashment3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMAddingCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightKKMAddingCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightKKMAdding3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMCloseCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightKKMCloseCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightKKMClose3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMPrintOFDCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightKKMPrintOFDCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightKKMPrintOFD3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMCheckLastFNCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightKKMCheckLastFNCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightKKMCheckLastFN3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMStateCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightKKMStateCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightKKMState3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightKKMListCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightKKMListCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightKKMList3" + ctl.UO_id).setValue(true);
        }
    },

    onRightKKMXReportCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightKKMXReport1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightKKMXReport3" + ctl.UO_id).setValue(true); }
    },
    onRightKKMOpenCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightKKMOpen1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightKKMOpen3" + ctl.UO_id).setValue(true); }
    },
    onRightKKMEncashmentCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightKKMEncashment1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightKKMEncashment3" + ctl.UO_id).setValue(true); }
    },
    onRightKKMAddingCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightKKMAdding1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightKKMAdding3" + ctl.UO_id).setValue(true); }
    },
    onRightKKMCloseCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightKKMClose1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightKKMClose3" + ctl.UO_id).setValue(true); }
    },
    onRightKKMPrintOFDCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightKKMPrintOFD1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightKKMPrintOFD3" + ctl.UO_id).setValue(true); }
    },
    onRightKKMCheckLastFNCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightKKMCheckLastFN1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightKKMCheckLastFN3" + ctl.UO_id).setValue(true); }
    },
    onRightKKMStateCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightKKMState1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightKKMState3" + ctl.UO_id).setValue(true); }
    },
    onRightKKMListCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightKKMList1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightKKMList3" + ctl.UO_id).setValue(true); }
    },




    // *** Другое *** *** ***
    onRightOther0Checked: function (ctl, val) {
        //if (varBlock) { return; }

        //Если Системная запись - ничего не делает, просто выводит сообщение!
        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("RightDevelopCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDevelopCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDevelop1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightAPI10sCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightAPI10sCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightAPI10s1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirWebShopUOsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirWebShopUOsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirWebShopUOs1" + ctl.UO_id).setValue(true);
        }
        else {
            Ext.getCmp("RightDevelopCheck" + ctl.UO_id).setValue(false); Ext.getCmp("RightDevelopCheck" + ctl.UO_id).setReadOnly(true); Ext.getCmp("RightDevelop3" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightAPI10sCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightAPI10sCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightAPI10s1" + ctl.UO_id).setValue(true);
            Ext.getCmp("RightDirWebShopUOsCheck" + ctl.UO_id).setValue(true); Ext.getCmp("RightDirWebShopUOsCheck" + ctl.UO_id).setReadOnly(false); Ext.getCmp("RightDirWebShopUOs1" + ctl.UO_id).setValue(true);
        }
    },

    onRightDevelopCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDevelop1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDevelop3" + ctl.UO_id).setValue(true); }
    },

    onRightAPI10sCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightAPI10s1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightAPI10s3" + ctl.UO_id).setValue(true); }
    },

    onRightDirWebShopUOsCheckChecked: function (ctl, val) {
        if (varBlock) { return; }

        if (Ext.getCmp("SysRecord" + ctl.UO_id).getValue() == true) { Ext.Msg.alert(lanOrgName, "Эта запись системная! Эти действия не будут сохранены!"); }

        if (val) { Ext.getCmp("RightDirWebShopUOs1" + ctl.UO_id).setValue(true); }
        else { Ext.getCmp("RightDirWebShopUOs3" + ctl.UO_id).setValue(true); }
    },


});