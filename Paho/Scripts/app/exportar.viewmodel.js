
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
                if (self.selectedCountryId() == 25)
                    msg += "\n" + " - Enter year of report generation";
                else
                    msg += "\n" + " - Ingrese el año de generación del reporte";
            if (self.Month() == "" && self.SE() == "" && self.StartDate() == "" && self.EndDate() == "" && self.Year() == "")
                if (self.selectedCountryId() == 25)
                    msg += "\n" + " - Enter any restriction criteria for the report (Year, Month, WE, Start Date and End Date) "
                else
                    msg += "\n" + " - Ingrese algún critero de restricción del reporte (Año, Mes, SE, Fecha inicio y Fecha fin) "
        }

        if (msg !== "") {
            if (self.selectedCountryId() == 25)
                alert('Reports:' + msg);
            else
                alert('Reportes:' + msg);
            return false;
        }

        return true;
    };

    self.exportar = function () {
        var namevalues = { Report: self.Report(), CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), Month: self.Month(), SE: self.SE(), StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null, EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null, ReportCountry:self.selectedReportCountryId(), RegionID:self.selectedRegionId(),YearFrom: self.YearFrom(),YearTo: self.YearTo() }
        if(self.validate() == true)
            window.open(app.dataModel.getExportar + "?" + $.param(namevalues, true), "_blank");
    };


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