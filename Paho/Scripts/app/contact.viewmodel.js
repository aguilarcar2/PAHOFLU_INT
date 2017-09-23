function ContactViewModel(app, dataModel) {
    var self = this;
    var date_format_ = app.dataModel.date_format_;
    var date_format_moment = app.dataModel.date_format_moment;
    var date_format_ISO = app.dataModel.date_format_ISO;
    var date_hospital = new Date();
    var date_reg_date = new Date();
    var date_DOB = new Date();
    //var date_DOB_dummy = new Date();

    // autocomplete
    self.firstName = ko.observable();
    self.people = ko.observableArray();
    self.hasGet = ko.observable(false);
    self.hasHospitalID = ko.observable("");

    self.FeverDateDummy = ko.observable("");

    self.Id = ko.observable("");
    self.UsrCountry = ko.observable(app.Views.Home.UsrCountry()); // Pais del usuario logueado

    // Campos para flujo
    self.DataStatement = ko.observable($("#o_S").val()); // Estado del documento 1=Ingresando datos sin enviar, 2=Enviado al siguiente estado del flujo
    self.flow_record = ko.observable(0);
    self.flow_institution = ko.observable(0);
    self.flow_max = ko.observable(1);
    self.flow_close_case = ko.observable(99);  // Este valor siempre sera 99 cuando un caso este cerrado
    self.flow_open_always = ko.observable(false); // Esta variable es solo para los laboratorios que estan marcados en la tabla de configuracion de flujo

    self.SavePrev_var = ko.observable(true);
    self.IsSurv = ko.observable("");
    self.SurvInusual = ko.observable(false);
    self.servicesArr = ko.observableArray(institutions);
    self.selectedServiceId = ko.observable("");
    self.Brote = ko.observable(false);
    self.region_institucional = ko.observable("");
    self.region_salud = ko.observable("");
    self.region_pais = ko.observable("");
    self.hospitalName = ko.observable("");
    self.VisHopName = ko.computed(function () {
        if (self.hospitalName != "") {
            return true;
        } else {
            return false;
        }
    }, self);
    self.LName1 = ko.observable("");
    self.LName2 = ko.observable("");
    self.VisLName2Mand = ko.computed(function () {
        return (self.UsrCountry() == 9) ? true : false;

    }, self);
    self.FName1 = ko.observable("");
    self.FName2 = ko.observable("");
    self.DocumentType = ko.observable("");
    self.NoExpediente = ko.observable("");
    self.NationalId = ko.observable("");
    self.DOB = ko.observable(null);
    self.DOB_dummy = ko.observable(null);
    self.Age = ko.observable("");
    self.AMeasure = ko.observable("");
    self.AMeasure.subscribe(function(newmeasure){
        //self.CalculateDOB();
    });
    self.ActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? true : false;
    }, self);
    self.NoActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? false : true;
    }, self);


    self.selectedNationalityID = ko.observable("");
    self.selectedNativepeopleID = ko.observable("");

    self.ShowOnlyAdult = ko.computed(function () {
            return (self.AMeasure() == "Month" || self.AMeasure() == "Day" || (self.AMeasure() == "Year" && self.Age() < 15)) ? false : true;
    }, self);

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

    // Validacion del RUT para chile
    self.esrut = function (rut) {
        //if (rut == 0) {
        //    alert("Debe ingresar un RUT");
        //    document.form1.Tx_RUN2.focus();//me posiciono en el campo nuevamente
        //    return false;
        //} else {
        if (rut.search("-") <= 0) {
            alert(msgValidationRUN + rut + msgValidationRUNDashedPart2);
            self.NoExpediente("");//limpio el valor del campo rut
            $("#NoExpediente").focus();//me posiciono en el campo nuevamente
            return false;
        }
            if (self.EsRut(rut) == false) {
                alert(msgValidationRUN + rut + msgValidationRUNValidPart2);
                self.NoExpediente("");//limpio el valor del campo rut
                $("#NoExpediente").focus();//me posiciono en el campo nuevamente
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

    self.displayService = ko.computed(function () {
        console.log(self.servicesArr().length)
        return (self.servicesArr().length > 1) ? true : false;
    }, self);

    self.DocumentType.subscribe(function (NewRUN) {
        if (self.UsrCountry() != 25) {//Solo ejecuta este subscribe si el país es diferente de Suriname
            self.NoExpediente("");
        }

    });

    self.NoExpediente.subscribe(function (NewRUN) {

        if (self.UsrCountry() == 9 && NewRUN != "") {
            var string_length = NewRUN.length;
            //console.log(isNaN(self.NoExpediente()));
            //console.log(self.NoExpediente());
            //console.log(/^[1-9]\d$/.test(self.NoExpediente()));
            //|| /^[0-9]\d$/.test(self.NoExpediente()) == false
            if (self.DocumentType() == 1 && (string_length != 9 || isNaN(self.NoExpediente()) == true )) {
                alert(msgValidationDocumentFormat);
                self.NoExpediente("");
            } else if (self.DocumentType() == 2 && string_length != 10) {
                alert(msgValidationInsuredNumber);
                self.NoExpediente("");
            } else if (self.DocumentType() == 3 && string_length != 14) {
                alert(msgValidationInternalDoc);
                self.NoExpediente("");
            }

        }

        if (self.UsrCountry() == 7 && NewRUN != "" && self.DocumentType() == 4 && NewRUN != "9.999.999" && $("#ITy").val() != "2") {
            self.esrut(NewRUN);
        }

    });

    //self.NoExpediente.subscribe(function (NewDT) {

    //    if (self.UsrCountry() == 7 && self.NoExpediente() != "" && NewDT == 4 && self.NoExpediente() != "9.999.999" && $("#ITy").val() != "2") {
    //        self.esrut(NewRUN);
    //    }

    //});

    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7) ? true : false;
        
    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;

    }, self);

    self.EnableCR = ko.computed(function () {
        return (self.UsrCountry() == 9) ? true : false;

    }, self);

    self.Age.subscribe(function (newage) {
        //if (self.AMeasure() != "" || self.AMeasure() != "Year") self.CalculateDOB();
        if (self.AMeasure() == "") self.AMeasure("Year");
    });
    self.CalculateDOB = function () {
        if (self.AMeasure() == "") self.AMeasure("Year");
        var measure = self.AMeasure() == "Year" ? "years" : (self.AMeasure() == "Month" ? 'months' : 'days');
        var dob = new Date(moment().add(self.Age() * -1, measure));
            //.format(date_format_moment);
        if ((self.Age() && self.Age() != "") && (!self.DOB() || self.DOB() == "" || $("#DOB").prop("disabled"))) {
            $("#DOB").prop("disabled", true);
            self.DOB_dummy(dob);
        }
    };
    self.DOB.subscribe(function (nedob) {
        if ($("#DOB").prop("disabled")) return;
        if (self.hasGet() == true) return;
        if (self.DOB() && self.DOB() != "") {
            console.log(self.DOB());
            console.log(moment().diff(self.DOB(), 'years'));
            if (moment().diff(self.DOB(), 'years') > 0) {
                self.Age(moment().diff(self.DOB(), 'years'));
                self.AMeasure("Year");
                
            } else if (moment().diff(self.DOB(), 'months') > 0) {
                self.Age(moment().diff(self.DOB(), 'months'));
                self.AMeasure("Month");
                
            } else {
                self.Age(moment().diff(self.DOB(), 'days'));
                self.AMeasure("Day");
                
            }
            $("#Age").prop("disabled", true);
            $("#AMeasure").prop("disabled", true);
            self.DOB_dummy(self.DOB);
        } else {
            self.Age("");
            self.AMeasure("");
            $("#Age").prop("disabled", false);
            $("#AMeasure").prop("disabled", false);
        }
    });
    self.Gender = ko.observable("");
    self.HospitalDate = ko.observable("");
    self.RegDate = ko.observable(new Date());
    self.FullName = ko.computed(function () {
        return self.FName1() + " " + (self.FName2() || "") + " " + self.LName1() + " " + (self.LName2() || "");
    });
    self.AgeGroup = ko.computed(function () {      
        if (!(self.Age() || self.DOB())) return "";
        if (self.UsrCountry() != 7) {
            if (self.AMeasure() == "Day" || (self.AMeasure() == "Month" && self.Age() <= 12)) return "Niños menores de 2 años"
            if (self.Age() < 2) return "Niños menores de 2 años";
            if (self.Age() < 5) return "2 a 4 años";
            if (self.Age() < 15) return "5 a 14 años";
            if (self.Age() < 34) return "15 a 34 años";
            if (self.Age() < 65) return "35 a 64 años";
            return "65 años y más";
        } else
        {
            if (self.AMeasure() == "Day" || (self.AMeasure() == "Month" && self.Age() <= 12)) return "Niños menores de 2 años"
            if (self.Age() < 2) return "Niños menores de 2 años";
            if (self.Age() < 5) return "2 a 4 años";
            if (self.Age() < 20) return "5 a 19 años";
            if (self.Age() < 40) return "20 a 39 años";
            if (self.Age() < 60) return "40 a 59 años";
            return "60 años y más";
        }
        
    });

    self.NotShowHospitalization = ko.computed(function () {
        if (self.IsSurv() == "1")
        {
            return true;
        } else {
            return false;
        }      
    }, self);

    self.SurvSARI = ko.computed(function () {
        if (self.IsSurv() != "2") {
            return true;
        } else {
            return false;
        }
    }, self);

    self.SurvSARICHI = ko.computed(function () {
        if (self.IsSurv() != "2" && self.UsrCountry() == 7) {
            return true;
        } else {
            return false;
        }
    }, self);

    self.NacionalidadCHI = ko.computed(function () {
        if (self.selectedNationalityID() == "7") {
            return true;
        } else {
            self.selectedNativepeopleID("");
            return false;
        }
    }, self);

    self.SurvSARICR = ko.computed(function () {
        if (self.IsSurv() != "2" && self.UsrCountry() == 9) {
            return true;
        } else {
            return false;
        }
    }, self);

    self.SurvILI = ko.computed(function () {
        if (self.IsSurv() == "2") {
            return true;
        } else {
            return false;
        }
    }, self);

    self.SurvILICR = ko.computed(function () {
        if (self.IsSurv() == "2" && self.UsrCountry() == 9) {
            return true;
        } else {
            return false;
        }
    }, self);

    self.SurvILICH = ko.computed(function () {
        if (self.IsSurv() == "2" && self.UsrCountry() == 7) {
            return true;
        } else {
            return false;
        }
    }, self);
 
    self.AddFluCase = function () {

        app.Views.Home.ResetFluCase();
    };

    self.Borrar = function () {
        if (confirm(msgConfirmationDeleteRecord)) {
            $.getJSON(app.dataModel.deletetUrl, { id: self.Id() }, function(data, status) {
                    console.log(data);
                    app.Views.Home.CancelEdit();
                })
                .fail(function(jqXHR, textStatus, errorThrown) {
                    //alert(errorThrown);
                    console.log(errorThrown);
                });
        }
    };

    self.ResetContact = function () {

        //self.Id = ko.observable("");
        if ($.isFunction(self.Id)) {
            self.Id = ko.observable("");
            self.Id("");
        }
        else {
            self.Id = "";
        }
        self.Id(null);
        self.IsSurv("");
        self.SurvInusual(false);
        self.Brote(false);
        self.hospitalName("");
        self.LName1("");
        self.LName2("");
        self.FName1("");
        self.FName2("");
        self.DocumentType("");
        self.NoExpediente("");
        self.NationalId("");
        self.DOB(null);
        self.DOB_dummy(null);
        self.Age("");
        self.AMeasure("");
        self.Gender("");
        self.HospitalDate(null);
        self.RegDate(new Date());
        self.selectedNationalityID("");
        self.selectedNativepeopleID("");
        self.hasHospitalID("");
        // esto es para resetear el flujo
        self.flow_institution(0);
        self.flow_record(0);
        self.DataStatement(1);
    };

    self.FormatDateWrite = function (node_date) {
        //var self_date = node_date
        //console.log(self_date);
        console.log(node_date);

    };

    self.EnableSavePrev = ko.computed(function () {

        if (self.UsrCountry() == 7) {

            if (self.IsSurv() && self.DocumentType() && self.NoExpediente() != "" && self.LName1() != "" && self.FName1() != "" && self.DOB() != "" && self.Gender()
            && app.Views.GEO.selectedCountryId() != "" && app.Views.GEO.selectedAreaId() != "" && app.Views.GEO.selectedStateId() != "" && app.Views.GEO.selectedNeighborhoodId() != ""
            && app.Views.Risk.VaccinFuente() && app.Views.Risk.VacInfluenza() && app.Views.Risk.AntiViral() && app.Views.Risk.RiskFactors()
            && app.Views.Hospital.FeverDate() != "" && app.Views.Hospital.AntecedentesFiebre() && app.Views.Hospital.IsSample()) {
                self.SavePrev_var(false);
                if ($("#ITy").val() == "2") {
                    //$("#tab-lab :input").prop('disabled', false);
                }
                return false
            } else {
                if (self.IsSurv() && self.DocumentType() && self.NoExpediente() != "" && self.LName1() != "" && self.FName1() != "" && self.DOB() != "" && self.Gender() && self.HospitalDate() != ""
                    && app.Views.GEO.selectedCountryId() != "" && app.Views.Hospital.FeverDate() != "" && app.Views.Hospital.IsSample() && $("#ITy").val() == "2") {
                    //$("#tab-lab :input").prop('disabled', false);
                } else {
                    //$("#tab-lab :input").prop('disabled', true);
                }
                self.SavePrev_var(true);
                
                return true;
            }
        } else {
            self.SavePrev_var(false);
            return false;
        }
        
    }, self);

    self.OnlyCR = ko.computed(function () {
        if (self.UsrCountry() == 9) {
            return true;
        } else {
            return false;
        }
    }, self);

    self.validate = function (nextStep) {
        var msg = "";

        date_hospital = parseDate($("#HospitalDate").val(), date_format_);
        date_reg_date = parseDate($("#RegDate").val(), date_format_);
        date_inicio_sintomas = parseDate($("#FeverDate").val(), date_format_);
        date_DOB = typeof (self.DOB()) == "object" ? self.DOB() : parseDate(self.DOB(), date_format_);

        if (date_inicio_sintomas != null)
            if (moment(date_hospital).isBefore(moment(date_inicio_sintomas))) {
                msg += "\n" + msgValidationOnsetFeverDate;
                $("#HospitalDate").focus();
            }
                

        if (self.IsSurv() == "")  
            msg += "\n" + msgValidationSurvType;

        
        if (!self.DocumentType() && self.UsrCountry() != 25)//agregado el 25 para que esta validación ignore SURINAME
            msg += "\n" + msgValidationTypeOfDocument;

        if (!self.NoExpediente()) {
            msg += "\n" + msgValidationDocumentIDRequired;
        }
            

        if ($("#HospitalDate").val() == "")
            msg += "\n" + msgValidationNotificationDateRequired;
        if ($("#HospitalDate").val() != "" && !moment(moment(date_hospital).format(date_format_moment), [date_format_moment], true).isValid())
            msg += "\n" + msgValidationNotificationDate;
        //if ($("#RegDate").val() == "")
        //    msg += "\n" + "Fecha de notificación es requerido";
        if ($("#RegDate").val() != "" && !moment(moment(date_reg_date).format(date_format_moment), [date_format_moment], true).isValid())
            msg += "\n" + msgValidationRecordDate;
        if (date_hospital != null && date_reg_date != null && moment(date_hospital).isAfter(moment(date_reg_date)))
        {
            msg += "\n" + msgValidationRecordDateLater;
            $("#HospitalDate").focus();
        }
            
        if (!self.LName1())
            msg += "\n" + msgValidationFirstLastNameRequired;
        if (!self.LName2() && self.UsrCountry == 9 )
            msg += "\n" + msgValidationSecondLastNameRequired;
        if (!self.FName1())
            msg += "\n" + msgValidationFirstNameRequired;


//        if (!self.DOB())
//            msg += "\n" + "Date of birth is invalid";
//        if (self.DOB() && !moment(moment(self.DOB()).format("DD/MM/YYYY"), ["DD/MM/YYYY"], true).isValid())
//            msg += "\n" + "Date of birth is invalid";
//        if (self.HospitalDate() && self.DOB() && moment(self.HospitalDate()).isBefore(moment(self.DOB())))
//            msg += "\n" + "Date of birth is posterior to the Capture Date";
//        if (self.RegDate() && self.DOB() && moment(self.RegDate()).isBefore(moment(self.DOB())))
//            msg += "\n" + "Fecha de nacimiento no puede ser posterior a la de notificación ";
        if (!self.Gender())
            msg += "\n" + msgValidationSexRequired;

        // Validaciones Chile

        if (self.UsrCountry() == 7) {
            if (date_DOB == null || date_DOB == "")
                msg += "\n" + msgValidationTypeDOB;
            if (moment(date_DOB).isAfter(moment(date_hospital)))
                msg += "\n" + msgValidationDOBLater;
        }
                

        //if (msg !== "") { alert('DAT. VIGILANCIA:' + msg); $('#tabs').tabs({ active: 0 }); return false; }
        if (msg !== "") { alert(msgValidationSurvData + msg); $('#tabs').tabs({ active: 0 }); return false; }
        if (nextStep != null) nextStep();
        return true;
    };

    self.Cancel = function () {
        if (confirm(msgConfirmationExitRecord)) {
            app.Views.Home.CancelEdit();
        }
    };

    self.GetPatient = function () {

        if ((self.DocumentType() == "" && self.UsrCountry() != 25) || self.NoExpediente() == "")
        {
            alert(msgConfirmationDocumentTypeNumberRequired);
        } else {
            $.getJSON(app.dataModel.getPatientInformationUrl, { DTP: self.DocumentType(), DNP: self.NoExpediente() }, function (data, status) {
                if (data.length <= 0) {
                    alert(msgValidationPacientInfoMissing);
                }
                else {
                    self.FName1(data[0].nombre1);
                    self.FName2(data[0].nombre2);
                    self.LName1(data[0].apellido1);
                    self.LName2(data[0].apellido2);
                    var DOB_dumm = new Date(data[0].DOB);
                    DOB_dumm.setHours(DOB_dumm.getHours() + 8);
                    self.DOB(DOB_dumm);
                    //console.log(moment.utc(moment(data[0].DOB.replace(/"/g, '').replace(/[*?^${}|[\]\\]/g, '').replace('-0600', '')).format(date_format_moment)).toDate())  // seconds
                    self.Gender(data[0].sexo);
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(msgValidationServerError);
                //console.log(errorThrown);
            });
        }
            
        
    };

    self.GetContact = function (id) {
        self.Id(id);
        self.hasGet(true);
        $("#RecordNumber").text(id);
        
        $.getJSON(app.dataModel.getContactUrl, { id: id }, function (data, status) {
                self.IsSurv(data.Surv);
                self.SurvInusual(data.SurvInusual);
                self.Brote(data.Brote);
                self.hospitalName(data.hospitalName);
                self.region_institucional(data.region_institucional);
                self.region_salud(data.region_salud);
                self.region_pais(data.region_salud);
                self.DocumentType(data.DocumentType);
                self.selectedServiceId(data.hospitalIDRecord);
                self.LName1(data.LName1);
                self.LName2(data.LName2);
                self.FName1(data.FName1);
                self.FName2(data.FName2);
                self.NoExpediente(data.NoExpediente);
                self.NationalId(data.NationalId);
                if (data.DOB) {
                    self.DOB(moment(data.DOB).format(date_format_moment));
                    self.DOB_dummy(moment(data.DOB).format(date_format_moment));
                } else {
                    self.DOB(null);
                    self.CalculateDOB();
                }
                self.Age(data.Age);
                self.AMeasure(data.AMeasure);
                if (data.Gender)
                    self.Gender(data.Gender);
                self.HospitalDate(moment(data.HospitalDate).format(date_format_moment));
                self.RegDate(moment(data.RegDate).format(date_format_moment));
                console.log(data.RegDate);
                self.selectedNationalityID(data.nationality);
                self.selectedNativepeopleID(data.nativepeople);
                self.hasHospitalID(data.hospitalIDRecord);
                self.DataStatement(data.DataStatement);
                self.flow_record(data.flow_record);
                self.flow_institution(data.flow_institution);
                self.flow_max(data.flow_max);
                self.flow_open_always(data.flow_open_always);
                $("#o_S").val(data.DataStatement);
                
                $("button[id^='Siguiente']").attr("disabled", false);
                $("button[id^='Atras']").attr("disabled", false);
            })
            .fail(function(jqXHR, textStatus, errorThrown) {
                //alert(errorThrown);
                console.log(errorThrown);
            });

        self.hasGet(false);
    };

    self.Save = function (option_Save, object_Save) {

        app.Views.Home.ValidateAll(option_Save);

    };

    self.SavePrev = function () {
        app.Views.Contact.validate(function () {
            app.Views.Home.SaveAll();
        });
    };

    self.SaveContact = function (nextStep) {
        date_hospital = parseDate($("#HospitalDate").val(), date_format_);
        date_reg_date = parseDate($("#RegDate").val(), date_format_);
        date_DOB = parseDate($("#DOB").val(), date_format_);
        date_fever_dummy = parseDate($("#FeverDate").val(), date_format_);
        if (self.DocumentType() == "" && self.UsrCountry() == 25) {//sirve para que en el caso de Suriname, el DocumentType se llene con un valor predeterminado            
            self.DocumentType(8);
        }
         $.post(app.dataModel.saveContactUrl,
            {
                id: self.Id(),
                Surv: self.IsSurv(),
                SInusual: self.SurvInusual(),
                Brote: self.Brote(),
                LName1: self.LName1() == null ? "" : self.LName1().toLocaleUpperCase(),
                LName2: self.LName2() == null ? "" : self.LName2().toLocaleUpperCase(),
                FName1: self.FName1() == null ? "" : self.FName1().toLocaleUpperCase(),
                FName2: self.FName2() == null ? "" : self.FName2().toLocaleUpperCase(),
                DocumentType: self.DocumentType(),
                NationalId: self.NationalId() == null ? "" : self.NationalId().toLocaleUpperCase(),
                DOB: moment(date_DOB).format(date_format_ISO),
                Age: self.Age(),
                AMeasure: self.AMeasure(),
                Gender: self.Gender(),
                HospitalDate: moment(date_hospital).format(date_format_ISO),
                RegDate: moment(date_reg_date).format(date_format_ISO),
                HospitalId: self.selectedServiceId() > 0 ? self.selectedServiceId() : app.Views.Home.selectedInstitutionId(),
                nativepeople: self.selectedNativepeopleID(),
                nationality: self.selectedNationalityID(),
                NoExpediente: self.NoExpediente() == null ? "" : self.NoExpediente().toLocaleUpperCase(),
                DateFeverDummy: moment(date_fever_dummy).format(date_format_ISO)
            },
            function (data) {
                if (typeof data != "number") {
                    alert(data);
                    self.displayFilters(true);
                    self.editMode(false);
                    self.showGrid(true);
                    self.ReloadFluCases();
                } else {
                    app.Views.Contact.Id(data);
                    app.Views.GEO.Id = data;
                    app.Views.Hospital.Id = data;
                    app.Views.Risk.Id = data;
                    app.Views.Lab.Id = data;
                    if (nextStep) nextStep();
                }
                
            },
            "json"
         );
        return true;
    };
    // Revision del flujo si es epidemiologico o laboratorio
    self.Flow_Local_Institution_Lab = ko.computed(function () {
        console.log("Contact epi - frecord_lab -- " + self.flow_record() + ", finstitution_lab -- " + self.flow_institution() + ", dataStatement_lab -- " + self.DataStatement() + ", userRole " + app.Views.Home.UserRole() + ", Inst" + $("#ITy").val() + ", OpenAlways" + self.flow_open_always());
        if ((self.flow_record() == (self.flow_institution() - 1) && (self.DataStatement() == 2 || self.DataStatement() == null || self.flow_open_always() == true)) || (self.flow_record() == self.flow_institution() && (self.DataStatement() == 1 || self.DataStatement() == null || self.flow_open_always() == true)) || (self.flow_close_case() == 99 && self.flow_open_always() == true)) {
            return true;
        }
        else {
            return false;
        }

    }, self);

    self.Flow_Local_Institution_Epi = ko.computed(function () {

        //console.log("Contact epi - frecord_epi -- " + self.flow_record() + ", finstitution_epi -- " + self.flow_institution() + ", dataStatement_epi -- " + self.DataStatement() + ", userRole " + app.Views.Home.UserRole() + ", Inst" + $("#ITy").val());
        if ($("#ITy").val() == "2") {
            return false;
        } else if ($("#ITy").val() == "3"  && app.Views.Home.UserRole() == "adm") {
            return true;
        }
        else if ((self.flow_record() == 0 && self.DataStatement() == 1) ||  (self.flow_record() == self.flow_institution() && self.DataStatement() == 2) || (self.Id() == "") || (self.flow_record() == self.flow_max() && (self.DataStatement() == 2 || self.DataStatement() == null))) {  // Case Status ==3 cuando esta cerrado
            return true;
        }
        else {
            return false;
        }

    }, self);

    self.addPerson = function () {
        self.people.push({ firstName: vm.firstName() });
        self.firstName('');
    }

    return self;

};

app.addViewModel({
    name: "Contact",
    bindingMemberName: "contact",
    factory: ContactViewModel
});