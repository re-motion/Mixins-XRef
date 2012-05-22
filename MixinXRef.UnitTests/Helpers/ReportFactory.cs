using System;
using System.Linq;
using System.Reflection;
using MixinXRef.Formatting;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Report;
using MixinXRef.Utility;

namespace MixinXRef.UnitTests.Helpers
{
  internal static class ReportFactory
  {
    public static AssemblyReportGenerator CreateAssemblyReportGenerator (params InvolvedType[] types)
    {
      return new AssemblyReportGenerator (types, new IdentifierGenerator<Assembly> (), new IdentifierGenerator<Type> ());
    }

    public static InterfaceReportGenerator CreateInterfaceReportGenerator (IRemotionReflector remotionReflector, IOutputFormatter outputFormatter, params InvolvedType[] types)
    {
      return new InterfaceReportGenerator (types,
                                           new IdentifierGenerator<Assembly> (),
                                           new IdentifierGenerator<Type> (),
                                           new IdentifierGenerator<MemberInfo> (),
                                           new IdentifierGenerator<Type> (),
                                           remotionReflector,
                                           outputFormatter);
    }

    public static MemberReportGenerator CreateMemberReportGenerator (Type type, IOutputFormatter outputFormatter)
    {
      return new MemberReportGenerator (type, new InvolvedType (type), new IdentifierGenerator<Type> (), new IdentifierGenerator<MemberInfo> (), outputFormatter);
    }

    public static InvolvedTypeReportGenerator CreateInvolvedTypeReportGenerator (IRemotionReflector remotionReflector, IOutputFormatter outputFormatter, params InvolvedType[] involvedTypes)
    {
      var assemblyIdentifierGenerator = StubFactory.CreateIdentifierGeneratorStub (new Assembly[0]);
      var involvedTypeIdentifierGenerator = StubFactory.CreateIdentifierGeneratorStub (involvedTypes.Select (t => t.Type));

      return new InvolvedTypeReportGenerator (involvedTypes,
                                              assemblyIdentifierGenerator,
                                              involvedTypeIdentifierGenerator,
                                              new IdentifierGenerator<MemberInfo> (),
                                              new IdentifierGenerator<Type> (),
                                              new IdentifierGenerator<Type> (),
                                              remotionReflector,
                                              outputFormatter);
    }
  }
}
