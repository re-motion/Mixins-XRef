using System;
using System.Diagnostics;
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
      
      //const string assemblyDir = @"C:\Development\Remotion-Contrib\MixinXRef\trunk\MixinXRef.UnitTests\bin\Debug\";
      const string assemblyDirectory = @"C:\Users\patrick.groess\Desktop\ActaNovaClientWebBin\bin";
      const string outputFile = @"C:\Users\patrick.groess\Desktop\MixinReport.xml";
      //const string assemblyDirectory = @"C:\Users\julian.lettner\Desktop\ActaNovaClientWebBin\bin";
      //const string outputFile = @"C:\Users\julian.lettner\Desktop\MixinReport.xml";

      Assembly[] assemblies = GetAssemblies (assemblyDirectory);

      var stopwatch = Stopwatch.StartNew();
      var mixinConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);
      Console.WriteLine ("MixinConfiguration: " + stopwatch.Elapsed);

      stopwatch.Reset();
      stopwatch.Start();
      var involvedTypes = new InvolvedTypeFinder (mixinConfiguration).FindInvolvedTypes();
      Console.WriteLine ("FindInvolvedTypes: " + stopwatch.Elapsed);

      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();
      var readonlyassemblyIdentifierGenerator = assemblyIdentifierGenerator.GetReadonlyIdentiferGenerator ("none");
      var involvedTypeIdentiferGenerator = new IdentifierGenerator<Type>();
      var interfaceIdentiferGenerator = new IdentifierGenerator<Type>();
      var attributeIdentiferGenerator = new IdentifierGenerator<Type>();

      var assemblyReport = new AssemblyReportGenerator (
          assemblies, involvedTypes, assemblyIdentifierGenerator, involvedTypeIdentiferGenerator);

      var involvedReport = new InvolvedTypeReportGenerator (
          involvedTypes,
          mixinConfiguration,
          readonlyassemblyIdentifierGenerator,
          involvedTypeIdentiferGenerator,
          interfaceIdentiferGenerator,
          attributeIdentiferGenerator);
      var interfaceReport = new InterfaceReportGenerator (
          involvedTypes, readonlyassemblyIdentifierGenerator, involvedTypeIdentiferGenerator, interfaceIdentiferGenerator);
      var attributeReport = new AttributeReportGenerator (
          involvedTypes, readonlyassemblyIdentifierGenerator, involvedTypeIdentiferGenerator, attributeIdentiferGenerator);

      stopwatch.Reset();
      stopwatch.Start();
      var compositeReportGenerator = new CompositeReportGenerator (assemblyReport, involvedReport, interfaceReport, attributeReport);
      Console.WriteLine ("CompositeReportGenerator: " + stopwatch.Elapsed);

      stopwatch.Reset();
      stopwatch.Start();
      XElement report = compositeReportGenerator.GenerateXml();
      String creationTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss");
      report.Add(new XAttribute("creationTime", creationTime));
      Console.WriteLine ("GenerateXml: " + stopwatch.Elapsed);

      stopwatch.Reset();
      stopwatch.Start();
      new XDocument (report).Save (outputFile);
      Console.WriteLine ("Save: " + stopwatch.Elapsed);
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