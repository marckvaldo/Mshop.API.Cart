namespace MShop.API.Cart.Extension
{
    public class ExtensionResponse
    {
        public static object Success(object data)
        {
            return new
            {
                success = true,
                data
            };
        }
        public static object Error(List<string> errors)
        {
            return new
            {
                success = false,
                errors
            };
        }
    }
}
