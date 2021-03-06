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
    class Atomic :
        GenericBoostModule
    {
        public Atomic() :
            base("atomic")
        {}

        protected override void
        Init()
        {
            base.Init();

            this.BoostSource.AddFiles("$(packagedir)/libs/atomic/src/*.cpp");

            this.BoostSource.PrivatePatch(settings =>
                {
                    var preprocessor = settings as C.ICommonPreprocessorSettings;
                    preprocessor.PreprocessorDefines.Add("BOOST_ATOMIC_SOURCE");

                    var compiler = settings as C.ICommonCompilerSettings;
                    if (settings is VisualCCommon.ICommonCompilerSettings)
                    {
                        compiler.DisableWarnings.AddUnique("4324"); // boost_1_60_0\libs\atomic\src\lockpool.cpp(69): warning C4324: 'boost::atomics::detail::`anonymous-namespace'::padded_lock<0>': structure was padded due to alignment specifier
                    }
                    if (settings is ClangCommon.ICommonCompilerSettings)
                    {
                        compiler.DisableWarnings.AddUnique("unused-parameter"); // boost_1_60_0/boost/atomic/detail/ops_gcc_x86_dcas.hpp:525:113: error: unused parameter 'order' [-Werror,-Wunused-parameter]
                    }
                });

            this.PublicPatch((settings, appliedTo) =>
                {
                    if (settings is C.ICommonPreprocessorSettings preprocessor)
                    {
                        preprocessor.PreprocessorDefines.Add("BOOST_ATOMIC_DYN_LINK");
                    }
                });
        }
    }

    namespace tests
    {
        class Lockfree :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/atomic/test/lockfree.cpp");
                this.CompileAndLinkAgainst<Atomic>(this.TestSource);
            }
        }
    }
}
