
function TicketViewModel(app, dataModel) {
    var self = this;
    self.Id = "";
    self.UsrCountry = ko.observable(selcty); // Pais del usuario logueado
    self.ticketSubject = ko.observable("");
    self.ticketMsg = ko.observable("");
    self.ticketStatus = ko.observable("");
    self.ticketID = ko.observable("");
    //alert(msgTicketMissingValues);
    //alert(self.ticketSubject());
    //alert(typeof self.ticketSubject());
    //alert(self.ticketMsg());
    //alert(typeof self.ticketMsg());

    self.SendTicket = function () {        
        if ((typeof self.ticketSubject() != "undefined") && self.ticketSubject() != "" && (typeof self.ticketMsg() != "undefined") && self.ticketMsg() != "") {
            //alert("Send");
            var formData = new FormData();
            //formData = $('#myfile')[0].files[0];
           
            $('#attachmentLoader').show();
            $('#newTicket').prop('disabled', true);
            formData.append("FileUpload", $('#ticketFile')[0].files[0]);
            formData.append("FileUpload", $('#ticketFile2')[0].files[0]);
            formData.append("FileUpload", $('#ticketFile3')[0].files[0]);
            //self.ticketID = ko.observable("");

            formData.append("UsrCountry", self.UsrCountry());
            formData.append("ticketSubject", self.ticketSubject());
            formData.append("ticketMsg", self.ticketMsg());
            console.log(formData);
            
            $.ajax({
                url: app.dataModel.createTicketUrl,
                type: 'POST',
                async:true,
                data: formData,
                processData: false,
                contentType: false,// not json               
                complete: function (data) {
                    console.log(data);
                    $('#attachmentLoader').css('display', 'none');
                    if (data.responseText == "1") {
                        alert("Ticket ingresado exitósamente");
                        /*$('#SubjectTicket').val('');
                        $('#DescriptionTicket').val('');
                        $("#TicketList tr").remove();*/
                        //-----------------------------
                        $('#newTicket').prop('disabled', false);
                        $('#SubjectTicket').val("");
                        $('#DescriptionTicket').val("");
                        $('#ticketFile').val("");
                        $('#ticketFile2').val("");
                        $('#ticketFile3').val("");
                        $('#fileSize1').text("");
                        $('#fileSize2').text("");
                        $('#fileSize3').text("");
                        $('#fileSizeTotal').text("");
                        $('#fileSizeTotal').css('color', 'blue');
                        $('#newTicket').prop('disabled', false);
                        $('#clearButton1').css('display', 'none');
                        $('#clearButton2').css('display', 'none');
                        $('#clearButton3').css('display', 'none');
                        //-----------------------------
                        $('#SubjectTicket').val('').change();
                        $('#DescriptionTicket').val('').change();
                        $('#ticketStatus').val('').change();
                        $('#IDTicket').val('').change();
                        /*self.ticketSubject = ko.observable("");
                        self.ticketMsg = ko.observable("");
                        self.ticketStatus = ko.observable("");
                        self.ticketID = ko.observable("");*/
                        GetTicketList();
                    }
                    else {
                        alert("El ticket no se pudo ingresar, por favor, envíe un correo electrónico al administrador del sistema");
                    }

                },
                error: function (response) {
                    console.log(response.responseText);
                    $('#newTicket').prop('disabled', true);
                    alert(response.responseText);
                    alert("El ticket no se pudo ingresar, por favor, envíe un correo electrónico al administrador del sistema");
                    $('#SubjectTicket').val('');
                    $('#DescriptionTicket').val('');
                }
            });
            
        }
        else {
            /*alert(self.ticketSubject());
            alert(typeof self.ticketSubject());
            alert(self.ticketMsg());
            alert(typeof self.ticketMsg());*/
            alert(msgTicketMissingValues);
        }
    };

    self.EditTicket = function () {
        var formData = new FormData();
        /*alert(typeof self.ticketID());
        alert(self.ticketID());
        alert(typeof self.ticketSubject());
        alert(self.ticketSubject());
        alert(typeof self.ticketMsg());
        alert(self.ticketMsg());
        alert(typeof self.ticketStatus());
        alert(self.ticketStatus());*/
        if ((typeof self.ticketID() != "undefined") && self.ticketID() != "" && (typeof self.ticketSubject() != "undefined") && self.ticketSubject() != "" && (typeof self.ticketMsg() != "undefined") && self.ticketMsg() != "" && (typeof self.ticketStatus() != "undefined") && self.ticketStatus() != "0") {
            $('#attachmentLoader').show();
            $('#newTicket').prop('disabled', true);
            $('#editTicket').prop('disabled', true);            
            formData.append("UsrCountry", self.UsrCountry());
            formData.append("ticketSubject", self.ticketSubject());
            formData.append("ticketMsg", self.ticketMsg());
            formData.append("ticketStatus", self.ticketStatus());
            formData.append("ticketID", self.ticketID());
            console.log(formData);

            $.ajax({
                url: app.dataModel.editTicketUrl,
                type: 'POST',
                async: true,
                data: formData,
                processData: false,
                contentType: false,// not json               
                complete: function (data) {
                    console.log(data);                    
                    $('#attachmentLoader').css('display', 'none');
                    if (data.responseText == "1") {
                        alert("Ticket actualizado");                        
                        //-----------------------------
                        $('#newTicket').prop('disabled', false);
                        $('#SubjectTicket').val("");
                        $('#DescriptionTicket').val("");
                        $('#ticketFile').val("");
                        $('#ticketFile2').val("");
                        $('#ticketFile3').val("");
                        $('#fileSize1').text("");
                        $('#fileSize2').text("");
                        $('#fileSize3').text("");
                        $('#fileSizeTotal').text("");
                        $('#fileSizeTotal').css('color', 'blue');
                        $('#newTicket').prop('disabled', false);
                        $('#clearButton1').css('display', 'none');
                        $('#clearButton2').css('display', 'none');
                        $('#clearButton3').css('display', 'none');
                        $('#ticketStatus').val("0");
                        //-----------------------------
                        
                        $('#SubjectTicket').prop('disabled', false);
                        $('#DescriptionTicket').prop('disabled', false);
                        $('#ticketStatus').prop('disabled', true);
                        //-----------------------------
                        $('#SubjectTicket').val('').change();
                        $('#DescriptionTicket').val('').change();
                        $('#ticketStatus').val('').change();
                        $('#IDTicket').val('').change();

                        //self.ticketSubject = ko.observable("");
                        //self.ticketMsg = ko.observable("");
                        //self.ticketStatus = ko.observable("");
                        //self.ticketID = ko.observable("");
                        GetTicketList();
                        //GetTicketList();
                    }
                    else {
                        alert("El ticket no se pudo ingresar, por favor, envíe un correo electrónico al administrador del sistema");
                    }

                },
                error: function (response) {
                    console.log(response.responseText);
                    $('#newTicket').prop('disabled', true);
                    alert(response.responseText);
                    alert("El ticket no se pudo actualizar, por favor, envíe un correo electrónico al administrador del sistema");
                    $('#SubjectTicket').val('');
                    $('#DescriptionTicket').val('');
                }
            });            
        }
        else {            
            alert(msgTicketMissingValues); alert("v");
        }
    };

    /*self.GetTicketList = function () {        
        $.postJSON(app.dataModel.getTicketUrl, { })
            .success(function (data, textStatus, jqXHR) {
                console.log(data);
                $("#TicketList").show();                
                var trHTML = '';
                $.each(data, function (i, item) {
                    trHTML += '<tr><td style="width:10%;">' + item.ID + '</td><td style="width:10%;">' + item.subject + '</td><td style="width:10%;">' + item.description + '</td><td style="width:10%;">' + item.status + '</td><td style="width:10%;">' + item.ticketDate + '</td></tr>';
                });
                $('#TicketList').append(trHTML);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
                alert("La lista de tickets no pudo ser obtenida");
                $('#SubjectTicket').val('');
                $('#DescriptionTicket').val('');
            })                
    };*/

        

    self.GetYearSummaryForYearItems = function () {
        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "") {
            $.postJSON(app.dataModel.createTicketUrl, { UsrCountry: self.UsrCountry(), ticketSubject: self.ticketSubject(), ticketMsg: self.ticketMsg })
               .success(function (data, textStatus, jqXHR) {
                   console.log(data);
               })
               .fail(function (jqXHR, textStatus, errorThrown) {
                   alert(errorThrown);
               })
        }
    };

    return self;
};


app.addViewModel({
    name: "Ticket",
    bindingMemberName: "ticket",
    factory: TicketViewModel
});


/*var TicketViewModel = function () {
    var self = this;
    self.Id = "";
    self.UsrCountry = ko.observable(selcty); // Pais del usuario logueado
    self.ticketSubject = ko.observable(self.ticketSubject);
    self.ticketMsg = ko.observable(self.ticketMsg);
    alert("Entro en el modelo");
    alert(self.ticketSubject);
    
};*/

//ko.applyBindings(new TicketViewModel()); // This makes Knockout get to work