namespace EventHarbor.Class
{
    internal class InputValidation
    {
        /// <summary>
        /// Constructor for InputValidation class
        /// </summary>
        public InputValidation() { }

        /// <summary>
        /// Validates the input to ensure it's a valid number
        /// </summary>
        /// <param name="input">The input to validate</param>
        /// <param name="itemToValidate">The item being validated</param>
        /// <returns>The validated number</returns>
        public int ValidateNumber(string input, string itemToValidate)
        {
            // Check if input is null or empty
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException($"{itemToValidate} není číslo, zkontroluj hodnoty");
            }

            // Try to parse the input as an integer
            if (int.TryParse(input, out int output))
            {
                return output;
            }
            else
            {
                throw new ArgumentException($"{itemToValidate} není platné číslo, zkontroluj hodnoty");
            }
        }

        /// <summary>
        /// Validates the input to ensure it's a valid text
        /// </summary>
        /// <param name="input">The input to validate</param>
        /// <param name="itemToValidate">The item being validated</param>
        /// <returns>The validated text</returns>
        public string ValidateText(string input, string itemToValidate)
        {
            // Check if input is not null or empty, and trim the input
            if (!string.IsNullOrEmpty(input))
            {
                return input.Trim();
            }
            else
            {
                throw new ArgumentException($"{itemToValidate} nemůže být prázdný, zadej prosím hodnotu");
            }
        }

        /// <summary>
        /// Validates the input to ensure it's a valid DateOnly
        /// </summary>
        /// <param name="input">The input to validate</param>
        /// <param name="itemToValidate">The item being validated</param>
        /// <returns>The validated DateOnly</returns>
        public DateOnly ValidateDateOnly(DateTime? input, string itemToValidate)
        {
            return ConvertFromDateTime(input, itemToValidate);
        }

        /// <summary>
        /// Converts a DateTime to a DateOnly
        /// </summary>
        /// <param name="input">The input to convert</param>
        /// <param name="itemToValidate">The item being converted</param>
        /// <returns>The converted DateOnly</returns>
        private DateOnly ConvertFromDateTime(DateTime? input, string itemToValidate)
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
