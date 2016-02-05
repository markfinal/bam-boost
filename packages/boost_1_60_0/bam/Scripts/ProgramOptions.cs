using Bam.Core;
namespace boost
{
    [ModuleGroup("Thirdparty/Boost")]
    sealed class ProgramOptions :
        GenericBoostModule
    {
        public ProgramOptions() :
            base("program_options")
        {}

        protected override void
        Init(
            Module parent)
        {
            base.Init(parent);

            this.BoostSource.AddFiles("$(packagedir)/libs/program_options/src/*.cpp");
        }
    }
}
