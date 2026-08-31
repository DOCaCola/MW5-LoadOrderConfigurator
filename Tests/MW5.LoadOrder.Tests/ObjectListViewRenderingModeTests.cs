using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;

namespace MW5.LoadOrder.Tests;

[TestClass]
public sealed class ObjectListViewRenderingModeTests
{
    [STATestMethod]
    public void SearchHighlightUsesOwnerDrawOnlyWhileHighlightingIsActive()
    {
        using var form = new DockModListForm();

        Assert.IsFalse(form.modObjectListView.OwnerDraw);
        Assert.IsNull(form.modObjectListView.AlwaysGroupByColumn);
        Assert.IsFalse(form.modObjectListView.ShowGroups);
        Assert.IsTrue(form.modObjectListView.UseSmoothPixelScrolling);

        form.SetSearchHighlightText("Yet");
        Assert.IsTrue(form.modObjectListView.OwnerDraw);

        form.SetSearchHighlightText(" ");
        Assert.IsFalse(form.modObjectListView.OwnerDraw);
    }
}
