using Mshop.Core.Message;

namespace Mshop.Core.DomainObject
{
    public class Result<T> where T : IModelOutPut
    {
        public Result(bool isSuccess, T? data, INotification notifications)
        {
            IsSuccess = isSuccess;
            Data = data;
        }

        public bool IsSuccess { get;}

        public T Data { get;}

        public INotification Notification { get;}

        public static Result<T> Success(T data) => new(true, data, new Notifications());
        public static Result<T> Error(INotification notifications) => new(false, default, notifications);
        public static Result<T> Error() => new(false, default, new Notifications());

    }
}
