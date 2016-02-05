using Bam.Core;
namespace boost
{
    [ModuleGroup("Thirdparty/Boost")]
    sealed class BoostHeaders :
        C.HeaderLibrary
    {
        protected override void
        Init(
            Module parent)
        {
            base.Init(parent);

            var headers = this.CreateHeaderContainer();

            headers.AddFiles("$(packagedir)/boost/chrono/*.hpp");
            headers.AddFiles("$(packagedir)/boost/chrono/detail/*.hpp");
            headers.AddFiles("$(packagedir)/boost/chrono/io/*.hpp");
            headers.AddFiles("$(packagedir)/boost/chrono/io_v1/*.hpp");

            headers.AddFiles("$(packagedir)/boost/date_time/*.hpp");

            headers.AddFiles("$(packagedir)/boost/filesystem/*.hpp");
            headers.AddFiles("$(packagedir)/boost/filesystem/detail/*.hpp");

            headers.AddFiles("$(packagedir)/boost/program_options/*.hpp");
            headers.AddFiles("$(packagedir)/boost/program_options/detail/*.hpp");

            headers.AddFiles("$(packagedir)/boost/system/*.hpp");
            headers.AddFiles("$(packagedir)/boost/system/detail/*.hpp");

            headers.AddFiles("$(packagedir)/boost/thread/*.hpp");

            // TODO more

            this.PublicPatch((settings, appliedTo) =>
                {
                    var compiler = settings as C.ICommonCompilerSettings;
                    if (null != compiler)
                    {
                        compiler.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)"));
                    }
                });
        }
    }
}
