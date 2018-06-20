$(document).ready(function () {

    onLoad();    

    function onLoad() {
        var isEdit = $("#ID").val() != undefined;
        //toggleAccessLevel();
        toggleRegion(isEdit);
        $("#RegionInstitucionalGroup").hide();
        $("#RegionSaludGroup").hide();
        $("#RegionPaisGroup").hide();
        $("#div-PCR").hide();
        $("#div-IFI").hide();
        $("#div-NPHL").hide();
        $("#div-OPbL").hide();

        if ($("#cod_region_institucional").children().length > 1)
            $("#RegionInstitucionalGroup").show();

        if ($("#cod_region_salud").children().length > 1)
            $("#RegionSaludGroup").show();

        if ($("#cod_region_pais").children().length > 1)
            $("#RegionPaisGroup").show();

        toggleInstitutionType();
        toggleAccessLevel();

        //$("#OrdenPrioritybyLab").ForceNumericOnly();
        //$('#OrdenPrioritybyLab').keyup(function (e) {
        //    if (/\D/g.test(this.value)) {
        //        // Filter non-digits from input value.
        //        this.value = this.value.replace(/\D/g, '');
        //    }
        //});

    }

    function toggleAccessLevel(isEdit) {
        accessLevel = $("#AccessLevel").val();
        console.log(accessLevel);
        if (accessLevel == "Service") {
            $("#div-father").show();
            if (isEdit) return;
            loadFatherInst();
        }
        else if (accessLevel == "Area") {
            $("#RegionInstitucionalGroup").hide();
            $("#RegionSaludGroup").hide();
            $("#RegionPaisGroup").hide();
            $("#div-father").hide();
        }
        else if (accessLevel != "Area" && accessLevel != "Service") {
            if ($("#cod_region_institucional").children().length > 1)
                $("#RegionInstitucionalGroup").show();

            if ($("#cod_region_salud").children().length > 1)
                $("#RegionSaludGroup").show();

            if ($("#cod_region_pais").children().length > 1)
                $("#RegionPaisGroup").show();
        }

        else {
            $("#div-father").hide();
        }
    }

    function toggleInstitutionType() {
        accessLevel = $("#InstitutionType").val();
        if (accessLevel == "Lab") {
            $("#div-PCR").show();
            $("#div-IFI").show();
            $("#div-NPHL").show();
            $("#div-OPbL").show();     
        } else {
            $("#div-PCR").hide();
            $("#div-IFI").hide();
            $("#div-NPHL").hide();
            $("#div-OPbL").hide();
        }
    }

    $("#AccessLevel").change(function () {
        toggleAccessLevel();
    });

    $("#InstitutionType").change(function () {
        toggleInstitutionType();
    });

    $("#cod_region_institucional").change(function () {
        toggleRegion();
    });

    $("#cod_region_salud").change(function () {
        toggleRegion();
    });

    $("#cod_region_pais").change(function () {
        toggleRegion();
    });

    function loadFatherInst() {
        $("#Father_ID").empty();
        $.ajax({
            type: 'POST',
            url: '/CatInstitucion/GetInstituciones',
            dataType: 'json',
            data: { tipo: $("#InstitutionType").val() },
            success: function (instituciones) {
                $.each(instituciones, function (i, institucion) {
                    $("#Father_ID").append('<option value="'
                                        + institucion.Value + '">'
                                        + institucion.Text + '</option>');
                });
            },
            error: function (ex) {
                console.log('Failed to retrieve states.' + ex);
            }
        });
        return false;
    }
	
    function toggleRegion() {
        regInst = $("#cod_region_institucional");
        regSalud = $("#cod_region_salud");
        regPais = $("#cod_region_pais");

		regInstGroup = $("#RegionInstitucionalGroup");
		regSaludGroup = $("#RegionSaludGroup");
		regPaisGroup = $("#RegionPaisGroup");
	}
	
    $('form').on('submit', function (e) { 
	    $("#InstType").val($("#InstitutionType").val())
	    return;
    });
});


