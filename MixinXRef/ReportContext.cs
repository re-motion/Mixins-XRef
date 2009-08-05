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
      IInvolvedTypeFinder involvedTypeFinder
      )
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeFinder", involvedTypeFinder);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);

      Assemblies = assemblies;
      AssemblyIdentifierGenerator = assemblyIdentifierGenerator;
      InvolvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      InterfaceIdentifierGenerator = interfaceIdentifierGenerator;
      InvolvedTypeFinder = involvedTypeFinder;
    }

    public IEnumerable<Assembly> Assemblies { get; set; }
    public IdentifierGenerator<Assembly> AssemblyIdentifierGenerator { get; set; }
    public IdentifierGenerator<Type> InvolvedTypeIdentifierGenerator { get; set; }
    public IdentifierGenerator<Type> InterfaceIdentifierGenerator { get; set; }
    public IInvolvedTypeFinder InvolvedTypeFinder { get; set; }
  }
}