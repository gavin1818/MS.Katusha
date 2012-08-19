﻿// This software is part of the Autofac IoC container
// Copyright (c) 2012 Autofac Contributors
// http://autofac.org
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System.Web.Http.Dependencies;

namespace Autofac.Integration.WebApi
{
    /// <summary>
    /// Extension methods to the <see cref="IDependencyResolver"/> interface.
    /// </summary>
    public static class DependencyResolverExtensions
    {
        /// <summary>
        /// Gets the root lifetime scope from the Autofac dependency resolver.
        /// </summary>
        public static ILifetimeScope GetRootLifetimeScope(this IDependencyResolver dependencyResolver)
        {
            var resolver = dependencyResolver as AutofacWebApiDependencyResolver;
            return (resolver == null) ? null : resolver.Container;
        }

        /// <summary>
        /// Gets the request lifetime scope from the Autofac dependency scope.
        /// </summary>
        public static ILifetimeScope GetRequestLifetimeScope(this IDependencyScope dependencyScope)
        {
            var scope = dependencyScope as AutofacWebApiDependencyScope;
            return (scope == null) ? null : scope.LifetimeScope;
        }
    }
}