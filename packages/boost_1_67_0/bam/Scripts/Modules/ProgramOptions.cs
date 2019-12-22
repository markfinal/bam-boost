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
namespace boost
{
    class ProgramOptions :
        GenericBoostModule,
        C.IPublicHeaders
    {
        public ProgramOptions() :
            base("program_options")
        {}

        Bam.Core.TokenizedString C.IPublicHeaders.SourceRootDirectory { get; } = null;

        Bam.Core.StringArray C.IPublicHeaders.PublicHeaderPaths { get; } = new Bam.Core.StringArray(
            "boost/program_options.hpp",
            "boost/program_options/**"
        );

        protected override void
        Init()
        {
            base.Init();

            this.BoostSource.AddFiles("$(packagedir)/libs/program_options/src/*.cpp");

            this.CompileAgainstPublicly<Config>(this.BoostSource);
            this.CompileAgainstPublicly<Any>(this.BoostSource);
            this.CompileAgainstPublicly<Exception>(this.BoostSource);
            this.CompileAgainstPublicly<Bind>(this.BoostSource);
            this.CompileAgainstPublicly<LexicalCast>(this.BoostSource);
            this.CompileAgainstPublicly<SmartPtr>(this.BoostSource);

            /*
            this.BoostSource.PrivatePatch(settings =>
                {
                    if (settings is VisualCCommon.ICommonCompilerSettings)
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("4458"); // boost_1_60_0\libs\program_options\src\cmdline.cpp(104): warning C4458: declaration of 'args' hides class member
                        compiler.DisableWarnings.AddUnique("4456"); // boost_1_60_0\libs\program_options\src\variables_map.cpp(71): warning C4456: declaration of 'original_token' hides previous local declaration
                    }
                });
                */
        }
    }

    namespace tests
    {
        class Parsers_test :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/program_options/test/parsers_test.cpp");
                /*
                this.CompileAndLinkAgainst<ProgramOptions>(this.TestSource);
                */
            }
        }
    }
}
