using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Resources.Abstract;
using Resources.Concrete;
using System.Configuration;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace Paho.Controllers
{
    public class ResourcesM : ControllerBase
    {
        private static IResourceProvider resourceProvider = new DbResourceProvider();
        
        /// <summary>Estado caso</summary>
        /// 
        public string getMessage(string msg, int? countryID, string countryLang)
        {
            string dbMessage = msg;
            string dbCountry = countryID.ToString();
            string dbLang = countryLang;
                                   
            //dbMessage = "Pepito";
            dbMessage = resourceProvider.GetResource(dbMessage, dbCountry).ToString();
            if (dbMessage == "")
            {
                dbMessage = msg;
                dbMessage = resourceProvider.GetResource(dbMessage, dbLang).ToString();
            }
            return dbMessage;
        }
        public static string msgCaselistVigTabCase
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabCase", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Dat. vigilancia</summary>
        public static string msgCaselistVigTabContact
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContact", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Edad:</summary>
        public static string msgCaselistVigTabContactLabelAge
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelAge", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Grupo de edad</summary>
        public static string msgCaselistVigTabContactLabelAgeGroup
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelAgeGroup", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Otro registro</summary>
        public static string msgCaselistVigTabContactLabelAnotherRecord
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelAnotherRecord", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Historia clínica</summary>
        public static string msgCaselistVigTabContactLabelClinicalHistory
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelClinicalHistory", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha de captación:</summary>
        public static string msgCaselistVigTabContactLabelDateOfCollect
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelDateOfCollect", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha de detección</summary>
        public static string msgCaselistVigTabContactLabelDateOfDetection
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelDateOfDetection", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha de notificación:</summary>
        public static string msgCaselistVigTabContactLabelDateOfNotification
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelDateOfNotification", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Día</summary>
        public static string msgCaselistVigTabContactLabelDay
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelDay", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha de nacimiento:</summary>
        public static string msgCaselistVigTabContactLabelDOB
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelDOB", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Cédula</summary>
        public static string msgCaselistVigTabContactLabelDocumentName
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelDocumentName", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Pueblo originario</summary>
        public static string msgCaselistVigTabContactLabelEthnicity
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelEthnicity", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Pueblo...</summary>
        public static string msgCaselistVigTabContactLabelEthnicityLabel
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelEthnicityLabel", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>1er. nombre</summary>
        public static string msgCaselistVigTabContactLabelFirstFirstName
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelFirstFirstName", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>1er. apellido</summary>
        public static string msgCaselistVigTabContactLabelFirstLastName
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelFirstLastName", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Extranjero con número de asegurado</summary>
        public static string msgCaselistVigTabContactLabelForeigner
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelForeigner", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Establecimiento:</summary>
        public static string msgCaselistVigTabContactLabelHospital
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelHospital", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>No. Reg. de hospital</summary>
        public static string msgCaselistVigTabContactLabelHospitalRegNumber
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelHospitalRegNumber", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>No. identificación:</summary>
        public static string msgCaselistVigTabContactLabelIdNo
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelIdNo", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Tipo identificación:</summary>
        public static string msgCaselistVigTabContactLabelIdType
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelIdType", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>ETI</summary>
        public static string msgCaselistVigTabContactLabelIli
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelIli", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Documento interno</summary>
        public static string msgCaselistVigTabContactLabelInternalDocument
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelInternalDocument", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>2do. apellido</summary>
        public static string msgCaselistVigTabContactLabelLastLastName
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelLastLastName", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>2do. nombre</summary>
        public static string msgCaselistVigTabContactLabelMiddleName
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelMiddleName", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Mes</summary>
        public static string msgCaselistVigTabContactLabelMonth
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelMonth", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Nacionalidad</summary>
        public static string msgCaselistVigTabContactLabelNacionality
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelNacionality", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Siguiente</summary>
        public static string msgCaselistVigTabContactLabelNextButton
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelNextButton", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Otro</summary>
        public static string msgCaselistVigTabContactLabelOther
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelOther", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Brote</summary>
        public static string msgCaselistVigTabContactLabelOutbreak
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelOutbreak", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Pasaporte</summary>
        public static string msgCaselistVigTabContactLabelPassport
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelPassport", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha de registro:</summary>
        public static string msgCaselistVigTabContactLabelRecordDate
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelRecordDate", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Región CCSS</summary>
        public static string msgCaselistVigTabContactLabelRegionCCSS
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelRegionCCSS", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Región MINSA</summary>
        public static string msgCaselistVigTabContactLabelRegionMINSA
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelRegionMINSA", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>RUN</summary>
        public static string msgCaselistVigTabContactLabelRUN
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelRUN", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>IRAG</summary>
        public static string msgCaselistVigTabContactLabelSari
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelSari", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Sexo</summary>
        public static string msgCaselistVigTabContactLabelSex
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelSex", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Femenino</summary>
        public static string msgCaselistVigTabContactLabelSexFemale
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelSexFemale", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Masculino</summary>
        public static string msgCaselistVigTabContactLabelSexMale
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelSexMale", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Indefinido</summary>
        public static string msgCaselistVigTabContactLabelSexUndefined
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelSexUndefined", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Indeterminado</summary>
        public static string msgCaselistVigTabContactLabelSexUndetermined
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelSexUndetermined", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Vigilancia</summary>
        public static string msgCaselistVigTabContactLabelSurv
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelSurv", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Caso inusitado</summary>
        public static string msgCaselistVigTabContactLabelUnusualCase
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelUnusualCase", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Año</summary>
        public static string msgCaselistVigTabContactLabelYear
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelYear", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Años, meses o días</summary>
        public static string msgCaselistVigTabContactLabelYMorD
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabContactLabelYMorD", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Ref. geográfica</summary>
        public static string msgCaselistVigTabGeo
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeo", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Atrás</summary>
        public static string msgCaselistVigTabGeoBtnBack
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoBtnBack", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Siguiente</summary>
        public static string msgCaselistVigTabGeoBtnNext
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoBtnNext", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Dirección</summary>
        public static string msgCaselistVigTabGeoLabelAddress
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelAddress", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Área</summary>
        public static string msgCaselistVigTabGeoLabelArea
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelArea", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Teléfono</summary>
        public static string msgCaselistVigTabGeoLabelPhone
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelPhone", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>País de procedencia</summary>
        public static string msgCaselistVigTabGeoLabelProcedenceCountry
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelProcedenceCountry", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Lugar de residencia</summary>
        public static string msgCaselistVigTabGeoLabelResidence
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelResidence", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Cantón</summary>
        public static string msgCaselistVigTabGeoLabelResidenceCanton
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelResidenceCanton", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Comuna</summary>
        public static string msgCaselistVigTabGeoLabelResidenceComune
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelResidenceComune", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>/Comunidad</summary>
        public static string msgCaselistVigTabGeoLabelResidenceComunity
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelResidenceComunity", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>País de residencia</summary>
        public static string msgCaselistVigTabGeoLabelResidenceCountry
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelResidenceCountry", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Departamento</summary>
        public static string msgCaselistVigTabGeoLabelResidenceDepartamento
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelResidenceDepartamento", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Distrito</summary>
        public static string msgCaselistVigTabGeoLabelResidenceDistrict
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelResidenceDistrict", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Latitud de residencia</summary>
        public static string msgCaselistVigTabGeoLabelResidenceLatitude
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelResidenceLatitude", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Longitud de residencia</summary>
        public static string msgCaselistVigTabGeoLabelResidenceLongitude
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelResidenceLongitude", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Municipio</summary>
        public static string msgCaselistVigTabGeoLabelResidenceMunicipio
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelResidenceMunicipio", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Provincia</summary>
        public static string msgCaselistVigTabGeoLabelResidenceProvince
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelResidenceProvince", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Región</summary>
        public static string msgCaselistVigTabGeoLabelResidenceRegion
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelResidenceRegion", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Rural</summary>
        public static string msgCaselistVigTabGeoLabelRural
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelRural", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Desconocida</summary>
        public static string msgCaselistVigTabGeoLabelUnknown
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelUnknown", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Urbana</summary>
        public static string msgCaselistVigTabGeoLabelUrban
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelUrban", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Dónde se encontraba hace 2 semanas?</summary>
        public static string msgCaselistVigTabGeoLabelWhere2WeeksAgo
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelWhere2WeeksAgo", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Cantón</summary>
        public static string msgCaselistVigTabGeoLabelWhereCanton
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelWhereCanton", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Comuna</summary>
        public static string msgCaselistVigTabGeoLabelWhereComune
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelWhereComune", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Pais</summary>
        public static string msgCaselistVigTabGeoLabelWhereCountry
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelWhereCountry", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Departamento</summary>
        public static string msgCaselistVigTabGeoLabelWhereDepartamento
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelWhereDepartamento", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Distrito/Comunidad</summary>
        public static string msgCaselistVigTabGeoLabelWhereDistrictComunity
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelWhereDistrictComunity", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Municipio</summary>
        public static string msgCaselistVigTabGeoLabelWhereMunicipio
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelWhereMunicipio", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Provncia</summary>
        public static string msgCaselistVigTabGeoLabelWhereProvince
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelWhereProvince", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Región</summary>
        public static string msgCaselistVigTabGeoLabelWhereRegion
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabGeoLabelWhereRegion", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Bitácora</summary>
        public static string msgCaselistVigTabHistory
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabHistory", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Inf. clínica</summary>
        public static string msgCaselistVigTabHospital
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabHospital", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Laboratorio</summary>
        public static string msgCaselistVigTabLab
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabLab", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Registro</summary>
        public static string msgCaselistVigTabRecord
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRecord", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Hist. médico</summary>
        public static string msgCaselistVigTabRisk
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRisk", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Antiviral</summary>
        public static string msgCaselistVigTabRiskLabelAntiviral
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelAntiviral", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Tratamiento de antiviral?</summary>
        public static string msgCaselistVigTabRiskLabelAntiviralTreatment
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelAntiviralTreatment", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha de administración de antiviral</summary>
        public static string msgCaselistVigTabRiskLabelAntiviralTreatmentDoseDate
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelAntiviralTreatmentDoseDate", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha de fin de antiviral</summary>
        public static string msgCaselistVigTabRiskLabelAntiviralTreatmentEndDate
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelAntiviralTreatmentEndDate", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>No</summary>
        public static string msgCaselistVigTabRiskLabelAntiviralTreatmentNo
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelAntiviralTreatmentNo", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Dosis</summary>
        public static string msgCaselistVigTabRiskLabelAntiviralTreatmentOseltaDose
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelAntiviralTreatmentOseltaDose", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha de inicio de antiviral</summary>
        public static string msgCaselistVigTabRiskLabelAntiviralTreatmentStartDate
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelAntiviralTreatmentStartDate", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Tipo de antiviral</summary>
        public static string msgCaselistVigTabRiskLabelAntiviralTreatmentType
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelAntiviralTreatmentType", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Sin información</summary>
        public static string msgCaselistVigTabRiskLabelAntiviralTreatmentTypeNoInfo
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelAntiviralTreatmentTypeNoInfo", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>No sabe</summary>
        public static string msgCaselistVigTabRiskLabelAntiviralTreatmentUnknown
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelAntiviralTreatmentUnknown", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Si</summary>
        public static string msgCaselistVigTabRiskLabelAntiviralTreatmentYes
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelAntiviralTreatmentYes", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>BCG</summary>
        public static string msgCaselistVigTabRiskLabelBCG
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelBCG", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha</summary>
        public static string msgCaselistVigTabRiskLabelBCGDate
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelBCGDate", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Dosis (fecha)</summary>
        public static string msgCaselistVigTabRiskLabelDose
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelDose", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Primera dosis (fecha)</summary>
        public static string msgCaselistVigTabRiskLabelFirstDose
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelFirstDose", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Influenza</summary>
        public static string msgCaselistVigTabRiskLabelFlu
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelFlu", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Haemophilus</summary>
        public static string msgCaselistVigTabRiskLabelHaemophilus
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelHaemophilus", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha</summary>
        public static string msgCaselistVigTabRiskLabelHaemophilusDate
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelHaemophilusDate", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Neumococo</summary>
        public static string msgCaselistVigTabRiskLabelNeumococo
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelNeumococo", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha</summary>
        public static string msgCaselistVigTabRiskLabelNeumococoDate
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelNeumococoDate", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Pentavalente</summary>
        public static string msgCaselistVigTabRiskLabelPentavalente
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPentavalente", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha</summary>
        public static string msgCaselistVigTabRiskLabelPentavalenteDate
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPentavalenteDate", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Embarazo</summary>
        public static string msgCaselistVigTabRiskLabelPregnancy
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnancy", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Primero</summary>
        public static string msgCaselistVigTabRiskLabelPregnancyTrimesterFirst
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnancyTrimesterFirst", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Segundo</summary>
        public static string msgCaselistVigTabRiskLabelPregnancyTrimesterSecond
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnancyTrimesterSecond", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Tercero</summary>
        public static string msgCaselistVigTabRiskLabelPregnancyTrimesterThird
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnancyTrimesterThird", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>No sabe</summary>
        public static string msgCaselistVigTabRiskLabelPregnancyTrimesterUnknown
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnancyTrimesterUnknown", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Trimestre de gestación</summary>
        public static string msgCaselistVigTabRiskLabelPregnancyTrimesterV1
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnancyTrimesterV1", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Trimestre</summary>
        public static string msgCaselistVigTabRiskLabelPregnancyTrimesterV2
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnancyTrimesterV2", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Semana de gestación</summary>
        public static string msgCaselistVigTabRiskLabelPregnancyWeek
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnancyWeek", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Embarazada</summary>
        public static string msgCaselistVigTabRiskLabelPregnant
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnant", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>No sabe</summary>
        public static string msgCaselistVigTabRiskLabelPregnantDoesntKnow
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnantDoesntKnow", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>No</summary>
        public static string msgCaselistVigTabRiskLabelPregnantNo
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnantNo", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Puerperio</summary>
        public static string msgCaselistVigTabRiskLabelPregnantPuerperium
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnantPuerperium", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Si</summary>
        public static string msgCaselistVigTabRiskLabelPregnantYes
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelPregnantYes", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Asma</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesAsthma
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesAsthma", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Parálisis cerebral</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesCerebralPalsy
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesCerebralPalsy", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Cardiopatía crónica</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesChronicHeartDisease
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesChronicHeartDisease", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Enfermedad hepática crónica</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesChronicHepaticDisease
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesChronicHepaticDisease", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Enfermedad neurológica crónica</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesChronicNeurologicalDisease
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesChronicNeurologicalDisease", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Enfermedad pulmonar crónica</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesChronicPulmonaryDisease
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesChronicPulmonaryDisease", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Enfermedad renal crónica</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesChronicRenalDisease
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesChronicRenalDisease", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Diabetes</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesDiabetes
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesDiabetes", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Inmunocomprometido por enfermedad o tratamiento</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesImmunocompromisedByIllnesOrTreatment
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesImmunocompromisedByIllnesOrTreatment", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Comorbilidades</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesLabel
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsComorbiditiesLabel", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Factores de riesgo y otras comorbilidades</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsLabelV1
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsLabelV1", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Factores de riesgo</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsLabelV2
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsLabelV2", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Factor de riesgo</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabel
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabel", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Alcoholismo</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelAlcoholism
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelAlcoholism", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Síndrome de Down</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelDown
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelDown", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Trabajador de salud</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelHealthcareWorker
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelHealthcareWorker", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Indígena</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelNative
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelNative", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>No</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelNo
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelNo", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Obesidad</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesity
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesity", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>IMC entre 30 y 40</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityIMC3040
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityIMC3040", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>IMC mayor de 40</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityIMC40
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityIMC40", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>No</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityNo
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityNo", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>IMC de obesidad sin datos</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityNoData
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityNoData", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Sin datos o sin obesidad</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityUnknown
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityUnknown", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Si, obesidad</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityYes
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityYes", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Si, mórbida</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityYesMorbid
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelObesityYesMorbid", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Otra comorbilidad</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelOtherComorbidities
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelOtherComorbidities", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Tabaquismo</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelTabaquism
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelTabaquism", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Sin información</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelUnknown
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelUnknown", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Si</summary>
        public static string msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelYes
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelRiskFactorsPresenceLabelYes", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Segunda dosis (fecha)</summary>
        public static string msgCaselistVigTabRiskLabelSecondDose
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelSecondDose", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Vacuna</summary>
        public static string msgCaselistVigTabRiskLabelVaccine
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelVaccine", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Vacuna</summary>
        public static string msgCaselistVigTabRiskLabelVaccineLabel
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelVaccineLabel", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha</summary>
        public static string msgCaselistVigTabRiskLabelVaccineLabelDate
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelVaccineLabelDate", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Dosis</summary>
        public static string msgCaselistVigTabRiskLabelVaccineLabelDosage
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelVaccineLabelDosage", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>No</summary>
        public static string msgCaselistVigTabRiskLabelVaccineLabelNo
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelVaccineLabelNo", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>No info.</summary>
        public static string msgCaselistVigTabRiskLabelVaccineLabelNoInfo
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelVaccineLabelNoInfo", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Si</summary>
        public static string msgCaselistVigTabRiskLabelVaccineLabelYes
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelVaccineLabelYes", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fuente de la información de vacuna</summary>
        public static string msgCaselistVigTabRiskLabelVaccineSourceRecord
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelVaccineSourceRecord", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fuente vacuna...</summary>
        public static string msgCaselistVigTabRiskLabelVaccineSourceRecordSelect
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabRiskLabelVaccineSourceRecordSelect", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Seleccione...</summary>
        public static string msgSelectLabel
        {
            get
            {
                return resourceProvider.GetResource("msgSelectLabel", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Mensaje de prueba</summary>
        public static string testMessage
        {
            get
            {
                return resourceProvider.GetResource("testMessage", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Nuevo registro</summary>
        public static string viewHome_btnNew
        {
            get
            {
                return resourceProvider.GetResource("viewHome_btnNew", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Buscar</summary>
        public static string viewHome_btnSearch
        {
            get
            {
                return resourceProvider.GetResource("viewHome_btnSearch", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Apellidos</summary>
        public static string viewHome_msgApellidos
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgApellidos", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>TOTAL DE REGISTROS DE LA VIGILANCIA</summary>
        public static string viewHome_msgCaselist
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgCaselist", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Registros pendientes</summary>
        public static string viewHome_msgCaselist_Pend
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgCaselist_Pend", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Estatus</summary>
        public static string viewHome_msgCaselistEstatus
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgCaselistEstatus", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Id. Reg.</summary>
        public static string viewHome_msgCaselistIdReg
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgCaselistIdReg", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Vig.</summary>
        public static string viewHome_msgCaselistVig
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgCaselistVig", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Conclusión</summary>
        public static string viewHome_msgConclusion
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgConclusion", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Docu. identi.</summary>
        public static string viewHome_msgDocuIdent
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgDocuIdent", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Fecha noti.</summary>
        public static string viewHome_msgFechaNoti
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgFechaNoti", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Establecimiento de salud:</summary>
        public static string viewHome_msgHospitals
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgHospitals", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>IFI</summary>
        public static string viewHome_msgIFI
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgIFI", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Establecimiento de salud:</summary>
        public static string viewHome_msgLabs
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgLabs", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Nombres</summary>
        public static string viewHome_msgNombre
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgNombre", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>PCR</summary>
        public static string viewHome_msgPCR
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgPCR", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Región:</summary>
        public static string viewHome_msgRegionals
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgRegionals", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>No. identificación:</summary>
        public static string viewHome_msgSCasenumber
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgSCasenumber", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Cond. egreso:</summary>
        public static string viewHome_msgSCaseStatus
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgSCaseStatus", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Apellido:</summary>
        public static string viewHome_msgSLastName
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgSLastName", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Nombre:</summary>
        public static string viewHome_msgSName
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgSName", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Hasta Fecha noti.:</summary>
        public static string viewHome_msgSNEndDate
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgSNEndDate", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>De Fecha noti.:</summary>
        public static string viewHome_msgSNStartDate
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgSNStartDate", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

        /// <summary>Id registro:</summary>
        public static string viewHome_msgSRecordId
        {
            get
            {
                return resourceProvider.GetResource("viewHome_msgSRecordId", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }

    }

}

