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
      IInvolvedTypeFinder involvedTypeFinder
      )
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeFinder", involvedTypeFinder);

      Assemblies = assemblies;
      AssemblyIdentifierGenerator = assemblyIdentifierGenerator;
      InvolvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      InvolvedTypeFinder = involvedTypeFinder;
    }

    public IEnumerable<Assembly> Assemblies { get; set; }
    public IdentifierGenerator<Assembly> AssemblyIdentifierGenerator { get; set; }
    public IdentifierGenerator<Type> InvolvedTypeIdentifierGenerator { get; set; }
    public IInvolvedTypeFinder InvolvedTypeFinder { get; set; }
  }
}