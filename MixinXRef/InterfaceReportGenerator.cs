using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using Remotion.Mixins.Context;

namespace MixinXRef
{
  public class InterfaceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IRemotionReflection _remotionReflection;
    private readonly IOutputFormatter _outputFormatter;

    public InterfaceReportGenerator (
        InvolvedType[] involvedTypes,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator,
        IRemotionReflection remotionReflection,
        IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _remotionReflection = remotionReflection;
      _outputFormatter = outputFormatter;
    }


    public XElement GenerateXml ()
    {
      var allInterfaces = GetAllInterfaces();
      var completeInterfaces = GetCompleteInterfaces();

      return new XElement (
          "Interfaces",
          from usedInterface in allInterfaces.Keys
          where !_remotionReflection.IsInfrastructureType (usedInterface)
          select GenerateInterfaceElement (usedInterface, allInterfaces, completeInterfaces.Contains(usedInterface))
          );
    }

    public HashSet<Type> GetCompleteInterfaces ()
    {
      var allCompleteInterfaces = new HashSet<Type> ();

      foreach (var involvedType in _involvedTypes)
      {
        if (!involvedType.IsTarget) continue;

        foreach (var completeInterface in involvedType.ClassContext.GetProperty ("CompleteInterfaces"))
        {
          allCompleteInterfaces.Add (completeInterface.To<Type>());
        }
      }

      return allCompleteInterfaces;
    }


    private Dictionary<Type, List<Type>> GetAllInterfaces ()
    {
      var allInterfaces = new Dictionary<Type, List<Type>>();

      foreach (var involvedType in _involvedTypes)
      {
        foreach (var usedInterface in involvedType.Type.GetInterfaces())
        {
          if (!allInterfaces.ContainsKey (usedInterface))
            allInterfaces.Add (usedInterface, new List<Type>());

          allInterfaces[usedInterface].Add (involvedType.Type);
        }

        if (involvedType.IsTarget)
        {
          foreach (var completeInterface in involvedType.ClassContext.GetProperty ("CompleteInterfaces"))
          {
            var completeInterfaceType = completeInterface.To<Type>();
            if (!allInterfaces.ContainsKey (completeInterfaceType))
              allInterfaces.Add (completeInterfaceType, new List<Type>());

            allInterfaces[completeInterfaceType].Add (involvedType.Type);
          }
        }
      }

      return allInterfaces;
    }

    private XElement GenerateInterfaceElement (Type usedInterface, Dictionary<Type, List<Type>> allInterfaces, bool isCompleteInterface)
    {
      return new XElement (
          "Interface",
          new XAttribute ("id", _interfaceIdentifierGenerator.GetIdentifier (usedInterface)),
          new XAttribute ("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier (usedInterface.Assembly)),
          new XAttribute ("namespace", usedInterface.Namespace),
          new XAttribute("name", _outputFormatter.GetShortFormattedTypeName(usedInterface)),
          new XAttribute ("is-complete-interface", isCompleteInterface),
          new MemberReportGenerator (usedInterface, null, _outputFormatter).GenerateXml(),
          new XElement (
              "ImplementedBy",
              from implementingType in allInterfaces[usedInterface]
              select
                  new XElement (
                  "InvolvedType",
                  new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (implementingType)))
              )
          );
    }
  }
}