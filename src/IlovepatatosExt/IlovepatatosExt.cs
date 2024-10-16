using JetBrains.Annotations;
using Oxide.Core;
using Oxide.Core.Extensions;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly]
public class IlovepatatosExt : Extension
{
    private static readonly VersionNumber s_extensionVersion = new(1, 0, 0);

    public override string Name => "IlovepatatosExt";
    public override string Author => "Ilovepatatos";
    public override VersionNumber Version => s_extensionVersion;

    public override bool SupportsReloading => true;

    public IlovepatatosExt(ExtensionManager manager) : base(manager) { }

    public override void OnModLoad()
    {
        base.OnModLoad();
        JsonConvertersHandler.Initialize();
    }
}