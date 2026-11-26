#if CARBON

using API.Assembly;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly]
public class CarbonExtension : ICarbonExtension
{
    void ICarbonAddon.Awake(EventArgs args) { }

    void ICarbonAddon.OnLoaded(EventArgs args)
    {
        JsonConvertersHandler.Initialize();
    }

    void ICarbonAddon.OnUnloaded(EventArgs args) { }
}

#endif