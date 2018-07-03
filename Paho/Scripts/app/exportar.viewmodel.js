function ExportarViewModel(app, dataModel) {
    var self = this;
    var date_format_moment = app.dataModel.date_format_ISO;
    self.UsrCountry = ko.observable(selcty);
    self.displayFilters = ko.observable(false);
    self.selectedCountryId = ko.observable(CountryID);
    self.countries = ko.observableArray(countries);
    self.institutions = ko.observableArray(institutions);
    self.reportsCountries = ko.observableArray(reportsCountries);
    self.regions = ko.observableArray(regions);
    self.selectedInstitutionId = ko.observable("");
    self.selectedRegionId = ko.observable("");
    self.selectedReportCountryId = ko.observable("");

    self.activecountries = ko.computed(function () {
        return $.grep(self.countries(), function (v) {
            return v.Active === true;
        });
    });

    self.ReloadInstitutions = function () {
        if (typeof self.selectedCountryId() === "undefined") {
            return;
        }
        self.loadInstitutions();
        //$("#HospitalsGroup").show();
    };

    self.loadInstitutions = function () {
        $.getJSON(app.dataModel.getInstitutionsUrl, { CountryID: self.selectedCountryId(), RegionID: self.selectedRegionId() }, function (data, status) {
            self.institutions(data);
            })
            .fail(function(jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            });
    };

    self.Report = ko.observable("Cases");

    self.YearFrom =  ko.observable("");
    self.YearTo =  ko.observable("");

    self.Year =  ko.observable("");
    self.Month =  ko.observable("");
    self.SE =  ko.observable("");
    self.StartDate = ko.observable(null);
    self.EndDate = ko.observable(null);
    self.Surv = ko.observable("");
    self.SurvInusual = ko.observable(false);
    //self.SurvInusual = ko.observable("2");      // Todos        //#### CAFQ

    self.validate = function (nextStep) {
        var msg = "";
        var selectCountryUsr = self.selectedCountryId() ? self.selectedCountryId() : CountryID

        if ($("#Hospitals").children().length <= 2 && self.selectedInstitutionId() == null)
        {
            if (self.selectedCountryId() == 25)
                msg += "\n" + " - You are required to enter the hospital";
            else
                msg += "\n" + " - Es requerido que ingrese la institución";
        }

        if ($("#Report").val() == "R1" || $("#Report").val() == "R2" || $("#Report").val() == "R3" || $("#Report").val() == "R4" ){
            if ( self.Year() == "" )
                if (self.selectedCountryId() == 25 || self.selectedCountryId() == 17)
                    msg += "\n" + " - Enter year of report generation";
                else
                    msg += "\n" + " - Ingrese el año de generación del reporte";
            if (self.Month() == "" && self.SE() == "" && self.StartDate() == "" && self.EndDate() == "" && self.Year() == "")
                if (self.selectedCountryId() == 25 || self.selectedCountryId() == 17)
                    msg += "\n" + " - Enter any restriction criteria for the report (Year, Month, WE, Start Date and End Date) "
                else
                    msg += "\n" + " - Ingrese algún critero de restricción del reporte (Año, Mes, SE, Fecha inicio y Fecha fin) "
        }

        if (msg !== "") {
            if (self.selectedCountryId() == 25 || self.selectedCountryId() == 17)
                alert('Reports:' + msg);
            else
                alert('Reportes:' + msg);
            return false;
        }

        return true;
    };

    self.exportar = function () {
        //var namevalues = { Report: self.Report(), CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), Month: self.Month(), SE: self.SE(), StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null, EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null, ReportCountry: self.selectedReportCountryId(), RegionID: self.selectedRegionId(), YearFrom: self.YearFrom(), YearTo: self.YearTo() }
        //#### CAFQ
        var namevalues = { Report: self.Report(), CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), Month: self.Month(), SE: self.SE(), StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null, EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null, ReportCountry: self.selectedReportCountryId(), RegionID: self.selectedRegionId(), YearFrom: self.YearFrom(),
            YearTo: self.YearTo(), Surv: self.Surv(), Inusual: self.SurvInusual() }            //#### CAFQ
        if(self.validate() == true)
            window.open(app.dataModel.getExportar + "?" + $.param(namevalues, true), "_blank");
    };

    self.IsReporteLabNPHL = ko.computed(function () {            //**** CAFQ
        if (self.selectedReportCountryId() == 79) {
            return true;
        } else {
            return false;
        }
    }, self);                           //**** CAFQ

    self.getCasosNPHL = function () {                           //**** CAFQ
        var _CountryID = self.selectedCountryId() ? self.selectedCountryId() : CountryID
        var _StartDate = self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null
        var _EndDate = self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null

        $.ajax({
            type: 'POST',
            url: 'Exportar/ListaCasosPorEnviarNPHL',
            data: "{'Report':'" + self.Report() + "'" +
                    ",'CountryID':" + _CountryID + "" +
                    ",'HospitalID':'" + self.selectedInstitutionId() + "'" +
                    ",'Year':'" + self.Year() + "'" +
                    ",'Month':'" + self.Month() + "'" +
                    ",'SE':'" + self.SE() + "'" +
                    ",'StartDate':'" + _StartDate + "'" +
                    ",'EndDate':'" + (self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null) + "'" +
                    ",'ReportCountry':'" + self.selectedReportCountryId() + "'" +
                    ",'RegionID':'" + self.selectedRegionId() + "'" +
                    ",'YearFrom':'" + self.YearFrom() + "'" +
                    ",'YearTo':'" + self.YearTo() + "'" +
                    ",'Surv':'" + self.Surv() + "'" +
                    ",'Inusual':'" + self.SurvInusual() + "'" +
                    "}",
            contentType: "application/json",
            dataType: 'json',
            success: function (response) {
                var json_obj = JSON.parse(response);
                jsonX = JSON.parse(response);

                var i = 0;
                var container = $('#modalBody');
                container.empty();
                $('<table />', { id: 'tableAntecedentes', class: 'table-own', style: 'width:500px;', border: 2, cellspacing: 10, cellpadding: 5 }).appendTo(container);

                var container = $('#tableAntecedentes');
                $('<tr />', { id: 'trheadersLabel', style: 'width:500px;', class: 'table-tr-own' }).appendTo(container);

                var container = $('#trheadersLabel');
                $('<td />', { id: 'tdheadersLabel1', colspan: 1, class: 'table-td-own', text: ' ' }).appendTo(container);
                $('<td />', { id: 'tdheadersLabel2', colspan: 1, class: 'table-td-own', text: 'Reg. No' }).appendTo(container);
                $('<td />', { id: 'tdheadersLabel3', colspan: 1, class: 'table-td-own', text: 'JPL LAB NUMBER' }).appendTo(container);
                $('<td />', { id: 'tdheadersLabel4', colspan: 1, class: 'table-td-own', text: 'First Name' }).appendTo(container);
                $('<td />', { id: 'tdheadersLabel5', colspan: 1, class: 'table-td-own', text: 'Last Name' }).appendTo(container);

                $.each(jsonX, function (index, element) {
                    ++i;

                    var container = $('#tableAntecedentes');
                    $('<tr />', { id: 'trheaders' + i.toString(), style: 'width:500px;', class: 'table-tr-own' }).appendTo(container);

                    var colspanThis = 1;
                    var container = $('#trheaders' + i.toString());
                    $('<td />', { id: 'tdheaders' + i.toString() + '0', colspan: colspanThis, style: 'width:30px;', class: 'table-td-own', text: element[0] }).appendTo(container);
                    $('<td />', { id: 'tdheaders' + i.toString() + '1', colspan: colspanThis, class: 'table-td-own', text: element[0] }).appendTo(container);
                    $('<td />', { id: 'tdheaders' + i.toString() + '2', colspan: colspanThis, class: 'table-td-own', text: element[1] }).appendTo(container);
                    $('<td />', { id: 'tdheaders' + i.toString() + '3', colspan: colspanThis, class: 'table-td-own', text: element[2] }).appendTo(container);
                    $('<td />', { id: 'tdheaders' + i.toString() + '4', colspan: colspanThis, class: 'table-td-own', text: element[3] }).appendTo(container);

                    var container = $('#tdheaders' + i.toString() + '0');
                    container.empty();
                    //$('<input />', { id:'inpCheck' + i.toString(), name:'inpCheck' + i.toString(), type:'checkbox', class:'table-td-own', value:'1' }).appendTo(container);
                    $('<input />', { id: 'inpCheck' + i.toString(), name: 'inpCheck' + i.toString(), type: 'checkbox', class: 'table-td-own', value: i.toString() }).appendTo(container);
                })
            }
        });
    };                           //**** CAFQ

    self.exportarNPHL = function () {                           //**** CAFQ
        var Casos = "";

        $("input:checkbox:checked").each(function () {
            var indice = ($(this).val()).toString()
            //alert($(this).attr("id"));
            caso = $('#tdheaders' + indice + '1').html()
            if (caso != "") {
                if (Casos == "")
                    Casos = caso;
                else
                    Casos = Casos + "#" + caso;
            }
        });

        if (Casos == "") {
            $('#myModal').modal('hide');                // Cerrando el modal
            alert("No items selected");
            return "";
        }

        var namevalues = {
            Report: self.Report(),
            CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID,
            HospitalID: self.selectedInstitutionId(),
            Year: self.Year(),
            Month: self.Month(),
            SE: self.SE(),
            StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null,
            EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null,
            ReportCountry: self.selectedReportCountryId(),
            RegionID: self.selectedRegionId(),
            YearFrom: self.YearFrom(),
            YearTo: self.YearTo(),
            Surv: self.Surv(),
            Inusual: self.SurvInusual(),
            CasosNPHL: Casos
        }

        if (self.validate() == true) {
            window.open(app.dataModel.getExportar + "?" + $.param(namevalues, true), "_blank");
            $('#myModal').modal('hide');                // Cerrando el modal
        }
    };                           //**** CAFQ

    self.url = ko.computed(function () {
        var namevalues = { Report: self.Report(), CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), Month: self.Month(), SE: self.SE(), StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null, EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null }
        return app.dataModel.getExportar + "?" + $.param(namevalues, true);
    });
    //self.Generate = function () {
    //    if (self.validate() == true) {
    //        var namevalues = { Report: self.Report(), CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), Month: self.Month(), SE: self.SE(), StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null, EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null }
    //        return app.dataModel.getExportar + "?" + $.param(namevalues, true);
    //    }
    //};


 };

app.addViewModel({
    name: "Exportar",
    bindingMemberName: "exportar",
    factory: ExportarViewModel
});