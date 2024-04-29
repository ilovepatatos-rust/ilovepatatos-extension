using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class CodeInstructionEx
{
    public static CodeInstruction WithLabels(this CodeInstruction instruction, params Label[] labels)
    {
        instruction.labels.AddRange(labels);
        return instruction;
    }
}