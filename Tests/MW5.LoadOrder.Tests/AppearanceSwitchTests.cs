using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using MW5_Mod_Manager.Controls;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MW5.LoadOrder.Tests;

[TestClass]
[DoNotParallelize]
public sealed class AppearanceSwitchTests
{
    [STATestMethod]
    public void ExistingControlsCanSwitchDarkLightDark()
    {
        Application.EnableVisualStyles();
        AppearanceManager.Initialize();
        bool originalDarkMode = AppearanceManager.Current.DarkMode;

        try
        {
            using var form = new TestAppearanceForm();
            _ = form.Handle;
            ToolStripRenderer originalToolbarRenderer =
                form.Toolbar.Renderer;

            AppearanceManager.ApplyForTests(true);
            Assert.AreEqual(
                AppearanceManager.Current.Colors.Control,
                form.ActionButton.BackColor);
            Assert.AreEqual(FlatStyle.Flat, form.ActionButton.FlatStyle);
            Assert.AreEqual(
                AppearanceManager.Current.Colors.Surface,
                form.Input.BackColor);

            AppearanceManager.ApplyForTests(false);
            Assert.AreEqual(SystemColors.Control, form.BackColor);
            Assert.AreEqual(FlatStyle.Standard, form.ActionButton.FlatStyle);
            Assert.AreEqual(SystemColors.Window, form.Input.BackColor);
            Assert.AreSame(
                originalToolbarRenderer,
                form.Toolbar.Renderer);
            Assert.AreEqual(
                ToolStripRenderMode.Custom,
                form.Toolbar.RenderMode);

            AppearanceManager.ApplyForTests(true);
            Assert.AreEqual(
                AppearanceManager.Current.Colors.Control,
                form.ActionButton.BackColor);
            Assert.AreEqual(FlatStyle.Flat, form.ActionButton.FlatStyle);
        }
        finally
        {
            AppearanceManager.ApplyForTests(originalDarkMode);
        }
    }

    [STATestMethod]
    public void MainFormThemeSwitchPreservesDockContents()
    {
        Application.EnableVisualStyles();
        AppearanceManager.Initialize();
        bool originalDarkMode = AppearanceManager.Current.DarkMode;

        try
        {
            using var form = new MainForm();
            form.ResetDockWindowLayout();
            AppearanceManager.Register(form);
            Assert.AreEqual(AutoScaleMode.Dpi, form.AutoScaleMode);

            object[] originalContents = form.dockPanel1.Contents
                .Cast<object>()
                .ToArray();
            Assert.AreEqual(3, originalContents.Length);

            AppearanceManager.ApplyForTests(!originalDarkMode);

            object[] switchedContents = form.dockPanel1.Contents
                .Cast<object>()
                .ToArray();
            Assert.AreEqual(originalContents.Length, switchedContents.Length);
            foreach (object content in originalContents)
                CollectionAssert.Contains(switchedContents, content);
            Assert.AreEqual(
                !originalDarkMode,
                DockModListForm.Instance
                    .olvColumnFreeSpaceDummy.IsVisible);
            Assert.AreSame(
                form.dockPanel1.Font,
                form.dockPanel1.Theme.Skin
                    .DockPaneStripSkin.TextFont);
            Assert.AreSame(
                form.dockPanel1.Font,
                form.dockPanel1.Theme.Skin
                    .AutoHideStripSkin.TextFont);
        }
        finally
        {
            AppearanceManager.ApplyForTests(originalDarkMode);
        }
    }

    private sealed class TestAppearanceForm : LocForm
    {
        public TestAppearanceForm()
        {
            BackColor = SystemColors.Control;
            ActionButton = new Button
            {
                BackColor = SystemColors.Control,
                FlatStyle = FlatStyle.Standard,
                Text = "Action"
            };
            Input = new TextBox
            {
                BackColor = SystemColors.Window
            };
            Toolbar = new ToolStrip
            {
                Renderer = new ToolStripProfessionalRenderer()
            };
            Controls.Add(ActionButton);
            Controls.Add(Input);
            Controls.Add(Toolbar);
        }

        public Button ActionButton { get; }

        public TextBox Input { get; }

        public ToolStrip Toolbar { get; }
    }
}
