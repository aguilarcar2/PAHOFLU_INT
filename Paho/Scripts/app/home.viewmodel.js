function HomeViewModel(app, dataModel) {
    var self = this;    
    self.newEnable = ko.observable(false);
    self.showGrid = ko.observable(true);
    self.editMode = ko.observable(false);
    self.newMode = ko.observable(false);
    self.displayFilters = ko.observable(true);
    self.myHometown = ko.observable("");
    self.countries = ko.observableArray(countries);
    self.regions = ko.observableArray(regions);
    self.activecountries = ko.computed(function () {
        return $.grep( self.countries(), function(v) {
            return v.Active === true;
        });
    });
    
    // Catalogos de laboratorio
    self.CTT = ko.observableArray(CTT);
    self.CTR = ko.observableArray(CTR);
    self.CVT = ko.observableArray(CVT);
    self.CVST = ko.observableArray(CVST);
    self.CVL = ko.observableArray(CVL);

    self.CNP = ko.observableArray(CNP);
    self.CVS = ko.observableArray(CVS);

    // Control de flujo
    self.UserRole = ko.observable(URle);

    self.UsrCountry = ko.observable(selcty);
    self.selectedCountryId = ko.observable("");
    self.selectedRegionId = ko.observable("");
    self.institutions = ko.observableArray(institutions);
    self.selectedInstitutionId = ko.observable("");
    self.SCasenumber = ko.observable("");
    self.SNStartDate = ko.observable("");
    self.SNEndDate = ko.observable("");
    
    self.labs = ko.observableArray(labs);
    self.labsHospital = ko.observableArray([]);
    self.RecordHistoryItems = ko.observableArray([]);

    self.shouldcaselist = ko.computed(function () {
        return typeof self.selectedInstitutionId() !== "undefined" && self.selectedInstitutionId() !== "" && self.showGrid();
    }, self);    
    self.shouldtabs = ko.computed(function () {
        return self.editMode();
    });
    self.newButton = ko.computed(function () {
        return typeof self.selectedInstitutionId() !== "undefined" && self.selectedInstitutionId() !== "" && !self.editMode();
    });

    // Inicia EsRut validacion para Chile
    self.revisarDigito = function (componente) {
        var crut = componente
        largo = crut.length;
        if (largo < 2) {
            return false;
        }
        if (largo > 2)
            rut = crut.substring(0, largo - 1);
        else
            rut = crut.charAt(0);
        dv = crut.charAt(largo - 1);

        if (dv != '0' && dv != '1' && dv != '2' && dv != '3' && dv != '4' && dv != '5' && dv != '6' && dv != '7' && dv != '8' && dv != '9' && dv != 'k' && dv != 'K') {
            return false;
        }

        if (rut == null || dv == null)
            return 0

        var dvr = '0'
        suma = 0
        mul = 2

        for (i = rut.length - 1 ; i >= 0; i--) {
            suma = suma + rut.charAt(i) * mul
            if (mul == 7)
                mul = 2
            else
                mul++
        }
        res = suma % 11
        if (res == 1)
            dvr = 'k'
        else if (res == 0)
            dvr = '0'
        else {
            dvi = 11 - res
            dvr = dvi + ""
        }
        if (dvr.toUpperCase() != dv.toUpperCase()) {
            return false
        }

        return true
    }; // Fin revisar digito RUT

    // Inicia EsRut validacion para Chile
    self.EsRut = function (texto) {
        var tmpstr = "";
        for (i = 0; i < texto.length ; i++)
            if (texto.charAt(i) != ' ' && texto.charAt(i) != '.' && texto.charAt(i) != '-')
                tmpstr = tmpstr + texto.charAt(i);
        texto = tmpstr;
        largo = texto.length;

        if (largo < 2) {
            return false;
        }

        for (i = 0; i < largo ; i++) {
            if (texto.charAt(i) != "0" && texto.charAt(i) != "1" && texto.charAt(i) != "2" && texto.charAt(i) != "3" && texto.charAt(i) != "4" && texto.charAt(i) != "5" && texto.charAt(i) != "6" && texto.charAt(i) != "7" && texto.charAt(i) != "8" && texto.charAt(i) != "9" && texto.charAt(i) != "k" && texto.charAt(i) != "K") {
                return false;
            }
        }

        var invertido = "";
        for (i = (largo - 1), j = 0; i >= 0; i--, j++)
            invertido = invertido + texto.charAt(i);
        var dtexto = "";
        dtexto = dtexto + invertido.charAt(0);
        dtexto = dtexto + '-';
        cnt = 0;

        for (i = 1, j = 2; i < largo; i++, j++) {
            if (cnt == 3) {
                dtexto = dtexto + '.';
                j++;
                dtexto = dtexto + invertido.charAt(i);
                cnt = 1;
            }
            else {
                dtexto = dtexto + invertido.charAt(i);
                cnt++;
            }
        }

        invertido = "";
        for (i = (dtexto.length - 1), j = 0; i >= 0; i--, j++)
            invertido = invertido + dtexto.charAt(i);

        if (self.revisarDigito(texto))
            return true;

        return false;
    }; // fin EsRut

    self.esrut = function (rut) {
        //if (rut == 0) {
        //    alert("Debe ingresar un RUT");
        //    document.form1.Tx_RUN2.focus();//me posiciono en el campo nuevamente
        //    return false;
        //} else {
        if (rut.search("-") <= 0) {
            alert(viewValidateRUN + rut + viewValidateRUNSinDash);
            self.SCasenumber("");//limpio el valor del campo rut
            $("#SCasenumber").focus();//me posiciono en el campo nuevamente
            return false;
        }
        if (self.EsRut(rut) == false) {
            alert(viewValidateRUN + rut + viewValidateRUNNotValid);
            self.NoExpediente("");//limpio el valor del campo rut
            $("#SCasenumber").focus();//me posiciono en el campo nuevamente
            return false;
        } else {
            if (rut.length == 9) {
                var la = rut.substring(0, 8);
                var digito = rut.charAt(8);

            } else {
                var la = rut.substring(0, 8);
                var digito = rut.charAt(9);
            }

        }
        //}
    };  // Fin validacion es RUT

    self.SCasenumber.subscribe(function (NewRUN) {
        if (self.UsrCountry() == 7 && NewRUN != "" && NewRUN != "9.999.999") {
            self.esrut(NewRUN);
        }
    });

    self.ReloadFluCases = function () {
        if (typeof self.selectedInstitutionId() === 'undefined' && typeof self.selectedRegionId() === 'undefined') {
            $("#caselist").jqGrid('clearGridData');
            $("#SCasenumber").val("");
            $("#SHospRegis").val("");
            $("#SCaseStatus").val("");
            $("#SNStartDate").val("");
            $("#SNEndDate").val("");
            $("#caselist_pend").jqGrid('clearGridData');
            console.log("reload_1");

        }
        else {
            if (self.selectedRegionId() > 0) {
                self.ReloadInstitutions();
            }
            //else {
            //    self.loadInstitutions();
            //}
            $("#caselist").trigger("reloadGrid");
            $.getJSON(app.dataModel.getLabsHospitalUrl, { institutionId: self.selectedInstitutionId() }, function (data, status) {
                self.labsHospital(data.LabsHospital);
            });
            $("#caselist_pend").trigger('reloadGrid');
            console.log("reload_2");
        }
    };

    self.NewFluCase = function () {
        //self.ResetFluCase();
        //self.EditFluCase();
        $("#RecordNumber").text("Nuevo");
        if ($("#ITy").val() == "1") {
            $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', false);
            $("#RegDate").attr('disabled', true);
            $("#tab-lab :input").prop('disabled', true);
            self.ResetFluCase();
            self.EditFluCase();
            if (app.Views.Contact.Id() == null) {
                $("a[href*='tab-case']").hide();
                $("#tab-case").hide();
                $("a[href*='tab-lab']").hide();
            }

        } else
        {
            $("#tab-lab :input").prop('disabled', false);
        }
        $("#tabs").tabs("option", "active", 0);
        self.selectedInstitutionId() != "" ?  app.Views.Contact.hospitalName($('#Hospitals option:selected').text()) : "";
    };


    self.CancelEdit = function () {
        self.displayFilters(true);
        self.editMode(false);
        self.showGrid(true);
        self.ReloadFluCases();
        //self.selectedInstitutionId("");
    };

    self.EditFluCase = function () {
        $("#caselist").jqGrid('clearGridData');
        $("#caselist_pend").jqGrid('clearGridData');
        self.showGrid(false);
        self.editMode(true);
        self.displayFilters(false);
        $('#tabs').tabs({ active: 0 });
        app.Views.Hospital.LabsHospital(self.labsHospital());
        app.Views.Lab.LabsResult(self.labs());


    };





    self.ResetFluCase = function () {
        app.Views.Contact.ResetContact();
        app.Views.GEO.ResetGEO();
        app.Views.Risk.ResetRisk();
        app.Views.Hospital.ResetHospital();
        app.Views.Lab.ResetLab();
        //$("#save").attr("disabled", false);
        //$("#saveGEO").attr("disabled", false);
        //$("#saveRisk").attr("disabled", false);
        //$("#saveHospital").attr("disabled", false);
        //$("#saveLab").attr("disabled", false);
        $("#saveGeneral").attr("disabled", false);
        $("#saveGeneralPrev").attr("disabled", true);
        //$("#cancel").attr("disabled", false);
        //$("#cancelGEO").attr("disabled", false);
        //$("#cancelRisk").attr("disabled", false);
        //$("#cancelHospital").attr("disabled", false);
        //$("#cancelLab").attr("disabled", false);
        $("#cancelGeneral").attr("disabled", false);
        //$("#BorrarGeneral").attr("disabled", false);
        $("#RecordNumber").text("Nuevo");
    };

    self.SaveAll = function (option_Save) {
        $("#save").attr("disabled", true);
        $("#saveGEO").attr("disabled", true);
        $("#saveRisk").attr("disabled", true);
        $("#saveHospital").attr("disabled", true);
        $("#saveLab").attr("disabled", true);
        $("#saveGeneral").attr("disabled", true);
        $("#saveGeneralPrev").attr("disabled", true); 
        $("#BorrarGeneral").attr("disabled", true);
        $("#o_S").val(option_Save);

        if ($("#ITy").val() == "2") {
            app.Views.Lab.SaveLab(function () {
                alert(viewValidateSavedInfo + app.Views.Lab.Id);
                if (option_Save == 1 || option_Save ==2)
                    app.Views.Home.CancelEdit();
                if (option_Save == 0)
                    app.Views.Home.NewFluCase();
            });
        }
        else {
            app.Views.Contact.SaveContact(function () {
                app.Views.GEO.SaveGEO(function () {
                    app.Views.Risk.SaveRisk(function () {
                        app.Views.Hospital.SaveHospital(function () {
                            if ($("#ITy").val() != "2") {
                                alert(viewValidateSavedRecord + app.Views.Hospital.Id);
                                //app.Views.Home.CancelEdit();
                                if (option_Save == 1 || option_Save == 2) app.Views.Home.CancelEdit();
                                if (option_Save == 0) app.Views.Home.NewFluCase();
                                //app.Views.Home.ResetFluCase();
                            } else {
                                app.Views.Lab.SaveLab(function () {
                                    alert(viewValidateSavedRecord + app.Views.Lab.Id);
                                    app.Views.Home.CancelEdit();
                                });
                            }
                            
                        });
                    });
                });
            });
        }
         
    };

    self.ValidateAll = function (option_Save) {
        if ($("#ITy").val() == "2") {
            app.Views.Lab.validate(function () {
                self.SaveAll(option_Save);
            });
        } else {
            app.Views.Contact.validate(function () {
                app.Views.GEO.validate(function () {
                    app.Views.Risk.validate(function () {
                        app.Views.Hospital.validate(function () {
                            if ($("#ITy").val() != "2") {
                                self.SaveAll(option_Save);
                            } else {
                                app.Views.Lab.validate(function () {
                                    self.SaveAll(option_Save);
                                });
                            }
                        });
                    });
                });
            });
        };
    };

    self.ReloadInstitutions = function () {        
        if (typeof self.selectedCountryId() === "undefined") {
            return;
        }
        self.loadInstitutions();
        $("#HospitalsGroup").show();
        $("#RegionalsGroup").show();
    };

    self.loadInstitutions = function () {
        $.getJSON(app.dataModel.getInstitutionsUrl, { CountryID: self.selectedCountryId() === "" ? self.UsrCountry() : self.selectedCountryId(), RegionID: self.selectedRegionId() }, function (data, status) {
            self.institutions(data);
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
    };

       
    Sammy(function () {
        this.get('#home', function () {           
            // Make a call to the protected Web API by passing in a Bearer Authorization Header
            $.ajax({
                method: 'get',
                url: app.dataModel.userInfoUrl,
                contentType: "application/json; charset=utf-8",
                headers: {
                    'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
                },
                success: function (data) {
                    self.myHometown(data.hometown);
                }
            }); 
        });
       
        this.get('/', function () { this.app.runRoute('get', '#home') });
    });

    // Search case
    self.SearchFlucases = function () {
        if (typeof self.selectedInstitutionId() === 'undefined') {
            $("#caselist").jqGrid('clearGridData');
            $("#caselist_pend").jqGrid('clearGridData');
        }
        else {
            $("#caselist").trigger("reloadGrid");
            $("#caselist_pend").trigger("reloadGrid");
        }
    };

    self.FlowData = function () {
        //console.log("FlowData - frecord_flowMax -- " + app.Views.Contact.flow_max() + ", frecord_flowdata -- " + app.Views.Contact.flow_record() + ", finsti_flowdata -- " + app.Views.Contact.flow_institution() + ", dataStatement_flowdata -- " + app.Views.Contact.DataStatement() + ", userRole " + self.UserRole() + ", Inst" + $("#ITy").val());
        
        if (($("#ITy").val() != 2) && self.UserRole() == "adm") {
            console.log("aqui _ adm");
            $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', false);
            //self.ModDataNo();
            $("#tab-lab :input").prop('disabled', true);
        }
        //else if (($("#ITy").val() == "1" && self.UserRole() == "stf" && app.Views.Contact.flow_record() == app.Views.Contact.flow_max() && (app.Views.Contact.DataStatement() == 2 || app.Views.Contact.DataStatement() == null))) {
        //    console.log("aqui _ stf 1");
        //    $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', false);
        //    self.ModDataNo();
        //    $("#tab-lab :input").prop('disabled', true);
        //}   // Esto fue comentariado porque ya no necesitamos flujo en la parte epidemiologica, fue desactivado por pedido de RRR
        //else if (($("#ITy").val() == "1" && self.UserRole() == "stf" && app.Views.Contact.flow_record() == 0)) {  // Este cambio porque ya no necesitamos flujo en la parte epidemiologica por requerimiento de RRR
        else if (($("#ITy").val() == "1" && self.UserRole() == "stf" )) {
            console.log("aqui _ stf 2");
            $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', false);
            self.ModDataNo();
            $("#tab-lab :input").prop('disabled', true);
         } else if ($("#ITy").val() == "1" &&  app.Views.Contact.Flow_Local_Institution_Epi() == false)
         {
             console.log("aqui _ stf 3");
            $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', true);
            self.ModDataNo();
            $("#tab-lab :input").prop('disabled', true);
        }
    };

    self.FlowDataHospital = function () {
        console.log("FlowDataHospital");
            $("#div_hospital :input").prop('disabled', false);
    };

    self.FlowDataCaseStatus = function () {
        console.log("FlowCaseStatus");
        if (app.Views.Hospital.CaseStatus() == "3" && self.UserRole() != "adm") {
            console.log("aqui _ CaseStatus 3");
            $("#tabs :input").prop('disabled', true); // Modificacion para que se pueda modificar el registro aunque este cerrado el caso.
            self.ModDataNo();
            if (self.OpenAlwaysLab() == true) {
                $("#PrintM1").prop('disabled', false);
                $("#addLabTest_1").prop('disabled', false);
                $("#PrintM2").prop('disabled', false);
                $("#addLabTest_2").prop('disabled', false);
                $("#PrintM3").prop('disabled', false);
                $("#addLabTest_3").prop('disabled', false);
                
            }
            $("a[href*='tab-case']").show();
            $("#tab-case").show();
            $("#tabs").tabs("refresh");
        }
        else if ($("#ITy").val() == "1" && (app.Views.Hospital.IsSample() === "true" && app.Views.Lab.Processed() === "false")) {
            console.log("aqui _  Flow status no processed");
            $("a[href*='tab-case']").show();
            $("#tab-case").show();
            $("#tabs").tabs("refresh");
        }
        else if (app.Views.Contact.SurvSARI() == true && app.Views.Hospital.IsSample() === "true" && (app.Views.Lab.FinalResult() == "" || app.Views.Hospital.Destin() == "" || app.Views.Hospital.HospExDate() == "" || app.Views.Hospital.HospExDate() == null)) {
            console.log("aqui _ no processed 3");
            $("a[href*='tab-case']").hide();
            $("#tab-case").hide();
            $("#tabs").tabs("refresh");
        }
        else if ($("#ITy").val() == "1" && (app.Views.Hospital.CaseStatus() != "" && app.Views.Hospital.CaseStatus() != null)) {
            console.log("aqui _ CaseStatus");
            $("#tab-lab :input").prop('disabled', true);
            self.FlowDataHospital();
        }
    };

    self.FlowDataLab = function () {
        if ($("#ITy").val() == "2" && app.Views.Contact.Flow_Local_Institution_Lab() == true) {
            //console.log("aqui _ FlowDataLab");
            $("#tab-lab :input").prop('disabled', false);
        }
    };

    self.FlowDataLabAfter = function () {

        var flow_check = $.grep(app.Views.Lab.LabTests(), function (x) {         
            //console.log("EndFlow " + x.EndFlow());
            return x.EndFlow() === "TRUE";
        });


        if (($("#ITy").val() == "1" || $("#ITy").val() == "3") && flow_check.length > 0) {
            //console.log("DataLabAfter epi 1" + flow_check.length);
            $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', false);
            self.ModDataNo();
            $("#saveGeneral_1").show();
            $("#saveGeneral_2").show();
        }
        else if ($("#ITy").val() == "2" && (app.Views.Contact.flow_record() > app.Views.Contact.flow_institution()) && self.OpenAlwaysLab() == false) {
            //console.log("DataLabAfter epi 2" + flow_check.length);
            $("#tab-lab :input").prop('disabled', true);
        } else if ($("#ITy").val() == "2" && (app.Views.Contact.flow_record() == app.Views.Contact.flow_institution() && app.Views.Contact.DataStatement() == 2) && self.OpenAlwaysLab() == false) {
            //console.log("DataLabAfter epi 3" + flow_check.length);
            $("#tab-lab :input").prop('disabled', true);
        } else if ($("#ITy").val() == "1" || $("#ITy").val() == "3") {
            //console.log("DataLabAfter epi 4" + flow_check.length);
            $("#tab-lab :input").prop('disabled', true);
        }

        //console.log("aqui _ LabAfter");

        $("#PrintM1").prop('disabled', false);
        $("#PrintM2").prop('disabled', false);
        $("#PrintM3").prop('disabled', false);

    };


    self.ModDataNo = function () {
            $("#RegDate").attr('disabled', true);
            $("#FeverDate").attr('disabled', true);
            $("#HospitalDate").attr('disabled', true);
            $("#DocumentType").attr('disabled', true);
            $("#NoExpediente").attr('disabled', true);
            $("#Apellidos1").attr('disabled', true);
            $("#Nombres1").prop('disabled', true);
            $("#Apellidos1").attr('disabled', true);
            $("#DOB").prop('disabled', true);
            $("#Age").attr('disabled', true);
            $("#AMeasure").prop('disabled', true);
            $("input[id*='Sex']").prop('disabled', true);
            //$("input[id*='IsSample']").prop('disabled', true);
    };

    self.OpenAlwaysLab = function () {
        //console.log("Home OpenAlwaysLab - frecord_lab -- " + self.flow_record() + ", finstitution_lab -- " + self.flow_institution() + ", dataStatement_lab -- " + self.DataStatement() + ", userRole " + app.Views.Home.UserRole() + ", Inst" + $("#ITy").val() + ", OpenAlways" + self.flow_open_always());
        if (app.Views.Contact.flow_close_case() == 99 && app.Views.Contact.flow_open_always() == true) {
            return true;
        }
        else {
            return false;
        }
    };

    self.AddRecordHistoryItems = function (element, index, array) {
        //self.RecordHistoryItems.push(new RecordHistoryItems(element));
        self.RecordHistoryItems.push(element);

        //alert(JSON.stringify(element));
    };
   

    self.GetRecordHistory = function (id) {
        $.postJSON(app.dataModel.getRecordHistoryURL, {caseId: id})
            .success(function (data, textStatus, jqXHR) {
                self.RecordHistoryItems([]);
                //alert(JSON.stringify(data));
                data.forEach(self.AddRecordHistoryItems);
                //data.forEach(function (name) { alert(JSON.stringify(name)); });
                //console.log(data);
                //self.SummayItems([]);
                //data.forEach(self.AddSummayItem);                             

            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            })
        
    }

    return self;
}

app.addViewModel({
    name: "Home",
    bindingMemberName: "home",
    factory: HomeViewModel
});




