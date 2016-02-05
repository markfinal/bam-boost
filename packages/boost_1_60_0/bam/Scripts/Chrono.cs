using Bam.Core;
namespace boost
{
    [ModuleGroup("Thirdparty/Boost")]
    sealed class Chrono :
        GenericBoostModule
    {
        public Chrono() :
            base("chrono")
        {}

        protected override void
        Init(
            Module parent)
        {
            base.Init(parent);

            this.BoostSource.AddFiles("$(packagedir)/libs/chrono/src/*.cpp");
        }
    }
}
