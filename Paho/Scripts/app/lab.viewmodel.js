function LabTest(SampleNumber) {
    var self = this;
    var date_receive = new Date();
    var date_format_ = app.dataModel.date_format_;
    var date_format_moment = app.dataModel.date_format_moment;
    var date_format_ISO = app.dataModel.date_format_ISO;
   

    date_receive = (SampleNumber == 1) ? jQuery.type(app.Views.Lab.RecDate()) === 'date' ? app.Views.Lab.RecDate() : parseDate($("#RecDate").val(), date_format_) : (SampleNumber == 2) ? jQuery.type(app.Views.Lab.RecDate2()) === 'date' ? app.Views.Lab.RecDate2() : parseDate($("#RecDate2").val(), date_format_) : (SampleNumber == 3) ? jQuery.type(app.Views.Lab.RecDate3()) === 'date' ? app.Views.Lab.RecDate3() : parseDate($("#RecDate3").val(), date_format_) : null;
    
    self.OrderArray
    self.Id = "";
    self.UsrCountry = ko.observable(app.Views.Home.UsrCountry()); // Pais del usuario logueado
    self.UsrInstID = ko.observable($('#IIDL').val()); // ID de la institucion del usuario
    self.CaseLabID = "";
    self.LabDummy = ko.observable("");
    self.ProcLab = ko.observable("");
    self.LabID = ko.observable("");
    self.CanEdit = ko.observable(true);
    self.CanPCR = ko.observable(app.Views.Lab.CanPCRLab());
    self.CanIFI = ko.observable(app.Views.Lab.CanIFILab());
    self.EndFlow = ko.observable("");
    self.ProcLabName = ko.observable("");
    self.ProcessLab = ko.observable("");
    self.SampleNumber = ko.observable(SampleNumber);
    self.TestType = ko.observable("");
    self.OrdenTestType = 99;
    self.TestDate = ko.observable(null);
    self.TestResultID = ko.observable("");
    self.OrdenTestResultID = 99;
    self.VirusTypeID = ko.observable("");
    self.CTVirusType = ko.observable("").extend({numeric : 2});
    self.CTRLVirusType = ko.observable("").extend({ numeric: 2 });
    self.OrdenVirusTypeID = 99;
    self.OtherVirusTypeID = ko.observable("");
    self.CTOtherVirusType = ko.observable("").extend({ numeric: 2 });
    self.CTRLOtherVirusType = ko.observable("").extend({ numeric: 2 });
    self.OtherVirus = ko.observable("");
    self.VirusSubTypeID = ko.observable("");
    self.CTSubType = ko.observable("").extend({ numeric: 2 });
    self.CTRLSubType = ko.observable("").extend({ numeric: 2 });
    self.OrdenSubTypeID = 99;
    // Modificacion solicitada por Rodrigo - Chile
    self.TestResultID_VirusSubType = ko.observable("");
    self.OrdenTestResultID_VirusSubType = 99;
    self.VirusSubTypeID_2 = ko.observable("");
    self.CTSubType_2 = ko.observable("").extend({ numeric: 2 });
    self.CTRLSubType_2 = ko.observable("").extend({ numeric: 2 });
    self.OrdenSubTypeID_2 = 99;
    self.TestResultID_VirusSubType_2 = ko.observable("");
    self.OrdenTestResultID_VirusSubType_2 = 99;
    // Termina modificación
    self.VirusLineageID = ko.observable("");
    self.CTLineage = ko.observable("").extend({ numeric: 2 });
    self.CTRLLineage = ko.observable("").extend({ numeric: 2 });
    self.OrdenLineageID = 99;
    self.RNP = ko.observable("").extend({ numeric: 2 });
    self.CTRLRNP = ko.observable("").extend({ numeric: 2 });
    self.CTRLNegative = ko.observable("").extend({ numeric: 2 });
    self.TestEndDate = ko.observable(null);
    // Área de virus
    self.InfA = ko.observable("");
    self.InfB = ko.observable("");
    self.ParaInfI = ko.observable("");
    self.ParaInfII = ko.observable("");
    self.ParaInfIII = ko.observable("");
    self.RSV = ko.observable("");
    self.Adenovirus = ko.observable("");
    self.Metapneumovirus = ko.observable("");
    self.hideLabOptions = false;
    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7) ? true : false;

    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;

    }, self);

    self.IFI_option = ko.computed(function () {
        return (self.UsrCountry() == 7 && self.TestType() == 1) ? true : false;

    }, self);

    self.TestDate.subscribe(function (newTestDate) {
        var current_value = jQuery.type(newTestDate) === 'date' ? newTestDate : parseDate(newTestDate, date_format_);
        //date_receive = (SampleNumber == 1) ? parseDate($("#RecDate").val(), date_format_) : (SampleNumber == 2) ? parseDate($("#RecDate2").val(), date_format_) : (SampleNumber == 2) ? parseDate($("#RecDate3").val(), date_format_) : null;
        //date_receive = typeof (date_receive) == "object" ? date_receive : parseDate(date_receive, date_format_);
        date_receive = (SampleNumber == 1) ? jQuery.type(app.Views.Lab.RecDate()) === 'date' ? app.Views.Lab.RecDate() : parseDate($("#RecDate").val(), date_format_) : (SampleNumber == 2) ? jQuery.type(app.Views.Lab.RecDate2()) === 'date' ? app.Views.Lab.RecDate2() : parseDate($("#RecDate2").val(), date_format_) : (SampleNumber == 3) ? jQuery.type(app.Views.Lab.RecDate3()) === 'date' ? app.Views.Lab.RecDate3() : parseDate($("#RecDate3").val(), date_format_) : null;

        if (date_receive == null || date_receive == "") {
            //alert("Por favor ingrese antes la fecha de recepción de muestra de la Muestra 1");
            alert(msgValidationTestDateInvalidDate);
            self.TestDate(null);
        } else {
            if (moment(current_value).isBefore(moment(date_receive), "days")) {
                //alert("La fecha de proceso de la Muestra 1 no puede ser menor a la fecha de recepción de muestra de la Muestra 1");
                alert(msgValidationTestDateProcessAlert);
                self.TestDate(null);
            }
        }
        
    });

    self.TestEndDate.subscribe(function (newTestEndDate) {
        var current_value = jQuery.type(newTestEndDate) === 'date' ? newTestEndDate : parseDate(newTestEndDate, date_format_);
        var date_TestBeginDate = jQuery.type(self.TestDate()) === 'date' ? self.TestDate() : parseDate(self.TestDate(), date_format_);

        if (date_TestBeginDate == null || date_TestBeginDate == "") {
            //alert("Por favor ingrese antes la fecha de inicio del proceso");
            alert(msgValidationTestBeginDateInvalidDate);
            self.TestEndDate("");
        } else {
            if (moment(current_value).isBefore(moment(date_TestBeginDate), "days")) {
                //alert("La fecha de fin de proceso de la Muestra 1 no puede ser menor a la fecha de inicio de proceso de muestra de la Muestra 1");
                alert(msgValidationTestBeginDateProcessAlert);
                self.TestEndDate("");
            }
        }

    });

    self.EnableTestResult = ko.computed(function () {
        if (self.TestType() != "" ) {
            return true;
        }
        else {
            self.TestResultID("");
            self.OrdenTestResultID = 99;
            return false;
        }
    }, self);

    self.EnableVirusTypes = ko.computed(function () {
        //if (((self.TestResultID() != "U" && self.TestResultID() != "N" && self.EnableCHI()) || (self.TestResultID() != "U" && self.DisableCHI())) && self.TestResultID() != "" && typeof self.TestResultID() != "undefined") { // Desactivado por requerimiento de RRR y Suriname porque si les interesa ingresar cuando es negativo
        if (self.TestResultID() != "NA" && self.TestResultID() != "NB" && self.TestResultID() != "I" && self.TestResultID() != "V" && self.TestResultID() != "U" && self.TestResultID() != "" && typeof self.TestResultID() != "undefined") {
            if (self.TestType() == 1 && self.TestResultID() == "N"){  
                return false;
            } else {
                return true;
            }   
        }
        else {
            self.VirusTypeID("");
            self.OrdenVirusTypeID = 99;
            return false;
        }
    }, self);

    self.EnableCTVirusType = ko.computed(function () {
        if ( self.UsrCountry() == 7 && self.TestType() == 2 && self.VirusTypeID() != "") {
            return true;
        }
        else {
            self.CTVirusType("");
            return false;
        }
            
    }, self);

    self.EnableOtherVirusTypes = ko.computed(function () {
        if (self.VirusTypeID() == "9" ) {
            return true;
        }
        else {
            self.OtherVirusTypeID("");
            return false;
        }
    }, self);

    self.EnableCTOtherVirusType = ko.computed(function () {
        if (self.UsrCountry() == 7 && self.VirusTypeID() == 9 && self.TestType() == 2) {
            return true;
        }
        else {
            self.CTOtherVirusType("");
            return false;
        }

    }, self);

    self.EnableVirusSubTypeID = ko.computed(function () {
        //var result = (self.VirusTypeID() == "1" && self.TestResultID() == "P"); // modificacion solicitada por Rodrigo
        //console.log("EnableVirusSubTypeID");
        var result = (self.VirusTypeID() == "1" && self.TestResultID() == "P");
        if (!result) {
            self.VirusSubTypeID("");
            self.OrdenSubTypeID = 99;
            self.TestResultID_VirusSubType("");
            self.CTSubType("");
            self.CTRLSubType("")
        };
        if (self.TestType() == "1") {
            self.VirusSubTypeID("");
            self.OrdenSubTypeID = 99; self.TestResultID_VirusSubType("");
            self.CTSubType("");
            self.CTRLSubType("")
            return false;
        }
            
        return result;
    }, self);

    // Aqui agregar la funcion para esconder el resultado del subtipo

    self.EnableVirusSubTypeID_2 = ko.computed(function () {
        //var result = (self.VirusTypeID() == "1" && self.TestResultID() == "P"); // modificacion solicitada por Rodrigo
        //console.log("EnableVirusSubTypeID_2");
        var result = (self.VirusTypeID() == "1" && self.UsrCountry() == 7 && self.TestResultID() == "P");
        if (!result) {
            self.VirusSubTypeID_2("");
            self.OrdenSubTypeID_2 = 99;
            self.TestResultID_VirusSubType_2("");
            self.CTSubType_2("");
            self.CTRLSubType_2("")
        };
        if (self.TestType() == "1") {
            self.VirusSubTypeID_2("");
            self.OrdenSubTypeID_2 = 99;
            self.TestResultID_VirusSubType_2("");
            self.CTSubType_2("");
            self.CTRLSubType_2("")
            return false;
        }

        return result;
    }, self);

    //CT Subtype 
    self.EnableCTSubType = ko.computed(function () {
        if ((self.VirusSubTypeID() == 3 || self.VirusSubTypeID() == 4 || self.VirusSubTypeID() == 5 || self.VirusSubTypeID() == 10) && self.TestType() == 2 && self.UsrCountry() == 7) {

            return true;
        }
        else {
            self.CTSubType("");
            return false;
        }
            
    }, self);
    self.EnableCTSubType_2 = ko.computed(function () {
        if ((self.VirusSubTypeID_2() == 3 || self.VirusSubTypeID_2() == 4 || self.VirusSubTypeID_2() == 5 || self.VirusSubTypeID_2() == 10) && self.TestType() == 2 && self.UsrCountry() == 7) {

            return true;
        }
        else {
            self.CTSubType_2("");
            return false;
        }

    }, self);
    self.EnableCTLineage = ko.computed(function () {
        if ((self.VirusLineageID() == 2 || self.VirusLineageID() == 3) && self.TestType() == 2 && self.UsrCountry() == 7) {

            return true;
        }
        else {
            self.CTLineage("");
            return false;
        }

    }, self);

    self.EnableRNP = ko.computed(function () {
        // Desactivado porque solicitaron que fuera para todos los virus
        //(self.VirusSubTypeID() == 3 || self.VirusSubTypeID() == 10 || self.VirusTypeID() == 1 || self.VirusTypeID() == 2)
        if (self.TestType() == 2 && self.UsrCountry() == 7) {

            return true;
        }
        else {
            self.RNP("");
            return false;
        }

    }, self);


    self.EnableCTRLNegative = ko.computed(function () {
        // Desactivado porque solicitaron que fuera para todos los virus
        //(self.VirusSubTypeID() == 3 || self.VirusSubTypeID() == 10 || self.VirusTypeID() == 1 || self.VirusTypeID() == 2)
        //if (self.TestType() == 2 && self.UsrCountry() == 7 && self.TestResultID() == "N") { // Modificado a solicitur de Rodrigo que siempre aparezca aunque el resultado sea positivo
        if (self.TestType() == 2 && self.UsrCountry() == 7 ) {

            return true;
        }
        else {
            self.CTRLNegative("");
            return false;
        }

    }, self);

    self.EnableVirusLineageID = ko.computed(function () {
        var result = (self.VirusTypeID() == "2"  && self.TestResultID() == "P"); if (!result) self.VirusLineageID("");
        if (self.TestType() == "1") {
            self.VirusLineageID("");
            self.OrdenLineageID = 99;
            return false;
        }
        return result;
    }, self);

    self.ShowProcessedLab = ko.computed(function () {
        return (self.ProcessLab() === "true")
    }, self);
    self.NotShowProcessedLab = ko.computed(function () {
        return (self.ProcessLab() === "false")
    }, self);

    self.ProcessLab.subscribe(function (process) {
        
        self.TestType(null);
        self.TestDate(null);
        self.TestResultID("");
        self.VirusTypeID("");
        self.CTVirusType("");
        self.CTRLVirusType("");
        self.VirusSubTypeID("");
        self.CTSubType("");
        self.CTRLSubType("");
        self.TestResultID_VirusSubType("");
        self.VirusSubTypeID_2("");
        self.CTSubType_2("");
        self.CTRLSubType_2("");
        self.TestResultID_VirusSubType_2("");
        self.VirusLineageID("");
        self.CTLineage("");
        self.CTRLLineage("");
        self.RNP("");
        self.CTRLNegative("");
        self.TestEndDate(null);
        self.InfA("");
        self.InfB("");
        self.ParaInfI("");
        self.ParaInfII("");
        self.ParaInfIII("");
        self.RSV("");
        self.Adenovirus("");
        self.Metapneumovirus("");
    });


    // Asignacion de resultado automaticamente
    self.VirusTypeID.subscribe(function (new_virus) {
        if (new_virus != "" && new_virus != null) {
            var category = ko.utils.arrayFirst(app.Views.Home.CVT(), function (category) {
                return category.Id === new_virus;
            });
            if (category != null && category != "undefined" )
            self.OrdenVirusTypeID = category.orden;
            app.Views.Lab.OrdenFinalResult();
        }
        
    });

    self.VirusSubTypeID.subscribe(function (new_subtype) {
        if (new_subtype != "" && new_subtype != null) {

            var category = ko.utils.arrayFirst(app.Views.Home.CVST(), function (category) {
                return category.Id === new_subtype;
            });
            if (category != null && category != "undefined")
            self.OrdenSubTypeID = category.orden;
            app.Views.Lab.OrdenFinalResult();
        }

    });

    self.VirusLineageID.subscribe(function (new_lineage) {
        if (new_lineage != "" && new_lineage != null) {

            var category = ko.utils.arrayFirst(app.Views.Home.CVL(), function (category) {
                return category.Id === new_lineage;
            });
            if (category != null && category != "undefined")
            self.OrdenLineageID = category.orden;
            app.Views.Lab.OrdenFinalResult();
        }

    });

    self.TestType.subscribe(function (new_test_type) {
        if (new_test_type != "" && new_test_type != null) {

            var category = ko.utils.arrayFirst(app.Views.Home.CTT(), function (category) {
                return category.Id === new_test_type;
            });
            if (category != null && category != "undefined")
                self.OrdenTestType = category.orden;
            app.Views.Lab.OrdenFinalResult();
        }

    });

    self.TestResultID.subscribe(function (new_test_result) {
        if (new_test_result != "" && new_test_result != null) {
            //alert(new_test_result);
            var category = ko.utils.arrayFirst(app.Views.Home.CTR(), function (category) {
                return category.Id === new_test_result;
            });
            //alert(category.orden);
            if (category != null && category != "undefined")
            self.OrdenTestResultID = category.orden;

            app.Views.Lab.OrdenFinalResult();
        }

    });

    self.TestResultID_VirusSubType.subscribe(function (new_test_result_virussubtype) {
        if (new_test_result_virussubtype != "" && new_test_result_virussubtype != null) {
            //alert(new_test_result);
            var category = ko.utils.arrayFirst(app.Views.Home.CTR(), function (category) {
                return category.Id === new_test_result_virussubtype;
            });
            //alert(category.orden);
            if (category != null && category != "undefined")
                self.OrdenTestResultID_VirusSubType = category.orden;

            app.Views.Lab.OrdenFinalResult();
        }

    });

    self.TestResultID_VirusSubType_2.subscribe(function (new_test_result_virussubtype) {
        if (new_test_result_virussubtype != "" && new_test_result_virussubtype != null) {
            //alert(new_test_result);
            var category = ko.utils.arrayFirst(app.Views.Home.CTR(), function (category) {
                return category.Id === new_test_result_virussubtype;
            });
            //alert(category.orden);
            if (category != null && category != "undefined")
                self.OrdenTestResultID_VirusSubType_2 = category.orden;

            app.Views.Lab.OrdenFinalResult();
        }

    });


    self.VisibleFalseIFIVirus = function (option, item) {
        //console.log('item id = ' + item.id);
        if (!(app.Views.Lab.CanPCRLab() == true) && typeof (item) != 'undefined' ) {
            //console.log('RemoveIFIVirus');
            //console.log(item);
            if (item.Id == 12 || item.Id == 11 || item.Id == 10) {
                ko.applyBindingsToNode(option, {
                    attr: {
                        'style': 'display:none'
                    }
                }, item);
            }

        }

    };

    self.VisibleTestResultIFI = function (option, item) {
        //console.log('item id = ');
        //console.log(app.Views.Lab.CanIFILab());
        //console.log(typeof (item));
        //console.log(app.Views.Lab.LabTests().length);
        if (typeof (item) != 'undefined' && app.Views.Home.UsrCountry() != 7)
        {
                if (item.Id == 'NA' || item.Id == 'NB') {
                    ko.applyBindingsToNode(option, {
                        attr: {
                            'style': 'display:none'
                        }
                    }, item);
                }
        } else if (typeof (item) != 'undefined' && app.Views.Home.UsrCountry() == 7) {
            if(app.Views.Lab.CanPCRLab() == true && app.Views.Lab.CanIFILab() == false)
            {
                if (item.Id == 'NA' || item.Id == 'NB') {
                    ko.applyBindingsToNode(option, {
                        attr: {
                            'style': 'display:none'
                        }
                    }, item);
                }
            }
        }
        

    };

};

function LabViewModel(app, dataModel) {
    var self = this;
    var date_format_ = app.dataModel.date_format_;
    var date_format_moment = app.dataModel.date_format_moment;
    var date_format_ISO = app.dataModel.date_format_ISO;

    var rec_date = new Date();
    var date_close_date_lab = new Date();
    var date_lab_test_start = new Date();
    var date_lab_text_final = new Date();
    

    //Orden para el resultado final automaticamente

    self.OrderDummy = ko.observableArray([]);
    self.OrderArrayFinalResult = ko.observableArray([]);

    self.Id = "";
    self.UsrCountry = ko.observable(app.Views.Home.UsrCountry()); // Pais del usuario logueado
    self.UsrInstID = ko.observable($('#IIDL').val()); // ID de la institucion del usuario
    self.ISPID = ko.observable(97);  //Es 97 porque es el ID de la tabla institución
    self.hasReset = ko.observable(false);
    self.hasGet = ko.observable(false);
    self.RecDate = ko.observable(null);
    self.TempSample1 = ko.observable("").extend({ numeric: 2 });
    self.Processed = ko.observable("");
    self.NoProRen = ko.observable("");
    self.NoProRenId = ko.observable("");

    self.RecDate2 = ko.observable(null);
    self.TempSample2 = ko.observable("").extend({ numeric: 2 });
    self.Processed2 = ko.observable("");
    self.NoProRen2 = ko.observable("");
    self.NoProRenId2 = ko.observable("");

    self.RecDate3 = ko.observable(null);
    self.TempSample3 = ko.observable("").extend({ numeric: 2 });
    self.Processed3 = ko.observable("");
    self.NoProRen3 = ko.observable("");
    self.NoProRenId3 = ko.observable("");

    self.CanPCRLab = ko.observable(true);
    self.CanIFILab = ko.observable(true);

    self.RecDate.subscribe(function (newRecDate) {
        if (self.hasReset() != true && newRecDate != "" && newRecDate != null)
        {
            var current_value = jQuery.type(newRecDate) === 'date' ? newRecDate : parseDate(newRecDate, date_format_);
            var date_sample_date_ = jQuery.type(app.Views.Hospital.SampleDate()) === 'date' ? app.Views.Hospital.SampleDate() : parseDate(app.Views.Hospital.SampleDate(), date_format_);

            if ((date_sample_date_ == null || date_sample_date_ == "") && self.hasReset() != true) {
                //alert("Por favor ingrese antes la fecha de toma muestra de la Muestra 1");
                alert(msgValidationSampleDateS1);
                    self.RecDate(null);
                } else {
                    if (moment(current_value).isBefore(moment(date_sample_date_), "days")) {
                        //alert("La fecha de recepción de Muestra 1 no puede ser menor a la fecha de toma muestra de la Muestra 1");
                        alert(msgValidationSampleDateValidateS1);
                        self.RecDate(null);
                    }
                  }
        }
    });

    self.RecDate2.subscribe(function (newRecDate2) {
        if (self.UsrCountry() == 7 || self.UsrCountry() == 3 || self.UsrCountry() == 25) {
            if (self.hasReset() != true && newRecDate2 != "") {
                var current_value = jQuery.type(newRecDate2) === 'date'  ? newRecDate2 : parseDate(newRecDate2, date_format_);
                var date_sample_date_ = jQuery.type(app.Views.Hospital.SampleDate2()) === 'date'  ? app.Views.Hospital.SampleDate2() : parseDate(app.Views.Hospital.SampleDate2(), date_format_);

                if (date_sample_date_ == null || date_sample_date_ == "") {
                    //alert("Por favor ingrese antes la fecha de toma muestra de la Muestra 2");
                    alert(msgValidationSampleDateS2);
                    self.RecDate2(null);
                } else {
                    if (moment(current_value).isBefore(moment(date_sample_date_), "days")) {
                        //alert("La fecha de recepción de Muestra 2 no puede ser menor a la fecha de toma muestra de la Muestra 2");
                        alert(msgValidationSampleDateValidateS2);
                        self.RecDate2(null);
                    }
                }
            }
        }

    });

    self.RecDate3.subscribe(function (newRecDate3) {
        if (self.UsrCountry() == 7) {
            if (self.hasReset() != true && newRecDate3 != "") {
                var current_value = jQuery.type(newRecDate3) === 'date'  ? newRecDate3 : parseDate(newRecDate3, date_format_);
                var date_sample_date_ = jQuery.type(app.Views.Hospital.SampleDate3()) === 'date' ? app.Views.Hospital.SampleDate3() : parseDate(app.Views.Hospital.SampleDate3(), date_format_);
                if (self.UsrCountry() == 7) {
                    if (self.hasReset() != true && newRecDate3 != "") {
                        if (date_sample_date_ == null || date_sample_date_ == "") {
                            //alert("Por favor ingrese antes la fecha de toma muestra de la Muestra 3");
                            alert(msgValidationSampleDateS3);
                            self.RecDate3(null);
                        } else {
                            if (moment(current_value).isBefore(moment(date_sample_date_), "days")) {
                                //alert("La fecha de recepción de Muestra 3 no puede ser menor a la fecha de toma muestra de la Muestra 3");
                                alert(msgValidationSampleDateValidateS3);
                                self.RecDate3(null);
                            }
                        }
                    }
                }
            }
        }
        
    });

    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7) ? true : false;

    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;

    }, self);

    self.EnableCHIPrint = ko.computed(function () {
        return (self.UsrCountry() == 7 && $("#ITy").val() == 2  ) ? true : false;

    }, self);

    //self.VisibleMuestra2 = ko.computed(function () {
    //    return (self.UsrCountry() == 7 && app.Views.Hospital.SampleDate2() && self.RecDate()) ? true : false;

    //}, self);

    //self.VisibleMuestra3 = ko.computed(function () {
    //    return (self.UsrCountry() == 7 && app.Views.Hospital.SampleDate3() && self.RecDate2()) ? true : false;

    //}, self);

    self.VisibleMuestraRec2 = ko.computed(function () {
        return ((self.UsrCountry() == 7 || self.UsrCountry() == 3 ) && app.Views.Hospital.SampleDate2() != null) ? true : false;

    }, self);

    self.VisibleMuestraRec3 = ko.computed(function () {
        return (self.UsrCountry() == 7 && app.Views.Hospital.SampleDate3() != null ) ? true : false;

    }, self);

    self.FinalResult = ko.observable("");
    self.FinalResultVirusTypeID = ko.observable("");
    self.FinalResultVirusSubTypeID = ko.observable("");
    self.FinalResultVirusLineageID = ko.observable("");

    self.FinalResult_2 = ko.observable("");
    self.FinalResultVirusTypeID_2 = ko.observable("");
    self.FinalResultVirusSubTypeID_2 = ko.observable("");
    self.FinalResultVirusLineageID_2 = ko.observable("");

    self.FinalResult_3 = ko.observable("");
    self.FinalResultVirusTypeID_3 = ko.observable("");
    self.FinalResultVirusSubTypeID_3 = ko.observable("");
    self.FinalResultVirusLineageID_3 = ko.observable("");

    self.resetFinalResult = function () {

        self.FinalResult("");
        self.FinalResultVirusTypeID("");
        self.FinalResultVirusSubTypeID("");
        self.FinalResultVirusLineageID("");
        self.FinalResult_2("");
        self.FinalResultVirusTypeID_2("");
        self.FinalResultVirusSubTypeID_2("");
        self.FinalResultVirusLineageID_2("");
        self.FinalResult_3("");
        self.FinalResultVirusTypeID_3("");
        self.FinalResultVirusSubTypeID_3("");
        self.FinalResultVirusLineageID_3("");

    };


    self.ShowProcessed = ko.computed(function () {
        self.NoProRen("");
        self.NoProRenId("");
        self.resetFinalResult();
        return (self.Processed() === "true")
    }, self);

    self.ShowProcessed2 = ko.computed(function () {
        self.NoProRen2("");
        self.NoProRenId2("");
        self.resetFinalResult();
        return (self.Processed2() === "true")
    }, self);

    self.ShowProcessed3 = ko.computed(function () {
        self.NoProRen3("");
        self.NoProRenId3("");
        self.resetFinalResult();
        return (self.Processed3() === "true")
    }, self);

    self.NotShowProcessed = ko.computed(function () {
        return (self.Processed() === "false")
    }, self);
    self.NotShowProcessedOther = ko.computed(function () {
        return (self.NoProRenId() === "5")
    }, self);

    self.NotShowProcessed2 = ko.computed(function () {
        return (self.Processed2() === "false")
    }, self);

    self.NotShowProcessedOther2 = ko.computed(function () {
        return (self.NoProRenId2() === "5")
    }, self);

    self.NotShowProcessed3 = ko.computed(function () {
        return (self.Processed3() === "false")
    }, self);

    self.NotShowProcessedOther3 = ko.computed(function () {
        return (self.NoProRenId3() === "5")
    }, self);


    self.EndLabDate = ko.observable(null);
    self.Comments = ko.observable("");


    self.LabTests = ko.observableArray([]);
    self.LabTests_Sample2 = ko.observableArray([]);
    self.LabTests_Sample3 = ko.observableArray([]);
    self.LabsResult = ko.observableArray([]);
    self.SubTypeByLabRes = ko.observableArray([]);
    self.ArrayValidate = ko.observableArray([]);
    
    //self.SaveAndAdd_1 = ko.observable(true);
    //self.SaveAndAdd_2 = ko.observable(true);
    //self.SaveAndAdd_3 = ko.observable(true);
    self.CanConclude = ko.observable(true);
    self.FResult = ko.observable("");

    self.FinalResult.subscribe(function (newFinalResult) {
        if (newFinalResult != "" && newFinalResult != "undefined" && newFinalResult != null)
        {
            $("a[href*='tab-case']").show();
            $("#tab-case").show();
            $("#tabs").tabs("refresh");
            if (app.Views.Contact.SurvILI() == true) {
                $("#CaseStatus").attr("disabled", false);
                }
            if (app.Views.Contact.SurvSARI() == true && app.Views.Hospital.Destin() != "" && self.CanConclude() == true) {
                $("#CaseStatus").attr("disabled", false);
            }
            //$("#tabs").tabs("refresh");

        } else if ((newFinalResult == "" || newFinalResult == "undefined" || newFinalResult == null) && app.Views.Hospital.IsSample() === "true") {
            $("a[href*='tab-case']").hide();
            $("#tab-case").hide();
        }
    });



    self.EnableFinalResultVirusSubTypeID = ko.computed(function () {
        var result = self.FinalResultVirusTypeID() == "1";
        if (!result) self.FinalResultVirusSubTypeID("");
        return result;
    }, self);
    self.EnableFinalResultVirusLineageID = ko.computed(function () {
        var result = self.FinalResultVirusTypeID() == "2"; if (!result) self.FinalResultVirusLineageID(""); return result;
    }, self);

    self.EnableFinalResultVirusSubTypeID_2 = ko.computed(function () {
        var result = self.FinalResultVirusTypeID_2() == "1";
        if (!result) self.FinalResultVirusSubTypeID_2("");
        return result;
    }, self);

    self.EnableFinalResultVirusLineageID_2 = ko.computed(function () {
        var result = self.FinalResultVirusTypeID_2() == "2"; if (!result) self.FinalResultVirusLineageID_2(""); return result;
    }, self);

    self.EnableFinalResultVirusSubTypeID_3 = ko.computed(function () {
        var result = self.FinalResultVirusTypeID_3() == "1";
        if (!result) self.FinalResultVirusSubTypeID_3("");
        return result;
    }, self);

    self.EnableFinalResultVirusLineageID_3 = ko.computed(function () {
        var result = self.FinalResultVirusTypeID_3() == "2"; if (!result) self.FinalResultVirusLineageID_3(""); return result;
    }, self);

    self.Processed.subscribe(function (NewIsProcessed) {
        if (NewIsProcessed == "false")
            self.LabTests([]);
    });

    self.EnableVirusTypesFinal = ko.computed(function () {
        //if (self.FinalResult() != "") {
        //    $("#CaseStatus").attr("disabled", false);
        //} else {
        //    $("#CaseStatus").attr("disabled", true);
        //}
        if (self.FinalResult() == "N" || self.FinalResult() == "U" || self.FinalResult() == "") {
            self.FinalResultVirusTypeID("");
            return false;
        }
        else return true;
    }, self);
    //self.SetFResult = function () {
    //    var virustypes;
    //    var labtests = $.grep(self.LabTests(), function (v) {
    //        return v.TestType() === "2" && v.TestResultID() === "P" && (v.VirusTypeID() === "2" || v.VirusTypeID() === "3" || v.VirusTypeID() === "4" || v.VirusTypeID() === "5")
    //    });
    //    if (labtests.length > 0) {
    //        console.log(labtests[0].VirusTypes());
    //        console.log(labtests[0].VirusTypeID());
    //        virustypes = $.grep(labtests[0].VirusTypes(), function (v) {
    //            return v.Id == labtests[0].VirusTypeID()
    //        });
    //        console.log(virustypes);
    //        return self.FResult(virustypes[0].Name);
    //    }

    //    labtests = $.grep(self.LabTests(), function (v) {
    //        return v.TestType() === "1" && v.TestResultID() === "P" && (v.VirusTypeID() === "1" || v.VirusTypeID() === "5")
    //    });
    //    if (labtests.length > 0) {
    //        console.log(labtests[0].VirusTypes());
    //        console.log(labtests[0].VirusTypeID());
    //        virustypes = $.grep(labtests[0].VirusTypes(), function (v) {
    //            return v.Id == labtests[0].VirusTypeID()
    //        });
    //        console.log(virustypes);
    //        return self.FResult(virustypes[0].Name);
    //    }

    //    labtests = $.grep(self.LabTests(), function (v) {
    //        return v.TestResultID() === "P"
    //    });
    //    if (labtests.length > 0) {
    //        console.log(labtests[0].VirusTypes());
    //        console.log(labtests[0].VirusTypeID());
    //        virustypes = $.grep(labtests[0].VirusTypes(), function (v) {
    //            return v.Id == labtests[0].VirusTypeID()
    //        });
    //        console.log(virustypes);
    //        return self.FResult(virustypes[0].Name);
    //    }
    //};
    //self.LabTests.subscribe(function (newlabtests)  {
    //    self.SetFResult();
    //});  


    self.generateSortFn = function (props) {
        return function (a, b) {
            for (var i = 0; i < props.length; i++) {
                var prop = props[i];
                var name = prop.name;
                var reverse = prop.reverse;
                if (a[name] < b[name])
                    return reverse ? 1 : -1;
                if (a[name] > b[name])
                    return reverse ? -1 : 1;
            }
            return 0;
        };
    };

    self.isISP = ko.computed(function () {
        
        if (self.UsrCountry() == 7) {
            var labtests_check = $.grep(self.LabsResult(), function (v) {
                return v.Id === "97";
            });
            if (labtests_check.length >= 1)
                return true
            else
                return false;
        }
        else {
            return true
        };
    }, self);


    // Using a deep get method to find nested object properties
    self.objectSort = function (column) {

        self.players(self.players().sort(function (a, b) {

            var playerA = self.deepGet(a, column.property),
            playerB = self.deepGet(b, column.property);

            if (playerA < playerB) {
                return (column.state() === self.ascending) ? -1 : 1;
            }
        else if (playerA > playerB) {
            return (column.state() === self.ascending) ? 1 : -1;
        }
        else {
                return 0
         }
        }));
    };

    self.addLabTest = function (sample_number, data) {
        var erroMsg = self.validatebeforeadd(1,2);
        if (erroMsg) {
            alert(erroMsg);
            return;
        }
        var labtest = new LabTest(sample_number);
        labtest.hideLabOptions = true;
        //console.log(labtest.displayLabOptions + " **** ");
        labtest.CaseLabID = self.Id;
        labtest.SampleNumber(sample_number);
        if (sample_number == 1) {
            self.LabTests.push(labtest);
        } else if (sample_number == 2) {
            self.LabTests_Sample2.push(labtest);
        } else if (sample_number == 3) {
            self.LabTests_Sample3.push(labtest);
        }
         
    };

    self.removeLabTest = function (sample_number, data) {

        self.players.remove(function (player) { return player.name == Name });

        //var labtest = new LabTest(sample_number);
        //labtest.hideLabOptions = true;
        ////console.log(labtest.displayLabOptions + " **** ");
        //labtest.CaseLabID = self.Id;
        //labtest.SampleNumber(sample_number);
        //if (sample_number == 1) {
        //    self.LabTests.remove(labtest);
        //} else if (sample_number == 2) {
        //    self.LabTests_Sample2.remove(labtest);
        //} else if (sample_number == 3) {
        //    self.LabTests_Sample3.remove(labtest);
        //}

    };


    self.ResetLab = function () {
        self.hasReset(true);
        self.Id = "";
        self.CanIFILab(false);
        self.CanPCRLab(false);
        self.RecDate(null);
        //self.RecDate(null);
        self.Processed("");
        self.TempSample1("");
        self.RecDate2(null);
        self.Processed2("");
        self.TempSample2("");
        self.RecDate3(null);
        self.Processed3("");
        self.TempSample3("");
        self.EndLabDate(null);
        self.FResult("");
        self.Comments("");
        self.FinalResult("");
        self.FinalResultVirusTypeID("");
        self.FinalResultVirusSubTypeID("");
        self.FinalResultVirusLineageID("");
        self.FinalResult_2("");
        self.FinalResultVirusTypeID_2("");
        self.FinalResultVirusSubTypeID_2("");
        self.FinalResultVirusLineageID_2("");
        self.FinalResult_3("");
        self.FinalResultVirusTypeID_3("");
        self.FinalResultVirusSubTypeID_3("");
        self.FinalResultVirusLineageID_3("");
        self.NoProRen("");
        self.NoProRen2("");
        self.NoProRen3("");
        self.NoProRenId("");
        self.NoProRenId2("");
        self.NoProRenId3("");
        self.LabTests([]);
        self.LabTests_Sample2([]);
        self.LabTests_Sample3([]);
        self.LabsResult([]);
        self.SubTypeByLabRes([]);
        
    };

    self.validate = function (nextStep) {
        var msg = "";
        rec_date = jQuery.type(self.RecDate()) === 'date' ? self.RecDate() : parseDate($("#RecDate").val(), date_format_);
        date_close_date_lab = jQuery.type(self.EndLabDate()) === 'date' ? self.EndLabDate() :  parseDate($("#EndLabDate").val(), date_format_);
        //var date_ShipDate = new Date();
        date_ShipDate = jQuery.type(app.Views.Hospital.ShipDate()) === 'date' ? app.Views.Hospital.ShipDate() : parseDate($("#ShipDate").val(), date_format_);

        //if (app.Views.Hospital.IsSample() === "true" && $("#ITy").val() != 1 && $("#RecDate").val() !="") {
        if (app.Views.Hospital.IsSample() === "true" && $("#ITy").val() != 1 && self.Processed() == "true") {

            if ($("#RecDate").val() == "")
                msg += "\n" + msgValidateRecDateRequired;
                    //"Fecha de recepción es requerida";
            if ($("#RecDate").val() != "" && !moment(moment(rec_date).format(date_format_moment), [date_format_moment], true).isValid())
                msg += "\n" + msgValidateRecDateInvalid;
                    //"Fecha de recepción es inválida ";

            if (date_ShipDate != null && rec_date != null && moment(rec_date).isBefore(moment(date_ShipDate), 'days'))
                msg += "\n" + msgValidateRecDateShipDate;
                    //"'Fecha de recepcion' no puede ser anterior a la 'Fecha de envío'";
            

            if (self.Processed() == "true") {
                if (self.LabTests().length <= 0) {
                    msg += "\n" + msgValidateInsertResult;
                        //"Inserte al menos un resultado";
                }

                // Verificación de procesos uno de la muestra 1
                var errRes = self.validatebeforeadd(1,1);
                if (errRes != "") {
                    msg += errRes;
                }
            }

            var labtests_check = $.grep(self.LabTests(), function (v) {
                return v.TestType() === "2";
            });
            if (labtests_check.length > 0) {
                if ($("#EndLabDate").val() == "")
                    msg += "\n" + msgValidateFinalResultDate;
                        //"Fecha del resultado final es requerida";

                if ($("#EndLabDate").val() != "") {
                    if (!moment(moment(date_close_date_lab).format(date_format_moment), [date_format_moment], true).isValid())
                        msg += "\n" + msgValidateFinalResultDateInvalid;
                            //"Fecha del resultado final es inválida";

                    if ($("#RecDate").val() != "" && moment(rec_date).isAfter(moment(date_close_date_lab), 'days'))
                        msg += "\n" + msgValidateFinalResultDateRecDate;
                            //"'Fecha del resultado final' no puede ser anterior a la 'Fecha de recepción'";

                    if (!self.FinalResult() || self.FinalResult() == "") {
                        msg += "\n" + msgValidateFinalResult;
                            //"Selecione un resultado final";
                    }
                }
            }

            

        }

        if (self.Processed() == "false" &&  self.NoProRenId() == "") {
            msg += "\n" + msgValidateProcessedReason;
                //"Indique la razón porque no fue procesada la muestra";
        }

        if (msg !== "") { alert(msgValidationTitleLab + msg); $('#tabs').tabs({ active: 4 }); return false; }

        //if (self.UsrCountry == 7) {
        //    if (msg !== "") { alert('LABORATORIO:' + msg); $('#tabs').tabs({ active: 4 }); return false; }
        //} else {
        //    if (msg !== "") { alert('LABORATORIO:' + msg); $('#tabs').tabs({ active: 5 }); return false; }
        //}
        
        if (nextStep != null) nextStep();
        return true;
    };

    self.validatebeforeadd = function (nextStep, SaveVerif) {
        var msg = "";
        self.ArrayValidate((nextStep == 1) ? self.LabTests() : (nextStep == 2) ? self.LabTests_Sample2() : self.LabTests_Sample3());
        for (index = 0; index < self.ArrayValidate().length; ++index) {
            date_1 = self.ArrayValidate()[index].TestDate();
            date_2 = self.ArrayValidate()[index].TestEndDate();
            date_Received = self.RecDate();
            var date_test_start = new Date();
            var date_test_final = new Date();
            var date_RecDate = new Date();
            date_test_start = jQuery.type(date_1) === 'date' ? date_1 : parseDate(date_1, date_format_);
            date_test_final = jQuery.type(date_2) === 'date' ? date_2 : parseDate(date_2, date_format_);
            date_RecDate = jQuery.type(date_Received) === 'date' ? date_Received : parseDate($("#RecDate").val(), date_format_);

            if (self.ArrayValidate()[index].ProcLab() === "" || typeof self.ArrayValidate()[index].ProcLab() === "undefined")
                msg += "\n" + msgValidateProcessSelectLab;
                    //"'Laboratorio': Seleccione el laboratorio que procesa la prueba";

            if (self.ArrayValidate()[index].ProcessLab() === "")
                msg += "\n" + msgValidateProcessProcessLab;
                    //"'Procesada': Indique si la prueba fue procesada";
 
            if (self.ArrayValidate()[index].ProcessLab() === "true") {

                if (date_RecDate != null && date_test_start != null && moment(date_test_start).isBefore(moment(date_RecDate), 'days'))
                    msg += "\n" + msgValidateProcessStartReceiptDate;
                    //'Process start date' can not be less than the 'Date of receipt of the sample'
                        //"'Fecha de inicio del proceso' no puede ser menor a la 'Fecha de recepcion de la muestra'";
                //return false;

                if (date_test_start === null)
                    msg += "\n" + msgValidateProcessStartRequired;
                    //Process start date is required
                    //    "Fecha de inicio de proceso es requerida";
                if (date_test_start != null && !moment(moment(date_test_start).format(date_format_moment), [date_format_moment], true).isValid())
                    msg += "\n" + msgValidateProcessStartInvalid;
                        //"Fecha de inicio de proceso es inválida";
                //if (date_RecDate != null && date_test_start != null && moment(date_test_start).isBefore(moment(date_RecDate), 'days'))
                //    msg += "\n" + "'Start date' cannot be less than the 'Received date'";

                if (date_test_final === null)
                    msg += "\n" + msgValidateProcessEndDateRequired;
                        //"Fecha final del proceso es requerida";
                if (date_test_final != null && !moment(moment(date_test_final).format(date_format_moment), [date_format_moment], true).isValid())
                    msg += "\n" + msgValidateProcessEndDateInvalid;
                        //"Fecha final del proceso es inválida";
                if (date_test_start != null && date_test_final != null
                    && moment(date_test_start).isAfter(moment(date_test_final), 'days'))
                    msg += "\n" + msgValidateProcessEndDateStartDate;
                        //'Process end date' can not be less than 'Process start date'
                        //"'Fecha final del proceso' no puede ser menor a la 'Fecha de inicio del proceso'";

                if (self.ArrayValidate()[index].TestType() === "")
                    msg += "\n" + msgValidateProcessType;
                        //Insert the type of completed process
                        //"Indique el tipo de proceso realizado";

                if (self.ArrayValidate()[index].TestResultID() === "")
                    msg += "\n" + msgValidateProcessResult;
                        //Insert the result of the process
                        //"Indique el resultado del proceso";

                if ((self.ArrayValidate()[index].TestResultID() === "P" || self.ArrayValidate()[index].TestResultID() === "I") && (self.ArrayValidate()[index].VirusTypeID() === "" || self.ArrayValidate()[index].VirusTypeID() == undefined))
                {
                    msg += "\n" + msgValidateProcessVirus;
                        //Insert the detected virus
                        //"Ingrese el virus detectado";
                }
                else {
                    if (self.ArrayValidate()[index].UsrCountry() == 7 && self.ArrayValidate()[index].CTVirusType() === "" && self.ArrayValidate()[index].VirusTypeID() != "" && self.ArrayValidate()[index].TestType() === "2")
                    { msg += "\n" + "Ingreso el CT de Virus"; }
                }

                if ((self.ArrayValidate()[index].TestResultID() === "P" || self.ArrayValidate()[index].TestResultID() === "I") && self.ArrayValidate()[index].VirusTypeID() === "9" && (self.ArrayValidate()[index].OtherVirusTypeID() === "" || self.ArrayValidate()[index].OtherVirusTypeID() == undefined) && self.ArrayValidate()[index].OtherVirus() === "")
                {
                    msg += "\n" + msgValidateProcessOtherVirus;
                      //  Insert the other detected virus
                      //"Indique el otro virus detectado";
                }
                else {
                    if (self.ArrayValidate()[index].UsrCountry() == 7 && self.ArrayValidate()[index].CTOtherVirusType() === "" && self.ArrayValidate()[index].VirusTypeID() === "9" && self.ArrayValidate()[index].TestType() === "2" && self.ArrayValidate()[index].OtherVirusTypeID() != "")
                    { msg += "\n" + "Ingreso el CT de Otro Virus"; }
                }


                if (self.ArrayValidate()[index].TestResultID() === "P" && self.ArrayValidate()[index].VirusTypeID() === "1" && (self.ArrayValidate()[index].VirusSubTypeID() === "" || self.ArrayValidate()[index].VirusSubTypeID() == undefined) && self.ArrayValidate()[index].TestType() === "2")
                {
                    msg += "\n" + msgValidateProcessSubtype;
                      //  Insert the detected Subtype
                      //"Ingrese el Subtipo detectado";
                }
                else {
                    if ((self.ArrayValidate()[index].VirusSubTypeID() == 3 || self.ArrayValidate()[index].VirusSubTypeID() == 10) && self.ArrayValidate()[index].UsrCountry() == 7 && self.ArrayValidate()[index].CTSubType() === "" && self.ArrayValidate()[index].VirusSubTypeID() != "1" && self.ArrayValidate()[index].VirusTypeID() === "1" && self.ArrayValidate()[index].TestType() === "2")
                    { msg += "\n" + "Ingreso el CT de Subtipo"; }
                }

                if (self.ArrayValidate()[index].LabID() == self.ISPID() || self.UsrCountry() != 7) {
                    if (self.ArrayValidate()[index].TestResultID() === "P" && self.ArrayValidate()[index].VirusTypeID() === "2" && self.ArrayValidate()[index].VirusLineageID() === "" && self.ArrayValidate()[index].TestType() === "2")
                    {
                        msg += "\n" + msgValidateProcessLinage;
                          //Insert the detected Linage
                          //"Ingrese el Linaje detectado";
                    }
                    else {
                        if ((self.ArrayValidate()[index].VirusLineageID() == 2 || self.ArrayValidate()[index].VirusLineageID() == 3) && self.ArrayValidate()[index].UsrCountry() == 7 && self.ArrayValidate()[index].CTLineage() === "" && self.ArrayValidate()[index].VirusTypeID() === "2" && self.ArrayValidate()[index].TestType() === "2")
                        { msg += "\n" + "Ingreso el CT de Linaje"; }
                    }
                }
                

                if (self.ArrayValidate()[index].RNP() === "" && self.ArrayValidate()[index].TestType() === "2" && self.ArrayValidate()[index].UsrCountry() == 7)
                { msg += "\n" + "Ingrese el RNP"; }

            }
            
        };

        var ArrayValidate = $.grep(self.ArrayValidate(), function (v) {
            return (v.TestType() === "2" && v.ProcLab() == self.UsrInstID() && (v.LabID() == "" || v.LabID() == self.UsrInstID() ));
        });

        if (SaveVerif == 1 && ArrayValidate.length > 1)
            ArrayValidate.length = ArrayValidate.length - 1;

        if (ArrayValidate.length >= 4)
            msg += "\n" + msgValidateProcessPCR3times;
                //For PCR can not be more than 3 processes for the same laboratory
                //"Para PCR no puede ser mayor a 3 procesos para el mismo laboratorio";

        ArrayValidate = $.grep(self.ArrayValidate(), function (v) {
            return (v.TestType() === "1" && v.ProcLab() == self.UsrInstID());
        });
        if (ArrayValidate.length >= 3)
            msg += "\n" + msgValidateProcessIFI3times;
                //For IF it can not be more than 3 processes for the same laboratory
                //"Para IF no puede ser mayor a 3 procesos para el mismo laboratorio";
        
        return msg;
    };

    self.GetLab = function (id) {
        self.Id = id;       
        $.getJSON(app.dataModel.getLabUrl, { id: id }, function (data, status) {
            (data.RecDate) ? self.RecDate(moment(data.RecDate).clone().toDate()) : self.RecDate(null);
                self.Processed((data.Processed != null) ? data.Processed.toString() : "");
                self.NoProRen(data.NoProRen);
                self.NoProRenId(data.NoProRenId);
                self.TempSample1(data.TempSample1);
                self.hasGet(true);

                (data.RecDate2) ? self.RecDate2(moment(data.RecDate2).clone().toDate()) : self.RecDate2(null);
                self.Processed2((data.Processed2 != null) ? data.Processed2.toString() : "");
                self.NoProRen2(data.NoProRen2);
                self.NoProRenId2(data.NoProRenId2);
                self.TempSample2(data.TempSample2);

                (data.RecDate3) ? self.RecDate3(moment(data.RecDate3).clone().toDate()) : self.RecDate3(null);
                self.Processed3((data.Processed3 != null) ? data.Processed3.toString() : "");
                self.NoProRen3(data.NoProRen3);
                self.NoProRenId3(data.NoProRenId3);
                self.TempSample3(data.TempSample3);

                (data.EndLabDate) ? self.EndLabDate(moment(data.EndLabDate).clone().toDate()) : self.EndLabDate(null);
                self.FResult(data.FResult);
                self.Comments(data.Comments);
                self.FinalResult(data.FinalResult);
                self.FinalResultVirusTypeID(data.FinalResultVirusTypeID);
                self.FinalResultVirusSubTypeID(data.FinalResultVirusSubTypeID);
                self.FinalResultVirusLineageID(data.FinalResultVirusLineageID);
                self.FinalResult_2(data.FinalResult_2);
                self.FinalResultVirusTypeID_2(data.FinalResultVirusTypeID_2);
                self.FinalResultVirusSubTypeID_2(data.FinalResultVirusSubTypeID_2);
                self.FinalResultVirusLineageID_2(data.FinalResultVirusLineageID_2);
                self.FinalResult_3(data.FinalResult_3);
                self.FinalResultVirusTypeID_3(data.FinalResultVirusTypeID_3);
                self.FinalResultVirusSubTypeID_3(data.FinalResultVirusSubTypeID_3);
                self.FinalResultVirusLineageID_3(data.FinalResultVirusLineageID_3);
            // Comentariado porque el laboratorio no puede cerrar el caso
                //if (self.FinalResult() != "" ) {
                //    $("#CaseStatus").attr("disabled", false);
                //} else
                //{
                //    $("#CaseStatus").attr("disabled", true);
                //}
                self.LabsResult(data.LabsResult);
                self.SubTypeByLabRes(data.SubTypeByLabRes);
                self.CanConclude(data.CanConclude);
                app.Views.Home.CanConclude(data.CanConclude);
                app.Views.Home.SaveAndAdd_1(data.SaveAndAdd_1);
                app.Views.Home.SaveAndAdd_2(data.SaveAndAdd_2);
                app.Views.Home.SaveAndAdd_3(data.SaveAndAdd_3);
                //app.Views.Contact.flow_record(data.flow_record);
                //app.Views.Contact.flow_institution(data.flow_institution);
                //$("#o_S").val(data.DataStatement);
                self.CanIFILab(data.CanIFILab);
                self.CanPCRLab(data.CanPCRLab);
                self.LabTests([]);
                if (data.LabTests != "") {                  
                    for (index = 0; index < data.LabTests.length; ++index) {
                        var labtest = new LabTest(1);
                        labtest.Id = data.LabTests[index].Id;
                        labtest.CaseLabID = self.Id;
                        labtest.CanEdit(data.LabTests[index].CanEdit);
                        //console.log(data.LabTests[index].CanEdit);
                        labtest.CanPCR((typeof data.LabTests[index].CanPCR === "undefined" ) ? false : data.LabTests[index].CanPCR );
                        labtest.CanIFI((typeof data.LabTests[index].CanIFI === "undefined") ? false : data.LabTests[index].CanIFI);
                        labtest.EndFlow(data.LabTests[index].EndFlow);
                        labtest.ProcLab(data.LabTests[index].ProcLab.toString());
                        labtest.LabID(data.LabTests[index].ProcLab.toString());
                        labtest.ProcLabName(data.LabTests[index].ProcLabName.toString());
                        labtest.ProcessLab(data.LabTests[index].ProcessLab.toString());
                        labtest.TestType(data.LabTests[index].TestType);
                        labtest.SampleNumber(1);
                        if (data.LabTests[index].TestDate)
                            labtest.TestDate(moment(data.LabTests[index].TestDate).clone().toDate());
                        labtest.TestResultID((app.Views.Home.UsrCountry() == 7 && data.LabTests[index].TestType == 1 && data.LabTests[index].TestResultID == 'N' && data.LabTests[index].VirusTypeID == 1) ? 'NA' : (app.Views.Home.UsrCountry() == 7 && data.LabTests[index].TestType == 1 && data.LabTests[index].TestResultID == 'N' && data.LabTests[index].VirusTypeID == 2) ? 'NB' : data.LabTests[index].TestResultID);
                        labtest.VirusTypeID((app.Views.Home.UsrCountry() == 7 && data.LabTests[index].TestType == 1  && data.LabTests[index].TestResultID == 'N') ? null : data.LabTests[index].VirusTypeID);
                        labtest.CTVirusType(data.LabTests[index].CTVirusType);
                        labtest.CTRLVirusType(data.LabTests[index].CTRLVirusType);
                        labtest.OtherVirusTypeID(data.LabTests[index].OtherVirusTypeID);
                        labtest.CTOtherVirusType(data.LabTests[index].CTOtherVirusType);
                        labtest.CTRLOtherVirusType(data.LabTests[index].CTRLOtherVirusType);
                        labtest.OtherVirus(data.LabTests[index].OtherVirus);
                        labtest.InfA(data.LabTests[index].InfA);
                        labtest.VirusSubTypeID(data.LabTests[index].VirusSubTypeID);
                        //console.log(data.LabTests[index].CTSubType);
                        labtest.CTSubType(data.LabTests[index].CTSubType);
                        labtest.CTRLSubType(data.LabTests[index].CTRLSubType);
                        labtest.TestResultID_VirusSubType(data.LabTests[index].TestResultID_VirusSubType);
                        labtest.VirusSubTypeID_2(data.LabTests[index].VirusSubTypeID_2);
                        labtest.CTSubType_2(data.LabTests[index].CTSubType_2);
                        labtest.CTRLSubType_2(data.LabTests[index].CTRLSubType_2);
                        labtest.TestResultID_VirusSubType_2(data.LabTests[index].TestResultID_VirusSubType_2);
                        labtest.InfB(data.LabTests[index].InfB);
                        labtest.VirusLineageID(data.LabTests[index].VirusLineageID);
                        labtest.CTLineage(data.LabTests[index].CTLineage);
                        labtest.CTRLLineage(data.LabTests[index].CTRLLineage);
                        labtest.ParaInfI(data.LabTests[index].ParaInfI);
                        labtest.ParaInfII(data.LabTests[index].ParaInfII);
                        labtest.ParaInfIII(data.LabTests[index].ParaInfIII);
                        labtest.RSV(data.LabTests[index].RSV);
                        labtest.Adenovirus(data.LabTests[index].Adenovirus);
                        labtest.Metapneumovirus(data.LabTests[index].Metapneumovirus);
                        labtest.RNP(data.LabTests[index].RNP);
                        labtest.CTRLRNP(data.LabTests[index].CTRLRNP);
                        labtest.CTRLNegative(data.LabTests[index].CTRLNegative);
                        if (data.LabTests[index].TestEndDate)
                            labtest.TestEndDate(moment(data.LabTests[index].TestEndDate).clone().toDate());
                        self.LabTests.push(labtest);
                    }
                }

                self.LabTests_Sample2([]);
                if (data.LabTests_Sample2 != "") {
                    for (index = 0; index < data.LabTests_Sample2.length; ++index) {
                        var labtest_s2 = new LabTest(2);
                        labtest_s2.Id = data.LabTests_Sample2[index].Id;
                        labtest_s2.CaseLabID = self.Id;
                        labtest_s2.CanEdit(data.LabTests_Sample2[index].CanEdit);
                        labtest_s2.CanPCR((typeof data.LabTests_Sample2[index].CanPCR === "undefined") ? false : data.LabTests_Sample2[index].CanPCR);
                        labtest_s2.CanIFI((typeof data.LabTests_Sample2[index].CanIFI === "undefined") ? false : data.LabTests_Sample2[index].CanIFI);
                        labtest_s2.ProcLab(data.LabTests_Sample2[index].ProcLab.toString());
                        labtest_s2.LabID(data.LabTests_Sample2[index].ProcLab.toString());
                        labtest_s2.ProcLabName(data.LabTests_Sample2[index].ProcLabName.toString());
                        labtest_s2.ProcessLab(data.LabTests_Sample2[index].ProcessLab.toString());
                        labtest_s2.TestType(data.LabTests_Sample2[index].TestType);
                        labtest_s2.SampleNumber(2);
                        if (data.LabTests_Sample2[index].TestDate)
                            labtest_s2.TestDate(moment(data.LabTests_Sample2[index].TestDate).clone().toDate());
                        //labtest_s2.TestResultID(data.LabTests_Sample2[index].TestResultID);
                        //labtest_s2.VirusTypeID(data.LabTests_Sample2[index].VirusTypeID);
                        labtest_s2.TestResultID((app.Views.Home.UsrCountry() == 7 && data.LabTests_Sample2[index].TestType == 1 && data.LabTests_Sample2[index].TestResultID == 'N' && data.LabTests_Sample2[index].VirusTypeID == 1) ? 'NA' : (app.Views.Home.UsrCountry() == 7 && data.LabTests_Sample2[index].TestType == 1 && data.LabTests_Sample2[index].TestResultID == 'N' && data.LabTests_Sample2[index].VirusTypeID == 2) ? 'NB' : data.LabTests_Sample2[index].TestResultID);
                        labtest_s2.VirusTypeID((app.Views.Home.UsrCountry() == 7 && data.LabTests_Sample2[index].TestType == 1 && data.LabTests_Sample2[index].TestResultID == 'N') ? null : data.LabTests_Sample2[index].VirusTypeID);
                        labtest_s2.CTVirusType(data.LabTests_Sample2[index].CTVirusType);
                        labtest_s2.CTRLVirusType(data.LabTests_Sample2[index].CTRLVirusType);
                        labtest_s2.OtherVirusTypeID(data.LabTests_Sample2[index].OtherVirusTypeID);
                        labtest_s2.CTOtherVirusType(data.LabTests_Sample2[index].CTOtherVirusType);
                        labtest_s2.CTRLOtherVirusType(data.LabTests_Sample2[index].CTRLOtherVirusType);
                        labtest_s2.OtherVirus(data.LabTests_Sample2[index].OtherVirus);
                        labtest_s2.InfA(data.LabTests_Sample2[index].InfA);
                        labtest_s2.VirusSubTypeID(data.LabTests_Sample2[index].VirusSubTypeID);
                        labtest_s2.CTSubType(data.LabTests_Sample2[index].CTSubType);
                        labtest_s2.CTRLSubType(data.LabTests_Sample2[index].CTRLSubType);
                        labtest_s2.InfB(data.LabTests_Sample2[index].InfB);
                        labtest_s2.VirusLineageID(data.LabTests_Sample2[index].VirusLineageID);
                        labtest_s2.CTLineage(data.LabTests_Sample2[index].CTLineage);
                        labtest_s2.CTRLLineage(data.LabTests_Sample2[index].CTRLLineage);
                        labtest_s2.ParaInfI(data.LabTests_Sample2[index].ParaInfI);
                        labtest_s2.ParaInfII(data.LabTests_Sample2[index].ParaInfII);
                        labtest_s2.ParaInfIII(data.LabTests_Sample2[index].ParaInfIII);
                        labtest_s2.RSV(data.LabTests_Sample2[index].RSV);
                        labtest_s2.Adenovirus(data.LabTests_Sample2[index].Adenovirus);
                        labtest_s2.Metapneumovirus(data.LabTests_Sample2[index].Metapneumovirus);
                        labtest_s2.RNP(data.LabTests_Sample2[index].RNP);
                        labtest_s2.CTRLRNP(data.LabTests_Sample2[index].CTRLRNP);
                        labtest_s2.CTRLNegative(data.LabTests_Sample2[index].CTRLNegative);
                        if (data.LabTests_Sample2[index].TestEndDate)
                            labtest_s2.TestEndDate(moment(data.LabTests_Sample2[index].TestEndDate).clone().toDate());
                        self.LabTests_Sample2.push(labtest_s2);
                    }
                }

                self.LabTests_Sample3([]);
                if (data.LabTests_Sample3 != "") {
                    for (index = 0; index < data.LabTests_Sample3.length; ++index) {
                        var labtest_s3 = new LabTest(3);
                        labtest_s3.Id = data.LabTests_Sample3[index].Id;
                        labtest_s3.CaseLabID = self.Id;
                        labtest_s3.CanEdit(data.LabTests_Sample3[index].CanEdit);
                        labtest_s3.CanPCR((typeof data.LabTests_Sample3[index].CanPCR === "undefined") ? false : data.LabTests_Sample3[index].CanPCR);
                        labtest_s3.CanIFI((typeof data.LabTests_Sample3[index].CanIFI === "undefined") ? false : data.LabTests_Sample3[index].CanIFI);
                        labtest_s3.ProcLab(data.LabTests_Sample3[index].ProcLab.toString());
                        labtest_s3.LabID(data.LabTests_Sample3[index].ProcLab.toString());
                        labtest_s3.ProcLabName(data.LabTests_Sample3[index].ProcLabName.toString());
                        labtest_s3.ProcessLab(data.LabTests_Sample3[index].ProcessLab.toString());
                        labtest_s3.TestType(data.LabTests_Sample3[index].TestType);
                        labtest_s3.SampleNumber(3);
                        if (data.LabTests_Sample3[index].TestDate)
                            labtest_s3.TestDate(moment(data.LabTests_Sample3[index].TestDate).clone().toDate());
                        //labtest_s3.TestResultID(data.LabTests_Sample3[index].TestResultID);
                        //labtest_s3.VirusTypeID(data.LabTests_Sample3[index].VirusTypeID);
                        labtest_s3.TestResultID((app.Views.Home.UsrCountry() == 7 && data.LabTests_Sample3[index].TestType == 1 && data.LabTests_Sample3[index].TestResultID == 'N' && data.LabTests_Sample3[index].VirusTypeID == 1) ? 'NA' : (app.Views.Home.UsrCountry() == 7 && data.LabTests_Sample3[index].TestType == 1 && data.LabTests_Sample3[index].TestResultID == 'N' && data.LabTests_Sample3[index].VirusTypeID == 2) ? 'NB' : data.LabTests_Sample3[index].TestResultID);
                        labtest_s3.VirusTypeID((app.Views.Home.UsrCountry() == 7 && data.LabTests_Sample3[index].TestType == 1 && data.LabTests_Sample3[index].TestResultID == 'N') ? null : data.LabTests_Sample3[index].VirusTypeID);
                        labtest_s3.CTVirusType(data.LabTests_Sample3[index].CTVirusType);
                        labtest_s3.CTRLVirusType(data.LabTests_Sample3[index].CTRLVirusType);
                        labtest_s3.OtherVirusTypeID(data.LabTests_Sample3[index].OtherVirusTypeID);
                        labtest_s3.CTOtherVirusType(data.LabTests_Sample3[index].CTOtherVirusType);
                        labtest_s3.CTRLOtherVirusType(data.LabTests_Sample3[index].CTRLOtherVirusType);
                        labtest_s2.OtherVirus(data.LabTests_Sample3[index].OtherVirus);
                        labtest_s3.InfA(data.LabTests_Sample3[index].InfA);
                        labtest_s3.VirusSubTypeID(data.LabTests_Sample3[index].VirusSubTypeID);
                        labtest_s3.CTSubType(data.LabTests_Sample3[index].CTSubType);
                        labtest_s3.CTRLSubType(data.LabTests_Sample3[index].CTRLSubType);
                        labtest_s3.InfB(data.LabTests_Sample3[index].InfB);
                        labtest_s3.VirusLineageID(data.LabTests_Sample3[index].VirusLineageID);
                        labtest_s3.CTLineage(data.LabTests_Sample3[index].CTLineage);
                        labtest_s3.CTRLLineage(data.LabTests_Sample3[index].CTRLLineage);
                        labtest_s3.ParaInfI(data.LabTests_Sample3[index].ParaInfI);
                        labtest_s3.ParaInfII(data.LabTests_Sample3[index].ParaInfII);
                        labtest_s3.ParaInfIII(data.LabTests_Sample3[index].ParaInfIII);
                        labtest_s3.RSV(data.LabTests_Sample3[index].RSV);
                        labtest_s3.Adenovirus(data.LabTests_Sample3[index].Adenovirus);
                        labtest_s3.Metapneumovirus(data.LabTests_Sample3[index].Metapneumovirus);
                        labtest_s3.RNP(data.LabTests_Sample3[index].RNP);
                        labtest_s3.CTRLRNP(data.LabTests_Sample3[index].CTRLRNP);
                        labtest_s3.CTRLNegative(data.LabTests_Sample3[index].CTRLNegative);
                        if (data.LabTests_Sample3[index].TestEndDate)
                            labtest_s3.TestEndDate(moment(data.LabTests_Sample3[index].TestEndDate).clone().toDate());
                        self.LabTests_Sample3.push(labtest_s3);
                    }
                }

                $("#FinalResult").prop('disabled', true);
                $("#FinalResultVirusTypeID").prop('disabled', true);
                $("#FinalResultVirusSubTypeID").prop('disabled', true);
                $("#FinalResultVirusLineageID").prop('disabled', true);
                $("#FinalResult_2").prop('disabled', true);
                $("#FinalResultVirusTypeID_2").prop('disabled', true);
                $("#FinalResultVirusSubTypeID_2").prop('disabled', true);
                $("#FinalResultVirusLineageID_2").prop('disabled', true);
                $("#FinalResult_3").prop('disabled', true);
                $("#FinalResultVirusTypeID_3").prop('disabled', true);
                $("#FinalResultVirusSubTypeID_3").prop('disabled', true);
                $("#FinalResultVirusLineageID_3").prop('disabled', true);

                self.hasGet(false);
                if (self.FinalResult() == "") {
                    self.OrdenFinalResult();
                };

            })
             .fail(function (jqXHR, textStatus, errorThrown) {
                 alert(errorThrown);
             })
    };


    self.OrdenFinalResult = function () {

        if (self.hasGet() == false) {
            self.OrderArrayFinalResult([]);
            self.OrderDummy([]);
            self.resetFinalResult();
            self.OrderArrayFinalResult(self.LabTests().concat(self.LabTests_Sample2()).concat(self.LabTests_Sample3()));
            self.OrderDummy(self.OrderArrayFinalResult.sort(self.generateSortFn([{ name: 'OrdenTestResultID' }, { name: 'OrdenVirusTypeID' }, { name: 'OrdenTestResultID_VirusSubType' }, { name: 'OrdenTestResultID_VirusSubType_2' }, { name: 'OrdenSubTypeID' }, { name: 'OrdenLineageID' }, { name: 'OrdenTestType' }])));

            self.OrderArrayFinalResult([]);

            self.OrderDummy().forEach(function (v, i) {

                if (i == 0) {
                    //console.log(v);
                    self.OrderArrayFinalResult.push(v);
                } else if (self.OrderDummy()[i - 1].VirusTypeID() != v.VirusTypeID() && v.TestResultID() != "N" && v.TestResultID() != "NA" && v.TestResultID() != "NB") {
                      self.OrderArrayFinalResult.push(v);
                }
            });

            self.OrderArrayFinalResult().forEach(function (v, i) {                
                if (i == 0) {
                    self.EndLabDate(v.TestEndDate());
                    self.FinalResult((v.TestResultID() == "NA" || v.TestResultID() == "NB") ? 'N' : v.TestResultID());
                    if (v.TestResultID() != "N" && v.TestResultID() != "NA" && v.TestResultID() != "NB") {
                        self.FinalResultVirusTypeID(v.VirusTypeID());
                        if (v.TestResultID_VirusSubType() ==  "P")
                            self.FinalResultVirusSubTypeID(v.VirusSubTypeID());
                        if (v.TestResultID_VirusSubType_2() == "P")
                            self.FinalResultVirusSubTypeID(v.VirusSubTypeID_2());
                        self.FinalResultVirusLineageID(v.VirusLineageID());
                    }
                    
                }
                if (i == 1) {
                    self.FinalResult_2((v.TestResultID() == "NA" || v.TestResultID() == "NB") ? 'N' : v.TestResultID());
                    if (v.TestResultID() != "N" && v.TestResultID() != "NA" && v.TestResultID() != "NB") {
                        self.FinalResultVirusTypeID_2(v.VirusTypeID());
                        self.FinalResultVirusSubTypeID_2(v.VirusSubTypeID());
                        self.FinalResultVirusLineageID_2(v.VirusLineageID());
                    }
                }
                if (i == 2) {
                    self.FinalResult_3((v.TestResultID() == "NA" || v.TestResultID() == "NB") ? 'N' : v.TestResultID());
                    if (v.TestResultID() != "N" && v.TestResultID() != "NA" && v.TestResultID() != "NB") {
                        self.FinalResultVirusTypeID_3(v.VirusTypeID());
                        self.FinalResultVirusSubTypeID_3(v.VirusSubTypeID());
                        self.FinalResultVirusLineageID_3(v.VirusLineageID());
                    }
                }
            });

            $("#FinalResult").prop('disabled', true);
            $("#FinalResultVirusTypeID").prop('disabled', true);
            $("#FinalResultVirusSubTypeID").prop('disabled', true);
            $("#FinalResultVirusLineageID").prop('disabled', true);
            $("#FinalResult_2").prop('disabled', true);
            $("#FinalResultVirusTypeID_2").prop('disabled', true);
            $("#FinalResultVirusSubTypeID_2").prop('disabled', true);
            $("#FinalResultVirusLineageID_2").prop('disabled', true);
            $("#FinalResult_3").prop('disabled', true);
            $("#FinalResultVirusTypeID_3").prop('disabled', true);
            $("#FinalResultVirusSubTypeID_3").prop('disabled', true);
            $("#FinalResultVirusLineageID_3").prop('disabled', true);
            
        }
    };

    self.Save = function () {
        app.Views.Home.ValidateAll();
    };

    self.Cancel = function () {
        if (confirm("Usted esta apunto de salir del registro, está seguro?")) {
            app.Views.Home.CancelEdit();
        }
    };

    self.SaveLab = function (nextStep) {
        var postData = [];
        rec_date = jQuery.type(self.RecDate()) === 'date' ? self.RecDate() : parseDate($("#RecDate").val(), date_format_);
        rec_date2 = jQuery.type(self.RecDate2()) === 'date' ? self.RecDate2() : parseDate($("#RecDate2").val(), date_format_);
        rec_date3 = jQuery.type(self.RecDate3()) === 'date' ? self.RecDate3() : parseDate($("#RecDate3").val(), date_format_);
        date_close_date_lab = jQuery.type(self.EndLabDate()) === 'date' ? self.EndLabDate() : parseDate($("#EndLabDate").val(), date_format_);

        //alert($("#o_S").val());

        for (var index = 0; index < self.LabTests().length; ++index) {
            date_1 = self.LabTests()[index].TestDate();
            date_2 = self.LabTests()[index].TestEndDate();
            var date_test_start = new Date();
            var date_test_final = new Date();
            date_test_start = jQuery.type(date_1) === 'date' ? date_1 : parseDate(date_1, date_format_);
            date_test_final = jQuery.type(date_2) === 'date' ? date_2 : parseDate(date_2, date_format_);

            var newLabID = (self.LabTests()[index].LabID() === "" || typeof self.LabTests()[index].LabID() === "undefined") ?
                self.LabTests()[index].ProcLab() : self.LabTests()[index].LabID();

            postData.push({
                ID: self.LabTests()[index].Id,
                CaseLabID: self.Id,
                LabID: newLabID,
                Processed: self.LabTests()[index].ProcessLab(),
                SampleNumber: self.LabTests()[index].SampleNumber(),
                VirusTypeID: self.LabTests()[index].TestResultID() == 'NA' ? 1 : self.LabTests()[index].TestResultID() == 'NB' ? 2 : self.LabTests()[index].VirusTypeID(),
                CTVirusType: self.LabTests()[index].CTVirusType(),
                CTRLVirusType: self.LabTests()[index].CTRLVirusType(),
                OtherVirusTypeID: self.LabTests()[index].OtherVirusTypeID(),
                CTOtherVirusType: self.LabTests()[index].CTOtherVirusType(),
                CTRLOtherVirusType: self.LabTests()[index].CTRLOtherVirusType(),
                OtherVirus: self.LabTests()[index].OtherVirus(),
                InfA: self.LabTests()[index].InfA(),
                VirusSubTypeID: self.LabTests()[index].VirusSubTypeID(),
                CTSubType: self.LabTests()[index].CTSubType(),
                CTRLSubType: self.LabTests()[index].CTRLSubType(),
                TestResultID_VirusSubType: self.LabTests()[index].TestResultID_VirusSubType(),
                VirusSubTypeID_2: self.LabTests()[index].VirusSubTypeID_2(),
                CTSubType_2: self.LabTests()[index].CTSubType_2(),
                CTRLSubType_2: self.LabTests()[index].CTRLSubType_2(),
                TestResultID_VirusSubType_2: self.LabTests()[index].TestResultID_VirusSubType_2(),
                InfB: self.LabTests()[index].InfB(),
                VirusLineageID: self.LabTests()[index].VirusLineageID(),
                CTLineage: self.LabTests()[index].CTLineage(),
                CTRLLineage: self.LabTests()[index].CTRLLineage(),
                ParaInfI: self.LabTests()[index].ParaInfI(),
                ParaInfII: self.LabTests()[index].ParaInfII(),
                ParaInfIII: self.LabTests()[index].ParaInfIII(),
                RSV: self.LabTests()[index].RSV(),
                Adenovirus: self.LabTests()[index].Adenovirus(),
                Metapneumovirus: self.LabTests()[index].Metapneumovirus(),
                RNP: self.LabTests()[index].RNP(),
                CTRLRNP: self.LabTests()[index].CTRLRNP(),
                CTRLNegative: self.LabTests()[index].CTRLNegative(),
                TestResultID: self.LabTests()[index].TestResultID() == 'NA' || self.LabTests()[index].TestResultID() == 'NB' ? 'N' : self.LabTests()[index].TestResultID(),
                TestType: self.LabTests()[index].TestType(),
                TestDate: moment(date_test_start).format(date_format_ISO),
                TestEndDate: moment(date_test_final).format(date_format_ISO)
            });
        }
        //console.log(postData);
        // Muestra numero 2 dos
        for (var index = 0; index < self.LabTests_Sample2().length; ++index) {
            date_1 = self.LabTests_Sample2()[index].TestDate();
            date_2 = self.LabTests_Sample2()[index].TestEndDate();
            var date_test_start = new Date();
            var date_test_final = new Date();
            date_test_start = jQuery.type(date_1) === 'date' ? date_1 : parseDate(date_1, date_format_);
            date_test_final = jQuery.type(date_2) === 'date' ? date_2 : parseDate(date_2, date_format_);

            var newLabID = (self.LabTests_Sample2()[index].LabID() === "" || typeof self.LabTests_Sample2()[index].LabID() === "undefined") ?
                self.LabTests_Sample2()[index].ProcLab() : self.LabTests_Sample2()[index].LabID();

            postData.push({
                ID: self.LabTests_Sample2()[index].Id,
                CaseLabID: self.Id,
                LabID: newLabID,
                Processed: self.LabTests_Sample2()[index].ProcessLab(),
                SampleNumber: self.LabTests_Sample2()[index].SampleNumber(),
                //VirusTypeID: self.LabTests_Sample2()[index].VirusTypeID(),
                VirusTypeID: self.LabTests_Sample2()[index].TestResultID() == 'NA' ? 1 : self.LabTests_Sample2()[index].TestResultID() == 'NB' ? 2 : self.LabTests_Sample2()[index].VirusTypeID(),
                CTVirusType: self.LabTests_Sample2()[index].CTVirusType(),
                CTRLVirusType: self.LabTests_Sample2()[index].CTRLVirusType(),
                OtherVirusTypeID: self.LabTests_Sample2()[index].OtherVirusTypeID(),
                CTOtherVirusType: self.LabTests_Sample2()[index].CTOtherVirusType(),
                CTRLOtherVirusType: self.LabTests_Sample2()[index].CTRLOtherVirusType(),
                OtherVirus: self.LabTests_Sample2()[index].OtherVirus(),
                InfA: self.LabTests_Sample2()[index].InfA(),
                VirusSubTypeID: self.LabTests_Sample2()[index].VirusSubTypeID(),
                CTSubType: self.LabTests_Sample2()[index].CTSubType(),
                CTRLSubType: self.LabTests_Sample2()[index].CTRLSubType(),
                InfB: self.LabTests_Sample2()[index].InfB(),
                VirusLineageID: self.LabTests_Sample2()[index].VirusLineageID(),
                CTLineage: self.LabTests_Sample2()[index].CTLineage(),
                CTRLLineage: self.LabTests_Sample2()[index].CTRLLineage(),
                ParaInfI: self.LabTests_Sample2()[index].ParaInfI(),
                ParaInfII: self.LabTests_Sample2()[index].ParaInfII(),
                ParaInfIII: self.LabTests_Sample2()[index].ParaInfIII(),
                RSV: self.LabTests_Sample2()[index].RSV(),
                Adenovirus: self.LabTests_Sample2()[index].Adenovirus(),
                Metapneumovirus: self.LabTests_Sample2()[index].Metapneumovirus(),
                RNP: self.LabTests_Sample2()[index].RNP(),
                CTRLRNP: self.LabTests_Sample2()[index].CTRLRNP(),
                CTRLNegative: self.LabTests_Sample2()[index].CTRLNegative(),
                //TestResultID: self.LabTests_Sample2()[index].TestResultID(),
                TestResultID: self.LabTests_Sample2()[index].TestResultID() == 'NA' || self.LabTests_Sample2()[index].TestResultID() == 'NB' ? 'N' : self.LabTests_Sample2()[index].TestResultID(),  
                TestType: self.LabTests_Sample2()[index].TestType(),
                TestDate: moment(date_test_start).format(date_format_ISO),
                TestEndDate: moment(date_test_final).format(date_format_ISO)
            });
        }

        // Muestra 3 tres
        for (var index = 0; index < self.LabTests_Sample3().length; ++index) {
            date_1 = self.LabTests_Sample3()[index].TestDate();
            date_2 = self.LabTests_Sample3()[index].TestEndDate();
            var date_test_start = new Date();
            var date_test_final = new Date();
            date_test_start = jQuery.type(date_1) === 'date' ? date_1 : parseDate(date_1, date_format_);
            date_test_final = jQuery.type(date_2) === 'date' ? date_2 : parseDate(date_2, date_format_);

            var newLabID = (self.LabTests_Sample3()[index].LabID() === "" || typeof self.LabTests_Sample3()[index].LabID() === "undefined") ?
                self.LabTests_Sample3()[index].ProcLab() : self.LabTests_Sample3()[index].LabID();

            postData.push({
                ID: self.LabTests_Sample3()[index].Id,
                CaseLabID: self.Id,
                LabID: newLabID,
                Processed: self.LabTests_Sample3()[index].ProcessLab(),
                SampleNumber: self.LabTests_Sample3()[index].SampleNumber(),
                //VirusTypeID: self.LabTests_Sample3()[index].VirusTypeID(),
                VirusTypeID: self.LabTests_Sample3()[index].TestResultID() == 'NA' ? 1 : self.LabTests_Sample3()[index].TestResultID() == 'NB' ? 2 : self.LabTests_Sample3()[index].VirusTypeID(),
                CTVirusType: self.LabTests_Sample3()[index].CTVirusType(),
                CTRLVirusType: self.LabTests_Sample3()[index].CTRLVirusType(),
                OtherVirusTypeID: self.LabTests_Sample3()[index].OtherVirusTypeID(),
                CTOtherVirusType: self.LabTests_Sample3()[index].CTOtherVirusType(),
                CTRLOtherVirusType: self.LabTests_Sample3()[index].CTRLOtherVirusType(),
                OtherVirus: self.LabTests_Sample3()[index].OtherVirus(),
                InfA: self.LabTests_Sample3()[index].InfA(),
                VirusSubTypeID: self.LabTests_Sample3()[index].VirusSubTypeID(),
                CTSubType: self.LabTests_Sample3()[index].CTSubType(),
                CTRLSubType: self.LabTests_Sample3()[index].CTRLSubType(),
                InfB: self.LabTests_Sample3()[index].InfB(),
                VirusLineageID: self.LabTests_Sample3()[index].VirusLineageID(),
                CTLineage: self.LabTests_Sample3()[index].CTLineage(),
                CTRLLineage: self.LabTests_Sample3()[index].CTRLLineage(),
                ParaInfI: self.LabTests_Sample3()[index].ParaInfI(),
                ParaInfII: self.LabTests_Sample3()[index].ParaInfII(),
                ParaInfIII: self.LabTests_Sample3()[index].ParaInfIII(),
                RSV: self.LabTests_Sample3()[index].RSV(),
                Adenovirus: self.LabTests_Sample3()[index].Adenovirus(),
                Metapneumovirus: self.LabTests_Sample3()[index].Metapneumovirus(),
                RNP: self.LabTests_Sample3()[index].RNP(),
                CTRLRNP: self.LabTests_Sample3()[index].CTRLRNP(),
                CTRLNegative: self.LabTests_Sample3()[index].CTRLNegative(),
                //TestResultID: self.LabTests_Sample3()[index].TestResultID(),
                TestResultID: self.LabTests_Sample3()[index].TestResultID() == 'NA' || self.LabTests_Sample3()[index].TestResultID() == 'NB' ? 'N' : self.LabTests_Sample3()[index].TestResultID(),
                TestType: self.LabTests_Sample3()[index].TestType(),
                TestDate: moment(date_test_start).format(date_format_ISO),
                TestEndDate: moment(date_test_final).format(date_format_ISO)
            });
        }



        //return false
        //alert(self.Comments);
        $.ajax({
            url: app.dataModel.saveLabUrl,
            type: "POST",
            data: JSON.stringify({
                id: self.Id,
                RecDate: $("#RecDate").val() == "" ? null : moment(rec_date).format(date_format_ISO),
                Processed: self.Processed() === "true" ? true : self.Processed() === "false" ? false : null,
                NoProRen: self.NoProRen() ? self.NoProRen().toLocaleUpperCase() : "",
                NoProRenId: self.NoProRenId(),
                TempSample1: self.TempSample1(),

                RecDate2: $("#RecDate2").val() == "" ? null : moment(rec_date2).format(date_format_ISO),
                Processed2: self.Processed2() === "true" ? true : self.Processed2() === "false" ? false : null,
                NoProRen2: self.NoProRen2() ? self.NoProRen2().toLocaleUpperCase() : "",
                NoProRenId2: self.NoProRenId2(),
                TempSample2: self.TempSample2(),

                RecDate3: $("#RecDate3").val() == "" ? null : moment(rec_date3).format(date_format_ISO),
                Processed3: self.Processed3() === "true" ? true : self.Processed3() === "false" ? false : null,
                NoProRen3: self.NoProRen3() ? self.NoProRen3().toLocaleUpperCase() : "",
                NoProRenId3: self.NoProRenId3(),
                TempSample3: self.TempSample3(),

                EndLabDate: $("#EndLabDate").val() == "" ? null : moment(date_close_date_lab).format(date_format_ISO),
                FResult: self.FResult(),          
                Comments: self.Comments() ? self.Comments().toLocaleUpperCase() : "",                  
                FinalResult: self.FinalResult(),
                FinalResultVirusTypeID: self.FinalResultVirusTypeID(),
                FinalResultVirusSubTypeID: self.FinalResultVirusSubTypeID(),
                FinalResultVirusLineageID: self.FinalResultVirusLineageID(),
                FinalResult_2: self.FinalResult_2(),
                FinalResultVirusTypeID_2: self.FinalResultVirusTypeID_2(),
                FinalResultVirusSubTypeID_2: self.FinalResultVirusSubTypeID_2(),
                FinalResultVirusLineageID_2: self.FinalResultVirusLineageID_2(),
                FinalResult_3: self.FinalResult_3(),
                FinalResultVirusTypeID_3: self.FinalResultVirusTypeID_3(),
                FinalResultVirusSubTypeID_3: self.FinalResultVirusSubTypeID_3(),
                FinalResultVirusLineageID_3: self.FinalResultVirusLineageID_3(),
                DataStatement: $("#o_S").val(),
                LabTests: postData
            }),
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (nextStep) nextStep();
                //alert(data);
            }
        });
        return true;
    };


}

app.addViewModel({
    name: "Lab",
    bindingMemberName: "lab",
    factory: LabViewModel
});