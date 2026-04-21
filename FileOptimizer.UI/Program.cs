namespace FileOptimizer.UI;

static class Program
{
    [STAThread]
    static void Main()
    {
        AppConsole.Initialize();
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}
