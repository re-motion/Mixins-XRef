// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef.UnitTests.CustomRemotionReflector
{
  public class CustomRemotionReflector : IRemotionReflector
  {
    private Assembly _remotionAssembly;
    private Assembly _remotionInterfaceAssembly;

    private Assembly LoadFile (string assemblyDirectory, string assemblyFileName)
    {
      return Assembly.LoadFile (Path.GetFullPath (Path.Combine (assemblyDirectory, assemblyFileName)));
    }

    public IEnumerable<string> GetRemotionAssemblyFileNames ()
    {
      return new[] { "Remotion.dll", "Remotion.Interfaces.dll" };
    }

    public void LoadRemotionAssemblies ()
    {
      
    }

    public IRemotionReflector Initialize(string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      _remotionAssembly = LoadFile (assemblyDirectory, "Remotion.dll");
      _remotionInterfaceAssembly = LoadFile (assemblyDirectory, "Remotion.Interfaces.dll");

      return this;
    }

    public void InitializeLogging ()
    {
      
    }

    public ITypeDiscoveryService GetTypeDiscoveryService ()
    {
      var type = _remotionInterfaceAssembly.GetType ("Remotion.Reflection.TypeDiscovery.ContextAwareTypeDiscoveryUtility", true);
      return ReflectedObject.CallMethod (type, "GetInstance").To<ITypeDiscoveryService>();
    }

    public bool IsRelevantAssemblyForConfiguration(Assembly assembly)
    {
      return true;
    }

    public virtual bool IsNonApplicationAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      return
          assembly.GetCustomAttributes (false).Any (
              attribute => attribute.GetType ().FullName == "Remotion.Reflection.NonApplicationAssemblyAttribute");
    }

    public virtual bool IsConfigurationException (Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      return exception.GetType ().FullName == "Remotion.Mixins.ConfigurationException";
    }

    public virtual bool IsValidationException (Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      return exception.GetType ().FullName == "Remotion.Mixins.Validation.ValidationException";
    }

    public virtual bool IsInfrastructureType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return type.Assembly.GetName ().Name == "Remotion.Interfaces";
    }

    public virtual bool IsInheritedFromMixin (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var mixinBaseType = _remotionInterfaceAssembly.GetType ("Remotion.Mixins.IInitializableMixin", true);
      return mixinBaseType.IsAssignableFrom (type);
    }

    public virtual ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionUtilityType = _remotionAssembly.GetType ("Remotion.Mixins.TargetClassDefinitionUtility", true);
      return ReflectedObject.CallMethod (targetClassDefinitionUtilityType, "GetConfiguration", targetType, mixinConfiguration);
    }

    public virtual ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      var declarativeConfigurationBuilderType = _remotionAssembly.GetType ("Remotion.Mixins.Context.DeclarativeConfigurationBuilder", true);
      return ReflectedObject.CallMethod (declarativeConfigurationBuilderType, "BuildConfigurationFromAssemblies", new object[] { assemblies });
    }

    public virtual ReflectedObject GetValidationLogFromValidationException (Exception validationException)
    {
      ArgumentUtility.CheckNotNull ("validationException", validationException);

      return new ReflectedObject (validationException).GetProperty ("ValidationLog");
    }

    public ReflectedObject GetTargetCallDependencies(ReflectedObject mixinDefinition)
    {
      return mixinDefinition;
    }

    public ICollection<Type> GetComposedInterfaces (ReflectedObject classContext)
    {
      return new Type[0];
    }

    public ReflectedObject GetNextCallDependencies(ReflectedObject mixinDefinition)
    {
      return mixinDefinition;
    }
  }
}