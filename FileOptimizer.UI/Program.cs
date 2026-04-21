namespace FileOptimizer.UI;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        AppConsole.Initialize(args);
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}
