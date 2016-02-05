using Bam.Core;
namespace boost
{
    [ModuleGroup("Thirdparty/Boost")]
    sealed class DateTime :
        GenericBoostModule
    {
        public DateTime() :
            base("date_time")
        {}

        protected override void
        Init(
            Module parent)
        {
            base.Init(parent);

            this.BoostSource.AddFiles("$(packagedir)/libs/date_time/src/gregorian/*.cpp");
        }
    }
}
