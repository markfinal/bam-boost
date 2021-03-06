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
    [Bam.Core.ModuleGroup("Thirdparty/Boost/tests")]
    abstract class GenericBoostTest :
        C.Cxx.ConsoleApplication
    {
        protected C.Cxx.ObjectFileCollection TestSource
        {
            get;
            private set;
        }

        protected override void
        Init()
        {
            base.Init();

            this.TestSource = this.CreateCxxSourceCollection();
            this.TestSource.PrivatePatch(settings =>
                {
                    var cxxCompiler = settings as C.ICxxOnlyCompilerSettings;
                    cxxCompiler.LanguageStandard = C.Cxx.ELanguageStandard.Cxx11;
                    cxxCompiler.StandardLibrary = C.Cxx.EStandardLibrary.libcxx;
                    cxxCompiler.ExceptionHandler = C.Cxx.EExceptionHandler.Asynchronous;

                    if (settings is VisualCCommon.ICommonCompilerSettings vcCompiler)
                    {
                        vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level1;
                    }

                    if (settings is GccCommon.ICommonCompilerSettings gccCompiler)
                    {
                        gccCompiler.AllWarnings = false;
                        gccCompiler.ExtraWarnings = false;
                        gccCompiler.Pedantic = false;
                    }

                    if (settings is ClangCommon.ICommonCompilerSettings clangCompiler)
                    {
                        clangCompiler.AllWarnings = false;
                        clangCompiler.ExtraWarnings = false;
                        clangCompiler.Pedantic = false;
                    }
                });

            this.PrivatePatch(settings =>
                {
                    if (settings is C.ICxxOnlyLinkerSettings cxxLinker)
                    {
                        cxxLinker.StandardLibrary = C.Cxx.EStandardLibrary.libcxx;
                    }
                    if (settings is GccCommon.ICommonLinkerSettings gccLinker)
                    {
                        gccLinker.CanUseOrigin = true;
                        gccLinker.RPath.AddUnique("$ORIGIN");
                    }
                });
        }
    }
}
