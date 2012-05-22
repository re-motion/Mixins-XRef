using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class TargetCallDependenciesReportGenerator : IReportGenerator
  {
    private readonly ReflectedObject _mixinDefinition;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IRemotionReflector _remotionReflector;
    private readonly IOutputFormatter _outputFormatter;

    public TargetCallDependenciesReportGenerator (ReflectedObject mixinDefinition, IIdentifierGenerator<Assembly> assemblyIdentifierGenerator, IRemotionReflector remotionReflector, IOutputFormatter outputFormatter)
    {
      _mixinDefinition = mixinDefinition;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _remotionReflector = remotionReflector;
      _outputFormatter = outputFormatter;
    }

    public XElement GenerateXml ()
    {
      var element = new XElement ("TargetCallDependencies");

      foreach (var targetCallDependencyDefinition in _remotionReflector.GetTargetCallDependencies (_mixinDefinition))
        element.Add (CreateDependencyElement (targetCallDependencyDefinition));

      return element;
    }

    private XElement CreateDependencyElement (ReflectedObject targetCallDependencyDefinition)
    {
      var targetClassDefinition = _mixinDefinition.GetProperty ("TargetClass");
      var requiredType = targetCallDependencyDefinition.GetProperty ("RequiredType").GetProperty ("Type").To<Type> ();
      var element = new XElement ("Dependency", new XAttribute ("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier (requiredType.Assembly)),
                                                new XAttribute ("namespace", requiredType.Namespace),
                                                new XAttribute ("name", _outputFormatter.GetShortFormattedTypeName (requiredType)),
                                                new XAttribute ("is-interface", requiredType.IsInterface));
      if (requiredType.IsInterface)
      {
        var implementedByTarget = targetClassDefinition.GetProperty ("ImplementedInterfaces").Any (i => i.To<Type> () == requiredType);
        var addedByMixin = targetClassDefinition.GetProperty ("ReceivedInterfaces").Any (i => i.GetProperty ("InterfaceType").To<Type> () == requiredType);
        var implementedDynamically = !implementedByTarget && !addedByMixin;

        element.Add (new XAttribute ("is-implemented-by-target", implementedByTarget));
        element.Add (new XAttribute ("is-added-by-mixin", addedByMixin));
        element.Add (new XAttribute ("is-implemented-dynamically", implementedDynamically));
      }

      return element;
    }
  }
}