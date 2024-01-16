using System.Windows.Markup;

namespace EventHarbor.Class
{
    /// <summary>
    /// Helper class for data binding enum values to ComboBox
    /// </summary>
    class EnumBindingSourceExtension : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumBindingSourceExtension(Type enumType)
        {
            if (enumType == null || !enumType.IsEnum)
            {
                throw new ArgumentNullException(nameof(enumType));
            }
            this.EnumType = enumType;
        }




        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }




    }
}
