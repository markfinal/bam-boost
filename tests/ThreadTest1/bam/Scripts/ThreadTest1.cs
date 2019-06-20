namespace ThreadTest1
{
    public sealed class ConfigureOSX :
        Bam.Core.IPackageMetaDataConfigure<Clang.MetaData>
    {
        void
        Bam.Core.IPackageMetaDataConfigure<Clang.MetaData>.Configure(
            Clang.MetaData instance)
        {
            instance.MacOSXMinimumVersionSupported = "10.9";
        }
    }

    class ThreadTest1 :
        C.Cxx.ConsoleApplication
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            var source = this.CreateCxxSourceContainer("$(packagedir)/source/*.cpp");
            this.CompileAndLinkAgainst<boost.Thread>(source);

            source.PrivatePatch(settings =>
            {
                var cxxCompiler = settings as C.ICxxOnlyCompilerSettings;
                cxxCompiler.ExceptionHandler = C.Cxx.EExceptionHandler.Asynchronous;
            });

            this.PrivatePatch(settings =>
            {
                if (settings is GccCommon.ICommonLinkerSettings gccLinker)
                {
                    gccLinker.CanUseOrigin = true;
                    gccLinker.RPath.AddUnique("$ORIGIN");
                    var linker = settings as C.ICommonLinkerSettings;
                    linker.Libraries.AddUnique("-lpthread");
                }
            });
        }
    }

    sealed class ThreadTestRuntime :
        Publisher.Collation
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.SetDefaultMacrosAndMappings(EPublishingType.ConsoleApplication);
            this.Include<ThreadTest1>(C.Cxx.ConsoleApplication.ExecutableKey);
        }
    }
}
