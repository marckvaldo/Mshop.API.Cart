using Polly;
using Polly.CircuitBreaker;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Infra.Consumer.CircuitBreakerPolicy
{
    public class CircuitBreaker : ICircuitBreaker
    {
        private  AsyncCircuitBreakerPolicy _circuitBreaker;
        public void Start(Func<Exception, bool> exceptionHandler, int allowBeforeBreaking, TimeSpan durationOfBreak)
        {
           _circuitBreaker = Policy
                  //.Handle<RedisConnectionException>() // Exceção específica
                  //.Or<Exception>() // Geral, caso necessário
                  .Handle(exceptionHandler)
                  .CircuitBreakerAsync(allowBeforeBreaking, durationOfBreak); // 1 erro em 30 segundos
        }

        public string GetStatus()
        {
            return _circuitBreaker.CircuitState.ToString();
        }

        public async Task<T> ExecuteActionAsync<T>(Func<Task<T>> action)
        {
            return await _circuitBreaker.ExecuteAsync(action);
        }

    }
}
