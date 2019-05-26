using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Windows;


namespace WpfApp2
{
    public class Checker : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(!(value is string str)) return new ValidationResult(false,"Wrong type of data!");
            int cashint;
            if (!(Int32.TryParse(str, out cashint))) return new ValidationResult(false,"Value should be integer!");
            if (cashint >= 5000)
            {
                return new ValidationResult(true,"");
            }
                return new ValidationResult(false,$"Salary can not be less than 5000!");
        }
    }

    public class Visible_collapsed : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<ValidationError> roc)
            {
                if (roc.Count > 0) return Visibility.Visible;
                return Visibility.Collapsed;
            }
            return null;
        }


         public  object ConvertBack( object value,  Type targetType, object parameter, CultureInfo culture)
        {

                throw new NotImplementedException();
        }
    }


    class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            if (!(value is DateTime dt))
                return null;
            string str = "";
            str = $"{dt.Day}.{dt.Month}.{dt.Year}";
            return str;
        }

        public object ConvertBack(object value,
        Type targetType,object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class SelectTemplateForRole : DataTemplateSelector
    {
        public DataTemplate defaultTmpl { get; set; }
        public DataTemplate ceoTmpl { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Employee e   &&   e.CompanyRole   ==   Role.CEO ) return ceoTmpl;

            return defaultTmpl;
        }
    }

    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type _enumType;
        public Type EnumType
        {
            get { return this._enumType; }
            set
            {
                if (value != this._enumType)
                {

                    if (null   !=   value)
                    {
                        Type   enumType   =  Nullable.GetUnderlyingType(value) ?? value;
                        if (   !enumType.IsEnum)
                              throw new     ArgumentException("Type must be for an Enum.");
                    }

                    this._enumType = value;
                }
            }
        }

        public   EnumBindingSourceExtension() { }

        public   EnumBindingSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        public   override    object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null    ==   this._enumType)
                 throw new InvalidOperationException("The EnumType must be specified.");

                Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
                Array enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType  ==  this._enumType)
                return enumValues;

            Array   tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
                enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }
}
