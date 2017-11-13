
window.common = (function () {
    var common = {};

    common.getFragment = function getFragment() {
        if (window.location.hash.indexOf("#") === 0) {
            return parseQueryString(window.location.hash.substr(1));
        } else {
            return {};
        }
    };

    function parseQueryString(queryString) {
        var data = {},
            pairs, pair, separatorIndex, escapedKey, escapedValue, key, value;

        if (queryString === null) {
            return data;
        }

        pairs = queryString.split("&");

        for (var i = 0; i < pairs.length; i++) {
            pair = pairs[i];
            separatorIndex = pair.indexOf("=");

            if (separatorIndex === -1) {
                escapedKey = pair;
                escapedValue = null;
            } else {
                escapedKey = pair.substr(0, separatorIndex);
                escapedValue = pair.substr(separatorIndex + 1);
            }

            key = decodeURIComponent(escapedKey);
            value = decodeURIComponent(escapedValue);

            data[key] = value;
        }

        return data;
    }

    return common;
})();

$(document).ready(function () {

    // Configuración para que el Ajax no sea Asincronico
    $.ajaxSetup({
        async: false
    });

    (function ($) {
        $.postJSON = function (url, data) {
            var o = {
                url: url,
                type: "POST",
                dataType: "json",
                contentType: 'application/json; charset=utf-8'
            };
            if (data !== undefined) {
                o.data = JSON.stringify(data);
            }
            return $.ajax(o);
        };
    }(jQuery));


    
    $('#DiagPrinAdm').autocomplete({
        minLength: 3,
        source:function (request, response)
            {
                $.ajax({
                    url : "/cases/GetCIE10",
                    type : "GET",
                    data: { term: $('#DiagPrinAdm').val(), max: 15, code: "-J"  },
                    dataType: "json",
                    async: true,
                    success: function (data) {
                        response($.map(data, function (el) {
                            return {
                                label: el.label,
                                value: el.value
                            };
                        }));
                    }
                });
        },
        change: function (event, ui) {
            if (ui.item) {
                // do whatever you want to when the item is found
            }
            else {
                alert("Seleccione una opción valida");
                $('#DiagPrinAdm').val("");
            }

        },
        select: function (event, ui) {
            // just in case you want to see the ID
            var accountVal = ui.item.value;
            $('#DiagPrinAdmVal').val(accountVal).change();

            // now set the label in the textbox
            var accountText = ui.item.label;
            $('#DiagPrinAdm').val(accountText).change();

            return false;
        },
        focus: function (event, ui) {
            // this is to prevent showing an ID in the textbox instead of name 
            // when the user tries to select using the up/down arrow of his keyboard
            //$('#DiagPrinAdm').val(ui.item.label);
            return false;
        }
    });

    $('#DiagEg').autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                url: "/cases/GetCIE10",
                type: "GET",
                data: { term: $('#DiagEg').val(), max: 15, code: "-J" },
                dataType: "json",
                async: true,
                success: function (data) {
                    response($.map(data, function (el) {
                        return {
                            label: el.label,
                            value: el.value
                        };
                    }));
                }
            });
        },
        change: function (event, ui) {
            if (ui.item) {
                // do whatever you want to when the item is found
            }
            else {
                alert("Seleccione una opción valida");
                $('#DiagEg').val("");
            }

        },
        select: function (event, ui) {
            // just in case you want to see the ID
            var accountVal = ui.item.value;
            $('#DiagEgVal').val(accountVal).change();

            // now set the label in the textbox
            var accountText = ui.item.label;
            $('#DiagEg').val(accountText).change();

            return false;
        },
        focus: function (event, ui) {
            // this is to prevent showing an ID in the textbox instead of name 
            // when the user tries to select using the up/down arrow of his keyboard
            //$('#DiagPrinAdm').val(ui.item.label);
            return false;
        }
    });

    $('#Salon').autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                url: "/cases/GetSalon",
                type: "GET",
                data: { term: $('#Salon').val(), max: 15 },
                dataType: "json",
                async: true,
                success: function (data) {
                    response($.map(data, function (el) {
                        return {
                            label: el.label,
                            value: el.value
                        };
                    }));
                }
            });
        },
        change: function (event, ui) {
            if (ui.item) {
                // do whatever you want to when the item is found
            }
            else {
                // do whatever you want to when the item is not found
                alert("Seleccione una opción valida");
                $('#Salon').val("");
            }

        },
        select: function (event, ui) {
            // just in case you want to see the ID
            var accountVal = ui.item.value;
            $('#SalonVal').val(accountVal).change();

            // now set the label in the textbox
            var accountText = ui.item.label;
            $('#Salon').val(accountText).change();

            return false;
        },
        focus: function (event, ui) {
            // this is to prevent showing an ID in the textbox instead of name 
            // when the user tries to select using the up/down arrow of his keyboard
            //$('#DiagPrinAdm').val(ui.item.label);
            return false;
        }
    });

    //$('#NoExpediente').autocomplete({
    //    minLength: 8,
    //    source: function (request, response) {
    //        $.ajax({
    //            url: "/cases/GetPatientInformation",
    //            type: "POST",
    //            data: { DTP: $('#DocumentType').val(), DNP: $('#NoExpediente').val(), max: 15 },
    //            dataType: "json",
    //            async: true,
    //            success: function (data) {
    //                response($.map(data, function (el) {
    //                    return {
    //                        label: el.label,
    //                        value: el.value
    //                    };
    //                }));
    //            }
    //        });
    //    },
    //    change: function (event, ui) {
    //        if (ui.item) {
    //            // do whatever you want to when the item is found
    //        }
    //        else {
    //            alert("Seleccione una opción valida");
    //            $('#NoExpediente').val("");
    //        }

    //    },
    //    select: function (event, ui) {
    //        // just in case you want to see the ID
    //        var accountVal = ui.item.value;
    //        $('#NoExpediente').val(accountVal).change();

    //        // now set the label in the textbox
    //        var accountText = ui.item.label;
    //        $('#NoExpediente').val(accountText).change();

    //        return false;
    //    },
    //    focus: function (event, ui) {
    //        // this is to prevent showing an ID in the textbox instead of name 
    //        // when the user tries to select using the up/down arrow of his keyboard
    //        //$('#DiagPrinAdm').val(ui.item.label);
    //        return false;
    //    }
    //});




})

