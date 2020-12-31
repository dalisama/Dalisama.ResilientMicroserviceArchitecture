using System;

namespace Dalisama.ResilientMicroserviceArchitecture.Common
{
    public abstract class ServiceStatus
    {
        public event EventHandler<ServiceHealthEventArgs> ServiceHealthDegaded;
        public event EventHandler<ServiceHealthEventArgs> ServiceHealthUp;
        public bool IsHealthy { get; set; }

        public virtual void RaiseServiceHealthDegadedEvent(ServiceHealthEventArgs serviceHealthEventArgs)
        {
            OnServiceHealthDegaded(serviceHealthEventArgs);
        }
        public virtual void RaiseServiceHealthUpEvent(ServiceHealthEventArgs serviceHealthEventArgs)
        {
            OnServiceHealthUp(serviceHealthEventArgs);
        }
        protected virtual void OnServiceHealthDegaded(ServiceHealthEventArgs serviceHealthEventArgs)
        {
            EventHandler<ServiceHealthEventArgs> handler = ServiceHealthDegaded;
            IsHealthy = false;
            if (handler != null)
            {
                handler(this, serviceHealthEventArgs);
            }
        }
        protected virtual void OnServiceHealthUp(ServiceHealthEventArgs serviceHealthEventArgs)
        {
            EventHandler<ServiceHealthEventArgs> handler = ServiceHealthUp;
            IsHealthy = true;
            if (handler != null)
            {
                handler(this, serviceHealthEventArgs);
            }
        }

    }

}
