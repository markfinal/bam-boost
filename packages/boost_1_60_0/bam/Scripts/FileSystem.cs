using Bam.Core;
namespace boost
{
    [ModuleGroup("Thirdparty/Boost")]
    sealed class FileSystem :
        GenericBoostModule
    {
        public FileSystem() :
            base("filesystem")
        {}

        protected override void
        Init(
            Module parent)
        {
            base.Init(parent);

            this.CreateHeaderContainer("$(packagedir)/libs/filesystem/src/*.hpp");
            this.BoostSource.AddFiles("$(packagedir)/libs/filesystem/src/*.cpp");
        }
    }
}
