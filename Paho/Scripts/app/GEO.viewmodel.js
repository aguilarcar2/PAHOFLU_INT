﻿function GEOViewModel(app, dataModel) {
    var self = this;
    self.Id = "";
    self.hasReset = ko.observable(false);
    self.UsrCountry = ko.observable(app.Views.Home.UsrCountry()); // Pais del usuario logueado
    //****
    self.selectedCountryId = ko.observable();

    self.Areas = ko.observableArray();
    self.selectedAreaId = ko.observable();
    self.States = ko.observableArray();
    self.selectedStateId = ko.observable();
    self.Neighborhoods = ko.observableArray();
    self.selectedNeighborhoodId = ko.observable();
    self.Hamlets = ko.observableArray();
    self.selectedHamletId = ko.observable();
    self.Colonies = ko.observableArray();
    self.selectedColonyId = ko.observable();

    self.selectedCountryOrigin = ko.observable();

    self.Locals = ko.observableArray();
    self.selectedLocalId = ko.observable();

    self.Latitude = ko.observable();
    self.Longitude = ko.observable();
    self.fullCodeGEORef = ko.observable();

    //****
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

    //****
    self.ReloadAreas = function (select, event) {
        //console.log("RA1");
        //console.log("1111->");
        //console.log(select);
        //console.log(event);
        //if (event.originalEvent) { //
        //    console.log("user changed1111");
        //} else { // 
        //    console.log("program changed1111");
        //    return;
        //}
        //console.log("1111F->");

        self.Areas("");
        self.selectedAreaId("");
        //self.selectedStateId("");
        //self.selectedNeighborhoodId("");
        //self.selectedHamletId("");
        //self.selectedColonyId("");
        self.Latitude("");
        self.Longitude("");
        //$("#Latitude").val("");
        //$("#Longitude").val("");
        //self.Hamlets("");
        //self.Colonies("");

        if (typeof self.selectedCountryId() === "undefined") {
            //****
            //self.fullCodeGEORef(self.selectedAreaId() + "-" + self.selectedStateId() + "-" + self.selectedNeighborhoodId() + "-" + self.selectedHamletId() + "-" + self.selectedColonyId());
            return;
        }

        $.getJSON(app.dataModel.getAreasUrl, { CountryID: self.selectedCountryId() }, function (data, status) {
            self.Areas(data);
            if ($.isFunction(select)) select();
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
        //****
        self.fullCodeGEORef(self.selectedAreaId() + "-" + self.selectedStateId() + "-" + self.selectedNeighborhoodId() + "-" + self.selectedHamletId() + "-" + self.selectedColonyId());
    };
    //****
    self.ReloadStates = function (select, event) {
        //console.log("RS1");
        //console.log("2222->");
        ////console.log(select);
        ////console.log(event);
        //if (event.originalEvent) { //
        //    console.log("user changed2222");
        //} else { // 
        //    console.log("program changed2222");
        //    return;
        //}
        //console.log("2222F->");

        self.States("");
        //self.Neighborhoods("");
        //self.Hamlets("");
        //self.Colonies("");
        self.selectedStateId("");
        //self.selectedNeighborhoodId("");
        //self.selectedHamletId("");
        //self.selectedColonyId("");
        self.Latitude("");
        self.Longitude("");
        //$("#Latitude").val("");
        //$("#Longitude").val("");

        if (typeof self.selectedAreaId() === "undefined") {
            //console.log("RS2");
            //self.Neighborhoods("");
            //self.Hamlets("");
            //self.Colonies("");
            //****
            //self.fullCodeGEORef(self.selectedAreaId() + "-" + self.selectedStateId() + "-" + self.selectedNeighborhoodId() + "-" + self.selectedHamletId() + "-" + self.selectedColonyId());
            return;
        }
        //console.log("RS3");
        $.getJSON(app.dataModel.getStatesUrl, { AreaID: self.selectedAreaId() }, function (data, status) {
            self.States(data);
            if ($.isFunction(select)) select();
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })

        if (self.UsrCountry() == 17) {
            self.ReloadParishPostOffice();
        }
        //****
        self.fullCodeGEORef(self.selectedAreaId() + "-" + self.selectedStateId() + "-" + self.selectedNeighborhoodId() + "-" + self.selectedHamletId() + "-" + self.selectedColonyId());
    };
    //****
    self.ReloadNeighborhoods = function (select, event) {
        //console.log("RN1");
        //console.log("3333->");
        ////console.log(select);
        ////console.log(event);
        //if (event.originalEvent) { //
        //    console.log("user changed3333");
        //} else { // 
        //    console.log("program changed3333");
        //    return;
        //}
        //console.log("3333F->");

        self.Neighborhoods("");
        //self.Hamlets("");
        //self.Colonies("");
        self.selectedNeighborhoodId("");
        //self.selectedHamletId("");
        //self.selectedColonyId("");
        self.Latitude("");
        self.Longitude("");
        //$("#Latitude").val("");
        //$("#Longitude").val("");

        if (typeof self.selectedStateId() === "undefined") {
            //console.log("RN2");
            //****
            //self.fullCodeGEORef(self.selectedAreaId() + "-" + self.selectedStateId() + "-" + self.selectedNeighborhoodId() + "-" + self.selectedHamletId() + "-" + self.selectedColonyId());
            return;
        }
        if (typeof self.selectedStateId() != "undefined" && self.selectedStateId() != "" && self.selectedStateId() > 0) {
            //console.log("RN3");
            $.getJSON(app.dataModel.getNeighborhoodsUrl, { StateID: self.selectedStateId() }, function (data, status) {
                self.Neighborhoods(data);
                if ($.isFunction(select)) select();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            })
        }
        //****
        //console.log("RN4");
        self.fullCodeGEORef(self.selectedAreaId() + "-" + self.selectedStateId() + "-" + self.selectedNeighborhoodId() + "-" + self.selectedHamletId() + "-" + self.selectedColonyId());
    };

    self.ReloadLocalsAndNeighborhoods = function () {
        self.ReloadNeighborhoods();
    };

    //#### CAFQ: 181018
    self.ReloadHamlets = function (select, event) {
        //console.log("RH1");
        //console.log("4444->");
        ////console.log(select);
        ////console.log(event);
        //if (event.originalEvent) { //
        //    console.log("user changed4444");
        //} else { // 
        //    console.log("program changed4444");
        //    return;
        //}
        //console.log("4444F->");

        self.Hamlets("");
        self.selectedHamletId("");
        //self.selectedColonyId("");
        self.Latitude("");
        self.Longitude("");
        //$("#Latitude").val("");
        //$("#Longitude").val("");

        if (typeof self.selectedNeighborhoodId() === "undefined") {
            //console.log("RH2");
            //****
            //self.fullCodeGEORef(self.selectedAreaId() + "-" + self.selectedStateId() + "-" + self.selectedNeighborhoodId() + "-" + self.selectedHamletId() + "-" + self.selectedColonyId());
            return;
        }
        if (typeof self.selectedNeighborhoodId() != "undefined" && self.selectedNeighborhoodId() != "" && self.selectedNeighborhoodId() > 0) {
            //console.log("RH3");
            $.getJSON(app.dataModel.getHamletsUrl, { NeighborhoodId: self.selectedNeighborhoodId() }, function (data, status) {
                self.Hamlets(data);
                if ($.isFunction(select)) select();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            })

        }
        //****
        self.fullCodeGEORef(self.selectedAreaId() + "-" + self.selectedStateId() + "-" + self.selectedNeighborhoodId() + "-" + self.selectedHamletId() + "-" + self.selectedColonyId());
        //console.log("RH4");
    };

    //#### CAFQ: 181018
    self.ReloadColonies = function (select, event) {
        //console.log("RC1");
        //console.log("5555->");
        ////console.log(select);
        ////console.log(event);
        //if (event.originalEvent) { //
        //    console.log("user changed5555");
        //} else { // 
        //    console.log("program changed5555");
        //    return;
        //}
        //console.log("5555F->");

        self.Colonies("");
        self.selectedColonyId("");
        self.Latitude("");
        self.Longitude("");
        //$("#Latitude").val("");
        //$("#Longitude").val("");

        if (typeof self.selectedHamletId() === "undefined") {
            //console.log("RC2");
            //****
            //self.fullCodeGEORef(self.selectedAreaId() + "-" + self.selectedStateId() + "-" + self.selectedNeighborhoodId() + "-" + self.selectedHamletId() + "-" + self.selectedColonyId());
            return;
        }
        if (typeof self.selectedHamletId() != "undefined" && self.selectedHamletId() != "" && self.selectedHamletId() > 0) {
            //console.log("RC3");
            $.getJSON(app.dataModel.getColoniesUrl, { HamletId: self.selectedHamletId() }, function (data, status) {
                self.Colonies(data);
                if ($.isFunction(select)) select();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            })

        }
        //console.log("RC4");
        //****
        self.fullCodeGEORef(self.selectedAreaId() + "-" + self.selectedStateId() + "-" + self.selectedNeighborhoodId() + "-" + self.selectedHamletId() + "-" + self.selectedColonyId());
    };

    //#### 
    self.ReloadOthers = function (select, event) {
        //console.log("RO1");
        //****
        self.fullCodeGEORef(self.selectedAreaId() + "-" + self.selectedStateId() + "-" + self.selectedNeighborhoodId() + "-" + self.selectedHamletId() + "-" + self.selectedColonyId());
    };

    //****
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

    //**** Country case was diagnosed
    self.selectedCountryIdCWD = ko.observable();
    self.selectedCountryIdCWD.subscribe(function (newCountrySelect) {
        if (self.hasReset() != true) {
            if (self.UsrCountry() != self.selectedCountryIdCWD()) {
                //$("#AreaCWD").attr("disabled", true);
                //$("#StateCWD").attr("disabled", true);
                //$("#Neighborhoods").attr("disabled", true);
            } else if (self.UsrCountry() == self.selectedCountryIdCWD()) {
                $("#AreaCWD").attr("disabled", false);
                $("#StateCWD").attr("disabled", false);
                //$("#Neighborhoods").attr("disabled", false);      
            }
        }
    });

    //**** Level 1 case was diagnosed
    self.AreaNameCWD = ko.observable();

    self.AreasCWD = ko.observableArray();
    self.selectedAreaIdCWD = ko.observable();
    self.ReloadAreasCWD = function (select) {
        console.log("ReloadAreasCWD->START");
        //if (typeof self.selectedCountryIdCWD() === "undefined") {
        //    self.AreasCWD("");
        //    self.AreasNameCWD("");
        //    return;
        //}
        self.AreasCWD("");
        self.AreaNameCWD("");
        if (self.UsrCountry() == self.selectedCountryIdCWD()) {
            $.getJSON(app.dataModel.getAreasUrl, { CountryID: self.selectedCountryIdCWD() }, function (data, status) {
                self.AreasCWD(data);
                if ($.isFunction(select))
                    select();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            })
        }
        console.log("ReloadAreasCWD->END");
    };

    self.AreaCWDVisible = ko.computed(function () {
        if (typeof self.selectedCountryIdCWD() === "undefined") {
            return true;
        } else {
            return (self.UsrCountry() == self.selectedCountryIdCWD()) ? true : false;
        }       
    }, self);    

    //**** Level 2 case was diagnosed
    self.StateNameCWD = ko.observable();

    self.StatesCWD = ko.observableArray();
    self.selectedStateIdCWD = ko.observable();
    self.ReloadStatesCWD = function (select) {
        console.log("ReloadStatesCWD->START");
        //if (typeof self.selectedAreaIdCWD() === "undefined") {
        //    self.StatesCWD("");
        //    self.StatesNameCWD("");
        //    return;
        //}
        self.StatesCWD("");
        self.StateNameCWD("");
        if (self.UsrCountry() == self.selectedCountryIdCWD()) {
            $.getJSON(app.dataModel.getStatesUrl, { AreaID: self.selectedAreaIdCWD() }, function (data, status) {
                self.StatesCWD(data);
                if ($.isFunction(select))
                    select();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            })
        }
        console.log("ReloadStatesCWD->END");
    };



    self.StateCWDVisible = ko.computed(function () {
        if (typeof self.selectedCountryIdCWD() === "undefined") {
            return true;
        } else {
            return (self.UsrCountry() == self.selectedCountryIdCWD()) ? true : false;
        }
    }, self);

    //****
    self.selectedParishPostOfficeJMId = ko.observable();

    self.SearchUbicaResid = ko.observable("");              //####CAFQ: 180817
    self.SearchUbicaResidVal = ko.observable("");           //####CAFQ: 180817

    self.StreetNo = ko.observable();
    self.StreetName = ko.observable();
    self.ApartmentSuiteLot = ko.observable();
    self.Address2 = ko.observable();
    
    self.UrbanRural = ko.observable(0);
    self.Address = ko.observable("");
    
    self.ParishPostOfficeJM = ko.observableArray();
    self.Areas2weeks = ko.observableArray();
    self.States2weeks = ko.observableArray();
    self.selectedCountryId2weeks = ko.observable();
    self.selectedAreaId2weeks = ko.observable();
    self.selectedStateId2weeks = ko.observable();
    self.selectedNeighborhoodId2weeks = ko.observable();
    self.Neighborhoods2weeks = ko.observableArray();
    
    self.Locals2weeks = ko.observableArray();
    self.PhoneNumber = ko.observable("");

    self.regions = ko.observableArray(regions);             //#### CAFQ: 181008
    self.selectedRegionId = ko.observable("");              //#### CAFQ: 181008

    self.fullCodeGEORef.subscribe(function (newValueSelected) {
        //console.log("self.fullCodeGEORef.subscribe->START")

        if (self.UsrCountry() == 15) {                  // Honduras
            var geoData = self.fullCodeGEORef().split('-')

            if (typeof newValueSelected === "undefined" || newValueSelected === "") {
                //console.log("fullCodeGEORef_1");
                // Nada         
            } else {
                //console.log("fullCodeGEORef_2");
                bTraerDatos = false;
                if (self.selectedAreaId() > 0 && self.selectedStateId() > 0 && self.selectedNeighborhoodId() > 0 && self.selectedHamletId() > 0 && self.selectedColonyId() > 0)
                    bTraerDatos = true;
                else if(self.selectedAreaId() > 0 && self.selectedStateId() > 0 && self.selectedNeighborhoodId() > 0 && self.selectedHamletId() > 0)
                    bTraerDatos = true;

                if (bTraerDatos) {
                    //console.log("TRAER DATOSSSSSSSSSSSSS")

                    $.getJSON(app.dataModel.getGEOreferenceInformation, {
                        countryID: self.UsrCountry(), areaID: geoData[0], stateID: geoData[1], neighborhoodID: geoData[2], hamletID: geoData[3], colonyID: geoData[4]
                    }, function (data, status) {
                        if (data[0].latitude == 0) {
                            //console.log("LL: Datos vacios");
                            //self.Longitude("");
                            //self.Latitude("");
                            $("#Latitude").val("");
                            $("#Longitude").val("");
                        } else {
                            //self.Longitude(data[0].longitude);
                            //self.Latitude(data[0].latitude);
                            //console.log("LL: Existen datos");
                            $("#Latitude").val(data[0].latitude);
                            $("#Longitude").val(data[0].longitude);
                            //console.log("LL2");
                            //$(elemento).val(valor)
                        }
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        alert(msgValidationServerError);
                        //console.log(errorThrown);
                    });
                } else {
                    //console.log("LL: No cumple criterio");
                    $("#Latitude").val("");
                    $("#Longitude").val("");
                }
            }
        } else {
            //self.Longitude("");
            //self.Latitude("");
            $("#Latitude").val("");
            $("#Longitude").val("");
        }

        //console.log("m1->");
        //console.log(newValueSelected);
        //console.log(self.Latitude());
        //console.log(self.Longitude());
        //console.log("m2->");

        //console.log("self.fullCodeGEORef.subscribe->END")
    }, this);

    //self.selectedColonyId.subscribe(function (newValueSelected) {
    //    console.log("self.selectedColonyId.subscribe->START")

    //    if (self.UsrCountry() == 15) {                  // Honduras
    //        self.fullCodeGEORef(self.selectedAreaId() + "-" + self.selectedStateId() + "-" + self.selectedNeighborhoodId() + "-" + self.selectedHamletId() + "-" + self.selectedColonyId());
    //     }

    //    console.log("self.selectedColonyId.subscribe->END")
    //}, this);
    

    /*
    self.selectedHamletId.subscribe(function (newValueSelected) {
        console.log("self.selectedHamletId.subscribe->START")

        if (self.UsrCountry() == 15) {                  // Honduras
            if (typeof newValueSelected === "undefined" || newValueSelected === "") {
                // Nada         
                console.log("NADA-");
            } else {
                console.log("ALGO-");
                console.log(self.selectedColonyId());
                console.log("ALGO-0");
                if (typeof self.selectedColonyId() === "undefined" || self.selectedColonyId() === "") {
                    console.log("ALGO-2");
                    $.getJSON(app.dataModel.getGEOreferenceInformation, {
                        countryID: self.UsrCountry(), areaID: self.selectedAreaId(), stateID: self.selectedStateId(), neighborhoodID: self.selectedNeighborhoodId(),
                        hamletID: newValueSelected, colonyID: self.selectedColonyId()
                    }, function (data, status) {
                        if (data[0].latitude == 0) {
                            console.log("Por aqui");
                            self.Longitude("");
                            self.Latitude("");
                        } else {
                            console.log("Por alla");
                            self.Longitude(data[0].longitude);
                            self.Latitude(data[0].latitude);
                        }
                    })
                            .fail(function (jqXHR, textStatus, errorThrown) {
                                alert(msgValidationServerError);
                                //console.log(errorThrown);
                            });
                } else {
                    // NADA
                }
            }
        }

        console.log("self.selectedHamletId.subscribe->EMD")
    });
    */
    /*
    self.selectedColonyId.subscribe(function (newColonySelected) {
        console.log("self.selectedColonyId.subscribe->CO_START")
        console.log("CO_MA1-");
        console.log(newColonySelected);
        console.log("CO_MA2-");

        if (self.UsrCountry() == 15) {                  // Honduras
            if (typeof newColonySelected === "undefined" || newColonySelected === "") {
                // Nada         
                console.log("CO_NADA-");
            } else {
                console.log("CO_ALGO-");
                $.getJSON(app.dataModel.getGEOreferenceInformation, {
                    countryID: self.UsrCountry(), areaID: self.selectedAreaId(), stateID: self.selectedStateId(), neighborhoodID: self.selectedNeighborhoodId(),
                    hamletID: self.selectedHamletId(), colonyID: newColonySelected
                }, function (data, status) {
                    if (data[0].latitude == 0) {
                        console.log("CO_Por aqui");
                        self.Longitude("");
                        self.Latitude("");
                    } else {
                        console.log("CO_Por alla");
                        self.Longitude(data[0].longitude);
                        self.Latitude(data[0].latitude);
                    }
                })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        alert(msgValidationServerError);
                        //console.log(errorThrown);
                    });
            }
        }
        console.log("self.selectedColonyId.subscribe->CO_EMD")
    });
    */

    self.ActiveBOLCountry2weeks = ko.computed(function () {
        return (self.UsrCountry() == 3 && self.selectedCountryId2weeks() == app.Views.Contact.UsrCountry()) ? true : false;
    }, self);

    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7) ? true : false;
    }, self);

    self.EnableJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? true : false;
    }, self);

    /*self.EnableHON = ko.computed(function () {
        return (self.UsrCountry() == 15) ? true : false;
    }, self);*/

    self.ResetGEO = function () {
        self.hasReset(true);
        self.Id = "";
        //alert(app.Views.Home);
        //self.selectedCountryId() ? self.selectedCountryId() : (CountryID > 0) ? CountryID : 0
        self.selectedCountryId("");
        self.selectedCountryId(app.Views.Home.UsrCountry());
        self.selectedCountryOrigin("");
        self.selectedAreaId("");
        self.selectedParishPostOfficeJMId("");
        self.StreetNo("");
        self.StreetName("");
        self.ApartmentSuiteLot("");
        self.Address2("");
        self.selectedStateId("");
        self.selectedLocalId("");
        self.selectedNeighborhoodId("");
        self.selectedHamletId("");                  //#### CAFQ: 181018
        self.selectedColonyId("");                  //#### CAFQ: 181018
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
        self.selectedRegionId("")               //#### CAFQ: 181008
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

        self.selectedCountryId("");
        self.selectedCountryId(app.Views.Home.UsrCountry());
        self.selectedCountryOrigin("");
        self.selectedAreaId("");
        self.selectedParishPostOfficeJMId("");
        self.StreetNo("");
        self.StreetName("");
        self.ApartmentSuiteLot("");
        self.Address2("");
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
                self.selectedRegionId(data.RegionAddress);                  //#### CAFQ: 181008
                self.ReloadAreas(function() {
                    self.selectedAreaId(data.AreaID);
                    self.ReloadStates(function () {
                        self.selectedStateId(data.StateID);
                        if (self.UsrCountry() == 17) {
                            self.selectedParishPostOfficeJMId(data.ParishPostOfficeJMID);
                            self.StreetNo(data.StreetNo);
                            self.StreetName(data.StreetName);
                            self.ApartmentSuiteLot(data.ApartmentSuiteLot);
                            self.Address2(data.Address2);
                        }
                        self.ReloadNeighborhoods(function () {
                            self.selectedNeighborhoodId(data.NeighborhoodID);
                            self.ReloadHamlets(function () {
                                self.selectedHamletId(data.HamletID);
                                self.ReloadColonies(function () {
                                    self.selectedColonyId(data.ColonyID);
                                });
                            });
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

    self.ReloadParishPostOffice = function (select) {
        if (typeof self.selectedAreaId() === "undefined") {
            self.ParishPostOfficeJM("");
            return;
        }
        $.getJSON(app.dataModel.getParishPostOfficeUrl, { AreaID: self.selectedAreaId() }, function (data, status) {
            self.ParishPostOfficeJM(data);
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
                if (!(self.UsrCountry() == 17 && app.Views.Contact.SurvInusual() == 1))    //#### CAFQ: 180604 - Jamaica Universal
                    if (!(self.UsrCountry() == 3 && app.Views.Contact.SurvInusual() == 1))    //#### Bolivia inusitado
                        if (app.Views.Contact.IsSurv() != "4")
                            msg += "\n" + msgValidationAreaRequired;
            }
                
            if ((!self.selectedStateId() || self.selectedStateId() == "") && self.UsrCountry() != 17 && self.UsrCountry() != 11 && self.UsrCountry() != 119) {
                if (!(self.UsrCountry() == 3 && app.Views.Contact.SurvInusual() == 1))    //#### Bolivia inusitado
                    if (app.Views.Contact.IsSurv() != "4")
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
        $("#Hamlets").attr("disabled", false);                          //#### CAFQ: 181018
        $("#Colonies").attr("disabled", false);                         //#### CAFQ: 181018
        $.post(app.dataModel.saveGEOUrl,
            {
                id: self.Id,
                CountryId: self.selectedCountryId(),
                AreaId: self.selectedAreaId(),
                StateId: self.selectedStateId(),
                ParishPostOfficeJMId: self.selectedParishPostOfficeJMId(),
                StreetNo: self.StreetNo(),
                StreetName: self.StreetName(),
                ApartmentSuiteLot: self.ApartmentSuiteLot(),
                Address2: self.Address2(),
                LocalId: self.selectedLocalId(),
                NeighborhoodId: self.selectedNeighborhoodId(),
                HamletId: self.selectedHamletId(),                      //#### CAFQ: 181018
                ColonyId: self.selectedColonyId(),                      //#### CAFQ: 181018
                UrbanRural: self.UrbanRural(),
                CountryId2weeks: self.selectedCountryId2weeks(),
                AreaId2weeks: self.selectedAreaId2weeks(),
                StateId2weeks: self.selectedStateId2weeks(),
                NeighborhoodId2weeks: self.selectedNeighborhoodId2weeks(),
                Address: self.Address() ? self.Address().toLocaleUpperCase() : "",
                CountryOrigin: self.selectedCountryOrigin(),
                PhoneNumber: self.PhoneNumber(),
                Latitude: self.Latitude(),
                Longitude: self.Longitude(),
                RegionAddress: self.selectedRegionId()             //#### CAFQ: 181008
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