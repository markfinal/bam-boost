#region License
// Copyright (c) 2010-2019, Mark Final
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
        GenericBoostModule,
        C.IPublicHeaders
    {
        public DateTime() :
            base("date_time")
        {}

        Bam.Core.TokenizedString C.IPublicHeaders.SourceRootDirectory { get; } = null;

        Bam.Core.StringArray C.IPublicHeaders.PublicHeaderPaths { get; } = new Bam.Core.StringArray(
            "boost/date_time.hpp",
            "boost/date_time/**"
        );

        protected override void
        Init()
        {
            base.Init();

            this.BoostSource.AddFiles("$(packagedir)/libs/date_time/src/gregorian/*.cpp");

            this.CompileAgainstPublicly<Exception>(this.BoostSource);
            this.CompileAgainstPublicly<SmartPtr>(this.BoostSource);
            this.CompileAgainstPublicly<Mpl>(this.BoostSource);
            this.CompileAgainstPublicly<Numeric>(this.BoostSource);

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                this.CompileAgainstPublicly<WinAPI>(this.BoostSource);
            }

            /*
            this.PublicPatch((settings, appliedTo) =>
                {
                    if (settings is ClangCommon.ICommonCompilerSettings)
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("c++11-long-long"); // boost_1_60_0/boost/functional/hash/hash.hpp:241:32: error: 'long long' is a C++11 extension
                    }
                });
                */

            /*
            this.BoostSource.PrivatePatch(settings =>
                {
                    if (settings is GccCommon.ICommonCompilerSettings)
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        if (this.BoostSource.Compiler.Version.AtLeast(GccCommon.ToolchainVersion.GCC_5))
                        {
                            compiler.DisableWarnings.AddUnique("deprecated-declarations"); // boost_1_60_0/boost/date_time/gregorian/greg_facet.hpp:293:12: error: 'template<class> class std::auto_ptr' is deprecated [-Werror=deprecated-declarations]
                        }
                        if (this.BoostSource.Compiler.Version.AtLeast(GccCommon.ToolchainVersion.GCC_8))
                        {
                            compiler.DisableWarnings.AddUnique("parentheses"); // boost_1_67_0/boost/mpl/assert.hpp:188:21: error: unnecessary parentheses in declaration of 'assert_arg' [-Werror=parentheses]
                        }
                    }
                });
                */
        }
    }

    namespace tests
    {
        class Testgregorian_calendar :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/date_time/test/testgregorian_calendar.cpp");
                /*
                this.CompileAndLinkAgainst<DateTime>(this.TestSource);
                */
            }
        }
    }
}
