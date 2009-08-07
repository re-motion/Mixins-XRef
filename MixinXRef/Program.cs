using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Mixins.Context;

namespace MixinXRef
{
  internal class Program
  {
    private static void Main (string[] args)
    {
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;

      const string assemblyDir = @"C:\Users\patrick.groess\Desktop\temp\";
      Assembly[] assemblies = GetAssemblies (assemblyDir);

      var mixinConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);

      var involvedTypes = new InvolvedTypeFinder (mixinConfiguration).FindInvolvedTypes();

      ReadonlyIdentifierGenerator<Assembly> assemblyIdentifierGenerator = CreateAssemblyIdentifierGenerator(assemblies);
      var involvedTypeIdentiferGenerator = new IdentifierGenerator<Type>();
      var interfaceIdentiferGenerator = new IdentifierGenerator<Type>();
      var attributeIdentiferGenerator = new IdentifierGenerator<Type>();

      var assemblyReport = new AssemblyReportGenerator (
          assemblies, involvedTypes, assemblyIdentifierGenerator, involvedTypeIdentiferGenerator);     
      
      var involvedReport = new InvolvedTypeReportGenerator (
          involvedTypes, assemblyIdentifierGenerator, involvedTypeIdentiferGenerator, interfaceIdentiferGenerator, attributeIdentiferGenerator);
      var interfaceReport = new InterfaceReportGenerator (
          involvedTypes, assemblyIdentifierGenerator, involvedTypeIdentiferGenerator, interfaceIdentiferGenerator);
      var attributeReport = new AttributeReportGenerator (
          involvedTypes, assemblyIdentifierGenerator, involvedTypeIdentiferGenerator, attributeIdentiferGenerator);

      var compositeReportGenerator = new CompositeReportGenerator (assemblyReport, involvedReport, interfaceReport, attributeReport);

      XElement report = compositeReportGenerator.GenerateXml();

      new XDocument (report).Save (@"C:\Users\patrick.groess\Desktop\MixinReport.xml");
    }

    private static ReadonlyIdentifierGenerator<Assembly> CreateAssemblyIdentifierGenerator (Assembly[] assemblies)
    {
      var identifierGenerator =new IdentifierGenerator<Assembly>();
      foreach (var assembly in assemblies)
      {
        identifierGenerator.GetIdentifier (assembly);
      }
      return identifierGenerator.GetReadonlyIdentiferGenerator ("dummy-value");
    }


    private static Assembly[] GetAssemblies (string assemblyDir)
    {
      // get all assemblies
      string[] dlls = Directory.GetFiles (assemblyDir, "*.dll");
      string[] exes = Directory.GetFiles (assemblyDir, "*.exe");

      string[] assemblyFiles = new string[dlls.Length + exes.Length];
      dlls.CopyTo (assemblyFiles, 0);
      exes.CopyTo (assemblyFiles, dlls.Length);

      Assembly[] assemblies = new Assembly[assemblyFiles.Length];
      for (int i = 0; i < assemblyFiles.Length; i++)
        assemblies[i] = Assembly.LoadFile (assemblyFiles[i]);
      return assemblies;
    }


    private static Assembly CurrentDomainAssemblyResolve (object sender, ResolveEventArgs args)
    {
      // All assemblies in the target directory have already been loaded.
      // Therefore, we can be sure that the referenced assembly has already been loaded if it is in the right directory.
      AssemblyName assemblyName = new AssemblyName (args.Name);
      return
          AppDomain.CurrentDomain.GetAssemblies().Where (a => AssemblyName.ReferenceMatchesDefinition (assemblyName, a.GetName())).SingleOrDefault();
    }
  }
}