using Orleans;
using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;
using System.Threading.Tasks;

namespace TestGrains
{
    public class GrainState
    {
        public int Count { get; set; }
    }

    public interface ITaskGrain : IGrainWithStringKey
    {
        [Transaction(TransactionOption.Create)]
        Task<GrainState> Execute();
    }

    [Reentrant]
    public class TaskGrain : Grain, ITaskGrain
    {
        private readonly ITransactionalState<GrainState> _state;

        public TaskGrain(
            [TransactionalState(nameof(TaskGrain), Constants.StorageName)]
            ITransactionalState<GrainState> state)
        {
            _state = state;
        }

        async Task<GrainState> ITaskGrain.Execute()
        {
            var state = await _state.PerformRead(_ => _);
            state.Count++;
            await _state.PerformUpdate(e => e.Count = state.Count);
            return state;
        }
    }
}