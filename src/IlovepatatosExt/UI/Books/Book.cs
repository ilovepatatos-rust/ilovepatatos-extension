using JetBrains.Annotations;
using Oxide.Ext.UiFramework.Builder;
using Oxide.Ext.UiFramework.Builder.UI;
using Oxide.Ext.UiFramework.Plugins;

namespace Oxide.Ext.IlovepatatosExt.UI;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class Book
{
    protected readonly List<IPage> Pages = new();
    protected readonly Dictionary<ulong, BasePlayer> ActiveReaders = new();
    protected readonly Dictionary<ulong, IPage> ReaderToPage = new();

    public PagesUserInterface PagesUserInterface { get; set; }

    public int AmountPages => Pages.Count;
    public abstract string PanelName { get; }

    public virtual bool Open(IUiFrameworkPlugin uiPlugin, BasePlayer player, int page = 0)
    {
        if (player == null)
            throw new ArgumentNullException(nameof(player));

        if (!Pages.ContainsIndex(page))
            return false;

        var content = Pages[page];

        ulong userID = player.userID;
        ActiveReaders[userID] = player;
        ReaderToPage[userID] = content;

        content.UserInterface.AddUi(player);

        var builder = CreatePagesCountInterface(uiPlugin, page);
        builder?.AddUi(player);

        return true;
    }

    public virtual bool Open(IUiFrameworkPlugin uiPlugin, IEnumerable<BasePlayer> players, int page = 0)
    {
        if (players == null)
            throw new ArgumentNullException(nameof(players));

        if (!Pages.ContainsIndex(page))
            return false;

        IPage content = Pages[page];

        foreach (BasePlayer player in players)
        {
            if (player == null)
                continue;

            ulong userID = player.userID;
            ActiveReaders[userID] = player;
            ReaderToPage[userID] = content;
        }

        content.UserInterface.AddUi(players);

        var builder = CreatePagesCountInterface(uiPlugin, page);
        builder?.AddUi(players);

        return true;
    }

    public virtual void Close()
    {
        BaseBuilder.DestroyUi(PanelName);
    }

    public virtual void Close(BasePlayer player)
    {
        if (player == null)
            throw new ArgumentNullException(nameof(player));

        ulong userID = player.userID;
        ActiveReaders.Remove(userID);
        ReaderToPage.Remove(userID);

        BaseBuilder.DestroyUi(player, PanelName);
    }

    public virtual void Close(IEnumerable<BasePlayer> players)
    {
        if (players == null)
            throw new ArgumentNullException(nameof(players));

        foreach (BasePlayer player in players)
        {
            if (player == null)
                continue;

            ulong userID = player.userID;
            ActiveReaders.Remove(userID);
            ReaderToPage.Remove(userID);
        }

        BaseBuilderUtility.DestroyUi(players, PanelName);
    }

    public IEnumerable<BasePlayer> GetActiveReadersAtPage(int page)
    {
        foreach (var userToPage in ReaderToPage)
        {
            if (userToPage.Value.PageNumber != page)
                continue;

            if (ActiveReaders.TryGetValue(userToPage.Key, out BasePlayer player))
                yield return player;
        }
    }

    protected virtual void Write(IPage page, bool createPageUserInterface = true)
    {
        if (page == null)
            throw new ArgumentNullException(nameof(page));

        if (createPageUserInterface)
            page.UserInterface = page.CreateUserInterface().ToCachedBuilder();

        page.PageNumber = Pages.Count;
        Pages.Add(page);
    }

    protected virtual UiBuilder CreatePagesCountInterface(IUiFrameworkPlugin uiPlugin, int page)
    {
        return PagesUserInterface?.CreatePageInterface(uiPlugin, page);
    }
}