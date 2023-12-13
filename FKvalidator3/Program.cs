namespace FKvalidator3_1
{
    internal static class Program
    {
        //public static cfgForApp cfgForApp=new ();       
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            ÑfgForApp cfg = new ÑfgForApp();
            WorkWithVks work = new(cfg);
            Application.Run(new Form1(work));
        }
    }
}