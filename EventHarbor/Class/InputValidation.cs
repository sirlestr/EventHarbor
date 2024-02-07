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

namespace EventHarbor.Class
{
    internal  class InputValidation
    {
       public InputValidation() { }


        //todo
        public int ValidateNumber(string input)
        {
            
            if (string.IsNullOrEmpty(input))
            {
                throw new Exception("Input cannot be empty");
                
            }

            if (int.TryParse(input, out int output))
            {
                return output;
            }
            else
            {
                throw new ArgumentException("Input is not a valid number");
            }


        }


        public string  ValidateText(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input.Trim();
            }
            else
            {
                throw new ArgumentException("Input cannot be empty");
            }
        }



        public DateOnly ValidateDateOnly(DateTime? input)
        {
            return ConvertFromDateTime(input);
        }

        private DateOnly ConvertFromDateTime(DateTime? input)
        {
            if (input.HasValue)
            {
                return DateOnly.FromDateTime(input.Value);
            }
            else
            {
                throw new Exception("Input cannot be empty");
            }
        }
    }
}
