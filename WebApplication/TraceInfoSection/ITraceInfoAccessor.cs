using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WebApplication.TraceInfoSection
{
    public interface ITraceInfoAccessor
    {
        ITraceInfo TraceInfo { get; set; }
    }

    public class TraceInfoAccessor : ITraceInfoAccessor
    {
        private static readonly AsyncLocal<TraceInfoHolder> _traceInfoHolder = new AsyncLocal<TraceInfoHolder>();

        public ITraceInfo TraceInfo
        {
            get => _traceInfoHolder.Value?.TraceInfo;
            set
            {
                TraceInfoHolder holder = _traceInfoHolder.Value;
                if (holder != null)
                {
                    // Clear current TraceInfo trapped in the AsyncLocals, as its done.
                    holder.TraceInfo = null;
                }

                if (value != null)
                {
                    // Use an object indirection to hold the TraceInfo in the AsyncLocal,
                    // so it can be cleared in all ExecutionContexts when its cleared.
                    _traceInfoHolder.Value = new TraceInfoHolder {TraceInfo = value};
                }
            }
        }

        private class TraceInfoHolder
        {
            public ITraceInfo TraceInfo;
        }
    }

    public static class TraceInfoAccessorInjectionExtensions
    {
        public static IServiceCollection AddTraceInfoAccessor(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ITraceInfoAccessor, TraceInfoAccessor>();
            return serviceCollection;
        }
    }
}