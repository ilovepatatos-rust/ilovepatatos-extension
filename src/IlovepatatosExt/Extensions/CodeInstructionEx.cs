using System.Reflection.Emit;
using Harmony;
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