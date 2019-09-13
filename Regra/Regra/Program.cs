using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Regra
{
    class Program
    {
        public class Regra
        {
            public int Id { get; set; }

            public int IdRegraVariavel { get; set; }

            public int IdRegraCondicao { get; set; }

            public string ValorA
            {
                get;
                set;
            }

            public string Operacao
            {
                get;
                set;
            }

            public string ValorB
            {
                get;
                set;
            }

            public string TipoDoValor { get; set; }

            public bool RealizaCalculo { get; set; } = false;

            public Regra(int id, int idRegraVariavel, int idRegraCondicao, string operacao)
            {
                this.Id = id;
                this.IdRegraVariavel = idRegraVariavel;
                this.IdRegraCondicao = idRegraCondicao;
                this.Operacao = operacao;
            }

            public Regra(int id,
                       int idRegraVariavel,
                       string Operator,
                       string targetValue,
                       string TargetType,
                       bool realizaCalculo = false)
            {
                this.Id = id;
                this.ValorA = ValorA;
                this.Operacao = Operator;
                this.IdRegraVariavel = idRegraVariavel;
                this.TipoDoValor = TargetType;
                this.RealizaCalculo = realizaCalculo;
                this.ValorB = targetValue;
            }

            public Regra(int id,
                        string valorA,
                        string Operator,
                        int idRegraCondicao,
                        string tipoDoValor,
                        bool realizaCalculo = false)
            {
                this.Id = id;
                this.ValorA = valorA;
                this.Operacao = Operator;
                this.IdRegraCondicao = idRegraCondicao;
                this.TipoDoValor = tipoDoValor;
                this.RealizaCalculo = realizaCalculo;
            }

            public Regra(int id,
                        string valorA,
                        string operacao,
                        string valorB,
                        string tipoDoValor,
                        bool realizaCalculo = false)
            {
                this.Id = id;
                this.ValorA = valorA;
                this.Operacao = operacao;
                this.ValorB = valorB;
                this.TipoDoValor = tipoDoValor;
                this.RealizaCalculo = realizaCalculo;
            }
        }

        static void Main(string[] args)
        {
            List<Regra> regras = new List<Regra>
            {
                new Regra(1, "True", "Equal", "True", "System.Boolean"),
                new Regra(2, "A", "Equal", "B", "System.String"),
                new Regra(3, 1, 2, "Or"),
                new Regra(4, "C", "Equal", "D", "System.String"),
                new Regra(5, 3, 4, "Or"), //COMEÇAR AQUI                         
                new Regra(7, "2019", "Subtract","10", "System.Int32", true),
                new Regra(8, "2008", "GreaterThan", 7, "System.Int32"), // COMECAR AQUI
                new Regra(9, 8, 5, "And"), // COMECAR AQUI,
                new Regra(10, "10,10", "Add", "20,20", "System.Double", true),
                new Regra(11, 10, "Subtract", "2" , "System.Double", true), //COMECAR AQUI
                new Regra(12, "10", "NotEqual", "20", "System.Int32"),
                new Regra(13, 9, 12, "Or") // Todas as Regras
            };

            var bla = aplicaRegra(13, regras);

            var result = aplicaRegra(5, regras);
            var resultA = aplicaRegra(8, regras);
            var resultB = aplicaRegra(9, regras);
            //var resultC = aplicaRegra(11, regras);
            var resultD = aplicaRegra(12, regras);

            //CustomRule.Executar();
        }

        static dynamic aplicaRegra(int IdRegra, List<Regra> lstRegras)
        {
            try
            {
                Regra regra = lstRegras.FirstOrDefault(r => r.Id == IdRegra);
                dynamic resultadoA = null;
                dynamic resultadoB = null;

                if (regra == null)
                    return false;

                //REGRA VARIAVEL
                if (regra.IdRegraVariavel > 0)
                    resultadoA = aplicaRegra(regra.IdRegraVariavel, lstRegras);
                else
                    resultadoA = regra.ValorA;

                //REGRA CONDIÇÃO
                if (regra.IdRegraCondicao > 0)
                    resultadoB = aplicaRegra(regra.IdRegraCondicao, lstRegras);
                else
                    resultadoB = regra.ValorB;


                //VALIDAR OS TIPOS
                if (!string.IsNullOrEmpty(regra.TipoDoValor) && 
                    (
                        (!resultadoA.GetType().FullName.Equals(resultadoB.GetType().FullName)) ||
                        (!resultadoA.GetType().Equals(Type.GetType(regra.TipoDoValor)) || !resultadoB.GetType().Equals(Type.GetType(regra.TipoDoValor)))
                    )
                )
                {
                    resultadoA = converter(resultadoA, Type.GetType(regra.TipoDoValor));
                    resultadoB = converter(resultadoB, Type.GetType(regra.TipoDoValor));
                }

                //APLICA CALCULO
                if (regra.RealizaCalculo)
                {
                    return compilarRegra(resultadoA, resultadoB, regra.Operacao);
                }

                return blnCompilarRegra(resultadoA, resultadoB, regra.Operacao);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static dynamic converter(dynamic objeto, Type tipoConvercao)
        {
            return Convert.ChangeType(objeto, tipoConvercao);
        }

        static bool blnCompilarRegra<T>(T objetoA, T objetoB, string operador)
        {
            var parametro = Expression.Parameter(typeof(T));


            Expression expressao = criarExpressao<T>(objetoB, operador, parametro);

            // build a lambda function User->bool and compile it
            return Expression.Lambda<Func<T, bool>>(expressao, parametro).Compile()(objetoA);
        }

        static T compilarRegra<T>(T objetoA, T objetoB, string operador)
        {
            var parametro = Expression.Parameter(typeof(T));
            Expression expr = criarExpressao<T>(objetoB, operador, parametro);

            // build a lambda function User->bool and compile it
            return Expression.Lambda<Func<T, T>>(expr, parametro).Compile()(objetoA);
        }

        static Expression criarExpressao<T>(T objetoB, string operador, ParameterExpression parametro)
        {
            Expression esquerda = parametro;
            Type typPropriedade = typeof(T);

            ExpressionType operacao;

            if (ExpressionType.TryParse(operador, out operacao))
            {
                var direita = Expression.Constant(converter(objetoB, typPropriedade));
                // use a binary operation, e.g. 'Equal' -> 'u.Age == 15'
                return Expression.MakeBinary(operacao, esquerda, direita);
            }
            else
            {
                var metodo = typPropriedade.GetMethod(operador);
                var tyParametro = metodo.GetParameters()[0].ParameterType;
                var direita = Expression.Constant(converter(objetoB, tyParametro));
                // use a method call, e.g. 'Contains' -> 'u.Tags.Contains(some_tag)'
                return Expression.Call(esquerda, metodo, direita);
            }
        }
    }
}
