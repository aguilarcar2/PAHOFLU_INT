
function PrintTestViewModel(app, dataModel) {
	var self = this;
	//self.selectedCountryId = ko.observable("");
	//self.selectedInstitutionId = ko.observable("");
	self.Report = ko.observable("ExportLab");
	self.Year = ko.observable(new Date().getFullYear());
	self.Month = ko.observable("");
	self.SE = ko.observable("");
	self.StartDate = ko.observable(null);
	self.EndDate = ko.observable(null);
	self.Generate = function (SampleNumber, data ) {
	    console.log(data);
	    var namevalues = { Report: "ExportLab", CountryID: app.Views.Home.selectedCountryId() ? app.Views.Home.selectedCountryId() : app.Views.Home.UsrCountry(), InstitutionID: app.Views.Home.selectedInstitutionId() > 0 ? app.Views.Home.selectedInstitutionId() : app.Views.Lab.LabsResult()[0].Id, RecordID: app.Views.Contact.Id(), NumSample: SampleNumber > 0 ? SampleNumber : 1 }

	    if (app.Views.Lab.validate()) {
	        app.Views.Lab.SaveLab();
	        window.open(app.dataModel.getPrintTest + "?" + $.param(namevalues, true), "_blank");
	    }
	    
	    //$.getJSON(app.dataModel.getPrintTest, namevalues, function (data, status) {
	    //    alert(data);
	    //    })
        //    .fail(function (jqXHR, textStatus, errorThrown) {
        //        //alert(errorThrown);
        //        console.log(errorThrown);
        //    });
	};
};

app.addViewModel({
	name: "PrintTest",
	bindingMemberName: "printtest",
	factory: PrintTestViewModel
});