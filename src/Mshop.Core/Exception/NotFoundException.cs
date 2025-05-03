namespace Mshop.Core.Exception
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string? message) : base(message) { }

        public static void ThrowIfnull(object? @object, string exceptionMessage = "Not found")
        {
            if (@object == null)
                throw new NotFoundException(exceptionMessage);
        }

    }
}
