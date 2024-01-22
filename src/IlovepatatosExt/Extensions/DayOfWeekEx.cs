using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class DayOfWeekEx
{
    public static readonly int AmountDays = Enum.GetNames(typeof(DayOfWeek)).Length;

    public static DayOfWeek NextDay(this DayOfWeek day)
    {
        int value = (int)day;
        int nextValue = (value + 1) % AmountDays;
        return (DayOfWeek)nextValue;
    }
}