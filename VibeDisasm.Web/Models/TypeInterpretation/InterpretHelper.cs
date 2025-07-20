namespace VibeDisasm.Web.Models.TypeInterpretation;

public static class InterpretHelper
{
    public static string MemoryString(this Memory<byte> memory)
    {
        if (memory.IsEmpty)
            return "";

        var hex = string.Create(memory.Length * 6 - 2, memory, (span, mem) =>
        {
            for (int i = 0; i < mem.Length; i++)
            {
                var c1 = ToCharUpper(mem.Span[i] >> 4);
                var c2 = ToCharUpper(mem.Span[i]);
                span[i * 6] = '0';
                span[i * 6 + 1] = 'x';
                span[i * 6 + 2] = c1;
                span[i * 6 + 3] = c2;

                if (i != mem.Length - 1)
                {
                    span[i * 6 + 4] = ',';
                    span[i * 6 + 5] = ' ';
                }
            }
        });

        return hex;
    }

    // from microsoft's HexConverter
    public static char ToCharUpper(int value)
    {
        value &= 0xF;
        value += '0';

        if (value > '9')
        {
            value += ('A' - ('9' + 1));
        }

        return (char)value;
    }
}
