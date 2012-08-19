﻿// This software is part of the Autofac IoC container
// Copyright (c) 2007 - 2008 Autofac Contributors
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

using System;
using System.Linq;
using Autofac;
using Autofac.Core;

namespace AutofacContrib.Startable
{
    /// <summary>
    /// Can be used to instantiate an instance of all 'startable' services in
    /// a container.
    /// </summary>
    class Starter : IStarter
    {
        IComponentContext _context;

        /// <summary>
        /// Saved as an extended property to identify a component as startable.
        /// </summary>
        public const string IsStartablePropertyName = "Autofac.Extras.Startable.Starter.IsStartable";

        /// <summary>
        /// Initializes a new instance of the <see cref="Starter"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public Starter(IComponentContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        /// <summary>
        /// Start the startable components.
        /// </summary>
        public void Start()
        {
            var startableRegistrations =
                from cr in _context.ComponentRegistry.Registrations
                where cr.Metadata.ContainsKey(IsStartablePropertyName) &&
                    (bool)cr.Metadata[IsStartablePropertyName]
                select cr;

            foreach (var startable in startableRegistrations)
                _context.ResolveComponent(startable, Enumerable.Empty<Parameter>());
        }
    }
}
