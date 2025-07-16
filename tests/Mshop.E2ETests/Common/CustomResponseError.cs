namespace Mshop.E2ETests.Common
{
    public class CustomResponseErro
    {

        public List<string> Errors { get; set; }

        public bool Success { get; set; }

        public CustomResponseErro(List<string> errors, bool success)
        {
            Errors = errors;
            Success = success;
        }

        
    }
}
