$(document).ready(function () {

    onLoad();    

    function onLoad() {
        $("#Modify_Epi").hide();
        $("#Modify_Lab").hide();
        $("#Close_case").hide();
        toggleRoleAdmin();
    }

    function toggleRoleAdmin() {
        //var AdminCountry = $("#ITy").val();
        //console.log(AdminCountry);
        if (AdminCountry == "country") {        // InstitutionType
            $("#Modify_Epi").show();
            $("#Modify_Lab").show();
            $("#Close_case").show();
        } else {
            $("#Modify_Epi").hide();
            $("#checkModify_Epi").attr("checked", false);
            $("#Modify_Lab").hide();
            $("#checkModify_Lab").attr("checked", false);
            $("#Close_case").hide();
            $("#checkClose_case").attr("checked", false);
        }
    }

    //$("#checkAdmin").change(function () {
    //    if ($("#checkAdmin").is(":checked")) {
    //        $("#Modify_Epi").show();
    //        $("#Modify_Lab").show();
    //        $("#Close_case").show();
    //    } else {
    //        $("#Modify_Epi").hide();
    //        $("#checkModify_Epi").attr("checked", false);
    //        $("#Modify_Lab").hide();
    //        $("#checkModify_Lab").attr("checked", false);
    //        $("#Close_case").hide();
    //        $("#checkClose_case").attr("checked", false);
    //    }

    //});

    $("#CountryID").change(function () {
        loadCountryInstitutions();
    });

    function loadCountryInstitutions() {
        $("#InstitutionID").empty();
        $.ajax({
            type: 'POST',
            url: '/UsersAdmin/GetCountryInstitutions',
            dataType: 'json',
            data: { countryID: $("#CountryID").val() },
            success: function (institutions) {
                $.each(institutions, function (i, institutions) {
                    $("#InstitutionID").append('<option value="'
                                        + institutions.Value + '" ' + '' + '>'
                                        + institutions.Text + '</option>');
                });
            },
            error: function (ex) {
                console.log('Failed to retrieve institutions.' + ex);
            }
        });

        return false;
    } 
});


