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

    self.CalculateEW = function (FieldDate, FieldAct, FieldActYear) {
        if ($("#" + FieldDate).val() != "") {
            var date_ew = parseDate($("#" + FieldDate).val(), date_format_);
            var fwky_date = new Date(moment(date_ew).year(), 0, 1).getDay();
            var weekno = moment(date_ew).week();
            var weeknoISO = moment(date_ew).isoWeek();

            if (date_ew == null) {

                FieldAct(null);
                FieldActYear(null);

            } else {

                if (fwky_date > 3) {
                    var month = 11, day = 31;
                    var end_date_year_ant = new Date(moment(date_ew).year() - 1, month, day--);

                    if (weekno == 1 && moment(date_ew).month() == 0) {
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
                    else {
                        FieldAct(weekno - 1);
                        if (FieldActYear != "")
                            FieldActYear(date_ew.getFullYear());
                    }
                } else {
                    if (weekno == 1 && moment(date_ew).month() == 11) {
                        var fwky_date_prox = new Date(moment(date_ew).year() + 1, 0, 1).getDay();

                        if (fwky_date_prox > 3) {
                            FieldAct(53);
                            FieldActYear(date_ew.getFullYear());
                        } else {
                            FieldAct(weekno);
                            FieldActYear(date_ew.getFullYear() + 1);
                        }
                    } else {
                        FieldAct(weekno);
                        if (FieldActYear != "")
                            FieldActYear(date_ew.getFullYear());
                    }
                }
            }
        }
    };
   
    self.FeverDate = ko.observable(new Date());
    self.FeverDate.subscribe(function (newFeverDate) {
        
        var current_value = jQuery.type(newFeverDate) === 'date' ? newFeverDate : parseDate(newFeverDate, date_format_);
        var date_hospital_ = jQuery.type(app.Views.Contact.HospitalDate()) === 'date' ? app.Views.Contact.HospitalDate() : parseDate(app.Views.Contact.HospitalDate(), date_format_);
        var date_antiviral_ = jQuery.type(app.Views.Risk.AntiViralDate()) === 'date' ? app.Views.Risk.AntiViralDate() : parseDate(app.Views.Risk.AntiViralDate(), date_format_);
        self.CalculateEW("FeverDate", self.FeverEW, self.FeverEY);

        if (self.hasReset() != true) {
            if (date_hospital_ == null || date_hospital_ == "") {
                alert(msgValidationNotificationDateEnter);
                self.FeverDate(null);
            } else {
                if (moment(current_value).isAfter(moment(date_hospital_))) {
                    alert(msgValidationNotificationDateGtFeverDate);
                    self.FeverDate(null);
                }
            }

            if (app.Views.Risk.AntiViral() == "1") {
                if (date_antiviral_ == null || date_antiviral_ == "") {
                    alert(msgValidationAntiviralDateEnter);
                    self.FeverDate(null);
                } else {
                    if (current_value != null && date_hospital_ != null && moment(current_value).isAfter(moment(date_antiviral_))) {
                        alert(msgValidationAntiviralDateGtFeverDate);
                        self.HospAmDate(null);
                    }
                }
            }
        }

    });
    self.FeverEW = ko.observable("");
    self.FeverEY = ko.observable("");
    
    self.DiagPrinAdm = ko.observable("");
    self.DiagPrinAdmVal = ko.observable("");

    self.DiagOtroAdm = ko.observable("");

    self.DiagEW = ko.observable("");
    self.DiagEY = ko.observable("");
    self.DiagDate = ko.observable(new Date());
    //self.DiagDate.subscribe(function (newDiagDate) {
    //    self.CalculateEW("DiagDate", self.DiagEW, "");
    //});


    self.HospEW = ko.observable("");
    self.HospEY = ko.observable("");
    self.HospAmDate = ko.observable(new Date());
    self.HospAmDate.subscribe(function (newHospAmDate) {
        self.CalculateEW("HospAmDate", self.HospEW, self.HospEY);

        //if (self.UsrCountry() == 7 && self.UsrCountry() == 3) {
        var current_value = typeof (newHospAmDate) == "object" ? newHospAmDate : parseDate(newHospAmDate, date_format_);
        var date_hospital_ = typeof (app.Views.Contact.HospitalDate()) == "object" ? app.Views.Contact.HospitalDate() : parseDate(app.Views.Contact.HospitalDate(), date_format_);
        var date_fever_ = typeof (self.FeverDate()) == "object" ? self.FeverDate() : parseDate(self.FeverDate(), date_format_);
        var date_hosp_disc_ = typeof (self.HospExDate()) == "object" ? self.HospExDate() : parseDate(self.HospExDate(), date_format_);

        if (self.hasReset() != true) {
            if (date_hospital_ == null || date_hospital_ == "") {
                alert(msgValidationNotificationDateEnter);
                self.HospAmDate("");
            } else {
                if (moment(current_value).isAfter(moment(date_hospital_))) {
                    alert(msgValidationNotificationDateGtHospDate);
                    self.HospAmDate("");
                }
                if (current_value != null && date_fever_ != null && moment(current_value).isBefore(moment(date_fever_), "days")) {
                    alert(msgValidationHospDateGtFeverDate);
                    self.HospAmDate("");
                }
            }
        }
        //       }

    });


    self.HospExDate = ko.observable(new Date());
    self.HospExDate.subscribe(function (newHospExDate) {

        //if (self.UsrCountry() == 7 && self.UsrCountry() == 3) {
        var current_value = typeof (newHospExDate) == "object" ? newHospExDate : parseDate(newHospExDate, date_format_);
        var date_hospital_ = typeof (self.HospAmDate()) == "object" ? self.HospAmDate() : parseDate(self.HospAmDate(), date_format_);

        if (current_value == null)
        {

            $("a[href*='tab-case']").hide();
            $("#tab-case").hide();
            $("#CaseStatus").attr("disabled", true);
            //self.Destin("");
        } else if (self.Destin() == "") {

            $("a[href*='tab-case']").hide();
            $("#tab-case").hide();
            $("#CaseStatus").attr("disabled", true);
        } 
        else {

            if (self.UsrCountry() == 9 && self.Destin() == 'D') {
                self.FalleDate(current_value);
            }

            if (app.Views.Lab.CanConclude() == true) {
                $("a[href*='tab-case']").show();
                $("#tab-case").show();
                $("#tabs").tabs("refresh");
                $("#CaseStatus").attr("disabled", false);
            } else {
                $("a[href*='tab-case']").hide();
                $("#tab-case").hide();
                $("#CaseStatus").attr("disabled", true);
            }

        }

        if (self.hasReset() != true) {
            if (date_hospital_ == null || date_hospital_ == "") {
                alert(msgValidationNotificationDateEnter);
                self.HospExDate("");
            } else {
                if (moment(current_value).isBefore(moment(date_hospital_), "days")) {
                    alert(msgValidationHospExDateGtHospDate);
                    self.HospExDate("");
                }
            }
        }
        //        }
    });

    self.Destin = ko.observable("");
    self.FalleDate = ko.observable(new Date());
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
                alert(viewValidateHospitalizationDate);
                self.FalleDate("");
            } else {
                if (moment(current_value).isBefore(moment(date_hospital_), "days")) {
                    alert(viewValidateHospDateLtDefDate);
                    self.FalleDate("");
                }
                if (date_UCI_ex != null && date_UCI_ex != "") {
                    if (moment(current_value).isBefore(moment(date_UCI_ex), "days")) {
                        alert(viewValidateExitDateLtDefDate);
                        self.FalleDate("");
                    }
                }
            }
        }
    });

    self.ViewFallecido = ko.computed(function () {
        if (self.UsrCountry() != 9 && self.Destin() == 'D') {
            return true;
        } 
        else if (self.UsrCountry() == 9 && self.Destin() == 'D')
        {
            self.FalleDate(self.HospExDate());
            return false;
        } else {
            return false;
        }
        //return (self.Destin() == 'D') ? (if (self.UsrCountry() != 9) {  return true} else  { return false;} ): false;  
    }, self);
   
    self.ViewRefer = ko.computed(function () {
        return (self.Destin() == 'R') ? true : false;

    }, self);

    self.ICUAmDate = ko.observable(new Date());
    self.ICUAmDate.subscribe(function (newICUAmDate) {
        self.CalculateEW("ICUAmDate", self.ICUEW, self.ICUEY);

 //       if (self.UsrCountry() == 7 && self.UsrCountry() == 3) {
            var current_value = typeof (newICUAmDate) == "object" ? newICUAmDate : parseDate(newICUAmDate, date_format_);
            var date_hospital_ = typeof (self.HospAmDate()) == "object" ? self.HospAmDate() : parseDate(self.HospAmDate(), date_format_);
            var date_hospital_ex = typeof (self.HospExDate()) == "object" ? self.HospExDate() : parseDate(self.HospExDate(), date_format_);

            if (self.hasReset() != true) {
                if (date_hospital_ == null || date_hospital_ == "") {
                    alert(viewValidateHospDateMandatory);
                    self.ICUAmDate("");
                } else {
                    if (moment(current_value).isBefore(moment(date_hospital_), "days")) {
                        alert(viewValidateHospDateLtUCIDate);
                        self.ICUAmDate("");
                    }
                    if (date_hospital_ex != null && date_hospital_ex != "") {
                        if (moment(current_value).isAfter(moment(date_hospital_ex), "days")) {
                            alert(viewValidateExitDateGtUCIDate);
                            self.ICUAmDate("");
                        }
                    }
                    
                }
            }
 //       }
    });

    self.ICUEW = ko.observable("");
    self.ICUEY = ko.observable("");
    self.ICUExDate = ko.observable(new Date());
    self.ICUExDate.subscribe(function (newICUAmDate) {
 //       if (self.UsrCountry() == 7 && self.UsrCountry() == 3) {
            var current_value = typeof (newICUAmDate) == "object" ? newICUAmDate : parseDate(newICUAmDate, date_format_);
            var date_hospital_ = typeof (self.HospAmDate()) == "object" ? self.HospAmDate() : parseDate(self.HospAmDate(), date_format_);
            var date_hospital_ex = typeof (self.HospExDate()) == "object" ? self.HospExDate() : parseDate(self.HospExDate(), date_format_);
            var date_icu_adm = typeof (self.ICUAmDate()) == "object" ? self.ICUAmDate() : parseDate(self.ICUAmDate(), date_format_);
            var date_death = typeof (self.FalleDate()) == "object" ? self.FalleDate() : parseDate(self.FalleDate(), date_format_);

            if (self.hasReset() != true) {
                if (date_hospital_ == null || date_hospital_ == "") {
                    alert(viewValidateHospDateMandatory);
                    self.ICUExDate("");
                } else {
                    if (date_hospital_ == null || date_hospital_ == "") {
                        alert(viewValidateUCIAdmission);
                        self.ICUExDate("");
                    } else {
                        if (moment(current_value).isBefore(moment(date_hospital_), "days")) {
                            alert(viewValidateHospDateLtUCIExitDate);
                            self.ICUExDate("");
                        }
                        if (moment(current_value).isAfter(moment(date_hospital_ex), "days")) {
                            alert(viewValidateHospExitDateGtUCIExitDate);
                            self.ICUExDate("");
                        }
                        if (moment(current_value).isBefore(moment(date_icu_adm), "days")) {
                            alert(viewValidateUPCDateGtUCI);
                            self.ICUExDate("");
                        }
                        if (date_death != null || date_death != "") {
                            if (moment(current_value).isAfter(moment(date_death), "days")) {
                                alert(viewValidateDeathDateGtUCIExitDate);
                                self.ICUExDate("");
                            }
                        }
                    }
                }
            }
//        }
    });
    self.EnableICUDate = ko.computed(function () {
        //if (self.UsrCountry() != 7){
        //    return true;
        //} else {
            if (self.ICU() == 1) {
                return true;
            } else {
                self.ICUAmDate("");
                self.ICUExDate("");
                return false;
            }
        //}
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
    self.DiagEgOtro = ko.observable("");                //#### CAFQ

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
            alert(viewValidateOnsetFeverDate);
            self.DiagDate(null);
        } else {

            if (moment(self.DiagDate()).isBefore(moment(self.FeverDate()))) {
                alert(viewValidateDiagnosticDate);
                self.DiagDate(null);
            }
        }
    };

    

    
    self.IsSample = ko.observable(false);
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

            if (app.Views.Contact.SurvILI() == true && app.Views.Lab.FinalResult() && app.Views.Lab.CanConclude() == true) {
                //$("#CaseStatus").attr("disabled", false);
                $("a[href*='tab-case']").show();
                $("#tab-case").show();
                $("#CaseStatus").attr("disabled", false);
                $("#tabs").tabs("refresh");
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

            if (app.Views.Contact.SurvILI() == true && self.IsSample() === "false") {
                //$("#CaseStatus").attr("disabled", false);
                $("a[href*='tab-case']").show();
                $("#tab-case").show();
                $("#CaseStatus").attr("disabled", false);
                $("#tabs").tabs("refresh");
            } else if (self.Destin() != "" || app.Views.Contact.SurvILI() == true) {
                $("#CaseStatus").attr("disabled", false);
            } else {
                if (app.Views.Contact.IsSurv() == 2) {
                    $("a[href*='tab-case']").show();
                    $("#tab-case").show();
                    $("#CaseStatus").attr("disabled", false);
                    $("#tabs").tabs("refresh");
                } else {
                    $("a[href*='tab-case']").hide();
                    $("#tab-case").hide();
                    $("#CaseStatus").attr("disabled", true);
                    $("#tabs").tabs("refresh");
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

    self.SampleDate = ko.observable(new Date());
    self.SampleDate.subscribe(function (newSampleDate) {
        var current_value = typeof (newSampleDate) == "object" ? newSampleDate : parseDate(newSampleDate, date_format_);
        var date_fever_ = typeof (self.FeverDate()) == "object" ? self.FeverDate() : parseDate(self.FeverDate(), date_format_);


        if (self.hasReset() != true) {
            if (date_fever_ == null || date_fever_ == "") {
                alert(viewValidateOnsetFeverDate);
                self.SampleDate(null);
            } else {
                if (moment(current_value).diff(date_fever_, 'days', false) < 0) {
                    alert(viewValidateSampleDateGtOnsetFeverDate);
                    self.SampleDate(null);
                }
            }
        }

    });
    self.SampleType = ko.observable("");
    self.ShipDate = ko.observable(new Date());
    self.ShipDate.subscribe(function (newShipDate) {
        var current_value = typeof (newShipDate) == "object" ? newShipDate : parseDate(newShipDate, date_format_);
        var date_fever_ = typeof (self.FeverDate()) == "object" ? self.FeverDate() : parseDate(self.FeverDate(), date_format_);
        var Sample1Date = typeof (self.SampleDate()) == "object" ? self.SampleDate() : parseDate(self.SampleDate(), date_format_);

        if (self.hasReset() != true) {
            if (Sample1Date == null || Sample1Date == "") {
                alert(msgValidationSampleDateS1);
                self.ShipDate(null);
            } else {
                if (moment(current_value).diff(Sample1Date, 'days', false) < 0) {
                    alert(viewValidateShipDateGtSampleDate);
                    self.ShipDate(null);
                }
            }
        }
    });
    self.labs = ko.computed(function () {
          return app.Views.Home.labs();       
    }, self);
    self.LabsHospital = ko.observableArray();

    self.LabId = ko.observable("");

    self.SampleDate2 = ko.observable(new Date());
    //if (self.UsrCountry() == 7 || self.UsrCountry() == 3 || self.UsrCountry() == 25) {
        self.SampleDate2.subscribe(function (newSampleDate2) {
            if (newSampleDate2 != "") {
                var current_value = typeof (newSampleDate2) == "object" ? newSampleDate2 : parseDate(newSampleDate2, date_format_);
                var date_sampledate_ = typeof (self.SampleDate()) == "object" ? self.SampleDate() : parseDate(self.SampleDate(), date_format_);
                var date_fever_ = typeof (self.FeverDate()) == "object" ? self.FeverDate() : parseDate(self.FeverDate(), date_format_);


                if (self.hasReset() != true && newSampleDate2 != "") {
                    if (date_fever_ == null || date_fever_ == "") {
                        alert(viewValidateOnsetFeverDate);
                        self.SampleDate2(null);
                    } else {
                        if (moment(current_value).isBefore(moment(date_fever_))) {
                            alert(viewValidateSampleDateGtOnsetFeverDate);
                            self.SampleDate2(null);
                        }
                        if (moment(current_value).diff(date_sampledate_, 'days', false) < 0) {
                            alert(viewValidateSample2DateGtSample1Date);
                            self.SampleDate2(null);
                        }

                        if (moment(current_value).diff(date_fever_, 'days', false) > 15) {
                            alert(viewValidateSample2DateLtOnsetDate);
                            self.SampleDate2(null);
                        }
                    }
                }
            }
        });
    //}
    
    self.SampleType2 = ko.observable("");
    self.ShipDate2 = ko.observable(new Date());
    self.ShipDate2.subscribe(function (newShipDate) {
        var current_value = typeof (newShipDate) == "object" ? newShipDate : parseDate(newShipDate, date_format_);
        var date_fever_ = typeof (self.FeverDate()) == "object" ? self.FeverDate() : parseDate(self.FeverDate(), date_format_);
        var Sample2Date = typeof (self.SampleDate2()) == "object" ? self.SampleDate2() : parseDate(self.SampleDate2(), date_format_);

        if (self.hasReset() != true) {
            if (Sample2Date == null || Sample2Date == "") {
                alert(msgValidationSampleDateS1);
                self.ShipDate2(null);
            } else {
                if (moment(current_value).diff(Sample2Date, 'days', false) < 0) {
                    alert(viewValidateShipDateGtSampleDate);
                    self.ShipDate2(null);
                }
            }
        }
    });
    self.LabId2 = ko.observable("");

    self.SampleDate3 = ko.observable(new Date());

    self.SampleDate3.subscribe(function (newSampleDate3) {
        if (self.UsrCountry() == 7) {
            var current_value = typeof (newSampleDate3) == "object" ? newSampleDate3 : parseDate(newSampleDate3, date_format_);
            var date_sampledate_ = typeof (self.SampleDate()) == "object" ? self.SampleDate() : parseDate(self.SampleDate(), date_format_);
            var date_sampledate2_ = typeof (self.SampleDate2()) == "object" ? self.SampleDate2() : parseDate(self.SampleDate2(), date_format_);
            var date_fever_ = typeof (self.FeverDate()) == "object" ? self.FeverDate() : parseDate(self.FeverDate(), date_format_);


            if (self.hasReset() != true && newSampleDate3 != "") {
                if (date_fever_ == null || date_fever_ == "") {
                    alert(viewValidateOnsetFeverDate);
                    self.SampleDate3(null);
                } else {
                    if (moment(current_value).isBefore(moment(date_fever_))) {
                        alert(viewValidateSampleDateGtOnsetFeverDate);
                        self.SampleDate3(null);
                    }
                    if (moment(current_value).diff(date_sampledate_, 'days', false) < 0) {
                        alert(viewValidateSample3DateGtSample1Date);
                        self.SampleDate3(null);
                    }
                    if (moment(current_value).diff(date_sampledate2_, 'days', false) < 0) {
                        alert(viewValidateSample3DateGtSample2Date);
                        self.SampleDate3(null);
                    }

                    if (moment(current_value).diff(date_fever_, 'days', false) > 15) {
                        alert(viewValidateSample3DateLtOnsetDate);
                        self.SampleDate3(null);
                    }
                }
            }
        }
        });

    self.SampleType3 = ko.observable("");
    self.ShipDate3 = ko.observable(new Date());
    self.ShipDate3.subscribe(function (newShipDate) {
        var current_value = typeof (newShipDate) == "object" ? newShipDate : parseDate(newShipDate, date_format_);
        var date_fever_ = typeof (self.FeverDate()) == "object" ? self.FeverDate() : parseDate(self.FeverDate(), date_format_);
        var Sample3Date = typeof (self.SampleDate3()) == "object" ? self.SampleDate3() : parseDate(self.SampleDate3(), date_format_);

        if (self.hasReset() != true) {
            if (Sample3Date == null || Sample3Date == "") {
                alert(msgValidationSampleDateS1);
                self.ShipDate3(null);
            } else {
                if (moment(current_value).diff(Sample3Date, 'days', false) < 0) {
                    alert(viewValidateShipDateGtSampleDate);
                    self.ShipDate3(null);
                }
            }
        }

    });
    self.LabId3 = ko.observable("");

    self.VisibleMuestra2 = ko.computed(function () {
        return ((self.UsrCountry() == 7 || self.UsrCountry() == 3 ) && self.SampleDate() && app.Views.Contact.IsSurv() == 1) ? true : false;

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

    self.IsInusitadoSintomatologia = ko.computed(function () {                         //**** CAFQ
        if (self.UsrCountry() == 3) {                               // Bolivia
            return (app.Views.Contact.IsInusitado() == true) ? true : false;
        } else {
            return true;
        }
    }, self);             //**** CAFQ
    self.Temperatura = ko.observable().extend({ numeric: 1 });                      //**** CAFQ
    self.DolorCabeza = ko.observable("");                                           //**** CAFQ
    self.Mialgia = ko.observable("");                                               //**** CAFQ
    self.Erupcion = ko.observable("");                                              //**** CAFQ
    self.ErupcionLocaliz = ko.observable("");                                       //**** CAFQ
    self.DolorMuscular = ko.observable("");                                         //**** CAFQ
    self.DolorMuscularLocaliz = ko.observable("");                                  //**** CAFQ
    self.Disnea = ko.observable("");                                                //**** CAFQ
    self.IsErupcion = ko.computed(function () {                                     //**** CAFQ
        return (self.Erupcion() == true) ? true : false;
    }, self);                                             //**** CAFQ
    self.IsDolorMuscular = ko.computed(function () {                    //**** CAFQ
        return (self.DolorMuscular() == true) ? true : false;
    }, self);                                             //**** CAFQ
    self.SintomHemorrag = ko.observable("");                                             //**** CAFQ
    self.SintomHemorragDesc = ko.observable("");                                             //**** CAFQ
    self.AlteracEstadoMental = ko.observable("");                                             //**** CAFQ
    self.Altralgia = ko.observable("");                                             //**** CAFQ
    self.IsSintomaHemorragico = ko.computed(function () {               //**** CAFQ
        return (self.SintomHemorrag() == true) ? true : false;
    }, self);                     //**** CAFQ
    self.Escalofrios = ko.observable("");                                             //**** CAFQ
    self.Conjuntivitis = ko.observable("");                                             //**** CAFQ
    self.Rinitis = ko.observable("");                                             //**** CAFQ
    self.DiarreaAguda = ko.observable("");                                             //**** CAFQ
    self.DiarreaCronica = ko.observable("");                                             //**** CAFQ
    self.Mareo = ko.observable("");                                             //**** CAFQ
    self.FalloDesarrollo = ko.observable("");                                             //**** CAFQ
    self.Hepatomegalea = ko.observable("");                                             //**** CAFQ
    self.Ictericia = ko.observable("");                                             //**** CAFQ
    self.Linfadenopatia = ko.observable("");                                             //**** CAFQ
    self.Malestar = ko.observable("");                                             //**** CAFQ
    self.Nauseas = ko.observable("");                                             //**** CAFQ
    self.RigidezNuca = ko.observable("");                                             //**** CAFQ
    self.Paralisis = ko.observable("");                                             //**** CAFQ
    self.RespiratSuperior = ko.observable("");                                             //**** CAFQ
    self.RespiratInferior = ko.observable("");                                             //**** CAFQ
    self.DolorRetrorobitario = ko.observable("");                                             //**** CAFQ
    self.PerdidaPeso = ko.observable("");                                             //**** CAFQ
    self.Otro = ko.observable("");                                             //**** CAFQ
    self.OtroDesc = ko.observable("");                                             //**** CAFQ
    self.IsOtro = ko.computed(function () {               //**** CAFQ
        return (self.Otro() == true) ? true : false;
    }, self);                                             //**** CAFQ
    /*self.InfeccHospitFecha = ko.observable("");                                             //**** CAFQ
    self.InfeccHospit = ko.observable("");                                          //**** CAFQ     Radiobutton
    self.IsInfeHospPrev = ko.computed(function () {               //**** CAFQ
        return (self.InfeccHospit() == "1") ? true : false;
    }, self);                                             //**** CAFQ*/

    self.CaseStatus = ko.observable("");
    self.CaseComply = ko.observable("");
    self.CloseDate = ko.observable(new Date());
    self.ObservationCase = ko.observable("");
    self.EnableCloseDate = ko.computed(function () {
        var result = self.CaseStatus() == "3" || self.CaseStatus() == "2"; if (!result) self.CloseDate(null); return result;
    }, self);

    self.Destin.subscribe(function (NewDestin) {
        if (app.Views.Contact.SurvSARI() == true && app.Views.Contact.IsSurv() != "" && NewDestin != null && NewDestin != "") {
            if (self.HospExDate() == "" || self.HospExDate() == "undefined" || self.HospExDate() == null)
            {
                alert(viewValidateExitDateBeforeDestin);
                //self.Destin(""); Desactivado por requerimiento de RRR
                $("a[href*='tab-case']").hide();
                $("#tab-case").hide();
                $("#CaseStatus").attr("disabled", true);
                $("#tabs").tabs("refresh");
                $("#HospExDate").focus();
             } else if (NewDestin != "" && self.IsSample() === "false") {
                $("a[href*='tab-case']").show();
                $("#tab-case").show();
                $("#CaseStatus").attr("disabled", false);
                $("#tabs").tabs("refresh");
                if (self.UsrCountry() == 9 && NewDestin == 'D') { self.FalleDate(self.HospExDate()); }
             } else if (NewDestin != "" && self.IsSample() === "true" && app.Views.Lab.FinalResult() != "" && typeof app.Views.Lab.FinalResult() != "undefined" && app.Views.Lab.CanConclude() == true) {
                $("a[href*='tab-case']").show();
                $("#tab-case").show();
                $("#CaseStatus").attr("disabled", false);
                $("#tabs").tabs("refresh");
                if (self.UsrCountry() == 9 && NewDestin == 'D') { self.FalleDate(self.HospExDate()); }
             } else if (self.IsSample() === "true" && app.Views.Lab.FinalResult() != "" && typeof app.Views.Lab.FinalResult() != "undefined" && app.Views.Lab.CanConclude() == true) {
                $("a[href*='tab-case']").show();
                $("#tab-case").show();
                $("#tabs").tabs("refresh");
                if (self.UsrCountry() == 9 && NewDestin == 'D') { self.FalleDate(self.HospExDate()); }
            } else if (self.IsSample() === "true" && app.Views.Lab.Processed() === "false" ) {  // preguntar a Rodrigo
                    $("a[href*='tab-case']").show();
                    $("#tab-case").show();
                    $("#CaseStatus").attr("disabled", false);
                    $("#tabs").tabs("refresh");
                    if (self.UsrCountry() == 9 && NewDestin == 'D') { self.FalleDate(self.HospExDate()); }
            } else if (app.Views.Contact.Id() != null) {
                $("a[href*='tab-case']").hide();
                $("#tab-case").hide();
                $("#CaseStatus").attr("disabled", true);
                if (self.UsrCountry() == 9 && NewDestin == 'D') { self.FalleDate(self.HospExDate()); }
            }
            //$("#tabs").tabs("refresh");

        } else if (NewDestin == null || NewDestin == "") {
            $("a[href*='tab-case']").hide();
            $("#tab-case").hide();
            $("#tabs").tabs("refresh");
            $("#CaseStatus").attr("disabled", true);
            self.CaseStatus("");
            self.CloseDate(null);
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
       self.ICU(null);
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
       self.DiagEgOtro("");         //#### CAFQ
       self.IsSample(false);
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
       self.ResetHospitalInusual();         //#### CAFQ
       self.hasReset(false);
    };

    self.ResetHospitalInusual = function () {
        //alert('En ResetHospitalInusual3');
        self.Temperatura("");                                           //**** CAFQ
        self.Mialgia("");                                               //**** CAFQ
        self.Erupcion("");                                              //**** CAFQ
        self.ErupcionLocaliz("");                                       //**** CAFQ
        self.DolorMuscular("");                                         //**** CAFQ
        self.DolorMuscularLocaliz("");                                  //**** CAFQ
        self.Disnea("");                                                //**** CAFQ
        self.SintomHemorrag("");                                             //**** CAFQ
        self.SintomHemorragDesc("");                                             //**** CAFQ
        self.AlteracEstadoMental("");                                             //**** CAFQ
        self.Altralgia("");                                             //**** CAFQ
        self.Escalofrios("");                                             //**** CAFQ
        self.Conjuntivitis("");                                             //**** CAFQ
        self.Rinitis("");                                             //**** CAFQ
        self.DiarreaAguda("");                                             //**** CAFQ
        self.DiarreaCronica("");                                             //**** CAFQ
        self.Mareo("");                                             //**** CAFQ
        self.FalloDesarrollo("");                                             //**** CAFQ
        self.Hepatomegalea("");                                             //**** CAFQ
        self.Ictericia("");                                             //**** CAFQ
        self.Linfadenopatia("");                                             //**** CAFQ
        self.Malestar("");                                             //**** CAFQ
        self.Nauseas("");                                             //**** CAFQ
        self.RigidezNuca("");                                             //**** CAFQ
        self.Paralisis("");                                             //**** CAFQ
        self.RespiratSuperior("");                                             //**** CAFQ
        self.RespiratInferior("");                                             //**** CAFQ
        self.DolorRetrorobitario("");                                             //**** CAFQ
        self.PerdidaPeso("");                                             //**** CAFQ
        self.Otro("");                                             //**** CAFQ
        self.OtroDesc("");                                             //**** CAFQ
        /*self.InfeccHospitFecha("");                                             //**** CAFQ
        self.InfeccHospit("");                                          //**** CAFQ     Radiobutton*/
    }

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
            msg += "\n" + viewValidateOnsetInvalid;
        if (date_notification != null && date_fever != null && moment(date_fever).isAfter(moment(date_notification), "days")) {
            msg += "\n" + viewValidateOnsetLtNotiDate;
            $("#FeverDate").focus();
        }
            

        if (date_diagnostic != null && date_fever != null && moment(date_diagnostic).isBefore(moment(date_fever))) {
            msg += "\n" + viewValidateDiagnosticDateGtOnsetFeverDate;
            $("#DiagDate").focus();
        }
            
        if (app.Views.Contact.NoActiveBOL == true) {
            if (!self.Adenopatia() && !self.AntecedentesFiebre() && !self.Rinorrea() && !self.Malestar() && !self.Nauseas() && !self.DolorMuscular() && !self.Disnea() && !self.DolorCabeza() && !self.Estridor() && !self.Tos() && !self.Tiraje() && !self.Odinofagia()) {
                msg += "\n" + viewValidateSelectSypmtom;
            }
        }
        
        if (app.Views.Contact.SurvSARI() == true) {
            if ($("#HospAmDate").val() == "")
                msg += "\n" + viewValidateHospDateRequired;
            if ($("#HospAmDate").val() != "" && !moment(moment(date_hosp_adm).format(date_format_moment), [date_format_moment], true).isValid())
                msg += "\n" + viewValidateHospDateInvalid;
            if (date_hosp_adm != null && date_fever != null && moment(date_hosp_adm).isBefore(moment(date_fever), "days")) {
                msg += "\n" + viewValidateHospDateGtOnsetDate;
                $("#HospAmDate").focus();
            }

            if (date_hosp_adm != null && date_hosp_disc != null && moment(date_hosp_disc).isBefore(moment(date_hosp_adm), "days")) {
                msg += "\n" + viewValidateHospExitDateGtHospDate;
                $("#HospExDate").focus();
            }
        }

        // Validacion de fecha de fallecido

        if (self.Destin() == "D") {
            if ($("#FalleDate").val() == "")
                msg += "\n" + viewValidateDeathDateRequired;
            if ($("#FalleDate").val() != "" && !moment(moment(date_falle).format(date_format_moment), [date_format_moment], true).isValid())
                msg += "\n" + viewValidateDeathDateInvalid;
            if (date_falle != null && date_fever != null && moment(date_falle).isBefore(moment(date_fever), "days")) {
                msg += "\n" + viewValidateDeathDateGtOnsetDate;
                $("#FalleDate").focus();
            }

            if (date_falle != null && date_hosp_adm != null && moment(date_falle).isBefore(moment(date_hosp_adm), "days")) {
                msg += "\n" + viewValidateDeatDateGtHospDate;
                $("#FalleDate").focus();
            }
        }

        if ((self.HospExDate() == "" || self.HospExDate() == "undefined" || self.HospExDate() == null) && self.Destin() != "") {
            msg += "\n" + viewValidateExitDateRequired;
            //self.Destin(""); Desactivado por requerimiento de RRR
            $("#HospExDate").focus();
        }
        
         
        // Validaciones de Chile

        if (app.Views.Contact.SurvSARICHI() == true) {
            if (!self.ICU() || self.ICU() == "")
                msg += "\n" + viewValidateUPCRequired;
        }

//        if (!self.ICUAmDate())
//            msg += "\n" + "Fecha de UCI es requerida";
//        if (self.ICUAmDate() && !moment(moment(self.ICUAmDate()).format("MM/DD/YYYY"), ["MM/DD/YYYY"], true).isValid())
//            msg += "\n" + "Fecha de UCI es invalida";
//        if (self.HospAmDate() && self.ICUAmDate() && moment(self.HospAmDate()).isBefore(moment(self.ICUAmDate())))
//           msg += "\n" + "Fecha de UCI no puede ser posterior a la de hospitalización";

        if (!self.IsSample())
            msg += "\n" + viewValidateSampleInfoRequired;

        if (self.IsSample() == "true") {
            if ($("#SampleDate").val() == "")
                msg += "\n" + viewValidateSampleDateRequired;
            if ($("#SampleDate").val() != "" && !moment(moment(date_sample).format(date_format_moment), [date_format_moment], true).isValid())
                msg += "\n" + viewValidateSampleDateInvalid;
            if (date_sample != null && date_fever != null && moment(date_sample).isBefore(moment(date_fever), "days")) {
                msg += "\n" + viewValidateSampleDateGtOnsetDate;
                $("#SampleDate").focus()
            }
                

            if (date_sample != null && date_ship != null && moment(date_ship).isBefore(moment(date_sample), "days")) {
                msg += "\n" + viewValidateSentSampleDateGtSampleDate;
                $("#ShipDate").focus()
            }
                
            //if (self.HospAmDate() && self.SampleDate() && moment(self.HospAmDate()).isBefore(moment(self.SampleDate())))
            //    msg += "\n" + "Fecha de toma de muestra no puede ser posterior a la de hospitalización";
            if (!self.SampleType() || self.SampleType() == "")
                msg += "\n" + viewValidateSampleTypeRequired;



        }

        if (self.CaseStatus() == "3" || self.CaseStatus() == "2") {
            console.log(" CloseDate " + self.CloseDate());
            if ((self.CloseDate() == "" || self.CloseDate() == "undefined" || self.CloseDate() == null)) {
                if ($("#CloseDate").val() == "")
                    msg += "\n" + viewValidateCloseDateRequired;
                if ($("#CloseDate").val() != "" && !moment(moment(date_close_case).format(date_format_moment), [date_format_moment], true).isValid())
                    msg += "\n" + viewValidateCloseDateInvalid;

                if (app.Views.Lab.EndLabDate() && self.CloseDate() && moment(app.Views.Lab.EndLabDate()).isAfter(moment(self.CloseDate())))
                    msg += "\n" + viewValidateCloseDateGtLabDate;
            }

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
                if (data.IsSample == true) {
                    self.IsSample("true");
                } else if (data.IsSample == false) {
                    self.IsSample("false");
                } else self.IsSample("");
                if (data.FeverDate)
                    self.FeverDate(moment(data.FeverDate).clone().toDate());
                else self.FeverDate(null);
                self.CalculateEW("FeverDate", self.FeverEW, self.FeverEY);
                if (data.DiagDate)
                    self.DiagDate(moment(data.DiagDate).clone().toDate());
                else self.DiagDate(null);
                self.CalculateEW("DiagDate", self.DiagEW, self.DiagEY);
                if (data.HospAmDate)
                    self.HospAmDate(moment(data.HospAmDate).clone().toDate());
                else self.HospAmDate(null)
                self.CalculateEW("HospAmDate", self.HospEW, self.HospEY);
                if (data.HospExDate)
                    self.HospExDate(moment(data.HospExDate).clone().toDate());
                else self.HospExDate(null)
                self.ICU(data.ICU);
                if (data.ICUAmDate)
                    self.ICUAmDate(moment(data.ICUAmDate).clone().toDate());
                else self.ICUAmDate(null)
                if (data.ICUAmDate)
                    self.CalculateEW("ICUAmDate", self.ICUEW, self.ICUEY);
                if (data.ICUExDate)
                    self.ICUExDate(moment(data.ICUExDate).clone().toDate());
                else self.ICUExDate(null);
                self.Destin(data.Destin);
                self.DestinICU(data.DestinICU);
                self.InstReferName(data.InstReferName);
                if (data.FalleDate)
                    self.FalleDate(moment(data.FalleDate).clone().toDate());
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
                    self.SampleDate(moment(data.SampleDate).clone().toDate());
                else self.SampleDate(null)
                self.SampleType(data.SampleType);
                if (data.ShipDate)
                    self.ShipDate(moment(data.ShipDate).clone().toDate());
                else self.ShipDate(null);
                self.LabId(data.LabId);
            //Segunda muestra
                if (data.SampleDate2)
                    self.SampleDate2(moment(data.SampleDate2).clone().toDate());
                else self.SampleDate2(null)
                self.SampleType2(data.SampleType2);
                if (data.ShipDate2)
                    self.ShipDate2(moment(data.ShipDate2).clone().toDate());
                else self.ShipDate2(null);
                self.LabId2(data.LabId2);
            //Tercera muestra
                if (data.SampleDate3)
                    self.SampleDate3(moment(data.SampleDate3).clone().toDate());
                else self.SampleDate3(null)
                self.SampleType3(data.SampleType3);
                if (data.ShipDate3)
                    self.ShipDate3(moment(data.ShipDate3).clone().toDate());
                else self.ShipDate3(null);
                self.LabId3(data.LabId3);

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

                self.Temperatura(data.Temperatura);                         //#### CAFQ
                self.DolorCabeza(data.DolorCabeza);                         //#### CAFQ
                self.Mialgia(data.Mialgia);                               //#### CAFQ
                self.Erupcion(data.Erupcion);                               //#### CAFQ
                self.ErupcionLocaliz(data.ErupcionLocaliz);                 //#### CAFQ
                self.DolorMuscular(data.DolorMuscular);                     //#### CAFQ
                self.DolorMuscularLocaliz(data.DolorMuscularLocaliz);       //#### CAFQ
                self.Disnea(data.Disnea);                                   //#### CAFQ
                self.SintomHemorrag(data.SintomHemorrag);                     //#### CAFQ
                self.SintomHemorragDesc(data.SintomHemorragDesc);                     //#### CAFQ
                self.AlteracEstadoMental(data.AlteracEstadoMental);                     //#### CAFQ
                self.Altralgia(data.Altralgia);                     //#### CAFQ
                self.Escalofrios(data.Escalofrios);                     //#### CAFQ
                self.Conjuntivitis(data.Conjuntivitis);                     //#### CAFQ
                self.Rinitis(data.Rinitis);                     //#### CAFQ
                self.DiarreaAguda(data.DiarreaAguda);                     //#### CAFQ
                self.DiarreaCronica(data.DiarreaCronica);                     //#### CAFQ
                self.Mareo(data.Mareo);                     //#### CAFQ
                self.FalloDesarrollo(data.FalloDesarrollo);                     //#### CAFQ
                self.Hepatomegalea(data.Hepatomegalea);                    //#### CAFQ
                self.Ictericia(data.Ictericia);                    //#### CAFQ
                self.Linfadenopatia(data.Linfadenopatia);                    //#### CAFQ
                self.Malestar(data.Malestar);                    //#### CAFQ
                self.Nauseas(data.Nauseas);                    //#### CAFQ
                self.RigidezNuca(data.RigidezNuca);                    //#### CAFQ
                self.Paralisis(data.Paralisis);                    //#### CAFQ
                self.RespiratSuperior(data.RespiratSuperior);                    //#### CAFQ
                self.RespiratInferior(data.RespiratInferior);                    //#### CAFQ
                self.DolorRetrorobitario(data.DolorRetrorobitario);                    //#### CAFQ
                self.PerdidaPeso(data.PerdidaPeso);                    //#### CAFQ
                self.Otro(data.Otro);                    //#### CAFQ
                self.OtroDesc(data.OtroDesc);                    //#### CAFQ
                /*self.InfeccHospit(data.InfeccHospit);                    //#### CAFQ
                self.InfeccHospitFecha(data.InfeccHospitFecha);                    //#### CAFQ*/

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
                else 
                    self.DiagEg("");
                self.DiagEgOtro(data.DiagEgOtro);       //#### CAFQ

                self.Tiraje(data.Tiraje);
                self.Odinofagia(data.Odinofagia);
                self.CaseStatus(data.CaseStatus);
                if (data.CloseDate)
                    self.CloseDate(moment(data.CloseDate).clone().toDate());
                self.ObservationCase( data.ObservationCase  != null ? data.ObservationCase : "" );
                self.hasReset(false);
                //if (app.Views.Contact.selectedServiceId() > 0) {
                //    alert("labs_list");
                //    $.getJSON(app.dataModel.getLabsHospitalUrl, { institutionId: app.Views.Contact.selectedServiceId() }, function (data, status) {
                //        console.log(data.LabsHospital);
                //        console.log(self.LabsHospital());
                //        self.LabsHospital(data.LabsHospital);
                //        console.log(self.LabsHospital());
                //    });
                //} else {
                    self.LabsHospital(data.LabsHospital);
                //}
                

            })
            .fail(function(jqXHR, textStatus, errorThrown) {
                console.log(errorThrown);
            });
    };
    
    self.Save = function () {
        app.Views.Home.ValidateAll();
    };

    self.Cancel = function () {
        if (confirm(viewConfirmExit)) {
            app.Views.Home.CancelEdit();
        }
    };

    self.SaveHospital = function (nextStep) {
        date_fever = jQuery.type(self.FeverDate()) === 'date' ? self.FeverDate() : parseDate($("#FeverDate").val(), date_format_);
        date_diagnostic = jQuery.type(self.DiagDate()) === 'date' ? self.DiagDate() : parseDate($("#DiagDate").val(), date_format_);
        date_hosp_adm = jQuery.type(self.HospAmDate()) === 'date' ? self.HospAmDate() : parseDate($("#HospAmDate").val(), date_format_);
        date_hosp_disc = jQuery.type(self.HospExDate()) === 'date' ? self.HospExDate() : parseDate($("#HospExDate").val(), date_format_);
        date_ICU_adm = jQuery.type(self.ICUAmDate()) === 'date' ? self.ICUAmDate() : parseDate($("#ICUAmDate").val(), date_format_);
        date_ICU_disc = jQuery.type(self.ICUExDate()) === 'date' ? self.ICUExDate() : parseDate($("#ICUExDate").val(), date_format_);
        date_falle = jQuery.type(self.FalleDate()) === 'date' ? self.FalleDate() : parseDate($("#FalleDate").val(), date_format_);
        date_sample = jQuery.type(self.SampleDate()) === 'date' ? self.SampleDate() : parseDate($("#SampleDate").val(), date_format_);
        date_ship = jQuery.type(self.ShipDate()) === 'date' ? self.ShipDate() : parseDate($("#ShipDate").val(), date_format_);
        date_sample2 = jQuery.type(self.SampleDate2()) === 'date' ? self.SampleDate2() : parseDate($("#SampleDate2").val(), date_format_);
        date_ship2 = jQuery.type(self.ShipDate2()) === 'date' ? self.ShipDate2() : parseDate($("#ShipDate2").val(), date_format_);
        date_sample3 = jQuery.type(self.SampleDate3()) === 'date' ? self.SampleDate3() : parseDate($("#SampleDate3").val(), date_format_);
        date_ship3 = jQuery.type(self.ShipDate3()) === 'date' ? self.ShipDate3() : parseDate($("#ShipDate3").val(), date_format_);
        date_close_case = jQuery.type(self.CloseDate()) === 'date' ? self.CloseDate() : parseDate($("#CloseDate").val(), date_format_);
        //date_InfeccHospitFecha = parseDate($("#InfeccHospitFecha").val(), date_format_);        //#### CAFQ

        // falta la fecha de cierre de caso

        /*
        console.log(self.Temperatura());                        //#### DESARROLLO
        console.log(parseFloat(self.Temperatura()));            //#### DESARROLLO 
        console.log(parseFloat(self.Temperatura(), '####,##'));            //#### DESARROLLO 
        //var numero = self.Temperatura();
        //console.log(self.Temperatura().replace(".", ","));
        //numero = numero.toString().replace('.', ',');
        var numero = self.Temperatura().toString().replace('.', ',');
        console.log(numero.toString().replace('.', ','));
        console.log("self.SaveHospital->MEDIO");                        //#### DESARROLLO
        */
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
                LabId: $("#SampleDate3").val() == "" ? null : self.LabId(),
                SampleDate2: $("#SampleDate2").val() == "" ? null : moment(date_sample2).format(date_format_ISO),
                SampleType2: self.SampleType2(),
                ShipDate2: $("#ShipDate2").val() == "" ? null : moment(date_ship2).format(date_format_ISO),
                LabId2: $("#SampleDate3").val() == "" ? null : self.LabId2(),
                SampleDate3: $("#SampleDate3").val() == "" ? null : moment(date_sample3).format(date_format_ISO),
                SampleType3: self.SampleType3(),
                ShipDate3: $("#ShipDate3").val() == "" ? null : moment(date_ship3).format(date_format_ISO),
                LabId3: $("#SampleDate3").val() == "" ? null : self.LabId3(),
                Adenopatia: self.Adenopatia() != true ? false : self.Adenopatia(),
                AntecedentesFiebre: self.AntecedentesFiebre() != true ? false : self.AntecedentesFiebre(),
                Rinorrea: self.Rinorrea() != true ? false : self.Rinorrea(),
                Malestar: self.Malestar() != true ? false : self.Malestar(),
                Nauseas: self.Nauseas() != true ? false : self.Nauseas(),
                DolorMuscular: self.DolorMuscular() != true ? false : self.DolorMuscular(),
                Disnea: self.Disnea() != true ? false : self.Disnea(),
                DolorCabeza: self.DolorCabeza() != true ? false : self.DolorCabeza(),
                Estridor: self.Estridor() != true ? false : self.Estridor(),
                Tos: self.Tos() != true ? false : self.Tos(),
                Temperatura: self.Temperatura().toString().replace('.', ','),                  //#### CAFQ
                DolorCabeza: self.DolorCabeza() != true ? false : self.DolorCabeza(),
                Mialgia: self.Mialgia(),                                  //#### CAFQ
                Erupcion: self.Erupcion(),                                  //#### CAFQ
                ErupcionLocaliz: self.ErupcionLocaliz(),                    //#### CAFQ
                DolorMuscular: self.DolorMuscular(),
                DolorMuscularLocaliz: self.DolorMuscularLocaliz(),          //#### CAFQ
                Disnea: self.Disnea(),
                SintomHemorrag: self.SintomHemorrag(),                      //#### CAFQ
                SintomHemorragDesc: self.SintomHemorragDesc(),              //#### CAFQ
                AlteracEstadoMental: self.AlteracEstadoMental(),            //#### CAFQ
                Altralgia: self.Altralgia(),                  //#### CAFQ
                Escalofrios: self.Escalofrios(),                  //#### CAFQ
                Conjuntivitis: self.Conjuntivitis(),                  //#### CAFQ
                Rinitis: self.Rinitis(),                  //#### CAFQ
                DiarreaAguda: self.DiarreaAguda(),                  //#### CAFQ
                DiarreaCronica: self.DiarreaCronica(),                  //#### CAFQ
                Mareo: self.Mareo(),                  //#### CAFQ
                FalloDesarrollo: self.FalloDesarrollo(),                  //#### CAFQ
                Hepatomegalea: self.Hepatomegalea(),                  //#### CAFQ
                Ictericia: self.Ictericia(),                  //#### CAFQ
                Linfadenopatia: self.Linfadenopatia(),                  //#### CAFQ
                Malestar: self.Malestar(),                  //#### CAFQ
                Nauseas: self.Nauseas(),                  //#### CAFQ
                RigidezNuca: self.RigidezNuca(),                  //#### CAFQ
                Paralisis: self.Paralisis(),                  //#### CAFQ
                RespiratSuperior: self.RespiratSuperior(),                  //#### CAFQ
                RespiratInferior: self.RespiratInferior(),                  //#### CAFQ
                DolorRetrorobitario: self.DolorRetrorobitario(),                  //#### CAFQ
                PerdidaPeso: self.PerdidaPeso(),                  //#### CAFQ
                Otro: self.Otro(),                  //#### CAFQ
                OtroDesc: self.OtroDesc(),                  //#### CAFQ
                /*InfeccHospit: self.InfeccHospit(),                  //#### CAFQ
                InfeccHospitFecha: $("#InfeccHospitFecha").val() == "" ? null : moment(date_InfeccHospitFecha).format(date_format_ISO),    //#### CAFQ*/
                DifResp: self.DifResp() != true ? false : self.DifResp(),
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
                DiagEgOtro: self.DiagEgOtro() == null ? "" : self.DiagEgOtro().toLocaleUpperCase(),         //#### CAFQ
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