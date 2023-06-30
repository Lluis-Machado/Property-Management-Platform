namespace AuthenticationAPI.Utils
{
    public static class AppShutdown
    {
        public static void Shutdown()
        {
            try
            {
                File.Copy("ocelot.json.bak", "ocelot.json", true);
            }
            catch { }
        }
    }
}
