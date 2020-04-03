using SmallRepair.Management.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmallRepair.Api.Model
{

    public class RequiredIfAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessageFormatString = "The {0} field is required.";
        private readonly string _dependentProperties;
        private readonly object _valueProperty;

        public RequiredIfAttribute(string dependentProperties, object valueProperty)
        {
            _dependentProperties = dependentProperties;
            _valueProperty = valueProperty;
            ErrorMessage = DefaultErrorMessageFormatString;
        }

        protected override ValidationResult IsValid(Object value, ValidationContext context)
        {
            Object instance = context.ObjectInstance;
            Type type = instance.GetType();

            Object propertyValue = type.GetProperty(_dependentProperties).GetValue(instance, null);

            if (propertyValue != null)
            {
                if (propertyValue.Equals(_valueProperty) && typeof(int).Equals(value.GetType()) && (int)value <= 0)
                    return new ValidationResult(context.DisplayName + " required. ");
                else if (propertyValue.Equals(_valueProperty) && typeof(double).Equals(value.GetType()) && (double)value <= 0)
                    return new ValidationResult(context.DisplayName + " required. ");
                else
                    return ValidationResult.Success;
            }

            return new ValidationResult(context.DisplayName + " required. ");
        }
    }

    public class PartViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        [Range(1, 4)]
        public EnmMalfunctionType MalfunctionType { get; set; }

        [Required]
        [Range(1, 3)]
        public EnmIntensityType IntensityType { get; set; }

        [RequiredIf("MalfunctionType", EnmMalfunctionType.Hail)]
        public double UnitaryValue { get; set; }

        [RequiredIf("MalfunctionType", EnmMalfunctionType.Hail)]
        public int Quantity { get; set; }
    }

    public class ManualPartViewModel
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public double UnitaryValue { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
