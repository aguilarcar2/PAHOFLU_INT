function AppDataModel() {
    var self = this;
    // Routes
    self.userInfoUrl = "/api/Me";
    self.siteUrl = "/";
    self.getStatesUrl = "/cases/GetStates";
    self.getNeighborhoodsUrl = "/cases/GetNeighborhoods";
    self.getLocalsUrl = "/cases/GetNeighborhoods";
    self.getAreasUrl = "/cases/GetAreas"; 
    self.getContactUrl = "/cases/GetContact";
    self.getPatientInformationUrl = "/cases/GetPatientInformation"
    self.saveContactUrl = "/cases/SaveContact";  
    self.getHospitalUrl = "/cases/GetHospital";
    self.saveHospitalUrl = "/cases/SaveHospital";
    self.getRiskUrl = "/cases/GetRisk";
    self.saveRiskUrl = "/cases/SaveRisk"
    self.getGEOUrl = "/cases/GetGEO";
    self.saveGEOUrl = "/cases/SaveGEO"
    self.getLabUrl = "/cases/GetLab";
    self.getRecordHistoryURL = "/cases/GetRecordHistory";
    self.getLabsHospitalUrl = "/cases/GetLabsHospital";
    self.saveLabUrl = "/cases/SaveLab";
    self.getVirusTypesUrl = "/testcases/GetVirusTypes";
    self.getInstitutionsUrl = "/cases/GetInstitutions";
    self.getRegionsUrl = "/cases/GetRegions";
    self.getSummayDetailsUrl = "/summary/GetSummaryDetails";
    self.getSummayDetailsExcel = "/exportar/GetSummaryDetailsExcel";
    self.getSummaryForYearUrl = "/summary/GetSummaryForYear"
    self.saveSummayDetailsUrl = "/summary/SaveSummary";
    self.getCasesInExcelUrl = "/exportar/GetCasesInExcel";
    self.getExportar = "/exportar/GetExcel";
    self.getFluid = "/fluid/Generate";
    self.getExportarGraphics = "/graphics/GenerateGraphic";
    self.getPrintTest =  "/printtest/GetPrint"
    self.getFlucasesUrl = "/report/GetFluCases";
    self.deletetUrl = "/cases/DeleteCase";

    //sistema de tickets
    self.createTicketUrl = "/ticket/CreateTicket";
    self.getTicketUrl = "/ticket/GetTicket";

    // Formato de fechas
    //self.date_format_ = "dd/mm/yyyy";
    self.date_format_ = "dd/mm/yyyy";
    self.date_format_moment = "DD/MM/YYYY";
    self.date_format_ISO = "YYYY-MM-DD";

    //self.getFluCasesUrl = "/cases/GetFluCases";

    // Route operations

    // Other private operations

    // Operations

    // Data
    self.returnUrl = self.siteUrl;

    // Data access operations
    self.setAccessToken = function (accessToken) {
        sessionStorage.setItem("accessToken", accessToken);
    };

    self.getAccessToken = function () {
        return sessionStorage.getItem("accessToken");
    };


}
