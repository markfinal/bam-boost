using Bam.Core;
namespace boost
{
    [ModuleGroup("Thirdparty/Boost")]
    sealed class Thread :
        GenericBoostModule
    {
        public Thread() :
            base("thread")
        {}

        protected override void
        Init(
            Module parent)
        {
            base.Init(parent);

            this.BoostSource.AddFiles("$(packagedir)/libs/thread/src/*.cpp");

            if (this.BuildEnvironment.Platform.Includes(EPlatform.Windows))
            {
                this.BoostSource.AddFiles("$(packagedir)/libs/thread/src/win32/*.cpp");
                this.BoostSource.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.PreprocessorDefines.Add("BOOST_THREAD_BUILD_LIB");
                    });
            }
        }
    }
}
