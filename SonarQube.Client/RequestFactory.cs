using System;
using System.Collections.Generic;
using System.Linq;

namespace SonarQube.Client
{
    public class RequestFactory
    {
        private readonly Dictionary<Type, SortedList<string, Func<object>>> requestMappings =
            new Dictionary<Type, SortedList<string, Func<object>>>();

        public RequestFactory RegisterRequest<TRequest, TRequestImpl>(string version)
            where TRequest : IRequestBase
            where TRequestImpl : TRequest, new()
        {
            return RegisterRequest<TRequest, TRequestImpl>(version, () => new TRequestImpl());
        }

        public RequestFactory RegisterRequest<TRequest, TRequestImpl>(string version, Func<TRequestImpl> factory)
            where TRequest : IRequestBase
            where TRequestImpl : TRequest
        {
            if (!requestMappings.TryGetValue(typeof(TRequest), out var map))
            {
                map = new SortedList<string, Func<object>>(StringComparer.OrdinalIgnoreCase);
                requestMappings[typeof(TRequest)] = map;
            }
            map[version] = () => factory();
            return this;
        }

        /// <summary>
        /// Creates a new TRequest implementation for the specified SonarQube version.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request implementation to create.</typeparam>
        /// <param name="version">
        /// SonarQube version to return a request implementation for. The default value returns the
        /// latest registered implementation.
        /// </param>
        /// <returns>New TRequest implementation for the specified SonarQube version.</returns>
        public TRequest Create<TRequest>(string version = null)
            where TRequest : IRequestBase
        {
            if (requestMappings.TryGetValue(typeof(TRequest), out var map))
            {
                bool LessOrEqualThanVersion(KeyValuePair<string, Func<object>> entry) =>
                    version == null || StringComparer.OrdinalIgnoreCase.Compare(entry.Key, version) <= 0;

                var factory = map.LastOrDefault(LessOrEqualThanVersion).Value;

                if (factory != null)
                {
                    return (TRequest)factory();
                }

                throw new InvalidOperationException($"Could not find compatible implementation of '{typeof(TRequest).Name}' for SonarQube {version}.");
            }
            throw new InvalidOperationException($"Could not find implementation for '{typeof(TRequest).Name}'.");
        }
    }
}
