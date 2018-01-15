
function TicketViewModel(app, dataModel) {
    var self = this;
    self.Id = "";
    self.UsrCountry = ko.observable(selcty); // Pais del usuario logueado
    self.ticketSubject = ko.observable("");
    self.ticketMsg = ko.observable("");
    //alert(msgTicketMissingValues);
    //alert(self.ticketSubject());
    //alert(typeof self.ticketSubject());
    //alert(self.ticketMsg());
    //alert(typeof self.ticketMsg());

    self.SendTicket = function () {        
        if ((typeof self.ticketSubject() != "undefined") && self.ticketSubject() != "" && (typeof self.ticketMsg() != "undefined") && self.ticketMsg() != "") {
            //alert("Send");
            $.postJSON(app.dataModel.createTicketUrl, { UsrCountry: self.UsrCountry(), ticketSubject: self.ticketSubject(), ticketMsg: self.ticketMsg() })
                .success(function (data, textStatus, jqXHR) {
                    console.log(data);
                    if (data == 1) {
                        alert("Ticket ingresado exitósamente");
                        $('#SubjectTicket').val('');
                        $('#DescriptionTicket').val('');
                        $("#TicketList tr").remove();
                        self.ticketSubject = ko.observable("");
                        self.ticketMsg = ko.observable("");
                        GetTicketList();
                    }
                    else {
                        alert("El ticket no se pudo ingresar, por favor, envíe un correo electrónico al administrador del sistema");
                    }
                })
               .fail(function (jqXHR, textStatus, errorThrown) {
                   alert(errorThrown);
                   alert("El ticket no se pudo ingresar, por favor, envíe un correo electrónico al administrador del sistema");
                   $('#SubjectTicket').val('');
                   $('#DescriptionTicket').val('');
               })
        }
        else {
            /*alert(self.ticketSubject());
            alert(typeof self.ticketSubject());
            alert(self.ticketMsg());
            alert(typeof self.ticketMsg());*/
            alert(msgTicketMissingValues);
        }
    };

    self.GetTicketList = function () {        
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
    };

        

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