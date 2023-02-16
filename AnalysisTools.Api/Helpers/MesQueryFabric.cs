using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace analysistools.api.Helpers
{
    /// <summary>
    /// This class allows to create different queries to fetch data from MES database.
    /// </summary>
    public static class MesQueryFabric
    {
        /// <summary>
        /// Query used to know wheter the unit is golden or not.
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <returns></returns>
        public static string IsGoldenQuery(string SerialNumber)
        {
            return $"SELECT AL1.RUN_DATE, AL5.PDK_AUFTR FROM EVAPROD.PD_LFD_RUN AL1, EVAPROD.PD_LFD_MAT AL2, EVAPROD.PD_STM_PRP AL3, EVACOMP.PD_LFD_BMN AL4, EVAPROD.PD_LFD_AUF AL5 WHERE (AL2.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL4.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL1.PRP_DATE_ID=AL3.PRP_DATE_ID AND AL5.PRD_SPC_SID=AL1.PRD_SPC_SID)  AND (AL3.PRP_VAR IN ('FAKRA', 'FAKRA_NEW','PINCHECK') AND AL1.RUNID='{SerialNumber}') ORDER BY AL1.RUN_DATE DESC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY";
        }

        /// <summary>
        /// Query to fetch test details of optical products.
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="ToleranceInMinutes"></param>
        /// <param name="TestTime"></param>
        /// <returns></returns>
        public static string TestDetailsQuery(string SerialNumber, double ToleranceInMinutes, DateTime TestTime)
        {
            DateTime fromTestDate = TestTime.AddMinutes(-ToleranceInMinutes);
            DateTime toTestDate = TestTime.AddMinutes(ToleranceInMinutes);
            string fromTestDateString = fromTestDate.ToString("dd-MMM-yyyy HH:mm:ss");
            string toTestDateString = toTestDate.ToString("dd-MMM-yyyy HH:mm:ss");
            return $"SELECT AL1.RUNID, AL1.RUN_STATE, TO_CHAR(AL1.RUN_DATE, 'DD-MM-YYYY HH24:MI:SS') AS RUN_DATE, AL6.BMT_NAME, AL2.PDK_MATERIAL, AL3.PRP_VAR, AL1.RUNID_TYPE, AL4.MRK_NUM, AL5.MRK_BEZ, AL4.MRK_WERT, AL4.MRK_EIN_GUT, AL5.MRK_USG, AL5.MRK_OSG, AL7.PDK_AUFTR FROM EVAPROD.PD_LFD_RUN AL1, EVAPROD.PD_LFD_MAT AL2, EVAPROD.PD_STM_PRP AL3, EVAPROD.PD_LFD_MED2 AL4, EVAPROD.PD_STM_MRK AL5, EVACOMP.PD_LFD_BMN AL6, EVAPROD.PD_LFD_AUF AL7 WHERE (AL2.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL6.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL1.PRP_DATE_ID=AL3.PRP_DATE_ID AND AL1.RUN_KEY_PRT=AL4.RUN_KEY_PRT AND AL1.RUN_SEQ_KEY=AL4.RUN_SEQ_KEY AND AL1.PRP_DATE_ID=AL5.PRP_DATE_ID AND AL5.PRP_DATE_ID=AL3.PRP_DATE_ID AND AL4.MRK_NUM=AL5.MRK_NUM AND AL7.PRD_SPC_SID=AL1.PRD_SPC_SID)  AND (AL4.MRK_EIN_GUT='F' AND AL3.PRP_VAR IN ('FAKRA', 'FAKRA_NEW','PINCHECK', 'PINCHEK2') AND AL1.RUNID='{SerialNumber}' AND AL1.RUN_STATE='F' AND AL1.RUN_DATE BETWEEN TO_DATE('{fromTestDateString}','dd-mon-yyyy hh24:mi:ss') AND TO_DATE('{toTestDateString}','dd-mon-yyyy hh24:mi:ss')) OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY";
        }

        /// <summary>
        /// Query to retrieve information about an optical product.
        /// If the product is golden it only gets the last test (Golden units are tested a lot of times, and are not necessary).
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="IsGolden"></param>
        /// <returns></returns>
        public static string ProductQuery(string SerialNumber, bool IsGolden)
        {
            return IsGolden ? QueryForGoldenProduct(SerialNumber) : QueryForNormalProduct(SerialNumber);
        }

        /// <summary>
        /// Return ICT Analytics query, please set TestNumber to null if you don't need it.
        /// Remember that if you pass a TestNumber, Component will be ignored.
        /// </summary>
        /// <param name="Model">Product model you are interested in</param>
        /// <param name="Component">Component to search tests</param>
        /// <param name="TestNumber">You can also give test number instead of component</param>
        /// <param name="fromDate">When the query begins (Difference must be less than 1 day)</param>
        /// <param name="toDate">When the query ends (Difference must be less than 1 day)</param>
        /// <returns></returns>
        public static string ICTAnalytics(string Model, string Component, string TestNumber, DateTime fromDate, DateTime toDate)
        {
            if (toDate < fromDate)
                throw new InvalidDataException("FromDate must be greater that ToDate");
            
            var timeDifference = toDate - fromDate;
            if(timeDifference.Days > 3)
                throw new InvalidDataException("Difference between date must be less than 3 days");

            string fromTestDateString = fromDate.ToString("dd-MMM-yyyy HH:mm:ss");
            string toTestDateString = toDate.ToString("dd-MMM-yyyy HH:mm:ss");
            if (TestNumber == null)
                return $"SELECT AL1.RUNID, AL1.RUN_STATE, TO_CHAR(AL1.RUN_DATE, 'DD-MM-YYYY HH24:MI:SS') AS RUN_DATE, AL6.BMT_NAME, AL2.PDK_MATERIAL, AL4.MRK_NUM, AL5.MRK_BEZ, AL4.MRK_WERT, AL5.MRK_EINHEIT, AL5.MRK_USG, AL5.MRK_OSG FROM EVAPROD.PD_LFD_RUN AL1, EVAPROD.PD_LFD_MAT AL2, EVAPROD.PD_STM_PRP AL3, EVAPROD.PD_LFD_MED2 AL4, EVAPROD.PD_STM_MRK AL5, EVACOMP.PD_LFD_BMN AL6 WHERE (AL2.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL6.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL1.PRP_DATE_ID=AL3.PRP_DATE_ID AND AL1.RUN_KEY_PRT=AL4.RUN_KEY_PRT AND AL1.RUN_SEQ_KEY=AL4.RUN_SEQ_KEY AND AL1.PRP_DATE_ID=AL5.PRP_DATE_ID AND AL5.PRP_DATE_ID=AL3.PRP_DATE_ID AND AL4.MRK_NUM=AL5.MRK_NUM)  AND (AL2.PDK_MATERIAL='{Model}' AND AL6.BMT_NAME LIKE '%ICT%' AND AL5.MRK_BEZ='{Component}'  AND AL1.RUN_DATE BETWEEN TO_DATE('{fromTestDateString}','dd-mon-yyyy hh24:mi:ss') AND TO_DATE('{toTestDateString}','dd-mon-yyyy hh24:mi:ss'))";
            return     $"SELECT AL1.RUNID, AL1.RUN_STATE, TO_CHAR(AL1.RUN_DATE, DD-MM-YYYY HH24:MI:SS') AS RUN_DATE, AL6.BMT_NAME, AL2.PDK_MATERIAL, AL4.MRK_NUM, AL5.MRK_BEZ, AL4.MRK_WERT, AL5.MRK_EINHEIT, AL5.MRK_USG, AL5.MRK_OSG FROM EVAPROD.PD_LFD_RUN AL1, EVAPROD.PD_LFD_MAT AL2, EVAPROD.PD_STM_PRP AL3, EVAPROD.PD_LFD_MED2 AL4, EVAPROD.PD_STM_MRK AL5, EVACOMP.PD_LFD_BMN AL6 WHERE (AL2.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL6.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL1.PRP_DATE_ID=AL3.PRP_DATE_ID AND AL1.RUN_KEY_PRT=AL4.RUN_KEY_PRT AND AL1.RUN_SEQ_KEY=AL4.RUN_SEQ_KEY AND AL1.PRP_DATE_ID=AL5.PRP_DATE_ID AND AL5.PRP_DATE_ID=AL3.PRP_DATE_ID AND AL4.MRK_NUM=AL5.MRK_NUM)  AND (AL2.PDK_MATERIAL='{Model}' AND AL6.BMT_NAME LIKE '%ICT%' AND AL5.MRK_NUM='{TestNumber}' AND AL1.RUN_DATE BETWEEN TO_DATE('{fromTestDateString}','dd-mon-yyyy hh24:mi:ss') AND TO_DATE('{toTestDateString}','dd-mon-yyyy hh24:mi:ss'))";            
        }

        private static string QueryForGoldenProduct(string SerialNumber)
        {
            return $"SELECT AL1.RUNID, AL1.RUN_STATE AS State, TO_CHAR(AL1.RUN_DATE, 'DD-MM-YYYY HH24:MI:SS') AS TestDate, AL4.BMT_NAME AS Station, AL2.PDK_MATERIAL FROM EVAPROD.PD_LFD_RUN AL1, EVAPROD.PD_LFD_MAT AL2, EVAPROD.PD_STM_PRP AL3, EVACOMP.PD_LFD_BMN AL4 WHERE (AL2.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL4.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL1.PRP_DATE_ID=AL3.PRP_DATE_ID)  AND (AL3.PRP_VAR IN ('FAKRA', 'FAKRA_NEW', 'PINCHECK', 'PINCHEK2') AND AL1.RUNID='{SerialNumber}') ORDER BY AL1.RUN_DATE DESC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY";
        }

        private static string QueryForNormalProduct(string SerialNumber)
        {
            return $"SELECT AL1.RUNID, AL1.RUN_STATE AS State, TO_CHAR(AL1.RUN_DATE, 'DD-MM-YYYY HH24:MI:SS') AS TestDate, AL4.BMT_NAME AS Station, AL2.PDK_MATERIAL FROM EVAPROD.PD_LFD_RUN AL1, EVAPROD.PD_LFD_MAT AL2, EVAPROD.PD_STM_PRP AL3, EVACOMP.PD_LFD_BMN AL4 WHERE (AL2.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL4.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL1.PRP_DATE_ID=AL3.PRP_DATE_ID)  AND (AL3.PRP_VAR IN ('FAKRA', 'FAKRA_NEW', 'PINCHECK', 'PINCHEK2') AND AL1.RUNID='{SerialNumber}') ORDER BY AL1.RUN_DATE DESC";
        }

        public static string QueryForScreenLDMProcess(string SerialNumber)
        {
            return $"SELECT WIP.T_WIP_SUBSET_LOG.Unit_Id_In, WIP.T_WIP_SUBSET_LOG.Unit_Id_In_Type, WIP.T_WIP_SUBSET_LOG.Dst_Equipment, WIP.T_WIP_SUBSET_LOG.Operator, TO_CHAR(WIP.T_WIP_SUBSET_LOG.Created, 'DD-MM-YYYY HH24:MI:SS') AS Created, WIP.T_WIP_SUBSET_LOG.Transaction_Type FROM WIP.T_WIP_SUBSET_LOG WHERE WIP.T_WIP_SUBSET_LOG.Unit_Id_In='{SerialNumber}' ORDER by WIP.T_WIP_SUBSET_LOG.Created ASC ";
        }

        public static string QueryForScreenLDM2020(DateTime fromDate, DateTime toDate, string Station, string IdType)
        {
            if (toDate < fromDate)
                throw new InvalidDataException("FromDate must be greater that ToDate");

            string fromTestDateString = fromDate.ToString("dd-MMM-yyyy HH:mm:ss");
            string toTestDateString = toDate.ToString("dd-MMM-yyyy HH:mm:ss");

            return $"SELECT WIP.T_Wip_Subset_Log.Unit_Id_In, WIP.T_Wip_Subset_Log.Unit_Id_In_Type, WIP.T_Wip_Subset_Log.Created, WIP.T_Wip_Job.Product_Definition, WIP.T_Wip_Subset_Log.Operator, WIPREP.T_Car_Subunit.Car_Id, WIP.T_Wip_Subset.Equipment FROM WIP.T_WIP_SUBSET_LOG, WIP.T_WIP_JOB, WIPREP.T_CAR_SUBUNIT, WIP.T_WIP_SUBSET WHERE WIP.T_WIP_SUBSET_LOG.Created BETWEEN to_date('{fromTestDateString}','dd-mon-yyyy hh24:mi:ss') AND to_date('{toTestDateString}','dd-mon-yyyy hh24:mi:ss') AND WIP.T_WIP_SUBSET_LOG.Operator='{Station}' AND WIP.T_WIP_SUBSET_LOG.Unit_Id_In_Type='{IdType}' AND WIP.T_WIP_JOB.Job=WIP.T_WIP_SUBSET_LOG.Src_Job AND WIP.T_WIP_SUBSET.Unit_Id_In=WIP.T_WIP_SUBSET_LOG.Unit_Id_Out AND WIP.T_WIP_SUBSET.Unit_Id_In_Type=WIP.T_WIP_SUBSET_LOG.Unit_Id_Out_Type AND WIPREP.T_CAR_SUBUNIT.Subunit_Id=WIP.T_WIP_SUBSET_LOG.Unit_Id_Out ORDER by WIP.T_WIP_SUBSET_LOG.Created ASC";
        }

        public static string QueryForRawDataFPY(string product,DateTime fromDate, DateTime toDate)
        {
            if (toDate < fromDate)
                throw new InvalidDataException("FromDate must be greater that ToDate");

            string fromTestDateString = fromDate.ToString("dd-MMM-yyyy HH:mm:ss");
            string toTestDateString = toDate.ToString("dd-MMM-yyyy HH:mm:ss");

            return $"SELECT AL1.RUNID, AL2.PDK_AUFTR, AL1.RUN_STATE, AL1.RUN_DATE, AL3.PDK_MATERIAL, AL5.BMT_NAME, AL4.PRP_VAR, AL1.RUNID_TYPE FROM EVAPROD.PD_LFD_RUN AL1, EVAPROD.PD_LFD_AUF AL2, EVAPROD.PD_LFD_MAT AL3, EVAPROD.PD_STM_PRP AL4, EVAPROD.PD_LFD_BMN AL5 WHERE (AL5.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL2.PRD_SPC_SID=AL1.PRD_SPC_SID AND AL3.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL1.PRP_DATE_ID=AL4.PRP_DATE_ID)  AND (AL1.RUNID_TYPE LIKE '{product}' AND AL1.RUN_DATE BETWEEN to_date '{fromTestDateString}', 'dd-mon-yyyy hh24:mi:ss') AND to_date('{toTestDateString}', 'dd-mon-yyyy hh24:mi:ss'))";


        }

        public static string QueryFails(DateTime FromDate, DateTime ToDate)
        {
            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 7)) throw new ArgumentException("Solo se permite maximo 7 dias");

            //DD-MM-yyyy HH24:MI:SS
            //string Timing1 = "04-05-2022 12:00:00";
            //string Timing2 = "04-05-2022 12:10:00";
            string fromDate = FromDate.ToString("dd-MM-yyyy HH:mm:ss");
            string toDate = ToDate.ToString("dd-MM-yyyy HH:mm:ss");

            return $"SELECT DISTINCT AL1.RUNID, AL1.RUN_STATE, AL1.RUN_DATE, AL6.MRK_WERT, AL2.BMT_NAME, AL6.MRK_NUM, AL4.PDK_MATERIAL, AL1.RUNID_TYPE, AL8.MRK_TXT, AL9.ASLP_KOM, AL10.TXT_INFO FROM EVAPROD.PD_LFD_RUN AL1, EVACOMP.PD_LFD_BMN AL2, EVAPROD.PD_LFD_MAT AL4, EVAPROD.PD_LFD_MED2 AL6, ETL_EVA_STG.V_PD_LFD_MXT2 AL8, EVAREAD.V_PD_LFD_MED AL9, EVAPROD.RUN_TED AL10 WHERE (AL1.BMT_DAT_ID=AL2.BMT_DAT_ID AND AL4.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL1.RUN_KEY_PRT=AL6.RUN_KEY_PRT AND AL1.RUN_SEQ_KEY=AL6.RUN_SEQ_KEY AND AL8.RUN_DATE=AL1.RUN_DATE AND AL8.CAP_DATE=AL1.CAP_DATE AND AL8.RUN_KEY_PRT=AL1.RUN_KEY_PRT AND AL8.RUN_KEY_PRT=AL6.RUN_KEY_PRT AND AL8.RUN_SEQ_KEY=AL1.RUN_SEQ_KEY AND AL8.RUN_SEQ_KEY=AL6.RUN_SEQ_KEY AND AL8.MRK_NUM=AL6.MRK_NUM AND AL9.RUNID=AL1.RUNID AND AL9.RUNID_TYPE=AL1.RUNID_TYPE AND AL9.RUN_DATE=AL1.RUN_DATE AND AL9.MRK_NUM=AL8.MRK_NUM AND AL10.RUNID_TYPE=AL1.RUNID_TYPE AND AL10.RUNID_TYPE=AL9.RUNID_TYPE AND AL10.RUNID=AL1.RUNID AND AL10.RUNID=AL9.RUNID AND AL10.RUN_DATE=AL1.RUN_DATE AND AL10.RUN_DATE=AL8.RUN_DATE AND AL10.RUN_DATE=AL9.RUN_DATE AND AL10.RUN_STATE=AL1.RUN_STATE AND AL10.PRD_PAG_SID=AL1.PRD_PAG_SID AND AL10.PRP_DATE_ID=AL1.PRP_DATE_ID AND AL10.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL10.BMT_DAT_ID=AL2.BMT_DAT_ID AND AL10.PRD_SPC_SID=AL1.PRD_SPC_SID AND AL10.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL10.PRD_MAT_SID=AL4.PRD_MAT_SID AND AL10.SFT_DATE=AL1.SFT_DATE)  AND (AL1.RUN_DATE BETWEEN TO_DATE('" + fromDate + "', 'DD-MM-yyyy HH24:MI:SS') AND TO_DATE('" + toDate + "', 'DD-MM-yyyy HH24:MI:SS') AND AL1.RUN_STATE = 'F' AND(AL2.BMT_NAME LIKE '%ANA%' OR AL2.BMT_NAME LIKE 'REP%' OR AL2.BMT_NAME LIKE '%SCRAP%') AND AL6.MRK_EIN_GUT = 'F' AND(NOT AL9.ASLP_KOM IS NULL))";
        }

        public static string QueryForProducedIDR(string FamilyICTs, string fromDate, string toDate)
        {
            return $"SELECT COUNT(*) AS PRODUCED FROM (SELECT DISTINCT RUNID FROM (SELECT AL1.RUNID, AL1.RUN_STATE, AL1.RUN_DATE, AL4.BMT_NAME, AL2.PDK_MATERIAL, AL3.PRP_VAR, AL1.RUNID_TYPE, AL5.PDK_AUFTR FROM EVAPROD.PD_LFD_RUN AL1, EVAPROD.PD_LFD_MAT AL2, EVAPROD.PD_STM_PRP AL3, EVACOMP.PD_LFD_BMN AL4, EVAPROD.PD_LFD_AUF AL5 WHERE (AL2.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL4.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL1.PRP_DATE_ID=AL3.PRP_DATE_ID AND AL5.PRD_SPC_SID=AL1.PRD_SPC_SID)  AND (AL3.PRP_VAR LIKE '%ICT%' AND AL1.RUN_DATE BETWEEN TO_DATE('{fromDate}', 'DD-MM-yyyy HH24:MI:SS') AND TO_DATE('{toDate}', 'DD-MM-yyyy HH24:MI:SS') AND AL2.PDK_MATERIAL LIKE 'A%' AND AL1.RUN_STATE='P' AND (NOT AL5.PDK_AUFTR LIKE '%gold%') AND AL4.BMT_NAME IN ({FamilyICTs}))))";


        }

        public static string QueryForDataFPY(string Producto, DateTime FromDate, DateTime ToDate)
        {
            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) throw new ArgumentException("Solo se permite maximo 7 dias");

            //DD-MM-yyyy HH24:MI:SS
            //string Timing1 = "04-05-2022 12:00:00";
            //string Timing2 = "04-05-2022 12:10:00";
            string fromDate = FromDate.ToString("dd-MM-yyyy HH:mm:ss");
            string toDate = ToDate.ToString("dd-MM-yyyy HH:mm:ss");

            return $"SELECT AL1.RUNID, AL2.PDK_AUFTR, AL1.RUN_STATE, AL1.RUN_DATE, AL3.PDK_MATERIAL, AL5.BMT_NAME, AL4.PRP_VAR, AL1.RUNID_TYPE FROM EVAPROD.PD_LFD_RUN AL1, EVAPROD.PD_LFD_AUF AL2, EVAPROD.PD_LFD_MAT AL3, EVAPROD.PD_STM_PRP AL4, EVAPROD.PD_LFD_BMN AL5 WHERE (AL5.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL2.PRD_SPC_SID=AL1.PRD_SPC_SID AND AL3.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL1.PRP_DATE_ID=AL4.PRP_DATE_ID) AND (AL1.RUNID_TYPE LIKE '{Producto}' AND AL1.RUN_DATE BETWEEN to_date('{fromDate}', 'dd-mon-yyyy hh24:mi:ss') AND to_date('{toDate}', 'dd-mon-yyyy hh24:mi:ss'))";
           //return $"SELECT AL1.RUNID, AL2.PDK_AUFTR, AL1.RUN_STATE, AL1.RUN_DATE, AL3.PDK_MATERIAL, AL5.BMT_NAME, AL4.PRP_VAR, AL1.RUNID_TYPE FROM EVAPROD.PD_LFD_RUN AL1, EVACOMP.PD_LFD_AUF AL2, EVAPROD.PD_LFD_MAT AL3, EVAPROD.PD_STM_PRP AL4, EVAPROD.PD_LFD_BMN AL5 WHERE (AL2.PRD_SPC_SID=AL1.PRD_SPC_SID AND AL3.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL4.PRP_DATE_ID=AL1.PRP_DATE_ID AND AL5.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL5.ZUGRIFF=AL4.ZUGRIFF) AND (AL1.RUNID_TYPE LIKE 'LDM2_FIN' AND AL1.RUN_DATE BETWEEN '14-02-2023 07:00:00' AND '15-02-2023 07:00:00')";
        }

        public static string QueryForFailFPY(string Producto, DateTime FromDate, DateTime ToDate)
        {
            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 7)) throw new ArgumentException("Solo se permite maximo 7 dias");

            //DD-MM-yyyy HH24:MI:SS
            //string Timing1 = "04-05-2022 12:00:00";
            //string Timing2 = "04-05-2022 12:10:00";
            string fromDate = FromDate.ToString("dd-MMM-yyyy HH:mm:ss");
            string toDate = ToDate.ToString("dd-MMM-yyyy HH:mm:ss");

            return $"SELECT AL1.RUNID, AL7.PDK_AUFTR, AL1.RUN_STATE, AL1.RUN_DATE, AL2.PDK_MATERIAL, AL6.BMT_NAME, AL3.PRP_VAR, AL1.RUNID_TYPE, AL4.MRK_NUM, AL5.MRK_BEZ, SUBSTR(AL4.MRK_WERT, 0, 8) AS Redondeado, AL4.MRK_EIN_GUT, AL5.MRK_USG, AL5.MRK_OSG FROM EVAPROD.PD_LFD_RUN AL1, EVAPROD.PD_LFD_MAT AL2, EVAPROD.PD_STM_PRP AL3, EVAPROD.PD_LFD_MED2 AL4, EVAPROD.PD_STM_MRK AL5, EVACOMP.PD_LFD_BMN AL6, EVAPROD.PD_LFD_AUF AL7 WHERE (AL2.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL6.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL1.PRP_DATE_ID=AL3.PRP_DATE_ID AND AL1.RUN_KEY_PRT=AL4.RUN_KEY_PRT AND AL1.RUN_SEQ_KEY=AL4.RUN_SEQ_KEY AND AL1.PRP_DATE_ID=AL5.PRP_DATE_ID AND AL5.PRP_DATE_ID=AL3.PRP_DATE_ID AND AL4.MRK_NUM=AL5.MRK_NUM AND AL7.PRD_SPC_SID=AL1.PRD_SPC_SID) AND (AL4.MRK_EIN_GUT='F' AND AL1.RUNID_TYPE LIKE '{Producto}' AND AL1.RUN_DATE BETWEEN to_date('{fromDate}', 'dd-mon-yyyy hh24:mi:ss') AND to_date('{toDate}', 'dd-mon-yyyy hh24:mi:ss') AND AL1.RUN_STATE='F')";

        }

    }
}
