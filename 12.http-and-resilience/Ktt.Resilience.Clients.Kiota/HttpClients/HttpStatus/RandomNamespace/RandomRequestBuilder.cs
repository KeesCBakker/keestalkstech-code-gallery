// <auto-generated/>
#pragma warning disable CS0618
using Ktt.Resilience.Clients.Kiota.HttpClients.HttpStatus.RandomNamespace.TwoZeroZeroFiveZeroZeroFiveZeroTwoFiveZeroThree;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace Ktt.Resilience.Clients.Kiota.HttpClients.HttpStatus.RandomNamespace
{
    /// <summary>
    /// Builds and executes requests for operations under \random
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class RandomRequestBuilder : BaseRequestBuilder
    {
        /// <summary>The TwoZeroZeroFiveZeroZeroFiveZeroTwoFiveZeroThree property</summary>
        public global::Ktt.Resilience.Clients.Kiota.HttpClients.HttpStatus.RandomNamespace.TwoZeroZeroFiveZeroZeroFiveZeroTwoFiveZeroThree.TwoZeroZeroFiveZeroZeroFiveZeroTwoFiveZeroThreeRequestBuilder TwoZeroZeroFiveZeroZeroFiveZeroTwoFiveZeroThree
        {
            get => new global::Ktt.Resilience.Clients.Kiota.HttpClients.HttpStatus.RandomNamespace.TwoZeroZeroFiveZeroZeroFiveZeroTwoFiveZeroThree.TwoZeroZeroFiveZeroZeroFiveZeroTwoFiveZeroThreeRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Ktt.Resilience.Clients.Kiota.HttpClients.HttpStatus.RandomNamespace.RandomRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public RandomRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/random", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Ktt.Resilience.Clients.Kiota.HttpClients.HttpStatus.RandomNamespace.RandomRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public RandomRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/random", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
