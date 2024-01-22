using JetBrains.Annotations;
using Oxide.Ext.UiFramework.Builder.Cached;
using Oxide.Ext.UiFramework.Builder.UI;

namespace Oxide.Ext.IlovepatatosExt.UI;

[UsedImplicitly]
public abstract class Page : IPage
{
    public int PageNumber { get; set; }
    public CachedUiBuilder UserInterface { get; set; }

    public abstract UiBuilder CreateUserInterface();
}