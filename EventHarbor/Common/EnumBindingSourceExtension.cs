using System.Windows.Markup;

namespace EventHarbor.Common;

public class EnumBindingSourceExtension : MarkupExtension
{
    public Type EnumType { get; }

    public EnumBindingSourceExtension(Type enumType)
    {
        if (enumType is null || !enumType.IsEnum)
            throw new ArgumentException("EnumType must be a non-null enum type", nameof(enumType));
        EnumType = enumType;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
        => Enum.GetValues(EnumType);
}
