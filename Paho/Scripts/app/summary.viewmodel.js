
function SummaryYearItem(data) {
    var self = this;
    //****
    self.ColETINumST = data.ColETINumST;
    self.ColETIDenoST = data.ColETIDenoST;
    self.ColETINumEmerFST = data.ColETINumEmerFST;
    self.ColETINumEmerMST = data.ColETINumEmerMST;
    self.ColETINumEmerST = data.ColETINumEmerST;

    self.ColHospTST = data.ColHospTST;
    self.ColUCITST = data.ColUCITST;
    self.ColFalleTST = data.ColFalleTST;
    self.ColHospSARITST = data.ColHospSARITST;          //#### CAFQ: 181101
    self.ColUCISARITST = data.ColUCISARITST;            //#### CAFQ: 181101
    self.ColFalleSARITST = data.ColFalleSARITST;        //#### CAFQ: 181101
    self.ColNeuTST = data.ColNeuTST;
    self.ColCCSARITST = data.ColCCSARITST;              //####CAFQ
    self.ColVentTST = data.ColVentTST;

    self.ColILICasesST = data.ColILICasesST;
    self.ColILISamplesTakenST = data.ColILISamplesTakenST;
    self.ColTotalVisitsST = data.ColTotalVisitsST;

    self.ColEpiYear = data.EpiYear;
    self.ColEpiWeek = data.EpiWeek;
    self.ColEpiYearWeek = data.EpiYear + "-" + data.EpiWeek;
    self.StartDateOfWeek = data.StartDateOfWeek;
    self.WeekendDate = data.WeekendDate

    self.ColETIFST = ko.observable("");     //??
    self.ColETIMST = ko.observable("");     //??

    self.UsrCountry = ko.observable(selcty);                // Pais del usuario logueado
    //****
    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7) ? true : false;
    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;
    }, self);

    self.ActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? true : false;
    }, self);

    self.NoActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? false : true;
    }, self);

    self.ActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? true : false;
    }, self);

    self.NoActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? false : true;
    }, self);

    self.ActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? true : false;
    }, self);

    self.NoActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? false : true;
    }, self);

    self.ActiveCOS = ko.computed(function () {
        return (self.UsrCountry() == 9) ? true : false;
    }, self);

    self.NoActiveCOS = ko.computed(function () {
        return (self.UsrCountry() == 9) ? false : true;
    }, self);

    self.ActiveDOM = ko.computed(function () {
        return (self.UsrCountry() == 11) ? true : false;
    }, self);

    self.NoActiveDOM = ko.computed(function () {
        return (self.UsrCountry() == 11) ? false : true;
    }, self);

    self.ActiveCAY = ko.computed(function () {
        return (self.UsrCountry() == 119) ? true : false;
    }, self);

    self.NoActiveCAY = ko.computed(function () {
        return (self.UsrCountry() == 119) ? false : true;
    }, self);

    self.ActiveHON = ko.computed(function () {
        return (self.UsrCountry() == 15) ? true : false;
    }, self);
}

function SummayItem(data) {
    var self = this;

    self.Id = data.Id;
    self.UsrCountry = ko.observable(selcty);                    // Pais del usuario logueado
    self.CaseSummaryId = data.CaseSummaryId;
    self.AgeGroup = data.AgeGroup;

    //if (self.UsrCountry() == 7 || self.UsrCountry() == 3) {
    //    self.AgeGroupDescription = AgeGroupDescriptionCHI[parseInt(self.AgeGroup) - 1];
    //} else if (self.UsrCountry() == 25 || self.UsrCountry() == 11 || self.UsrCountry() == 18) {
    //    self.AgeGroupDescription = AgeGroupDescriptionSUR[parseInt(self.AgeGroup) - 1];
    //} else {
    //    self.AgeGroupDescription = AgeGroupDescription[parseInt(self.AgeGroup) - 1];
    //}

    self.AgeGroupDescription = CatAgeGroup[parseInt(self.AgeGroup) - 1].AgeGroup;
    //****
    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7) ? true : false;
    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;
    }, self);

    self.ActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? true : false;
    }, self);

    self.NoActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? false : true;
    }, self);

    self.ActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? true : false;
    }, self);

    self.NoActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? false : true;
    }, self);

    self.ActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? true : false;
    }, self);

    self.NoActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? false : true;
    }, self);

    self.ActiveCOS = ko.computed(function () {
        return (self.UsrCountry() == 9) ? true : false;
    }, self);

    self.NoActiveCOS = ko.computed(function () {
        return (self.UsrCountry() == 9) ? false : true;
    }, self);

    self.ActiveDOM = ko.computed(function () {
        return (self.UsrCountry() == 11) ? true : false;
    }, self);

    self.NoActiveDOM = ko.computed(function () {
        return (self.UsrCountry() == 11) ? false : true;
    }, self);

    self.ActiveCAY = ko.computed(function () {
        return (self.UsrCountry() == 119) ? true : false;
    }, self);

    self.NoActiveCAY = ko.computed(function () {
        return (self.UsrCountry() == 119) ? false : true;
    }, self);

    self.ActiveHON = ko.computed(function () {
        return (self.UsrCountry() == 15) ? true : false;
    }, self);

    //****
    self.ETIDenoFem = ko.observable(data.ETIDenoFem);
    self.ETIDenoMaso = ko.observable(data.ETIDenoMaso);

    if (self.DisableCHI() || self.ActiveBOL() || self.ActiveHON()) {
        self.ETIDenoST = ko.computed(function () {
            return parseInt(self.ETIDenoFem()) + parseInt(self.ETIDenoMaso());
        }, self);
    } else {
        self.ETIDenoST = ko.observable(data.ETIDenoST);
    }
    //****
    self.ETINumFem = ko.observable(data.ETINumFem);
    self.ETINumMaso = ko.observable(data.ETINumMaso);

    if (self.DisableCHI() || self.ActiveBOL() || self.ActiveHON()) {
        self.ETINumST = ko.computed(function () {
            return parseInt(self.ETINumFem()) + parseInt(self.ETINumMaso());
        }, self);

    } else {
        self.ETINumST = ko.observable(data.ETINumST);
    }
    //****
    self.ETINumEmerFem = ko.observable(data.ETINumEmerFem);
    self.ETINumEmerMaso = ko.observable(data.ETINumEmerMaso);

    if (self.DisableCHI() || self.ActiveBOL()) {
        self.ETINumEmerST = ko.computed(function () {
            return parseInt(self.ETINumEmerFem()) + parseInt(self.ETINumEmerMaso());
        }, self);

    } else {
        self.ETINumEmerST = ko.observable(data.ETINumEmerST);
    }
    //****
    self.HospFem = ko.observable(data.HospFem);
    self.HospMaso = ko.observable(data.HospMaso);

    console.log("m1");
    if (self.DisableCHI() || self.ActiveBOL() || self.ActiveHON()) {    // || self.ActiveHON()
        /*if (self.ActiveHON()) {
            console.log("m2");
            self.HospST = ko.observable(data.HospST);
        }*/

        console.log("m3");
        self.HospST = ko.computed(function () {
            console.log("m4");
            return parseInt(self.HospFem()) + parseInt(self.HospMaso());
        }, self);

    } else {
        console.log("m5");
        self.HospST = ko.observable(data.HospST);

        /*if (self.ActiveHON()) {
            console.log("m6");
            self.HospST = ko.computed(function () {
                console.log("m7");
                return parseInt(self.HospFem()) + parseInt(self.HospMaso());
            }, self);
        }*/

    }
    //****
    self.HospSARIFem = ko.observable(data.HospSARIFem);
    self.HospSARIMaso = ko.observable(data.HospSARIMaso);

    if (self.ActiveHON()) {
        self.HospSARIST = ko.computed(function () {
            return parseInt(self.HospSARIFem()) + parseInt(self.HospSARIMaso());
        }, self);
    } else {
        self.HospSARIST = ko.observable(data.HospSARIST);
    }
    //****
    self.UCIFem = ko.observable(data.UCIFem);
    self.UCIMaso = ko.observable(data.UCIMaso);

    if (self.DisableCHI() || self.ActiveBOL() || self.ActiveHON()) {
        self.UCIST = ko.computed(function () {
            return parseInt(self.UCIFem()) + parseInt(self.UCIMaso());
        }, self);
    } else {
        self.UCIST = ko.observable(data.UCIST);
    }
    //****
    self.UCISARIFem = ko.observable(data.UCISARIFem);
    self.UCISARIMaso = ko.observable(data.UCISARIMaso);

    if (self.ActiveHON()) {
        self.UCISARIST = ko.computed(function () {
            return parseInt(self.UCISARIFem()) + parseInt(self.UCISARIMaso());
        }, self);
    } else {
        self.UCISARIST = ko.observable(data.UCISARIST);
    }
    //****
    self.DefFem = ko.observable(data.DefFem);
    self.DefMaso = ko.observable(data.DefMaso);

    if (self.DisableCHI() || self.ActiveBOL() || self.ActiveHON()) {
        self.DefST = ko.computed(function () {
            return parseInt(self.DefFem()) + parseInt(self.DefMaso());
        }, self);
    } else {
        self.DefST = ko.observable(data.DefST);
    }
    //****
    self.DefSARIFem = ko.observable(data.DefSARIFem);
    self.DefSARIMaso = ko.observable(data.DefSARIMaso);

    if (self.ActiveHON()) {
        self.DefSARIST = ko.computed(function () {
            return parseInt(self.DefSARIFem()) + parseInt(self.DefSARIMaso());
        }, self);
    } else {
        self.DefSARIST = ko.observable(data.DefSARIST);
    }
    //****
    self.NeuFem = ko.observable(data.NeuFem);
    self.NeuMaso = ko.observable(data.NeuMaso);

    self.NeuST = ko.observable(data.NeuST);
    //****
    self.CCSARIFem = ko.observable(data.CCSARIFem);
    self.CCSARIMaso = ko.observable(data.CCSARIMaso);

    if (self.ActiveHON()) {
        self.CCSARIST = ko.computed(function () {
            return parseInt(self.CCSARIFem()) + parseInt(self.CCSARIMaso());
        }, self);

    } else {
        self.CCSARIST = ko.observable(data.CCSARIST);
    }
    //****
    self.VentFem = ko.observable(data.VentFem);
    self.VentMaso = ko.observable(data.VentMaso);

    self.VentST = ko.observable(data.VentST);
    //**** ILI Jamaica
    self.ILICases = ko.observable(data.ILICases);
    self.ILISamplesTaken = ko.observable(data.ILISamplesTaken);
    self.TotalVisits = ko.observable(data.TotalVisits);
    //****
    self.MakeValueOfSummayItem = function () {
        return {
            Id: self.Id,
            CaseSummaryId: self.CaseSummaryId,
            AgeGroup: self.AgeGroup,
            ETINumFem: typeof (self.ETINumFem()) !== "number" ? parseInt(self.ETINumFem()) : self.ETINumFem(),
            ETINumMaso: typeof (self.ETINumMaso()) !== "number" ? parseInt(self.ETINumMaso()) : self.ETINumMaso(),
            ETINumST: typeof (self.ETINumST()) !== "number" ? parseInt(self.ETINumST()) : self.ETINumST(),
            ETINumEmerFem: typeof (self.ETINumEmerFem()) !== "number" ? parseInt(self.ETINumEmerFem()) : self.ETINumEmerFem(),
            ETINumEmerMaso: typeof (self.ETINumEmerMaso()) !== "number" ? parseInt(self.ETINumEmerMaso()) : self.ETINumEmerMaso(),
            ETINumEmerST: typeof (self.ETINumEmerST()) !== "number" ? parseInt(self.ETINumEmerST()) : self.ETINumEmerST(),
            ETIDenoFem: typeof (self.ETIDenoFem()) !== "number" ? parseInt(self.ETIDenoFem()) : self.ETIDenoFem(),
            ETIDenoMaso: typeof (self.ETIDenoMaso()) !== "number" ? parseInt(self.ETIDenoMaso()) : self.ETIDenoMaso(),
            ETIDenoST: typeof (self.ETIDenoST()) !== "number" ? parseInt(self.ETIDenoST()) : self.ETIDenoST(),
            HospFem: typeof (self.HospFem()) !== "number" ? parseInt(self.HospFem()) : self.HospFem(),
            HospMaso: typeof (self.HospMaso()) !== "number" ? parseInt(self.HospMaso()) : self.HospMaso(),
            HospST: typeof (self.HospST()) !== "number" ? parseInt(self.HospST()) : self.HospST(),
            ILICases: typeof (self.ILICases()) !== "number" ? parseInt(self.ILICases()) : self.ILICases(),
            UCIFem: typeof (self.UCIFem()) !== "number" ? parseInt(self.UCIFem()) : self.UCIFem(),
            UCIMaso: typeof (self.UCIMaso()) !== "number" ? parseInt(self.UCIMaso()) : self.UCIMaso(),
            UCIST: typeof (self.UCIST()) !== "number" ? parseInt(self.UCIST()) : self.UCIST(),

            HospSARIFem: typeof (self.HospSARIFem()) !== "number" ? parseInt(self.HospSARIFem()) : self.HospSARIFem(),
            HospSARIMaso: typeof (self.HospSARIMaso()) !== "number" ? parseInt(self.HospSARIMaso()) : self.HospSARIMaso(),
            HospSARIST: typeof (self.HospSARIST()) !== "number" ? parseInt(self.HospSARIST()) : self.HospSARIST(),

            UCISARIFem: typeof (self.UCISARIFem()) !== "number" ? parseInt(self.UCISARIFem()) : self.UCISARIFem(),
            UCISARIMaso: typeof (self.UCISARIMaso()) !== "number" ? parseInt(self.UCISARIMaso()) : self.UCISARIMaso(),
            UCISARIST: typeof (self.UCISARIST()) !== "number" ? parseInt(self.UCISARIST()) : self.UCISARIST(),

            DefFem: typeof (self.DefFem()) !== "number" ? parseInt(self.DefFem()) : self.DefFem(),
            DefMaso: typeof (self.DefMaso()) !== "number" ? parseInt(self.DefMaso()) : self.DefMaso(),
            DefST: typeof (self.DefST()) !== "number" ? parseInt(self.DefST()) : self.DefST(),

            DefSARIFem: typeof (self.DefSARIFem()) !== "number" ? parseInt(self.DefSARIFem()) : self.DefSARIFem(),
            DefSARIMaso: typeof (self.DefSARIMaso()) !== "number" ? parseInt(self.DefSARIMaso()) : self.DefSARIMaso(),
            DefSARIST: typeof (self.DefSARIST()) !== "number" ? parseInt(self.DefSARIST()) : self.DefSARIST(),

            TotalVisits: typeof (self.TotalVisits()) !== "number" ? parseInt(self.TotalVisits()) : self.TotalVisits(),
            ILISamplesTaken: typeof (self.ILISamplesTaken()) !== "number" ? parseInt(self.ILISamplesTaken()) : self.ILISamplesTaken(),

            NeuFem: typeof (self.NeuFem()) !== "number" ? parseInt(self.NeuFem()) : self.NeuFem(),
            NeuMaso: typeof (self.NeuMaso()) !== "number" ? parseInt(self.NeuMaso()) : self.NeuMaso(),
            NeuST: typeof (self.NeuST()) !== "number" ? parseInt(self.NeuST()) : self.NeuST(),
            CCSARIFem: typeof (self.CCSARIFem()) !== "number" ? parseInt(self.CCSARIFem()) : self.CCSARIFem(),
            CCSARIMaso: typeof (self.CCSARIMaso()) !== "number" ? parseInt(self.CCSARIMaso()) : self.CCSARIMaso(),
            CCSARIST: typeof (self.CCSARIST()) !== "number" ? parseInt(self.CCSARIST()) : self.CCSARIST(),
            VentFem: typeof (self.VentFem()) !== "number" ? parseInt(self.VentFem()) : self.VentFem(),
            VentMaso: typeof (self.VentMaso()) !== "number" ? parseInt(self.VentMaso()) : self.VentMaso(),
            VentST: typeof (self.VentST()) !== "number" ? parseInt(self.VentST()) : self.VentST()
        };
    }
}

function SummaryViewModel(app, dataModel) {
    var self = this;
    /////////////var msgSavedData = app.dataModel.MsgSavedData;      //#### CAFQ 

    var date_format_moment = app.dataModel.date_format_moment;
    var date_format_ISO = app.dataModel.date_format_ISO;
    var date_format_ = app.dataModel.date_format_;
    var date_format_DatePicker = app.dataModel.date_format_DatePicker;

    self.Id = "";
    self.UsrCountry = ko.observable(selcty); // Pais del usuario logueado
    self.selectedCountryId = ko.observable("");
    self.countries = ko.observableArray(countries);
    self.activecountries = ko.computed(function () {
        return $.grep(self.countries(), function (v) {
            return v.Active === true;
        });
    });
    self.selectedHospitalId = ko.observable("");
    self.selectedHospitalId.subscribe(function (HospitalSelect) {
        self.ChangeHospitalAndDate();
    });

    self.hospitals = ko.observableArray(institutions);
    self.HospitalDate = ko.observable(null);
    self.HospitalEW = ko.observable("");
    self.HospitalYE = ko.observable("");

    self.ColHospTST01 = ko.observable("");
    self.ColUCITST01 = ko.observable("");
    self.ColFalleTST01 = ko.observable("");

    self.ColILICasesST01 = ko.observable("");
    self.ColILISamplesTakenST01 = ko.observable("");
    self.ColTotalVisitsST01 = ko.observable("");

    self.ColETIFST = ko.observable("");
    self.ColETIMST = ko.observable("");

    //self.CalculateEW = function (FieldDate, FieldAct, FieldActYear) {
    //    if ($("#" + FieldDate).val() != "") {
    //        ////////var date_ew = new Date($("#" + FieldDate).datepicker('getDate', { dateFormat: date_format_DatePicker }));
    //        var date_ew = new Date($("#" + FieldDate).val());
    //        var fwky_date = new Date(moment(date_ew).year(), 0, 1).getDay();
    //        var weekno = moment(date_ew).week();
    //        var weeknoISO = moment(date_ew).isoWeek();

    //        if (date_ew == null) {
    //            FieldAct(null);
    //            FieldActYear(null);

    //        } else {

    //            if (fwky_date > 3) {
    //                var month = 11, day = 31;
    //                var end_date_year_ant = new Date(moment(date_ew).year() - 1, month, day--);

    //                if (weekno == 1 && moment(date_ew).month() == 0) {
    //                    var fwky_date_ant = new Date(moment(date_ew).year() - 1, 0, 1).getDay();
    //                    var fwdoyant = moment(end_date_year_ant).isoWeek();
    //                    if (fwky_date_ant > 3) {

    //                        FieldAct(fwdoyant - 1);

    //                    } else {

    //                        if (weekno == 1 && moment(date_ew).month() == 0 && fwky_date_ant <= 3) {
    //                            FieldAct(53);
    //                            fwdoyant = 53;
    //                        }
    //                        else
    //                            FieldAct(fwdoyant);
    //                    }
    //                    if (FieldActYear != "")
    //                        if (fwdoyant == 52 || fwdoyant == 53)
    //                            FieldActYear(date_ew.getFullYear() - 1);
    //                        else
    //                            FieldActYear(date_ew.getFullYear());
    //                }
    //                else if (weekno == 1 && moment(date_ew).month() != 0) {
    //                    FieldAct(moment(date_ew).isoWeek() - 1);
    //                    if (FieldActYear != "")
    //                        FieldActYear(date_ew.getFullYear());
    //                }
    //                else {
    //                    FieldAct(weekno - 1);
    //                    if (FieldActYear != "")
    //                        FieldActYear(date_ew.getFullYear());
    //                }
    //            } else {
    //                if (weekno == 1 && moment(date_ew).month() == 11) {
    //                    var fwky_date_prox = new Date(moment(date_ew).getFullYear() + 1, 0, 1).getDay();

    //                    if (fwky_date_prox > 3) {
    //                        FieldAct(53);
    //                        FieldActYear(date_ew.getFullYear());
    //                    } else {
    //                        FieldAct(weekno);
    //                        FieldActYear(date_ew.getFullYear() + 1);
    //                    }
    //                } else {
    //                    FieldAct(weekno);
    //                    if (FieldActYear != "")
    //                        FieldActYear(date_ew.getFullYear());
    //                }
    //            }
    //        }
    //    }
    //};

    self.CalculateEW = function (FieldDate, FieldAct, FieldActYear) {
        if ($("#" + FieldDate).val() != "") {
            var date_ew = new Date($("#" + FieldDate).datepicker('getDate', {
                dateFormat: date_format_DatePicker
            }));
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
                        var fwky_date_prox = new Date(date_ew.getFullYear() + 1, 0, 1).getDay();

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

    self.HospitalDate.subscribe(function (HospitalDate) {
        self.ChangeHospitalAndDate();
        self.CalculateEW("HospitalDate", self.HospitalEW, self.HospitalYE);
    });

    self.SummayItems = ko.observableArray([]);
    self.SummaryForYearItems = ko.observableArray([]);

    //****
    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7) ? true : false;
    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;

    }, self);

    self.ActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? true : false;
    }, self);

    self.NoActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? false : true;
    }, self);

    self.ActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? true : false;
    }, self);

    self.NoActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? false : true;
    }, self);

    self.ActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? true : false;
    }, self);

    self.NoActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? false : true;
    }, self);

    self.ActiveCOS = ko.computed(function () {
        return (self.UsrCountry() == 9) ? true : false;
    }, self);

    self.NoActiveCOS = ko.computed(function () {
        return (self.UsrCountry() == 9) ? false : true;
    }, self);

    self.ActiveDOM = ko.computed(function () {
        return (self.UsrCountry() == 11) ? true : false;
    }, self);

    self.NoActiveDOM = ko.computed(function () {
        return (self.UsrCountry() == 11) ? false : true;
    }, self);

    self.ActiveCAY = ko.computed(function () {
        return (self.UsrCountry() == 119) ? true : false;
    }, self);

    self.ActiveHON = ko.computed(function () {
        return (self.UsrCountry() == 15) ? true : false;
    }, self);

    self.NoActiveCAY = ko.computed(function () {
        return (self.UsrCountry() == 119) ? false : true;
    }, self);
    //****
    self.PickFirstDay = function (date) {
        var day = date.getDay();
        return [day == 0, " "];
    };

    self.PickLastDay = function (date) {
        var day = date.getDay();
        return [day == 6, " "];
    };

    self.AddHospital = function (element, index, array) {
        self.hospitals.push({ Id: element.Id, Name: element.Name });
    };

    self.ReloadHospitals = function () {
        if (typeof self.selectedCountryId === "undefined") {
            return;
        }
        $.getJSON(app.dataModel.getInstitutionsUrl, { CountryID: self.selectedCountryId() }, function (data, status) {
            self.hospitals(data);
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
    };

    self.AddSummayItem = function (element, index, array) {
        self.SummayItems.push(new SummayItem(element));
    };

    self.AddSummaryForYearItems = function (element, index, array) {
        self.SummaryForYearItems.push(new SummaryYearItem(element));
    };

    // Esto es para calcular los totales de las columnas
    //****
    self.ColETIDenoFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETIDenoFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColETIDenoMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETIDenoMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColETIDenoST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETIDenoST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColETIFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETINumFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColETIMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETINumMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColETINumST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETINumST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColETINumEmerFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETINumEmerFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColETINumEmerMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETINumEmerMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColETINumEmerST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETINumEmerST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColHospFST = ko.computed(function () {
        var numberofitems = 0;
        //console.log("a1a");
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            //console.log("a1b");
            numberofitems += parseInt(r.HospFem());
        });
        //console.log("a1c");
        return parseInt(numberofitems);
    }, self);

    self.ColHospMST = ko.computed(function () {
        var numberofitems = 0;
        //console.log("a2a");
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            //console.log("a2b");
            numberofitems += parseInt(r.HospMaso());
        });
        //console.log("a2c");
        return parseInt(numberofitems);
    }, self);

    self.ColHospTST = ko.computed(function () {
        var numberofitems = 0;
        console.log("a3a");
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            console.log("a3b");
            numberofitems += parseInt(r.HospST());
            //console.log(numberofitems);
        });
        //console.log(numberofitems);
        console.log("a3c");
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColHospSARIFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.HospSARIFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColHospSARIMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.HospSARIMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColHospSARITST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.HospSARIST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColUCIFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.UCIFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColUCIMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.UCIMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColUCITST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.UCIST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColUCISARIFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.UCISARIFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColUCISARIMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.UCISARIMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColUCISARITST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.UCISARIST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColFalleFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.DefFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColFalleMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.DefMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColFalleTST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.DefST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColFalleSARIFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.DefSARIFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColFalleSARIMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.DefSARIMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColFalleSARITST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.DefSARIST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColNeuTST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.NeuST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColCCSARIFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.CCSARIFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColCCSARIMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.CCSARIMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColCCSARITST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.CCSARIST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColVentTST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.VentST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColILICasesST = ko.computed(function () {                      //#### CAFQ: ILI Jamaica
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            if (r['ILICases'] != undefined)
                numberofitems += parseInt(r.ILICases());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColILISamplesTakenST = ko.computed(function () {               //#### CAFQ: ILI Jamaica
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            if (r['ILISamplesTaken'] != undefined)
                numberofitems += parseInt(r.ILISamplesTaken());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColTotalVisitsST = ko.computed(function () {                   //#### CAFQ: ILI Jamaica
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            if (r['TotalVisits'] != undefined)
                numberofitems += parseInt(r.TotalVisits());
        });
        return parseInt(numberofitems);
    }, self);

    //**** Aquí termina el calculo de los totales
    self.MakeValuesOfSummayItems = function () {
        var index;
        var ValuesOfSummayItems = [];
        for (index = 0; index < self.SummayItems().length; ++index) {
            ValuesOfSummayItems.push(self.SummayItems()[index].MakeValueOfSummayItem());
        };
        //console.log(ValuesOfSummayItems);
        return ValuesOfSummayItems;
    };

    self.ChangeHospitalAndDate = function () {
        self.SummayItems.removeAll();
        $("#TotalSummary").hide();
        $("#LabelSummary").hide();
        $("#ButtonSummary").hide();
    };

    self.CancelarItems = function () {
        self.SummayItems.removeAll();

        if ($.isFunction(self.Id)) {
            self.Id = ko.observable("");
            self.Id("");
        }
        else {
            self.Id = "";
        }

        self.selectedHospitalId("");
        self.HospitalDate("");
        self.HospitalEW("");
        self.HospitalYE("");
        $("#TotalSummary").hide();
        $("#LabelSummary").hide();
        $("#ButtonSummary").hide();
    };

    self.GetYearSummaryForYearItems = function () {
        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "") {
            $.postJSON(app.dataModel.getSummaryForYearUrl, { hospitalId: self.selectedHospitalId() })
               .success(function (data, textStatus, jqXHR) {
                   $("#LabelBandeja").show();
                   $("#TotalBandeja").show();
                   self.SummaryForYearItems([]);
                   data.forEach(self.AddSummaryForYearItems);
                   //console.log(data);
                   //self.SummayItems([]);
                   //data.forEach(self.AddSummayItem);                             
               })
               .fail(function (jqXHR, textStatus, errorThrown) {
                   alert(errorThrown);
               })
        }
    };

    self.GetYearSummaryForYearItemsJM = function () {
        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "") {
            $.postJSON(app.dataModel.getSummaryForYearUrlJM, { hospitalId: self.selectedHospitalId() })
               .success(function (data, textStatus, jqXHR) {
                   $("#LabelBandeja").show();
                   $("#TotalBandeja").show();
                   self.SummaryForYearItems([]);
                   data.forEach(self.AddSummaryForYearItems);
                   //console.log(data);
                   //self.SummayItems([]);
                   //data.forEach(self.AddSummayItem);                             
               })
               .fail(function (jqXHR, textStatus, errorThrown) {
                   alert(errorThrown);
               })
        }
    };

    self.GetSummaryExcel = function () {
        //var namevalues = { Report: self.Report(), CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), Month: self.Month(), SE: self.SE(), StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null, EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null, ReportCountry: self.selectedReportCountryId() }
        //if (self.validate() == true)
        var namevalues = { hospitalId: self.selectedHospitalId(), hospitalDate: moment(self.HospitalDate()).format(date_format_ISO), EpiWeek: self.HospitalEW(), EpiYear: self.HospitalYE() };
        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "" && (typeof self.HospitalDate() != "undefined") && self.HospitalDate() != "") {
            window.open(app.dataModel.getSummayDetailsExcel + "?" + $.param(namevalues, true), "_blank");
        } else {
            if ((typeof self.selectedHospitalId() == "undefined") || self.selectedHospitalId() == "") {
                //alert("Seleccionar un establecimiento es requerido");
                if ((self.UsrCountry() == 25) || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Hospital is required");
                else
                    alert("Seleccionar un establecimiento es requerido");
            }

            if ((typeof self.HospitalDate() == "undefined") || self.HospitalDate() == "") {
                if ((self.UsrCountry() == 25) || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Date is required");
                else
                    alert("La fecha es requerida");
            }
        }
    }

    self.GetSummayItems = function () {
        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "" && (typeof self.HospitalDate() != "undefined") && self.HospitalDate() != "") {
            DatetoSend = jQuery.type(self.HospitalDate()) === 'date' ? self.HospitalDate() : parseDate($("#HospitalDate").val(), date_format_)
            $.postJSON(app.dataModel.getSummayDetailsUrl, { hospitalId: self.selectedHospitalId(), hospitalDate: moment(DatetoSend).format(date_format_ISO), EpiWeek: self.HospitalEW(), EpiYear: self.HospitalYE() })
            .success(function (data, textStatus, jqXHR) {
                self.SummayItems([]);
                data.forEach(self.AddSummayItem);
                $("#LabelSummary").show();
                $("#TotalSummary").show();
                $("#ButtonSummary").show();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            })
        } else {

            if ((typeof self.selectedHospitalId() == "undefined") || self.selectedHospitalId() == "") {
                if (self.UsrCountry() == 25 || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Hospital is required");
                else
                    alert("Seleccionar un establecimiento es requerido");
            }

            if ((typeof self.HospitalDate() == "undefined") || self.HospitalDate() == "") {
                if (self.UsrCountry() == 25 || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Date is required");
                else
                    alert("La fecha es requerida");
            }
        }
    };

    self.GetSummayItemsJM = function () {
        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "" && (typeof self.HospitalDate() != "undefined") && self.HospitalDate() != "") {
            DatetoSend = jQuery.type(self.HospitalDate()) === 'date' ? self.HospitalDate() : parseDate($("#HospitalDate").val(), date_format_)
            $.postJSON(app.dataModel.getSummayDetailsUrlJM, { hospitalId: self.selectedHospitalId(), hospitalDate: moment(DatetoSend).format(date_format_ISO), EpiWeek: self.HospitalEW(), EpiYear: self.HospitalYE() })
            .success(function (data, textStatus, jqXHR) {
                self.SummayItems([]);
                data.forEach(self.AddSummayItem);
                $("#LabelSummary").show();
                $("#TotalSummary").show();
                $("#ButtonSummary").show();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            })
        } else {
            if ((typeof self.selectedHospitalId() == "undefined") || self.selectedHospitalId() == "") {
                if (self.UsrCountry() == 25 || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Hospital is required");
                else
                    alert("Seleccionar un establecimiento es requerido");
            }

            if ((typeof self.HospitalDate() == "undefined") || self.HospitalDate() == "") {
                if (self.UsrCountry() == 25 || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Date is required");
                else
                    alert("La fecha es requerida");
            }
        }
    };

    self.SaveSummayItems = function () {
        //console.log(self.MakeValuesOfSummayItems());
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: app.dataModel.saveSummayDetailsUrl,
            data: JSON.stringify({ casesummaryDetails: self.MakeValuesOfSummayItems() }),
            contentType: 'application/json; charset=utf-8',
            success: function (data, textStatus, jqXHR) {
                if ((self.UsrCountry() == 25) || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Data saved...");
                    ///////alert(msgSavedData);
                else
                    alert(data);
                self.CancelarItems();
            },
        })
    };

    self.SaveSummayItemsJM = function () {
        //console.log(self.MakeValuesOfSummayItems());
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: app.dataModel.saveSummayDetailsUrlJM,
            data: JSON.stringify({ casesummaryDetails: self.MakeValuesOfSummayItems() }),
            contentType: 'application/json; charset=utf-8',
            success: function (data, textStatus, jqXHR) {
                if ((self.UsrCountry() == 25) || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Data saved...");
                    ///////alert(msgSavedData);
                else
                    alert(data);
                self.CancelarItems();
            },
        })
    };

    self.showEpiWeek_AM = function (option, item) {
        //console.log("aquí");
        console.log(self.HospitalDate());
    };

    return self;
};

app.addViewModel({
    name: "Summary",
    bindingMemberName: "summary",
    factory: SummaryViewModel
});

/** Bandeja de denominadores **/
function showEpiWeek(data, event) {
    var date = moment.unix(data.StartDateOfWeek).utc().format(moment_date_format);
    var week = data.ColEpiWeek;
    var year = data.ColEpiYear;

    $("#HospitalDate").val(date);
    $("#HospitalDate").change();
    $("#HospitalEW").val(week);
    $("#HospitalEW").change();
    $("#HospitalYE").val(year);
    $("#HospitalYE").change();
    $("#search").click();
}

function showEpiWeekJM(data, event) {
    var date = moment.unix(data.WeekendDate).utc().format(moment_date_format);
    var week = data.ColEpiWeek;
    var year = data.ColEpiYear;

    $("#HospitalDate").val(date);
    $("#HospitalDate").change();
    $("#HospitalEW").val(week);
    $("#HospitalEW").change();
    $("#HospitalYE").val(year);
    $("#HospitalYE").change();
    $("#search").click();
}
/** Fin Bandeja de denominadores **/