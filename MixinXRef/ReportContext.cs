using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace MixinXRef
{
  public class ReportContext
  {
    public ReportContext (
      IEnumerable<Assembly> assemblies,
      IdentifierGenerator<Assembly> assemblyIdentifierGenerator,
      IdentifierGenerator<Type> involvedTypeIdentifierGenerator,
      IdentifierGenerator<Type> interfaceIdentifierGenerator,
      IdentifierGenerator<Type> attributeIdentifierGenerator,
      IInvolvedTypeFinder involvedTypeFinder
      )
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeFinder", involvedTypeFinder);
      
      Assemblies = assemblies;
      AssemblyIdentifierGenerator = assemblyIdentifierGenerator;
      InvolvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      InterfaceIdentifierGenerator = interfaceIdentifierGenerator;
      AttributeIdentifierGenerator = attributeIdentifierGenerator;
      InvolvedTypeFinder = involvedTypeFinder;
    }

    public IEnumerable<Assembly> Assemblies { get; set; }
    public IdentifierGenerator<Assembly> AssemblyIdentifierGenerator { get; set; }
    public IdentifierGenerator<Type> InvolvedTypeIdentifierGenerator { get; set; }
    public IdentifierGenerator<Type> InterfaceIdentifierGenerator { get; set; }
    public IdentifierGenerator<Type> AttributeIdentifierGenerator { get; set; }
    public IInvolvedTypeFinder InvolvedTypeFinder { get; set; }
  }
}