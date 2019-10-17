using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using Unity;

namespace TotalLoss.API
{
    public class UnityResolver : IDependencyResolver
    {
        protected IUnityContainer unityContainer;

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.unityContainer = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return unityContainer.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return unityContainer.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = unityContainer.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            unityContainer.Dispose();
        }
    }
}