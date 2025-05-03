using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Infra.Consumer.CircuitBreakerPolicy
{
    public interface ICircuitBreaker
    {
        public void Start(Func<Exception, bool> exceptionHandler, int allowBeforeBreaking, TimeSpan durationOfBreak);

        public Task<T> ExecuteActionAsync<T>(Func<Task<T>> action);
    }
}
