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
    class Wave :
        GenericBoostModule
    {
        public Wave() :
            base("wave")
        {}

        protected override void
        Init()
        {
            base.Init();

            this.BoostSource.AddFiles("$(packagedir)/libs/wave/src/*.cpp");
            this.BoostSource.AddFiles("$(packagedir)/libs/wave/src/cpplexer/re2clex/*.cpp");

            /*
            if (this is C.Cxx.DynamicLibrary)
            {
                this.LinkAgainst<Thread>();
                this.CompilePubliclyAndLinkAgainst<System>(this.BoostSource);
                this.CompilePubliclyAndLinkAgainst<DateTime>(this.BoostSource);
                this.LinkAgainst<Chrono>();
            }
            */

            // TODO: this is a hack
            // for some reason, symbols like expression_grammar_gen (which is templated) are not exported to the
            // shared object with the visibility attribute set
            this.BoostSource.PrivatePatch(settings =>
                {
                    if (settings is GccCommon.ICommonCompilerSettings gccCompiler)
                    {
                        gccCompiler.Visibility = GccCommon.EVisibility.Default;
                    }
                    if (settings is ClangCommon.ICommonCompilerSettings clangCompiler)
                    {
                        clangCompiler.Visibility = ClangCommon.EVisibility.Default;
                    }
                });

            this.BoostSource.PrivatePatch(settings =>
                {
                    if (settings is ClangCommon.ICommonCompilerSettings)
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("unused-parameter"); // boost_1_60_0/boost/wave/util/flex_string.hpp:292:16: error: unused parameter 'alloc'
                    }
                    if (settings is VisualCCommon.ICommonCompilerSettings)
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("4512"); // boost_1_60_0\boost/spirit/home/classic/core/scanner/scanner.hpp(198) : warning C4512: 'boost::spirit::classic::scanner_policies<iter_policy_t,boost::spirit::classic::match_policy,boost::spirit::classic::action_policy>' : assignment operator could not be generated
                        compiler.DisableWarnings.AddUnique("4709"); // boost_1_60_0\boost/spirit/home/classic/phoenix/primitives.hpp(59) : warning C4709: comma operator within array index expression
                    }
                });

            this.PublicPatch((settings, appliedTo) =>
                {
                    var compiler = settings as C.ICommonCompilerSettings;
                    if (settings is GccCommon.ICommonCompilerSettings)
                    {
                        compiler.DisableWarnings.AddUnique("unused-local-typedefs"); // boost_1_60_0/boost/spirit/home/classic/core/non_terminal/impl/grammar.ipp:286:68: error: typedef 'iterator_t' locally defined but not used
                        compiler.DisableWarnings.AddUnique("unused-parameter"); // boost_1_60_0/boost/wave/grammars/cpp_grammar.hpp:730:1: error: unused parameter 'act_pos'
                        compiler.DisableWarnings.AddUnique("missing-field-initializers"); // boost_1_60_0/boost/atomic/detail/bitwise_cast.hpp:39:14: error: missing initializer for member 'boost::atomics::detail::bitwise_cast(const From&) [with To = long unsigned int; From = void*]::<anonymous struct>::to'

                        if (this.BoostSource.Compiler.Version.AtLeast(GccCommon.ToolchainVersion.GCC_5))
                        {
                            compiler.DisableWarnings.AddUnique("deprecated-declarations"); // boost_1_60_0/boost/spirit/home/classic/core/non_terminal/impl/grammar.ipp:159:18: error: 'template<class> class std::auto_ptr' is deprecated [-Werror=deprecated-declarations]
                            if (0 != (this.BuildEnvironment.Configuration & Bam.Core.EConfiguration.NotDebug))
                            {
                                compiler.DisableWarnings.AddUnique("strict-overflow"); // boost_1_64_0/boost/wave/grammars/cpp_expression_value.hpp:189:43: error: assuming signed overflow does not occur when assuming that (X + c) < X is always false [-Werror=strict-overflow]
                            }
                        }
                        if (this.BoostSource.Compiler.Version.AtLeast(GccCommon.ToolchainVersion.GCC_7))
                        {
                            compiler.DisableWarnings.AddUnique("format-overflow");
                        }
                    }
                    if (settings is VisualCCommon.ICommonCompilerSettings)
                    {
                        compiler.DisableWarnings.AddUnique("4245"); // boost_1_60_0\boost/wave/grammars/cpp_expression_grammar.hpp(376) : warning C4245: 'argument' : conversion from 'boost::wave::token_category' to 'unsigned long', signed/unsigned mismatch
                        compiler.DisableWarnings.AddUnique("4100"); // boost_1_60_0\boost/wave/grammars/cpp_grammar.hpp(732) : warning C4100: 'act_pos' : unreferenced formal parameter
                        compiler.DisableWarnings.AddUnique("4702"); // boost_1_60_0\boost\wave\cpplexer\re2clex\cpp_re2c_lexer.hpp(327) : warning C4702: unreachable code
                        compiler.DisableWarnings.AddUnique("4706"); // boost_1_60_0\boost\wave\util\cpp_iterator.hpp(723) : warning C4706: assignment within conditional expression

                        // vc2015
                        compiler.DisableWarnings.AddUnique("4458"); // boost_1_60_0\boost/wave/util/flex_string.hpp(1805): warning C4458: declaration of 'pointer' hides class member
                        compiler.DisableWarnings.AddUnique("4459"); // boost_1_60_0\boost/spirit/home/classic/core/scanner/impl/skipper.ipp(101): warning C4459: declaration of 'iter_policy_t' hides global declaration
                        compiler.DisableWarnings.AddUnique("4477"); // boost_1_60_0\boost/wave/util/cpp_iterator.hpp(797): warning C4477: 'sprintf' : format string '%ld' requires an argument of type 'long', but variadic argument 1 has type 'size_t'
                    }
                    if (settings is ClangCommon.ICommonCompilerSettings)
                    {
                        if (this.BoostSource.Compiler.Version.AtLeast(ClangCommon.ToolchainVersion.Xcode_7))
                        {
                            compiler.DisableWarnings.AddUnique("unused-local-typedef"); // boost_1_60_0/boost/spirit/home/classic/core/non_terminal/impl/grammar.ipp:286:68: error: unused typedef 'iterator_t' [-Werror,-Wunused-local-typedef]
                        }
                    }
                });
        }
    }

    namespace tests
    {
        class Testwave :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/wave/test/testwave/testwave.cpp");
                this.TestSource.AddFiles("$(packagedir)/libs/wave/test/testwave/testwave_app.cpp");
                /*
                this.CompileAndLinkAgainst<Wave>(this.TestSource);
                this.CompileAndLinkAgainst<ProgramOptions>(this.TestSource);
                this.CompileAndLinkAgainst<FileSystem>(this.TestSource);
                this.CompileAndLinkAgainst<Thread>(this.TestSource);
                */

                this.PrivatePatch(settings =>
                    {
                        if (settings is C.ICommonLinkerSettingsLinux)
                        {
                            var linker = settings as C.ICommonLinkerSettings;
                            linker.Libraries.AddUnique("-lpthread");
                        }
                    });
            }
        }
    }
}
