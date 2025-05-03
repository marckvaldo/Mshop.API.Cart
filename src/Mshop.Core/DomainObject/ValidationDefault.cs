using Mshop.Core.Message;

namespace Mshop.Core.DomainObject
{

    public static class ValidationDefault
    {
        public static void NotNull(string target, string fieldName, INotification notification)
        {
            if (target is null)
                notification.AddNotifications($"O {fieldName} não deve ser null");
        }

        public static void NotNullOrEmpty(string target, string fieldName, INotification notification)
        {
            if (string.IsNullOrWhiteSpace(target))
                notification.AddNotifications($"O {fieldName} não pode set vario ou null");
        }

        public static void NotNullOrEmpty(Guid? target, string fieldName, INotification notification)
        {
            if (target == null || target.Value == Guid.Empty)
                notification.AddNotifications($"O {fieldName} não pode set vario ou null");
        }

        public static void MinLength(string target, int minLength, string fieldName, INotification notification)
        {
            NotNull(target, fieldName, notification);

            if (target is not null && target.Length < minLength)
                notification.AddNotifications($"O {fieldName} não pode ser menor que {minLength} characters");
        }

        public static void MaxLength(string target, int maxLength, string fieldName, INotification notification)
        {
            NotNull(target, fieldName, notification);

            if (target is not null && target.Length > maxLength)
                notification.AddNotifications($"O {fieldName} não pode ser maior que {maxLength} characters");
        }

        //Numbers
        public static void MustPositive(decimal target, string fieldName, INotification notification)
        {
            if (target < 0)
                notification.AddNotifications($"O {fieldName} deve ser um numero positivo");
        }

        public static void MustBiggerThan(decimal target, decimal value, string fieldName, INotification notification)
        {
            if (target < value)
                notification.AddNotifications($"O {fieldName} deve ser maior {value}");
        }

        public static void MustBiggerOrEqualThan(decimal target, decimal value, string fieldName, INotification notification)
        {
            if (target <= value)
                notification.AddNotifications($"O {fieldName} deve ser igual ou maior que {value}");
        }

        public static void MustBeLessThan(decimal target, decimal value, string fieldName, INotification notification)
        {
            if (target > value)
                notification.AddNotifications($"O {fieldName} dever ser menor que {value}");
        }

        public static void MustLessOrEqualThan(decimal target, decimal value, string fieldName, INotification notification)
        {
            if (target >= value)
                notification.AddNotifications($"O {fieldName} dever ser igual ou menor que {value}");
        }

        public static void NotEqual(decimal target, decimal value, string fieldName, INotification notification, string? messagem = null)
        {
            if (messagem is null)
                messagem = $"O {fieldName} não é igual a {value}";

            if (target != value)
                notification.AddNotifications(messagem);
        }

        public static void NotNullGuid(Guid target, string fieldName, INotification notification)
        {
            if (target == Guid.Empty)
                notification.AddNotifications($"O {fieldName} não deve ser null");
        }


        public static void IsValidGuid(string target, string fieldName, INotification notification)
        {
            Guid guidValid = Guid.Empty;
            if (!Guid.TryParse(target, out guidValid))
                notification.AddNotifications($" {fieldName} invalid");


        }
    }
}
