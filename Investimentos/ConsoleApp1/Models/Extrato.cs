using System;
using System.Collections.Generic;

public class Fundo
{
    public string conta { get; set; }
    public DateTime data { get; set; }
    public string descricao { get; set; }
    public decimal? debito { get; set; }
    public decimal? credito { get; set; }
    public decimal? saldo { get; set; }
}

public static class FundoHelper
{
    public static void AddFundo(ref List<Fundo> fundos, Fundo fundo)
    {
        if (!fundo.credito.HasValue && !fundo.debito.HasValue && !fundo.saldo.HasValue) return;
        fundos.Add(fundo);
    }

    public static string GetDescricao(string s)
    {
        s = s.Replace("|", " ");
        s = UI.RemoveLineEndings(s);
        s = UI.RemoveDoubleSpaces(s);
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

}

