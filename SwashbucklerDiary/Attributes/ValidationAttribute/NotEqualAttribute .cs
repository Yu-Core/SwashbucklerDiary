using System.ComponentModel.DataAnnotations;

namespace SwashbucklerDiary.Attributes
{
    public class NotEqualAttribute : CompareAttribute
    {
        public NotEqualAttribute(string otherProperty) : base(otherProperty)
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var validationResult = base.IsValid(value, validationContext);

            string[]? memberNames = validationContext.MemberName != null
                   ? new[] { validationContext.MemberName }
                   : null;
            return validationResult is null 
                ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames) 
                : null;
        }
    }
}
