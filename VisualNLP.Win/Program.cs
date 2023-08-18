using System.Configuration;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Win.ApplicationBuilder;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraEditors;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Utils;
using System.Reflection;
using Python.Runtime;
using Python.Included;
using System.Diagnostics;

namespace VisualNLP.Win;

static class Program
{
    private static bool ContainsArgument(string[] args, string argument)
    {
        return args.Any(arg => arg.TrimStart('/').TrimStart('-').ToLower() == argument.ToLower());
    }
    static async System.Threading.Tasks.Task LoadPython()
    {
        Installer.LogMessage += Installer_LogMessage;
        await Installer.SetupPython();
        if (!Installer.IsPipInstalled())
        {
            await Installer.TryInstallPip();
        }
        if (!Installer.IsModuleInstalled("spacy"))
        {
            await Installer.PipInstallModule("spacy");
        }
        if (!Installer.IsModuleInstalled("torch"))
        {
            await Installer.PipInstallModule("torch");
        }
        PythonEngine.Initialize();
        dynamic sys = Py.Import("sys");

        Console.WriteLine("Python version: " + sys.version);
    }

    private static void Installer_LogMessage(string obj)
    {
        Debug.WriteLine($"{DateTime.Now.ToString()}pip:" + obj);
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static int Main(string[] args)
    {
        LoadPython().Wait();



        if (ContainsArgument(args, "help") || ContainsArgument(args, "h"))
        {
            Console.WriteLine("Updates the database when its version does not match the application's version.");
            Console.WriteLine();
            Console.WriteLine($"    {Assembly.GetExecutingAssembly().GetName().Name}.exe --updateDatabase [--forceUpdate --silent]");
            Console.WriteLine();
            Console.WriteLine("--forceUpdate - Marks that the database must be updated whether its version matches the application's version or not.");
            Console.WriteLine("--silent - Marks that database update proceeds automatically and does not require any interaction with the user.");
            Console.WriteLine();
            Console.WriteLine($"Exit codes: 0 - {DBUpdaterStatus.UpdateCompleted}");
            Console.WriteLine($"            1 - {DBUpdaterStatus.UpdateError}");
            Console.WriteLine($"            2 - {DBUpdaterStatus.UpdateNotNeeded}");
            return 0;
        }
        DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.Latest;
#if EASYTEST
        DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
        WindowsFormsSettings.LoadApplicationSettings();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        DevExpress.Utils.ToolTipController.DefaultController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
        if (Tracing.GetFileLocationFromSettings() == DevExpress.Persistent.Base.FileLocation.CurrentUserApplicationDataFolder)
        {
            Tracing.LocalUserAppDataPath = Application.LocalUserAppDataPath;
        }
        Tracing.Initialize();

        string connectionString = null;
        if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null)
        {
            connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }
#if EASYTEST
        if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
            connectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
        }
#endif
        ArgumentNullException.ThrowIfNull(connectionString);
        var winApplication = ApplicationBuilder.BuildApplication(connectionString);

        if (ContainsArgument(args, "updateDatabase"))
        {
            using var dbUpdater = new WinDBUpdater(() => winApplication);
            return dbUpdater.Update(
                forceUpdate: ContainsArgument(args, "forceUpdate"),
                silent: ContainsArgument(args, "silent"));
        }

        try
        {
            winApplication.Setup();
            winApplication.Start();
        }
        catch (Exception e)
        {
            winApplication.StopSplash();
            winApplication.HandleException(e);
        }
        return 0;
    }
}
