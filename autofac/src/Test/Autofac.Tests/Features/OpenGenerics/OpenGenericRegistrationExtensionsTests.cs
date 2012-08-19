﻿using System;
using System.Linq;
using NUnit.Framework;
using Autofac.Core;
using System.Collections.Generic;

namespace Autofac.Tests.Features.OpenGenerics
{
    // ReSharper disable UnusedTypeParameter
    public interface IG<T>
    // ReSharper restore UnusedTypeParameter
    {
    }

    public class G<T> : IG<T>
    {
        public G()
        {
        }

        public G(int i)
        {
            I = i;
        }

        public int I { get; private set; }
    }

    [TestFixture]
    public class OpenGenericRegistrationExtensionsTests
    {
        [Test]
        public void BuildGenericRegistration()
        {
            var componentType = typeof(G<>);
            var serviceType = typeof(IG<>);
            var concreteServiceType = typeof(IG<int>);

            var cb = new ContainerBuilder();
            cb.RegisterGeneric(componentType)
                .As(serviceType);
            var c = cb.Build();

            object g1 = c.Resolve(concreteServiceType);
            object g2 = c.Resolve(concreteServiceType);

            Assert.IsNotNull(g1);
            Assert.IsNotNull(g2);
            Assert.AreNotSame(g1, g2);
            Assert.IsTrue(g1.GetType().GetGenericTypeDefinition() == componentType);
        }

        [Test]
        public void ExposesImplementationType()
        {
            var cb = new ContainerBuilder();
            cb.RegisterGeneric(typeof(G<>)).As(typeof(IG<>));
            var container = cb.Build();
            IComponentRegistration cr;
            Assert.IsTrue(container.ComponentRegistry.TryGetRegistration(
                new TypedService(typeof(IG<int>)), out cr));
            Assert.AreEqual(typeof(G<int>), cr.Activator.LimitType);
        }

        [Test]
        public void FiresPreparing()
        {
            int preparingFired = 0;
            var cb = new ContainerBuilder();
            cb.RegisterGeneric(typeof(G<>))
                .As(typeof(IG<>))
                .OnPreparing(e => ++preparingFired);
            var container = cb.Build();
            container.Resolve<IG<int>>();
            Assert.AreEqual(1, preparingFired);
        }

        [Test]
        public void WhenNoServicesExplicitlySpecifiedGenericComponentTypeIsService()
        {
            var cb = new ContainerBuilder();
            cb.RegisterGeneric(typeof(G<>));
            var c = cb.Build();
            c.AssertRegistered<G<int>>();
        }

        [Test]
        public void SuppliesParameterToConcreteComponent()
        {
            var cb = new ContainerBuilder();
            cb.RegisterGeneric(typeof(G<>)).WithParameter(new NamedParameter("i", 42));
            var c = cb.Build();
            var g = c.Resolve<G<string>>();
            Assert.AreEqual(42, g.I);
        }

        [Test]
        public void WhenRegistrationNamedGenericRegistrationsSuppliedViaName()
        {
            const string name = "n";
            var cb = new ContainerBuilder();
            cb.RegisterGeneric(typeof(G<>))
                .Named(name, typeof(IG<>));
            var c = cb.Build();
            Assert.That(c.IsRegisteredWithName<IG<int>>(name));
            Assert.That(c.IsRegisteredWithName<IG<string>>(name));
        }

        [Test]
        public void RegisterGenericRejectsNonOpenGenericTypes()
        {
            var cb = new ContainerBuilder();
            Assert.Throws<ArgumentException>(() => cb.RegisterGeneric(typeof(List<int>)));
        }

        // ReSharper disable UnusedTypeParameter
        public interface ITwoParams<T, U> { }
        // ReSharper restore UnusedTypeParameter
        
        public class TwoParams<T, U> : ITwoParams<T, U> { }

        [Test]
        public void MultipleTypeParametersAreMatched()
        {
            var cb = new ContainerBuilder();
            cb.RegisterGeneric(typeof(TwoParams<,>)).As(typeof(ITwoParams<,>));
            var c = cb.Build();
            c.Resolve<ITwoParams<int, string>>();
        }

        [Test, Ignore("Not sure where to detect this but would be nice.")]
        public void NonGenericServiceTypesAreRejected()
        {
            var cb = new ContainerBuilder();
            Assert.Throws<ArgumentException>(() =>
            {
                cb.RegisterGeneric(typeof(IList<>)).As(typeof(object));
                cb.Build();
            });
        }

        [Test]
        public void WhenAnOpenGenericIsRegisteredWithANameItProvidesOnlyOneImplementation()
        {
            var cb = new ContainerBuilder();
            cb.RegisterGeneric(typeof(G<>)).Named("n", typeof(IG<>));
            var c = cb.Build();
            Assert.AreEqual(1, c.ResolveNamed<IEnumerable<IG<int>>>("n").Count());
        }

        [Test]
        public void WhenAnOpenGenericIsRegisteredWithANameItCannotBeResolvedWithoutOne()
        {
            var cb = new ContainerBuilder();
            cb.RegisterGeneric(typeof(G<>)).Named("n", typeof(IG<>));
            var c = cb.Build();
            Assert.AreEqual(0, c.Resolve<IEnumerable<IG<int>>>().Count());
        }

        [Test]
        public void WhenAnOpenGenericIsRegisteredItProvidesOnlyOneImplementation()
        {
            var cb = new ContainerBuilder();
            cb.RegisterGeneric(typeof(G<>)).As(typeof(IG<>));
            var c = cb.Build();
            Assert.AreEqual(1, c.Resolve<IEnumerable<IG<int>>>().Count());
        }
    }
}
