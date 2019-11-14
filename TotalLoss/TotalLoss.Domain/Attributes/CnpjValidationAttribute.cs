using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TotalLoss.Domain.Attributes
{
    public class CnpjValidationAttribute : ValidationAttribute
    {
        public CnpjValidationAttribute() { }

        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;

            return ValidateCNPJ(value.ToString());
        }

        public bool ValidateCNPJ(string valueCNPJ) 
        {
            bool[] cnpjValid;
            string cnpj;
            string ftmt;
            int nrDig;
            int[] digitos, soma, resultado;

            ftmt = "6543298765432";
            digitos = new int[14];
            soma = new int[2];
            soma[0] = 0;
            soma[1] = 0;
            resultado = new int[2];
            resultado[0] = 0;
            resultado[1] = 0;
            cnpjValid = new bool[2];
            cnpjValid[0] = false;
            cnpjValid[1] = false;

            try
            {
                cnpj = valueCNPJ.Replace(".", "");
                cnpj = cnpj.Replace("/", "");
                cnpj = cnpj.Replace("-", "");

                for (nrDig = 0; nrDig < 14; nrDig++)
                {
                    digitos[nrDig] = int.Parse(
                     cnpj.Substring(nrDig, 1));
                    if (nrDig <= 11)
                        soma[0] += (digitos[nrDig] *
                        int.Parse(ftmt.Substring(
                          nrDig + 1, 1)));
                    if (nrDig <= 12)
                        soma[1] += (digitos[nrDig] *
                        int.Parse(ftmt.Substring(
                          nrDig, 1)));
                }

                for (nrDig = 0; nrDig < 2; nrDig++)
                {
                    resultado[nrDig] = (soma[nrDig] % 11);
                    if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1))
                        cnpjValid[nrDig] = (
                        digitos[12 + nrDig] == 0);

                    else
                        cnpjValid[nrDig] = (
                        digitos[12 + nrDig] == (
                        11 - resultado[nrDig]));
                }

                return (cnpjValid[0] && cnpjValid[1]);
            }
            catch
            {
                return false;
            }
        }
    }
}