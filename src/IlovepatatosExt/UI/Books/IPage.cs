using Oxide.Ext.UiFramework.Builder.Cached;
using Oxide.Ext.UiFramework.Builder.UI;

namespace Oxide.Ext.IlovepatatosExt.UI;

public interface IPage
{
    int PageNumber { get; set; }

    CachedUiBuilder UserInterface { get; set; }
    UiBuilder CreateUserInterface();
}