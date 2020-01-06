function BaselineconfViewModel(app, dataModel) {
    console.log("BaselineconfViewModel->START");

    var self = this;

    self.Year = ko.observable();
    self.StartWeek = ko.observable();
    self.EndWeek = ko.observable();
    self.Title = ko.observable();

    self.countries = ko.observableArray(countries);
    self.selectedCountryId = ko.observable();

    self.ReloadParameters = function () {
        console.log("ReloadParameters->START");
        console.log(typeof self.selectedCountryId());


        if (typeof self.selectedCountryId() === "undefined") {
            self.ResetParameters();
            return;
        }

        self.loadParameters();

        //$("#HospitalsGroup").show();
        console.log("ReloadParameters->END");
    };

    self.loadParameters = function () {
        console.log("loadParameters->START");
        $.getJSON(app.dataModel.getBaselineConfigUrl, { CountryID: self.selectedCountryId() }, function (data, status) {
            self.ResetParameters();

            var json_obj = JSON.parse(data);

            self.Year(json_obj.Year);
            self.StartWeek(json_obj.StartWeek);
            self.EndWeek(json_obj.TotalWeek);
            self.Title(json_obj.Title);



        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        });

        console.log("loadParameters->END");
    };

    self.ResetParameters = function () {
        console.log("ResetParameters->START");

        self.Year("");
        self.StartWeek("");
        self.EndWeek("");
        self.Title("");

        console.log("ResetParameters->END");
    }

    self.SaveParameters = function () {
        console.log("SaveParameters->START");
        console.log(self.selectedCountryId());
        //if ((typeof self.ticketSubject() != "undefined") && self.ticketSubject() != "" && (typeof self.ticketMsg() != "undefined") && self.ticketMsg() != "") {

        if ((typeof self.selectedCountryId() != "undefined")) {
            console.log("ID valido...");
            var formData = new FormData();

            //$('#attachmentLoader').show();
            //$('#newTicket').prop('disabled', true);

            formData.append("FileUpload", $('#ticketFile')[0].files[0]);
            //formData.append("FileUpload", $('#ticketFile2')[0].files[0]);
            //formData.append("FileUpload", $('#ticketFile3')[0].files[0]);

            formData.append("CountryID", self.selectedCountryId());
            formData.append("Year", self.Year());
            formData.append("StartWeek", self.StartWeek());
            formData.append("EndWeek", self.EndWeek());
            formData.append("Title", self.Title());

            console.log(formData);



            $.ajax({
                url: app.dataModel.saveBaselineConfigUrl,
                type: 'POST',
                async: true,
                data: formData,
                processData: false,
                contentType: false,// not json               
                complete: function (data) {
                    console.log(data);
                    //$('#attachmentLoader').css('display', 'none');
                    if (data.responseText == "1") {
                        alert("Se actualizo la plantilla de líneas basales exitosamente...");

                        self.selectedCountryId("");
                        self.ResetParameters();
                        //CleanForm();
                        //GetTicketList();
                    }
                    else {
                        alert("No se pudo actualizar la plantilla de líneas Basales");
                    }

                },
                error: function (response) {
                    console.log(response.responseText);
                    //$('#newTicket').prop('disabled', true);
                    //alert(response.responseText);
                    //alert("El ticket no se pudo ingresar, por favor, envíe un correo electrónico al administrador del sistema");
                    //$('#SubjectTicket').val('');
                    //$('#DescriptionTicket').val('');
                }
            });

        }
        else {
            console.log("NO PROCESADO...z");
        }
        console.log("SaveParameters->END");
    } // END SaveParameters
    //else {
    //    alert(msgTicketMissingValues);
    //}

    console.log("BaselineconfViewModel->END");
};

//}

app.addViewModel({
    name: "Baselineconf",
    bindingMemberName: "baselineconf",
    factory: BaselineconfViewModel
});


//var date_format_moment = app.dataModel.date_format_ISO;

//self.UsrCountry = ko.observable(selcty);
//self.displayFilters = ko.observable(false);
//self.displayFilters = ko.observable(true);                  //#### CAFQ: 180703


//self.selectedCountryId = ko.observable(CountryID);
//console.log("x11");
//console.log(self.countries());
//console.log("x12");
//self.activeCountries = ko.computed(function () {
//    console.log("bs1");
//    return $.grep(self.countries(), function (v) {
//        return v.Active === true;
//    });
//    console.log("bs2");
//});

//self.selectedAreaId = ko.observable("");
//self.institutions = ko.observableArray(institutions);
//self.reportsCountries = ko.observableArray(reportsCountries);
//self.regions = ko.observableArray(regions);
//self.selectedInstitutionId = ko.observable("");
//self.selectedRegionId = ko.observable("");
//self.selectedReportCountryId = ko.observable("");
//self.selectedReportCountryId.subscribe(function (newValue) {
//    // Reporte epidemiologico, Número de casos y % de hospitalizaciones por IRAG, Total fallecidos por IRAG y Casos por IRAG,
//    // Hospitalizaciones Totales
//    var reportID = ["2", "3", "4", "6", "10"];
//    bDisable = false;
//    self.Surv("0");                 // Seleccione

//    jQuery.each(reportsCountries, function (i, oRepo) {
//        if (self.selectedReportCountryId() == oRepo.Id) {
//            if (reportID.indexOf(oRepo.orden) != -1) {
//                bDisable = true;
//                self.Surv("1");                     // IRAG
//                return true;                        // Sale del each
//            }
//        }
//    });

//    $("#Surv").prop("disabled", bDisable);
//});
//self.Surv = ko.observable("");
//self.SurvInusual = ko.observable(false);
//self.Sentinel = ko.observable("");



//self.ActiveHON = ko.computed(function () {
//    return (self.UsrCountry() == 15) ? true : false;
//}, self);

//self.ReloadInstitutions = function () {
//    if (typeof self.selectedCountryId() === "undefined") {
//        return;
//    }
//    self.loadInstitutions();
//    //$("#HospitalsGroup").show();
//};

//self.loadInstitutions = function () {
//    $.getJSON(app.dataModel.getInstitutionsUrl, { CountryID: self.selectedCountryId(), RegionID: self.selectedRegionId() }, function (data, status) {
//        self.institutions(data);
//    })
//        .fail(function (jqXHR, textStatus, errorThrown) {
//            alert(errorThrown);
//        });
//};

//self.selectedAreaId.subscribe(function (NewAreaID) {
//    if (NewAreaID > 0) {
//        //("#Regionals").attr('disabled', true);
//        self.selectedRegionId(0);
//    }

//    self.loadInstitutions();
//    $("#caselist").trigger("reloadGrid");
//    $("#caselist_pend").trigger('reloadGrid');

//    if (NewAreaID == 0 && self.institutions().length > 1) {
//        self.selectedInstitutionId(0);
//    }
//});

//self.Report = ko.observable("Cases");

//self.YearFrom = ko.observable("");
//self.YearTo = ko.observable("");


//self.Month = ko.observable("");
//self.SE = ko.observable("");



//self.validate = function (nextStep) {
//    var msg = "";
//    var selectCountryUsr = self.selectedCountryId() ? self.selectedCountryId() : CountryID

//    if ($("#Reports").val() == "") {
//        msg += "\n" + " - " + msgViewExportarValidateSelectionReport
//    }

//    if ($("#Hospitals").children().length <= 2 && self.selectedInstitutionId() == null) {
//        msg += "\n" + " - " + msgViewExportarValidateSelectionHospital;
//    }

//    if ($("#Report").val() == "R1" || $("#Report").val() == "R2" || $("#Report").val() == "R3" || $("#Report").val() == "R4") {
//        if (self.Year() == "")
//            msg += "\n" + " - " + msgViewExportarValidateSelectionYear;

//        if (self.Month() == "" && self.SE() == "" && self.StartDate() == "" && self.EndDate() == "" && self.Year() == "")
//            msg += "\n" + " - " + msgViewExportarValidateSelectionCriteria;
//    }

//    if (msg !== "") {
//        alert(msgViewExportarLabelReports + ": " + msg)
//        return false;
//    }

//    return true;
//};

//self.exportar = function () {
//    var namevalues = {
//        Report: self.Report(), CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(),
//        Year: self.Year(), Month: self.Month(), SE: self.SE(), StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null,
//        EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null, ReportCountry: self.selectedReportCountryId(),
//        RegionID: self.selectedRegionId(), YearFrom: self.YearFrom(), YearTo: self.YearTo(), Surv: self.Surv(), Inusual: self.SurvInusual(),
//        Area: self.selectedAreaId(), Sentinel: self.Sentinel()
//    }
//    if (self.validate() == true)
//        window.open(app.dataModel.getExportar + "?" + $.param(namevalues, true), "_blank");
//};

//self.IsReporteLabNPHL = ko.computed(function () {            //**** CAFQ
//    if (self.selectedReportCountryId() == 79) {
//        return true;
//    } else {
//        return false;
//    }
//}, self);                           //**** CAFQ

//self.getCasosNPHL = function () {                           //**** CAFQ
//    var _CountryID = self.selectedCountryId() ? self.selectedCountryId() : CountryID
//    var _StartDate = self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null
//    var _EndDate = self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null

//    $.ajax({
//        type: 'POST',
//        url: 'Exportar/ListaCasosPorEnviarNPHL',
//        data: "{'Report':'" + self.Report() + "'" +
//                ",'CountryID':" + _CountryID + "" +
//                ",'HospitalID':'" + self.selectedInstitutionId() + "'" +
//                ",'Year':'" + self.Year() + "'" +
//                ",'Month':'" + self.Month() + "'" +
//                ",'SE':'" + self.SE() + "'" +
//                ",'StartDate':'" + _StartDate + "'" +
//                ",'EndDate':'" + (self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null) + "'" +
//                ",'ReportCountry':'" + self.selectedReportCountryId() + "'" +
//                ",'RegionID':'" + self.selectedRegionId() + "'" +
//                ",'YearFrom':'" + self.YearFrom() + "'" +
//                ",'YearTo':'" + self.YearTo() + "'" +
//                ",'Surv':'" + self.Surv() + "'" +
//                ",'Inusual':'" + self.SurvInusual() + "'" +
//                "}",
//        contentType: "application/json",
//        dataType: 'json',
//        success: function (response) {
//            var json_obj = JSON.parse(response);
//            jsonX = JSON.parse(response);

//            var i = 0;
//            var container = $('#modalBody');
//            container.empty();
//            $('<table />', { id: 'tableAntecedentes', class: 'table-own', style: 'width:500px;', border: 2, cellspacing: 10, cellpadding: 5 }).appendTo(container);

//            var container = $('#tableAntecedentes');
//            $('<tr />', { id: 'trheadersLabel', style: 'width:500px;', class: 'table-tr-own' }).appendTo(container);

//            var container = $('#trheadersLabel');
//            $('<td />', { id: 'tdheadersLabel1', colspan: 1, class: 'table-td-own', text: ' ' }).appendTo(container);
//            $('<td />', { id: 'tdheadersLabel2', colspan: 1, class: 'table-td-own', text: 'Reg. No' }).appendTo(container);
//            $('<td />', { id: 'tdheadersLabel3', colspan: 1, class: 'table-td-own', text: 'JPL LAB NUMBER' }).appendTo(container);
//            $('<td />', { id: 'tdheadersLabel4', colspan: 1, class: 'table-td-own', text: 'First Name' }).appendTo(container);
//            $('<td />', { id: 'tdheadersLabel5', colspan: 1, class: 'table-td-own', text: 'Last Name' }).appendTo(container);

//            $.each(jsonX, function (index, element) {
//                ++i;

//                var container = $('#tableAntecedentes');
//                $('<tr />', { id: 'trheaders' + i.toString(), style: 'width:500px;', class: 'table-tr-own' }).appendTo(container);

//                var colspanThis = 1;
//                var container = $('#trheaders' + i.toString());
//                $('<td />', { id: 'tdheaders' + i.toString() + '0', colspan: colspanThis, style: 'width:30px;', class: 'table-td-own', text: element[0] }).appendTo(container);
//                $('<td />', { id: 'tdheaders' + i.toString() + '1', colspan: colspanThis, class: 'table-td-own', text: element[0] }).appendTo(container);
//                $('<td />', { id: 'tdheaders' + i.toString() + '2', colspan: colspanThis, class: 'table-td-own', text: element[1] }).appendTo(container);
//                $('<td />', { id: 'tdheaders' + i.toString() + '3', colspan: colspanThis, class: 'table-td-own', text: element[2] }).appendTo(container);
//                $('<td />', { id: 'tdheaders' + i.toString() + '4', colspan: colspanThis, class: 'table-td-own', text: element[3] }).appendTo(container);

//                var container = $('#tdheaders' + i.toString() + '0');
//                container.empty();
//                //$('<input />', { id:'inpCheck' + i.toString(), name:'inpCheck' + i.toString(), type:'checkbox', class:'table-td-own', value:'1' }).appendTo(container);
//                $('<input />', { id: 'inpCheck' + i.toString(), name: 'inpCheck' + i.toString(), type: 'checkbox', class: 'table-td-own', value: i.toString() }).appendTo(container);
//            })
//        }
//    });
//};                           //**** CAFQ

//self.exportarNPHL = function () {                           //**** CAFQ
//    var Casos = "", casoNPHL = "";

//    $("input:checkbox:checked").each(function () {
//        var indice = ($(this).val()).toString()

//        caso = $('#tdheaders' + indice + '1').html()
//        casoNPHL = $('#tdheaders' + indice + '2').html()
//        if (caso != "") {
//            if (Casos == "")
//                Casos = caso;
//            else
//                Casos = Casos + "#" + caso;
//            Casos = Casos + ":" + casoNPHL
//        }
//    });

//    if (Casos == "") {
//        $('#myModal').modal('hide');                // Cerrando el modal
//        alert("No items selected");
//        return "";
//    }
//    //alert(Casos);
//    var namevalues = {
//        Report: self.Report(),
//        CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID,
//        HospitalID: self.selectedInstitutionId(),
//        Year: self.Year(),
//        Month: self.Month(),
//        SE: self.SE(),
//        StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null,
//        EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null,
//        ReportCountry: self.selectedReportCountryId(),
//        RegionID: self.selectedRegionId(),
//        YearFrom: self.YearFrom(),
//        YearTo: self.YearTo(),
//        Surv: self.Surv(),
//        Inusual: self.SurvInusual(),
//        CasosNPHL: Casos,
//        Area: self.selectedAreaId()
//    }

//    if (self.validate() == true) {
//        window.open(app.dataModel.getExportar + "?" + $.param(namevalues, true), "_blank");
//        $('#myModal').modal('hide');                // Cerrando el modal
//    }
//};                           //**** CAFQ

//self.url = ko.computed(function () {
//    var namevalues = { Report: self.Report(), CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), Month: self.Month(), SE: self.SE(), StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null, EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null }
//    return app.dataModel.getExportar + "?" + $.param(namevalues, true);
//});
////self.Generate = function () {
////    if (self.validate() == true) {
////        var namevalues = { Report: self.Report(), CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), Month: self.Month(), SE: self.SE(), StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null, EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null }
////        return app.dataModel.getExportar + "?" + $.param(namevalues, true);
////    }
////};

