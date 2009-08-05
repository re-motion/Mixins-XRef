using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace MixinXRef
{
  public class ReportContext
  {
    private readonly IEnumerable<Assembly> _assemblies;
    private readonly IdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly InvolvedTypeFinder _involvedTypeFinder;

    public ReportContext (
      IEnumerable<Assembly> assemblies,
      IdentifierGenerator<Assembly> assemblyIdentifierGenerator,
      IdentifierGenerator<Type> involvedTypeIdentifierGenerator,
      InvolvedTypeFinder involvedTypeFinder
      )
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeFinder", involvedTypeFinder);

      _assemblies = assemblies;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _involvedTypeFinder = involvedTypeFinder;
    }

    public IEnumerable<Assembly> Assemblies
    {
      get { return _assemblies; }
    }

    public IdentifierGenerator<Assembly> AssemblyIdentifierGenerator
    {
      get { return _assemblyIdentifierGenerator; }
    }

    public IdentifierGenerator<Type> InvolvedTypeIdentifierGenerator
    {
      get { return _involvedTypeIdentifierGenerator; }
    }

    public InvolvedTypeFinder InvolvedTypeFinder
    {
      get { return _involvedTypeFinder; }
    }
  }
}