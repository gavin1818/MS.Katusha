﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Builder;
using Autofac.Features.Metadata;
using Autofac.Features.Scanning;
using NUnit.Framework;
using Autofac.Tests.Scenarios.ScannedAssembly;
using Autofac.Core;
using Autofac.Core.Lifetime;
using System.Reflection;

namespace Autofac.Tests.Features.Scanning
{
    [TestFixture]
    public class ScanningRegistrationTests
    {
        static readonly Assembly ScenarioAssembly = typeof(AComponent).Assembly;

        [Test]
        public void WhenAssemblyIsScannedTypesRegisteredByDefault()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(AComponent).Assembly);
            var c = cb.Build();

            c.AssertRegistered<AComponent>();
            c.AssertSharing<AComponent>(InstanceSharing.None);
            c.AssertLifetime<AComponent, CurrentScopeLifetime>();
            c.AssertOwnership<AComponent>(InstanceOwnership.OwnedByLifetimeScope);
        }

        [Test]
        public void WhenTypesRegisteredAsSelfConcreteTypeIsService()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(AComponent).Assembly)
                .AsSelf();
            var c = cb.Build();

            c.AssertRegistered<AComponent>();
        }

        [Test]
        public void WhenNameAndMetadataMappingAppliedValueCalculatedFromType()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(AComponent).Assembly)
                .WithMetadata("Name", t => t.Name);

            var c = cb.Build();

            var a = c.Resolve<Meta<AComponent>>();

            Assert.AreEqual(typeof(AComponent).Name, a.Metadata["Name"]);
        }

        [Test]
        public void WhenMetadataMappingAppliedValuesCalculatedFromType()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(SaveCommand).Assembly)
                .Where(t => t != typeof(UndoRedoCommand))
                .WithMetadata(t => t.GetMethods().ToDictionary(m => m.Name, m => (object)m.ReturnType));

            var c = cb.Build();
            var s = c.Resolve<Meta<SaveCommand>>();

            Assert.IsTrue(s.Metadata.ContainsKey("Execute"));
        }

        [Test]
        public void WhenFiltersAppliedNonMatchingTypesExcluded()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(AComponent).Assembly)
                .Where(t => t.Name.StartsWith("A"))
                .Where(t => t.Name.StartsWith("AC"));
            var c = cb.Build();

            c.AssertRegistered<AComponent>();
            c.AssertNotRegistered<BComponent>();
            c.AssertNotRegistered<A2Component>();
        }

        [Test]
        public void WhenServiceSpecifiedDirectlyAllMatchingTypesImplementIt()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(AComponent).Assembly)
                .As(typeof(object));
            var c = cb.Build();

            c.AssertRegistered<object>();
            c.AssertNotRegistered<AComponent>();
        }

        [Test]
        public void WhenServicesSpecifiedByFunctionEachTypeMappedIndependently()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(AComponent).Assembly)
                .As(t => new KeyedService(t.Name, typeof(object)));
            var c = cb.Build();

            c.AssertRegistered<object>("AComponent");
            c.AssertRegistered<object>("BComponent");
            c.AssertNotRegistered<AComponent>();
        }

        [Test]
        public void NameMappingRegistersKeyedServices()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(AComponent).Assembly)
                .Named(t => t.Name, typeof(object));
            var c = cb.Build();

            c.AssertRegistered<object>("AComponent");
        }

        [Test]
        public void NameMappingRegistersKeyedServicesWithGenericSyntax()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(AComponent).Assembly)
                .Named<object>(t => t.Name);
            var c = cb.Build();

            c.AssertRegistered<object>("AComponent");
        }

        [Test]
        public void TypeMappingRegistersTypedServices()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(AComponent).Assembly)
                .As(t => typeof(object));
            var c = cb.Build();

            c.AssertRegistered<object>();
        }

        [Test]
        public void AsImplementedInterfacesRegistersImplementedInterfaces()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(AComponent).Assembly)
                .AsImplementedInterfaces();
            var c = cb.Build();

            c.AssertRegistered<IAService>();
            c.AssertRegistered<IBService>();
        }

        [Test]
        public void WhenFilterAppliedDefaultSelfRegistrationOmitted()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(AComponent).Assembly)
                .AsImplementedInterfaces();
            var c = cb.Build();

            c.AssertNotRegistered<AComponent>();
        }

        [Test]
        public void AsClosedTypesOfNullTypeProvidedThrowsException()
        {
            var cb = new ContainerBuilder();
            Assert.Throws<ArgumentNullException>(() => cb.RegisterAssemblyTypes(typeof(ICommand<>).Assembly).
                AsClosedTypesOf(null));
        }

        [Test]
        public void AsClosedTypesOfOpenGenericInterfaceTypeProvidedClosingGenericTypesRegistered()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(ICommand<>).Assembly)
                .AsClosedTypesOf(typeof(ICommand<>));
            var c = cb.Build();

            Assert.That(c.Resolve<ICommand<SaveCommandData>>(), Is.TypeOf<SaveCommand>());
            Assert.That(c.Resolve<ICommand<DeleteCommandData>>(), Is.TypeOf<DeleteCommand>());
        }

        [Test]
        public void AsClosedTypesOfOpenGenericAbstractClassTypeProvidedClosingGenericTypesRegistered()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(Message<>).Assembly)
                .AsClosedTypesOf(typeof(Message<>));
            var c = cb.Build();

            Assert.That(c.Resolve<Message<string>>(), Is.TypeOf<StringMessage>());
        }

        [Test]
        public void AsSelfExposesConcreteTypeAsService()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(A2Component).Assembly)
                .AsImplementedInterfaces()
                .AsSelf();
            var c = cb.Build();

            Assert.That(c.IsRegistered<A2Component>());
        }

        [Test]
        public void AsClosedTypesOfMultipleServicesPerClassExposesAllServices()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(UndoRedoCommand).Assembly)
                .AsClosedTypesOf(typeof(ICommand<>));
            var c = cb.Build();

            var r = c.RegistrationFor<ICommand<UndoCommandData>>();
            Assert.That(r.Services.Contains(new TypedService(typeof(ICommand<RedoCommandData>))));
        }

        [Test]
        public void DoesNotIncludeDelegateTypesThusNotOverridingGeneratedFactories()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(HasNestedFactoryDelegate).Assembly);
            var c = cb.Build();
            c.Resolve<HasNestedFactoryDelegate.Factory>();
        }

        [Test]
        public void WhenScannedTypesAreRegisteredOnRegisteredHandlersAreCalled()
        {
            var onRegisteredCalled = false;

            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(AComponent).Assembly)
                .OnRegistered(e => onRegisteredCalled = true);
            cb.Build();

            Assert.That(onRegisteredCalled);
        }

        [Test]
        public void WhenTypedServicesAreSpecifiedImplicitFilterApplied()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(typeof(A2Component).Assembly)
                .As<IAService>();
            var c = cb.Build();
            // Without the filter this line would throw anyway
            var a = c.Resolve<IEnumerable<IAService>>();
            Assert.AreEqual(1, a.Count());
        }

        [Test]
        public void WhenExceptionsProvideConfigurationComponentConfiguredAppropriately()
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(ScenarioAssembly)
                .Except<AComponent>(ac => ac.SingleInstance());
            var c = cb.Build();
            var a1 = c.Resolve<AComponent>();
            var a2 = c.Resolve<AComponent>();
            Assert.AreSame(a1, a2);
        }

        [Test]
        public void SingleRegistrationCanBeRegisteredAsSelf()
        {
            var cb = new ContainerBuilder();

            cb.RegisterType<A2Component>()
                .As<IAService>()
                .AsSelf();

            var c = cb.Build();
            c.AssertRegistered<A2Component>();
            c.AssertRegistered<IAService>();
        }

        [Test]
        public void SingleRegistrationCanBeRegisteredAsImplementedInterfaces()
        {
            var cb = new ContainerBuilder();

            cb.RegisterType<A2Component>()
                .AsImplementedInterfaces();

            var c = cb.Build();
            c.AssertRegistered<IAService>();
            c.AssertRegistered<IBService>();
        }

        [Test]
        public void WhenTransformingTypesToServicesNonAssignableServicesAreExcluded()
        {
            var cb = new ContainerBuilder();

            cb.RegisterAssemblyTypes(ScenarioAssembly)
                .As(t => new KeyedService("foo", typeof(ScanningRegistrationTests)));

            var c = cb.Build();

            Assert.IsFalse(c.IsRegisteredWithKey<ScanningRegistrationTests>("foo"));
        }

        [Test]
        public void WhenTransformingTypesToServicesComponentsWithNoServicesAreExcluded()
        {
            var cb = new ContainerBuilder();

            cb.RegisterAssemblyTypes(ScenarioAssembly)
                .As(t => new KeyedService("foo", typeof(ScanningRegistrationTests)));

            var c = cb.Build();

            Assert.IsFalse(c.ComponentRegistry.Registrations.Any(r =>
                r.Activator.LimitType == typeof(AComponent)));
        }

        [Test]
        public void WhenMappingToMultipleTypedServicesEachExposedAsService()
        {
            var c = RegisterScenarioAssembly(a => a.As(t => t.GetInterfaces()));
            var cd1 = c.ComponentRegistry.Registrations.Single(r => r.Activator.LimitType == typeof(A2Component));
            Assert.That(cd1.Services.Contains(new TypedService(typeof(IAService))));
            Assert.That(cd1.Services.Contains(new TypedService(typeof(IBService))));
        }

        [Test]
        public void ByDefaultIDisposableIsNotAServiceInterface()
        {
            var c = RegisterScenarioAssembly(a => a.AsImplementedInterfaces());
            Assert.IsFalse(c.IsRegistered<IDisposable>());
        }

        [Test]
        public void WhenDerivingKeysDynamically_TheCorrectOverloadIsChosen()
        {
            const string key = "akey";
            var c = RegisterScenarioAssembly(a => a.Keyed<IAService>(t => key));
            Assert.That(c.IsRegisteredWithKey<IAService>(key));
        }

        [Test]
        public void PreserveExistingDefaults()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<AnExistingComponent>().As<IAService>();
            cb.RegisterAssemblyTypes(ScenarioAssembly)
                .As<IAService>()
                .PreserveExistingDefaults();

            var c = cb.Build();

            c.AssertComponentRegistrationOrder<IAService, AnExistingComponent, A2Component>();
        }


        public IContainer RegisterScenarioAssembly(Action<IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>> configuration = null)
        {
            var cb = new ContainerBuilder();
            var config = cb.RegisterAssemblyTypes(ScenarioAssembly);
            if (configuration != null)
                configuration(config);
            return cb.Build();
        }

        // ReSharper disable ClassNeverInstantiated.Local
        /// <summary>
        /// Test class used in the <see cref="ScanningRegistrationTests.PreserveExistingDefaults"/> test case.
        /// </summary>
        private class AnExistingComponent : IAService
        {
        }

        [Test]
        public void MetadataCanBeScannedFromAMatchingAttributeInterface()
        {
            var c = RegisterScenarioAssembly(a => a
                .Where(t => t == typeof (ScannedComponentWithName))
                .WithMetadataFrom<IHaveName>());

            IComponentRegistration r;
            c.ComponentRegistry.TryGetRegistration(new TypedService(typeof (ScannedComponentWithName)), out r);

            object name;
            r.Metadata.TryGetValue("Name", out name);

            Assert.AreEqual("My Name", name);
        }

        [Test]
        public void ScanningKeyedRegistrationsFilterByAssignabilityBeforeMappingKey()
        {
            const string k = "key";
            var c = RegisterScenarioAssembly(a => a.Keyed<IAService>(t =>
            {
                Assert.That(typeof(IAService).IsAssignableFrom(t));
                return k;
            }));
            Assert.That(c.IsRegisteredWithKey<IAService>(k));
        }
    }
}
