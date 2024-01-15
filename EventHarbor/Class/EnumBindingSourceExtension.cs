using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace EventHarbor.Class
{
    class EnumBindingSourceExtension : MarkupExtension
    {
        public Type EnumType { get;  private set; }
        
        public EnumBindingSourceExtension(Type enumType)
        {
            if (enumType == null|| !enumType.IsEnum) 
            {  
                throw new ArgumentNullException(nameof(enumType));
            }
            this.EnumType = enumType;
        }



        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetNames(this.EnumType);
            //return Enum.GetValues(EnumType);
        }
    }
}
