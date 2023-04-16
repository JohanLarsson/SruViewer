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

        if (sru.StartsWith("#BLANKETT", StringComparison.Ordinal))
        {
            SkipLine(ref sru);
            SkipPrefix(ref sru, "#IDENTITET");
            SkipLine(ref sru);
            SkipLineWithPrefix(ref sru, "#NAMN");
        }

        var quantity = int.Parse(ValueSpan(ref sru, "#UPPGIFT 31n0 "));
        var symbol = ValueSpan(ref sru, "#UPPGIFT 31n1 ").Trim().ToString();
        var proceeds = int.Parse(ValueSpan(ref sru, "#UPPGIFT 31n2 "));
        var basis = int.Parse(ValueSpan(ref sru, "#UPPGIFT 31n3 "));
        var win = int.Parse(ValueSpan(ref sru, "#UPPGIFT 31n4 "));
        var loss = int.Parse(ValueSpan(ref sru, "#UPPGIFT 31n5 "));

        SkipLineWithPrefix(ref sru, "#UPPGIFT 3300");
        SkipLineWithPrefix(ref sru, "#UPPGIFT 3301");
        SkipLineWithPrefix(ref sru, "#UPPGIFT 3304");
        SkipLineWithPrefix(ref sru, "#UPPGIFT 3305");
        SkipLineWithPrefix(ref sru, "#UPPGIFT 7014");
        SkipLineWithPrefix(ref sru, "#BLANKETTSLUT");
        SkipLineWithPrefix(ref sru, "#FIL_SLUT");

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

        static void SkipPrefix(ref ReadOnlySpan<char> sru, string prefix)
        {
            if (sru.StartsWith(prefix, StringComparison.Ordinal))
            {
                sru = sru[prefix.Length..];
                return;
            }

            throw new FormatException($"expected {prefix}");
        }

        static void SkipLineWithPrefix(ref ReadOnlySpan<char> sru, string prefix)
        {
            if (sru.StartsWith(prefix, StringComparison.Ordinal))
            {
                SkipLine(ref sru);
            }
        }

        static ReadOnlySpan<char> ValueSpan(ref ReadOnlySpan<char> sru, string prefix)
        {
            if (sru.Length < prefix.Length)
            {
                throw new FormatException("sru too short to contain field");
            }

            for (var i = 0; i < prefix.Length; i++)
            {
                if (i != prefix.Length - 3 &&
                    sru[i] != prefix[i])
                {
                    throw new FormatException($"Expected field {prefix}");
                }
            }

            sru = sru[prefix.Length..];
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
