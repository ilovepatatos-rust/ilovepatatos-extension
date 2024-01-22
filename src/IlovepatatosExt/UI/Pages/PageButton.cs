using JetBrains.Annotations;
using Oxide.Ext.UiFramework.Builder;
using Oxide.Ext.UiFramework.Colors;
using Oxide.Ext.UiFramework.Offsets;
using Oxide.Ext.UiFramework.Positions;
using Oxide.Ext.UiFramework.UiElements;

namespace Oxide.Ext.IlovepatatosExt.UI;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PageButton
{
    public UiPosition Anchors = UiPosition.MiddleLeft;
    public UiOffset Offset = new(-10, -10, 10, 10);

    public string Command = "pages.previous";

    public bool UseImageFileStorage;
    public string ActiveImageUrlOrPng = "";
    public string InactiveImageUrlOrPng = "";

    public void CreateUserInterface(BaseUiBuilder builder, UiReference parent, bool isActive, int page)
    {
        if (UseImageFileStorage)
        {
            if (isActive)
            {
                string command = string.Format(Command, page);
                builder.ImageFileStorageButton(parent, Anchors, Offset, UiColor.Clear, ActiveImageUrlOrPng, command);
            }
            else
            {
                builder.ImageFileStorage(parent, Anchors, Offset, InactiveImageUrlOrPng);
            }
        }
        else
        {
            if (isActive)
            {
                string command = string.Format(Command, page);
                builder.WebImageButton(parent, Anchors, Offset, UiColor.Clear, ActiveImageUrlOrPng, command);
            }
            else
            {
                builder.WebImage(parent, Anchors, Offset, InactiveImageUrlOrPng);
            }
        }
    }

    public static PageButton Previous => new()
    {
        Anchors = UiPosition.MiddleLeft,
        ActiveImageUrlOrPng = "https://i.imgur.com/5PU6sBz.png",
        InactiveImageUrlOrPng = "https://i.imgur.com/EgxWgQr.png"
    };

    public static PageButton Next => new()
    {
        Anchors = UiPosition.MiddleRight,
        ActiveImageUrlOrPng = "https://i.imgur.com/Ml579N1.png",
        InactiveImageUrlOrPng = "https://i.imgur.com/LuQcmTn.png"
    };
}