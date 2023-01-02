namespace WeightContestService
{
    public static class GlobalData
    {
        public static string DatabaseConnectionIP { get; set; }
        public static string DatabaseConnectionPort { get; set; }
        public static string DatabaseName { get; set; }
        public static string DatabaseUserId { get; set; }
        public static string DatabaseUserPassword { get; set; }
        public static string ConnectionString => $"Server=.;Database={DatabaseConnectionIP},{DatabaseConnectionPort};Database={DatabaseName};User Id = {DatabaseUserId}; Password = {DatabaseUserPassword}; TrustServerCertificate=Yes";
        //localhost,1433;Database=WEIGHT_CONTEST_DB;User Id = sa; Password=P @sswOrd; TrustServerCertificate=Yes
        public static string ImageLocationPath { get; set; }
    }
}
