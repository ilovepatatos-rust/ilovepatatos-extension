using JetBrains.Annotations;
using Oxide.Ext.UiFramework.Builder.UI;
using Oxide.Ext.UiFramework.Colors;
using Oxide.Ext.UiFramework.Enums;
using Oxide.Ext.UiFramework.Offsets;
using Oxide.Ext.UiFramework.Positions;
using Oxide.Ext.UiFramework.UiElements;

namespace Oxide.Ext.IlovepatatosExt.UI;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PagesUserInterface
{
    public int AmountPages;

    public string ParentPanelName = UiLayer.Overlay.ToString();
    public string PanelName = "pages (Panel)";

    public UiPosition Anchors = UiPosition.MiddleMiddle;
    public UiOffset Offset = new(-37.073f, -11.665f, 37.073f, 11.665f);

    public int TextSize = 13;
    public string TextFormat = "{0}/{1}";
    public UiFont TextFont = UiFont.RobotoCondensedRegular;
    public UiColor TextColor = UiColor.White;

    public PageButton PreviousButton { get; private set; } = PageButton.Previous;
    public PageButton NextButton { get; private set; } = PageButton.Next;

    public UiBuilder CreatePageInterface(int currentPage)
    {
        var builder = UiBuilder.Create(Anchors, Offset, PanelName, ParentPanelName);

        string text = string.Format(TextFormat, currentPage + 1, AmountPages);
        CreateTextInterface(builder, builder.Root, text);

        bool isPreviousActive = currentPage > 0;
        PreviousButton.CreateUserInterface(builder, builder.Root, isPreviousActive, currentPage - 1);

        bool isNextActive = currentPage < AmountPages - 1;
        NextButton.CreateUserInterface(builder, builder.Root, isNextActive, currentPage + 1);

        return builder;
    }

    private void CreateTextInterface(UiBuilder builder, UiReference parent, string text)
    {
        UiPosition anchors = UiPosition.Full;
        UiOffset offset = UiOffset.None;

        builder.SetCurrentFont(TextFont);
        builder.Label(parent, anchors, offset, text, TextSize, TextColor);
    }
}