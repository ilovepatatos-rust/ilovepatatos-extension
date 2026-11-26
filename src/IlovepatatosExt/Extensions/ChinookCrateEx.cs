using JetBrains.Annotations;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ChinookCrateEx
{
    private static readonly Dictionary<NetworkableId, int> s_chinookCrateInstanceIdToDelay = new();

    /// <summary>
    /// <para>Returns the remaining time in seconds until the crate unlocks.</para>
    /// </summary>
    [MustUseReturnValue]
    public static int Remaining(this HackableLockedCrate crate)
    {
        int objective = GetObjectiveTimer(crate);
        int remaining = Mathf.RoundToInt(objective - crate.hackSeconds);
        return Math.Max(0, remaining - 1);
    }

    public static void StartHacking(this HackableLockedCrate crate, int seconds)
    {
        s_chinookCrateInstanceIdToDelay[crate.net.ID] = seconds;
        Interface.CallHook("_OnCrateHack", crate);

        crate.BroadcastEntityMessage("HackingStarted", layerMask: 257);
        using (BaseEntity.FlagsUpdateScope flags = crate.StartSetFlags(BaseEntity.FlagsUpdateMode.SendNetworkUpdate))
            flags.Set(HackableLockedCrate.Flag_Hacking, true);

        crate.CancelInvoke(crate.HackProgress);
        crate.CancelInvoke(crate.UpdateChinookCrateCountdown);
        crate.InvokeRepeating(crate.UpdateChinookCrateCountdown, 1f, 1f);

        RpcTarget target = RpcTarget.NetworkGroup("UpdateHackProgress", crate);
        crate.ClientRPC(target, 0, seconds);

        crate.RefreshDecay();
    }

    private static int GetObjectiveTimer(NetworkableId id)
    {
        return s_chinookCrateInstanceIdToDelay.TryGetValue(id, out int seconds) ? seconds : Mathf.RoundToInt(HackableLockedCrate.requiredHackSeconds);
    }

    private static int GetObjectiveTimer(HackableLockedCrate crate)
    {
        return crate.net == null ? Mathf.RoundToInt(HackableLockedCrate.requiredHackSeconds) : GetObjectiveTimer(crate.net.ID);
    }

    private static void UpdateChinookCrateCountdown(this HackableLockedCrate crate)
    {
        crate.hackSeconds++;
        int objective = GetObjectiveTimer(crate);

        if (crate.hackSeconds > objective)
        {
            Interface.CallHook("_OnCrateHackEnd", crate);

            crate.RefreshDecay();
            using (BaseEntity.FlagsUpdateScope flags = crate.StartSetFlags(BaseEntity.FlagsUpdateMode.SendNetworkUpdate))
            {
                flags.Set(HackableLockedCrate.Flag_FullyHacked, true);
                flags.Set(HackableLockedCrate.Flag_Hacking, false);
            }

            crate.isLootable = true;

            crate.CancelInvoke(crate.UpdateChinookCrateCountdown);
            s_chinookCrateInstanceIdToDelay.Remove(crate.net.ID);
        }

        RpcTarget target = RpcTarget.NetworkGroup("UpdateHackProgress", crate);
        crate.ClientRPC(target, (int)crate.hackSeconds, objective);
    }
}