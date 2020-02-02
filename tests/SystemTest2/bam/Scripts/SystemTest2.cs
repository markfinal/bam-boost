namespace SystemTest2
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

    sealed class UserConfiguration :
        Bam.Core.IOverrideModuleConfiguration
    {
        void
        Bam.Core.IOverrideModuleConfiguration.execute(
            Bam.Core.IModuleConfiguration config,
            Bam.Core.Environment buildEnvironment)
        {
            if (config is boost.ConfigureBoost boostConfig)
            {
                boostConfig.EnableAutoLinking = false;
            }
        }
    }

    class SystemTest2 :
        C.Cxx.ConsoleApplication
    {
        protected override void
        Init()
        {
            base.Init();

            var source = this.CreateCxxSourceCollection("$(packagedir)/source/*.cpp");
            source.CompileAgainstSDK<boost.SDK>();
            var boostLibraryTypesToLink = this.LinkAgainstSDK<boost.SDK>();
            boostLibraryTypesToLink.Clear();
            boostLibraryTypesToLink.Add(typeof(boost.System));

            source.PrivatePatch(settings =>
            {
                if (settings is C.ICxxOnlyCompilerSettings cxxCompiler)
                {
                    cxxCompiler.ExceptionHandler = C.Cxx.EExceptionHandler.Asynchronous;

                    // TODO: should this be part of the Boost SDK Module?
                    cxxCompiler.LanguageStandard = C.Cxx.ELanguageStandard.Cxx11;
                }

                if (settings is C.ICommonCompilerSettings compiler)
                {
                    compiler.WarningsAsErrors = true;
                    if (settings is VisualCCommon.ICommonCompilerSettings vcCompiler)
                    {
                        vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                    }
                    if (settings is GccCommon.ICommonCompilerSettings gccCompiler)
                    {
                        gccCompiler.AllWarnings = true;
                        gccCompiler.ExtraWarnings = true;
                        gccCompiler.Pedantic = true;

                        // TODO: should this be part of the Boost SDK Module?
                        if (source.Compiler.Version.AtLeast(GccCommon.ToolchainVersion.GCC_8))
                        {
                            compiler.DisableWarnings.AddUnique("parentheses");
                        }
                    }
                    if (settings is ClangCommon.ICommonCompilerSettings clangCompiler)
                    {
                        clangCompiler.AllWarnings = true;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;

                        // TODO: should this be part of the Boost SDK Module?
                        if (source.BitDepth == C.EBit.ThirtyTwo)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter");
                        }
                    }
                }
            });

            this.PrivatePatch(settings =>
            {
                if (settings is C.ICommonLinkerSettingsLinux linuxLinker)
                {
                    linuxLinker.CanUseOrigin = true;
                    linuxLinker.RPath.AddUnique("$ORIGIN");
                }
            });
        }
    }

    sealed class SystemTestRuntime :
        Publisher.Collation
    {
        protected override void
        Init()
        {
            base.Init();

            this.SetDefaultMacrosAndMappings(EPublishingType.ConsoleApplication);
            this.Include<SystemTest2>(C.Cxx.ConsoleApplication.ExecutableKey);
        }
    }
}
