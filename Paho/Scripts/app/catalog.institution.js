$(document).ready(function () {

    onLoad();    

    function onLoad() {
        var isEdit = $("#ID").val() != undefined;
        toggleAccessLevel();
        toggleRegion(isEdit);
    }

    function toggleAccessLevel(isEdit) {
        accessLevel = $("#AccessLevel").val();
        if (accessLevel == "Country" || accessLevel == "Area" || accessLevel == "Regional") {
            $("#div-father").hide();            
        } else {
            $("#div-father").show();
            if (isEdit) return;
            loadFatherInst();
        }
    }

    $("#AccessLevel").change(function () {
        toggleAccessLevel();
    });

    $("#InstitutionType").change(function () {
        toggleAccessLevel();
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
		
		if (regInst.val() != 0){
			regSalud.val(0);
			regPais.val(0);
			regInstGroup.show();
			regSaludGroup.hide();
			regPaisGroup.hide();
		} else if (regSalud.val() != 0){
			regInst.val(0);
			regPais.val(0);
			regInstGroup.hide();
			regSaludGroup.show();
			regPaisGroup.hide();
		} else if (regPais.val() != 0){
			regInst.val(0);
			regSalud.val(0);
			regInstGroup.hide();
			regSaludGroup.hide();
			regPaisGroup.show();
		} else {
		    regInstGroup.show();
		    regSaludGroup.show();
		    regPaisGroup.show();
		}
	}
	
    $('form').on('submit', function (e) { 
	    $("#InstType").val($("#InstitutionType").val())
	    return;
    });
});


