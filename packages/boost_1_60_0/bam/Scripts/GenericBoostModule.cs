using Bam.Core;
namespace boost
{
    abstract class GenericBoostModule :
        C.StaticLibrary
    {
        protected GenericBoostModule(
            string name)
        {
            this.Name = name;
        }

        private string Name
        {
            get;
            set;
        }

        protected C.Cxx.ObjectFileCollection BoostSource
        {
            get;
            private set;
        }

        protected C.HeaderFileCollection BoostHeaders
        {
            get;
            private set;
        }

        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.Macros["OutputName"] = TokenizedString.CreateVerbatim(string.Format("boost_{0}-vc120-mt-1_60", this.Name));
            this.Macros["libprefix"] = TokenizedString.CreateVerbatim("lib");

            this.BoostHeaders = this.CreateHeaderContainer(string.Format("$(packagedir)/boost/{0}/**.hpp", this.Name));

            this.BoostSource = this.CreateCxxSourceContainer();

            this.PublicPatch((settings, appliedTo) =>
                {
                    var compiler = settings as C.ICommonCompilerSettings;
                    if (null != compiler)
                    {
                        compiler.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)"));
                    }
                });

            if (this.BuildEnvironment.Platform.Includes(EPlatform.Windows))
            {
                this.BoostSource.PrivatePatch(settings =>
                    {
                        var cxxCompiler = settings as C.ICxxOnlyCompilerSettings;
                        cxxCompiler.ExceptionHandler = C.Cxx.EExceptionHandler.Asynchronous;

                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level2; // does not compile warning-free above this level
                        }
                    });

                if (this.Librarian is VisualCCommon.Librarian)
                {
                    this.CompileAgainst<WindowsSDK.WindowsSDK>(this.BoostSource);
                }
            }
        }
    }
}
