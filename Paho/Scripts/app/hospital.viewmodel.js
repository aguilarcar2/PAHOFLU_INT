function HospitalViewModel(app, dataModel) {
    var self = this;
    var date_format_ = app.dataModel.date_format_;
    var date_format_moment = app.dataModel.date_format_moment;
    var date_format_ISO = app.dataModel.date_format_ISO;
    var date_hospital = new Date();
    var date_fever = new Date();
    var date_diagnostic = new Date();
    var date_hosp_adm = new Date();
    var date_hosp_disc = new Date();
    var date_ICU_adm = new Date();
    var date_ICU_disc = new Date();
    var date_sample = new Date();
    var date_ship = new Date();
    var date_close_case = new Date();
    
    //alert(app.dataModel.date_format_);
    //alert(date_format_moment);

    self.Id = "";
    self.UsrCountry = ko.observable(app.Views.Home.UsrCountry()); // Pais del usuario logueado
    self.hasReset = ko.observable(false);
    self.CHNum = ko.observable("");
    self.Salon = ko.observable("");
    self.SalonVal = ko.observable("");
    //self.SalonVal.subscribe(function (prueba) {
    //    alert("valor " + prueba + " ; " + self.SalonVal());
    //    //console.log(prueba);
    //});

    //Agregar bolivia para la fecha de diagnostico
    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7 ) ? true : false;
        
    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;

    }, self);
   
    self.FeverDate = ko.observable("");
    self.FeverDate.subscribe(function (newFeverDate) {
        var current_value = typeof (newFeverDate) == "object" ? newFeverDate : parseDate(newFeverDate, date_format_);
        var date_hospital_ = typeof (app.Views.Contact.HospitalDate()) == "object" ? app.Views.Contact.HospitalDate() : parseDate(app.Views.Contact.HospitalDate(), date_format_);
        self.CalculateEW("FeverDate", self.FeverEW, self.FeverEY);

        if (self.hasReset() != true) {
            if (date_hospital_ == null || date_hospital_ == "") {
                alert("Por favor ingrese antes la fecha de notificación");
                self.FeverDate("");
            } else {
                if (moment(current_value).isAfter(moment(date_hospital_))) {
                    alert("La fecha de inicio de síntomas no puede ser mayor a la fecha de notificación");
                    self.FeverDate("");
                }
            }
        }

    });
    self.FeverEW = ko.observable("");
    self.FeverEY = ko.observable("");
    
    self.DiagPrinAdm = ko.observable("");
    self.DiagPrinAdmVal = ko.observable("");

    self.DiagOtroAdm = ko.observable("");

    self.DiagDate = ko.observable("");
    self.DiagDate.subscribe(function (newDiagDate) {
        self.CalculateEW("DiagDate", self.DiagEW, "");
    });
    self.DiagEW = ko.observable("");


    self.HospAmDate = ko.observable("");
    self.HospAmDate.subscribe(function (newHospAmDate) {
        self.CalculateEW("HospAmDate", self.HospEW, "");

        //if (self.UsrCountry() == 7 && self.UsrCountry() == 3) {
            var current_value = typeof (newHospAmDate) == "object" ? newHospAmDate : parseDate(newHospAmDate, date_format_);
            var date_hospital_ = typeof (app.Views.Contact.HospitalDate()) == "object" ? app.Views.Contact.HospitalDate() : parseDate(app.Views.Contact.HospitalDate(), date_format_);
            var date_fever_ = typeof (self.FeverDate()) == "object" ? self.FeverDate() : parseDate(self.FeverDate(), date_format_);
            var date_hosp_disc_ = typeof (self.HospExDate()) == "object" ? self.HospExDate() : parseDate(self.HospExDate(), date_format_);

            if (self.hasReset() != true) {
                if (date_hospital_ == null || date_hospital_ == "") {
                    alert("Por favor ingrese antes la fecha de notificación");
                    self.HospAmDate("");
                } else {
                    if (moment(current_value).isAfter(moment(date_hospital_))) {
                        alert("La fecha de Ingreso de Hospitalización no puede ser mayor a la fecha de notificación");
                        self.HospAmDate("");
                    }
                    if (current_value != null && date_fever_ != null && moment(current_value).isBefore(moment(date_fever_), "days")) {
                        alert("La fecha de hospitalización (ingreso) no puede ser menor que la fecha de inicio de síntomas");
                        self.HospAmDate("");
                    }
                }
            }
 //       }

    });

    self.HospEW = ko.observable("");
    self.HospExDate = ko.observable("");
    self.HospExDate.subscribe(function (newHospExDate) {

        //if (self.UsrCountry() == 7 && self.UsrCountry() == 3) {
            var current_value = typeof (newHospExDate) == "object" ? newHospExDate : parseDate(newHospExDate, date_format_);
            var date_hospital_ = typeof (self.HospAmDate()) == "object" ? self.HospAmDate() : parseDate(self.HospAmDate(), date_format_);

            if (current_value == null) self.Destin("");

            if (self.hasReset() != true) {
                if (date_hospital_ == null || date_hospital_ == "") {
                    alert("Por favor ingrese antes la fecha de admisión de hospitalización");
                    self.HospExDate("");
                } else {
                    if (moment(current_value).isBefore(moment(date_hospital_), "days")) {
                        alert("La fecha de Egreso de Hospitalización no puede ser menor a la fecha de Ingreso de hospitalización");
                        self.HospExDate("");
                    }
                }
            }
//        }
    });

    self.Destin = ko.observable("");
    self.FalleDate = ko.observable("");
    self.InstReferName = ko.observable("");

    self.ICU = ko.observable("");

    self.FalleDate.subscribe(function (newFalleDate) {

        var current_value = typeof (newFalleDate) == "object" ? newFalleDate : parseDate(newFalleDate, date_format_);
        var date_hospital_ = typeof (self.HospAmDate()) == "object" ? self.HospAmDate() : parseDate(self.HospAmDate(), date_format_);
        var date_hospital_ex = typeof (self.HospExDate()) == "object" ? self.HospExDate() : parseDate(self.HospExDate(), date_format_);
        var date_UCI_ = typeof (self.ICUAmDate()) == "object" ? self.ICUAmDate() : parseDate(self.ICUAmDate(), date_format_);
        var date_UCI_ex = typeof (self.ICUExDate()) == "object" ? self.ICUExDate() : parseDate(self.ICUExDate(), date_format_);

        if (self.hasReset() != true) {

            if (date_hospital_ == null || date_hospital_ == "") {
                alert("Por favor ingrese antes la admisión de hospitalización");
                self.FalleDate("");
            } else {
                if (moment(current_value).isBefore(moment(date_hospital_), "days")) {
                    alert("La fecha de Admisión de Hospitalización no puede ser mayor a la fecha de fallecimiento" );
                    self.FalleDate("");
                }
                if (date_UCI_ex != null && date_UCI_ex != "") {
                    if (moment(current_value).isBefore(moment(date_UCI_ex), "days")) {
                        alert("La fecha de Egreso de "+ (self.UsrCountry() == 7 ? " UPC " : " UCI ") +" no puede ser mayor a la fecha de fallecimiento ");
                        self.FalleDate("");
                    }
                }

            }
        }


    });

    self.ViewFallecido = ko.computed(function () {
        return (self.Destin() == 'D') ? true : false;
        
    }, self);
   
    self.ViewRefer = ko.computed(function () {
        return (self.Destin() == 'R') ? true : false;

    }, self);

    self.ICUAmDate = ko.observable("");
    self.ICUAmDate.subscribe(function (newICUAmDate) {
        self.CalculateEW("ICUAmDate", self.ICUEW, "");

 //       if (self.UsrCountry() == 7 && self.UsrCountry() == 3) {
            var current_value = typeof (newICUAmDate) == "object" ? newICUAmDate : parseDate(newICUAmDate, date_format_);
            var date_hospital_ = typeof (self.HospAmDate()) == "object" ? self.HospAmDate() : parseDate(self.HospAmDate(), date_format_);
            var date_hospital_ex = typeof (self.HospExDate()) == "object" ? self.HospExDate() : parseDate(self.HospExDate(), date_format_);

            if (self.hasReset() != true) {
                if (date_hospital_ == null || date_hospital_ == "") {
                    alert("Por favor ingrese antes la admisión de hospitalización");
                    self.ICUAmDate("");
                } else {
                    if (moment(current_value).isBefore(moment(date_hospital_), "days")) {
                        alert("La fecha de Admisión de Hospitalización no puede ser mayor a la fecha ingreso a" + (self.UsrCountry() == 7 ? " UPC" : " UCI"));
                        self.ICUAmDate("");
                    }
                    if (date_hospital_ex != null && date_hospital_ex != "") {
                        if (moment(current_value).isAfter(moment(date_hospital_ex), "days")) {
                            alert("La fecha de Egreso de Hospitalización no puede ser menor a la fecha ingreso a" + (self.UsrCountry() == 7 ? " UPC" : " UCI"));
                            self.ICUAmDate("");
                        }
                    }
                    
                }
            }
 //       }
    });

    self.ICUEW = ko.observable("");
    self.ICUExDate = ko.observable("");
    self.ICUExDate.subscribe(function (newICUAmDate) {
 //       if (self.UsrCountry() == 7 && self.UsrCountry() == 3) {
            var current_value = typeof (newICUAmDate) == "object" ? newICUAmDate : parseDate(newICUAmDate, date_format_);
            var date_hospital_ = typeof (self.HospAmDate()) == "object" ? self.HospAmDate() : parseDate(self.HospAmDate(), date_format_);
            var date_hospital_ex = typeof (self.HospExDate()) == "object" ? self.HospExDate() : parseDate(self.HospExDate(), date_format_);
            var date_icu_adm = typeof (self.ICUAmDate()) == "object" ? self.ICUAmDate() : parseDate(self.ICUAmDate(), date_format_);
            var date_death = typeof (self.FalleDate()) == "object" ? self.FalleDate() : parseDate(self.FalleDate(), date_format_);

            if (self.hasReset() != true) {
                if (date_hospital_ == null || date_hospital_ == "") {
                    alert("Por favor ingrese antes la admisión de hospitalización");
                    self.ICUExDate("");
                } else {
                    if (date_hospital_ == null || date_hospital_ == "") {
                        alert("Por favor ingrese antes la admisión de" + (self.UsrCountry() == 7 ? " UPC" : " UCI"));
                        self.ICUExDate("");
                    } else {
                        if (moment(current_value).isBefore(moment(date_hospital_), "days")) {
                            alert("La fecha de Admisión de Hospitalización no puede ser mayor a la fecha egreso de" + (self.UsrCountry() == 7 ? " UPC" : " UCI"));
                            self.ICUExDate("");
                        }
                        if (moment(current_value).isAfter(moment(date_hospital_ex), "days")) {
                            alert("La fecha de Egreso de Hospitalización no puede ser menor a la fecha egreso de" + (self.UsrCountry() == 7 ? " UPC" : " UCI"));
                            self.ICUExDate("");
                        }
                        if (moment(current_value).isBefore(moment(date_icu_adm), "days")) {
                            alert("La fecha de Admisión a UPC no puede ser menor a la fecha egreso de" + (self.UsrCountry() == 7 ? " UPC" : " UCI"));
                            self.ICUExDate("");
                        }
                        if (date_death != null || date_death != "") {
                            if (moment(current_value).isAfter(moment(date_death), "days")) {
                                alert("La fecha de fallecido no puede ser menor a la fecha egreso de" + ( self.UsrCountry() == 7 ? " UPC" : " UCI" ) );
                                self.ICUExDate("");
                            }

                        }

                    }
                    
                }
            }
//        }
    });
    self.EnableICUDate = ko.computed(function () {
        if (self.UsrCountry() != 7){
            return true;
        } else {
            if (self.ICU() == 1) {
                return true;
            } else {
                self.ICUAmDate("");
                self.ICUExDate("");
                return false;
            }
        }
    }, self);

    self.DestinICU = ko.observable("");

    self.HallRadio = ko.observable("");
    self.UCInt = ko.observable("");
    self.UCri = ko.observable("");
    self.MecVent = ko.observable("");
    self.MecVentNoInv = ko.observable("");
    self.ECMO = ko.observable("");
    self.VAFO = ko.observable("");
    self.DiagEg = ko.observable("");
    self.DiagEgVal = ko.observable("");

    //self.CheckFeverDate = function (date_node) {
    //    var date_hospital_ = app.Views.Contact.HospitalDate();
        
    //    console.log(self.propertyIsEnumerable(date_node).valueOf());
    //    console.log(self.date_node);
        
    //    if (app.Views.Contact.HospitalDate() == null || app.Views.Contact.HospitalDate() == "") {
    //        alert("Por favor ingrese la fecha de notificación");
    //        self.FeverDate(null);
    //    } else {

    //        if (moment(self.FeverDate()).isAfter(moment(date_hospital_))) {
    //            alert("La fecha de inicio de síntomas no puede ser mayor a la fecha de notificación");
    //            self.FeverDate(null);
    //        }
                
    //    }
    //    self.FeverDate(moment(self.FeverDate()).format(date_format_moment));
    //};

    self.CheckDiagDate = function () {
        if (self.FeverDate() == null || self.FeverDate() == "") {
            alert("Por favor ingrese la fecha de inicio de síntomas");
            self.DiagDate(null);
        } else {

            if (moment(self.DiagDate()).isBefore(moment(self.FeverDate()))) {
                alert("La fecha de diagnóstico no puede ser menor a la fecha de inicio de síntomas");
                self.DiagDate(null);
            }

        }
    };

 

    self.CalculateEW = function (FieldDate, FieldAct, FieldActYear) {
        if ($("#" + FieldDate).val() != "") {
            var date_ew = parseDate($("#" + FieldDate).val(), date_format_);
            var fwky_date = new Date(moment(date_ew).year(), 0, 1).getDay();
            var weekno = moment(date_ew).week();
            var weeknoISO = moment(date_ew).isoWeek();

            if (fwky_date > 3) {
                var month = 11, day = 31;
                var end_date_year_ant = new Date(moment(date_ew).year() - 1, month, day--);

                if (weekno == 1 && moment(date_ew).month() == 0)
                {
                    var fwky_date_ant = new Date(moment(date_ew).year() - 1, 0, 1).getDay();
                    var fwdoyant = moment(end_date_year_ant).isoWeek();
                    if (fwky_date_ant > 3) {
                        
                            FieldAct(fwdoyant - 1);

                    } else {

                        if (weekno == 1 && moment(date_ew).month() == 0 && fwky_date_ant <= 3) {
                            FieldAct(53);
                            fwdoyant = 53;
                        }
                        else
                            FieldAct(fwdoyant);
                    }
                    if (FieldActYear != "")
                        if (fwdoyant == 52 || fwdoyant == 53)
                            FieldActYear(date_ew.getFullYear() - 1);
                        else
                            FieldActYear(date_ew.getFullYear());
                }
                else if (weekno == 1 && moment(date_ew).month() != 0) {
                    FieldAct(moment(date_ew).isoWeek() - 1);
                    if (FieldActYear != "")
                        FieldActYear(date_ew.getFullYear());
                }
                else
                {
                    FieldAct(weekno - 1);
                    if (FieldActYear != "")
                        FieldActYear(date_ew.getFullYear());
                }  
            } else {
                if (weekno == 1 && moment(date_ew).month() == 11) {
                    var fwky_date_prox = new Date(moment(date_ew).year() + 1, 0, 1).getDay();

                    if (fwky_date_prox > 3)
                    {
                        FieldAct(53);
                        FieldActYear(date_ew.getFullYear());
                    } else
                    {
                        FieldAct(weekno);
                        FieldActYear(date_ew.getFullYear() + 1);
                    }

                } else
                {
                    FieldAct(weekno);
                    if (FieldActYear != "")
                        FieldActYear(date_ew.getFullYear());
                }                          
            }

        }
    };

    
    self.IsSample = ko.observable("");
    $("#NotSample :input, #tab-lab :input").prop('disabled', true);
    self.IsSample.subscribe(function (NewIsSample) {
        if (self.IsSample() === "true") {
            $("a[href*='tab-lab']").show();
            $("#tab-lab").show();
            $("#tabs").tabs("refresh");
            if ($("#ITy").val() == 1)
            {
                $("#tab-lab :input").prop('disabled', true);
                $("#NotSample :input").prop('disabled', false);
                
            } else {
                //$("#NotSample :input, #tab-lab :input").prop('disabled', false);
                if (app.Views.Hospital.CaseStatus == "3") {
                    $("#tabs :input").prop('disabled', true);
                } else {
                    $("#tab-lab :input").prop('disabled', false);
                    $("#NotSample :input").prop('disabled', true);
                }
                
            }

            if (app.Views.Contact.SurvILI() == true && app.Views.Lab.FinalResult()) {
                $("#CaseStatus").attr("disabled", false);
            }
        }
        else {
            $("a[href*='tab-lab']").hide();
            $("#tab-lab").hide();
            $("#NotSample :input, #tab-lab :input").prop('disabled', true);
            self.SampleDate(null);
            self.SampleType(null);
            self.ShipDate(null);
            self.LabId(null);
            self.SampleDate2(null);
            self.SampleType2(null);
            self.ShipDate2(null);
            self.LabId2(null);
            self.SampleDate3(null);
            self.SampleType3(null);
            self.ShipDate3(null);
            self.LabId3(null);

            if (self.Destin() != "" || app.Views.Contact.SurvILI() == true) {
                $("#CaseStatus").attr("disabled", false);
            } else {
                if (app.Views.Contact.IsSurv() == 2) {
                    $("#CaseStatus").attr("disabled", false);
                } else {
                    $("#CaseStatus").attr("disabled", true);
                }
            }
        }
    });


    self.CaseIsSample = ko.computed(function () {
        return (app.Views.Contact.ActiveBOL) ? (self.IsSample() === "true") ? true : false : true;
    });

    self.CaseIsNoSample = ko.computed(function () {
        return (app.Views.Contact.ActiveBOL) ? (self.IsSample() === "false") ? true : false : true;
    });

    self.SampleDate = ko.observable("");
    self.SampleDate.subscribe(function (newSampleDate) {
        var current_value = typeof (newSampleDate) == "object" ? newSampleDate : parseDate(newSampleDate, date_format_);
        var date_fever_ = typeof (self.FeverDate()) == "object" ? self.FeverDate() : parseDate(self.FeverDate(), date_format_);


        if (self.hasReset() != true) {
            if (date_fever_ == null || date_fever_ == "") {
                alert("Por favor ingrese antes la fecha de inicio de síntomas");
                self.SampleDate("");
            } else {
                if (moment(current_value).diff(date_fever_, 'days', false) < 0) {
                    alert("La fecha de toma de muestra no puede ser menor a la fecha de inicio de síntomas");
                    self.SampleDate("");
                }
            }
        }

    });
    self.SampleType = ko.observable("");
    self.ShipDate = ko.observable("");
    self.labs = ko.computed(function () {
          return app.Views.Home.labs();       
    }, self);
    self.LabsHospital = ko.observableArray();

    self.LabId = ko.observable("");

    self.SampleDate2 = ko.observable("");
    if (self.UsrCountry() == 7) {
        self.SampleDate2.subscribe(function (newSampleDate2) {
            var current_value = typeof (newSampleDate2) == "object" ? newSampleDate2 : parseDate(newSampleDate2, date_format_);
            var date_sampledate_ = typeof (self.SampleDate()) == "object" ? self.SampleDate() : parseDate(self.SampleDate(), date_format_);
            var date_fever_ = typeof (self.FeverDate()) == "object" ? self.FeverDate() : parseDate(self.FeverDate(), date_format_);


            if (self.hasReset() != true && newSampleDate2 != "") {
                if (date_fever_ == null || date_fever_ == "") {
                    alert("Por favor ingrese antes la fecha de inicio de síntomas");
                    self.SampleDate2("");
                } else {
                    if (moment(current_value).isBefore(moment(date_fever_))) {
                        alert("La fecha de toma de muestra no puede ser menor a la fecha de inicio de síntomas");
                        self.SampleDate2("");
                    }
                    if (moment(current_value).diff(date_sampledate_, 'days', false) < 0) {
                        alert("La fecha de toma de muestra 2 no puede ser menor de la fecha de toma de muestra 1 ");
                        self.SampleDate2("");
                    }

                    if (moment(current_value).diff(date_fever_, 'days', false) > 15) {
                        alert("La fecha de toma de muestra 2 no puede ser de más de 15 días de diferencia con la fecha de inicio de síntomas ");
                        self.SampleDate2("");
                    }

                }
            }

        });
    }
    
    self.SampleType2 = ko.observable("");
    self.ShipDate2 = ko.observable("");
    self.LabId2 = ko.observable("");

    self.SampleDate3 = ko.observable("");
    if (self.UsrCountry() == 7) {
        self.SampleDate3.subscribe(function (newSampleDate3) {
            var current_value = typeof (newSampleDate3) == "object" ? newSampleDate3 : parseDate(newSampleDate3, date_format_);
            var date_sampledate_ = typeof (self.SampleDate()) == "object" ? self.SampleDate() : parseDate(self.SampleDate(), date_format_);
            var date_sampledate2_ = typeof (self.SampleDate2()) == "object" ? self.SampleDate2() : parseDate(self.SampleDate2(), date_format_);
            var date_fever_ = typeof (self.FeverDate()) == "object" ? self.FeverDate() : parseDate(self.FeverDate(), date_format_);


            if (self.hasReset() != true && newSampleDate3 != "") {
                if (date_fever_ == null || date_fever_ == "") {
                    alert("Por favor ingrese antes la fecha de inicio de síntomas");
                    self.SampleDate3("");
                } else {
                    if (moment(current_value).isBefore(moment(date_fever_))) {
                        alert("La fecha de toma de muestra no puede ser menor a la fecha de inicio de síntomas");
                        self.SampleDate3("");
                    }
                    if (moment(current_value).diff(date_sampledate_, 'days', false) < 0) {
                        alert("La fecha de toma de muestra 3 no puede ser menor de la fecha de toma de muestra 1 ");
                        self.SampleDate3("");
                    }
                    if (moment(current_value).diff(date_sampledate2_, 'days', false) < 0) {
                        alert("La fecha de toma de muestra 3 no puede ser menor de la fecha de toma de muestra 2 ");
                        self.SampleDate3("");
                    }

                    if (moment(current_value).diff(date_fever_, 'days', false) > 15) {
                        alert("La fecha de toma de muestra 3 no puede ser de más de 15 días de diferencia con la fecha de inicio de síntomas ");
                        self.SampleDate3("");
                    }



                }
            }

        });
    }
    self.SampleType3 = ko.observable("");
    self.ShipDate3 = ko.observable("");
    self.LabId3 = ko.observable("");

    self.VisibleMuestra2 = ko.computed(function () {
        return (self.UsrCountry() == 7 && self.SampleDate() && app.Views.Contact.IsSurv() == 1) ? true : false;

    }, self);
    self.VisibleMuestra3 = ko.computed(function () {
        return (self.UsrCountry() == 7 && self.SampleDate2() && app.Views.Contact.IsSurv() == 1) ? true : false;

    }, self);

    self.Adenopatia = ko.observable("");
    self.AntecedentesFiebre = ko.observable("");
    self.Rinorrea = ko.observable("");
    self.Malestar = ko.observable("");
    self.Nauseas = ko.observable("");
    self.DolorMuscular = ko.observable("");
    self.Disnea = ko.observable("");
    self.DolorCabeza = ko.observable("");
    self.Estridor = ko.observable("");
    self.Tos = ko.observable("");
    self.Tiraje = ko.observable("");
    self.Odinofagia = ko.observable("");
    self.DifResp = ko.observable("");
    self.MedSatOxig = ko.observable("");
    self.SatOxigPor = ko.observable("");
    self.CaseStatus = ko.observable("");
    self.CaseComply = ko.observable("");
    self.CloseDate = ko.observable(null);
    self.ObservationCase = ko.observable("");
    self.EnableCloseDate = ko.computed(function () {
        var result = self.CaseStatus() == "3" || self.CaseStatus() == "2"; if (!result) self.CloseDate(""); return result;
    }, self);

    self.Destin.subscribe(function (NewDestin) {

        if (app.Views.Contact.SurvSARI() == true && app.Views.Contact.IsSurv() != "" && NewDestin != null && NewDestin != "") {
            if (self.HospExDate() == "" || self.HospExDate() == "undefined" || self.HospExDate() == null)
            {
                alert("Es necesario ingresar antes la 'Fecha de Egreso' para poder ingresar la 'Condición de egreso' ");
                self.Destin("");
                $("#HospExDate").focus();
             }
            if (NewDestin != "" && self.IsSample() === "false") {
                $("a[href*='tab-case']").show();
                $("#tab-case").show();
                $("#CaseStatus").attr("disabled", false);
                $("#tabs").tabs("refresh");
            } else if (NewDestin != "" && self.IsSample() === "true" && app.Views.Lab.FinalResult() != "" && typeof app.Views.Lab.FinalResult() != "undefined") {
                $("a[href*='tab-case']").show();
                $("#tab-case").show();
                $("#CaseStatus").attr("disabled", false);
                $("#tabs").tabs("refresh");
            } else if (self.IsSample() === "true" && app.Views.Lab.FinalResult() != "" && typeof app.Views.Lab.FinalResult() != "undefined") {
                $("a[href*='tab-case']").show();
                $("#tab-case").show();
                $("#tabs").tabs("refresh");
            } else if (self.IsSample() === "true" && app.Views.Lab.Processed() === "false" ) {
                    $("a[href*='tab-case']").show();
                    $("#tab-case").show();
                    $("#tabs").tabs("refresh");
            } else if (app.Views.Contact.Id() != null) {
                $("a[href*='tab-case']").hide();
                $("#tab-case").hide();
                $("#CaseStatus").attr("disabled", true);
            }
            //$("#tabs").tabs("refresh");

            }
      });

    self.CaseStatus.subscribe(function (NewDestin) {

        //if (NewDestin != 3) {
        //    $("#saveGeneral_1").attr("disabled", false);
        //    $("#saveGeneral_1").toggle(true);
        //}
    });

    self.NotShowSatOxigPor = ko.computed(function () {
        if (self.MedSatOxig() == "1") {
            return true;
        } else {
            self.SatOxigPor("");
            return false;
        }
    }, self);

    self.ResetHospital = function () {
       self.hasReset(true);
       self.Id = "";
       self.CHNum("");
       self.Salon("");
       self.SalonVal("");
       self.DiagPrinAdm("");
       self.DiagPrinAdmVal(""); 
       self.DiagOtroAdm("");
       self.FeverDate(null);
       self.DiagDate(null);
       self.HospAmDate(null);
       self.HospExDate(null);
       self.ICUAmDate(null);
       self.ICUExDate(null);
       self.FalleDate(null);
       self.InstReferName("");
       self.Destin(null);
       self.Destin("");
       self.DestinICU("");
       self.HallRadio("");
       self.UCInt("");
       self.UCri("");
       self.MecVent("");
       self.MecVentNoInv("");
       self.ECMO("");
       self.VAFO("");
       self.DiagEg("");
       self.DiagEgVal("");
       self.IsSample("");
       self.SampleDate (null);
       self.SampleType("");
       self.ShipDate(null);
       self.LabId("");
       self.SampleDate2(null);
       self.SampleType2("");
       self.ShipDate2(null);
       self.LabId2("");
       self.SampleDate3(null);
       self.SampleType3("");
       self.ShipDate3(null);
       self.LabId3("");
       self.Adenopatia("");
       self.AntecedentesFiebre("");
       self.Rinorrea("");
       self.Malestar("");
       self.Nauseas("");
       self.DolorMuscular("");
       self.Disnea("");
       self.DolorCabeza("");
       self.Estridor("");
       self.Tos("");
       self.DifResp("");
       self.MedSatOxig("");
       self.SatOxigPor("");
       self.Tiraje("");
       self.Odinofagia("");
       self.DifResp("");
       self.MedSatOxig("");
       self.SatOxigPor("");
       self.CaseStatus("");
       self.CaseComply("");
       self.CloseDate(null);
       self.ObservationCase("");
       self.hasReset(false);
    };

    self.validate = function (nextStep) {
        var msg = "";
        date_notification = parseDate($("#HospDate").val(), date_format_);
        date_fever = parseDate($("#FeverDate").val(), date_format_);
        date_diagnostic = parseDate($("#DiagDate").val(), date_format_);
        date_hosp_adm =  parseDate($("#HospAmDate").val(), date_format_);
        date_hosp_disc =   parseDate($("#HospExDate").val(), date_format_);
        date_ICU_adm =  parseDate($("#ICUAmDate").val(), date_format_);
        date_ICU_disc = parseDate($("#ICUExDate").val(), date_format_);
        date_falle = parseDate($("#FalleDate").val(), date_format_);
        date_hospital = parseDate($("#HospDate").val(), date_format_);
        date_sample =  parseDate($("#SampleDate").val(), date_format_);
        date_ship = parseDate($("#ShipDate").val(), date_format_);
        date_close_case = parseDate($("#CloseDate").val(), date_format_);
        

        if ($("#FeverDate").val() == "")
            msg += "\n" + msgValidationOnsetOnFever;
        if ($("#FeverDate").val() != "" && !moment(moment(date_fever).format(date_format_moment), [date_format_moment], true).isValid())
            msg += "\n" + "Fecha de inicio de síntomas es inválida";
        if (date_notification != null && date_fever != null && moment(date_fever).isAfter(moment(date_notification), "days")) {
            msg += "\n" + "La fecha de inicio de síntomas no puede ser mayor a la fecha de notificación";
            $("#FeverDate").focus();
        }
            

        if (date_diagnostic != null && date_fever != null && moment(date_diagnostic).isBefore(moment(date_fever))) {
            msg += "\n" + "La fecha de diagnóstico no puede ser menor a la fecha de inicio de síntomas";
            $("#DiagDate").focus();
        }
            
        if (app.Views.Contact.NoActiveBOL == true) {
            if (!self.Adenopatia() && !self.AntecedentesFiebre() && !self.Rinorrea() && !self.Malestar() && !self.Nauseas() && !self.DolorMuscular() && !self.Disnea() && !self.DolorCabeza() && !self.Estridor() && !self.Tos() && !self.Tiraje() && !self.Odinofagia()) {
                msg += "\n" + "Seleccione por lo menos un síntoma";
            }
        }
        
        if (app.Views.Contact.SurvSARI() == true) {
            if ($("#HospAmDate").val() == "")
                msg += "\n" + "Fecha de hospitalización es requerida";
            if ($("#HospAmDate").val() != "" && !moment(moment(date_hosp_adm).format(date_format_moment), [date_format_moment], true).isValid())
                msg += "\n" + "Fecha de hospitalización es inválida";
            if (date_hosp_adm != null && date_fever != null && moment(date_hosp_adm).isBefore(moment(date_fever), "days")) {
                msg += "\n" + "La fecha de hospitalización (ingreso) no puede ser menor que la fecha de inicio de síntomas";
                $("#HospAmDate").focus();
            }

            if (date_hosp_adm != null && date_hosp_disc != null && moment(date_hosp_disc).isBefore(moment(date_hosp_adm), "days")) {
                msg += "\n" + "La fecha de hospitalización (egreso) no puede ser menor que la fecha de hospitalización(ingreso)";
                $("#HospExDate").focus();
            }
        }

        // Validacion de fecha de fallecido

        if (self.Destin() == "D") {
            if ($("#FalleDate").val() == "")
                msg += "\n" + "Fecha de fallecido es requerida";
            if ($("#FalleDate").val() != "" && !moment(moment(date_falle).format(date_format_moment), [date_format_moment], true).isValid())
                msg += "\n" + "Fecha de fallecido es inválida";
            if (date_falle != null && date_fever != null && moment(date_falle).isBefore(moment(date_fever), "days")) {
                msg += "\n" + "La fecha de fallecido no puede ser menor que la fecha de inicio de síntomas";
                $("#FalleDate").focus();
            }

            if (date_falle != null && date_hosp_adm != null && moment(date_falle).isBefore(moment(date_hosp_adm), "days")) {
                msg += "\n" + "La fecha de fallecido no puede ser menor que la fecha de hospitalización(ingreso)";
                $("#FalleDate").focus();
            }
        }
        
         
        // Validaciones de Chile

        if (app.Views.Contact.SurvSARICHI() == true) {
            if (!self.ICU() || self.ICU() == "")
                msg += "\n" + "Información de UPC es requerido";
        }

//        if (!self.ICUAmDate())
//            msg += "\n" + "Fecha de UCI es requerida";
//        if (self.ICUAmDate() && !moment(moment(self.ICUAmDate()).format("MM/DD/YYYY"), ["MM/DD/YYYY"], true).isValid())
//            msg += "\n" + "Fecha de UCI es invalida";
//        if (self.HospAmDate() && self.ICUAmDate() && moment(self.HospAmDate()).isBefore(moment(self.ICUAmDate())))
//           msg += "\n" + "Fecha de UCI no puede ser posterior a la de hospitalización";

        if (!self.IsSample())
            msg += "\n" + "Información de la muestra es requerida";

        if (self.IsSample() == "true") {
            if ($("#SampleDate").val() == "")
                msg += "\n" + "Fecha de toma de muestra es requerida";
            if ($("#SampleDate").val() != "" && !moment(moment(date_sample).format(date_format_moment), [date_format_moment], true).isValid())
                msg += "\n" + "Fecha de toma de muestra es inválida";
            if (date_sample != null && date_fever != null && moment(date_sample).isBefore(moment(date_fever), "days")) {
                msg += "\n" + "Fecha de toma de muestra no puede ser anterior a la fecha de inicio de síntomas";
                $("#SampleDate").focus()
            }
                

            if (date_sample != null && date_ship != null && moment(date_ship).isBefore(moment(date_sample), "days")) {
                msg += "\n" + "Fecha de envío de muestra no puede ser anterior a la fecha de toma de muestra";
                $("#ShipDate").focus()
            }
                
            //if (self.HospAmDate() && self.SampleDate() && moment(self.HospAmDate()).isBefore(moment(self.SampleDate())))
            //    msg += "\n" + "Fecha de toma de muestra no puede ser posterior a la de hospitalización";
            if (!self.SampleType() || self.SampleType() == "")
                msg += "\n" + "Tipo de muestra es requerido";

            //if (self.EnableCloseDate()) {
            //    if ($("#CloseDate").val() == "")
            //        msg += "\n" + "Fecha de cierre de caso es requerida";
            //    if ($("#CloseDate").val() != "" && !moment(moment(date_close_case).format(date_format_moment), [date_format_moment], true).isValid())
            //        msg += "\n" + "Fecha de cierre de caso es inválida";

            //    if (app.Views.Lab.EndLabDate() && self.CloseDate() && moment(app.Views.Lab.EndLabDate()).isAfter(moment(self.CloseDate())))
            //        msg += "\n" + "Fecha de cierre no puede ser menor a la de laboratorio";
            //}

        }

        if (msg !== "") { alert(msgValidationClinicalInfo + msg); $('#tabs').tabs({ active: 3 }); return false; }
        if (nextStep != null) nextStep();
        return true;
    };

    self.GetHospital = function (id) {
        self.Id = id;
        $.getJSON(app.dataModel.getHospitalUrl, { id: id, institutionId: app.Views.Home.selectedInstitutionId() }, function (data, status) {
                self.CHNum(data.CHNum);
                self.hasReset(true);
                if (data.FeverDate)
                    self.FeverDate(moment(data.FeverDate).format(date_format_moment));
                else self.FeverDate(null);
                self.CalculateEW("FeverDate", self.FeverEW, self.FeverEY);
                if (data.DiagDate)
                    self.DiagDate(moment(data.DiagDate).format(date_format_moment));
                else self.DiagDate(null);
                self.CalculateEW("DiagDate", self.DiagEW, "");
                if (data.HospAmDate)
                    self.HospAmDate(moment(data.HospAmDate).format(date_format_moment));
                else self.HospAmDate(null)
                self.CalculateEW("HospAmDate", self.HospEW, "");
                if (data.HospExDate)
                    self.HospExDate(moment(data.HospExDate).format(date_format_moment));
                else self.HospExDate(null)
                self.ICU(data.ICU);
                if (data.ICUAmDate)
                    self.ICUAmDate(moment(data.ICUAmDate).format(date_format_moment));
                else self.ICUAmDate(null)
                if (data.ICUAmDate)
                    self.CalculateEW("ICUAmDate", self.ICUEW, "");
                if (data.ICUExDate)
                    self.ICUExDate(moment(data.ICUExDate).format(date_format_moment));
                else self.ICUExDate(null);
                self.Destin(data.Destin);
                self.DestinICU(data.DestinICU);
                self.InstReferName(data.InstReferName);
                if (data.FalleDate)
                    self.FalleDate(moment(data.FalleDate).format(date_format_moment));
                else self.FalleDate(null);
                self.SalonVal(data.SalonVal);
                if (data.SalonVal != "" && data.SalonVal != null) {
                    $.getJSON("/cases/GetSalonID", { ID: data.SalonVal }, function (data, status) {
                        self.Salon(data.label);
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        console.log(errorThrown);
                    });
                }
                else self.Salon("");

                self.DiagPrinAdmVal(data.DiagPrinAdmVal);
                if (data.DiagPrinAdmVal != "" && data.DiagPrinAdmVal != null) {
                    $.getJSON("/cases/GetCIE10ID", { ID: data.DiagPrinAdmVal }, function (data, status) {
                        self.DiagPrinAdm(data.label);
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        console.log(errorThrown);
                    });
                }
                else self.DiagPrinAdm("");

                self.DiagOtroAdm(data.DiagOtroAdm);

                
                if (data.SampleDate)
                    self.SampleDate(moment(data.SampleDate).format(date_format_moment));
                else self.SampleDate(null)
                self.SampleType(data.SampleType);
                if (data.ShipDate)
                    self.ShipDate(moment(data.ShipDate).format(date_format_moment));
                else self.ShipDate(null);
                self.LabId(data.LabId);
            //Segunda muestra
                if (data.SampleDate2)
                    self.SampleDate2(moment(data.SampleDate2).format(date_format_moment));
                else self.SampleDate2(null)
                self.SampleType2(data.SampleType2);
                if (data.ShipDate2)
                    self.ShipDate2(moment(data.ShipDate2).format(date_format_moment));
                else self.ShipDate2(null);
                self.LabId2(data.LabId2);
            //Tercera muestra
                if (data.SampleDate3)
                    self.SampleDate3(moment(data.SampleDate3).format(date_format_moment));
                else self.SampleDate3(null)
                self.SampleType3(data.SampleType3);
                if (data.ShipDate3)
                    self.ShipDate3(moment(data.ShipDate3).format(date_format_moment));
                else self.ShipDate3(null);
                self.LabId3(data.LabId3);

               if (data.IsSample == true){
                self.IsSample("true");
               }else if (data.IsSample == false){
                   self.IsSample("false");
               } else self.IsSample("");

                self.Adenopatia(data.Adenopatia);
                self.AntecedentesFiebre(data.AntecedentesFiebre);
                self.Rinorrea(data.Rinorrea);
                self.Malestar(data.Malestar);
                self.Nauseas(data.Nauseas);
                self.DolorMuscular(data.DolorMuscular);
                self.Disnea(data.Disnea);
                self.DolorCabeza(data.DolorCabeza);
                self.Estridor(data.Estridor);
                self.Tos(data.Tos);
                self.DifResp(data.DifResp);
                self.MedSatOxig(data.MedSatOxig);
                self.SatOxigPor(data.SatOxigPor);
                self.HallRadio(data.HallRadio);
                self.UCInt(data.UCInt);
                self.UCri(data.UCri);
                self.MecVent(data.MecVent);
                self.MecVentNoInv(data.MecVentNoInv);
                self.ECMO(data.ECMO);
                self.VAFO(data.VAFO);
                self.DiagEgVal(data.DiagEgVal);
                if (data.DiagEgVal != "" && data.DiagEgVal != null) {
                    $.getJSON("/cases/GetCIE10ID", { ID: data.DiagEgVal }, function (data, status) {
                        self.DiagEg(data.label);
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        console.log(errorThrown);
                        self.DiagEg("");
                    });
                }
                else  self.DiagEg("");
                self.Tiraje(data.Tiraje);
                self.Odinofagia(data.Odinofagia);
                self.CaseStatus(data.CaseStatus);
                if (data.CloseDate)
                    self.CloseDate(moment(data.CloseDate).format(date_format_moment));
                self.ObservationCase( data.ObservationCase  != null ? data.ObservationCase : "" );
                self.hasReset(false);
                self.LabsHospital(data.LabsHospital);

            })
            .fail(function(jqXHR, textStatus, errorThrown) {
                console.log(errorThrown);
            });
    };
    
    self.Save = function () {
        app.Views.Home.ValidateAll();
    };

    self.Cancel = function () {
        if (confirm("Usted esta apunto de salir del registro, está seguro?")) {
            app.Views.Home.CancelEdit();
        }
    };

    self.SaveHospital = function (nextStep) {
        date_fever = parseDate($("#FeverDate").val(), date_format_);
        date_diagnostic = parseDate($("#DiagDate").val(), date_format_);
        date_hosp_adm = parseDate($("#HospAmDate").val(), date_format_);
        date_hosp_disc = parseDate($("#HospExDate").val(), date_format_);
        date_ICU_adm = parseDate($("#ICUAmDate").val(), date_format_);
        date_ICU_disc = parseDate($("#ICUExDate").val(), date_format_);
        date_falle = parseDate($("#FalleDate").val(), date_format_);
        date_sample = parseDate($("#SampleDate").val(), date_format_);
        date_ship = parseDate($("#ShipDate").val(), date_format_);
        date_sample2 = parseDate($("#SampleDate2").val(), date_format_);
        date_ship2 = parseDate($("#ShipDate2").val(), date_format_);
        date_sample3 = parseDate($("#SampleDate3").val(), date_format_);
        date_ship3 = parseDate($("#ShipDate3").val(), date_format_);
        date_close_case = parseDate($("#CloseDate").val(), date_format_);
        
        // falta la fecha de cierre de caso

        $.post(app.dataModel.saveHospitalUrl,
            {
                id: self.Id,
                CHNum: self.CHNum(),
                FeverDate: moment(date_fever).format(date_format_ISO),
                FeverEW: self.FeverEW(),
                FeverEY: self.FeverEY(),
                DiagDate: moment(date_diagnostic).format(date_format_ISO),
                DiagEW: self.DiagEW(),
                HospAmDate: moment(date_hosp_adm).format(date_format_ISO),
                HospEW: self.HospEW(),
                HospExDate: $("#HospExDate").val() == "" ? null : moment(date_hosp_disc).format(date_format_ISO),
                ICU: self.ICU(),
                ICUAmDate: $("#ICUAmDate").val() == "" ? null : moment(date_ICU_adm).format(date_format_ISO),
                ICUEW: self.ICUEW(),
                ICUExDate: $("#ICUExDate").val() == "" ? null : moment(date_ICU_disc).format(date_format_ISO),
                Destin: self.Destin(),
                FalleDate: $("#FalleDate").val() == "" ? null : moment(date_falle).format(date_format_ISO),
                InstReferName: self.InstReferName(),
                IsSample: self.IsSample() == "true" ? true : (self.IsSample() == "false" ? false : null),
                SampleDate: $("#SampleDate").val() == "" ? null : moment(date_sample).format(date_format_ISO),
                SampleType: self.SampleType(),
                ShipDate: $("#ShipDate").val() == "" ? null : moment(date_ship).format(date_format_ISO),
                LabId: self.LabId(),
                SampleDate2: $("#SampleDate2").val() == "" ? null : moment(date_sample2).format(date_format_ISO),
                SampleType2: self.SampleType2(),
                ShipDate2: $("#ShipDate2").val() == "" ? null : moment(date_ship2).format(date_format_ISO),
                LabId2: self.LabId2(),
                SampleDate3: $("#SampleDate3").val() == "" ? null : moment(date_sample3).format(date_format_ISO),
                SampleType3: self.SampleType3(),
                ShipDate3: $("#ShipDate3").val() == "" ? null : moment(date_ship3).format(date_format_ISO),
                LabId3: self.LabId3(),
                Adenopatia: self.Adenopatia(),
                AntecedentesFiebre: self.AntecedentesFiebre(),
                Rinorrea: self.Rinorrea(),
                Malestar: self.Malestar(),
                Nauseas: self.Nauseas(),
                DolorMuscular: self.DolorMuscular(),
                Disnea: self.Disnea(),
                DolorCabeza: self.DolorCabeza(),
                Estridor: self.Estridor(),
                Tos: self.Tos(),
                DifResp: self.DifResp(),
                MedSatOxig: self.MedSatOxig(),
                SatOxigPor: self.SatOxigPor(),
                SalonVal: self.SalonVal(),
                DiagPrinAdmVal: self.DiagPrinAdmVal(),
                DiagOtroAdm: self.DiagOtroAdm() == null ? "" : self.DiagOtroAdm().toLocaleUpperCase(),
                DestinICU: self.DestinICU(),
                HallRadio: self.HallRadio(),
                UCInt: self.UCInt(),
                UCri: self.UCri(),
                MecVent: self.MecVent(),
                MecVentNoInv: self.MecVentNoInv(),
                ECMO: self.ECMO(),
                VAFO: self.VAFO(),
                DiagEgVal: self.DiagEgVal(),
                Tiraje: self.Tiraje(),
                Odinofagia: self.Odinofagia(),
                CaseStatus: self.CaseStatus(),
                CloseDate: $("#CloseDate").val() == "" ? null : moment(date_close_case).format(date_format_ISO),
                ObservationCase: self.ObservationCase() == null ? "" : self.ObservationCase().toLocaleUpperCase(),
                DataStatement: 2
                
            },
            function (data) {
                if (nextStep) nextStep();
                //alert(data);
            },
            "json"
        );
        return true;
    };

    return self;

};

app.addViewModel({
    name: "Hospital",
    bindingMemberName: "hospital",
    factory: HospitalViewModel
});