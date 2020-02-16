<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Web.Default" EnableViewState="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Investimentos</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="cache-control" content="max-age=0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
    <meta http-equiv="pragma" content="no-cache" />
    <link rel="stylesheet" href="css/style.css" />
    <script src="js/Chart.min.js"></script>
    <script src="js/Chart.bundle.min.js"></script>
    <script src="js/utils.js"></script>
    <script src="js/tablesort.min.js"></script>
    <script src="js/tablesort.number.min.js"></script>
    <script src="js/jquery-3.3.1.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">

        <div id="divCanvas">
            <canvas id="canvas"></canvas>
        </div>

        <table id="table-fundos">
            <asp:Literal ID="litTable" runat="server" />
        </table>

        <br />

        <asp:Button ID="btnAtualizar" runat="server" Text="Atualizar" OnClick="btnAtualizar_Click" />

        <br /><br />

        <asp:Label ID="lblErro" runat="server" />

        <script type="text/javascript">
            var config = {
                type: 'line',
                data: {
                    labels: <%=json_Months%>,
                    datasets: <%=json_Datasets%>
                },
                options: {
                    maintainAspectRatio: false,
                    responsive: true,
                    legend: { display: false, position: 'right' },
                    title: { display: true, text: 'Fundos' },
                    tooltips: { mode: 'index', intersect: false, },
                    hover: { mode: 'nearest', intersect: true },
                    scales: {
                        xAxes: [{ display: true, scaleLabel: { display: true, labelString: '' } }],
                        yAxes: [{ display: true, scaleLabel: { display: true, labelString: 'Variação (%)' } }]
                    }
                }
            };

            var percentColors = [
                { pct: 0.0, color: { r: 0xff, g: 0x00, b: 0 } },
                { pct: 0.5, color: { r: 0xff, g: 0xff, b: 0 } },
                { pct: 1.0, color: { r: 0x00, g: 0xff, b: 0 } }];

            var getColorForPercentage = function (pct) {
                for (var i = 1; i < percentColors.length - 1; i++) {
                    if (pct < percentColors[i].pct) {
                        break;
                    }
                }
                var lower = percentColors[i - 1];
                var upper = percentColors[i];
                var range = upper.pct - lower.pct;
                var rangePct = (pct - lower.pct) / range;
                var pctLower = 1 - rangePct;
                var pctUpper = rangePct;
                var color = {
                    r: Math.floor(lower.color.r * pctLower + upper.color.r * pctUpper),
                    g: Math.floor(lower.color.g * pctLower + upper.color.g * pctUpper),
                    b: Math.floor(lower.color.b * pctLower + upper.color.b * pctUpper)
                };
                return 'rgb(' + [color.r, color.g, color.b].join(',') + ')';
            }

            window.onload = function () {
                var ctx = document.getElementById('canvas').getContext('2d');
                window.myLine = new Chart(ctx, config);

                new Tablesort(document.getElementById('table-fundos'));

                $('[percent]').each(function (i, el) {
                    var valor = parseFloat($(this).attr('percent')).toFixed(2);
                    $(this).css("background-color", getColorForPercentage(valor));
                });

            };

        </script>

    </form>
</body>
</html>
