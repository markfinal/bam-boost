#region License
// Copyright (c) 2010-2018, Mark Final
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of BuildAMation nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion // License
using Bam.Core;
namespace boost
{
    class Thread :
        GenericBoostModule
    {
        public Thread() :
            base("thread")
        {}

        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.BoostSource.AddFiles("$(packagedir)/libs/thread/src/*.cpp");

            if (this.BuildEnvironment.Platform.Includes(EPlatform.Windows))
            {
                this.BoostSource.AddFiles("$(packagedir)/libs/thread/src/win32/*.cpp");
                this.BoostSource.PrivatePatch(settings =>
                    {
                        var preprocessor = settings as C.ICommonPreprocessorSettings;
                        preprocessor.PreprocessorDefines.Add("BOOST_THREAD_BUILD_DLL"); // not _LIB, as this is a dynamic library

                        if (settings is VisualCCommon.ICommonCompilerSettings)
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.DisableWarnings.AddUnique("4100"); // boost_1_60_0\libs\thread\src\win32\thread.cpp(654) : warning C4100: 'TolerableDelay' : unreferenced formal parameter
                        }
                    });
            }
            else
            {
                this.BoostSource.AddFiles("$(packagedir)/libs/thread/src/pthread/thread.cpp");
                this.BoostSource.AddFiles("$(packagedir)/libs/thread/src/pthread/once.cpp");

                this.BoostSource.PrivatePatch(settings =>
                    {
                        if (settings is GccCommon.ICommonCompilerSettings gccCompiler)
                        {
                            gccCompiler.ExtraWarnings = false;
                        }

                        if (settings is ClangCommon.ICommonCompilerSettings)
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // boost_1_60_0/boost/atomic/detail/ops_gcc_x86_dcas.hpp:525:113: error: unused parameter 'order'
                        }
                    });
            }

            if (this is C.Cxx.DynamicLibrary)
            {
                this.LinkPubliclyAgainst<DateTime>();
                this.LinkPubliclyAgainst<System>();
                this.LinkPubliclyAgainst<Chrono>();
            }
        }
    }

    namespace tests
    {
        class Test_scheduler :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/thread/test/test_scheduler.cpp");
                this.CompileAndLinkAgainst<Thread>(this.TestSource);
                if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
                {
                    this.LinkAgainst<Atomic>(); // TODO: but not used at runtime?
                }

                this.PrivatePatch(settings =>
                    {
                        if (settings is GccCommon.ICommonLinkerSettings)
                        {
                            var linker = settings as C.ICommonLinkerSettings;
                            linker.Libraries.AddUnique("-lpthread");
                        }
                    });
            }
        }
    }
}
