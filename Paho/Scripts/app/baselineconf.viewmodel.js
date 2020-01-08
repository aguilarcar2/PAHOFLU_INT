function BaselineconfViewModel(app, dataModel) {
    //console.log("BaselineconfViewModel->START");
    var self = this;

    self.UsrCountry = ko.observable(selcty);

    self.Year = ko.observable();
    self.StartWeek = ko.observable();
    self.EndWeek = ko.observable();
    self.Title = ko.observable();
    self.StartYearDH = ko.observable();
    self.EndYearDH = ko.observable();    

    self.countries = ko.observableArray(countries);
    self.selectedCountryId = ko.observable();
    self.error = "";

    self.VisibleGetHistoricalData = ko.computed(function () {
        return (self.UsrCountry() == 32) ? true : false;
    }, self);

    self.Title = ko.computed(function () {
        var cTemp = null;
        
        if (typeof self.selectedCountryId() === "undefined") 
            cTemp = "";
        else {
            countryName = self.selectedCountryId();
            language = "SPA";

            //console.log(countries);
            countries.forEach(function (valor, indice, array) {
                if (valor.Id == self.selectedCountryId()) {
                    countryName = valor.Name;
                    language = valor.orden;
                }
            });

            if(language == "SPA")
                cTemp = "Líneas de base: " + countryName + ", porcentaje de positividad para influenza en " + self.Year() + " en comparación al período " +
                self.StartYearDH() + "-" + self.EndYearDH() + ". Semana epidemiológica " + self.StartWeek() + " a " + self.EndWeek();
            else
                cTemp = "Baseline: " + countryName + ", percentage of Hospital Admissions for Severe Acute Respiratory Illness (SARI " + self.Year() + ") (compared with " +
                self.StartYearDH() + "-" + self.EndYearDH() + ")"
        }

        return cTemp;
    }, self);

    self.ReloadParameters = function () {
        if (typeof self.selectedCountryId() === "undefined") {
            self.ResetParameters();
            return;
        };
        self.loadParameters();
    };

    self.loadParameters = function () {
        //console.log("loadParameters->START");
        $.getJSON(app.dataModel.getBaselineConfigUrl, { CountryID: self.selectedCountryId() }, function (data, status) {
            self.ResetParameters();

            var json_obj = JSON.parse(data);

            self.Year((new Date).getFullYear());            
            self.StartWeek(json_obj.StartWeek);
            self.EndWeek(json_obj.TotalWeek);
            //self.Title(json_obj.Title);
            self.StartYearDH(json_obj.StartYearDH);
            self.EndYearDH(json_obj.EndYearDH);
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        });
        //console.log("loadParameters->END");
    };

    self.ResetParameters = function () {
        //console.log("ResetParameters->START");
        self.Year("");
        self.StartWeek("");
        self.EndWeek("");
        //self.Title("");
        self.StartYearDH("");
        self.EndYearDH("");
        //console.log("ResetParameters->END");
    }

    self.SaveParameters = function () {
        //console.log("SaveParameters->START");
        if ((typeof self.selectedCountryId() != "undefined")) {
            if (self.ValidarDatos() == true) {

                var formData = new FormData();

                formData.append("FileUpload", $('#ticketFile')[0].files[0]);
                formData.append("CountryID", self.selectedCountryId());
                formData.append("Year", self.Year());
                formData.append("StartWeek", self.StartWeek());
                formData.append("EndWeek", self.EndWeek());
                formData.append("Title", self.Title());
                formData.append("StartYearDH", self.StartYearDH());
                formData.append("EndYearDH", self.EndYearDH());

                $.ajax({
                    url: app.dataModel.saveBaselineConfigUrl,
                    type: 'POST',
                    async: true,
                    data: formData,
                    processData: false,
                    contentType: false,// not json               
                    complete: function (data) {
                        //console.log(data);
                        //$('#attachmentLoader').css('display', 'none');
                        if (data.responseText == "1") {
                            alert("Se actualizo la plantilla de líneas basales exitosamente...");
                            self.selectedCountryId("");
                            self.ResetParameters();
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
            } else {
                alert("Error: " + self.error);
            }


        }
        //console.log("SaveParameters->END");
    } // END SaveParameters
    
    self.ValidarDatos = function () {
        self.error = ""
        var resultado = true;

        //if (self.Year() == "") {
        if ($.isNumeric(self.Year()) == false) {
            self.error = self.error + "\n" + "Ingrese un valor numérico en el año de la actual temporada";
            resultado = false;
        }

        //if (self.StartWeek() == "") {
        if ($.isNumeric(self.StartWeek()) == false) {
            self.error = self.error + "\n" + "Ingrese un valor numérico en la semana de inicio de la actual temporada";
            resultado = false;
        }

        //if (self.EndWeek() == "") {
        if ($.isNumeric(self.EndWeek()) == false) {
            self.error = self.error + "\n" + "Ingrese un valor numérico en el total de semanas de la actual temporada";
            resultado = false;
        }
        
        if ($.isNumeric(self.StartYearDH()) == false) {
            self.error = self.error + "\n" + "Ingrese un valor numérico en el año inicial del histórico de datos";
            resultado = false;
        }

        if ($.isNumeric(self.EndYearDH()) == false) {
            self.error = self.error + "\n" + "Ingrese un valor numérico en el año final del histórico de datos";
            resultado = false;
        }

        if ($.isNumeric(self.StartYearDH()) && $.isNumeric(self.EndYearDH())) {
            if (self.StartYearDH() > self.EndYearDH()) {
                self.error = self.error + "\n" + "El año inicial no puede ser mayor que el año final del histórico de datos";
                resultado = false;
            }
        }

        if ($.isNumeric(self.Year()) && $.isNumeric(self.EndYearDH())) {
            if (self.EndYearDH() >= self.Year()) {
                self.error = self.error + "\n" + "El año final del histórico de datos debe ser menor que el año de la actual temporada";
                resultado = false;
            }
        }

        return resultado;
    }

    //console.log("BaselineconfViewModel->END");
};

app.addViewModel({
    name: "Baselineconf",
    bindingMemberName: "baselineconf",
    factory: BaselineconfViewModel
});
