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

        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.Macros["OutputName"] = TokenizedString.CreateVerbatim(string.Format("boost_{0}-vc120-mt-1_60", this.Name));
            this.Macros["libprefix"] = TokenizedString.CreateVerbatim("lib");

            this.BoostSource = this.CreateCxxSourceContainer();
            this.CompileAgainstPublicly<BoostHeaders>(this.BoostSource);
            if (this.BuildEnvironment.Platform.Includes(EPlatform.Windows))
            {
                this.BoostSource.PrivatePatch(settings =>
                    {
                        var cxxCompiler = settings as C.ICxxOnlyCompilerSettings;
                        cxxCompiler.ExceptionHandler = C.Cxx.EExceptionHandler.Asynchronous;
                    });

                if (this.Librarian is VisualCCommon.Librarian)
                {
                    this.CompileAgainst<WindowsSDK.WindowsSDK>(this.BoostSource);
                }
            }
        }
    }
}
