using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO.Packaging;

namespace EventHarbor.Class
{
    internal  class InputValidation
    {
       public InputValidation() { }


        //todo
        public int ValidateNumber(string input, string itemToValidate )
        {
            
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException($"{itemToValidate} není číslo, zkontroluj hodnoty");
                
            }

            if (int.TryParse(input, out int output))
            {
                return output;
            }
            else
            {
                throw new ArgumentException($"{itemToValidate} není platné číslo, zkontroluj hodnoty");
            }


        }


        public string  ValidateText(string input, string itemToValidate)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input.Trim();
            }
            else
            {
                throw new ArgumentException($"{itemToValidate}  nemůže být prázdný, zadej prosím hodnotu");
            }
        }



        public DateOnly ValidateDateOnly(DateTime? input, string itemToValidate)
        {
            return ConvertFromDateTime(input, itemToValidate);
        }

        private DateOnly ConvertFromDateTime(DateTime? input,string itemToValidate)
        {
            if (input.HasValue)
            {
                return DateOnly.FromDateTime(input.Value);
            }
            else
            {
                throw new Exception($"{itemToValidate} nemůže být prázdný, zadej prosím hodnotu");
            }
        }
    }
}
