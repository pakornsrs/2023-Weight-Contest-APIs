namespace WeightContestService
{
    public static class dotEnvReader
    {
        public static void Load()
        {
            var descrip = "";
            var path = AppDomain.CurrentDomain.BaseDirectory + ".env";
            if (!File.Exists(path))
                return;

            foreach (var line in File.ReadAllLines(path))
            {
                var parts = line.Split(
                    '=',
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue;

                descrip = parts[0].Replace(" ",string.Empty);

                switch (descrip)
                {
                    case "DB_CONNECTION_IP":
                        GlobalData.DatabaseConnectionIP = parts[1];
                        break;
                    case "DB_CONNECTION_PORT":
                        GlobalData.DatabaseConnectionPort = parts[1];
                        break;
                    case "DB_NAME":
                        GlobalData.DatabaseName = parts[1];
                        break;
                    case "DB_USER_ID":
                        GlobalData.DatabaseUserId = parts[1];
                        break;
                    case "DB_USER_PASSWORD":
                        GlobalData.DatabaseUserPassword = parts[1];
                        break;
                    case "IMAGE_LOCATION_PATH":
                        GlobalData.ImageLocationPath = parts[1];
                        break;
                    default:
                        break;
                }

                //Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}
