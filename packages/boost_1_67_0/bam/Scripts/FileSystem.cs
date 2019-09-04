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
    class FileSystem :
        GenericBoostModule
    {
        public FileSystem() :
            base("filesystem")
        {}

        protected override void
        Init()
        {
            base.Init();

            this.CreateHeaderCollection("$(packagedir)/libs/filesystem/src/*.hpp");
            this.BoostSource.AddFiles("$(packagedir)/libs/filesystem/src/*.cpp");

            this.BoostSource.PrivatePatch(settings =>
                {
                    if (settings is GccCommon.ICommonCompilerSettings gccCompiler)
                    {
                        gccCompiler.ExtraWarnings = false;
                        if (this.BoostSource.Compiler.Version.AtLeast(GccCommon.ToolchainVersion.GCC_5))
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.DisableWarnings.AddUnique("deprecated-declarations"); // boost_1_67_0/libs/filesystem/src/operations.cpp:2094:47: error: 'int readdir_r(DIR*, dirent*, dirent**)' is deprecated [-Werror=deprecated-declarations]
                        }
                    }

                    if (settings is ClangCommon.ICommonCompilerSettings clangCompiler)
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("unused-parameter"); // boost_1_60_0/boost/atomic/detail/ops_gcc_x86_dcas.hpp:525:113: error: unused parameter 'order'
                    }

                    if (settings is VisualCCommon.ICommonCompilerSettings vcCompiler)
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("4267"); // boost_1_60_0\libs\filesystem\src\unique_path.cpp(112) : warning C4267: 'argument' : conversion from 'size_t' to 'DWORD', possible loss of data
                        compiler.DisableWarnings.AddUnique("4244"); // boost_1_60_0\libs\filesystem\src\windows_file_codecvt.cpp(43) : warning C4244: 'argument' : conversion from '__int64' to 'int', possible loss of data
                    }
                });

            if (this is C.Cxx.DynamicLibrary)
            {
                this.LinkAgainst<System>();
            }
        }
    }

    namespace tests
    {
        class Locale_info :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/filesystem/test/locale_info.cpp");
                this.CompileAndLinkAgainst<FileSystem>(this.TestSource);
            }
        }
    }
}
