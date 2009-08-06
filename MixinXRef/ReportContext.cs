using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace MixinXRef
{
  public class ReportContext
  {
    public ReportContext (
      Assembly[] assemblies,
      InvolvedType[] involvedTypes,
      IdentifierGenerator<Assembly> assemblyIdentifierGenerator,
      IdentifierGenerator<Type> involvedTypeIdentifierGenerator,
      IdentifierGenerator<Type> interfaceIdentifierGenerator,
      IdentifierGenerator<Type> attributeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);

      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      
      Assemblies = assemblies;
      InvolvedTypes = involvedTypes;

      AssemblyIdentifierGenerator = assemblyIdentifierGenerator;
      InvolvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      InterfaceIdentifierGenerator = interfaceIdentifierGenerator;
      AttributeIdentifierGenerator = attributeIdentifierGenerator;
    }

    public Assembly[] Assemblies { get; private set; }
    public InvolvedType[] InvolvedTypes { get; private set; }

    public IdentifierGenerator<Assembly> AssemblyIdentifierGenerator { get; private set; }
    public IdentifierGenerator<Type> InvolvedTypeIdentifierGenerator { get; private set; }
    public IdentifierGenerator<Type> InterfaceIdentifierGenerator { get; private set; }
    public IdentifierGenerator<Type> AttributeIdentifierGenerator { get; private set; }
  }
}