using Bam.Core;
namespace boost
{
    [ModuleGroup("Thirdparty/Boost")]
    sealed class System :
        GenericBoostModule
    {
        public System() :
            base("system")
        {}

        protected override void
        Init(
            Module parent)
        {
            base.Init(parent);

            this.BoostSource.AddFiles("$(packagedir)/libs/system/src/*.cpp");
        }
    }
}
