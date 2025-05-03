namespace Mshop.Core.Exception
{
    public class ApplicationValidationException : ApplicationException
    {

        public ApplicationValidationException(
            string message
            ) : base(message)
        {
        }
    }


}