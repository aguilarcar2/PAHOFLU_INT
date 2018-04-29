﻿function GEOViewModel(app, dataModel) {
    var self = this;
    self.Id = "";
    self.hasReset = ko.observable(false);
    self.UsrCountry = ko.observable(app.Views.Home.UsrCountry()); // Pais del usuario logueado
    self.selectedCountryId = ko.observable();

    self.selectedCountryId.subscribe(function (newCountrySelect) {
        if (self.hasReset() != true) {
            if (self.UsrCountry() != self.selectedCountryId()) {
                $("#Area").attr("disabled", true);
                $("#provincia").attr("disabled", true);
                $("#Neighborhoods").attr("disabled", true);
            } else if (self.UsrCountry() == self.selectedCountryId()) {
                $("#Area").attr("disabled", false);
                $("#provincia").attr("disabled", false);
                $("#Neighborhoods").attr("disabled", false);      
            }
            
        }
        
    });
    self.selectedCountryOrigin = ko.observable();
    self.selectedAreaId = ko.observable();
    self.selectedStateId = ko.observable();
    self.selectedLocalId = ko.observable();
    self.selectedNeighborhoodId = ko.observable();
    self.UrbanRural = ko.observable(0);
    self.Address = ko.observable("");
    self.Areas = ko.observableArray();
    self.States = ko.observableArray();
    self.Areas2weeks = ko.observableArray();
    self.States2weeks = ko.observableArray();
    self.selectedCountryId2weeks = ko.observable();
    self.selectedAreaId2weeks = ko.observable();
    self.selectedStateId2weeks = ko.observable();
    self.selectedNeighborhoodId2weeks = ko.observable();
    self.Neighborhoods = ko.observableArray();
    self.Neighborhoods2weeks = ko.observableArray();
    self.Locals = ko.observableArray();
    self.Locals2weeks = ko.observableArray();
    self.PhoneNumber = ko.observable("");
    self.Latitude = ko.observable("");
    self.Longitude = ko.observable("");

    self.ActiveBOLCountry2weeks = ko.computed(function () {
        return (self.UsrCountry() == 3 && self.selectedCountryId2weeks() == app.Views.Contact.UsrCountry()) ? true : false;
    }, self);

    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7) ? true : false;
        
    }, self);

    self.ResetGEO = function () {
        self.hasReset(true);
        self.Id = "";
        //alert(app.Views.Home);
        //self.selectedCountryId() ? self.selectedCountryId() : (CountryID > 0) ? CountryID : 0
        self.selectedCountryId("");
        self.selectedCountryId(app.Views.Home.UsrCountry());
        self.selectedCountryOrigin("");
        self.selectedAreaId("");
        self.selectedStateId("");
        self.selectedLocalId("");
        self.selectedNeighborhoodId("");
        self.UrbanRural(0);
        self.selectedCountryId2weeks("");
        self.selectedAreaId2weeks("");
        self.selectedStateId2weeks("");
        self.selectedNeighborhoodId2weeks("");
        self.Address("");
        self.PhoneNumber("");
        self.Latitude("");
        self.Longitude("");
        self.hasReset(false);
    };

    self.ResetGEOInusual = function () {
        //self.hasReset(true);
        //self.Id = "";
        //alert(app.Views.Home);
        //self.selectedCountryId() ? self.selectedCountryId() : (CountryID > 0) ? CountryID : 0
        /*self.selectedCountryId("");
        self.selectedCountryId(app.Views.Home.UsrCountry());
        self.selectedCountryOrigin("");
        self.selectedAreaId("");
        self.selectedStateId("");
        self.selectedLocalId("");
        self.selectedNeighborhoodId("");*/
        self.UrbanRural(0);
        /*self.selectedCountryId2weeks("");
        self.selectedAreaId2weeks("");
        self.selectedStateId2weeks("");
        self.selectedNeighborhoodId2weeks("");
        self.Address("");
        self.PhoneNumber("");
        self.Latitude("");
        self.Longitude("");*/
        //self.hasReset(false);
    };

    self.GetGEO = function (id) {
        self.Id = id;
        $.getJSON(app.dataModel.getGEOUrl, { id: id }, function (data, status) {
                self.selectedCountryId(data.CountryID);
                if (data.UrbanRural) self.UrbanRural(data.UrbanRural);
                self.Address(data.Address);
                self.selectedCountryOrigin(data.CountryOrigin);
                self.PhoneNumber(data.PhoneNumber);
                self.Latitude(data.Latitude);
                self.Longitude(data.Longitude);
                self.ReloadAreas(function() {
                    self.selectedAreaId(data.AreaID);
                    self.ReloadStates(function () {
                        self.selectedStateId(data.StateID);
                        self.ReloadNeighborhoods(function () {
                            self.selectedNeighborhoodId(data.NeighborhoodID);
                        });
                        //self.ReloadLocals(function () {
                        //    self.selectedLocalId(data.LocalID);
                        //});
                    });
                });
                if (self.UsrCountry() == 3 && data.CountryID2weeks != "") {
                self.selectedCountryId2weeks(data.CountryID2weeks);
                self.ReloadAreas(function () {
                    self.selectedAreaId2weeks(data.AreaID2weeks);
                    self.ReloadStates2weeks(function () {
                        self.selectedStateId2weeks(data.StateID2weeks);
                        self.ReloadNeighborhoods2weeks(function () {
                            self.selectedNeighborhoodId2weeks(data.NeighborhoodID2weeks);
                        });
                        //self.ReloadLocals2weeks(function () {
                        //});
                    });
                });
             }
            })
         .fail(function (jqXHR, textStatus, errorThrown) {
             alert(errorThrown);
         })
    };

    self.ReloadAreas = function (select) {
        if (typeof self.selectedCountryId() === "undefined") {
            self.Areas("");
            return;
        }
        $.getJSON(app.dataModel.getAreasUrl, { CountryID: self.selectedCountryId() }, function (data, status) {
            self.Areas(data);
            if ($.isFunction(select)) select();
            })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
    };

    self.ReloadAreas2weeks = function (select) {
        if (self.UsrCountry() == 3) {
            if (typeof self.selectedCountryId2weeks() === "undefined") {
                self.Areas2weeks("");
                return;
            }
            $.getJSON(app.dataModel.getAreasUrl, { CountryID: self.selectedCountryId2weeks() }, function (data, status) {
                self.Areas2weeks(data);
                if ($.isFunction(select)) select();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            });
        }
    };
    self.ReloadStates = function (select) {    
        if (typeof self.selectedAreaId() === "undefined") {
            self.States("");
            return;
        }
        $.getJSON(app.dataModel.getStatesUrl, { AreaID: self.selectedAreaId() }, function (data, status) {
            self.States(data);
            if ($.isFunction(select)) select();
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
    };

    self.ReloadStates2weeks = function (select) {
        if (typeof self.selectedAreaId2weeks() === "undefined") {
            self.States2weeks("");
            return;
        }
        $.getJSON(app.dataModel.getStatesUrl, { AreaID: self.selectedAreaId2weeks() }, function (data, status) {
            self.States2weeks(data);
            if ($.isFunction(select)) select();
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
    };
    self.ReloadLocals = function (select) {    
        if (typeof self.selectedStateId() === "undefined") {
            self.Locals("");
            return;
        }
        $.getJSON(app.dataModel.getLocalsUrl, { StateID: self.selectedStateId() }, function (data, status) {
            self.Locals(data);
            if ($.isFunction(select)) select();
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
    };

    self.ReloadLocals2weeks = function (select) {
        if (typeof self.selectedStateId2weeks() === "undefined") {
            self.Locals2weeks("");
            return;
        }
        $.getJSON(app.dataModel.getLocalsUrl, { StateID: self.selectedStateId2weeks() }, function (data, status) {
            self.Locals2weeks(data);
            if ($.isFunction(select)) select();
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
    };
    self.ReloadNeighborhoods = function (select) {
        if (typeof self.selectedStateId() === "undefined") {
            self.Neighborhoods("");
            return;
        }
        $.getJSON(app.dataModel.getNeighborhoodsUrl, { StateID: self.selectedStateId() }, function (data, status) {
            self.Neighborhoods(data);
            if ($.isFunction(select)) select();
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
    };

    self.ReloadNeighborhoods2weeks = function (select) {
        if (typeof self.selectedStateId2weeks() === "undefined") {
            self.Neighborhoods2weeks("");
            return;
        }
        $.getJSON(app.dataModel.getNeighborhoodsUrl, { StateID: self.selectedStateId2weeks() }, function (data, status) {
            self.Neighborhoods2weeks(data);
            if ($.isFunction(select)) select();
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
    };

    self.ReloadLocalsAndNeighborhoods = function () {
        //self.ReloadLocals();
        self.ReloadNeighborhoods();
    };

    self.ReloadLocalsAndNeighborhoods2weeks = function () {
        //self.ReloadLocals2weeks();
        self.ReloadNeighborhoods2weeks();
    };

    self.validate = function (nextStep) {
        var msg = "";
        if (!self.selectedCountryId() || self.selectedCountryId() == "")
            msg += "\n" + viewValidateCountryRequired;
        if (self.UsrCountry() == self.selectedCountryId()) {
            if (!self.selectedAreaId() || self.selectedAreaId() == "") {
                msg += "\n" + msgValidationAreaRequired;
            }
                
            if (!self.selectedStateId() || self.selectedStateId() == "" && self.UsrCountry() != 17) {
                msg += "\n" + msgValidationStateRequired;
            }
                

            if (self.UsrCountry() == 7) {
                if (!self.Neighborhoods() || self.Neighborhoods() == "")
                    msg += "\n" + viewValidateComuneRequired;
            }
        }
        
        
        if (msg !== "") { alert(msgValidationGeoRef + msg); $('#tabs').tabs({ active: 1 }); return false; }
        if (nextStep != null) nextStep();
        return true;
    };

    self.Save = function () {
        app.Views.Home.ValidateAll();
    };

    self.Cancel = function () {
        if (confirm(viewConfirmExit)) {
            app.Views.Home.CancelEdit();
        }
    };

    self.SaveGEO = function (nextStep) {
        $("#Area").attr("disabled", false);
        $("#provincia").attr("disabled", false);
        $("#Neighborhoods").attr("disabled", false);
         $.post(app.dataModel.saveGEOUrl,
            {
                id: self.Id,
                CountryId: self.selectedCountryId(),
                AreaId: self.selectedAreaId(),
                StateId: self.selectedStateId(),
                LocalId: self.selectedLocalId(),
                NeighborhoodId: self.selectedNeighborhoodId(),
                UrbanRural: self.UrbanRural(),
                CountryId2weeks: self.selectedCountryId2weeks(),
                AreaId2weeks: self.selectedAreaId2weeks(),
                StateId2weeks: self.selectedStateId2weeks(),
                NeighborhoodId2weeks: self.selectedNeighborhoodId2weeks(),
                Address: self.Address() ? self.Address().toLocaleUpperCase() : "",
                CountryOrigin: self.selectedCountryOrigin(),
                PhoneNumber: self.PhoneNumber(),
                Latitude: self.Latitude(),
                Longitude: self.Longitude()
            },
            function (data) {
                if (nextStep) nextStep();
            },
            "json"
         );
        return true;
    };

    return self;

};

app.addViewModel({
    name: "GEO",
    bindingMemberName: "geo",
    factory: GEOViewModel
});