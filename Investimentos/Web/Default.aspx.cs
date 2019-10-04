using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Drawing;
using Database;
using System.Globalization;

namespace Web
{
    public partial class Default : System.Web.UI.Page
    {
        public static string json_Months;
        public static string json_Datasets;

        protected void Page_Load(object sender, EventArgs e)
        {
            // (localdb)\MSSQLLocalDB

            using (var context = new MoneyEntities())
            {
                // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------
                // Query
                // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------

                var Historico = (from f in context.Fundo
                                 join h in context.Historico on f.FundoID equals h.FundoID
                                 orderby h.Data
                                 select new HistoricoView { Fundo = f.Categoria + " " + f.Nome, URL = f.URL, Data = h.Data, Rendimento = h.Rendimento }).ToList();

                var Meses = (from d in Historico.AsEnumerable() select new { d.Data, DataFormatada = d.Data.ToString("MMM") + "/" + d.Data.ToString("yy") }).Distinct();
                var MesesFormatados = (from m in Meses select m.DataFormatada);
                var Fundos = (from h in Historico orderby h.Fundo select h.Fundo).Distinct();


                // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------
                // Gráfico - http://www.chartjs.org/samples/latest/
                // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------

                json_Months = JsonConvert.SerializeObject(MesesFormatados);
                var Datasets = new List<ChartDataset>();
                var Cores = new List<string>(new string[] { "rgb(255, 99, 132)", "rgb(255, 159, 64)", "rgb(75, 192, 192)", "rgb(153, 102, 255)", "rgb(255, 205, 86)", "rgb(201, 203, 207)", "rgb(54, 162, 235)" });
                var iCores = 0;

                foreach (var fundo in Fundos)
                {
                    var Dataset = new ChartDataset();
                    Dataset.label = fundo;
                    Dataset.data = new List<decimal>();
                    Dataset.borderColor = Cores[iCores];
                    Dataset.backgroundColor = Cores[iCores];
                    iCores = iCores == Cores.Count - 1 ? 0 : iCores + 1;
                    foreach (var mes in Meses)
                    {
                        var valor = (from h in Historico where h.Fundo == fundo && h.Data == mes.Data select h.Rendimento).SingleOrDefault();
                        if (valor == default(decimal)) Dataset.data.Add(0);
                        else Dataset.data.Add(valor);
                    }
                    Datasets.Add(Dataset);
                }

                json_Datasets = JsonConvert.SerializeObject(Datasets);


                // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------
                // Tabela
                // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------

                // Header
                litTable.Text = "<thead>";
                litTable.Text += "<tr>";
                litTable.Text += "<th>Fundo</th>";
                litTable.Text += "<th>Total 1 ano</th>";
                litTable.Text += "<th>Total 2 anos</th>";
                litTable.Text += "<th>Média Total</th>";
                litTable.Text += "<th>Média 2</th>";
                litTable.Text += "<th>Média 3</th>";
                litTable.Text += "<th>Média 6</th>";
                litTable.Text += "<th>Média 12</th>";
                litTable.Text += "<th>Média 18</th>";
                litTable.Text += "<th>Média 24</th>";
                litTable.Text += "<th>Média 30</th>";
                litTable.Text += "<th>Média 36</th>";
                litTable.Text += "<th>Média 42</th>";
                litTable.Text += "<th>Média 48</th>";
                litTable.Text += "<th>Média 54</th>";
                litTable.Text += "<th>Média 60</th>";
                litTable.Text += "</tr>";
                litTable.Text += "</thead>";

                // Rows
                litTable.Text += "<tbody>";

                foreach (var fundo in Fundos)
                {
                    var url = (from a in Historico where a.Fundo == fundo select a.URL).FirstOrDefault();

                    litTable.Text += "<tr>";
                    litTable.Text += "<td><a href=\"" + url + "\" target=\"_blank\">" + fundo + "</a></td>";

                    var data_min = (from A in Historico where A.Fundo == fundo select A.Data).Min();

                    // Totais
                    litTable.Text += GetTotal(fundo, Historico, data_min, 1);
                    litTable.Text += GetTotal(fundo, Historico, data_min, 2);

                    // Médias
                    litTable.Text += GetMedia(fundo, Historico, data_min, 9999);
                    litTable.Text += GetMedia(fundo, Historico, data_min, 2);
                    litTable.Text += GetMedia(fundo, Historico, data_min, 3);
                    litTable.Text += GetMedia(fundo, Historico, data_min, 6);
                    litTable.Text += GetMedia(fundo, Historico, data_min, 12);
                    litTable.Text += GetMedia(fundo, Historico, data_min, 18);
                    litTable.Text += GetMedia(fundo, Historico, data_min, 24);
                    litTable.Text += GetMedia(fundo, Historico, data_min, 30);
                    litTable.Text += GetMedia(fundo, Historico, data_min, 36);
                    litTable.Text += GetMedia(fundo, Historico, data_min, 42);
                    litTable.Text += GetMedia(fundo, Historico, data_min, 48);
                    litTable.Text += GetMedia(fundo, Historico, data_min, 54);
                    litTable.Text += GetMedia(fundo, Historico, data_min, 60);

                    litTable.Text += "</tr>";
                }

                litTable.Text += "</tbody>";
            }
        }

        private string GetTotal(string fundo, List<HistoricoView> Historico, DateTime data_min, int anos)
        {
            var data_max = data_min.AddYears(anos);
            var group_By = (from h in Historico where h.Data <= data_max group h by h.Fundo into g let f = g.FirstOrDefault() select new { f.Fundo, Total = g.Sum(c => c.Rendimento) }).ToList();
            var max = (from A in group_By select A.Total).Max();
            var min = (from A in group_By select A.Total).Min();
            var total_fundo = (from A in Historico where A.Fundo == fundo && A.Data <= data_max select A.Rendimento).Sum();
            decimal percent = 100;
            if (max - min != 0) percent = (total_fundo - min) / (max - min);
            return "<td percent=\"" + percent.ToString("N2").Replace(",", ".") + "\">" + total_fundo.ToString("N2") + "</td>";
        }

        private string GetMedia(string fundo, List<HistoricoView> Historico, DateTime data_min, int meses)
        {
            var data_max = data_min.AddMonths(meses);
            var group_By = (from h in Historico where h.Data <= data_max group h by h.Fundo into g let f = g.FirstOrDefault() select new { f.Fundo, Total = g.Average(c => c.Rendimento) }).ToList();
            var max = (from A in group_By select A.Total).Max();
            var min = (from A in group_By select A.Total).Min();
            var total_fundo = (from A in Historico where A.Fundo == fundo && A.Data <= data_max select A.Rendimento).Average();
            decimal percent = 100;
            if (max - min != 0) percent = (total_fundo - min) / (max - min);
            return "<td percent=\"" + percent.ToString("N2").Replace(",", ".") + "\">" + total_fundo.ToString("N2") + "</td>";
        }

        private static readonly Random rand = new Random();

        private Color GetRandomColour()
        {
            return Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
        }

        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            int erros = 0;

            //ConsoleApp1.Fundos.ETF.Sync();
            ConsoleApp1.Fundos.Yubb.Sync(ref erros);

            if (erros > 0) lblErro.Text = erros.ToString() + " erros";
            else lblErro.Text = "Finalizado";

            Page_Load(null, null);
        }
    }

    public class ChartDataset
    {
        public string label;
        public bool fill = false;
        public List<decimal> data;
        public string backgroundColor;
        public string borderColor;
    }

    public class HistoricoView
    {
        public string Fundo;
        public string URL;
        public DateTime Data;
        public decimal Rendimento;

        //public HistoricoView(string _Fundo, DateTime _Data, decimal _Rendimento)
        //{
        //    Fundo = _Fundo;
        //    Data = _Data;
        //    Rendimento = _Rendimento;
        //}
    }


    //public class FundoMediaTotal
    //{
    //    public string fundo;
    //    public decimal media;
    //    public bool top5Media;
    //    public decimal media3;
    //    public bool top5Media3;
    //    public decimal media6;
    //    public bool top5Media6;
    //    public decimal media12;
    //    public bool top5Media12;
    //    public decimal media24;
    //    public bool top5Media24;
    //    public decimal total;
    //    public bool top5Total;
    //}
}