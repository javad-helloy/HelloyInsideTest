﻿//-------------------------------------------------------------------------------
// <copyright file="IWebApiRequestScopeProvider.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2012 Ninject Project Contributors
//   Authors: Remo Gloor (remo.gloor@gmail.com)
//           
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
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
    using Ninject.Activation;
    using Ninject.Components;

    /// <summary>
    /// The provider for the request scope.
    /// </summary>
    public interface IWebApiRequestScopeProvider : INinjectComponent
    {
        /// <summary>
        /// Gets the request scope for the current activation context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The request scope.</returns>
        object GetRequestScope(IContext context);
    }
}