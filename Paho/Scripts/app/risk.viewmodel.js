function RiskViewModel(app, dataModel) {
    var self = this;
    var date_format_ = app.dataModel.date_format_;
    var date_format_moment = app.dataModel.date_format_moment;
    var date_format_ISO = app.dataModel.date_format_ISO;
    var date_influenza_1 = new Date();
    var date_influenza_2 = new Date();
    var date_antiviral = new Date();

    self.Id = "";
    self.UsrCountry = ko.observable(app.Views.Home.UsrCountry()); // Pais del usuario logueado
    self.Pregnant = ko.observable();
    self.Pperium = ko.observable();
    self.PregnantWeek = ko.observable("");
    self.Trimester = ko.observable();
    self.Vaccin = ko.observable(null);
    self.RiskFactors = ko.observable("");
    self.VacInfluenza = ko.observable("");
    self.VacInfluenzaDateFirst = ko.observable("");
    self.VacInfluenzaDateSecond = ko.observable("");
    self.VacBcg = ko.observable(null);
    self.VacBcgDosis = ko.observable(null);
    self.VacBcgDate = ko.observable("");
    self.VacNeumococo = ko.observable(null);
    self.VacNeumococoDosis = ko.observable(null);
    self.VacNeumococoDate = ko.observable("");
    self.VacTosFerina = ko.observable(null);
    self.VacTosFerinaDosis = ko.observable(null);
    self.VacTosFerinaDate = ko.observable("");
    self.VacHaemophilus = ko.observable(null);
    self.VacHaemophilusDate = ko.observable("");
    self.VaccinFuente = ko.observable(null);
    self.AntiViral = ko.observable("");
    self.AntiViralDate = ko.observable("");
    self.AntiViralDateEnd = ko.observable("");
    self.AntiViralType = ko.observable(null);
    self.AStartDate = ko.observable("");
    self.OseltaDose = ko.observable("");
    self.AntiViralDose = ko.observable("");

    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7 ) ? true : false;

    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;

    }, self);

    self.PregnantWeek.subscribe(function (NewWeek) {
        if (NewWeek == null || NewWeek == "") return false;
        if (isNaN(parseFloat(NewWeek)) === true) {
            alert("Semana gestacional solo puede ser numerico");
            self.PregnantWeek("");
            $("#PregnantWeek").focus();
        } else {
            if (NewWeek < 40) {
                self.Trimester(Math.round(NewWeek/12));
            } else {
                alert("La semana gestacional no puede ser mayor de 40");
                self.PregnantWeek("");
            }
        }
    });

    self.EnableVacInfluenzaDateFirst = ko.computed(function () {
        var result = self.VacInfluenza() == "1";
        if (!result) self.VacInfluenzaDateFirst("");
        return result;
    }, self);
    self.EnableVacInfluenzaDateSecond = ko.computed(function () {
        var result = self.VacInfluenza() == "1" && !(parseFloat(app.Views.Contact.Age()) >= 5 && app.Views.Contact.AMeasure() == 'Year');
        if (!result) self.VacInfluenzaDateSecond("");
        return result;
    }, self);
    self.EnableVacBcgDate = ko.computed(function () {
        var result = self.VacBcg() == "1";
        if (!result) {
            self.VacBcgDate("");
            self.VacBcgDosis("");
        }
        return result;
    }, self);
    self.EnableVacNeumococoDate = ko.computed(function () {
        var result = self.VacNeumococo() == "1";
        if (!result) {
            self.VacNeumococoDate("");
            self.VacNeumococoDosis("");
        }
        return result;
    }, self);
    self.EnableVacTosFerinaDate = ko.computed(function () {
        var result = self.VacTosFerina() == "1";
        if (!result) {
            self.VacTosFerinaDate("");
            self.VacTosFerinaDosis("");
            ////
        }
        return result;
    }, self);
    self.EnableVacHaemophilusDate = ko.computed(function () {
        var result = self.VacHaemophilus() == "1"; if (!result) self.VacHaemophilusDate(""); return result;
    }, self);
    self.EnableAntiViralDate = ko.computed(function () {
        var result = self.AntiViral() == "1"; if (!result) self.AntiViralDate(""); return result;
    }, self);
    self.EnableAntiViralDateEnd = ko.computed(function () {
        var result = self.AntiViral() == "1"; if (!result) self.AntiViralDateEnd(""); return result;
    }, self);
    self.EnableAntiViralType = ko.computed(function () {
        var result = self.AntiViral() == "1"; 
        if (!result) {
            self.AntiViralType(null);
            self.OseltaDose(null);
            self.AntiViralDose("");
        }
        return result;
    }, self);
    self.EnableOseltamivir = ko.computed(function () {
        var result = self.AntiViralType() == "1"; if (!result) self.OseltaDose(null); return result;
    }, self);
    self.EnableAntiviralDose = ko.computed(function () {
        var result = (self.AntiViralType() == "" || self.AntiViralType() == "5");
        if (!result) self.AntiViralDose("");
        return !result;
    }, self);

    self.HDisease = ko.observable(false);
    self.Diabetes = ko.observable(false);
    self.Neuro = ko.observable(false);
    self.Asthma = ko.observable(false);
    self.Pulmonary = ko.observable(false);
    self.Liver = ko.observable(false);
    self.Renal = ko.observable(false);
    self.Immunsupp = ko.observable(false);
    self.ParaCerebral = ko.observable(false);
    self.Indigena = ko.observable(false);
    self.TrabSalud = ko.observable(false);
    self.Desnutricion = ko.observable(false);
    self.Prematuridad = ko.observable(false);
    self.BajoPesoNacer = ko.observable(false);
    self.AusLacMat = ko.observable(false);
    self.Smoking = ko.observable(false);
    self.Alcohol = ko.observable(false);
    self.DownSyn = ko.observable(false);
    self.Obesity = ko.observable("");
    self.OtherRisk = ko.observable("");
    self.IsFemal = ko.computed(function () {
        return app.Views.Contact.Gender() === "Female" && app.Views.Contact.AMeasure() == "Year" && app.Views.Contact.Age() >= 10 && app.Views.Contact.Age() <= 50;
    });
    self.NotShowQuarter = ko.computed(function () {
        if (self.Pregnant() == "1")
        {
            if (self.UsrCountry() == 7) {
                $("#Primero, #Segundo, #Tercera").prop('disabled', true);
            } else {
                $("#Primero, #Segundo, #Tercera").prop('disabled', false);
            }
            
            return true;
        } else {
            self.Trimester("");
            return false;
        }      
    }, self);

    self.NotShowChilds = ko.computed(function () {
        if (app.Views.Contact.AMeasure() == "Month" || app.Views.Contact.AMeasure() == "Day" || (app.Views.Contact.AMeasure() == "Year" && app.Views.Contact.Age() < 3)) {
            return true;
        } else {
            self.Desnutricion(false);
            self.Prematuridad(false);
            self.BajoPesoNacer(false);
            self.AusLacMat(false);
            return false;
        }
    }, self);

    self.InfVacTR = ko.computed(function () {
        if ((app.Views.Contact.AMeasure() == "Month" && app.Views.Contact.Age() < 6) || app.Views.Contact.AMeasure() == "Day" ) {
            return false;
            self.VacInfluenza("");
            self.VacInfluenzaDateFirst("");
            self.VacInfluenzaDateSecond("");
        } else {
            return true;
        }
    }, self);

    self.DosisAdult = ko.computed(function () {
        if ( app.Views.Contact.AMeasure() == "Year" && app.Views.Contact.Age() > 5) {
            return true;
        } else {
            self.VacInfluenzaDateFirst("");
            return false;
        }
    }, self);

    self.DosisSecondChilds = ko.computed(function () {
        if (app.Views.Contact.AMeasure() == "Month" || app.Views.Contact.AMeasure() == "Day" || (app.Views.Contact.AMeasure() == "Year" && app.Views.Contact.Age() <= 5)) {
            return true;
        } else {
            self.VacInfluenzaDateSecond("");
            return false;
        }
    }, self);

    self.DosisChilds = ko.computed(function () {
        if (app.Views.Contact.AMeasure() == "Month" || app.Views.Contact.AMeasure() == "Day" || (app.Views.Contact.AMeasure() == "Year" && app.Views.Contact.Age() <= 5)) {
            return true;
        } else {
            return false;
        }
    }, self);

    self.RiskFactors.subscribe(function (NewRiskFactors) {
        if (NewRiskFactors == 0 || NewRiskFactors == "" || NewRiskFactors == 9 ) {
            //self.ResetRisk();
            self.HDisease(null);
            self.Diabetes(null);
            self.Neuro(null);
            self.Asthma(null);
            self.Pulmonary(null);
            self.Liver(null);
            self.Renal(null);
            self.Immunsupp(null);
            self.ParaCerebral(null);
            self.Indigena(null);
            self.TrabSalud(null);
            self.Desnutricion(null);
            self.Prematuridad(null);
            self.BajoPesoNacer(null);
            self.AusLacMat(null);
            self.DownSyn(null);
            self.Alcohol(null);
            self.Smoking(null);
            self.Obesity(null);
            $("#HDisease, #Diabetes, #Neuro, #Asthma, #Pulmonary, #Liver, #Renal, #Immunsupp, #ParaCerebral, #Indigena, #TrabSalud, #Desnutricion, #Prematuridad , #BajoPesoNacer, #AusLacMat, #DownSyn, #Alcohol, #Smoking, #combObesity").prop('disabled', true);

        }
        else {
            if ($("#ITy").val() == 2) {
                $("#HDisease, #Diabetes, #Neuro, #Asthma, #Pulmonary, #Liver, #Renal, #Immunsupp, #ParaCerebral, #Indigena, #TrabSalud, #Desnutricion, #Prematuridad , #BajoPesoNacer, #AusLacMat, #DownSyn, #Alcohol, #Smoking, #combObesity").prop('disabled', true);
            } else {
                $("#HDisease, #Diabetes, #Neuro, #Asthma, #Pulmonary, #Liver, #Renal, #Immunsupp, #ParaCerebral, #Indigena, #TrabSalud, #Desnutricion, #Prematuridad , #BajoPesoNacer, #AusLacMat, #DownSyn, #Alcohol, #Smoking, #combObesity").prop('disabled', false);
            }
            
        }
    });
    self.Pregnant.subscribe(function (NewPregnant) {
        console.log(NewPregnant);
        if (NewPregnant != 1) {
            self.Pregnant(null);
            self.Pperium(null);
            self.Trimester(null);
            $("#Pperium, #Primero, #Segundo, #Tercera, #Desconocido").prop('disabled', true);
        }
        else {
            $("#Pperium, #Primero, #Segundo, #Tercera, #Desconocido").prop('disabled', false);
        }
    });
   
    self.ResetRisk = function () {
        self.Id = "";
        self.Pregnant("");
        self.Pperium(null);
        self.Trimester(null);
        self.PregnantWeek(null);
        self.Vaccin(null);
        self.RiskFactors(null);
        self.HDisease(false);
        self.Diabetes(false);
        self.Neuro(false);
        self.Asthma(false);
        self.Pulmonary(false);
        self.Liver(false);
        self.Renal(false);
        self.Immunsupp(false);
        self.ParaCerebral (false);
        self.Indigena(false);
        self.TrabSalud(false);
        self.Desnutricion(false);
        self.Prematuridad(false);
        self.BajoPesoNacer(false);
        self.AusLacMat(false);
        self.Smoking(false);
        self.Alcohol(false);
        self.DownSyn(false);
        self.Obesity("");
        self.OtherRisk("");
        self.AStartDate(null);
        self.VacInfluenza("");
        self.VacInfluenzaDateFirst(null);
        self.VacInfluenzaDateSecond(null);
        self.VacBcg(null);
        self.VacBcgDate(null);
        self.VacBcgDosis("");
        self.VacNeumococo(null);
        self.VacNeumococoDate(null);
        self.VacNeumococoDosis("");
        self.VacTosFerina(null);
        self.VacTosFerinaDate(null);
        self.VacTosFerinaDosis("");
        self.VacHaemophilus(null);
        self.VacHaemophilusDate(null);
        self.VaccinFuente("");
        self.AntiViral("");
        self.AntiViralDate(null);
        self.AntiViralDateEnd(null);
        self.AntiViralType("");
        self.OseltaDose("");
        self.AntiViralDose("");

    };

    self.GetRisk = function (id) {
        self.Id = id;
        console.log("Getting ...");
        console.log(self.Id);
        $.getJSON(app.dataModel.getRiskUrl, { id: id }, function(data, status) {
                self.Vaccin(data.Vaccin);
                self.HDisease(data.HDisease);
                self.Diabetes(data.Diabetes);
                self.Neuro(data.Neuro);
                self.Asthma(data.Asthma);
                self.Pulmonary(data.Pulmonary);
                self.Liver(data.Liver);
                self.Renal(data.Renal);
                self.Immunsupp(data.Immunsupp);

                self.ParaCerebral(data.ParaCerebral);
                self.Indigena(data.Indigena);
                self.TrabSalud(data.TrabSalud);
                self.Desnutricion(data.Desnutricion);
                self.Prematuridad(data.Prematuridad);
                self.BajoPesoNacer(data.BajoPesoNacer);
                self.AusLacMat(data.AusLacMat);

                self.Pregnant(data.Pregnant);
                self.Pperium(data.Pperium);
                self.Trimester(data.Trimester);
                self.PregnantWeek(data.PregnantWeek);
                self.Smoking(data.Smoking);
                self.Alcohol(data.Alcohol);
                self.DownSyn(data.DownSyn);
                self.Obesity(data.Obesity);
                self.OtherRisk(data.OtherRisk);
                self.RiskFactors(data.RiskFactors);

                if (data.VacInfluenza) self.VacInfluenza(data.VacInfluenza);
                if (data.VacInfluenzaDateFirst) self.VacInfluenzaDateFirst(moment(data.VacInfluenzaDateFirst).format(date_format_moment)); else self.VacInfluenzaDateFirst(null);
                if (data.VacInfluenzaDateSecond) self.VacInfluenzaDateSecond(moment(data.VacInfluenzaDateSecond).format(date_format_moment)); else self.VacInfluenzaDateSecond(null);
                
                self.VacBcg(data.VacBcg);
                self.VacBcgDosis(data.VacBcgDosis);
                if (data.VacBcgDate) self.VacBcgDate(moment(data.VacBcgDate).format(date_format_moment)); else self.VacBcgDate(null);
                self.VacNeumococo(data.VacNeumococo);
                self.VacNeumococoDosis(data.VacNeumococoDosis);
                if (data.VacNeumococoDate) self.VacNeumococoDate(moment(data.VacNeumococoDate).format(date_format_moment)); else self.VacNeumococoDate(null);
                self.VacTosFerina(data.VacTosFerina);
                self.VacTosFerinaDosis(data.VacTosFerinaDosis);
                if (data.VacTosFerinaDate) self.VacTosFerinaDate(moment(data.VacTosFerinaDate).format(date_format_moment)); else self.VacTosFerinaDate(null);
                self.VacHaemophilus(data.VacHaemophilus);
                if (data.VacHaemophilusDate) self.VacHaemophilusDate(moment(data.VacHaemophilusDate).format(date_format_moment)); else self.VacHaemophilusDate(null);
                self.VaccinFuente(data.VaccinFuente);
                self.AntiViral(data.AntiViral);
                if (data.AntiViralDate) self.AntiViralDate(moment(data.AntiViralDate).format(date_format_moment)); else self.AntiViralDate(null);
                if (data.AntiViralDateEnd) self.AntiViralDateEnd(moment(data.AntiViralDateEnd).format(date_format_moment)); else self.AntiViralDateEnd(null);
                
                self.AntiViralType(data.AntiViralType);
                self.OseltaDose(data.OseltaDose);
                self.AntiViralDose(data.AntiViralDose);

            })
            .fail(function(jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            });
    };

    self.validate = function (nextStep) {
        var msg = "";
        date_influenza_1 = parseDate($("#VacInfluenzaDateFirst").val(), date_format_);
        date_influenza_2 = parseDate($("#VacInfluenzaDateSecond").val(), date_format_); 
        date_antiviral = parseDate($("#AntiViralDate").val(), date_format_);
        date_antiviral_end = parseDate($("#AntiViralDateEnd").val(), date_format_);
        date_neumococo = parseDate($("#VacNeumococoDate").val(), date_format_);
        date_DOB_dummy = parseDate($("#DOB_dummy").val(), date_format_);
        
        //alert($("#AntiViralDate").val());
        //alert(date_antiviral);
        //alert(parseDate($("#AntiViralDate").val(), date_format_));

        if (self.IsFemal()) {
//            alert($("input[name='Pregnant']:checked").val());
            if ($("input[name='Pregnant']:checked").val() == "")
                msg += "\n" + "Variable embarazo es requerida";

            if (self.Pregnant() == "1" && !self.Trimester())
                msg += "\n" + "Variable trimestre de embarazo es requerida";
        }

        if ((!self.VaccinFuente() || self.VaccinFuente() == "0") && self.UsrCountry() != 25)
            msg += "\n" + "Fuente de la información de la vacuna es requerida";


        // Validacion de Fecha de vacunas contra fecha de nacimiento

        if (date_neumococo != null)
            if (moment(date_neumococo).isBefore(moment(date_DOB_dummy))) {
                msg += "\n" + "La fecha de la vacuna de neumococo no puede ser menor a fecha de nacimiento";
                $("#VacNeumococoDate").focus();
            }

        if ((app.Views.Contact.AMeasure() == "Month" && app.Views.Contact.Age() > 5) && app.Views.Contact.AMeasure() != "Day") {

            if (!self.VacInfluenza() && !self.UsrCountry() == 7)
                msg += "\n" + "Información de la vacuna de Influenza es requerida";

            if (date_influenza_1 != null && self.VacInfluenza() == "1")
                if (moment(date_influenza_1).isBefore(moment(date_DOB_dummy))) {
                    msg += "\n" + "La fecha de la vacuna de Influenza no puede ser menor a fecha de nacimiento";
                    $("#VacInfluenzaDateFirst").focus();
                }

            if (date_influenza_1 != null && date_influenza_2 != null) {
                if (moment(date_influenza_2).isBefore(moment(date_DOB_dummy))) {
                    msg += "\n" + "La fecha de la vacuna de segunda dosis de Influenza no puede ser menor a fecha de nacimiento";
                    $("#VacInfluenzaDateSecond").focus();
                }
                if (moment(date_influenza_2).isBefore(moment(date_influenza_1))) {
                    msg += "\n" + "La fecha de la vacuna de segunda dosis de Influenza no puede ser menor a fecha de la primera dosis de Influenza";
                    $("#VacInfluenzaDateSecond").focus();
                }
            }
        }

        //Las fechas de las vacunas fueron desactivadas por petición de CRI
        
        //if (self.EnableVacInfluenzaDateFirst()){
        //    if ($("#VacInfluenzaDateFirst").val() == "")
        //        msg += "\n" + "Fecha de primera dosis de influenza es requerida";
        //    if ($("#VacInfluenzaDateFirst").val() != "" && !moment(moment(date_influenza_1).format(date_format_moment), [date_format_moment], true).isValid())
        //        msg += "\n" + "Fecha de primera dosis de influenza es inválida";
        //}

        //if (self.EnableVacInfluenzaDateSecond()) {
        //    if ($("#VacInfluenzaDateSecond").val() == "")
        //        msg += "\n" + "Fecha de segunda dosis de influenza es requerida";
        //    if ($("#VacInfluenzaDateSecond").val() != "" && !moment(moment(date_influenza_2).format(date_format_moment), [date_format_moment], true).isValid())
        //        msg += "\n" + "Fecha de segunda dosis de influenza es inválida";
        //}

        //if (!self.VacBcg())
        //    msg += "\n" + "Información para BCG es requerida";

        //if (self.EnableVacBcgDate()) {
        //    if (!self.VacBcgDate())
        //        msg += "\n" + "Fecha de aplicación de BCG es requerida";
        //    if (self.VacBcgDate() && !moment(moment(self.VacBcgDate()).format("DD/MM/YYYY"), ["DD/MM/YYYY"], true).isValid())
        //        msg += "\n" + "Fecha de aplicación de BCG es inválida";
        //}

        //if (!self.VacNeumococo())
        //    msg += "\n" + "Información de aplicación de Neumococo es requerida";

        //if (self.EnableVacNeumococoDate()) {
        //    if (!self.VacNeumococoDate())
        //        msg += "\n" + "Fecha de aplicación de Neumococo es requerida";
        //    if (self.VacNeumococoDate() && !moment(moment(self.VacNeumococoDate()).format("DD/MM/YYYY"), ["DD/MM/YYYY"], true).isValid())
        //        msg += "\n" + "Fecha de aplicación de Neumococo es inválida";
        //}
        
        //if (!self.VacTosFerina())
        //    msg += "\n" + "Información de aplicación de Tos Ferina es requerida";

        //if (self.EnableVacTosFerinaDate()) {
        //    if (!self.VacTosFerinaDate())
        //        msg += "\n" + "Fecha de aplicación de Tos Ferina es requerida";
        //    if (self.VacTosFerinaDate() && !moment(moment(self.VacTosFerinaDate()).format("DD/MM/YYYY"), ["DD/MM/YYYY"], true).isValid())
        //        msg += "\n" + "Fecha de aplicación de Tos Ferina es inválida";
        //}

        //if (!self.VacHaemophilus())
        //    msg += "\n" + Información de aplicación de Haemophilus es requerida";

        //if (self.EnableVacHaemophilusDate()) {
        //    if (!self.VacHaemophilusDate())
        //        msg += "\n" + "Fecha de aplicación de Haemophilus es requerida";
        //    if (self.VacHaemophilusDate() && !moment(moment(self.VacHaemophilusDate()).format("DD/MM/YYYY"), ["DD/MM/YYYY"], true).isValid())
        //        msg += "\n" + "Fecha de aplicación de Haemophilus es inválida";
        //}

        if (!self.AntiViral())
            msg += "\n" + msgValidationAntiviralTreatment;

        if (self.EnableAntiViralDate() && app.Views.Contact.SurvSARI() == true) {
            if (date_antiviral == null)
                msg += "\n" + "Fecha de tratamiento antiviral es requerida";
            if (date_antiviral != null && !moment(moment(date_antiviral).format(date_format_moment), [date_format_moment], true).isValid())
                msg += "\n" + "Fecha de tratamiento antiviral es inválida";
        }

        if (app.Views.Contact.ActiveBOL() && !self.AntiViralType() && self.AntiViral() == "1" ) {
            msg += "\n" + "Información de tipo de tratamiento antiviral es requerida";
        }

        if (!self.RiskFactors() || self.RiskFactors == "")
            msg += "\n" + msgValidationRiskFactor;

        if (msg !== "") { alert(msgValidationMedicalHistory + msg); $('#tabs').tabs({ active: 2 }); return false; }
        if (nextStep != null) nextStep();
        return true;
    };

    self.Save = function () {
        app.Views.Home.ValidateAll();
    };

    self.Cancel = function () {
        if (confirm("Usted esta apunto de salir del registro, está seguro?")) {
            app.Views.Home.CancelEdit();
        }
    };

    self.SaveRisk = function (nextStep) {
        date_influenza_1 = parseDate($("#VacInfluenzaDateFirst").val(), date_format_);
        date_influenza_2 = parseDate($("#VacInfluenzaDateSecond").val(), date_format_);
        date_neumococo = parseDate($("#VacNeumococoDate").val(), date_format_);
        date_tosferina = parseDate($("#VacTosFerinaDate").val(), date_format_);
        date_haemophilus = parseDate($("#VacHaemophilusDate").val(), date_format_);
        
        date_antiviral = parseDate($("#AntiViralDate").val(), date_format_);
        date_antiviral_end = parseDate($("#AntiViralDateEnd").val(), date_format_);
        

         $.post(app.dataModel.saveRiskUrl,
            {
                Id: self.Id,
                Vaccin: self.Vaccin(),
                HDisease: self.HDisease(),
                Diabetes: self.Diabetes(),
                Neuro: self.Neuro(),
                Asthma: self.Asthma(),
                Pulmonary: self.Pulmonary(),
                Liver: self.Liver(),
                Renal: self.Renal(),
                Immunsupp: self.Immunsupp(),
                Pregnant: self.Pregnant(),
                Pperium: self.Pperium(),

                ParaCerebral: self.ParaCerebral(),
                Indigena: self.Indigena(),
                TrabSalud: self.TrabSalud(),
                Desnutricion: self.Desnutricion(),
                Prematuridad: self.Prematuridad(),
                BajoPesoNacer: self.BajoPesoNacer(),
                AusLacMat: self.AusLacMat(),
                
                Trimester: self.Trimester(),
                PregnantWeek: self.PregnantWeek(),
                Smoking: self.Smoking(),
                Alcohol: self.Alcohol(),
                DownSyn: self.DownSyn(),
                Obesity: self.Obesity(),
                OtherRisk: self.OtherRisk() == null ? "" : self.OtherRisk().toLocaleUpperCase(),
                VacInfluenza: self.VacInfluenza(),
                VacInfluenzaDateFirst: $("#VacInfluenzaDateFirst").val() == "" ? null : moment(date_influenza_1).format(date_format_ISO),
                VacInfluenzaDateSecond: $("#VacInfluenzaDateSecond").val() == "" ? null : moment(date_influenza_2).format(date_format_ISO),
                VacBcg: self.VacBcg(),
                VacBcgDate: $("#VacBcgDate").val() == "" ? null : moment(self.VacBcgDate()).format(date_format_ISO),
                VacBcgDosis: self.VacBcgDosis(),
                VacNeumococo: self.VacNeumococo(),
                VacNeumococoDate: $("#VacNeumococoDate").val() == "" ? null : moment(date_neumococo).format(date_format_ISO),
                VacNeumococoDosis: self.VacNeumococoDosis,
                VacTosFerina: self.VacTosFerina(),
                VacTosFerinaDate: $("#VacTosFerinaDate").val() == "" ? null : moment(date_tosferina).format(date_format_ISO),
                VacTosFerinaDosis: self.VacTosFerinaDosis,
                VacHaemophilus: self.VacHaemophilus(),
                VacHaemophilusDate: $("#VacHaemophilusDate").val() == "" ? null : moment(date_haemophilus).format(date_format_ISO),
                VaccinFuente: self.VaccinFuente(),
                AntiViral: self.AntiViral(),
                AntiViralDate: $("#AntiViralDate").val() == "" ? null : moment(date_antiviral).format(date_format_ISO),
                AntiViralDateEnd: $("#AntiViralDateEnd").val() == "" ? null : moment(date_antiviral_end).format(date_format_ISO),
                AntiViralType: self.AntiViralType(),
                OseltaDose: self.OseltaDose(),
                AntiViralDose: self.AntiViralDose(),
                RiskFactors: self.RiskFactors()
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
    name: "Risk",
    bindingMemberName: "risk",
    factory: RiskViewModel
});