namespace VibeDisasm.Core;

public static class NumberFormatter
{
    public static string FormatNumber(this long number, bool includeSign = true)
    {
        var abs = Math.Abs(number);
        var sign = includeSign ? (number >= 0 ? "+" : "-") : "";
        string format;

        if (abs == 0)
        {
            format = "X2";
        }
        else if (abs <= 0xFF)
        {
            format = "X2";
        }
        else if (abs <= 0xFFFF)
        {
            format = "X4";
        }
        else
        {
            format = "X8";
        }

        return $"{sign}0x{abs.ToString(format)}";
    }

    public static string FormatNumber(this ulong number)
    {
        var abs = number;
        string format;

        if (abs == 0)
        {
            format = "X2";
        }
        else if (abs <= 0xFF)
        {
            format = "X2";
        }
        else if (abs <= 0xFFFF)
        {
            format = "X4";
        }
        else
        {
            format = "X8";
        }

        return $"+0x{abs.ToString(format)}";
    }
}
