using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuleSharp;
using RuleSharp.RuleEngine;
using ThirdAssembly;
using System.Reflection;

namespace TestConsoleApp
{
    class Program
    {


        static void Main(string[] args)
        {
            var designation = new Designation()
            {
                Id = 1,
                Name = "ManaGer",
            };

            var employee = new Employee
            {
                Id = 1,
                Name = "Nejimon",
                Designation = designation,
                LstDesignation = new List<Designation>() { new Designation() { Id = 1, Name = "Fulano" } }
            };

            //employee.Teste.Find(r => r.Id == 1)
            
            //new Func<TestConsoleApp.Employee, bool>((input) => { return input.Numeros.Contains(10) });

            var thirdAssemblyClass = new ClassFromAnotherAssembly
            {
                Id = 1
            };

            var runner = new RuleRunner();

            var statementResult = runner.IsStatementTrue("input.Id == 1", employee);
            var blockResult = runner.IsBlockTrue("if(input.Designation != null && input.Designation.Name != null && input.Designation.Name.ToLower() == \"manager\") return true; else return false;", employee);
            var blockResult1 = runner.IsBlockTrue("return input.LstDesignation.Find(r => r.Name == \"Fulano\") != null;", employee);            


            //demo of how types from another assembly can be used:
            var runner2 = new RuleRunner(new List<Assembly>
            {
                typeof(ClassFromAnotherAssembly).Assembly //you need to add reference to the external assembly like this.
            });

            var statementResult2 = runner2.IsStatementTrue("input.Id == 1", thirdAssemblyClass);



        }

    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Designation Designation { get; set; }

        public List<Designation> LstDesignation { get; set; }
    }

    public class Designation
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }



}
