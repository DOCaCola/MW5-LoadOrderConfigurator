using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace MW5.LoadOrder.Tests;

[TestClass]
public static class TestAssembly
{
    [AssemblyInitialize]
    public static void Initialize(TestContext _)
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
    }
}
