using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Regra
{
    class Program
    {
        public class Rule
        {
            public int Id { get; set; }

            public int IdRegraVariavel { get; set; }

            public int IdRegraCondicao { get; set; }

            public string MemberName
            {
                get;
                set;
            }

            public string Operator
            {
                get;
                set;
            }

            public string TargetValue
            {
                get;
                set;
            }

            public string TargetType { get; set; }

            public bool PerformsCalc { get; set; } = false;

            public Rule(int id, int idRegraVariavel, int idRegraCondicao, string Operator)
            {
                this.Id = id;
                this.IdRegraVariavel = idRegraVariavel;
                this.IdRegraCondicao = idRegraCondicao;
                this.Operator = Operator;
            }

            public Rule(int id, string MemberName, string Operator, int idRegraCondicao, string TargetType, bool performsCalc = false)
            {
                this.Id = id;
                this.MemberName = MemberName;
                this.Operator = Operator;
                this.IdRegraCondicao = idRegraCondicao;
                this.TargetType = TargetType;
                this.PerformsCalc = performsCalc;
            }

            public Rule(int id, string MemberName, string Operator, string TargetValue, string TargetType, bool performsCalc = false)
            {
                this.Id = id;
                this.MemberName = MemberName;
                this.Operator = Operator;
                this.TargetValue = TargetValue;
                this.TargetType = TargetType;
                this.PerformsCalc = performsCalc;
            }
        }

        static void Main(string[] args)
        {
            List<Rule> rules = new List<Rule>
            {
                new Rule(1, "A", "Equal", "A", "System.String"),
                new Rule(2, "A", "Equal", "B", "System.String"),
                new Rule(3, 1, 2, "Or"),
                new Rule(4, "C", "Equal", "D", "System.String"),
                new Rule(5, 3, 4, "Or"), //COMEÇAR AQUI                
                new Rule(7, "2019", "Subtract","10", "System.Int32", true),
                new Rule(8, "2008", "GreaterThan", 7, "System.Int32"), // COMECAR AQUI
                new Rule(9, 8, 5, "And"), // COMECAR AQUI,
                new Rule(10, "10", "Add", "20", "System.Int32", true),
                new Rule(11, "2", "Subtract", 10 , "System.Int32", true) //COMECAR AQUI
            };

            var result = AplicaRegra(5, rules);
            var resultA = AplicaRegra(8, rules);
            var resultB = AplicaRegra(9, rules);
            var resultB = AplicaRegra(11, rules);

        }

        static dynamic AplicaRegra(int IdRegra, List<Rule> lstRegras)
        {
            Rule rule = lstRegras.FirstOrDefault(r => r.Id == IdRegra);
            dynamic result = null;
            dynamic resultB = null;

            if (rule == null)
                return false;

            //REGRA VARIAVEL
            if (rule.IdRegraVariavel > 0)
                result = AplicaRegra(rule.IdRegraVariavel, lstRegras);
            else
                result = rule.MemberName;

            //REGRA CONDIÇÃO
            if (rule.IdRegraCondicao > 0)
                resultB = AplicaRegra(rule.IdRegraCondicao, lstRegras);
            else
                resultB = rule.TargetValue;


            //VALIDAR OS TIPOS
            if (!result.GetType().FullName.Equals(resultB.GetType().FullName))
            {
                result = Cast(result, Type.GetType(rule.TargetType));
                resultB = Cast(resultB, Type.GetType(rule.TargetType));
            }

            //APLICA CALCULO
            if (rule.PerformsCalc)
            {
                switch (rule.TargetType)
                {
                    case "System.Int32":
                        return intCompilarRegra(Convert.ToInt32(result), Convert.ToInt32(resultB), rule.Operator);
                    case "System.Double":
                        return dblCompilarRegra(Convert.ToDouble(result), Convert.ToDouble(resultB), rule.Operator);
                }
            }


            return blnCompilarRegra(result, resultB, rule.Operator);
        }

        public static dynamic Cast(dynamic obj, Type castTo)
        {
            return Convert.ChangeType(obj, castTo);
        }

        static bool blnCompilarRegra<T>(T objA, T objB, string Operator)
        {
            var paramUser = Expression.Parameter(typeof(T));
            Expression expr = CriarExpressao<T>(objA, objB, Operator, paramUser);

            // build a lambda function User->bool and compile it
            return Expression.Lambda<Func<T, bool>>(expr, paramUser).Compile()(objA);
        }

        static int intCompilarRegra<T>(T objA, T objB, string Operator)
        {
            var paramUser = Expression.Parameter(typeof(T));
            Expression expr = CriarExpressao<T>(objA, objB, Operator, paramUser);

            // build a lambda function User->bool and compile it
            return Expression.Lambda<Func<T, int>>(expr, paramUser).Compile()(objA);
        }

        static double dblCompilarRegra<T>(T objA, T objB, string Operator)
        {
            var paramUser = Expression.Parameter(typeof(T));
            Expression expr = CriarExpressao<T>(objA, objB, Operator, paramUser);

            // build a lambda function User->bool and compile it
            return Expression.Lambda<Func<T, double>>(expr, paramUser).Compile()(objA);
        }

        static Expression CriarExpressao<T>(T objA, T objB, string Operator, ParameterExpression param)
        {
            Expression left = param;
            Type tProp = typeof(T);

            ExpressionType tBinary;

            if (ExpressionType.TryParse(Operator, out tBinary))
            {
                var right = Expression.Constant(Convert.ChangeType(objB, tProp));
                // use a binary operation, e.g. 'Equal' -> 'u.Age == 15'
                return Expression.MakeBinary(tBinary, left, right);
            }
            else
            {
                var method = tProp.GetMethod(Operator);
                var tParam = method.GetParameters()[0].ParameterType;
                var right = Expression.Constant(Convert.ChangeType(objB, tParam));
                // use a method call, e.g. 'Contains' -> 'u.Tags.Contains(some_tag)'
                return Expression.Call(left, method, right);
            }
        }
    }
}
