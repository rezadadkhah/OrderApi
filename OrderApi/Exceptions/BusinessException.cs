namespace OrderApi.Domain.Exceptions
{
    namespace YourProject.Domain.Exceptions
    {
        public class BusinessException : Exception
        {
            private readonly string _message;

            public BusinessException(string message)
            {
                _message = message;
            }

            public String Message => _message;
        }
    }
}
