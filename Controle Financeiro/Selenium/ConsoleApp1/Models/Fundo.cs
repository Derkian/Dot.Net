using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Extrato
{
    public string conta { get; set; }
    public DateTime data { get; set; }
    public string descricao { get; set; }
    public decimal? debito { get; set; }
    public decimal? credito { get; set; }
    public decimal? saldo { get; set; }
}

public static class ExtratoHelper
{
    public static void AddExtrato(ref List<Extrato> extratos, Extrato extrato)
    {
        if (!extrato.credito.HasValue && !extrato.debito.HasValue && !extrato.saldo.HasValue) return;
        extratos.Add(extrato);
    }

    public static string GetDescricao(string s)
    {
        s = s.Replace("|", " ");
        s = RemoveLineEndings(s);
        s = RemoveDoubleSpaces(s);
        s = s.Trim();

        if (s.Length > 3 && s.Substring(s.Length - 3, 1) != " ")
        {
            var lastTwo = s.Substring(s.Length - 2);

            if (lastTwo == "SP" || lastTwo == "BR")
            {
                s = s.Substring(0, s.Length - 2) + " " + lastTwo;
            }
        }

        if (s == "Total da Fatura Anterior") s = null;

        return s;
    }

    private static string RemoveDoubleSpaces(string s)
    {
        RegexOptions options = RegexOptions.None;
        Regex regex = new Regex("[ ]{2,}", options);
        s = regex.Replace(s, " ");
        return s;
    }

    private static string RemoveLineEndings(string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            return value;
        }
        string lineSeparator = ((char)0x2028).ToString();
        string paragraphSeparator = ((char)0x2029).ToString();

        return value.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);
    }
}

