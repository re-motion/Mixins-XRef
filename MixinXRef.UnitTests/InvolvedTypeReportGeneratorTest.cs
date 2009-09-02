using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InvolvedTypeReportGeneratorTest
  {
    private ErrorAggregator<Exception> _configurationErrors;
    private ErrorAggregator<Exception> _validationErrors;
    private ReadonlyIdentifierGenerator<Type> _readonlyInvolvedTypeIdentifierGenerator;
    private IRemotionReflection _remotionReflection;
    private IOutputFormatter _outputFormatter;

    private readonly SummaryPicker _summaryPicker = new SummaryPicker();

    [SetUp]
    public void SetUp ()
    {
      _configurationErrors = new ErrorAggregator<Exception>();
      _validationErrors = new ErrorAggregator<Exception>();
      _remotionReflection = ProgramTest.GetRemotionReflection();
      _outputFormatter = new OutputFormatter();
    }

    [Test]
    public void GenerateXml_NoInvolvedTypes ()
    {
      var reportGenerator = CreateReportGenerator (
          new Assembly[0], new MixinConfiguration(), new IdentifierGenerator<Type>(), new IdentifierGenerator<Type>());
      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("InvolvedTypes");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_ForGenericTypeDefinition ()
    {
      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();

      var mixinConfiguration = new MixinConfiguration();

      var involvedType1 = new InvolvedType (typeof (GenericTarget<,>));

      var reportGenerator = CreateReportGenerator (
          new Assembly[0],
          mixinConfiguration,
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          involvedType1);

      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "InvolvedTypes",
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "GenericTarget<TParameter1, TParameter2>"),
              new XAttribute ("base", "Object"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-generic-definition", true),
              _summaryPicker.GetSummary (involvedType1.Type),
              new MemberReportGenerator (involvedType1.Type, reportGenerator.GetTargetClassDefinition(involvedType1), _outputFormatter).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType1.Type, interfaceIdentifierGenerator, _remotionReflection).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType1.Type, attributeIdentifierGenerator, _remotionReflection).GenerateXml(),
              new MixinReferenceReportGenerator (
                  involvedType1,
                  reportGenerator.GetTargetClassDefinition(involvedType1),
                  _readonlyInvolvedTypeIdentifierGenerator,
                  interfaceIdentifierGenerator,
                  attributeIdentifierGenerator,
                  _remotionReflection,
                  _outputFormatter).
                  GenerateXml(),
              new TargetReferenceReportGenerator (involvedType1, _readonlyInvolvedTypeIdentifierGenerator).GenerateXml()
              ));
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_InvolvedTypes ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();

      var involvedType1 = new InvolvedType (typeof (TargetClass1));
      involvedType1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());
      var involvedType2 = new InvolvedType (typeof (TargetClass2));
      involvedType2.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.Last());
      var involvedType3 = new InvolvedType (typeof (Mixin1));
      involvedType3.TargetTypes.Add (typeof (TargetClass1));
      var involvedType4 = new InvolvedType (typeof (Mixin2));
      involvedType4.TargetTypes.Add (typeof (TargetClass2));

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();

      InvolvedTypeReportGenerator reportGenerator = CreateReportGenerator (
          new Assembly[0],
          mixinConfiguration,
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          involvedType1,
          involvedType2,
          involvedType3,
          involvedType4);

      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "InvolvedTypes",
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass1"),
              new XAttribute ("base", "Object"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-generic-definition", false),
              _summaryPicker.GetSummary (involvedType1.Type),
              new MemberReportGenerator(involvedType1.Type, reportGenerator.GetTargetClassDefinition(involvedType1), _outputFormatter).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType1.Type, interfaceIdentifierGenerator, _remotionReflection).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType1.Type, attributeIdentifierGenerator, _remotionReflection).GenerateXml(),
              new MixinReferenceReportGenerator (
                  involvedType1,
                  reportGenerator.GetTargetClassDefinition(involvedType1),
                  _readonlyInvolvedTypeIdentifierGenerator,
                  interfaceIdentifierGenerator,
                  attributeIdentifierGenerator,
                  _remotionReflection,
                  _outputFormatter).
                  GenerateXml(),
              new TargetReferenceReportGenerator (involvedType1, _readonlyInvolvedTypeIdentifierGenerator).GenerateXml()
              ),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "1"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass2"),
              new XAttribute ("base", "Object"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-generic-definition", false),
              _summaryPicker.GetSummary (involvedType2.Type),
              new MemberReportGenerator(involvedType2.Type, reportGenerator.GetTargetClassDefinition(involvedType2), _outputFormatter).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType2.Type, interfaceIdentifierGenerator, _remotionReflection).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType2.Type, attributeIdentifierGenerator, _remotionReflection).GenerateXml(),
              new MixinReferenceReportGenerator (
                  involvedType2,
                  reportGenerator.GetTargetClassDefinition(involvedType2),
                  _readonlyInvolvedTypeIdentifierGenerator,
                  interfaceIdentifierGenerator,
                  attributeIdentifierGenerator,
                  _remotionReflection,
                  _outputFormatter).
                  GenerateXml(),
              new TargetReferenceReportGenerator (involvedType2, _readonlyInvolvedTypeIdentifierGenerator).GenerateXml()),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "2"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "Mixin1"),
              new XAttribute ("base", "Object"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", true),
              new XAttribute ("is-generic-definition", false),
              _summaryPicker.GetSummary (involvedType3.Type),
              new MemberReportGenerator(involvedType3.Type, reportGenerator.GetTargetClassDefinition(involvedType3), _outputFormatter).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType3.Type, interfaceIdentifierGenerator, _remotionReflection).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType3.Type, attributeIdentifierGenerator, _remotionReflection).GenerateXml(),
              new MixinReferenceReportGenerator (
                  involvedType3,
                  reportGenerator.GetTargetClassDefinition(involvedType3),
                  _readonlyInvolvedTypeIdentifierGenerator,
                  interfaceIdentifierGenerator,
                  attributeIdentifierGenerator,
                  _remotionReflection,
                  _outputFormatter).
                  GenerateXml(),
              new TargetReferenceReportGenerator (involvedType3, _readonlyInvolvedTypeIdentifierGenerator).GenerateXml()
              ),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "3"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "Mixin2"),
              new XAttribute ("base", "Object"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", true),
              new XAttribute ("is-generic-definition", false),
              _summaryPicker.GetSummary (involvedType4.Type),
              new MemberReportGenerator(involvedType4.Type, reportGenerator.GetTargetClassDefinition(involvedType4), _outputFormatter).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType4.Type, interfaceIdentifierGenerator, _remotionReflection).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType4.Type, attributeIdentifierGenerator, _remotionReflection).GenerateXml(),
              new MixinReferenceReportGenerator (
                  involvedType4,
                  reportGenerator.GetTargetClassDefinition(involvedType4),
                  _readonlyInvolvedTypeIdentifierGenerator,
                  interfaceIdentifierGenerator,
                  attributeIdentifierGenerator,
                  _remotionReflection,
                  _outputFormatter).
                  GenerateXml(),
              new TargetReferenceReportGenerator (involvedType4, _readonlyInvolvedTypeIdentifierGenerator).GenerateXml()
              )
          );


      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_DifferentAssemblies ()
    {
      var involvedType1 = new InvolvedType (typeof (TargetClass1));
      involvedType1.ClassContext = new ReflectedObject (new ClassContext (typeof (TargetClass1)));
      var involvedType2 = new InvolvedType (typeof (object));

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();

      var mixinConfiguration = new MixinConfiguration();

      InvolvedTypeReportGenerator reportGenerator =
          CreateReportGenerator (
              new[] { typeof (InvolvedType).Assembly, typeof (InvolvedTypeReportGeneratorTest).Assembly, typeof (object).Assembly },
              mixinConfiguration,
              interfaceIdentifierGenerator,
              attributeIdentifierGenerator,
              involvedType1,
              involvedType2);

      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "InvolvedTypes",
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "1"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass1"),
              new XAttribute ("base", "Object"),
              new XAttribute ("base-ref", "1"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-generic-definition", false),
              _summaryPicker.GetSummary (involvedType1.Type),
              new MemberReportGenerator(involvedType1.Type, reportGenerator.GetTargetClassDefinition(involvedType1), _outputFormatter).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType1.Type, interfaceIdentifierGenerator, _remotionReflection).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType1.Type, attributeIdentifierGenerator, _remotionReflection).GenerateXml(),
              new MixinReferenceReportGenerator (
                  involvedType1,
                  reportGenerator.GetTargetClassDefinition(involvedType1),
                  _readonlyInvolvedTypeIdentifierGenerator,
                  interfaceIdentifierGenerator,
                  attributeIdentifierGenerator,
                  _remotionReflection,
                  _outputFormatter).
                  GenerateXml(),
              new TargetReferenceReportGenerator (involvedType1, _readonlyInvolvedTypeIdentifierGenerator).GenerateXml()
              ),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "1"),
              new XAttribute ("assembly-ref", "2"),
              new XAttribute ("namespace", "System"),
              new XAttribute ("name", "Object"),
              new XAttribute ("base", "none"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-generic-definition", false),
              _summaryPicker.GetSummary (involvedType2.Type),
              new MemberReportGenerator(involvedType2.Type, reportGenerator.GetTargetClassDefinition(involvedType1), _outputFormatter).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType2.Type, interfaceIdentifierGenerator, _remotionReflection).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType2.Type, attributeIdentifierGenerator, _remotionReflection).GenerateXml(),
              new MixinReferenceReportGenerator (
                  involvedType2,
                  reportGenerator.GetTargetClassDefinition(involvedType1),
                  _readonlyInvolvedTypeIdentifierGenerator,
                  interfaceIdentifierGenerator,
                  attributeIdentifierGenerator,
                  _remotionReflection,
                  _outputFormatter).
                  GenerateXml(),
              new TargetReferenceReportGenerator (involvedType2, _readonlyInvolvedTypeIdentifierGenerator).GenerateXml()
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GetTargetClassDefinition ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .BuildConfiguration();
      var reportGenerator = CreateReportGenerator (
          new Assembly[0], mixinConfiguration, new IdentifierGenerator<Type>(), new IdentifierGenerator<Type>());
      var involvedType = new InvolvedType (typeof (TargetClass1));
      involvedType.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());

      var output = reportGenerator.GetTargetClassDefinition (involvedType).To<TargetClassDefinition>();
      var expectedOutput = TargetClassDefinitionUtility.GetConfiguration (typeof (TargetClass1), mixinConfiguration);

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void GetTargetClassDefinition_NoTarget ()
    {
      var mixinConfiguration = new MixinConfiguration();
      var reportGenerator = CreateReportGenerator (
          new Assembly[0], mixinConfiguration, new IdentifierGenerator<Type>(), new IdentifierGenerator<Type>());
      var involvedType = new InvolvedType (typeof (TargetClass1));

      var output = reportGenerator.GetTargetClassDefinition (involvedType);

      Assert.That (_configurationErrors.Exceptions.Count(), Is.EqualTo (0));
      Assert.That (_validationErrors.Exceptions.Count(), Is.EqualTo (0));
      Assert.That (output, Is.Null);
    }

    [Test]
    public void GetTargetClassDefinition_GenericTarget ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass (typeof (GenericTarget<,>)).AddMixin<Mixin1>()
          .BuildConfiguration();
      var reportGenerator = CreateReportGenerator (
          new Assembly[0], mixinConfiguration, new IdentifierGenerator<Type>(), new IdentifierGenerator<Type>());
      var involvedType = new InvolvedType (typeof (GenericTarget<,>));

      var output = reportGenerator.GetTargetClassDefinition (involvedType);

      Assert.That (_configurationErrors.Exceptions.Count(), Is.EqualTo (0));
      Assert.That (_validationErrors.Exceptions.Count(), Is.EqualTo (0));
      Assert.That (output, Is.Null);
    }

    [Test]
    public void GenerateXml_MixinConfigurationError ()
    {
      var involvedType = new InvolvedType (typeof (UselessObject));
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<UselessObject>().AddMixin<MixinWithConfigurationError>().BuildConfiguration();
      involvedType.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.Last());
      var reportGenerator = CreateReportGenerator (
          new Assembly[0], mixinConfiguration, new IdentifierGenerator<Type>(), new IdentifierGenerator<Type>());

      var output = reportGenerator.GetTargetClassDefinition (involvedType);

      Assert.That (_configurationErrors.Exceptions.Count(), Is.EqualTo (1));
      Assert.That (output, Is.Null);
    }

    [Test]
    public void GenerateXml_MixinValidationError ()
    {
      var involvedType = new InvolvedType (typeof (UselessObject));
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<UselessObject>().AddMixin<UselessObject>().BuildConfiguration();
      involvedType.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());
      var reportGenerator = CreateReportGenerator (
          new Assembly[0], mixinConfiguration, new IdentifierGenerator<Type>(), new IdentifierGenerator<Type>());

      var output = reportGenerator.GetTargetClassDefinition (involvedType);

      Assert.That (_validationErrors.Exceptions.Count(), Is.EqualTo (1));
      Assert.That (output, Is.Null);
    }

    private InvolvedTypeReportGenerator CreateReportGenerator (
        Assembly[] referencedAssemblies,
        MixinConfiguration mixinConfiguration,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator,
        IIdentifierGenerator<Type> attributeIdentifierGenerator,
        params InvolvedType[] involvedTypes)
    {
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();

      foreach (var referencedAssembly in referencedAssemblies)
        assemblyIdentifierGenerator.GetIdentifier (referencedAssembly);

      _readonlyInvolvedTypeIdentifierGenerator =
          new IdentifierPopulator<Type> (involvedTypes.Select (it => it.Type)).GetReadonlyIdentifierGenerator ("none");

      return new InvolvedTypeReportGenerator (
          involvedTypes,
          new ReflectedObject (mixinConfiguration),
          assemblyIdentifierGenerator,
          _readonlyInvolvedTypeIdentifierGenerator,
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          _configurationErrors,
          _validationErrors,
          _remotionReflection,
          new OutputFormatter()
          );
    }
  }
}