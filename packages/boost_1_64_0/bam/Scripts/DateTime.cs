#region License
// Copyright (c) 2010-2017, Mark Final
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
    class DateTime :
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

            this.PublicPatch((settings, appliedTo) =>
                {
                    var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                    if (null != clangCompiler)
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("c++11-long-long"); // boost_1_60_0/boost/functional/hash/hash.hpp:241:32: error: 'long long' is a C++11 extension
                    }
                });

            this.BoostSource.PrivatePatch(settings =>
                {
                    var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                    if (null != gccCompiler)
                    {
                        if (this.BoostSource.Compiler.IsAtLeast(5,4))
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.DisableWarnings.AddUnique("deprecated-declarations"); // boost_1_60_0/boost/date_time/gregorian/greg_facet.hpp:293:12: error: 'template<class> class std::auto_ptr' is deprecated [-Werror=deprecated-declarations]
                        }
                    }
                });
        }
    }

    namespace tests
    {
        class testgregorian_calendar :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/date_time/test/testgregorian_calendar.cpp");
                this.CompileAndLinkAgainst<DateTime>(this.TestSource);
            }
        }

        [Bam.Core.ModuleGroup("Thirdparty/Boost/tests")]
        sealed class DateTimeTests :
            Publisher.Collation
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                var anchor = this.Include<testgregorian_calendar>(C.Cxx.ConsoleApplication.Key, EPublishingType.ConsoleApplication);
                this.Include<DateTime>(C.Cxx.DynamicLibrary.Key, ".", anchor);
                this.Include<Chrono>(C.Cxx.DynamicLibrary.Key, ".", anchor);
                this.Include<System>(C.Cxx.DynamicLibrary.Key, ".", anchor);
            }
        }
    }
}