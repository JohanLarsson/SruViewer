namespace SruViewer;

using System;

public record SruItem(double Quantity, string Symbol, int Basis, int Proceeds, int Win, int Loss)
{
    public static SruItem? Read(ref ReadOnlySpan<char> sru)
    {
        if (sru.StartsWith("#FIL_SLUT", StringComparison.Ordinal))
        {
            sru = ReadOnlySpan<char>.Empty;
            return null;
        }

        if (sru.IsEmpty)
        {
            return null;
        }

        SkipPrefix(ref sru, "#BLANKETT");
        SkipLine(ref sru);
        SkipPrefix(ref sru, "#IDENTITET");
        SkipLine(ref sru);
        SkipPrefix(ref sru, "#UPPGIFT 3100 ");
        var quantity = int.Parse(ValueSpan(ref sru));
        SkipPrefix(ref sru, "#UPPGIFT 3101 ");
        var symbol = ValueSpan(ref sru).Trim().ToString();
        SkipPrefix(ref sru, "#UPPGIFT 3102 ");
        var proceeds = int.Parse(ValueSpan(ref sru));
        SkipPrefix(ref sru, "#UPPGIFT 3103 ");
        var basis = int.Parse(ValueSpan(ref sru));
        SkipPrefix(ref sru, "#UPPGIFT 3104 ");
        var win = int.Parse(ValueSpan(ref sru));
        SkipPrefix(ref sru, "#UPPGIFT 3105 ");
        var loss = int.Parse(ValueSpan(ref sru));
        SkipPrefix(ref sru, "#BLANKETTSLUT");
        SkipLine(ref sru);

        return new SruItem(quantity, symbol, basis, proceeds, win, loss);

        static void SkipLine(ref ReadOnlySpan<char> sru)
        {
            var pos = 0;
            while (pos < sru.Length)
            {
                if (sru[pos] == '\r' &&
                    pos < sru.Length - 1 &&
                    sru[pos + 1] == '\n')
                {
                    sru = sru[(pos + 2)..];
                    return;
                }

                if (sru[pos] is '\r' or '\n')
                {
                    sru = sru[(pos + 1)..];
                    return;
                }

                pos++;
            }

            sru = ReadOnlySpan<char>.Empty;
        }

        static void SkipPrefix(ref ReadOnlySpan<char> sru, string text)
        {
            if (sru.StartsWith(text, StringComparison.Ordinal))
            {
                sru = sru[text.Length..];
                return;
            }

            throw new FormatException($"expected {text}");
        }

        static ReadOnlySpan<char> ValueSpan(ref ReadOnlySpan<char> sru)
        {
            for (var i = 0; i < sru.Length; i++)
            {
                if (sru[i] is '\r' or '\n')
                {
                    var span = sru[..i];
                    sru = sru[i..];
                    SkipLine(ref sru);
                    return span;
                }
            }

            var original = sru;
            sru = ReadOnlySpan<char>.Empty;
            return original;
        }
    }
}
