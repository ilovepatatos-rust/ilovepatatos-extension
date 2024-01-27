using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class CoroutineUtility
{
    public static Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return ServerMgr.Instance.StartCoroutine(enumerator);
    }

    public static void StopCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
            ServerMgr.Instance.StopCoroutine(coroutine);
    }
}