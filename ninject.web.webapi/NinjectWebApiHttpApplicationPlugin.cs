//-------------------------------------------------------------------------------
// <copyright file="NinjectWebApiHttpApplicationPlugin.cs" company="bbv Software Services AG">
//   Copyright (c) 2012 bbv Software Services AG
//   Author: Remo Gloor (remo.gloor@gmail.com)
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Ninject.Web.WebApi
{
    using System.Web.Http;
    using System.Web.Http.Dependencies;

    using Ninject.Activation;
    using Ninject.Components;
    using Ninject.Web.Common;

    /// <summary>
    /// The web plugin implementation for MVC
    /// </summary>
    public class NinjectWebApiHttpApplicationPlugin : NinjectComponent, INinjectHttpApplicationPlugin
    {
        /// <summary>
        /// The ninject kernel.
        /// </summary>
        private readonly IKernel kernel;

        private readonly IWebApiRequestScopeProvider webApiRequestScopeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectWebApiHttpApplicationPlugin"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="webApiRequestScopeProvider">The web API request scope provider.</param>
        public NinjectWebApiHttpApplicationPlugin(IKernel kernel, IWebApiRequestScopeProvider webApiRequestScopeProvider)
        {
            this.kernel = kernel;
            this.webApiRequestScopeProvider = webApiRequestScopeProvider;
        }

        /// <summary>
        /// Gets the request scope.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The request scope.</returns>
        public object GetRequestScope(IContext context)
        {
            return this.webApiRequestScopeProvider.GetRequestScope(context);
        }
        
        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.kernel.Get<HttpConfiguration>().DependencyResolver = this.CreateDependencyResolver();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
        }

        public object RequestScope { get; set; }

        /// <summary>
        /// Creates the controller factory that is used to create the controllers.
        /// </summary>
        /// <returns>The created controller factory.</returns>
        protected IDependencyResolver CreateDependencyResolver()
        {
            return this.kernel.Get<IDependencyResolver>();
        }
    }
}