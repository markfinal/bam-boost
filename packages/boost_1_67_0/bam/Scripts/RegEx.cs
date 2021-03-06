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
    class RegEx :
        GenericBoostModule
    {
        public RegEx() :
            base("regex")
        {}

        protected override void
        Init()
        {
            base.Init();

            this.BoostSource.AddFiles("$(packagedir)/libs/regex/src/*.cpp");

            this.BoostSource.PrivatePatch(settings =>
                {
                    if (settings is ClangCommon.ICommonCompilerSettings clangCompiler)
                    {
                        clangCompiler.Visibility = ClangCommon.EVisibility.Default; // TODO: don't know why, but templated do_assign functions were missing at link without this
                    }
                    if (settings is VisualCCommon.ICommonCompilerSettings)
                    {
                        if (this.BoostSource.Compiler.Version.AtLeast(VisualCCommon.ToolchainVersion.VC2019_16_0))
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.DisableWarnings.AddUnique("4244"); // MSVC\14.22.27905\include\xstring(2348): warning C4244: 'argument': conversion from '_Ty' to 'const _Elem', possible loss of data
                        }
                    }
                    if (settings is GccCommon.ICommonCompilerSettings)
                    {
                        if (this.BoostSource.Compiler.Version.AtLeast(GccCommon.ToolchainVersion.GCC_7))
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.DisableWarnings.AddUnique("implicit-fallthrough");
                        }
                    }
                });

            this.PublicPatch((settings, appliedTo) =>
                {
                    if (settings is ClangCommon.ICommonCompilerSettings)
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("c++11-long-long"); // boost_1_60_0/boost/functional/hash/hash.hpp:241:32: error: 'long long' is a C++11 extension
                        compiler.DisableWarnings.AddUnique("unknown-pragmas"); // boost_1_60_0/boost/regex/v4/instances.hpp:124:34: error: unknown warning group '-Wkeyword-macro', ignored
                    }
                });
        }
    }

    namespace tests
    {
        class Capturestest :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/regex/test/captures/*.cpp");
                this.CompileAndLinkAgainst<RegEx>(this.TestSource);

                this.TestSource.PrivatePatch(settings =>
                    {
                        var preprocessor = settings as C.ICommonPreprocessorSettings;
                        preprocessor.PreprocessorDefines.Add("BOOST_REGEX_MATCH_EXTRA", "1");
                    });
            }
        }
    }
}
